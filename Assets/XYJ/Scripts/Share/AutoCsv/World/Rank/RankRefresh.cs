// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RankRefresh 
    {
        static Dictionary<int, RankRefresh> DataList = new Dictionary<int, RankRefresh>();

        static public Dictionary<int, RankRefresh> GetAll()
        {
            return DataList;
        }

        static public RankRefresh Get(int key)
        {
            RankRefresh value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RankRefresh.Get({0}) not find!", key);
            return null;
        }



        // 排行榜id
        public int id { get; set; }

        // 排行榜名称
        public string rankName { get; set; }

        // 更新时间周
        public int[] weekDays { get; set; }

        // 更新时间时
        public int hour { get; set; }

        // 更新时间分
        public int min { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RankRefresh);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RankRefresh);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RankRefresh> allDatas = new List<RankRefresh>();

            {
                string file = "Rank/RankRefresh.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int rankName_index = reader.GetIndex("rankName");
                int weekDays_index = reader.GetIndex("weekDays");
                int hour_index = reader.GetIndex("hour");
                int min_index = reader.GetIndex("min");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RankRefresh data = new RankRefresh();
						data.id = reader.getInt(i, id_index, 0);         
						data.rankName = reader.getStr(i, rankName_index);         
						data.weekDays = reader.getInts(i, weekDays_index, ';');         
						data.hour = reader.getInt(i, hour_index, 0);         
						data.min = reader.getInt(i, min_index, 0);         
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
                    CsvCommon.Log.Error("RankRefresh.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RankRefresh);
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


