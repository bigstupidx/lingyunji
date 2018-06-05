using UnityEditor;
using UnityEngine;
using EditorExtensions;
using System.IO;
using CommonBase;
using System.Collections.Generic;
using System;

/// <summary>
/// 关卡编辑器的xml转json工具
/// </summary>
public class LevelXmlToJson : EditorWindow
{
    static public LevelXmlToJson Instance { get; protected set; }
    string m_path;
    string m_name;

    [MenuItem("EBEditor/关卡编辑器xml文件转json")]
    static void ChangeLevelFile()
    {
        Instance = GetWindow<LevelXmlToJson>(false, "关卡编辑器", true);
        Instance.minSize = new Vector2(640.0f, 480.0f);

        Instance.titleContent.image = EditorIconContent.GetSystem(EditorIconContentType.TextAsset_Icon).image;
    }

    void OnEnable()
    {
        Instance = this;
        m_path = "";
    }

    void OnDisable()
    {
        Instance = null;
    }

    void OnGUI()
    {

        if (GUILayout.Button("选择xml文件夹", GUILayout.Height(30)))
        {
            m_path = EditorUtility.OpenFolderPanel("请找到xml的路径", "", "");
        }

        if(!string.IsNullOrEmpty(m_path))
        {
            GUILayout.Label("xml文件路径 : " + m_path);

            if (GUILayout.Button("开始转换", GUILayout.Height(30)))
            {
                string saveDir = EditorUtility.SaveFolderPanel("保存关卡配置文件", "", "");
                Debuger.Log("保存的路径 : " + saveDir);
                //解析xml
                ParseXml(m_path, saveDir);
            }
        }

        if(GUILayout.Button("选择单个文件", GUILayout.Height(30)))
        {
            m_name = EditorUtility.OpenFilePanel("请找到xml路径", "", "xml");
        }
        if(!string.IsNullOrEmpty(m_name))
        {
            GUILayout.Label("xml文件路径 : " + m_name);
            if(GUILayout.Button("开始转换单个文件", GUILayout.Height(30)))
            {
                string saveDir = EditorUtility.SaveFolderPanel("保存关卡配置文件", "", "");
                Debuger.Log("保存的路径 : " + saveDir);
                FileInfo fileInfo = new FileInfo(m_name);
                SaveFile(fileInfo, saveDir);
            }
        }
    }

    /// <summary>
    /// 解析xml
    /// </summary>
    /// <param name="m_path"></param>
    void ParseXml(string m_path, string saveDir)
    {
        DirectoryInfo rootDirInfo = new DirectoryInfo(m_path);
        foreach (FileInfo fileInfo in rootDirInfo.GetFiles("*.xml", SearchOption.AllDirectories))
        {
            //取出所有的xml文件，并获取相对路径
            if(!SaveFile(fileInfo, saveDir))
            {
                continue;
            }
        }
    }

    bool SaveFile(FileInfo fileInfo, string saveDir)
    {
        if (!System.IO.File.Exists(fileInfo.FullName))
        {
            Debuger.LogError("不存在文件 " + fileInfo.FullName);
            return false;
        }
        Stream stream = new MemoryStream(System.IO.File.ReadAllBytes(fileInfo.FullName));
        if (null == stream)
        {
            Debuger.LogError("GetLevelLogicConfig:" + fileInfo.FullName + " error!");
            return false;
        }
        LevelDesignConfig config = new LevelDesignConfig();
        SceneLogicHandler handler = new SceneLogicHandler(config);
        //解析xml
        //Debuger.LogWarning("fileInfo.FullName : " + fileInfo.FullName);
        try
        {
            XMLParser.parseXMLFile(handler, stream);
            //另存为json
            string json = JsonUtility.ToJson(config);
            string savePath = saveDir + "/" + fileInfo.Name.Substring(0, fileInfo.Name.IndexOf(".xml")) + ".json";
            //Debuger.Log("savePath : " + savePath);
            File.WriteAllText(savePath, json, System.Text.Encoding.UTF8);
        }
        catch (Exception e)
        {
            Debuger.LogError(fileInfo.FullName + "  这个文件有问题 ！！！");
        }
        return true;
    }
}

