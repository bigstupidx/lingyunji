using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.battle
{
    public class SkillAniGroup
    {

        public class AniInfo
        {
            public int aniIndex;
            public float aniSpeedMul;
            public bool forcePlay;
            public float timeLenght;
            public float timebegin;
        }

        SkillEventGroup m_curEventGroup = new SkillEventGroup();
        //动作结束时间
        public float m_aniFinishTime { get; private set; }
        //只有在第一帧可以设置结束时间
        bool m_canSetFinishTime;
        float m_aniBeginTime;
        //技能当前动作序号
        int m_curAniIndex;

        List<IActionUpdate> m_actionUpdates = new List<IActionUpdate>();


        //播放技能
        public void Play(SkillLogic skill)
        {
            m_curAniIndex = -1;
            if (m_actionUpdates.Count != 0)
                XYJLogger.LogError("actionUpdate在播放时必须为空");
            PlayNextAni(skill);
        }

        //部分action可以设置结束时间
        public void SetAniFinishTime(float finishTime)
        {
            if (m_canSetFinishTime)
                m_aniFinishTime = finishTime;
            else
                XYJLogger.LogError("只有事件开始帧等于动作开始帧才能修改动画时间");
        }

        public SkillEventGroup GetSkillEventGroup()
        {
            return m_curEventGroup;
        }

        //获得动作时间长度
        float GetPlayTimeLength(SkillLogic skill, int curid, float timeFrame)
        {
            List<SkillAniConfig> aniCfgList = skill.m_cfg.aniCfgList;
            if (aniCfgList == null || curid >= aniCfgList.Count)
                return 0;

            SkillAniConfig aniCfg = aniCfgList[curid];

            //实际动画时间
            float timeLenght = 0;

            //其动作播放时间由技能表所填写的施放时间决定
            if (aniCfg.type == SkillAniConfig.AniType.Casting)
                timeLenght = skill.m_cfg.castingTime;
            else if (aniCfg.type == SkillAniConfig.AniType.CastingContinue || aniCfg.type == SkillAniConfig.AniType.CastingContinueCanMove)
                timeLenght = skill.m_cfg.castingContinueTime;
            else
                timeLenght = timeFrame/aniCfg.aniSpeedMul;


            if (timeLenght == 0)
            {
                XYJLogger.LogError(string.Format("技能动作的时间为0 aniid={0} anitype={1}", aniCfg.key, aniCfg.type));
                timeLenght = 0.0001f;
            }
            return timeLenght;
        }

        public void Stop(SkillLogic skill)
        {
            List<SkillAniConfig> aniCfgList = skill.m_cfg.aniCfgList;
            //处理buffflg
            if (m_curAniIndex >= 0 && m_curAniIndex < aniCfgList.Count)
                HandleBuffFlg(skill.m_source, aniCfgList[m_curAniIndex], false);
            StopActionUpdate();
            m_curAniIndex = -1;

        }


        public void AddActionUpdate(IActionUpdate actionUpdate)
        {
            m_actionUpdates.Add(actionUpdate);
        }



        void StopActionUpdate()
        {
            foreach (var p in m_actionUpdates)
                p.Stop();
            m_actionUpdates.Clear();
        }

        public SkillAniConfig.AniType GetAniType(SkillLogic skill)
        {
            if (m_curAniIndex >= skill.m_cfg.aniCfgList.Count)
                return SkillAniConfig.AniType.Normal;
            SkillAniConfig aniCfg = skill.m_cfg.aniCfgList[m_curAniIndex];
            return aniCfg.type;
        }

        void HandleBuffFlg(IObject obj, SkillAniConfig cfg, bool isAdd)
        {
            switch (cfg.type)
            {
                //可以移动和技能
                case SkillAniConfig.AniType.CastingContinue:
                case SkillAniConfig.AniType.AttackAfter:
                    break;
                //施法动作能移动但不响应技能
                case SkillAniConfig.AniType.Casting:
                    obj.battle.m_buffMgr.ChangeFlag(BuffManager.Flag.NoSkill, isAdd);
                    break;
                case SkillAniConfig.AniType.AttackStay:
                    obj.battle.m_buffMgr.ChangeFlag(BuffManager.Flag.NoMove, isAdd);
                    break;
                case SkillAniConfig.AniType.CastingContinueCanMove:
                    obj.battle.m_buffMgr.ChangeFlag(BuffManager.Flag.NoSkill, isAdd);
                    break;
                default:
                    obj.battle.m_buffMgr.ChangeFlag(BuffManager.Flag.NoMove, isAdd);
                    obj.battle.m_buffMgr.ChangeFlag(BuffManager.Flag.NoSkill, isAdd);
                    break;
            }
        }

        //播放下个动作
        bool PlayNextAni(SkillLogic skill)
        {
            List<SkillAniConfig> aniCfgList = skill.m_cfg.aniCfgList;

            //处理buffflg
            if (m_curAniIndex >= 0)
                HandleBuffFlg(skill.m_source, aniCfgList[m_curAniIndex], false);
            //移除事件更新
            m_curAniIndex++;
            if (m_curAniIndex >= aniCfgList.Count)
                return false;

            HandleBuffFlg(skill.m_source, aniCfgList[m_curAniIndex], true);

            //清理actionUpdate
            StopActionUpdate();

            SkillAniConfig aniCfg = aniCfgList[m_curAniIndex];
            //设置技能真正释放了
            if (aniCfg.type != SkillAniConfig.AniType.Casting)
            {
                skill.m_source.battle.m_skillMgr.SetTruePlaySkill(skill.m_cfg);
            }


            //时间超出，减少误差
            float timePass = (m_curAniIndex == 0 ? 0 : BattleHelp.timePass - m_aniFinishTime);
            //根据上个被中断技能情况对speed修改

            m_aniBeginTime = BattleHelp.timePass - timePass;

            //帧的动画时间,没有放缩
            float timeFrame = (float)(aniCfg.endFrame - aniCfg.beginFrame) /AniConst.AnimationFrameRate;
            //设置动作时间长度，动作长度改变，事件持续时间也应该改变
            float aniTimeLenght = GetPlayTimeLength(skill, m_curAniIndex, timeFrame);
            float temFinishTime = m_aniFinishTime = m_aniBeginTime + aniTimeLenght;

            m_canSetFinishTime = true;
            //开始event
            m_curEventGroup.PlayEvent(aniCfg.beginFrame, timePass);
            //先update一下，只有在开始帧修改动作时长的事件才会影响到动作播放速度
            m_curEventGroup.Update(skill, aniCfgList[m_curAniIndex].eventCfgList);
            m_canSetFinishTime = false;

            //时间修改了结束时间
            if (temFinishTime != m_aniFinishTime)
            {
#if !COM_SERVER
                //客户端如果变速了，让客户端多一点时间，不然动作结束了移动还没结束，导致一些特效位置对不准
                if (m_aniFinishTime > temFinishTime)
                    m_aniFinishTime += 0.1f;
#endif
                aniTimeLenght = m_aniFinishTime - m_aniBeginTime;
            }

            //动作速度
            float aniSpeedMul;
            if (m_aniFinishTime > m_aniBeginTime)
                aniSpeedMul = timeFrame / aniTimeLenght;
            //如果瞬间完成的则动作速度设置一个较大的值
            else
                aniSpeedMul = 100.0f;

            m_curEventGroup.SetSpeed(aniSpeedMul);
            //播放动画
            if (skill.m_aniFun != null)
            {
                AniInfo info = new AniInfo() { aniIndex = m_curAniIndex, aniSpeedMul = aniSpeedMul, forcePlay = false, timeLenght = aniTimeLenght, timebegin = timePass };
                skill.m_aniFun(info);
            }

            return true;
        }

        //返回false表示结束了
        public bool Update(SkillLogic skill)
        {
            List<SkillAniConfig> aniCfgList = skill.m_cfg.aniCfgList;
            if (m_curAniIndex >= aniCfgList.Count)
            {
                XYJLogger.LogError(string.Format("技能动作已经结束，不应该走到这里 skillid={0} aniIndex={1}",skill.m_cfg.id, m_curAniIndex));
                return false;
            }

            foreach (var p in m_actionUpdates)
            {
                //设置时间表示结束，不然计算动画时长timeFix会有问题
                if (p.Update())
                    m_aniFinishTime = BattleHelp.timePass;
            }


            m_curEventGroup.Update(skill, aniCfgList[m_curAniIndex].eventCfgList);

            //下一段动画
            if (m_aniFinishTime <= BattleHelp.timePass)
                return PlayNextAni(skill);

            return true;
        }

        public float GetAniTimePass()
        {
            return BattleHelp.timePass - m_aniBeginTime;
        }
    }

}
