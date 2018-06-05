using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class JumpState : IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            obj.battle.m_moveMgr.StopMove();
        }

        public override void OnExit(IObject obj, StateType nextState)
        {
            obj.battle.m_jumpMgr.Stop();
        }
    }
}
