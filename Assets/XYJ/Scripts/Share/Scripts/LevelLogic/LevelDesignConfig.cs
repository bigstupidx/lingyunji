using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using GUIEditor;
using xys.json;
#endif
[System.Serializable]
public class LevelDesignConfig
#if UNITY_EDITOR
    : IJsonFile
#endif
{
    public string GetKey() { return m_key; }
    public void SetKey(string key) { m_key = key; }

    public string m_key;

#if UNITY_EDITOR
    //当前选择的逻辑
    public string m_curSelectLogic;
#endif

    //初始逻辑
    public string m_startLevelLogic;
    //逻辑列表
    public List<LevelLogicData> m_levelLogicList = new List<LevelLogicData>();
    //全局事件
    public List<LevelEventObjData> m_overalEventList = new List<LevelEventObjData>();

    public LevelLogicData StartLogic
    {
        get
        {
            var v =  GetLogic(m_startLevelLogic);
            if (v == null)
                v = m_levelLogicList[0];

            return v;
        }
    }

    public LevelLogicData GetLogic(string name)
    {
        for (int i = 0; i < m_levelLogicList.Count; ++i)
        {
            if (m_levelLogicList[i].m_name == name)
                return m_levelLogicList[i];
        }

        return null;
    }

    //关卡逻辑数据
    [System.Serializable]
    public class LevelLogicData
    {
        public string m_name;           //逻辑命名
        public string m_scene;          //场景
        public string m_sceneStyle;     //场景风格
        //public string m_eventSet;       //事件集

        public List<LevelBornData> m_levelBornList = new List<LevelBornData>();                 //出生点列表
        public List<LevelSpawnData> m_levelSpawnList = new List<LevelSpawnData>();              //刷新器列表
        public List<LevelPointData> m_levelPointList = new List<LevelPointData>();              //点集列表
        public List<LevelPathData> m_levelPathList = new List<LevelPathData>();            //路径点列表
        public List<LevelAreaData> m_levelAreaList = new List<LevelAreaData>();                 //区域集列表
        public List<LevelEventObjData> m_roomEventList = new List<LevelEventObjData>();         //房间内事件
    }

    //关卡刷新点数据
    [System.Serializable]
    public class LevelSpawnData
    {
        public enum SpawnType
        {
            Null,               //默认一次性刷新
            Time,               //间隔时间刷新
            Dead,               //死亡刷新
        }

        public string m_spawnId;                //刷新点id
        public string m_spawnGroupId;           //刷新点组id
        public string m_name;                   //刷新点名字
        public bool m_isRandomSpawn;            //是否随机刷新
        public bool m_lookToPlayer;             //是否面向主角
        public bool m_isToGround;               //保存贴地
        public bool backToBorn = true;          //非战斗时回到出生点
        public bool fullHp = true;              //回到出生点是否满满血
        public bool m_isPassivity;              //是否被动怪
        public float m_fieldOfVision;           //视野
        public bool m_shareView;                //是否共享视野
        public int m_startNum;                  //初始刷新的数量
        public float m_startCd;                 //初始出生间隔
        public int m_maxNum;                    //最大数量
        public int m_survivalLimit;             //存活上限
        public SpawnType m_spawnType;           //刷新方式
        public float m_spawnParam1;             //参数1
        public int m_spawnParam2;               //参数2
        public int m_spawnParam3;               //参数3

        //ai相关的东西
        public string m_patrolId;               //巡逻id
        public string m_bornAction;             //出生行为
        public string m_enterBattleAction;      //进入战斗行为
        public string m_defaultIdle;            //默认休闲待机动作
        public string m_bornBubb;               //出生冒泡
        public string m_enterBattleBubb;        //进入战斗冒泡
        public bool m_enterBattleSigh;          //进入战斗叹号
        public string m_randomIdle;             //休闲随机待机
        public string m_autoPatrol;             //自动巡逻

        public List<int> m_objs = new List<int>();                  //对象列表
        public List<string> m_names = new List<string>();           //点集的名字
        public List<Vector3> m_postions = new List<Vector3>();      //点集的坐标
        public List<Vector3> m_dirs = new List<Vector3>();          //点集的方向
        public List<Vector3> m_scales = new List<Vector3>();        //点集的大小
    }

    /// <summary>
    /// 关卡出生点数据
    /// </summary>
    [System.Serializable]
    public class LevelBornData
    {
        public string m_bornId;
        public Vector3 m_pos;
        public Vector3 m_dir;
    }

    ///// <summary>
    ///// 关卡事件数据
    ///// </summary>
    //[System.Serializable]
    //public class LevelEventData
    //{
    //    public string m_eventSetId;                                                             //事件集id
    //    public bool m_isOverAll;                                                                //是否是全局事件
    //    public List<LevelEventObjData> m_events = new List<LevelEventObjData>();                //事件集的列表
    //}

    [System.Serializable]
    public class LevelEventObjData
    {
        public string m_eventId;                                                                //事件id
        public List<LevelEventCondition> m_conditions = new List<LevelEventCondition>();        //条件列表
        public List<LevelEventAction> m_actions = new List<LevelEventAction>();                 //行为列表
        public bool m_showMonitor = true;
    }

    [System.Serializable]
    public class LevelEventCondition
    {
        //关卡事件的条件
        public ConditionType m_conditionType;               //条件
        public bool m_isOr = false;                         //是否全部达成
        public string m_param1;                             //参数1
        public string m_param2;                             //参数2
        public string m_param3;                             //参数3
        public string m_param4;                             //参数4
        public string m_param5;                             //参数5
    }

    [System.Serializable]
    public class LevelEventAction
    {
        //关卡事件的行为
        public ActionType m_actionType;                     //行为类型
        public float m_delay;                               //延迟时间
        public string m_param1;                             //参数1
        public string m_param2;                             //参数2
        public string m_param3;                             //参数3
        public string m_param4;                             //参数4
        public string m_param5;                             //参数5
    }

    /// <summary>
    /// 关卡点集数据
    /// </summary>
    [System.Serializable]   
    public class LevelPointData
    {
        public string m_pointSetId;
        public List<Vector3> m_postions = new List<Vector3>();
        public List<Vector3> m_dirs = new List<Vector3>();
        public List<string> m_names = new List<string>();
        public bool m_saveToGround = true;
    }

    [System.Serializable]
    public class LevelPathData
    {
        public string m_pathId;
        public float m_speed;
        public List<Vector3> m_postions = new List<Vector3>();
        public List<Vector3> m_dirs = new List<Vector3>();
        public List<string> m_names = new List<string>();
        public List<float> m_stayTimes = new List<float>();
        public List<float> m_speeds = new List<float>();
        public List<string> m_events = new List<string>();
        public bool m_saveToGround = true;
    }

    /// <summary>
    /// 关卡区域集数据
    /// </summary>
    [System.Serializable]
    public class LevelAreaData
    {
        public enum AreaType
        {
            Rect,                           //矩形区域
            Round,                          //圆形区域
        }

        public string m_areaSetId;
        public bool m_isInitOpen = true;    //是否初始开放
        public bool m_saveToGround = true;  //保存是否贴地

        public List<Vector3> m_postions = new List<Vector3>();
        public List<Vector3> m_dirs = new List<Vector3>();                             
        public List<Vector3> m_scales = new List<Vector3>();
        public List<string> m_names = new List<string>();                             
        public List<AreaType> m_types = new List<AreaType>();      //区域类型                        
        public List<Vector3> m_centers = new List<Vector3>();     //区域中心点           
        public List<Vector3> m_sizes = new List<Vector3>();       //区域大小
        public List<float> m_radiuses = new List<float>();      //区域半径
    }

    /// <summary>
    /// 事件的条件
    /// </summary>
    [System.Serializable]
    public enum ConditionType
    {
#if UNITY_EDITOR
        [Enum2Type.EnumValue("无")]
#endif
        Null,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("刷新点死亡数量")]
        [Enum2Type.ParamValue("p1", "刷新点ID", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "数量", DataType.Int, "-1")]
#endif
        DeadCount,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("进入到指定区域")]
        [Enum2Type.ParamValue("p1", "刷新点ID", DataType.String, "-1")]
        [Enum2Type.ParamValue("p2", "区域ID", DataType.String, "")]
