// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TipsContent 
    {
        static Dictionary<int, TipsContent> DataList = new Dictionary<int, TipsContent>();

        static public Dictionary<int, TipsContent> GetAll()
        {
            return DataList;
        }

        static public TipsContent Get(int key)
        {
            TipsContent value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TipsContent.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<string, List<TipsContent>> DataList_name = new Dictionary<string, List<TipsContent>>();

        static public Dictionary<string, List<TipsContent>> GetAllGroupByname()
        {
            return DataList_name;
        }

        static public List<TipsContent> GetGroupByname(string key)
        {
            List<TipsContent> value = null;
            if (DataList_name.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TipsContent.GetGroupByname({0}) not find!", key);
            return null;
        }


        // ID
        public int id { get; set; }

        // name
        public string name { get; set; }

        // 内容
        public string des { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TipsContent);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_name.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TipsContent);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TipsContent> allDatas = new List<TipsContent>();

            {
                string file = "BaseConfig/TipsContent.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.TryIndex("name:group");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TipsContent data = new TipsContent();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.des = reader.getStr(i, des_index);         
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
                    CsvCommon.Log.Error("TipsContent.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<TipsContent> l = null;
                    if (!DataList_name.TryGetValue(data.name, out l))
                    {
                        l = new List<TipsContent>();
                        DataList_name[data.name] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(TipsContent);
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


