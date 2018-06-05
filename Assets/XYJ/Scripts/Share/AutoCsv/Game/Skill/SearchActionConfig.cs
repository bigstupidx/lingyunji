// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SearchActionConfig 
    {
        static Dictionary<string, SearchActionConfig> DataList = new Dictionary<string, SearchActionConfig>();

        static public Dictionary<string, SearchActionConfig> GetAll()
        {
            return DataList;
        }

        static public SearchActionConfig Get(string key)
        {
            SearchActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SearchActionConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public string id { get; set; }

        // 搜索原点
        public SearchPoint searchPosition { get; set; }

        // 搜索范围
        public SearchSharp searchType { get; set; }

        // 范围参数
        public float[] searchPara { get; set; }

        // 范围位置偏移
        public float[] posOff { get; set; }

        // 数量上限
        public int searchMaxCnt { get; set; }

        // 阵营筛选
        public BattleRelation battleRelation { get; set; }

        // action集合
        public string[] actionStrs { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SearchActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SearchActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SearchActionConfig> allDatas = new List<SearchActionConfig>();

            {
                string file = "Skill/SearchActionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int searchPosition_index = reader.GetIndex("searchPosition");
                int searchType_index = reader.GetIndex("searchType");
                int searchPara_index = reader.GetIndex("searchPara");
                int posOff_index = reader.GetIndex("posOff");
                int searchMaxCnt_index = reader.GetIndex("searchMaxCnt");
                int battleRelation_index = reader.GetIndex("battleRelation");
                int actionStrs_index = reader.GetIndex("actionStrs");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SearchActionConfig data = new SearchActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.searchPosition = ((SearchPoint)reader.getInt(i, searchPosition_index, 0));         
						data.searchType = ((SearchSharp)reader.getInt(i, searchType_index, 0));         
						data.searchPara = reader.getFloats(i, searchPara_index, ';');         
						data.posOff = reader.getFloats(i, posOff_index, ';');         
						data.searchMaxCnt = reader.getInt(i, searchMaxCnt_index, 5);         
						data.battleRelation = ((BattleRelation)reader.getInt(i, battleRelation_index, 0));         
						data.actionStrs = reader.getStrs(i, actionStrs_index, ';');         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            {
                string file = "Skill/SearchActionConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int searchPosition_index = reader.GetIndex("searchPosition");
                int searchType_index = reader.GetIndex("searchType");
                int searchPara_index = reader.GetIndex("searchPara");
                int posOff_index = reader.GetIndex("posOff");
                int searchMaxCnt_index = reader.GetIndex("searchMaxCnt");
                int battleRelation_index = reader.GetIndex("battleRelation");
                int actionStrs_index = reader.GetIndex("actionStrs");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SearchActionConfig data = new SearchActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.searchPosition = ((SearchPoint)reader.getInt(i, searchPosition_index, 0));         
						data.searchType = ((SearchSharp)reader.getInt(i, searchType_index, 0));         
						data.searchPara = reader.getFloats(i, searchPara_index, ';');         
						data.posOff = reader.getFloats(i, posOff_index, ';');         
						data.searchMaxCnt = reader.getInt(i, searchMaxCnt_index, 5);         
						data.battleRelation = ((BattleRelation)reader.getInt(i, battleRelation_index, 0));         
						data.actionStrs = reader.getStrs(i, actionStrs_index, ';');         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            {
                string file = "Skill/SearchActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int searchPosition_index = reader.GetIndex("searchPosition");
                int searchType_index = reader.GetIndex("searchType");
                int searchPara_index = reader.GetIndex("searchPara");
                int posOff_index = reader.GetIndex("posOff");
                int searchMaxCnt_index = reader.GetIndex("searchMaxCnt");
                int battleRelation_index = reader.GetIndex("battleRelation");
                int actionStrs_index = reader.GetIndex("actionStrs");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SearchActionConfig data = new SearchActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.searchPosition = ((SearchPoint)reader.getInt(i, searchPosition_index, 0));         
						data.searchType = ((SearchSharp)reader.getInt(i, searchType_index, 0));         
						data.searchPara = reader.getFloats(i, searchPara_index, ';');         
						data.posOff = reader.getFloats(i, posOff_index, ';');         
						data.searchMaxCnt = reader.getInt(i, searchMaxCnt_index, 5);         
						data.battleRelation = ((BattleRelation)reader.getInt(i, battleRelation_index, 0));         
						data.actionStrs = reader.getStrs(i, actionStrs_index, ';');         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("SearchActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(SearchActionConfig);
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


