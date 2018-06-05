// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ShangHuiConfig 
    {
        static Dictionary<int, ShangHuiConfig> DataList = new Dictionary<int, ShangHuiConfig>();

        static public Dictionary<int, ShangHuiConfig> GetAll()
        {
            return DataList;
        }

        static public ShangHuiConfig Get(int key)
        {
            ShangHuiConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ShangHuiConfig.Get({0}) not find!", key);
            return null;
        }



        // 商会ID
        public int id { get; set; }

        // 父类型名
        public string parentName { get; set; }

        // 子类型名
        public string childName { get; set; }

        // 商会类型
        public int type { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ShangHuiConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ShangHuiConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ShangHuiConfig> allDatas = new List<ShangHuiConfig>();

            {
                string file = "ExchangeStore/ShangHuiConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int parentName_index = reader.GetIndex("parentName");
                int childName_index = reader.GetIndex("childName");
                int type_index = reader.GetIndex("type");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ShangHuiConfig data = new ShangHuiConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.parentName = reader.getStr(i, parentName_index);         
						data.childName = reader.getStr(i, childName_index);         
						data.type = reader.getInt(i, type_index, 0);         
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
                    CsvCommon.Log.Error("ShangHuiConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ShangHuiConfig);
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


