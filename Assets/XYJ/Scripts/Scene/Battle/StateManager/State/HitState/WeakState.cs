using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class WeakState : IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            float totalLenght = obj.battle.m_stateMgr.stateTimeLeft;
            float idleTime = totalLenght - obj.battle.m_aniMgr.GetAniLenght(AniConst.WeakToIdle);
            obj.battle.m_aniMgr.ClearQueued();
            
            //虚弱状态再进入虚弱,无需再次播放进入虚弱动作
            if (lastState != StateType.Weak)
            {
                obj.battle.m_aniMgr.PlayQueued(AniConst.Weak1);
                idleTime = idleTime - obj.battle.m_aniMgr.GetAniLenght(AniConst.Weak1);
            }

            obj.battle.m_aniMgr.PlayQueued(AniConst.Weak2, 1.0f, idleTime);        
            obj.battle.m_aniMgr.PlayQueued(AniConst.WeakToIdle);
        }

    }
}
