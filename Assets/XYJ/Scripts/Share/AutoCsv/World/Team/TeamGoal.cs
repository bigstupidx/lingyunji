// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TeamGoal 
    {
        static Dictionary<int, TeamGoal> DataList = new Dictionary<int, TeamGoal>();

        static public Dictionary<int, TeamGoal> GetAll()
        {
            return DataList;
        }

        static public TeamGoal Get(int key)
        {
            TeamGoal value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TeamGoal.Get({0}) not find!", key);
            return null;
        }



        // 目标ID
        public int id { get; set; }

        // 最小等级
        public int minLevel { get; set; }

        // 最大等级
        public int maxLevel { get; set; }

        // 名字
        public string name { get; set; }

        // 描述
        public string desc { get; set; }

        // 是否显示前往任务按钮
        public bool isShowGoTaskBtn { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TeamGoal);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TeamGoal);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TeamGoal> allDatas = new List<TeamGoal>();

            {
                string file = "Team/TeamGoal.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int minLevel_index = reader.GetIndex("minLevel");
                int maxLevel_index = reader.GetIndex("maxLevel");
                int name_index = reader.GetIndex("name");
                int desc_index = reader.GetIndex("desc");
                int isShowGoTaskBtn_index = reader.GetIndex("isShowGoTaskBtn");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TeamGoal data = new TeamGoal();
						data.id = reader.getInt(i, id_index, 0);         
						data.minLevel = reader.getInt(i, minLevel_index, 0);         
						data.maxLevel = reader.getInt(i, maxLevel_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.desc = reader.getStr(i, desc_index);         
						data.isShowGoTaskBtn = reader.getBool(i, isShowGoTaskBtn_index, false);         
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
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("TeamGoal.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TeamGoal);
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


