// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SingConfig 
    {
        static Dictionary<int, SingConfig> DataList = new Dictionary<int, SingConfig>();

        static public Dictionary<int, SingConfig> GetAll()
        {
            return DataList;
        }

        static public SingConfig Get(int key)
        {
            SingConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SingConfig.Get({0}) not find!", key);
            return null;
        }



        // 吟唱动作id
        public int id { get; set; }

        // 吟唱动作1
        public string anmation1 { get; set; }

        // 吟唱动作2
        public string anmation2 { get; set; }

        // 吟唱动作3
        public string anmation3 { get; set; }

        // 吟唱动作2时间
        public float singTime { get; set; }

        // 吟唱条类型
        public int uiShowType { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SingConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SingConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SingConfig> allDatas = new List<SingConfig>();

            {
                string file = "Task/SingConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int anmation1_index = reader.GetIndex("anmation1");
                int anmation2_index = reader.GetIndex("anmation2");
                int anmation3_index = reader.GetIndex("anmation3");
                int singTime_index = reader.GetIndex("singTime");
                int uiShowType_index = reader.GetIndex("uiShowType");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SingConfig data = new SingConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.anmation1 = reader.getStr(i, anmation1_index);         
						data.anmation2 = reader.getStr(i, anmation2_index);         
						data.anmation3 = reader.getStr(i, anmation3_index);         
						data.singTime = reader.getFloat(i, singTime_index, 0f);         
						data.uiShowType = reader.getInt(i, uiShowType_index, 0);         
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
                    CsvCommon.Log.Error("SingConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(SingConfig);
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