#endif
        EnterArea,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("进入关卡后指定时间")]
        [Enum2Type.ParamValue("p1", "流逝的时间(float)", DataType.Float, "")]
#endif
        EnterLevelTime,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("初始加载")]
#endif
        StartInit,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("怪物血量")]
        [Enum2Type.ParamValue("p1", "刷新点ID", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "血量百分比(float)", DataType.Float, "")]
#endif
        BloodPercent,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("区域吟唱完成")]
        [Enum2Type.ParamValue("p1", "区域触发器id", DataType.String, "")]
#endif
        AreaSing,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("npc吟唱完成")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
#endif
        NpcSing,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("事件完成")]
        [Enum2Type.ParamValue("p1", "事件", DataType.String, "")]
#endif
        ActionCompleted,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现完成")]
        [Enum2Type.ParamValue("p1", "表现id", DataType.Int, "1")]
#endif
        PerformanceFinish,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("个性事件是否触发")]
        [Enum2Type.ParamValue("p1", "个性事件表ID", DataType.Int, "0")]
        [Enum2Type.ParamValue("p2", "是否触发", DataType.Bool, "true")]
#endif
        PersonalityTrigger,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("服务器时间")]
        [Enum2Type.ParamValue("p1", "时间(时)", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "时间(分)", DataType.String, "")]
