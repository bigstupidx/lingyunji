#if !USE_HOT
namespace xys.hot
{
    using UnityEngine;
    using Config;
    using wTimer;
    using NetProto.Hot;
    using System.Collections.Generic;

    public partial class LevelMgr
    {
        LevelLogicBase m_levelLogic;
        LevelDefine m_levelDefine;
        SimpleTimer m_timer;
        public LevelEditorLogic m_editorLogic { get; private set; }

        public int levelId { get; private set; }
        public long zoneId { get; private set; }

        public void RegistEvent(Event.HotObjectEventSet localEvent)
        {
            //侦听事件
            localEvent.Subscribe<int>(EventID.Level_Change, OnChangeLevel);
            localEvent.Subscribe<long>(EventID.Level_Start, OnLevelStart);
            localEvent.Subscribe(EventID.Level_Exit, OnLevelExit);
        }

        /// <summary>
        /// 创建了一个关卡，初始化关卡的数据
        /// </summary>
        public void CreateLevel(long zoneId, int levelId)
        {
            if (null != m_levelLogic)
                m_levelLogic.OnExit();
            if (null != m_editorLogic)
                m_editorLogic.OnExit();

            this.levelId = levelId;
            this.zoneId = zoneId;

            m_levelDefine = LevelDefine.Get(this.levelId);
            if (null == m_levelDefine)
            {
                Debuger.LogError("关卡配置不存在 m_levelId " + this.levelId);
                return;
            }

            //创建关卡逻辑
            m_levelLogic = LevelLogicFactory.Create(m_levelDefine.levelType);
            if (null == m_levelLogic)
            {
                Debuger.LogError("不存在关卡类型");
                return;
            }
            m_levelLogic.Init(levelId);

            //创建关卡配置逻辑
            m_editorLogic = LevelEditorLogic.Create(m_levelDefine);

            m_timer = new SimpleTimer(App.my.mainTimer);
            m_timer.Register(0.1f, int.MaxValue, OnUpdate);

            //通知切换小地图
            App.my.eventSet.FireEvent<int>(EventID.MainPanel_ChangeMinimap, levelId);
        }

        /// <summary>
        /// 关卡创建
        /// </summary>
        /// <param name="id"></param>
        void OnChangeLevel(int id)
        {
            request.RequestChange(new NetProto.Int32() { value = id }, null);
        }

        /// <summary>
        /// 关卡开始
        /// </summary>
        void OnLevelStart(long zoneId)
        {
            NetProto.ZoneType zt; int auid; ushort serverid; ushort mapid;
            Common.Utility.Zone(zoneId, out zt, out auid, out serverid, out mapid);

            CreateLevel(zoneId, mapid);
        }

        /// <summary>
        /// 事件通知结束关卡
        /// </summary>
        void OnLevelExit()
        {
            ExitTheLevel();
        }

        /// <summary>
        /// 模拟帧执行
        /// </summary>
        void OnUpdate()
        {
            if (null != m_levelLogic)
                m_levelLogic.OnUpdate();
            if (null != m_editorLogic)
                m_editorLogic.OnUpdate();
        }

        /// <summary>
        /// 能否主动退出
        /// </summary>
        /// <param name="needCheckLeader">队长是否需要检测</param>
        /// <returns></returns>
        public bool CanInitiativeExit(bool needCheckLeader)
        {
            LevelDefine config = LevelDefine.Get(levelId);
            LocalPlayer player = App.my.localPlayer;
            if (null != player)
            {
                if (player.GetModule<TeamModule>().InTeam())
                {
                    if (needCheckLeader)
                    {
                        //队长也不显示
                        if (config.cantInitiativeExit)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //队长能显示，队员不能显示
                        if (!player.GetModule<TeamModule>().IsLeader())
                        {
                            //不能主动退出
                            if (config.cantInitiativeExit)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 判断某个关卡的次数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckLevelCount(int id)
        {
            if (levelData.count.ContainsKey(id))
            {
                int count = levelData.count[id];
                if (count <= m_levelDefine.maxChallengeTimes)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 关卡结束
        /// </summary>
        /// <param name="data"></param>
        public void LevelFinish(LevelFinish data)
        {
            if (m_levelLogic != null)
            {
                m_levelLogic.OnWinOrLost(data.status, data.chapterId);
            }
        }

        /// <summary>
        /// 关卡触发事件
        /// </summary>
        /// <param name="data"></param>
        public void LevelTriggerEvent(LevelTriggerEvent data)
        {
            m_editorLogic.LevelTriggerEvent(data);
        }

        /// <summary>
        /// 事件的条件打成通知，gm用
        /// </summary>
        public void LevelEventNotice(LevelEventNotice data)
        {
            m_editorLogic.LevelEventNotice(data);
        }

        /// <summary>
        /// 请求退出关卡
        /// </summary>
        public void ExitTheLevel()
        {
            request.LevelExit(null);
        }

        /// <summary>
        /// 获取当前关卡的剩余时间
        /// </summary>
        /// <returns></returns>
        public int GetLevelLastTime()
        {
            return m_levelLogic.GetLevelLastTime();
        }

        /// <summary>
        /// 获取刷新点的下标
        /// </summary>
        /// <param name="spawnId"></param>
        /// <returns></returns>
        public int GetSpawnIndex(string spawnId)
        {
            return m_editorLogic.GetSpawnIndex(spawnId);
        }

        /// <summary>
        /// 获取点集的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LevelDesignConfig.LevelPointData GetPointData(string id)
        {
            return m_editorLogic.GetPointById(id);
        }

        /// <summary>
        /// 判断刷新点是否要贴地
        /// </summary>
        /// <returns></returns>
        public bool SpawnNeedToGround(int spawnIndex)
        {
            LevelDesignConfig.LevelSpawnData data = GetSpawnData(spawnIndex);
            if (data != null)
            {
                return data.m_isToGround;
            }
            return true;
        }

        /// <summary>
        /// 获取刷新点的缩放
        /// </summary>
        /// <param name="spawnIndex"></param>
        /// <returns></returns>
        public Vector3 GetSpawnScale(int spawnIndex)
        {
            Vector3 scale = Vector3.one;
            LevelDesignConfig.LevelSpawnData data = GetSpawnData(spawnIndex);
            if (data != null && data.m_scales != null && data.m_scales.Count > 0)
            {
                scale = data.m_scales[0];
            }
            return scale;
        }

        /// <summary>
        /// 获取刷新点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LevelDesignConfig.LevelSpawnData GetSpawnData(int index)
        {
            return m_editorLogic.GetSpawnData(index);
        }

        /// <summary>
        /// 获取事件列表
        /// </summary>
        /// <param name="overAll"></param>
        /// <returns></returns>
        public List<LevelDesignConfig.LevelEventObjData> GetEventList(bool overAll)
        {
            return m_editorLogic.GetEventList(overAll);
        }

        public Dictionary<string, EventMonitorVo> GetConditionVo()
        {
            return m_editorLogic.m_conditionVoDic;
        }

        /// <summary>
        /// 获取路径数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<PathPointVo> GetPathById(string id)
        {
            return m_editorLogic.GetPathById(id);
        }

        /// <summary>
        /// 执行一个事件
        /// </summary>
        /// <param name="eventId"></param>
        public void DoEvent(string eventId)
        {
            m_editorLogic.DoEvent(eventId);
        }
    }
}
#endif