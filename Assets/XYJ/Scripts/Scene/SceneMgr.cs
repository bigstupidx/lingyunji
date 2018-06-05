namespace xys
{
    using Network;
    using NetProto;
    using System.Collections.Generic;
    using System.Collections;
    using System;

    public class SceneMgr
    {
        enum DestroyType
        {
            Destroy,//删除
            Exit,//退出
            Keep,//保留
        }

        Dictionary<int, IObject> m_objMap = new Dictionary<int, IObject>(); // 当前所有场景对象
        List<IObject> m_objList = new List<IObject>(); // 当前所有场景对象
        Dictionary<int, RemotePlayer> m_playerMap = new Dictionary<int, RemotePlayer>(); // 远程玩家

        Action m_actionAfterChangeScene;

        public SceneMgr()
        {
            App.my.handler.Reg<SOSyncs>(Protoid.A2C_AddSceneObject, OnAddSceneObject); // 添加场景对象
            App.my.handler.Reg<Int32s>(Protoid.A2C_RemoveObject, OnRemoveSceneObject); // 移除场景对象

            App.my.handler.Reg<wProtobuf.Int32AndBytes>(Protoid.A2C_SceneAttChange, OnSyncSceneAttChange); // 场景对象属性变化
            App.my.handler.Reg<ChangeSceneData>(Protoid.A2C_ChangeScene_Begin, OnBeginChangeMap); // 服务器通知开始切换场景
            App.my.handler.Reg(Protoid.A2C_FinishInitLoad, OnFinishInitLoad); // 初始加载完成

            App.my.eventSet.Subscribe<bool>(EventID.BeginLoadScene, OnBeginLoadScene);
            App.my.eventSet.Subscribe(EventID.FinishLoadScene, OnFinishLoadScene);
            App.my.eventSet.Subscribe(EventID.BackToLogin, OnBackToLogin);
        }

        public void Update()
        {
            if (App.my.appStateMgr.curState != AppStateType.GameIn)
                return;
            foreach(var p in m_objMap)
            {
                p.Value.Update();
            }
        }

        public IObject GetObj( int charSceneId )
        {
            IObject obj;
            if (m_objMap.TryGetValue(charSceneId, out obj))
                return obj;
            return null;
        }

        public Dictionary<int, IObject> GetObjs()
        {
            return m_objMap;
        }

        /// <summary>
        /// 获取场景中对应类型的对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<IObject> GetMapRoleByType(ObjectType type)
        {
            List<IObject> list = new List<IObject>();
            for(int i = 0; i < m_objList.Count; ++i)
            {
                if (m_objList[i].type == type)
                    list.Add(m_objList[i]);
            }
            if (list.Count > 0)
                return list;
            return null;
        }

        /// <summary>
        /// 获取刷新点的对象
        /// </summary>
        /// <param name="spawnId"></param>
        /// <returns></returns>
        public List<NpcBase> GetNpcsBySpawnId(int spawnId)
        {
            List<NpcBase> list = new List<NpcBase>();
            for (int i = 0; i < m_objList.Count; ++i)
            {
                NpcBase npc = m_objList[i] as NpcBase;
                if (npc != null && npc.m_refreshId == spawnId)
                {
                    list.Add(npc);
                }
            }

            if (list.Count > 0)
                return list;
            return null;
        }

        /// <summary>
        /// 请求切换场景
        /// </summary>
        /// <param name="id">关卡表id</param>
        /// <param name="action">切换完成之后的行为</param>
        public void ChangeScene(int id, Action action = null)
        {
            if(null != action)
                m_actionAfterChangeScene = action;

            App.my.eventSet.FireEvent<int>(EventID.Level_Change, id);
        }
        
        void OnBeginLoadScene(bool isSeamless)
        {
            if (!isSeamless)
                ClearAllObj();
            else
                SeamlessClearObj();
        }
       

        void ClearAllObj( bool backToLogin = false)
        {
            for (int i = m_objList.Count - 1; i >= 0; i--)
            {
                //本地玩家不调用销毁，当时重回登录需要销毁
                DestroyType destroyType = (backToLogin ||  m_objList[i] != App.my.localPlayer) ? DestroyType.Destroy : DestroyType.Exit;
                RemoveObj(m_objList[i].charSceneId, destroyType);
            }
               
            m_objList.Clear();
            m_objMap.Clear();
            m_playerMap.Clear();
        }

        void SeamlessClearObj()
        {
            for (int i = m_objList.Count - 1; i >= 0; i--)
            {
                //主角不需要销毁
                DestroyType destroyType = m_objList[i] == App.my.localPlayer ? DestroyType.Keep : DestroyType.Destroy;
                RemoveObj(m_objList[i].charSceneId, destroyType);

                if(destroyType != DestroyType.Keep)
                {
                }
            }
        }

        void OnFinishLoadScene()
        {
            //登录界面不算
            if (App.my.localPlayer.battle!=null)
            {
                if(!m_objList.Contains(App.my.localPlayer))
                    AddObj(App.my.localPlayer);
                //动态阴影管理
                DrawTargetObjectManage.Create();
            }

            if (null != m_actionAfterChangeScene)
                m_actionAfterChangeScene();
            m_actionAfterChangeScene = null;
        }

        void OnBackToLogin()
        {
            ClearAllObj(true);
            //App.my.localPlayer.battle.Awake();
        }

        IObject Create(ObjectType type,int charSceneId)
        {
            switch (type)
            {
                case ObjectType.Npc: return new NpcBase(type,charSceneId);
            case ObjectType.Player: return new RemotePlayer(charSceneId);
            case ObjectType.Pet: return new Pet(charSceneId);
            }
            return null;
        }

        // 添加一个场景对象
        void OnAddSceneObject(IPacket packet, SOSyncs sosyncs)
        {
            foreach (var p in sosyncs.objs)
            {
                IObject obj = Create(p.type,p.charSceneId);
                if (obj == null)
                    return;
                obj.InitDataByAOI(p);
                AddObj(obj);
            }

        }

        void AddObj( IObject obj )
        {
            if(m_objMap.ContainsKey(obj.charSceneId))
            {
                Debuger.LogError(string.Format("场景重复添加对象 charSceneId={0} type={1}",obj.charSceneId,obj.GetType()));
                return;
            }
            m_objMap.Add(obj.charSceneId, obj);
            m_objList.Add(obj);
            //开始创建模型
            obj.EnterScene();

            App.my.eventSet.FireEvent<IObject>(EventID.Scene_AddObj, obj);

            Debuger.Log(string.Format("角色进入场景 type={0} id={1}",obj.GetType(),obj.charSceneId));
        }

        void RemoveObj(int v, DestroyType destroyType = DestroyType.Destroy)
        {
            IObject obj;
            if(m_objMap.TryGetValue(v, out obj) && destroyType != DestroyType.Keep)
            {
                m_objMap.Remove(v);
                m_objList.Remove(obj);
                //正在播放死亡动画则先不删除模型,等动画结束再删除
                if (obj.battle!=null && obj.battle.m_stateMgr.m_curStType != battle.StateType.Dead)
                {
                    if (destroyType == DestroyType.Destroy)
                        obj.Destroy();
                    else if(destroyType == DestroyType.Exit)
                        obj.ExitScene();
                }

                App.my.eventSet.FireEvent<int>(EventID.Scene_RemoveObj, obj.charSceneId);
                Debuger.Log(string.Format("角色离开场景 type={0} id={1}", obj.GetType(), obj.charSceneId));
            }          
        }

        void OnRemoveSceneObject(IPacket packet, Int32s v)
        {
            foreach (var p in v.value)
            {
                RemoveObj(p, DestroyType.Destroy);
            }
        }

        void OnRemoveAllObjects(IPacket packet)
        {

        }

        void OnSyncSceneAttChange(IPacket packet, wProtobuf.Int32AndBytes data)
        {
            IObject obj = GetObj(data.id);
            if (obj == null)
            {
                Log.Error("OnSyncSceneAttChange charSceneID:{0} not find!", data.id);
                return;
            }

            BitStream stream = new BitStream(data.bytes);
            stream.WritePos = data.bytes.Length;
            while (stream.ReadSize != 0)
            {
                AttType id = (AttType)stream.ReadInt32();
                var att = obj.attributes.Get(id);

                AttributeChange change = new AttributeChange();
                change.id = id;
                change.oldValue = att.Clone();
                att.MergeFrom(stream);
                change.currentValue = att.Clone();

                obj.eventSet.FireEvent(id, change);
                Debuger.Log(string.Format("属性修改 id={0} old={1} new={2}", id, change.oldValue.realValue, change.currentValue.realValue));
            }
        }

        void OnBeginChangeMap(IPacket packet, ChangeSceneData data)
        {
            App.my.appStateMgr.Enter(AppStateType.ChangeScene, data);
            App.my.localPlayer.BeginChangeScene(data);
        }

        /// <summary>
        /// 初始加载完成
        /// </summary>
        void OnFinishInitLoad(IPacket packet)
        {
            App.my.main.StartCoroutine(WaitInitLoad());
        }

        /// <summary>
        /// 等待初始加载完成
        /// </summary>
        IEnumerator WaitInitLoad()
        {
            int count = 0;
            while (count <= 5)
            {
                yield return 5;
                count++;
            }
            App.my.uiSystem.loadingMgr.Hide();
        }
    }
}
