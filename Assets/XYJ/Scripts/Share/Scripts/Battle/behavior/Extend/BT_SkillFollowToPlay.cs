using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using xys.battle;
using behaviac;
using GameServer;
using Config;

namespace xys.AI
{
    public class BT_SkillFollowToPlay : BT_IAction
    {
        SkillConfig m_skillCfg;
        EBTStatus m_state;
        public override bool OnStart(object []para)
        {
            m_skillCfg = SkillConfig.Get((int)para[0]);

            if (m_skillCfg == null || m_agent.m_target == null)
            {
                m_state = EBTStatus.BT_FAILURE;
            }
            else
            {
                m_state = EBTStatus.BT_RUNNING;
                //距离够就直接施放
                if (m_skillCfg.range >= BattleHelp.GetAttackDistance(m_agent.m_obj.position, m_agent.m_target))
                    m_agent.m_obj.battle.m_skillMgr.RequestPlaySkill(m_skillCfg.id, m_agent.m_target);
                else
                    BeginMove();
            }
            return true;
        }
        public override void OnExit()
        {
            m_agent.m_obj.battle.m_moveMgr.StopMove();
        }

        void BeginMove()
        {
            m_agent.m_obj.battle.State_MoveToTarget(m_agent.m_target, m_skillCfg.range,
                (p)=> m_agent.m_obj.battle.m_skillMgr.RequestPlaySkill(m_skillCfg.id, m_agent.m_target));
        }

        public override void OnPause(bool pause)
        {
            if (pause)
            {
                //技能施放时被中断就算结束了
                if (m_agent.m_obj.battle.m_stateMgr.m_curStType == StateType.Skill)
                    m_state = EBTStatus.BT_SUCCESS;
            }
            else if(m_state == EBTStatus.BT_RUNNING)
                BeginMove();
        }


        public override EBTStatus OnUpdate()
        {
            if(m_state == EBTStatus.BT_RUNNING)
            {
                if (m_agent.m_obj.battle.m_stateMgr.m_curStType == StateType.Skill)
                {
                    if (m_agent.m_obj.battle.m_stateMgr.IsStateFinish())
                        return EBTStatus.BT_SUCCESS;
                }
                else if (m_agent.m_obj.battle.m_stateMgr.m_curStType != StateType.Move)
                {
                    return EBTStatus.BT_SUCCESS;
                }
            }
            return m_state;
        }
    }
}