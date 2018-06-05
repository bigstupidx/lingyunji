using NetProto;
using System;
using System.Collections.Generic;
using UnityEngine;
using wProtobuf;
using xys.battle;
using GameServer.Battle;
using Config;
using GameServer;

namespace xys.battle
{
    public class BattleManager
    {
        public IObject m_obj { get; private set; }

        public BattleAttriMgr m_attrMgr { get; protected set; }
        public StateManager m_stateMgr { get; protected set; }
        public SkillManagerBase m_skillMgr { get; protected set; }
        public BuffManagerBase m_buffMgr { get; protected set; }
        public MoveManagerBase m_moveMgr { get; protected set; }
        public BattleAttriLogic m_attrLogic { get { return m_attrMgr.logic; } }
        public TargetManager m_targetMgr { get; protected set; }
        public AIManager m_ai { get; protected set; }
        //是否本地控制的角色.客户端控制会向服务器请求技能，发送移动
        public bool m_isAiByLocal { get; protected set; }
        public float speed { get { return m_obj.speedValue / 100.0f; } }



        #region 提供给外部的接口
        //直线移动到位置
        public bool State_MoveToPos(Vector3 toPos, Action<object> callback = null, object para = null, float speed = 0)
        {
            m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
            m_moveMgr.SetMoveToPos(toPos, callback, para, speed);
            return true;
        }
        //移动到位置
        public bool State_MoveToTarget(IObject target, float arriveDistance = 0.5f, Action<object> callback = null, object para = null, float speed = 0)
        {
            m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
            m_moveMgr.SetMoveToTarget(target, arriveDistance, callback, para, speed);
            return true;
        }

        #endregion


        #region 内部实现
        List<IBattleComponent> components = new List<IBattleComponent>();
        List<IBattleUpdate> updates = new List<IBattleUpdate>();

        public BattleManager(IObject obj)
        {
            m_obj = obj;
        }


        protected virtual void InitComponent()
        {

        }
        public virtual void Awake()
        {
            InitComponent();
            foreach (var p in components)
            {
                p.OnAwake(m_obj);
            }
            foreach (var p in components)
            {
                p.OnStart();
                IBattleUpdate u = p as IBattleUpdate;
                if (u != null)
                    updates.Add(u);
            }
            m_obj.cfgInfo.InitByCfg(m_obj);
        }

        public virtual void EnterScene()
        {           
            foreach (var p in components)
            {
                p.OnEnterScene();
            }
        }

        public virtual void ExitScene()
        {
            foreach (var p in components)
            {
                p.OnExitScene();
            }
        }

        public virtual void Destroy()
        {
            foreach (var p in components)
            {
                p.OnDestroy();
            }
            updates.Clear();
            components.Clear();
            m_obj = null;
        }

        public void Update()
        {
            foreach (var p in updates)
            {
                p.OnUpdate();
            }
        }

        protected T AddCompoment<T>(T component) where T : IBattleComponent
        {
            components.Add(component);
            return component;
        }
        #endregion


    }
}
