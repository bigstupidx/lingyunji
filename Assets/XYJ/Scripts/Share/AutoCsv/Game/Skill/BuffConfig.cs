// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class BuffConfig 
    {
        static Dictionary<int, BuffConfig> DataList = new Dictionary<int, BuffConfig>();

        static public Dictionary<int, BuffConfig> GetAll()
        {
            return DataList;
        }

        static public BuffConfig Get(int key)
        {
            BuffConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("BuffConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public int id { get; set; }

        // 结束类型
        public FinishType finishType { get; set; }

        // 持续时间
        public float timeLen { get; set; }

        // 类型
        public string typename { get; set; }

        // 类型参数
        public string para { get; set; }

        // 叠加层数
        public int addMaxCnt { get; set; }

        // 不同施法者规则
        public AddType addType { get; set; }

        // 进入action
        public string[] beginActionStrs { get; set; }

        // 结束action
        public string[] endActionStrs { get; set; }

        // 间隔action
        public string[] tickActionStrs { get; set; }

        // 间隔时间
        public float tickTime { get; set; }

        // 特效
        public string effect { get; set; }

        // 描述
        public string des { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(BuffConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(BuffConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<BuffConfig> allDatas = new List<BuffConfig>();

            {
                string file = "Skill/BuffConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int finishType_index = reader.GetIndex("finishType");
                int timeLen_index = reader.GetIndex("timeLen");
                int typename_index = reader.GetIndex("typename");
                int para_index = reader.GetIndex("para");
                int addMaxCnt_index = reader.GetIndex("addMaxCnt");
                int addType_index = reader.GetIndex("addType");
                int beginActionStrs_index = reader.GetIndex("beginActionStrs");
                int endActionStrs_index = reader.GetIndex("endActionStrs");
                int tickActionStrs_index = reader.GetIndex("tickActionStrs");
                int tickTime_index = reader.GetIndex("tickTime");
                int effect_index = reader.GetIndex("effect");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        BuffConfig data = new BuffConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.finishType = ((FinishType)reader.getInt(i, finishType_index, 0));         
						data.timeLen = reader.getFloat(i, timeLen_index, 0f);         
						data.typename = reader.getStr(i, typename_index);         
						data.para = reader.getStr(i, para_index);         
						data.addMaxCnt = reader.getInt(i, addMaxCnt_index, 0);         
						data.addType = ((AddType)reader.getInt(i, addType_index, 0));         
						data.beginActionStrs = reader.getStrs(i, beginActionStrs_index, ';');         
						data.endActionStrs = reader.getStrs(i, endActionStrs_index, ';');         
						data.tickActionStrs = reader.getStrs(i, tickActionStrs_index, ';');         
						data.tickTime = reader.getFloat(i, tickTime_index, 0f);         
						data.effect = reader.getStr(i, effect_index);         
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
                    CsvCommon.Log.Error("BuffConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(BuffConfig);
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


