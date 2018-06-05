// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ActiveAward 
    {
        static Dictionary<int, ActiveAward> DataList = new Dictionary<int, ActiveAward>();

        static public Dictionary<int, ActiveAward> GetAll()
        {
            return DataList;
        }

        static public ActiveAward Get(int key)
        {
            ActiveAward value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ActiveAward.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 所需活跃度
        public int activenessRequire { get; set; }

        // 奖励id
        public int rewardId { get; set; }

        // 道具id
        public int itemId { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ActiveAward);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ActiveAward);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ActiveAward> allDatas = new List<ActiveAward>();

            {
                string file = "Level/ActiveAward.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int activenessRequire_index = reader.GetIndex("activenessRequire");
                int rewardId_index = reader.GetIndex("rewardId");
                int itemId_index = reader.GetIndex("itemId");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ActiveAward data = new ActiveAward();
						data.id = reader.getInt(i, id_index, 0);         
						data.activenessRequire = reader.getInt(i, activenessRequire_index, 0);         
						data.rewardId = reader.getInt(i, rewardId_index, 0);         
						data.itemId = reader.getInt(i, itemId_index, 0);         
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
                    CsvCommon.Log.Error("ActiveAward.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ActiveAward);
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


