using NetProto;
using xys.battle;
using System;
using System.Collections.Generic;
using UnityEngine;
using Config;


namespace xys.battle
{

    class PetAI : SimpleAttackAI
    {
        IObject m_master;
        int m_masterId;
        const float FOV = 5;
        const float BackToMasterDis = 10.0f;
        const float IdleFollowMasterDis = 3.0f;
        public override void OnEnter(object para)
        {
            base.OnEnter(para);
            Pet pet = m_obj as Pet;
            if (pet != null)
                m_masterId = pet.m_masterId;
            pet.battle.m_attrMgr.SetFieldOfView(FOV);
        }
        public override void OnExit()
        {
            base.OnExit();
            m_master = null;
        }

        public override void OnUpdate()
        {
            if (m_master == null)
            {
                m_master = App.my.sceneMgr.GetObj(m_masterId);
                if (m_master == null)
                    return;
            }

            
            switch (m_obj.battle.m_stateMgr.m_curStType)
            {
                case StateType.Idle:
                case StateType.Move:
                    float dis;
                    if(m_obj.battle.GetTarget() != null)
                        dis = BackToMasterDis;
                    else 
                        dis = IdleFollowMasterDis;
                    if (BattleHelp.GetDistance(m_master, m_obj) > dis)
                        MoveToMaster();
                    break;
            }

            base.OnUpdate();
        }

        void MoveToMaster()
        {
            m_obj.battle.State_MoveToTarget(m_master, 1.0f, (p) =>
            {
                //清理仇恨
                m_obj.battle.m_targetMgr.SetActive(false);
                m_obj.battle.m_targetMgr.SetActive(true);
                m_obj.battle.m_moveMgr.StopMove();
                //返回待机
                m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
            });
            return;
        }
    }
}
