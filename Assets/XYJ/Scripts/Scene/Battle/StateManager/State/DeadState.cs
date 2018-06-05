using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class DeadState : IState
    {
        int timerid;
        public override void OnEnter(IObject obj, StateType lastState, object para)
        {
            float timeLenght = obj.battle.m_aniMgr.GetAniLenght(AniConst.Dead);
            obj.battle.m_aniMgr.PlayAni(AniConst.Dead);
            timerid = App.my.mainTimer.Register<IObject>(timeLenght, 1,OnFinishAni, obj);
        }


        public override void OnExit(IObject obj, StateType nextState)
        {
            
        }

        void OnFinishAni(IObject obj)
        {
            //如果角色已经移除，则需要把模型删除,否则不用
            if (App.my.sceneMgr.GetObj(obj.charSceneId) == null)
                obj.Destroy();
        }
    }
}
