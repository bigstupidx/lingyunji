// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class StandardReward 
    {
        static Dictionary<int, StandardReward> DataList = new Dictionary<int, StandardReward>();

        static public Dictionary<int, StandardReward> GetAll()
        {
            return DataList;
        }

        static public StandardReward Get(int key)
        {
            StandardReward value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("StandardReward.Get({0}) not find!", key);
            return null;
        }



        // 角色等级
        public int id { get; set; }

        // 基础经验
        public int exp { get; set; }

        // 基础修为
        public int xiuwei { get; set; }

        // 修为丹奖励
        public int pill { get; set; }

        // 基础银贝
        public int silver { get; set; }

        // 基础宠物经验
        public int petexp { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(StandardReward);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(StandardReward);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<StandardReward> allDatas = new List<StandardReward>();

            {
                string file = "Reward/StandardReward.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int exp_index = reader.GetIndex("exp");
                int xiuwei_index = reader.GetIndex("xiuwei");
                int pill_index = reader.GetIndex("pill");
                int silver_index = reader.GetIndex("silver");
                int petexp_index = reader.GetIndex("petexp");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        StandardReward data = new StandardReward();
						data.id = reader.getInt(i, id_index, 0);         
						data.exp = reader.getInt(i, exp_index, 0);         
						data.xiuwei = reader.getInt(i, xiuwei_index, 0);         
						data.pill = reader.getInt(i, pill_index, 0);         
						data.silver = reader.getInt(i, silver_index, 0);         
						data.petexp = reader.getInt(i, petexp_index, 0);         
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
                    CsvCommon.Log.Error("StandardReward.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(StandardReward);
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


