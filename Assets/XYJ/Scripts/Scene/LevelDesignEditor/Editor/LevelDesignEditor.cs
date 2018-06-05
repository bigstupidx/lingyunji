using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using EditorExtensions;
using System.Runtime.InteropServices;

public partial class LevelDesignEditor : JsonDirectoryEditorBase<LevelDesignConfig>
{
    protected override string GetJsonDirectoryPath()
    {
        return "Data/Config/Edit/Level/LevelDesignConfig/";
    }

    static public LevelDesignEditor Instance { get; protected set; }
    Vector2 m_scrollPos = Vector3.zero;

    static List<string> m_allScenes;

    [MenuItem("EBEditor/关卡编辑器")]
    static void OpenSceneEditorWindow()
    {
        Instance = GetWindow<LevelDesignEditor>(false, "关卡编辑器", true);
        Instance.minSize = new Vector2(640.0f, 480.0f);

        Instance.titleContent.image = EditorIconContent.GetSystem(EditorIconContentType.Terrain_Icon).image;

        GetAllScene();
    }

    void OnEnable()
    {
        Instance = this;
        LoadCsvConfig();
        ClearData();
        ReloadAll();
    }

    void OnDisable()
    {
        Instance = null;
        ClearData();
        RecordLastConfigIndex();
    }

