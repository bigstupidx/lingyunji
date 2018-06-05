using Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;

namespace xys.battle
{

    class MoveToBornPosAI : ISimpleAI
    {
        Vector3 m_bornPos;
        float m_angle;
        public override void OnEnter(object para)
        {
            object[] pList = para as object[];
            m_bornPos = (Vector3)pList[0];
            m_angle = (float)pList[1];

            m_obj.battle.m_buffMgr.AddFlag(BuffManager.Flag.Bati);
            m_obj.battle.m_targetMgr.SetActive(false);
            ChangeState();
        }

        public override void OnExit()
        {
            m_obj.battle.m_buffMgr.RemoveFlag(BuffManager.Flag.Bati);
            m_obj.battle.m_moveMgr.StopMove();
        }

        public override void OnUpdate()
        {
            switch (m_obj.battle.m_stateMgr.m_curStType)
            {
                case StateType.Idle:
                    //case StateType.Move:
                    ChangeState();
                     break;
                default: break;
            }
        }

        void ChangeState()
        {
            m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
            m_obj.battle.m_moveMgr.SetMoveToPos(m_bornPos, (p) => 
            {
                m_obj.battle.m_targetMgr.SetActive(true);
                m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
                m_obj.battle.m_ai.ChangeIdleAI();

                m_obj.SetRotate(m_angle);
#if COM_SERVER
                if (m_obj.m_refreshData != null && m_obj.m_refreshData.fullHp)
                    m_obj.battle.m_attrLogic.AddHp((int)(m_obj.maxHpValue - m_obj.hpValue));
                m_obj.battle.m_moveMgr.SetPosAddSend(m_obj.position,m_obj.rotateAngle);
#endif
            },null,m_obj.battle.speed*kvBattle.MoveToBornPosSpeedMul);
        }
    }
}
