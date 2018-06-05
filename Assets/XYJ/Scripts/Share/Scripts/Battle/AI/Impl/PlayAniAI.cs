using Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;

namespace xys.battle
{
    /// <summary>
    /// 出场动作，进入战斗动作
    /// </summary>
    class PlayAniAI : ISimpleAI
    {
        public class Info
        {
            public bool bati;
            public float time;
            public bool backToIdleAI;
        }

        float m_timeFinish;
        Info m_info;
        public override void OnEnter(object para)
        {
            m_info = para as Info;
            m_timeFinish = BattleHelp.timePass + m_info.time;
            m_obj.battle.m_stateMgr.ChangeState(StateType.PlayAni,null, m_info.time);
            if (m_info.bati)
                m_obj.battle.m_buffMgr.AddFlag(BuffManager.Flag.Bati);
        }
        public override void OnExit()
        {
            if (m_info.bati)
                m_obj.battle.m_buffMgr.RemoveFlag(BuffManager.Flag.Bati);
            m_info = null;
        }

        public override void OnUpdate()
        {
            //不是霸体的时候可以被中断
            if(BattleHelp.timePass > m_timeFinish || m_obj.battle.m_stateMgr.m_curStType != StateType.PlayAni)
            {
                if (m_info.backToIdleAI)
                {
                    m_obj.battle.m_attrLogic.SetState(ObjectState.Idle);
                    m_obj.battle.m_ai.ChangeIdleAI();
                }

                else
                {
                    m_obj.battle.m_attrLogic.SetState(ObjectState.Battle);
                    m_obj.battle.m_ai.ChangeBattleAI();
                }
            }
        }
    }
}