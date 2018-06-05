using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{
    public class AppStateMgr
    {
        //Dictionary<AppStateType, IAppState> States = new Dictionary<AppStateType, IAppState>();

        IAppState currentState = null;

        public AppStateType curState { get { return currentState == null ? AppStateType.Null : currentState.state; } }

        public void Update()
        {
            if (currentState == null)
                return;

            currentState.Update();
        }

        IAppState Create(AppStateType type)
        {
            switch (type)
            {
            case AppStateType.BeginLogin: return new HotAppStateBase(AppStateType.BeginLogin, "xys.hot.BeginLoginState");
            case AppStateType.CreateCharacter: return new HotAppStateBase(AppStateType.CreateCharacter, "xys.hot.CreateCharacterState");
            case AppStateType.SelectCharacter: return new HotAppStateBase(AppStateType.SelectCharacter, "xys.hot.SelectCharacterState");
            case AppStateType.SelectServer: return new HotAppStateBase(AppStateType.SelectServer, "xys.hot.SelectServerState");
            case AppStateType.ChangeScene: return new ChangeSceneState();
            case AppStateType.GameIn: return new GameInState();
            }

            throw new System.Exception("error state:" + type);
        }

        // 进入状态
        public void Enter(AppStateType type, object p)
        {
            if (curState == type)
            {
                Debuger.ErrorLog("Repate Enter State:{0}!", type);
                return;
            }

            if (currentState != null)
            {
                currentState.Level();
                currentState = null;
            }

            currentState = Create(type);
            currentState.Enter(p);
        }

        public void Level(AppStateType type)
        {
            if (curState == type)
            {
                currentState = null;
            }
        }
    }
}
