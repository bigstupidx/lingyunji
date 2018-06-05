// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RankPanel 
    {
        static Dictionary<int, RankPanel> DataList = new Dictionary<int, RankPanel>();

        static public Dictionary<int, RankPanel> GetAll()
        {
            return DataList;
        }

        static public RankPanel Get(int key)
        {
            RankPanel value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RankPanel.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<ClientRankType, List<RankPanel>> DataList_clientRanktype = new Dictionary<ClientRankType, List<RankPanel>>();

        static public Dictionary<ClientRankType, List<RankPanel>> GetAllGroupByclientRanktype()
        {
            return DataList_clientRanktype;
        }

        static public List<RankPanel> GetGroupByclientRanktype(ClientRankType key)
        {
            List<RankPanel> value = null;
            if (DataList_clientRanktype.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RankPanel.GetGroupByclientRanktype({0}) not find!", key);
            return null;
        }


        // id
        public int id { get; set; }

        // 客户端排行定义
        public ClientRankType clientRanktype { get; set; }

        // 排行榜名称
        public string rankName { get; set; }

        // 关联服务器排行榜
        public int serverRankType { get; set; }

        // 我的排名文本
        public string rankOrderStr { get; set; }

        // 我的能力文本
        public string rankAbilityStr { get; set; }

        // 标题
        public string[] titles { get; set; }

        // 过滤类型
        public ClientRankFilterType filterType { get; set; }

        // 过滤参数
        public int[] filterParams { get; set; }

        // 过滤显示文本
        public string[] filterStrs { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RankPanel);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_clientRanktype.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RankPanel);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RankPanel> allDatas = new List<RankPanel>();

            {
                string file = "Rank/RankPanel.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int clientRanktype_index = reader.TryIndex("clientRanktype:group");
                int rankName_index = reader.GetIndex("rankName");
                int serverRankType_index = reader.GetIndex("serverRankType");
                int rankOrderStr_index = reader.GetIndex("rankOrderStr");
                int rankAbilityStr_index = reader.GetIndex("rankAbilityStr");
                int titles_index = reader.GetIndex("titles");
                int filterType_index = reader.GetIndex("filterType");
                int filterParams_index = reader.GetIndex("filterParams");
                int filterStrs_index = reader.GetIndex("filterStrs");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RankPanel data = new RankPanel();
						data.id = reader.getInt(i, id_index, 0);         
						data.clientRanktype = ((ClientRankType)reader.getInt(i, clientRanktype_index, 0));         
						data.rankName = reader.getStr(i, rankName_index);         
						data.serverRankType = reader.getInt(i, serverRankType_index, 0);         
						data.rankOrderStr = reader.getStr(i, rankOrderStr_index);         
						data.rankAbilityStr = reader.getStr(i, rankAbilityStr_index);         
						data.titles = reader.getStrs(i, titles_index, ';');         
						data.filterType = ((ClientRankFilterType)reader.getInt(i, filterType_index, 0));         
						data.filterParams = reader.getInts(i, filterParams_index, ';');         
						data.filterStrs = reader.getStrs(i, filterStrs_index, ';');         
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
                    CsvCommon.Log.Error("RankPanel.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<RankPanel> l = null;
                    if (!DataList_clientRanktype.TryGetValue(data.clientRanktype, out l))
                    {
                        l = new List<RankPanel>();
                        DataList_clientRanktype[data.clientRanktype] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(RankPanel);
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


