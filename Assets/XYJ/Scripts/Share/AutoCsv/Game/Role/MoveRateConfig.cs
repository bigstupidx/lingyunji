// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class MoveRateConfig 
    {
        static List<MoveRateConfig> DataList = new List<MoveRateConfig>();
        static public List<MoveRateConfig> GetAll()
        {
            return DataList;
        }

        static public MoveRateConfig Get(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("MoveRateConfig.Get({0}) not find!", key);
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

        static Dictionary<string, List<MoveRateConfig>> DataList_key = new Dictionary<string, List<MoveRateConfig>>();

        static public Dictionary<string, List<MoveRateConfig>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<MoveRateConfig> GetGroupBykey(string key)
        {
            List<MoveRateConfig> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("MoveRateConfig.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 模型ID
        public string key { get; set; }

        // 移动动作ID
        public string name { get; set; }

        // 默认移动速度
        public float aniSpeed { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(MoveRateConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(MoveRateConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<MoveRateConfig> allDatas = new List<MoveRateConfig>();

            {
                string file = "Role/MoveRateConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int name_index = reader.GetIndex("name");
                int aniSpeed_index = reader.GetIndex("aniSpeed");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        MoveRateConfig data = new MoveRateConfig();
						data.key = reader.getStr(i, key_index);         
						data.name = reader.getStr(i, name_index);         
						data.aniSpeed = reader.getFloat(i, aniSpeed_index, 0f);         
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

            foreach (var data in allDatas)
            {
                {
                    List<MoveRateConfig> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<MoveRateConfig>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(MoveRateConfig);
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


