// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TrumpCultivateExp 
    {
        static Dictionary<int, TrumpCultivateExp> DataList = new Dictionary<int, TrumpCultivateExp>();

        static public Dictionary<int, TrumpCultivateExp> GetAll()
        {
            return DataList;
        }

        static public TrumpCultivateExp Get(int key)
        {
            TrumpCultivateExp value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TrumpCultivateExp.Get({0}) not find!", key);
            return null;
        }



        // 潜修等级
        public int id { get; set; }

        // 升级经验
        public int exp { get; set; }

        // 需要法宝境界
        public int tastelv { get; set; }

        // 获得潜修点数
        public int point { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TrumpCultivateExp);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TrumpCultivateExp);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TrumpCultivateExp> allDatas = new List<TrumpCultivateExp>();

            {
                string file = "Trump/TrumpCultivateExp.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int exp_index = reader.GetIndex("exp");
                int tastelv_index = reader.GetIndex("tastelv");
                int point_index = reader.GetIndex("point");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TrumpCultivateExp data = new TrumpCultivateExp();
						data.id = reader.getInt(i, id_index, 0);         
						data.exp = reader.getInt(i, exp_index, 0);         
						data.tastelv = reader.getInt(i, tastelv_index, 0);         
						data.point = reader.getInt(i, point_index, 0);         
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
                    CsvCommon.Log.Error("TrumpCultivateExp.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TrumpCultivateExp);
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


