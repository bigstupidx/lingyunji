using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.battle
{
    /// <summary>
    /// 技能事件组的处理逻辑
    /// </summary>
    public class SkillEventGroup
    {
        //事件按顺序执行
        int m_curid;
        float m_timeBeginPlay;
        //动画放缩速度
        public float m_speedMul { get; private set; }
        //动画开始的帧数
        int m_aniBeginFrame;

        //技能动作事件,timePass为已经经过了的时间
        public void PlayEvent(int aniBeginFrame, float timePass)
        {
            m_curid = 0;
            m_timeBeginPlay = BattleHelp.timePass - timePass;
            m_aniBeginFrame = aniBeginFrame;
            m_speedMul = 1;
        }

        public void SetSpeed( float speed)
        {
            m_timeBeginPlay = BattleHelp.timePass;
            m_speedMul = speed;
        }

        //返回false表示结束
        public bool Update(SkillLogic skill,List<SkillEventConfig> eventCfgList )
        {
            //结束
            if (m_curid >= eventCfgList.Count)
                return false;
            float timePass = BattleHelp.timePass - m_timeBeginPlay;
            int curFrame;
            if(eventCfgList[m_curid].frameType == SkillEventConfig.FrameType.AniFrame)
                curFrame = (int)(timePass * AniConst.AnimationFrameRate * m_speedMul);
            else
                curFrame = (int)(timePass * AniConst.AnimationFrameRate);
            if (eventCfgList[m_curid].beginFrame - m_aniBeginFrame <= curFrame)
            {
                //执行action
                if (SkillManager.s_testSkillId == skill.m_cfg.id)
                    XYJLogger.LogError(string.Format("技能事件 event={0}" , eventCfgList[m_curid].key));


                ActionManager.HandleActionListAndSendMsg(skill, skill.m_source, skill.m_target, eventCfgList[m_curid].actionList, skill.m_source.position, skill.m_source.rotateAngle,false);

                m_curid++;
                if (m_curid >= eventCfgList.Count)
                    return false;
            }
            return true;
        }
    }
}
