// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TaskOptTipsDefine 
    {
        static Dictionary<int, TaskOptTipsDefine> DataList = new Dictionary<int, TaskOptTipsDefine>();

        static public Dictionary<int, TaskOptTipsDefine> GetAll()
        {
            return DataList;
        }

        static public TaskOptTipsDefine Get(int key)
        {
            TaskOptTipsDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TaskOptTipsDefine.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 操作说明
        public string desc { get; set; }

        // 操作图片
        public string icon { get; set; }

        // 触发事件
        public int eventId { get; set; }

        // 触发类型
        public int type { get; set; }

        // 触发条件
        public string condition { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TaskOptTipsDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TaskOptTipsDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TaskOptTipsDefine> allDatas = new List<TaskOptTipsDefine>();

            {
                string file = "Task/TaskOptTipsDefine#Task.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int desc_index = reader.GetIndex("desc");
                int icon_index = reader.GetIndex("icon");
                int eventId_index = reader.GetIndex("eventId");
                int type_index = reader.GetIndex("type");
                int condition_index = reader.GetIndex("condition");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskOptTipsDefine data = new TaskOptTipsDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.desc = reader.getStr(i, desc_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.eventId = reader.getInt(i, eventId_index, 0);         
						data.type = reader.getInt(i, type_index, 0);         
						data.condition = reader.getStr(i, condition_index);         
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
                    CsvCommon.Log.Error("TaskOptTipsDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TaskOptTipsDefine);
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


