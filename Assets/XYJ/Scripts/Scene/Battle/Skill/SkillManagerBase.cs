using UnityEngine;
using System.Collections;
using Config;
using xys.UI;

namespace xys.battle
{
    public class SkillManagerBase : SkillManager
    {
        protected IObject m_obj;
        //一次只能请求一个技能
        public bool m_requestSkill = false;
        public bool m_requestStopSkill = false;
        //请求技能超过一定时间没返回就取消
        public float m_requestSkillTime;

        //技能进度条
        ProgressBase m_skillProgress;
        public override void OnAwake(IObject obj)
        {
            base.OnAwake(obj);
            m_obj = obj;
            m_skillLogic.m_aniFun = Ani_Play;
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            m_obj = null;
        }

        public override void Stop()
        {
            base.Stop();
            m_requestStopSkill = false;
            m_obj.battle.OnFinishSkill();

            if (m_skillProgress != null)
                m_skillProgress.Stop();
            m_skillProgress = null;
        }

        //请求技能施放,如果单机模式则不需要请求服务器
        public bool RequestPlaySkill(int id, IObject target )
        {
            SkillConfig cfg = GetSkill(id, true).cfg;
            //设置朝向0表示朝向，-1表示不朝向
            if (cfg.lookAtTarget == 0 && target != null && target.isAlive)
                m_obj.SetRotate(BattleHelp.Vector2Angle(target.position, m_obj.position));
            //玩家没有目标时可以通过摇杆改变技能方向
            else if( BattleHelp.IsMe(m_obj) && App.my.input.IsMoving())
            {
                m_obj.SetRotate(BattleHelp.Vector2Angle(App.my.input.GetMoveWay()));
            }

            if (m_obj.battle.m_isAiByLocal)
            {
                return App.my.battleProtocol.Request_PlaySkill(m_obj,target, cfg);
            }
            else
            {
                PlaySkillImpl(cfg, target, m_obj.battle.m_skillMgr.GetSkillSearchTargets(cfg, target));
                return true;
            }
        }


        public virtual void SwitchSkillByCondition(SkillConditionConfig.Condition condition,object para=null)
        {
        }

        protected override void PlayJump(SkillConfig cfg)
        {
            m_obj.battle.m_jumpMgr.PlayJump(cfg);
        }

        float m_lastAniSpeed;
        //播放动作
        void Ani_Play(SkillAniGroup.AniInfo aniInfo )
        {
            int aniIndex = aniInfo.aniIndex;
            IObject source = m_obj;
            SkillLogic skill = m_skillLogic;
            SkillAniConfig aniCfg = skill.m_cfg.aniCfgList[aniIndex];

            float normalizedTime = 0;
            float endNormalizedTime = 1.0f;
            //如果动作的结束帧和下个动作的开始帧一致，为了避免不连贯，不要再播放动画
            bool playAnimation = true;

            //上个动作速度太快则下个子动作要重新播放，不然可能帧差别比较大
            if (aniIndex > 0
                && skill.m_cfg.aniCfgList[aniIndex].aniName == skill.m_cfg.aniCfgList[aniIndex - 1].aniName
                && skill.m_cfg.aniCfgList[aniIndex].beginFrame == skill.m_cfg.aniCfgList[aniIndex - 1].endFrame
                && !aniInfo.forcePlay
                )
            {
                int frameDif = Mathf.Abs(source.battle.m_aniMgr.GetCurTrueAniFrame() - skill.m_cfg.aniCfgList[aniIndex].beginFrame);
                //帧数不对，速度不对
                if (source.battle.m_aniMgr.IsAniFinish()
                    || (frameDif > 2)
                    || m_lastAniSpeed!= aniInfo.aniSpeedMul
                   )
                    playAnimation = true;
                else
                    playAnimation = false;
            }


            if (playAnimation)
            {
                SkillAniConfig p = skill.m_cfg.aniCfgList[aniIndex];
                float time = source.battle.m_aniMgr.GetAniLenght(aniCfg.aniName);
                endNormalizedTime = (float)p.endFrame / (time * AniConst.AnimationFrameRate);
                //动画开始的时间
                if (p.beginFrame != 0)
                {
                    normalizedTime = (float)p.beginFrame / (time * AniConst.AnimationFrameRate);
                }          
            }

            float anispeed = 1.0f * aniInfo.aniSpeedMul;
            m_lastAniSpeed = anispeed;
            //循环动画不放缩
            if (source.battle.m_aniMgr.IsAniLoop(aniCfg.aniName))
                anispeed = 1.0f;

            if (playAnimation)
                source.battle.m_aniMgr.PlayAni(aniCfg.aniName, anispeed, 0, normalizedTime, aniCfg.crossFade,null,null, endNormalizedTime);
            else
                source.battle.m_aniMgr.SetCurSpeed(anispeed);

            //只有自己才有技能进度条
            if(m_obj == App.my.localPlayer)
            {
                switch (aniCfg.type)
                {
                    case SkillAniConfig.AniType.Casting:
                    case SkillAniConfig.AniType.CastingContinueCanMove:
                    case SkillAniConfig.AniType.CastingContinue:
                        bool isRightToLeft = false;
                        if (aniCfg.type == SkillAniConfig.AniType.Casting)
                            isRightToLeft = true;
                        if (m_skillProgress != null)
                            m_skillProgress.Stop();
                        m_skillProgress = App.my.uiSystem.progressMgr.PlaySkillCasting(new ProgressData()
                        {
                            timeLenght = aniInfo.timeLenght,
                            timeBegin = aniInfo.timebegin,
                            rightToLeft = isRightToLeft,
                            skillid = skill.m_cfg.id,
                        });
                        break;

                    default: break;
                }
            }
        }
    }
}

