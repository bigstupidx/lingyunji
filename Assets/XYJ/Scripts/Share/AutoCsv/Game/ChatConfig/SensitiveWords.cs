// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SensitiveWords 
    {
        static Dictionary<int, SensitiveWords> DataList = new Dictionary<int, SensitiveWords>();

        static public Dictionary<int, SensitiveWords> GetAll()
        {
            return DataList;
        }

        static public SensitiveWords Get(int key)
        {
            SensitiveWords value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SensitiveWords.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<int, List<SensitiveWords>> DataList_id = new Dictionary<int, List<SensitiveWords>>();

        static public Dictionary<int, List<SensitiveWords>> GetAllGroupByid()
        {
            return DataList_id;
        }

        static public List<SensitiveWords> GetGroupByid(int key)
        {
            List<SensitiveWords> value = null;
            if (DataList_id.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SensitiveWords.GetGroupByid({0}) not find!", key);
            return null;
        }


        // ID
        public int id { get; set; }

        // 屏蔽内容
        public string desc { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SensitiveWords);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_id.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SensitiveWords);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SensitiveWords> allDatas = new List<SensitiveWords>();

            {
                string file = "ChatConfig/SensitiveWords.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.TryIndex("id:group");
                int desc_index = reader.GetIndex("desc");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SensitiveWords data = new SensitiveWords();
						data.id = reader.getInt(i, id_index, 0);         
						data.desc = reader.getStr(i, desc_index);         
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
                    CsvCommon.Log.Error("SensitiveWords.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<SensitiveWords> l = null;
                    if (!DataList_id.TryGetValue(data.id, out l))
                    {
                        l = new List<SensitiveWords>();
                        DataList_id[data.id] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(SensitiveWords);
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


