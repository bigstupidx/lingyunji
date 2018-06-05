// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TaskCountDefine 
    {
        static Dictionary<int, TaskCountDefine> DataList = new Dictionary<int, TaskCountDefine>();

        static public Dictionary<int, TaskCountDefine> GetAll()
        {
            return DataList;
        }

        static public TaskCountDefine Get(int key)
        {
            TaskCountDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TaskCountDefine.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 计数名称
        public string countDesc { get; set; }

        // 计数数量
        public int count { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TaskCountDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TaskCountDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TaskCountDefine> allDatas = new List<TaskCountDefine>();

            {
                string file = "Task/TaskCountDefine#PY.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int countDesc_index = reader.GetIndex("countDesc");
                int count_index = reader.GetIndex("count");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskCountDefine data = new TaskCountDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.countDesc = reader.getStr(i, countDesc_index);         
						data.count = reader.getInt(i, count_index, 0);         
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
            {
                string file = "Task/TaskCountDefine#Task.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int countDesc_index = reader.GetIndex("countDesc");
                int count_index = reader.GetIndex("count");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskCountDefine data = new TaskCountDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.countDesc = reader.getStr(i, countDesc_index);         
						data.count = reader.getInt(i, count_index, 0);         
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
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("TaskCountDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TaskCountDefine);
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


