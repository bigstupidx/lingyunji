using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class KnockDownState : IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            float finishTime = obj.battle.m_stateMgr.stateTimeLeft;

            //起身时间
            finishTime -= obj.battle.m_aniMgr.GetAniLenght(AniConst.StandUp);
            obj.battle.m_aniMgr.ClearQueued();

            //浮空
            if (lastState == StateType.Float)
            {
                float fallTime = 0.1f;
                obj.battle.actor.m_modelOffset.HitToGround(fallTime);
                //浮空直接切换倒地
                if (obj.battle.actor.m_modelOffset.HeightOff()<0.1f)
                {
                    obj.battle.m_aniMgr.PlayQueued(AniConst.KnockDownFromSky);
                }
                //浮空过程被倒地中断
                else
                {
                    float speed = obj.battle.m_aniMgr.GetSpeedByLenght(AniConst.FallFromSky, fallTime);
                    obj.battle.m_aniMgr.PlayQueued(AniConst.FallFromSky, speed, fallTime);
                    obj.battle.m_aniMgr.PlayQueued(AniConst.KnockDownFromSky, 1.0f, finishTime - fallTime);
                }
            }
            //倒地受击
            else if (lastState == StateType.KnockDown)
                obj.battle.m_aniMgr.PlayQueued(AniConst.KnockDownHit, 1.0f, finishTime);
            else
                obj.battle.m_aniMgr.PlayQueued(AniConst.KnockDown, 1.0f, finishTime);

            //添加起身动作
            obj.battle.m_aniMgr.PlayQueued(AniConst.StandUp);
        }

        public override void OnExit(IObject obj, StateType nextState) 
        {
            if (nextState != StateType.Float
                && nextState != StateType.KnockDown)
                obj.battle.actor.m_modelOffset.ResetHeight(obj);
        }
    }
}

