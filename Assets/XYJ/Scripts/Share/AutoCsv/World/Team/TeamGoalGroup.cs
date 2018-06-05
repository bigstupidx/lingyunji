// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TeamGoalGroup 
    {
        static Dictionary<int, TeamGoalGroup> DataList = new Dictionary<int, TeamGoalGroup>();

        static public Dictionary<int, TeamGoalGroup> GetAll()
        {
            return DataList;
        }

        static public TeamGoalGroup Get(int key)
        {
            TeamGoalGroup value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TeamGoalGroup.Get({0}) not find!", key);
            return null;
        }



        // 组ID
        public int id { get; set; }

        // 所属目标分类
        public int goalTypeId { get; set; }

        // 名字
        public string name { get; set; }

        // 普通目标
        public int commenGoalId { get; set; }

        // 精英目标
        public int elitGoalId { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TeamGoalGroup);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TeamGoalGroup);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TeamGoalGroup> allDatas = new List<TeamGoalGroup>();

            {
                string file = "Team/TeamGoalGroup.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int goalTypeId_index = reader.GetIndex("goalTypeId");
                int name_index = reader.GetIndex("name");
                int commenGoalId_index = reader.GetIndex("commenGoalId");
                int elitGoalId_index = reader.GetIndex("elitGoalId");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TeamGoalGroup data = new TeamGoalGroup();
						data.id = reader.getInt(i, id_index, 0);         
						data.goalTypeId = reader.getInt(i, goalTypeId_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.commenGoalId = reader.getInt(i, commenGoalId_index, 0);         
						data.elitGoalId = reader.getInt(i, elitGoalId_index, 0);         
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
                    CsvCommon.Log.Error("TeamGoalGroup.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TeamGoalGroup);
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


