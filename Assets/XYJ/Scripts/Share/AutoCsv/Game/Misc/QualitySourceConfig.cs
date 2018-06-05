// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class QualitySourceConfig 
    {
        static Dictionary<ItemQuality, QualitySourceConfig> DataList = new Dictionary<ItemQuality, QualitySourceConfig>();

        static public Dictionary<ItemQuality, QualitySourceConfig> GetAll()
        {
            return DataList;
        }

        static public QualitySourceConfig Get(ItemQuality key)
        {
            QualitySourceConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("QualitySourceConfig.Get({0}) not find!", key);
            return null;
        }



        // 品质参数
        public ItemQuality id { get; set; }

        // 颜色
        public string name { get; set; }

        // icon背景
        public string icon { get; set; }

        // tips背景
        public string tips { get; set; }

        // 对应名称颜色（策划看）
        public string color { get; set; }

        // 颜色代号
        public string colorname { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(QualitySourceConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(QualitySourceConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<QualitySourceConfig> allDatas = new List<QualitySourceConfig>();

            {
                string file = "Misc/QualitySourceConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int icon_index = reader.GetIndex("icon");
                int tips_index = reader.GetIndex("tips");
                int color_index = reader.GetIndex("color");
                int colorname_index = reader.GetIndex("colorname");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        QualitySourceConfig data = new QualitySourceConfig();
						data.id = ((ItemQuality)reader.getInt(i, id_index, 0));         
						data.name = reader.getStr(i, name_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.tips = reader.getStr(i, tips_index);         
						data.color = reader.getStr(i, color_index);         
						data.colorname = reader.getStr(i, colorname_index);         
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
                    CsvCommon.Log.Error("QualitySourceConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(QualitySourceConfig);
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


