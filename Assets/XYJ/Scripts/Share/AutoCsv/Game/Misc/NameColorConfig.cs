// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class NameColorConfig 
    {
        static Dictionary<int, NameColorConfig> DataList = new Dictionary<int, NameColorConfig>();

        static public Dictionary<int, NameColorConfig> GetAll()
        {
            return DataList;
        }

        static public NameColorConfig Get(int key)
        {
            NameColorConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("NameColorConfig.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 主体色值
        public string mainColor { get; set; }

        // 描边色值
        public string outlineColor { get; set; }

        // 主体色备注
        public string colorName { get; set; }

        // 描边色备注
        public string colorName1 { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(NameColorConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(NameColorConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<NameColorConfig> allDatas = new List<NameColorConfig>();

            {
                string file = "Misc/NameColorConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int mainColor_index = reader.GetIndex("mainColor");
                int outlineColor_index = reader.GetIndex("outlineColor");
                int colorName_index = reader.GetIndex("colorName");
                int colorName1_index = reader.GetIndex("colorName1");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        NameColorConfig data = new NameColorConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.mainColor = reader.getStr(i, mainColor_index);         
						data.outlineColor = reader.getStr(i, outlineColor_index);         
						data.colorName = reader.getStr(i, colorName_index);         
						data.colorName1 = reader.getStr(i, colorName1_index);         
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
                    CsvCommon.Log.Error("NameColorConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(NameColorConfig);
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


