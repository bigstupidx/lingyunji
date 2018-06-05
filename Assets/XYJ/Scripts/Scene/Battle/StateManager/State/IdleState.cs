using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class IdleState:IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            if (obj.battle.m_aniMgr.IsIdleStateNeedPlayAni())
                obj.battle.m_aniMgr.PlayStateAni();
        }
    }
}