#region 用来读取旧的xml
public class SceneLogicHandler : XMLHandlerReg
{
    LevelDesignConfig config;
    LevelDesignConfig.LevelLogicData logicData;
    LevelDesignConfig.LevelEventObjData eventData;

    public SceneLogicHandler(LevelDesignConfig config)
    {
        this.config = config;
        Init();
    }

    public void Init()
    {
        //之前没有房间的概念，需要new一个房间
        logicData = new LevelDesignConfig.LevelLogicData();
        logicData.m_name = "room";
        config.m_startLevelLogic = "room";
        config.m_curSelectLogic = "room";
        config.m_levelLogicList.Add(logicData);

        RegElementStart("SceneName", elementStart_Scene);
        RegElementStart("BornData", elementStart_BornList);

        // 刷新点数据
        RegElementStart("Refresh", elementStart_Refresh);
        //点集数据
        RegElementStart("Point", elementStart_Point);
        //区域集数据
        RegElementStart("Area", elementStart_Area);
        RegElementStart("ActionCondition", elementStart_ActionCondition);
        RegElementStart("EventGroup", elementStart_EventGroup);
        RegElementEnd("EventGroup", elementEnd_EventGroup);
        // 触发的行为
        RegElementStart("ActionEvent", elementStart_ActionEvent);
    }


    void elementEnd_EventGroup(string element)
    {
        if (eventData == null)
        {
            Debuger.LogError("elementEnd_EventGroup CurrentCheck == null");
            return;
        }

        if (eventData.m_conditions.Count == 0)
        {
            Debuger.LogError(string.Format("elementEnd_EventGroup name:{0} 事件列表为空!", eventData.m_eventId));
            logicData.m_roomEventList.Remove(eventData);
            eventData = null;
            return;
        }

        eventData = null;
    }

    void elementStart_ActionEvent(string element, XMLAttributes attributes)
    {
        LevelDesignConfig.LevelEventAction action = new LevelDesignConfig.LevelEventAction();

        if (!StringToEnum<LevelDesignConfig.ActionType>(attributes.getValue("type"), out action.m_actionType))
        {
            return;
        }

        action.m_param1 = attributes.getValueAsString("param1");
        action.m_param2 = attributes.getValueAsString("param2");
        action.m_param3 = attributes.getValueAsString("param3");
        action.m_param4 = attributes.getValueAsString("param4");
        action.m_param5 = attributes.getValueAsString("param5");
        action.m_delay = attributes.getValueAsFloat("delay", 0.0f);
        eventData.m_actions.Add(action);
    }

    void elementStart_ActionCondition(string element, XMLAttributes attributes)
    {
        LevelDesignConfig.LevelEventCondition condition = new LevelDesignConfig.LevelEventCondition();

        if (!StringToEnum<LevelDesignConfig.ConditionType>(attributes.getValue("type"), out condition.m_conditionType))
        {
            Debuger.LogError("elementStart_ActionEvent: error!" + attributes.getValue("type"));
            return;
        }

        condition.m_param1 = attributes.getValueAsString("param1");
        condition.m_param2 = attributes.getValueAsString("param2");
        condition.m_param3 = attributes.getValueAsString("param3");
        condition.m_param4 = attributes.getValueAsString("param4");
        condition.m_param5 = attributes.getValueAsString("param5");
        condition.m_isOr = attributes.getValueAsBool("isOr", false);
        eventData.m_conditions.Add(condition);
    }

    void elementStart_EventGroup(string element, XMLAttributes attributes)
    {
        eventData = new LevelDesignConfig.LevelEventObjData();
        logicData.m_roomEventList.Add(eventData);

        eventData.m_eventId = attributes.getValue("name");
        //CurrentCheck.isWork = attributes.getValueAsBool("work", true);
        //currentActionList = CurrentCheck.actionList;
        //currentConditionList = CurrentCheck.conditionList;
    }

    void elementStart_Scene(string element, XMLAttributes attributes)
    {
        logicData.m_scene = attributes.getValueAsString("sceneName", "");
        logicData.m_sceneStyle = attributes.getValueAsString("sceneStyle", "");
    }

