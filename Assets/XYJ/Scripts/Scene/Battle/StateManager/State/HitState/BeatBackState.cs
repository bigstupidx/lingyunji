using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class BeatBackState : IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            float ani2Time = AniConst.BeatBackToIdleTime;
            float ani1Time = obj.battle.m_stateMgr.stateTimeLeft - ani2Time;
            float ani1Speed = obj.battle.m_aniMgr.GetSpeedByLenght(AniConst.BeakBack1, ani1Time);

            obj.battle.m_aniMgr.ClearQueued();
            obj.battle.m_aniMgr.PlayQueued(AniConst.BeakBack1, ani1Speed, ani1Time);
            obj.battle.m_aniMgr.PlayQueued(AniConst.BeakBack2);          
        }
    }
}