#endif
        ServerTime,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("指定怪物ID死亡数量")]
        [Enum2Type.ParamValue("p1", "id", DataType.Int, "0")]
        [Enum2Type.ParamValue("p2", "数量", DataType.Int, "0")]
#endif
        AppointedDeadCount,

#if UNITY_EDITOR
        [Enum2Type.EnumExpel()]
#endif
        Max,
    }

    /// <summary>
    /// 事件的行为
    /// </summary>
    [System.Serializable]
    public enum ActionType
    {
        #region 常用事件0-9999
#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/无")]
#endif
        Null,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/移除刷新点(ID)")]
        [Enum2Type.ParamValue("p1", "刷新点ID", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "是否计数", DataType.Bool, "false")]
#endif
        RemoveRefresh = 100,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/激活刷新点(ID)")]
        [Enum2Type.ParamValue("p1", "刷新点ID", DataType.String, "")]
#endif
        ActivaRefresh = 200,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/随机激活组刷新点(ID)")]
        [Enum2Type.ParamValue("p1", "组刷新点ID", DataType.String, "")]
#endif
        ActivaGroupRefresh = 300,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/关闭刷新点(ID)")]
        [Enum2Type.ParamValue("p1", "刷新点", DataType.String, "")]
#endif
        UnactivaRefresh = 400,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/胜利")]
        [Enum2Type.ParamValue("p1", "销毁敌人", DataType.Bool, "true")]
#endif
        Win = 500,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/失败")]
#endif
        Lost = 600,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/激活区域")]
        [Enum2Type.ParamValue("p1", "区域集id", DataType.String, "")]
#endif
        ActiveArea = 700,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/关闭区域")]
        [Enum2Type.ParamValue("p1", "区域集id", DataType.String, "")]
#endif
        UnActiveArea = 800,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/角色表现")]
        [Enum2Type.ParamValue("p1", "表现id", DataType.String, "")]
#endif
        RoleAction = 900,
        
#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/设置复活点")]
        [Enum2Type.ParamValue("p1", "点集ID", DataType.String, "")]
#endif
        SetRelivePoint = 1000,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/设置角色到对应点")]
        //[Enum2Type.ParamValue("p1", "出生点ID", DataType.String, "")]
        [Enum2Type.ParamValue("p1", "点集ID", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "刷新点ID", DataType.String, "")]
#endif
        SetToPoint = 1100,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/锁定怪物")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
#endif
        LockMonster = 1200,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/解除锁定怪物")]
#endif
        UnLockMonster = 1300,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/激活寻路点")]
        [Enum2Type.ParamValue("p1", "区域集id", DataType.String, "")]
#endif
        ActiveFindPath = 1400,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("常用事件/关闭寻路点")]
#endif
        UnactiveFindPath = 1500,
        #endregion

        #region 战斗相关 10000-19999
#if UNITY_EDITOR
        [Enum2Type.EnumValue("战斗相关/激活战斗")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
#endif
        ActiveBattle = 10000,
#if UNITY_EDITOR
        [Enum2Type.EnumValue("战斗相关/失去目标")]
        [Enum2Type.ParamValue("p1", "刷新点", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "时间", DataType.Float, "")]
#endif
        LostTarget = 10100,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("战斗相关/恢复目标")]
        [Enum2Type.ParamValue("p1", "刷新点", DataType.String, "")]
#endif
        RecoverTarget = 10200,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("战斗相关/添加buff")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "buff id", DataType.String, "")]
#endif
        AddBuff = 10300,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("战斗相关/移除buff")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "buff id", DataType.String, "")]
#endif
        RemoveBuff = 10400,
        #endregion

        #region 活动相关 20000-29999
#if UNITY_EDITOR
        [Enum2Type.EnumValue("活动相关/发布消息")]
        [Enum2Type.ParamValue("p1", "ID", DataType.Int, "0")]
#endif
        ReleaseMsg = 20000,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("活动相关/活动胜利")]
        [Enum2Type.ParamValue("p1", "ID", DataType.Int, "0")]
#endif
        ActivityWin = 20100,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("活动相关/活动失败")]
        [Enum2Type.ParamValue("p1", "ID", DataType.Int, "0")]
