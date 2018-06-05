// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class NewbieRewardDefine 
    {
        static Dictionary<int, NewbieRewardDefine> DataList = new Dictionary<int, NewbieRewardDefine>();

        static public Dictionary<int, NewbieRewardDefine> GetAll()
        {
            return DataList;
        }

        static public NewbieRewardDefine Get(int key)
        {
            NewbieRewardDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("NewbieRewardDefine.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 时间
        public int time { get; set; }

        // 奖励ID
        public int award { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(NewbieRewardDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(NewbieRewardDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<NewbieRewardDefine> allDatas = new List<NewbieRewardDefine>();

            {
                string file = "Welfare/NewbieRewardDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int time_index = reader.GetIndex("time");
                int award_index = reader.GetIndex("award");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        NewbieRewardDefine data = new NewbieRewardDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.time = reader.getInt(i, time_index, 0);         
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
                    CsvCommon.Log.Error("NewbieRewardDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(NewbieRewardDefine);
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


