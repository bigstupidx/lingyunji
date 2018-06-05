using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    /// <summary>
    /// 战斗组件
    /// </summary>
    public interface IBattleComponent
    {
        void OnAwake(IObject obj);
        void OnStart();
        void OnDestroy();
        void OnEnterScene();
        void OnExitScene();
    }

    public interface IBattleUpdate
    {
        void OnUpdate();
    }

}
