// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanJiXianConfig 
    {
        static List<ClanJiXianConfig> DataList = new List<ClanJiXianConfig>();
        static public List<ClanJiXianConfig> GetAll()
        {
            return DataList;
        }

        static public ClanJiXianConfig Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].lv == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("ClanJiXianConfig.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].lv == key)
                    return i;
            }
            return -1;
        }



        // 集贤馆等级
        public int lv { get; set; }

        // 帮会要求等级
        public int needClanLv { get; set; }

        // 升级需求资金
        public int needGold { get; set; }

        // 升级消耗资金
        public int useGold { get; set; }

        // 升级时间
        public int useTime { get; set; }

        // 帮主人数上限
        public int leaderMax { get; set; }

        // 副帮主人数上限
        public int subLeaderMax { get; set; }

        // 长老人数上限
        public int elderMax { get; set; }

        // 花魁人数上限
        public int oiran { get; set; }

        // 帮会总人数
        public int clanMemerMax { get; set; }

        // 描述1
        public string tips1 { get; set; }

        // 描述2
        public string tips2 { get; set; }

        // 建筑名
        public string name { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ClanJiXianConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanJiXianConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanJiXianConfig> allDatas = new List<ClanJiXianConfig>();

            {
                string file = "Clan/ClanJiXianConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int lv_index = reader.GetIndex("lv");
                int needClanLv_index = reader.GetIndex("needClanLv");
                int needGold_index = reader.GetIndex("needGold");
                int useGold_index = reader.GetIndex("useGold");
                int useTime_index = reader.GetIndex("useTime");
                int leaderMax_index = reader.GetIndex("leaderMax");
                int subLeaderMax_index = reader.GetIndex("subLeaderMax");
                int elderMax_index = reader.GetIndex("elderMax");
                int oiran_index = reader.GetIndex("oiran");
                int clanMemerMax_index = reader.GetIndex("clanMemerMax");
                int tips1_index = reader.GetIndex("tips1");
                int tips2_index = reader.GetIndex("tips2");
                int name_index = reader.GetIndex("name");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanJiXianConfig data = new ClanJiXianConfig();
						data.lv = reader.getInt(i, lv_index, 0);         
						data.needClanLv = reader.getInt(i, needClanLv_index, 0);         
						data.needGold = reader.getInt(i, needGold_index, 0);         
						data.useGold = reader.getInt(i, useGold_index, 0);         
						data.useTime = reader.getInt(i, useTime_index, 0);         
						data.leaderMax = reader.getInt(i, leaderMax_index, 0);         
						data.subLeaderMax = reader.getInt(i, subLeaderMax_index, 0);         
						data.elderMax = reader.getInt(i, elderMax_index, 0);         
						data.oiran = reader.getInt(i, oiran_index, 0);         
						data.clanMemerMax = reader.getInt(i, clanMemerMax_index, 0);         
						data.tips1 = reader.getStr(i, tips1_index);         
						data.tips2 = reader.getStr(i, tips2_index);         
						data.name = reader.getStr(i, name_index);         
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
            
            DataList = allDatas;

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ClanJiXianConfig);
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


