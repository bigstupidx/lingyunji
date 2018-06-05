// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TaskTracDefine 
    {
        static List<TaskTracDefine> DataList = new List<TaskTracDefine>();
        static public List<TaskTracDefine> GetAll()
        {
            return DataList;
        }

        static public TaskTracDefine Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("TaskTracDefine.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<TaskTracDefine>> DataList_key = new Dictionary<int, List<TaskTracDefine>>();

        static public Dictionary<int, List<TaskTracDefine>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<TaskTracDefine> GetGroupBykey(int key)
        {
            List<TaskTracDefine> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TaskTracDefine.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 任务编号
        public int key { get; set; }

        // 任务需求-物品
        public int tracItemId { get; set; }

        // 追踪目标-npc
        public int tracRoleId { get; set; }

        // 追踪目标-场景触发器
        public string tarcLevel { get; set; }

        // 追踪目标-怪物
        public string tracRole { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TaskTracDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TaskTracDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TaskTracDefine> allDatas = new List<TaskTracDefine>();

            {
                string file = "Task/TaskTracDefine#Task.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int tracItemId_index = reader.GetIndex("tracItemId");
                int tracRoleId_index = reader.GetIndex("tracRoleId");
                int tarcLevel_index = reader.GetIndex("tarcLevel");
                int tracRole_index = reader.GetIndex("tracRole");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskTracDefine data = new TaskTracDefine();
						data.key = reader.getInt(i, key_index, 0);         
						data.tracItemId = reader.getInt(i, tracItemId_index, 0);         
						data.tracRoleId = reader.getInt(i, tracRoleId_index, 0);         
						data.tarcLevel = reader.getStr(i, tarcLevel_index);         
						data.tracRole = reader.getStr(i, tracRole_index);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            
            DataList = allDatas;

            foreach (var data in allDatas)
            {
                {
                    List<TaskTracDefine> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<TaskTracDefine>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(TaskTracDefine);
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


