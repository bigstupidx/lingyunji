// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SevenRewardDefine 
    {
        static Dictionary<int, SevenRewardDefine> DataList = new Dictionary<int, SevenRewardDefine>();

        static public Dictionary<int, SevenRewardDefine> GetAll()
        {
            return DataList;
        }

        static public SevenRewardDefine Get(int key)
        {
            SevenRewardDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SevenRewardDefine.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 天数
        public int day { get; set; }

        // 奖励ID
        public int award { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SevenRewardDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SevenRewardDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SevenRewardDefine> allDatas = new List<SevenRewardDefine>();

            {
                string file = "Welfare/SevenRewardDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int day_index = reader.GetIndex("day");
                int award_index = reader.GetIndex("award");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SevenRewardDefine data = new SevenRewardDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.day = reader.getInt(i, day_index, 0);         
						data.award = reader.getInt(i, award_index, 0);         
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
                    CsvCommon.Log.Error("SevenRewardDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(SevenRewardDefine);
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


