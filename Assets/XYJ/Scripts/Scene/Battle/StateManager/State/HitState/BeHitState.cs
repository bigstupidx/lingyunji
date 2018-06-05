using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class BeHitState : IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            float timeLenght = obj.battle.m_stateMgr.stateTimeLeft;
            float speed = obj.battle.m_aniMgr.GetSpeedByLenght(AniConst.BeHit, timeLenght);
            obj.battle.m_aniMgr.PlayAni(AniConst.BeHit, speed, timeLenght);
        }
    }
}
