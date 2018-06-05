// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class WorldDroped 
    {
        static Dictionary<int, WorldDroped> DataList = new Dictionary<int, WorldDroped>();

        static public Dictionary<int, WorldDroped> GetAll()
        {
            return DataList;
        }

        static public WorldDroped Get(int key)
        {
            WorldDroped value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("WorldDroped.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 随机次数
        public int randomCount { get; set; }

        // 不掉落权重
        public int noDrop { get; set; }

        // 掉落权重
        public int drop { get; set; }

        // 是否氏族掉落
        public bool isGuild { get; set; }

        // 掉落包
        public int dropid { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(WorldDroped);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(WorldDroped);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<WorldDroped> allDatas = new List<WorldDroped>();

            {
                string file = "Item/WorldDroped.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int randomCount_index = reader.GetIndex("randomCount");
                int noDrop_index = reader.GetIndex("noDrop");
                int drop_index = reader.GetIndex("drop");
                int isGuild_index = reader.GetIndex("isGuild");
                int dropid_index = reader.GetIndex("dropid");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        WorldDroped data = new WorldDroped();
						data.id = reader.getInt(i, id_index, 0);         
						data.randomCount = reader.getInt(i, randomCount_index, 0);         
						data.noDrop = reader.getInt(i, noDrop_index, 0);         
						data.drop = reader.getInt(i, drop_index, 0);         
						data.isGuild = reader.getBool(i, isGuild_index, false);         
						data.dropid = reader.getInt(i, dropid_index, 0);         
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
                    CsvCommon.Log.Error("WorldDroped.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(WorldDroped);
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


