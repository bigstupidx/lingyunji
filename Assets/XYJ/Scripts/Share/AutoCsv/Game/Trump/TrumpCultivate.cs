// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TrumpCultivate 
    {
        static Dictionary<int, TrumpCultivate> DataList = new Dictionary<int, TrumpCultivate>();

        static public Dictionary<int, TrumpCultivate> GetAll()
        {
            return DataList;
        }

        static public TrumpCultivate Get(int key)
        {
            TrumpCultivate value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TrumpCultivate.Get({0}) not find!", key);
            return null;
        }



        // 潜修效果等级
        public int id { get; set; }

        // 属性加成
        public int propertyvalue { get; set; }

        // 需要潜修点数
        public int soulpoints { get; set; }

        // 需要法宝境界
        public int tastelv { get; set; }

        // 需要总潜修等级
        public int soullvs { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TrumpCultivate);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TrumpCultivate);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TrumpCultivate> allDatas = new List<TrumpCultivate>();

            {
                string file = "Trump/TrumpCultivate.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int propertyvalue_index = reader.GetIndex("propertyvalue");
                int soulpoints_index = reader.GetIndex("soulpoints");
                int tastelv_index = reader.GetIndex("tastelv");
                int soullvs_index = reader.GetIndex("soullvs");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TrumpCultivate data = new TrumpCultivate();
						data.id = reader.getInt(i, id_index, 0);         
						data.propertyvalue = reader.getInt(i, propertyvalue_index, 0);         
						data.soulpoints = reader.getInt(i, soulpoints_index, 0);         
						data.tastelv = reader.getInt(i, tastelv_index, 0);         
						data.soullvs = reader.getInt(i, soullvs_index, 0);         
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
                    CsvCommon.Log.Error("TrumpCultivate.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TrumpCultivate);
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


