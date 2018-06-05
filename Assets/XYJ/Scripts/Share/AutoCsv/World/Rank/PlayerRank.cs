// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PlayerRank 
    {
        static Dictionary<int, PlayerRank> DataList = new Dictionary<int, PlayerRank>();

        static public Dictionary<int, PlayerRank> GetAll()
        {
            return DataList;
        }

        static public PlayerRank Get(int key)
        {
            PlayerRank value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PlayerRank.Get({0}) not find!", key);
            return null;
        }



        // 排行榜id
        public int id { get; set; }

        // 排行榜名称
        public string rankName { get; set; }

        // 排序字段
        public string[] cmpDbFields { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PlayerRank);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PlayerRank);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PlayerRank> allDatas = new List<PlayerRank>();

            {
                string file = "Rank/PlayerRank.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int rankName_index = reader.GetIndex("rankName");
                int cmpDbFields_index = reader.GetIndex("cmpDbFields");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PlayerRank data = new PlayerRank();
						data.id = reader.getInt(i, id_index, 0);         
						data.rankName = reader.getStr(i, rankName_index);         
						data.cmpDbFields = reader.getStrs(i, cmpDbFields_index, ';');         
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
                    CsvCommon.Log.Error("PlayerRank.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(PlayerRank);
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