    void DoSort()
    {
        SortOverallEvent();
        SortLogic();

        for (int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
        {
            LevelDesignConfig.LevelLogicData data = m_curEditConfig.m_levelLogicList[i];
            SortBorn(data);
            SortSpawn(data);
            SortPoint(data);
            SortArea(data);
            SortPath(data);
            SortEvent(data);
        }
    }

    protected override void DrawToolBarEx()
    {
        base.DrawToolBarEx();
        //文件夹选择
        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.MeshRenderer_Icon), EditorStyles.toolbarButton, GUILayout.Width(30)))
        {
            string filename = EditorUtility.OpenFilePanel("读取关卡配置文件", GetJsonDirectoryPath(), "json");
            filename = filename.Substring(0, filename.LastIndexOf("."));
            filename = filename.Substring(filename.LastIndexOf("/") + 1);

            if (m_editConfigNameList != null && m_editConfigNameList.Count > 0)
            {
                for (int i = 0; i < m_editConfigNameList.Count; ++i)
                {
                    if (m_editConfigNameList[i] == filename)
                    {
                        SelectConfig(i);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 选择配置之后
    /// </summary>
    protected override void ActionAfterSelect()
    {
        ClearData();
        //绘制全局事件
        DrawOverallEventHierarchy();
        //绘制逻辑的层次面板
        DrawLogicHierarchy();
        //绘制出生点
        DrawBornHierarchy();
        //绘制刷新器
        DrawSpawnHierarchy();
        //绘制点集
        DrawPointHierarchy();
        //绘制区域集
        DrawAreaHierarchy();
        //绘制路径
        DrawPathHierarchy();
        //绘制事件
        DrawEventHierarchy();

        DoSort();
    }
    
    /// <summary>
    /// 新增配置之后
    /// </summary>
    protected override void ActionAfterAdd()
    {
        ActionAfterSelect();
    }
    
    /// <summary>
    /// 保存配置之前
    /// </summary>
    protected override void ActionBeforeSave()
    {
        //判断当前是否有起始逻辑，必须要有才行
        if(string.IsNullOrEmpty(m_curEditConfig.m_startLevelLogic))
        {
            Debuger.LogError("没有选择初始逻辑");
        }

        //获取所有的数据
        GetAllData();

        //刷新点需要判断是否保存贴地，保存贴地的话需要重新计算坐标
        if (null != m_curEditConfig && null != m_curEditConfig.m_levelLogicList && m_curEditConfig.m_levelLogicList.Count > 0)
        {
            //for(int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
            {
                LevelDesignConfig.LevelLogicData logic = GetCurSelectData();
                Transform logicTrans = GetLogicTrans(logic.m_name);
                Collider[] colliders = logicTrans.GetComponentsInChildren<Collider>();
                for (int i = 0; i < colliders.Length; ++i)
                {
                    colliders[i].gameObject.layer = 2;
                }
                //刷新点贴地
                if (null != logic.m_levelSpawnList && logic.m_levelSpawnList.Count > 0)
                {
                    for(int j = 0; j < logic.m_levelSpawnList.Count; ++j)
                    {
                        LevelDesignConfig.LevelSpawnData spawn = logic.m_levelSpawnList[j];
                        if(spawn.m_isToGround)
                        {
                            string id = spawn.m_spawnId;
                            GameObject go = GetSpawnObj(id, logic);
                            for(int k = 0; k < spawn.m_postions.Count; ++k)
                            {
                                string name = spawn.m_names[k];
                                spawn.m_postions[k] = GetColliderPos(spawn.m_postions[k], ComLayer.Layer_RoleFallDownMask, true);
                                if (go != null)
                                {
                                    Transform trans = go.transform.Find(name);
                                    if (null != trans)
                                        trans.position = spawn.m_postions[k];
                                }
                            }
                        }
                    }
                }
                //点集贴地
                if(null != logic.m_levelPointList && logic.m_levelPointList.Count > 0)
                {
                    for(int j = 0; j < logic.m_levelPointList.Count; ++j)
                    {
                        LevelDesignConfig.LevelPointData point = logic.m_levelPointList[j];
                        if(point.m_saveToGround)
                        {
                            string id = point.m_pointSetId;
                            GameObject go = GetPointObj(id, logic);
                            for (int k = 0; k < point.m_postions.Count; ++k)
                            {
                                string name = point.m_names[k];
                                point.m_postions[k] = GetColliderPos(point.m_postions[k], ComLayer.Layer_RoleFallDownMask, true);
                                if(go != null)
                                {
                                    Transform trans = go.transform.Find(name);
                                    if (null != trans)
                                        trans.position = point.m_postions[k];
                                }
                            }
                        }
                    }
                }
                //路径
                if (null != logic.m_levelPathList && logic.m_levelPathList.Count > 0)
                {
                    for (int j = 0; j < logic.m_levelPathList.Count; ++j)
                    {
                        LevelDesignConfig.LevelPathData path = logic.m_levelPathList[j];
                        if (path.m_saveToGround)
                        {
                            string id = path.m_pathId;
                            GameObject go = GetPathObj(id, logic);
                            for (int k = 0; k < path.m_postions.Count; ++k)
                            {
                                string name = path.m_names[k];
                                path.m_postions[k] = GetColliderPos(path.m_postions[k], ComLayer.Layer_RoleFallDownMask, true);
                                if (go != null)
                                {
                                    Transform trans = go.transform.Find(name);
                                    if (null != trans)
                                        trans.position = path.m_postions[k];
                                }
                            }
                        }
                    }
                }

                //出生点贴地
                if (null != logic.m_levelBornList && logic.m_levelBornList.Count > 0)
                {
                    for(int j = 0; j < logic.m_levelBornList.Count; ++j)
                    {
                        LevelDesignConfig.LevelBornData born = logic.m_levelBornList[j];
                        string id = born.m_bornId;
                        GameObject go = GetBornObj(id, logic);
                        born.m_pos = GetColliderPos(born.m_pos, ComLayer.Layer_RoleFallDownMask, true);
                        go.transform.position = born.m_pos;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取所有的数据
    /// </summary>
    void GetAllData()
    {
        //全局数据
        SetOverallEventData();
        SetLogicData();

        //每个房间逻辑的数据
        for(int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
        {
            LevelDesignConfig.LevelLogicData data = m_curEditConfig.m_levelLogicList[i];

            SetBornData(data);
            SetSpawnData(data);
            SetPointData(data);
            SetAreaData(data);
            SetPathData(data);
            SetEventData(data);
        }
    }

    /// <summary>
    /// 重命名之后
    /// </summary>
    protected override void ActionAfterRename()
    {
        RecordLastConfigIndex();
        ReloadAll();
    }

    void OnGUI()
    {
        using (new AutoEditorVertical())
        {
            using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
            {
                // 工具栏
                DrawToolbarView();
            }

            // 是否有编辑内容提示
            if (!HasCurEditConfig())
            {
                EditorGUILayout.HelpBox("请选择或添加配置进行编辑！", MessageType.Warning);
                return;
            }

            // 显示编辑内容
            using (new AutoEditorScrollView(ref m_scrollPos))
            {
                DrawLevelDesign();
            }
        }
    }

    /// <summary>
    /// 绘制编辑器相关
    /// </summary>
    void DrawLevelDesign()
    {
        using (new AutoEditorVertical(EditorStylesEx.BoxArea))
        {
            //刷新按钮
            DrawRefresh();
        }
        using (new AutoEditorVertical(EditorStylesEx.BoxArea))
        {
            //全局事件
            DrawOverallEvent();
        }
        EditorGUILayout.Space();
        using (new AutoEditorVertical(EditorStylesEx.BoxArea))
        {
            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                //逻辑，编辑器由一个个逻辑组成
                DrawLogic();
            }
            EditorGUILayout.Space();
            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                //出生点
                DrawBorn();
            }
            EditorGUILayout.Space();
            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                //刷新器，只显示当前逻辑下的
                DrawSpawn();
            }
            EditorGUILayout.Space();
            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                //点集
                DrawPoint();
            }
            EditorGUILayout.Space();
            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                //区域集
                DrawArea();
            }
            EditorGUILayout.Space();
            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                //路径集
                DrawPath();
            }
            EditorGUILayout.Space();
            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                //局部事件
                DrawEvent();
            }
        }
    }

    /// <summary>
    /// 绘制刷新
    /// </summary>
    void DrawRefresh()
    {
        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Refresh), EditorStyles.toolbarButton))
        {
            GetAllData();
        }
    }

    /// <summary>
    /// 寻找对应的节点
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject RootFind(string name = "")
    {
        GameObject root = GameObject.Find("关卡编辑器");
        if (root == null)
        {
            root = new GameObject("关卡编辑器");
            //root.hideFlags = HideFlags.DontSave;
        }

        if (string.IsNullOrEmpty(name))
            return root;

        Transform child = root.transform.Find(name);
        if (child == null)
        {
            child = (new GameObject()).transform;
            child.name = name;
            child.transform.parent = root.transform;
        }

        return child.gameObject;
    }

    /// <summary>
    /// 寻找一个节点
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject FindNode(string name, Transform parent)
    {
        if (null != parent)
        {
            Transform child = parent.Find(name);
            if(null == child)
            {
                child = (new GameObject()).transform;
                child.name = name;
                child.transform.parent = parent.transform;
            }
            return child.gameObject;
        }
        return null;
    }

    /// <summary>
    /// 创建关卡编辑器的对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    GameObject CreateLevelDesignObj(string prefab, Transform parent)
    {
        Object obj = AssetDatabase.LoadAssetAtPath("Assets/Scripts/Scene/LevelDesignEditor/Res/" + prefab + ".prefab", typeof(Object));
        if(null != obj)
        {
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            if (go != null)
            {
                go.transform.SetParent(parent);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                return go;
            }
            else
                return null;
        }
        return null;
    }

    /// <summary>
    /// 清理数据
    /// </summary>
    public void ClearData()
    {
        GameObject parent = RootFind();
        if (parent != null)
        {
            parent.transform.DestroyChildren();
            DestroyImmediate(parent);
        }
    }

    /// <summary>
    /// 取得场景名字
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public string GetScenePath(string sceneName)
    {
        if (m_allScenes == null)
            GetAllScene();

        for (int i = 0; i < m_allScenes.Count; ++i)
        {
            string path = m_allScenes[i];
            string name = path.Substring(path.LastIndexOf("/") + 1);
            name = name.Substring(0, name.IndexOf("."));
            if (name == sceneName)
                return path;
        }
        return null;
    }

    /// <summary>
    /// 获取所有的场景
    /// </summary>
    static void GetAllScene()
    {
        if (null != m_allScenes)
            m_allScenes.Clear();
        m_allScenes = XTools.Utility.GetAllSceneList();
        m_allScenes.RemoveAll((string path) =>
        {
            return path.EndsWith("main.unity") || path.EndsWith("login.unity");
        });
    }

    /// <summary>
    /// 获取开始的逻辑
    /// </summary>
    /// <returns></returns>
    public LevelDesignConfig.LevelLogicData GetCurSelectData()
    {
        string name = m_curEditConfig.m_curSelectLogic;
        if(HasLogic())
        {
            for(int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
            {
                if(m_curEditConfig.m_levelLogicList[i].m_name == name)
                {
                    return m_curEditConfig.m_levelLogicList[i];
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 是否是开始逻辑
    /// </summary>
    /// <returns></returns>
    public bool IsStartLogic(string name)
    {
        if(name == m_curEditConfig.m_startLevelLogic)
        {
            return true;
        }
        return false;
    }

    public Transform GetLogicTrans(string name)
    {
        GameObject logicParent = RootFind(GetCurEditConfigName());
        return logicParent.transform.Find(name);
    }

    /// <summary>
    /// 获取全局事件的transform
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    public Transform GetOverallEventTrans(string eventId)
    {
        GameObject logicParent = RootFind(GetCurEditConfigName());
        GameObject eventParent = FindNode("全局事件", logicParent.transform);
        return eventParent.transform.Find(eventId);
    }

    /// <summary>
    /// 判断事件是否为全局
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public bool IsOverallEvent(Transform trans)
    {
        Transform parent = trans.parent;
        if(null != parent && parent.name == "全局事件")
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="select"></param>
    public void SetSelect(string select)
    {
        m_curEditConfig.m_curSelectLogic = select;
    }

    public void SetStartLogic(string start)
    {
        m_curEditConfig.m_startLevelLogic = start;
    }

    /// <summary>
    /// 读取csv配置表
    /// </summary>
    public void LoadCsvConfig()
    {
        CsvCommon.CsvLoadKey load = new CsvCommon.CsvLoadKey("./Data/Config/Auto/Game/");
        Config.RoleDefine.Load(load);
    }

    /// <summary>
    /// 重新加载
    /// </summary>
    public void Reload()
    {
        RecordLastConfigIndex();
        ReloadAll();
    }

    /// <summary>
    /// 计算贴地碰撞坐标
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="layerMask"></param>
    /// <param name="rise"></param>
    /// <returns></returns>
    public Vector3 GetColliderPos(Vector3 pos, int layerMask, bool rise)
    {
        //加一点修改，避免检查到下面的碰撞
        pos.y = pos.y + 0.5f;
        Vector3 tempPos = pos + (rise ? Vector3.up * 1000.0f : Vector3.zero);
        int minIndex = 0;
        float minDis = 2000;

        RaycastHit[] hits = Physics.RaycastAll(tempPos, Vector3.down, 2000, layerMask);
        if (null != hits && hits.Length > 0)
        {
            //找到原点下方最近的点
            for (int i = 0; i < hits.Length; ++i)
            {
                float dis = pos.y - hits[i].point.y;
                if (hits[i].point.y < pos.y && minDis > dis)
                {
                    minDis = dis;
                    minIndex = i;
                }
            }

            if (minIndex >= 0 && minIndex < hits.Length)
            {
                return hits[minIndex].point;
            }
        }
        return pos;
    }

    /// <summary>
    /// 排序born的位置
    /// </summary>
    public void SortBorn(LevelDesignConfig.LevelLogicData logic = null)
    {
        List<LevelDesignConfig.LevelBornData> list = (logic == null ? GetCurSelectData() : logic).m_levelBornList;
        if(list != null && list.Count > 0)
        {
            list.Sort(new LevelDesignBornCompare());
        }

        for(int i = 0; i < list.Count; ++i)
        {
            GameObject go = GetBornObj(list[i].m_bornId, logic);
            go.transform.SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// 排序刷新点
    /// </summary>
    public void SortSpawn(LevelDesignConfig.LevelLogicData logic = null)
    {
        List<LevelDesignConfig.LevelSpawnData> list = (logic == null ? GetCurSelectData() : logic).m_levelSpawnList;
        if (list != null && list.Count > 0)
        {
            list.Sort(new LevelDesignSpawnCompare());
        }

        for (int i = 0; i < list.Count; ++i)
        {
            GameObject go = GetSpawnObj(list[i].m_spawnId, logic);
            go.transform.SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// 排序点集
    /// </summary>
    public void SortPoint(LevelDesignConfig.LevelLogicData logic = null)
    {
        List<LevelDesignConfig.LevelPointData> list = (logic == null ? GetCurSelectData() : logic).m_levelPointList;
        if (list != null && list.Count > 0)
        {
            list.Sort(new LevelDesignPointCompare());
        }

        for (int i = 0; i < list.Count; ++i)
        {
            GameObject go = GetPointObj(list[i].m_pointSetId, logic);
            go.transform.SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// 区域排序
    /// </summary>
    public void SortArea(LevelDesignConfig.LevelLogicData logic = null)
    {
        List<LevelDesignConfig.LevelAreaData> list = (logic == null ? GetCurSelectData() : logic).m_levelAreaList;
        if (list != null && list.Count > 0)
        {
            list.Sort(new LevelDesignAreaCompare());
        }

        for (int i = 0; i < list.Count; ++i)
        {
            GameObject go = GetAreaObj(list[i].m_areaSetId, logic);
            go.transform.SetSiblingIndex(i);
        }
    }

    public void SortPath(LevelDesignConfig.LevelLogicData logic = null)
    {
        List<LevelDesignConfig.LevelPathData> list = (logic == null ? GetCurSelectData() : logic).m_levelPathList;
        if (list != null && list.Count > 0)
        {
            list.Sort(new LevelDesignPathCompare());
        }

        for (int i = 0; i < list.Count; ++i)
        {
            GameObject go = GetPathObj(list[i].m_pathId, logic);
            go.transform.SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// 事件排序
    /// </summary>
    public void SortEvent(LevelDesignConfig.LevelLogicData logic = null)
    {
        List<LevelDesignConfig.LevelEventObjData> list = (logic == null ? GetCurSelectData() : logic).m_roomEventList;
        if (list != null && list.Count > 0)
        {
            list.Sort(new LevelDesignEventCompare());
        }

        for (int i = 0; i < list.Count; ++i)
        {
            GameObject go = GetEventObj(list[i].m_eventId, logic);
            go.transform.SetSiblingIndex(i);

            //对action使用延迟来排序
            List<LevelDesignConfig.LevelEventAction> actionList = list[i].m_actions;
            if (actionList != null && actionList.Count > 0)
            {
                actionList.Sort(new LevelDesignActionCompare());
            }
        }
    }
    
    /// <summary>
    /// 全局事件排序
    /// </summary>
    public void SortOverallEvent()
    {
        List<LevelDesignConfig.LevelEventObjData> list = m_curEditConfig.m_overalEventList;
        if (list != null && list.Count > 0)
        {
            list.Sort(new LevelDesignEventCompare());
        }

        for (int i = 0; i < list.Count; ++i)
        {
            GameObject go = GetOverallEventObj(list[i].m_eventId);
            go.transform.SetSiblingIndex(i);

            //对action使用延迟来排序
            List<LevelDesignConfig.LevelEventAction> actionList = list[i].m_actions;
            if (actionList != null && actionList.Count > 0)
            {
                actionList.Sort(new LevelDesignActionCompare());
            }
        }
    }

    /// <summary>
    /// 房间逻辑排序
    /// </summary>
    public void SortLogic()
    {
        List<LevelDesignConfig.LevelLogicData> list = m_curEditConfig.m_levelLogicList;
        if (list != null && list.Count > 0)
        {
            list.Sort(new LevelDesignLogicCompare());
        }

        for (int i = 0; i < list.Count; ++i)
        {
            GameObject go = GetLogicObj(list[i].m_name);
            go.transform.SetSiblingIndex(i);
        }
    }
}

#region 排序类集合
/// <summary>
/// 出生点排序类
/// </summary>
public class LevelDesignBornCompare : IComparer<LevelDesignConfig.LevelBornData>
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int StrCmpLogicalW(string x, string y);


    public int Compare(LevelDesignConfig.LevelBornData x, LevelDesignConfig.LevelBornData y)
    {
        return StrCmpLogicalW(x.m_bornId, y.m_bornId);
    }
}

/// <summary>
/// 刷新点排序类
/// </summary>
public class LevelDesignSpawnCompare : IComparer<LevelDesignConfig.LevelSpawnData>
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int StrCmpLogicalW(string x, string y);


    public int Compare(LevelDesignConfig.LevelSpawnData x, LevelDesignConfig.LevelSpawnData y)
    {
        return StrCmpLogicalW(x.m_spawnId, y.m_spawnId);
    }
}

/// <summary>
/// 点集排序类
/// </summary>
public class LevelDesignPointCompare : IComparer<LevelDesignConfig.LevelPointData>
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int StrCmpLogicalW(string x, string y);


    public int Compare(LevelDesignConfig.LevelPointData x, LevelDesignConfig.LevelPointData y)
    {
        return StrCmpLogicalW(x.m_pointSetId, y.m_pointSetId);
    }
}

/// <summary>
/// 区域集排序类
/// </summary>
public class LevelDesignAreaCompare : IComparer<LevelDesignConfig.LevelAreaData>
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int StrCmpLogicalW(string x, string y);


    public int Compare(LevelDesignConfig.LevelAreaData x, LevelDesignConfig.LevelAreaData y)
    {
        return StrCmpLogicalW(x.m_areaSetId, y.m_areaSetId);
    }
}

/// <summary>
/// 路径排序类
/// </summary>
public class LevelDesignPathCompare : IComparer<LevelDesignConfig.LevelPathData>
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int StrCmpLogicalW(string x, string y);


    public int Compare(LevelDesignConfig.LevelPathData x, LevelDesignConfig.LevelPathData y)
    {
        return StrCmpLogicalW(x.m_pathId, y.m_pathId);
    }
}

/// <summary>
/// 事件排序类
/// </summary>
public class LevelDesignEventCompare : IComparer<LevelDesignConfig.LevelEventObjData>
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int StrCmpLogicalW(string x, string y);


    public int Compare(LevelDesignConfig.LevelEventObjData x, LevelDesignConfig.LevelEventObjData y)
    {
        return StrCmpLogicalW(x.m_eventId, y.m_eventId);
    }
}

public class LevelDesignActionCompare : IComparer<LevelDesignConfig.LevelEventAction>
{
    public int Compare(LevelDesignConfig.LevelEventAction x, LevelDesignConfig.LevelEventAction y)
    {
        return x.m_delay.CompareTo(y.m_delay);
    }
}

/// <summary>
/// 逻辑排序类
/// </summary>
public class LevelDesignLogicCompare : IComparer<LevelDesignConfig.LevelLogicData>
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int StrCmpLogicalW(string x, string y);


    public int Compare(LevelDesignConfig.LevelLogicData x, LevelDesignConfig.LevelLogicData y)
    {
        return StrCmpLogicalW(x.m_name, y.m_name);
    }
}
#endregion