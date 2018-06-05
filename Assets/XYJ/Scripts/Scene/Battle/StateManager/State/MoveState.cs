using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class MoveState : IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            CheckTween(obj);
            obj.battle.m_aniMgr.PlayStateAni();
        }

        public override void OnExit(IObject obj, StateType nextState)
        {
            //快跑结束动作
            if (nextState == StateType.Idle && obj.battle.m_stateMgr.isFastRun)
            {
                obj.battle.m_stateMgr.SetStateFinish(obj.battle.m_aniMgr.GetAniLenght(obj.battle.m_attrMgr.postureCfg.fastRunStop));
                obj.battle.m_aniMgr.PlayAni(obj.battle.m_attrMgr.postureCfg.fastRunStop);
                obj.battle.m_aniMgr.PlayQueued(obj.battle.m_attrMgr.postureCfg.normalIdle);
                obj.battle.m_aniMgr.SetIdleStateNoPlayAni();
            }

            obj.battle.m_moveMgr.StopMove();
        }  

        void CheckTween(IObject obj)
        {
            if(obj.battle.actor.m_rootTrans.GetComponent<iTween>() != null)
            {
                iTween.Stop(obj.battle.actor.m_rootTrans.gameObject);
            }
        }
    }
}
