// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ColorConfig 
    {
        static Dictionary<int, ColorConfig> DataList = new Dictionary<int, ColorConfig>();

        static public Dictionary<int, ColorConfig> GetAll()
        {
            return DataList;
        }

        static public ColorConfig Get(int key)
        {
            ColorConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ColorConfig.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 颜色名字
        public string colorName { get; set; }

        // 颜色码
        public string colorCode { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ColorConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ColorConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ColorConfig> allDatas = new List<ColorConfig>();

            {
                string file = "Misc/ColorConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int colorName_index = reader.GetIndex("colorName");
                int colorCode_index = reader.GetIndex("colorCode");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ColorConfig data = new ColorConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.colorName = reader.getStr(i, colorName_index);         
						data.colorCode = reader.getStr(i, colorCode_index);         
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
                    CsvCommon.Log.Error("ColorConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ColorConfig);
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