    /// <summary>
    /// 读取出生点信息
    /// </summary>
    void elementStart_BornList(string element, XMLAttributes attributes)
    {
        LevelDesignConfig.LevelBornData data = new LevelDesignConfig.LevelBornData();
        data.m_pos = StringToVector3(attributes.getValue("pos"), Vector3.zero);
        data.m_dir = StringToVector3(attributes.getValue("dir"), Vector3.zero);
        data.m_bornId = attributes.getValueAsString("id", "0");
        logicData.m_levelBornList.Add(data);
    }

    void elementStart_Refresh(string element, XMLAttributes attributes)
    {
        LevelDesignConfig.LevelSpawnData spawn = new LevelDesignConfig.LevelSpawnData();
        spawn.m_spawnId = attributes.getValue("id");
        logicData.m_levelSpawnList.Add(spawn);

        //spawn.m_refreshType = attributes.GetEnum("refreshType", SceneLogicConfig.Refresh.RefreshType.Point);
        //spawn.isBatch = attributes.getValueAsBool("isBatch", false);
        spawn.m_isRandomSpawn = attributes.getValueAsBool("isRandomPoint", false);
        spawn.m_lookToPlayer = attributes.getValueAsBool("isFaceMainRole", false);
        spawn.m_isToGround = attributes.getValueAsBool("isSaveToGround2", true);
        spawn.backToBorn = attributes.getValueAsBool("isGobackBorn", true);
        //spawn.isTransmitStone = attributes.getValueAsBool("isTransmitStone", false);
        //spawn.transmitOffset = attributes.getValueAsFloat("transmitOffset", 5);
        spawn.m_name = attributes.getValueAsString("refreshName", "");
        spawn.m_spawnGroupId = attributes.getValueAsString("groupId", "");
        //spawn.level = attributes.getValueAsString("level", "");
        //spawn.isPartnerAutoAttack = attributes.getValueAsBool("isPartnerAutoAttack", false);
        spawn.fullHp = attributes.getValueAsBool("isFullHp", true);
        spawn.m_isPassivity = attributes.getValueAsBool("isPassive", false);
        //refresh.team = attributes.GetEnum("team", SceneLogicConfig.Team.Enemy);
        spawn.m_postions = StringToVector3s(attributes.getValue("pos"), Vector3.zero);
        spawn.m_dirs = StringToVector3s(attributes.getValue("dir"), Vector3.zero);
        spawn.m_scales = StringToVector3s(attributes.getValue("scale"), Vector3.one);
        spawn.m_names = String2StringList(attributes.getValue("names"));
        //if (spawn.m_spawnType == LevelDesignConfig.LevelSpawnData.SpawnType.Area)
        //{
        //    spawn.types = String2TList(attributes.getValue("type"), SceneLogicConfig.AreaType.Rect);
        //    spawn.functionTypes = String2TList(attributes.getValue("functiontype"), SceneLogicConfig.AreaFunctionType.Normal);
        //}
        spawn.m_fieldOfVision = attributes.getValueAsFloat("fieldOfVision", -1f);
        //spawn.m_aiId = attributes.getValueAsString("aiId", "0");
        //refresh.m_bornEffect = attributes.getValueAsString("bornEffect", "0");
        spawn.m_startNum = attributes.getValueAsInteger("startNum", 0);
        spawn.m_maxNum = attributes.getValueAsInteger("maxNum", -1);
        spawn.m_survivalLimit = attributes.getValueAsInteger("lifeTotal", -1);
        //refresh.skillID = attributes.getValueAsString("skillID", "0");
        spawn.m_startCd = attributes.getValueAsFloat("bornCd", 0);
        spawn.m_patrolId = attributes.getValueAsString("patrol", "");
        spawn.m_bornAction = attributes.getValueAsString("bornAnimation", "");
        spawn.m_enterBattleAction = attributes.getValueAsString("beginBattleAnimation", "");
        spawn.m_defaultIdle = attributes.getValueAsString("defaultIdleAnimation", "");
        spawn.m_bornBubb = attributes.getValueAsString("bornBubbling", "");
        spawn.m_enterBattleBubb = attributes.getValueAsString("BeginBattleBubbling", "");
        spawn.m_enterBattleSigh = attributes.getValueAsBool("BeginBattleTip", false);
        spawn.m_randomIdle = attributes.getValueAsString("relaxPerformance", "");
        //spawn.m_battlePerformance = attributes.getValueAsString("battlePerformance", "");
        spawn.m_autoPatrol = attributes.getValueAsString("m_autoPatrol", "");
        //refresh.m_roleId = attributes.getValueAsInteger("roleId", 0);
        spawn.m_objs = new List<int>();
        string[] npcs = attributes.getValue("npcid").Split(':');
        for(int i = 0; i < npcs.Length; ++i)
        {
            spawn.m_objs.Add(StringToInt(npcs[i], 0));
        }
        //解析出生行为
        //spawn.m_refreshBornList = new List<SceneLogicConfig.RefreshActionItem>();
        //if (!string.IsNullOrEmpty(attributes.getValue("bornAction")))
        //{
        //    string[] bornList = attributes.getValue("bornAction").Split(';');
        //    for (int i = 0; i < bornList.Length; ++i)
        //    {
        //        if (bornList[i].Contains("|"))
        //        {
        //            SceneLogicConfig.RefreshActionItem born = new SceneLogicConfig.RefreshActionItem();
        //            born.m_action = Str2Enum.To(bornList[i].Split('|')[0], SceneLogicConfig.RefreshAction.BornSkill);
        //            born.m_parameter = bornList[i].Split('|')[1];

        //            spawn.m_refreshBornList.Add(born);
        //        }
        //    }
        //}
        ////解析战斗触发行为
        //spawn.m_refreshBattleActionList = new List<SceneLogicConfig.RefreshActionItem>();
        //if (!string.IsNullOrEmpty(attributes.getValue("battleAction")))
        //{
        //    string[] bornList = attributes.getValue("battleAction").Split(';');
        //    for (int i = 0; i < bornList.Length; ++i)
        //    {
        //        if (bornList[i].Contains("|"))
        //        {
        //            SceneLogicConfig.RefreshActionItem born = new SceneLogicConfig.RefreshActionItem();
        //            born.m_action = Str2Enum.To(bornList[i].Split('|')[0], SceneLogicConfig.RefreshAction.BornSkill);
        //            born.m_parameter = bornList[i].Split('|')[1];

        //            spawn.m_refreshBattleActionList.Add(born);
        //        }
        //    }
        //}
        //解析刷新点初始仇恨
        //spawn.m_hatredList = new List<SceneLogicConfig.RefreshHatred>();
        //if (!string.IsNullOrEmpty(attributes.getValue("hatredList")))
        //{
        //    string[] list = attributes.getValue("hatredList").Split(';');
        //    for (int i = 0; i < list.Length; ++i)
        //    {
        //        if (list[i].Contains("|"))
        //        {
        //            SceneLogicConfig.RefreshHatred hatred = new SceneLogicConfig.RefreshHatred();
        //            hatred.refreshId = list[i].Split('|')[0];
        //            hatred.startValue = int.Parse(list[i].Split('|')[1]);

        //            spawn.m_hatredList.Add(hatred);
        //        }
        //    }
        //}

        while (spawn.m_dirs.Count < spawn.m_postions.Count)
            spawn.m_dirs.Add(Vector3.zero);
        while (spawn.m_scales.Count < spawn.m_postions.Count)
            spawn.m_scales.Add(Vector3.one);

        if (!string.IsNullOrEmpty(attributes.getValue("condition")))
        {
            string[] cons = attributes.getValue("condition").Split(';');
            if (cons.Length == 0)
            {
                Debuger.Log(string.Format("condition:{0} error!", attributes.getValue("condition")));
            }
            else
            {
                spawn.m_spawnType = (LevelDesignConfig.LevelSpawnData.SpawnType)StringToInt(cons[0], 0);
                spawn.m_spawnParam1 = (cons.Length >= 2 ? StringToFloat(cons[1], 0f) : 0f);
                spawn.m_spawnParam2 = (cons.Length >= 3 ? StringToInt(cons[2], 0) : 0);
                spawn.m_spawnParam3 = (cons.Length >= 4 ? StringToInt(cons[3], 0) : 0);
            }
        }

    }

