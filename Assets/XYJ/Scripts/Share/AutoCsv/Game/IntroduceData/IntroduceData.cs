// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class IntroduceData 
    {
        static Dictionary<int, IntroduceData> DataList = new Dictionary<int, IntroduceData>();

        static public Dictionary<int, IntroduceData> GetAll()
        {
            return DataList;
        }

        static public IntroduceData Get(int key)
        {
            IntroduceData value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("IntroduceData.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 规则介绍说明
        public string desc { get; set; }

        // 备注
        public string note { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(IntroduceData);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(IntroduceData);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<IntroduceData> allDatas = new List<IntroduceData>();

            {
                string file = "IntroduceData/IntroduceData.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int desc_index = reader.GetIndex("desc");
                int note_index = reader.GetIndex("note");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        IntroduceData data = new IntroduceData();
						data.id = reader.getInt(i, id_index, 0);         
						data.desc = reader.getStr(i, desc_index);         
						data.note = reader.getStr(i, note_index);         
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
                    CsvCommon.Log.Error("IntroduceData.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(IntroduceData);
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


