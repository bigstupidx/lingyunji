// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class Bag 
    {
        static Dictionary<BagType, Bag> DataList = new Dictionary<BagType, Bag>();

        static public Dictionary<BagType, Bag> GetAll()
        {
            return DataList;
        }

        static public Bag Get(BagType key)
        {
            Bag value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("Bag.Get({0}) not find!", key);
            return null;
        }



        // id
        public BagType id { get; set; }

        // 名称
        public string name { get; set; }

        // 类型
        public int type { get; set; }

        // 格子总数
        public int sum { get; set; }

        // 开放格子编号
        public int openIndex { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(Bag);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(Bag);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<Bag> allDatas = new List<Bag>();

            {
                string file = "Bag/Bag.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int type_index = reader.GetIndex("type");
                int sum_index = reader.GetIndex("sum");
                int openIndex_index = reader.GetIndex("openIndex");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        Bag data = new Bag();
						data.id = ((BagType)reader.getInt(i, id_index, 0));         
						data.name = reader.getStr(i, name_index);         
						data.type = reader.getInt(i, type_index, 0);         
						data.sum = reader.getInt(i, sum_index, 0);         
						data.openIndex = reader.getInt(i, openIndex_index, 0);         
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
                    CsvCommon.Log.Error("Bag.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(Bag);
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