#endif
        ActivityLoss = 20200,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("活动相关/增加关卡时间")]
        [Enum2Type.ParamValue("p1", "时间长度(秒)", DataType.String, "")]
#endif
        AddTime = 20300,
        #endregion

        #region 表现相关  30000-39999      
#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/关卡提示")]
        [Enum2Type.ParamValue("p1", "tipsId", DataType.Int, "")]
#endif
        ShowLevelTip = 30000,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/逻辑暂停")]
#endif
        LogicPause = 30100,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/逻辑恢复")]
#endif
        LogicContinue = 30200,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/点击响应开关")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "开关", DataType.Int, "")]
#endif
        ClickOnOff = 30300,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/随机表现")]
        [Enum2Type.ParamValue("p1", "事件1 id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "事件2 id", DataType.String, "")]
        [Enum2Type.ParamValue("p3", "事件3 id", DataType.String, "")]
        [Enum2Type.ParamValue("p4", "事件4 id", DataType.String, "")]
        [Enum2Type.ParamValue("p5", "事件5 id", DataType.String, "")]
#endif
        RandomEvent = 30400,
        
#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/个性事件选项")]
        [Enum2Type.ParamValue("p1", "个性事件id", DataType.Int, "")]
#endif
        PersonalityEvent = 30500,

        #region 特效
#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/特效/播放特效")]
        [Enum2Type.ParamValue("p1", "特效名", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "时间", DataType.String, "")]
#endif
        PlayEffect = 30600,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/特效/播放挂点特效")]
        [Enum2Type.ParamValue("p1", "特效名", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "点集id(-1表示主角)", DataType.String, "")]
        [Enum2Type.ParamValue("p3", "时间", DataType.String, "")]
#endif
        PlayHangEffect = 30700,
        #endregion

        #region 动作
#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/动作/动作")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "动作名", DataType.String, "")]
        [Enum2Type.ParamValue("p3", "循环时间", DataType.Float, "-1")]
#endif
        RoleMotion = 30800,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/动作/动作无后续")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "动作名", DataType.String, "")]
#endif
        RoleMotionLastFrame = 30900,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/动作/巡逻")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "巡逻id", DataType.String, "")]
        [Enum2Type.ParamValue("p3", "是否正序循环", DataType.Bool, "True")]
#endif
        Patrol = 31000,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/动作/移动")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "点集id", DataType.String, "")]
        [Enum2Type.ParamValue("p3", "时间", DataType.Float, "0")]
#endif
        RoleMove = 31100,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/动作/转向")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "朝向类型", DataType.Int, "")]
        [Enum2Type.ParamValue("p3", "参数", DataType.String, "")]
        [Enum2Type.ParamValue("p4", "转向速度", DataType.Float, "0")]
#endif
        RoleRotate = 31200,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/动作/切换姿态")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "姿态id", DataType.String, "")]
#endif
        ChangePosture = 31300,
        #endregion

        #region 冒泡
#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/冒泡/冒泡")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "冒泡表id", DataType.String, "")]
        [Enum2Type.ParamValue("p3", "时间", DataType.Float, "4")]
#endif
        Bubbling = 31400,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/冒泡/随机冒泡")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "冒泡id", DataType.String, "")]
        [Enum2Type.ParamValue("p3", "冒泡概率", DataType.Float, "")]
        [Enum2Type.ParamValue("p4", "冒泡时间", DataType.Float, "")]
#endif
        RandomBubbling = 31500,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/冒泡/对白")]
        [Enum2Type.ParamValue("p1", "对白id", DataType.Int, "")]
#endif
        Dialogue = 31600,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/冒泡/冒泡开关")]
        [Enum2Type.ParamValue("p1", "刷新点id", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "开关", DataType.Int, "1")]
#endif
        BubblingOnOff = 31700,
        #endregion

        #region 动画
#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/动画/播放过场动画")]
        [Enum2Type.ParamValue("p1", "动画名称", DataType.String, "")]
#endif
        PlayCg = 31800,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/动画/播放交互动画")]
        [Enum2Type.ParamValue("p1", "交互对象名称", DataType.String, "")]
        [Enum2Type.ParamValue("p2", "动画名称", DataType.String, "")]
#endif
        Interaction = 31900,

#if UNITY_EDITOR
        [Enum2Type.EnumValue("表现相关/动画/播放镜头动画")]
        [Enum2Type.ParamValue("p1", "动画名称", DataType.String, "")]
#endif
        PlayCamera = 32000,
        #endregion
        #endregion
        
#if UNITY_EDITOR
        [Enum2Type.EnumExpel()]
#endif
        Max,
    }
}
