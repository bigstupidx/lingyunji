// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanActivityTimeConfig 
    {
        static Dictionary<int, ClanActivityTimeConfig> DataList = new Dictionary<int, ClanActivityTimeConfig>();

        static public Dictionary<int, ClanActivityTimeConfig> GetAll()
        {
            return DataList;
        }

        static public ClanActivityTimeConfig Get(int key)
        {
            ClanActivityTimeConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ClanActivityTimeConfig.Get({0}) not find!", key);
            return null;
        }



        // 活动id
        public int id { get; set; }

        // 时间1
        public int[] time1 { get; set; }

        // 时间2
        public int[] time2 { get; set; }

        // 时间3
        public int[] time3 { get; set; }

        // 时间4
        public int[] time4 { get; set; }

        // 时间5
        public int[] time5 { get; set; }

        // 时间6
        public int[] time6 { get; set; }

        // 时间7
        public int[] time7 { get; set; }

        // 时间8
        public int[] time8 { get; set; }

        // 时间9
        public int[] time9 { get; set; }

        // 时间10
        public int[] time10 { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ClanActivityTimeConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanActivityTimeConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanActivityTimeConfig> allDatas = new List<ClanActivityTimeConfig>();

            {
                string file = "Clan/ClanActivityTimeConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int time1_index = reader.GetIndex("time1");
                int time2_index = reader.GetIndex("time2");
                int time3_index = reader.GetIndex("time3");
                int time4_index = reader.GetIndex("time4");
                int time5_index = reader.GetIndex("time5");
                int time6_index = reader.GetIndex("time6");
                int time7_index = reader.GetIndex("time7");
                int time8_index = reader.GetIndex("time8");
                int time9_index = reader.GetIndex("time9");
                int time10_index = reader.GetIndex("time10");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanActivityTimeConfig data = new ClanActivityTimeConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.time1 = reader.getInts(i, time1_index, ';');         
						data.time2 = reader.getInts(i, time2_index, ';');         
						data.time3 = reader.getInts(i, time3_index, ';');         
						data.time4 = reader.getInts(i, time4_index, ';');         
						data.time5 = reader.getInts(i, time5_index, ';');         
						data.time6 = reader.getInts(i, time6_index, ';');         
						data.time7 = reader.getInts(i, time7_index, ';');         
						data.time8 = reader.getInts(i, time8_index, ';');         
						data.time9 = reader.getInts(i, time9_index, ';');         
						data.time10 = reader.getInts(i, time10_index, ';');         
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
                    CsvCommon.Log.Error("ClanActivityTimeConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ClanActivityTimeConfig);
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


