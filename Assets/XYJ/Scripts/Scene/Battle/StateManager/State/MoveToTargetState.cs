using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class MoveToTargetState : IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            obj.battle.m_aniMgr.PlayStateAni();
        } 
    }
}
