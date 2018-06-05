// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanFoodConfig 
    {
        static Dictionary<int, ClanFoodConfig> DataList = new Dictionary<int, ClanFoodConfig>();

        static public Dictionary<int, ClanFoodConfig> GetAll()
        {
            return DataList;
        }

        static public ClanFoodConfig Get(int key)
        {
            ClanFoodConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ClanFoodConfig.Get({0}) not find!", key);
            return null;
        }



        // 食物id
        public int ID { get; set; }

        // 食物名称
        public string foodname { get; set; }

        // 图标
        public string foodicon { get; set; }

        // 花费资金
        public int foodprice { get; set; }

        // 每点击一次吃多少
        public int percount { get; set; }

        // 食物总量
        public int foodmax { get; set; }

        // 吃一次奖励每人最多可食用量
        public int peraward { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ClanFoodConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanFoodConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanFoodConfig> allDatas = new List<ClanFoodConfig>();

            {
                string file = "Clan/ClanFoodConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int ID_index = reader.GetIndex("ID");
                int foodname_index = reader.GetIndex("foodname");
                int foodicon_index = reader.GetIndex("foodicon");
                int foodprice_index = reader.GetIndex("foodprice");
                int percount_index = reader.GetIndex("percount");
                int foodmax_index = reader.GetIndex("foodmax");
                int peraward_index = reader.GetIndex("peraward");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanFoodConfig data = new ClanFoodConfig();
						data.ID = reader.getInt(i, ID_index, 0);         
						data.foodname = reader.getStr(i, foodname_index);         
						data.foodicon = reader.getStr(i, foodicon_index);         
						data.foodprice = reader.getInt(i, foodprice_index, 0);         
						data.percount = reader.getInt(i, percount_index, 0);         
						data.foodmax = reader.getInt(i, foodmax_index, 0);         
						data.peraward = reader.getInt(i, peraward_index, 0);         
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
                if (DataList.ContainsKey(data.ID))
                {
                    CsvCommon.Log.Error("ClanFoodConfig.ID :{0} is repeated", data.ID);
                }
                else
                {
                    DataList[data.ID] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ClanFoodConfig);
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


