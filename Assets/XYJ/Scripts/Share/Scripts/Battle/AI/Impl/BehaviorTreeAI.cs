using behaviac;
using Config;
using GameServer;
using System;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;

namespace xys.battle
{

    class BehaviorTreeAI : ISimpleAI
    {
        BT_RoleAgent m_agent;
        public override void OnCreate(IObject obj)
        {
            base.OnCreate(obj);
            m_agent = new BT_RoleAgent((ObjectBase)m_obj);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            m_agent.Destroy();
            m_agent = null;
        }

        public override void OnEnter (object para)
        {
            m_agent.LoadAI(para as string);
            m_agent.SetActive(true);
        }

        public override void OnExit()
        {
            m_agent.Destroy();
            m_agent = null;
        }




        public override void OnUpdate()
        {
            m_agent.Update();
        }

        public override void OnPause(bool pause)
        {
            m_agent.SetActive(!pause);
        }
    }
}
