#if !USE_HOT
namespace xys.hot
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Config;
    using wTimer;

    using NetProto.Hot;

    public partial class LevelEditorLogic
    {
        /// <summary>
        /// 静态方法，创建关卡配置的逻辑
        /// </summary>
        /// <param name="levelDefine"></param>
        /// <returns></returns>
        public static LevelEditorLogic Create(LevelDefine levelDefine)
        {
            string configId = levelDefine.configId;
            if (string.IsNullOrEmpty(configId))
                return null;

            //读取json文件
            if(null != levelDefine.jsonConfig)
            {
                return new LevelEditorLogic(levelDefine);
            }
            return null;
        }

        //全局参数
        //编辑器配置
        LevelDesignConfig m_config;
        LevelDesignConfig.LevelLogicData m_curLogicData;
        //关卡表配置
        LevelDefine m_levelDefine;
        //前端要做的区域存储
        Dictionary<string, LevelLogicArea> m_areaDic;
        //记录是否已经进入区域
        Dictionary<string, bool> m_areaTriggerDic;
        //事件存储
        public Dictionary<string, LevelDesignConfig.LevelEventObjData> m_eventDic { get; private set; }
        public Dictionary<string, LevelDesignConfig.LevelEventObjData> m_oaEventDic { get; private set; }
        public Dictionary<string, EventMonitorVo> m_conditionVoDic { get; private set; }
        //刷新点是否冒泡
        public Dictionary<string, bool> m_spawnBubb { get; private set; }
        //计时器
        SimpleTimer m_timer;
        //关卡模块
        LevelModule m_levelModule;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LevelEditorLogic(LevelDefine levelDefine, string name = "")
        {
            m_levelModule = App.my.localPlayer.GetModule<LevelModule>();
            m_levelDefine = levelDefine;
            m_config = m_levelDefine.jsonConfig;
            m_timer = new SimpleTimer(App.my.mainTimer);
            if (!string.IsNullOrEmpty(name))
                m_curLogicData = m_config.GetLogic(name);
            else
                m_curLogicData = m_config.StartLogic;
            Init();
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        void Init()
        {
            InitAreaConfig();
            InitEventConfig();
            InitEvent();
            InitSpawnConfig();
        }

        /// <summary>
        /// 退出关卡
        /// </summary>
        public void OnExit()
        {

        }

        /// <summary>
        /// 帧执行
        /// </summary>
        public void OnUpdate()
        {
            CheckInArea();
        }

        /// <summary>
        /// 初始化区域的配置
        /// </summary>
        void InitAreaConfig()
        {
            m_areaTriggerDic = new Dictionary<string, bool>();
            m_areaDic = new Dictionary<string, LevelLogicArea>();
            for(int i = 0; i < m_curLogicData.m_levelAreaList.Count; ++i)
            {
                LevelDesignConfig.LevelAreaData data = m_curLogicData.m_levelAreaList[i];
                string id = data.m_areaSetId;
                if (m_areaDic.ContainsKey(id))
                {
                    continue;
                }
                LevelLogicArea area = new LevelLogicArea(data);
                m_areaDic.Add(id, area);
            }
        }

        /// <summary>
        /// 初始化事件配置
        /// </summary>
        void InitEventConfig()
        {
            m_eventDic = new Dictionary<string, LevelDesignConfig.LevelEventObjData>();
            for(int i = 0; i < m_curLogicData.m_roomEventList.Count; ++i)
            {
                LevelDesignConfig.LevelEventObjData data = m_curLogicData.m_roomEventList[i];
                m_eventDic.Add(data.m_eventId, data);
            }

            if(m_oaEventDic == null)
            {
                m_oaEventDic = new Dictionary<string, LevelDesignConfig.LevelEventObjData>();
                for(int i = 0; i < m_config.m_overalEventList.Count; ++i)
                {
                    LevelDesignConfig.LevelEventObjData data = m_config.m_overalEventList[i];
                    m_oaEventDic.Add(data.m_eventId, data);
                }
            }
        }

        #region 刷新点
        public void InitSpawnConfig()
        {
            m_spawnBubb = new Dictionary<string, bool>();
            for (int i = 0; i < m_curLogicData.m_levelSpawnList.Count; ++i)
            {
                m_spawnBubb.Add(m_curLogicData.m_levelSpawnList[i].m_spawnId, true);
            }
        }

        /// <summary>
        /// 获取刷新点的下标
        /// </summary>
        /// <param name="spawnId"></param>
        /// <returns></returns>
        public int GetSpawnIndex(string spawnId)
        {
            int index = 1;
            for(int i = 0; i < m_curLogicData.m_levelSpawnList.Count; ++i)
            {
                LevelDesignConfig.LevelSpawnData data = m_curLogicData.m_levelSpawnList[i];
                if (data.m_spawnId == spawnId)
                    return index;

                index++;
            }
            return 1;
        }

        /// <summary>
        /// 获取刷新点数据
        /// </summary>
        /// <returns></returns>
        public LevelDesignConfig.LevelSpawnData GetSpawnData(int index)
        {
            if (0 == index)
                return null;

            index--;
            if(index < m_curLogicData.m_levelSpawnList.Count)
            {
                return m_curLogicData.m_levelSpawnList[index];
            }
            return null;
        }
        #endregion

        #region 点集
        /// <summary>
        /// 获取点集数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LevelDesignConfig.LevelPointData GetPointById(string id)
        {
            for (int i = 0; i < m_curLogicData.m_levelPointList.Count; ++i)
            {
                LevelDesignConfig.LevelPointData data = m_curLogicData.m_levelPointList[i];
                if (data.m_pointSetId == id)
                    return data;
            }
            return null;
        }
        #endregion

        #region 路径点
        public List<PathPointVo> GetPathById(string id)
        {
            for (int i = 0; i < m_curLogicData.m_levelPathList.Count; ++i)
            {
                LevelDesignConfig.LevelPathData data = m_curLogicData.m_levelPathList[i];
                if (data.m_pathId == id)
                {
                    List<PathPointVo> pathList = new List<PathPointVo>();
                    for(int j = 0; j < data.m_postions.Count; ++j)
                    {
                        PathPointVo vo = new PathPointVo();
                        vo.pos = data.m_postions[j];
                        vo.stayTime = data.m_stayTimes[j];
                        if (data.m_speeds[j] == 0)
                            vo.speed = data.m_speed;
                        else
                            vo.speed = data.m_speeds[j];
                        vo.eventId = data.m_events[j];

                        pathList.Add(vo);
                    }
                    return pathList;
                }
            }
            return null;
        }
        #endregion

        #region 区域集
        /// <summary>
        /// 获取一个区域
        /// </summary>
        /// <returns></returns>
        public LevelLogicArea GetAreaById(string areaId)
        {
            if(m_areaDic.ContainsKey(areaId))
            {
                return m_areaDic[areaId];
            }
            return null;
        }

        /// <summary>
        /// 区域每帧检测
        /// </summary>
        void CheckInArea()
        {
            if (null == App.my.localPlayer || !App.my.localPlayer.isAlive)
                return;

            //区域Update
            if (m_areaDic != null)
            {
                Vector3 pos = App.my.localPlayer.position;
                foreach (var itor in m_areaDic)
                {
                    LevelLogicArea area = itor.Value;
                    if(area.IsInArea(pos))
                    {
                        //进入了区域
                        InAreaAction(itor.Key);
                    }
                    else
                    {
                        if(m_areaTriggerDic.ContainsKey(itor.Key))
                        {
                            //出区域
                            m_areaTriggerDic.Remove(itor.Key);
                            Debuger.Log("离开区域 " + itor.Key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 进入区域后的行为
        /// </summary>
        void InAreaAction(string id, bool immediately = false)
        {
            if(m_areaTriggerDic.ContainsKey(id) && !immediately)
            {
                return;
            }
            Debuger.Log("进入区域 " + id);
            m_areaTriggerDic.Add(id, true);

            //通知服务器进入区域
            C2ALevelRequest request = new C2ALevelRequest(hotApp.my.gameRPC);
            request.LevelEnterArea(new NetProto.Str() { value = id}, null);
        }
        #endregion

        #region 事件
        public List<LevelDesignConfig.LevelEventObjData> GetEventList(bool overAll)
        {
            if(overAll)
            {
                return m_config.m_overalEventList;
            }
            else
            {
                return m_curLogicData.m_roomEventList;
            }
        }

        /// <summary>
        /// 执行一个事件
        /// </summary>
        /// <param name="eventId"></param>
        public void DoEvent(string eventId)
        {
            LevelDesignConfig.LevelEventObjData eventData;
            if (m_eventDic.TryGetValue(eventId, out eventData) || m_oaEventDic.TryGetValue(eventId, out eventData))
            {
                TriggerAction(eventData, App.my.localPlayer.charid);
            }
        }
        #endregion
    }
}
#endif