using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
using GameServer;
using NetProto;

namespace xys.battle
{
    public partial class SkillManager : IBattleComponent, IBattleUpdate
    {
        //测试指定技能
        public static int s_testSkillId = 0;

        //能否施放技能
        public enum PlayResult
        {
            OK,
            Error_CD,
            Error_MP,
            Error_NoSkill,
            Error_Posture,
            Error,
        }

        public class SkillInfo
        {
            //下次可以施放的时间
            public float canPlayTime;
            public SkillConfig cfg;

            public bool IsCanPlayByCD()
            {
                return BattleHelp.timePass >= canPlayTime;
            }
        }

        //所有技能
        protected Dictionary<int, SkillInfo> m_allSkill = new Dictionary<int, SkillInfo>();
        protected SkillLogic m_skillLogic = new SkillLogic();
        //子弹逻辑
        List<BulletLogic> m_bullets = new List<BulletLogic>();

        //是否真正施放了
        bool m_isSetTruePlay;
        protected IObject m_obj;

        //在外部就把技能坐标和朝向设置好了
        public void PlaySkillImpl(SkillConfig cfg, IObject target, List<int> skillTargets )
        {
#if !COM_SERVER
            if (m_obj.battle.actor.mono.m_testSkill)
                XYJLogger.LogError("{0}施放技能{1} ", m_obj.charSceneId, cfg.id);
#endif
            if(cfg.type == SkillConfig.Type.QingGongSkill)
            {
                PlayJump(cfg);
                m_obj.battle.m_stateMgr.ChangeState(StateType.Jump);
                SetTruePlaySkill(cfg);
                Stop();
            }
            else
            {
                m_isSetTruePlay = false;
                m_obj.battle.m_stateMgr.ChangeState(StateType.Skill);
                m_skillLogic.Play(cfg, m_obj, target, skillTargets);
            }
        }


        protected virtual void PlayJump(SkillConfig cfg)
        {

        }

        public void PlaySkillImpl(int id, IObject target, List<int> skillTargets)
        {
            SkillInfo info = GetSkill(id, true);
            if (info == null)
                return;
            PlaySkillImpl(info.cfg, target, skillTargets);
        }

        //获取技能搜索目标
        public List<int> GetSkillSearchTargets(SkillConfig cfg, IObject target)
        {
            if (cfg.targetSearchAction != null)
                return cfg.targetSearchAction.GetSearchObjectid(m_obj, target);
            return null;
        }

        //设置真正使用了技能，这时候才可以进入CD
        public void SetTruePlaySkill(SkillConfig skill)
        {
            if (m_isSetTruePlay)
                return;
            m_isSetTruePlay = true;
            //设置cd 
            GetSkill(skill.id).canPlayTime = BattleHelp.timePass + skill.cd;

            OnSetTruePlaySkill(skill);

            if(BattleHelp.IsRunBattleLogic())
            {
                //设置战斗状态
                if (skill.type != SkillConfig.Type.QingGongSkill)
                    m_obj.battle.m_attrLogic.SetBattleState(true);

                //真气消耗
                m_obj.battle.m_attrLogic.Zhenqi_Add(-skill.costZhenqi);
            }

            //执行action
            if (skill.excuteActions.Count != 0)
            {
                ActionManager.HandleActionListAndSendMsg(m_skillLogic, m_obj, m_skillLogic.m_target, skill.excuteActions,m_obj.position,m_obj.rotateAngle);
            }

        }

        public SkillAniConfig.AniType GetAniType()
        {
            return m_skillLogic.GetAniType();
        }

        public virtual void Stop()
        {
            m_isSetTruePlay = false;
            m_skillLogic.Stop();
            StopSpecial();
        }

        public virtual void OnAwake(IObject obj)
        {
            m_obj = obj;
        }
        public virtual void OnStart()
        {

        }

        public virtual void OnEnterScene()
        {

        }
        public virtual void OnExitScene()
        {
            Stop();
        }

        public virtual void OnDestroy()
        {
            m_allSkill.Clear();
            m_skillLogic.Stop();
            m_obj = null;
        }

        public virtual void OnUpdate()
        {
            m_skillLogic.Update();
            UpdateSpecial();

            for (int i= m_bullets.Count - 1; i >= 0;i--)
            {
                //update下一帧才删除，让子弹
                if (m_bullets[i].IsFinish())
                    m_bullets.RemoveAt(i);
                else
                    m_bullets[i].Update(Time.deltaTime);
            }             
        }

        public void AddBullet(BulletLogic bullet)
        {
            m_bullets.Add(bullet);
        }

        protected virtual void OnSetTruePlaySkill(SkillConfig skill)
        {

        }

        protected SkillInfo AddSkill(int id)
        {
            if (m_allSkill.ContainsKey(id))
                return null;
            SkillConfig c = SkillConfig.Get(id);
            if (c == null)
                return null;
            SkillInfo info = new SkillInfo() { cfg = c };
            m_allSkill.Add(id, info);
            return info;
        }

        public SkillInfo GetSkill(int id, bool autoAdd = false)
        {
            SkillInfo p;
            if (m_allSkill.TryGetValue(id, out p))
                return p;
            else
            {
                if (autoAdd)
                    return AddSkill(id);
                else
                    return null;
            }
        }

        public bool IsPlaying()
        {
            return m_skillLogic.IsPlaying();
        }

        //能够移动的技能
        public bool IsSkillCanMove()
        {
            if (IsPlaying())
                return m_skillLogic.GetAniType() == SkillAniConfig.AniType.CastingContinueCanMove;
            else
                return false;
        }

        public SkillConfig GetCurSkill()
        {
            return m_skillLogic.GetCurSkill();
        }

        public SkillLogic GetSkillLogic()
        {
            return m_skillLogic;
        }

        //解控技能
        bool IsCanSkillByState(SkillInfo skill)
        {
            //施放条件
            if (skill.cfg.conditionCfg != null && skill.cfg.conditionCfg.effectTarget == EffectTarget.Self)
            {
                //状态不对
                if (skill.cfg.conditionCfg.isNeedCheckState && skill.cfg.conditionCfg.IsState(m_obj))
                    return true;
            }
            return false;
        }

        public PlayResult IsCanSkill(int id)
        {
            SkillInfo p;
            //不存在技能,当前版本直接添加
            if (!m_allSkill.TryGetValue(id, out p))
                p = AddSkill(id);

            //解控技能
            if (IsCanSkillByState(p))
                return PlayResult.OK;

            //有不能施放技能的flg
            if (m_obj.battle.m_buffMgr.IsFlag(BuffManager.Flag.NoSkill))
                return PlayResult.Error;

            if (!p.IsCanPlayByCD())
                return PlayResult.Error_CD;

            if (!p.cfg.IsCastingPostureLegal(m_obj.postureValue))
                return PlayResult.Error_Posture;

            return PlayResult.OK;
        }

        public float GetAniTimePass()
        {
            if (m_skillLogic.IsPlaying())
                return m_skillLogic.GetAniTimePass();
            else
                return 0;
        }
    }
}
