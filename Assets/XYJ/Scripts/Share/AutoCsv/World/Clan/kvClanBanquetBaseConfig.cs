// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class kvClanBanquetBaseConfig 
    {
        static List<kvClanBanquetBaseConfig> DataList = new List<kvClanBanquetBaseConfig>();
        static public List<kvClanBanquetBaseConfig> GetAll()
        {
            return DataList;
        }

        static public kvClanBanquetBaseConfig Get(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("kvClanBanquetBaseConfig.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return i;
            }
            return -1;
        }



        // 配置id
        public string key { get; set; }

        // 配置值
        public string value { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(kvClanBanquetBaseConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(kvClanBanquetBaseConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<kvClanBanquetBaseConfig> allDatas = new List<kvClanBanquetBaseConfig>();

            {
                string file = "Clan/kvClanBanquetBaseConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.GetIndex("key");
                int value_index = reader.GetIndex("value");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        kvClanBanquetBaseConfig data = new kvClanBanquetBaseConfig();
						data.key = reader.getStr(i, key_index);         
						data.value = reader.getStr(i, value_index);         
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

            {
                MethodInfo method = null;
                {
                    var curType = typeof(kvClanBanquetBaseConfig);
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


