using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{
    // 应用的状态类型
    public enum AppStateType
    {
        Null,
        BeginLogin, // 开始登陆状态
        SelectServer, // 选择服务器状态
        SelectCharacter, // 选择角色状态
        CreateCharacter, // 创建角色状态
        ChangeScene, // 切换场景状态
        GameIn, // 游戏内状态
    }

    public enum StateEvent
    {
        Begin, // 进入此状态
        End, // 离开此状态
    }

    public interface IAppState
    {
        AppStateType state { get; }
        EventAgent Event { get; }
        void Enter(object p); // 进入此状态
        void Level(); // 离开此状态
        void Update(); // 处于此状态下的桢更新
    }

    class AppStateBase : IAppState
    {
        public AppStateBase(AppStateType state)
        {
            this.state = state;
        }

        public AppStateType state { get; private set; }
        public EventAgent Event { get; protected set; }

        // 进入此状态
        public void Enter(object p)
        {
            Event = new EventAgent();
            OnEnter(p);
            App.my.eventSet.FireEvent<IAppState>(StateEvent.Begin, state, this);
        }

        protected virtual void OnEnter(object p) { }

        // 离开此状态
        public void Level()
        {
            Event.Release();
            OnLevel();
            App.my.eventSet.FireEvent<IAppState>(StateEvent.End, state, this);

            App.my.appStateMgr.Level(state);
        }

        protected virtual void OnLevel() { }

        // 处于此状态下的桢更新
        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate() { }
    }

    public class HotAppStateBase : IAppState
    {
        public HotAppStateBase(AppStateType state, string typeName)
        {
            this.state = state;
            refType = new RefType(typeName, this);
        }

        RefType refType;
        public EventAgent Event { get; protected set; }

        public AppStateType state { get; private set; }

        // 进入此状态
        public void Enter(object p)
        {
            Event = new EventAgent();
            refType.InvokeMethod("Enter", p);
            App.my.eventSet.FireEvent<IAppState>(StateEvent.Begin, state, this);
        }

        // 离开此状态
        public void Level()
        {
            Event.Release();
            refType.InvokeMethod("Level");
            App.my.eventSet.FireEvent<IAppState>(StateEvent.End, state, this);

            App.my.appStateMgr.Level(state);
        }

        // 处于此状态下的桢更新
        public void Update()
        {
            refType.InvokeMethod("Update");
        }
    }
}

