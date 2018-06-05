namespace xys
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Config;
    using wTimer;
    using NetProto;

    /// <summary>
    /// 关卡逻辑管理
    /// </summary>
    public class LevelLogicMgr
    {
        //LevelLogicBase m_levelLogic;
        //LevelDefine m_levelDefine;
        //SimpleTimer m_timer;
        //public LevelEditorLogic m_editorLogic { get; private set; }

        //public int levelId { get; private set; }
        //public long zoneId { get; private set; }

        //public LevelLogicMgr()
        //{
        //    //侦听事件
        //    //App.my.eventSet.Subscribe<int>(EventID.Level_Create, CreateLevel);
        //    App.my.eventSet.Subscribe(EventID.Level_Start, OnLevelStart);

        //    App.my.eventSet.Subscribe(EventID.BeginLoadScene, OnBeginLoadScene);
        //    App.my.eventSet.Subscribe(EventID.FinishLoadScene, OnFinishLoadScene);
        //}

        ///// <summary>
        ///// 创建了一个关卡，初始化关卡的数据
        ///// </summary>
        //public void CreateLevel(long zoneId, int levelId)
        //{
        //    if (null != m_levelLogic)
        //        m_levelLogic.OnExit();
        //    if (null != m_editorLogic)
        //        m_editorLogic.OnExit();

        //    this.levelId = levelId;
        //    this.zoneId = zoneId;

        //    m_levelDefine = LevelDefine.Get(this.levelId);
        //    if(null == m_levelDefine)
        //    {
        //        Debuger.LogError("关卡配置不存在 m_levelId " + this.levelId);
        //        return;
        //    }

        //    //创建关卡逻辑
        //    m_levelLogic = LevelLogicFactory.Create(m_levelDefine.levelType);
        //    if(null == m_levelLogic)
        //    {
        //        Debuger.LogError("不存在关卡类型");
        //        return;
        //    }
        //    m_levelLogic.Init(levelId);

        //    //创建关卡配置逻辑
        //    m_editorLogic = LevelEditorLogic.Create(m_levelDefine);
            
        //    m_timer = new SimpleTimer(App.my.mainTimer);
        //    m_timer.Register(0.1f, int.MaxValue, OnUpdate);

        //    //通知切换小地图
        //    App.my.eventSet.FireEvent<int>(EventID.MainPanel_ChangeMinimap, levelId);
        //}

        ///// <summary>
        ///// 关卡开始
        ///// </summary>
        //void OnLevelStart()
        //{
        //}

        ///// <summary>
        ///// 开始切换场景
        ///// </summary>
        //void OnBeginLoadScene()
        //{

        //}

        ///// <summary>
        ///// 加载场景结束
        ///// </summary>
        //void OnFinishLoadScene()
        //{

        //}

        ///// <summary>
        ///// 模拟帧执行
        ///// </summary>
        //void OnUpdate()
        //{
        //    if (null != m_levelLogic)
        //        m_levelLogic.OnUpdate();
        //    if (null != m_editorLogic)
        //        m_editorLogic.OnUpdate();
        //}

        ///// <summary>
        ///// 能否主动退出
        ///// </summary>
        ///// <param name="needCheckLeader">队长是否需要检测</param>
        ///// <returns></returns>
        //public bool CanInitiativeExit(bool needCheckLeader)
        //{
        //    LevelDefine config = LevelDefine.Get(levelId);
        //    LocalPlayer player = App.my.localPlayer;
        //    if(null != player)
        //    {
        //        if (player.GetModule<TeamModule>().InTeam())
        //        {
        //            if (needCheckLeader)
        //            {
        //                //队长也不显示
        //                if (config.cantInitiativeExit)
        //                {
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                //队长能显示，队员不能显示
        //                if (!player.GetModule<TeamModule>().IsLeader())
        //                {
        //                    //不能主动退出
        //                    if (config.cantInitiativeExit)
        //                    {
        //                        return false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// 关卡结束
        ///// </summary>
        ///// <param name="data"></param>
        //public void LevelFinish(LevelFinish data)
        //{
        //    if(m_levelLogic != null)
        //    {
        //        m_levelLogic.OnWinOrLost(data.status, data.chapterId);
        //    }
        //}

        ///// <summary>
        ///// 关卡触发事件
        ///// </summary>
        ///// <param name="data"></param>
        //public void LevelTriggerEvent(LevelTriggerEvent data)
        //{
        //    m_editorLogic.LevelTriggerEvent(data);
        //}

        ///// <summary>
        ///// 事件的条件打成通知，gm用
        ///// </summary>
        //public void LevelEventNotice(LevelEventNotice data)
        //{
        //    m_editorLogic.LevelEventNotice(data);
        //}

        ///// <summary>
        ///// 请求退出关卡
        ///// </summary>
        //public void ExitTheLevel()
        //{
        //    App.my.localPlayer.GetModule<LevelModule>().request.LevelExit(null, null);
        //}

        ///// <summary>
        ///// 获取当前关卡的剩余时间
        ///// </summary>
        ///// <returns></returns>
        //public int GetLevelLastTime()
        //{
        //    return m_levelLogic.GetLevelLastTime();
        //}

        ///// <summary>
        ///// 获取刷新点的下标
        ///// </summary>
        ///// <param name="spawnId"></param>
        ///// <returns></returns>
        //public int GetSpawnIndex(string spawnId)
        //{
        //    return m_editorLogic.GetSpawnIndex(spawnId);
        //}

        ///// <summary>
        ///// 获取点集的数据
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public LevelDesignConfig.LevelPointData GetPointData(string id)
        //{
        //    return m_editorLogic.GetPointById(id);
        //}

        ///// <summary>
        ///// 判断刷新点是否要贴地
        ///// </summary>
        ///// <returns></returns>
        //public bool SpawnNeedToGround(int spawnIndex)
        //{
        //    LevelDesignConfig.LevelSpawnData data = m_editorLogic.GetSpawnData(spawnIndex);
        //    if(data != null)
        //    {
        //        return data.m_isToGround;
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// 获取刷新点的缩放
        ///// </summary>
        ///// <param name="spawnIndex"></param>
        ///// <returns></returns>
        //public Vector3 GetSpawnScale(int spawnIndex)
        //{
        //    Vector3 scale = Vector3.one;
        //    LevelDesignConfig.LevelSpawnData data = m_editorLogic.GetSpawnData(spawnIndex);
        //    if(data != null && data.m_scales != null && data.m_scales.Count > 0)
        //    {
        //        scale = data.m_scales[0];
        //    }
        //    return scale;
        //}
    }
}