    //点集
    void elementStart_Point(string element, XMLAttributes attributes)
    {
        LevelDesignConfig.LevelPointData point = new LevelDesignConfig.LevelPointData();
        point.m_pointSetId = attributes.getValueAsString("id", "0");
        point.m_saveToGround = attributes.getValueAsBool("saveToPoint", true);
        point.m_postions = StringToVector3s(attributes.getValue("pos"), Vector3.zero);
        point.m_dirs = StringToVector3s(attributes.getValue("dir"), Vector3.zero);
        //point.scales = StringToVector3s(attributes.getValue("scale"), Vector3.one);
        point.m_names = String2StringList(attributes.getValue("name"));
        logicData.m_levelPointList.Add(point);
    }

    //区域集
    void elementStart_Area(string element, XMLAttributes attributes)
    {
        LevelDesignConfig.LevelAreaData area = new LevelDesignConfig.LevelAreaData();
        area.m_areaSetId = attributes.getValueAsString("id", "0");
        area.m_isInitOpen = attributes.getValueAsBool("isInitOpen", true);
        area.m_postions = StringToVector3s(attributes.getValue("pos"), Vector3.zero);
        area.m_dirs = StringToVector3s(attributes.getValue("dir"), Vector3.zero);
        area.m_scales = StringToVector3s(attributes.getValue("scale"), Vector3.one);
        area.m_names = String2StringList(attributes.getValue("name"));
        area.m_types = String2EnumList(attributes.getValue("type"), LevelDesignConfig.LevelAreaData.AreaType.Rect);
        //set.functionTypes = String2TList(attributes.getValue("functiontype"), SceneLogicConfig.AreaFunctionType.Normal);
        area.m_centers = StringToVector3s(attributes.getValue("center"), Vector3.zero);
        area.m_sizes = StringToVector3sWithName(attributes.getValue("size"), Vector3.zero);
        area.m_radiuses = String2FloatList(attributes.getValue("radius"));
        logicData.m_levelAreaList.Add(area);
    }

