// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class IgnoreSymbol 
    {
        static Dictionary<int, IgnoreSymbol> DataList = new Dictionary<int, IgnoreSymbol>();

        static public Dictionary<int, IgnoreSymbol> GetAll()
        {
            return DataList;
        }

        static public IgnoreSymbol Get(int key)
        {
            IgnoreSymbol value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("IgnoreSymbol.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public int id { get; set; }

        // 特殊符号
        public string specialName { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(IgnoreSymbol);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(IgnoreSymbol);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<IgnoreSymbol> allDatas = new List<IgnoreSymbol>();

            {
                string file = "Role/IgnoreSymbol.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int specialName_index = reader.GetIndex("specialName");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        IgnoreSymbol data = new IgnoreSymbol();
						data.id = reader.getInt(i, id_index, 0);         
						data.specialName = reader.getStr(i, specialName_index);         
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
                    CsvCommon.Log.Error("IgnoreSymbol.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(IgnoreSymbol);
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


