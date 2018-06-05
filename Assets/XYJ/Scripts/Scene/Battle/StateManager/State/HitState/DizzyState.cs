using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class DizzyState : IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            float totalLenght = obj.battle.m_stateMgr.stateTimeLeft;
            float idleTime = totalLenght - obj.battle.m_aniMgr.GetAniLenght(AniConst.DizzyToIdel);
            obj.battle.m_aniMgr.ClearQueued();

            //眩晕状态再进入眩晕,无需再次播放进入眩晕动作
            if (lastState != StateType.Weak)
            {
                obj.battle.m_aniMgr.PlayQueued(AniConst.Dizzy1);
                idleTime = idleTime - obj.battle.m_aniMgr.GetAniLenght(AniConst.Dizzy1);
            }

            obj.battle.m_aniMgr.PlayQueued(AniConst.Dizzy2, 1.0f, idleTime);
            obj.battle.m_aniMgr.PlayQueued(AniConst.DizzyToIdel);
        }
    }
}