    public List<Vector3> StringToVector3s(string text, Vector3 def)
    {
        List<Vector3> ress = new List<Vector3>();
        if (string.IsNullOrEmpty(text))
        {
            ress.Add(def);
            return ress;
        }

        string[] poss = text.Split(':');
        foreach (string pos in poss)
        {
            ress.Add(StringToVector3(pos, def));
        }

        return ress;
    }

    public List<Vector3> StringToVector3sWithName(string text, Vector3 def)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new List<Vector3>();
        }

        List<Vector3> list = new List<Vector3>();
        string[] vectors = text.Split(':');
        foreach (string vector in vectors)
        {
            string[] values = vector.Split('|');
            list.Add(StringToVector3(values[1], def));
        }
        return list;
    }

    public Vector3 StringToVector3(string text, Vector3 def)
    {
        if (string.IsNullOrEmpty(text))
            return def;

        string[] pos = text.Split(';');
        if (pos.Length != 3)
            return def;

        Vector3 v = Vector3.zero;
        if (float.TryParse(pos[0], out v.x) == false ||
            float.TryParse(pos[1], out v.y) == false ||
            float.TryParse(pos[2], out v.z) == false)
        {
            return def;
        }

        return v;
    }

    public static int StringToInt(string text, int def)
    {
        int v;
        if (int.TryParse(text, out v))
            return v;

        return def;
    }

    public static float StringToFloat(string text, float def)
    {
        float v;
        if (float.TryParse(text, out v))
            return v;

        return def;
    }

    protected static string GetDepth(int depth)
    {
        string s = "";
        while ((depth--) > 0)
        {
            s += "  ";
        }

        return s;
    }

    static string PositionToString(List<Vector3> poss, Vector3 def)
    {
        if (poss.Count == 0)
        {
            return "";
        }
        else if (poss.Count == 1)
        {
            if (poss[0] == def)
                return "";

            return string.Format("{0};{1};{2}", poss[0].x, poss[0].y, poss[0].z);
        }
        else
        {
            string text;
            if (poss[0] == def)
                text = "";
            else
                text = string.Format("{0};{1};{2}", poss[0].x, poss[0].y, poss[0].z);

            for (int i = 1; i < poss.Count; ++i)
            {
                if (poss[i] == def)
                    text += ":";
                else
                    text += string.Format(":{0};{1};{2}", poss[i].x, poss[i].y, poss[i].z);
            }

            return text;
        }
    }

    static string StringList2String(List<string> str)
    {
        if (str.Count == 0)
        {
            return "";
        }
        string returnStr = str[0];
        for (int i = 1; i < str.Count; ++i)
        {
            returnStr += (":" + str[i]);
        }
        return returnStr;
    }

    static List<string> String2StringList(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return new List<string>();
        }
        List<string> list = new List<string>();
        string[] strs = str.Split(':');
        foreach (string item in strs)
        {
            list.Add(item);
        }

        return list;
    }

    static string EnumList2String<T>(List<T> enums)
    {
        if (enums.Count == 0)
        {
            return "";
        }
        string returnStr = enums[0].ToString();
        for (int i = 1; i < enums.Count; ++i)
        {
            returnStr += (":" + enums[i].ToString());
        }
        return returnStr;
    }

    static List<T> String2EnumList<T>(string str, T def)
    {
        if (string.IsNullOrEmpty(str))
        {
            return new List<T>();
        }
        List<T> list = new List<T>();
        string[] strs = str.Split(':');
        foreach (string item in strs)
        {
            T returnT = StringToEnum<T>(item, def);
            list.Add(returnT);
        }

        return list;
    }

    static List<float> String2FloatList(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return new List<float>();
        }
        List<float> list = new List<float>();
        string[] strs = str.Split(':');
        foreach(string item in strs)
        {
            string[] values = item.Split('|');
            float result = StringToFloat(values[1], 0f);
            list.Add(result);
        }

        return list;
    }

    static string DicVector2String(Dictionary<string, Vector3> dic)
    {
        if (0 == dic.Count)
        {
            return "";
        }
        int index = 0;
        string returnStr = "";
        foreach (KeyValuePair<string, Vector3> pair in dic)
        {
            string str = pair.Key + "|" + string.Format("{0};{1};{2}", pair.Value.x, pair.Value.y, pair.Value.z);
            if (0 == index)
            {
                returnStr += str;
            }
            else
            {
                returnStr += (":" + str);
            }
            index++;
        }
        return returnStr;
    }

    Dictionary<string, Vector3> String2DicVector(string str, Vector3 def)
    {
        if (string.IsNullOrEmpty(str))
        {
            return new Dictionary<string, Vector3>();
        }
        Dictionary<string, Vector3> returnDic = new Dictionary<string, Vector3>();
        string[] vectors = str.Split(':');
        foreach (string vector in vectors)
        {
            string[] values = vector.Split('|');
            returnDic.Add(values[0], StringToVector3(values[1], def));
        }
        return returnDic;
    }

    static string DicFloat2String(Dictionary<string, float> dic)
    {
        if (0 == dic.Count)
        {
            return "";
        }
        int index = 0;
        string returnStr = "";
        foreach (KeyValuePair<string, float> pair in dic)
        {
            string str = pair.Key + "|" + pair.Value;
            if (0 == index)
            {
                returnStr += str;
            }
            else
            {
                returnStr += (":" + str);
            }
            index++;
        }
        return returnStr;
    }

    Dictionary<string, float> String2DicFloat(string str, float def)
    {
        if (string.IsNullOrEmpty(str))
        {
            return new Dictionary<string, float>(); ;
        }
        Dictionary<string, float> returnDic = new Dictionary<string, float>();
        string[] vectors = str.Split(':');
        foreach (string vector in vectors)
        {
            string[] values = vector.Split('|');
            returnDic.Add(values[0], StringToFloat(values[1], def));
        }
        return returnDic;
    }

    static string Concat(char c, List<string> ss)
    {
        if (ss.Count == 0)
        {
            return "";
        }
        else if (ss.Count == 1)
        {
            return ss[0];
        }
        else
        {
            string text = ss[0];
            for (int i = 1; i < ss.Count; ++i)
            {
                text += c;
                text += ss[i];
            }

            return text;
        }
    }

    static string PositionToString(Vector3 pos, Vector3 def)
    {
        if (pos == def)
            return "";

        return string.Format("{0};{1};{2}", pos.x, pos.y, pos.z);
    }

    static string Vector3ToString(Vector3 v3)
    {
        return string.Format("{0};{1};{2}", v3.x, v3.y, v3.z);
    }

    public static bool StringToEnum<T>(string str, out T v)
    {
        return Str2Enum.To<T>(str, out v);
    }

    public static T StringToEnum<T>(string str, T def)
    {
        return Str2Enum.To<T>(str, def);
    }
}
#endregion