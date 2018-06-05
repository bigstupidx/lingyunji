// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanBaiBaoConfig 
    {
        static List<ClanBaiBaoConfig> DataList = new List<ClanBaiBaoConfig>();
        static public List<ClanBaiBaoConfig> GetAll()
        {
            return DataList;
        }

        static public ClanBaiBaoConfig Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].buildLv == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("ClanBaiBaoConfig.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].buildLv == key)
                    return i;
            }
            return -1;
        }



        // 百宝阁等级
        public int buildLv { get; set; }

        // 氏族要求等级
        public int clanLv { get; set; }

        // 升级需求资金
        public int needGold { get; set; }

        // 升级消耗资金
        public int needUseGold { get; set; }

        // 升级时间
        public int time { get; set; }

        // 每周抽奖次数上限
        public int lotteryTime { get; set; }

        // 氏族宝物掉率
        public float rate { get; set; }

        // 描述1
        public string tips1 { get; set; }

        // 描述2
        public string tpis2 { get; set; }

        // 描述3
        public string tips3 { get; set; }

        // 建筑名
        public string name { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ClanBaiBaoConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanBaiBaoConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanBaiBaoConfig> allDatas = new List<ClanBaiBaoConfig>();

            {
                string file = "Clan/ClanBaiBaoConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int buildLv_index = reader.GetIndex("buildLv");
                int clanLv_index = reader.GetIndex("clanLv");
                int needGold_index = reader.GetIndex("needGold");
                int needUseGold_index = reader.GetIndex("needUseGold");
                int time_index = reader.GetIndex("time");
                int lotteryTime_index = reader.GetIndex("lotteryTime");
                int rate_index = reader.GetIndex("rate");
                int tips1_index = reader.GetIndex("tips1");
                int tpis2_index = reader.GetIndex("tpis2");
                int tips3_index = reader.GetIndex("tips3");
                int name_index = reader.GetIndex("name");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanBaiBaoConfig data = new ClanBaiBaoConfig();
						data.buildLv = reader.getInt(i, buildLv_index, 0);         
						data.clanLv = reader.getInt(i, clanLv_index, 0);         
						data.needGold = reader.getInt(i, needGold_index, 0);         
						data.needUseGold = reader.getInt(i, needUseGold_index, 0);         
						data.time = reader.getInt(i, time_index, 0);         
						data.lotteryTime = reader.getInt(i, lotteryTime_index, 0);         
						data.rate = reader.getFloat(i, rate_index, 0f);         
						data.tips1 = reader.getStr(i, tips1_index);         
						data.tpis2 = reader.getStr(i, tpis2_index);         
						data.tips3 = reader.getStr(i, tips3_index);         
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
                    var curType = typeof(ClanBaiBaoConfig);
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


