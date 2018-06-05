// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TeamActionNotification 
    {
        static List<TeamActionNotification> DataList = new List<TeamActionNotification>();
        static public List<TeamActionNotification> GetAll()
        {
            return DataList;
        }

        static public TeamActionNotification Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].ActionId == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("TeamActionNotification.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].ActionId == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<TeamActionNotification>> DataList_ActionId = new Dictionary<int, List<TeamActionNotification>>();

        static public Dictionary<int, List<TeamActionNotification>> GetAllGroupByActionId()
        {
            return DataList_ActionId;
        }

        static public List<TeamActionNotification> GetGroupByActionId(int key)
        {
            List<TeamActionNotification> value = null;
            if (DataList_ActionId.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TeamActionNotification.GetGroupByActionId({0}) not find!", key);
            return null;
        }


        // 行为
        public int ActionId { get; set; }

        // 行为结果
        public int teamError { get; set; }

        // 提示语
        public string tipKey { get; set; }

        // 提示语
        public string msg { get; set; }

        // 开关
        public bool isOpen { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TeamActionNotification);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_ActionId.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TeamActionNotification);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TeamActionNotification> allDatas = new List<TeamActionNotification>();

            {
                string file = "Team/TeamActionNotification.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int ActionId_index = reader.TryIndex("ActionId:group");
                int teamError_index = reader.GetIndex("teamError");
                int tipKey_index = reader.GetIndex("tipKey");
                int msg_index = reader.GetIndex("msg");
                int isOpen_index = reader.GetIndex("isOpen");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TeamActionNotification data = new TeamActionNotification();
						data.ActionId = reader.getInt(i, ActionId_index, 0);         
						data.teamError = reader.getInt(i, teamError_index, 0);         
						data.tipKey = reader.getStr(i, tipKey_index);         
						data.msg = reader.getStr(i, msg_index);         
						data.isOpen = reader.getBool(i, isOpen_index, false);         
                        if (lineParseMethod != null)
                            lineParseMethod.Invoke(null, new object[3] {data, reader, i });
                        allDatas.Add(data);
                    }
                    catch (System.Exception ex)
                    {
                        CsvCommon.Log.Error("file:{0} line:{1} error!", file, i);
                        CsvCommon.Log.Exception(ex);
                    }
                }
            }
            
            DataList = allDatas;

            foreach (var data in allDatas)
            {
                {
                    List<TeamActionNotification> l = null;
                    if (!DataList_ActionId.TryGetValue(data.ActionId, out l))
                    {
                        l = new List<TeamActionNotification>();
                        DataList_ActionId[data.ActionId] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(TeamActionNotification);
                    while (null != curType)
                    {
                        method = curType.GetMethod("OnLoadEnd", BindingFlags.Static | BindingFlags.NonPublic);
                        if (null != method)
                            break;
                        curType = curType.BaseType;
                    }
                }
                if (method != null)
                    method.Invoke(null, new object[0]);
            }
        }
    }
}


