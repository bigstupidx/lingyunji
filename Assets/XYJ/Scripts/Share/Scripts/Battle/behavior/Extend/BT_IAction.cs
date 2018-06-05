using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using xys.battle;
using behaviac;
using GameServer;

namespace xys.AI
{
    public class BT_IAction
    {
        public BT_RoleAgent m_agent;

        public virtual bool OnStart(object[] para)
        {
            return true;
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnPause(bool pause)
        {

        }

        public virtual EBTStatus OnUpdate()
        {
            return EBTStatus.BT_SUCCESS;
        }
    }




    public class ActionTaskCustom : LeafTask
    {
        BT_IAction m_action;

        public static ActionTaskCustom Create(string funName )
        {
            BT_IAction action = BT_ActionCreate.Create(funName);
            if(action!=null)
            {
                ActionTaskCustom p = new ActionTaskCustom();
                p.m_action = action;
                return p;
            }
            return null;
        }

        //自动生成的函数只有在onenter时调用一次
        protected override bool onenter(Agent pAgent)
        {
            BT_RoleAgent myAgent = pAgent as BT_RoleAgent;
            m_action.m_agent = myAgent;

            Action pActionNode = (Action)(this.GetNode());
            myAgent.m_curAction = m_action;
            if (pActionNode.Execute(pAgent, EBTStatus.BT_SUCCESS) == EBTStatus.BT_FAILURE)
                return false;
            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
            BT_RoleAgent myAgent = pAgent as BT_RoleAgent;
            myAgent.m_curAction = null;
            m_action.OnExit();
            m_action.m_agent = null;
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            return m_action.OnUpdate();
        }
    }
}