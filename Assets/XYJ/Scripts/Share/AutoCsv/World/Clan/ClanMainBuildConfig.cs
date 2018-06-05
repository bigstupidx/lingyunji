// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanMainBuildConfig 
    {
        static List<ClanMainBuildConfig> DataList = new List<ClanMainBuildConfig>();
        static public List<ClanMainBuildConfig> GetAll()
        {
            return DataList;
        }

        static public ClanMainBuildConfig Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].lv == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("ClanMainBuildConfig.Get({0}) not find!", key);
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



        // 帮会等级
        public int lv { get; set; }

        // 需求资金
        public int needGold { get; set; }

        // 需求活跃度
        public int needAct { get; set; }

        // 建筑等级需求
        public int buildLvNeed { get; set; }

        // 升级消耗资金
        public int levelUpNeedUse { get; set; }

        // 消耗时间
        public int useTime { get; set; }

        // 集贤馆等级上限
        public int jixianMax { get; set; }

        // 金库等级上限
        public int jinkuMax { get; set; }

        // 技坊等级上限
        public int jifangMax { get; set; }

        // 百宝阁等级上限
        public int baibaoMax { get; set; }

        // 维护基数帮会等级
        public float preserveClanNeed { get; set; }

        // 维护基数活跃度
        public int preserveAct { get; set; }

        // 维护基数建筑总等级
        public int preserveBuildSumLv { get; set; }

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
                var type = typeof(ClanMainBuildConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanMainBuildConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanMainBuildConfig> allDatas = new List<ClanMainBuildConfig>();

            {
                string file = "Clan/ClanMainBuildConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int lv_index = reader.GetIndex("lv");
                int needGold_index = reader.GetIndex("needGold");
                int needAct_index = reader.GetIndex("needAct");
                int buildLvNeed_index = reader.GetIndex("buildLvNeed");
                int levelUpNeedUse_index = reader.GetIndex("levelUpNeedUse");
                int useTime_index = reader.GetIndex("useTime");
                int jixianMax_index = reader.GetIndex("jixianMax");
                int jinkuMax_index = reader.GetIndex("jinkuMax");
                int jifangMax_index = reader.GetIndex("jifangMax");
                int baibaoMax_index = reader.GetIndex("baibaoMax");
                int preserveClanNeed_index = reader.GetIndex("preserveClanNeed");
                int preserveAct_index = reader.GetIndex("preserveAct");
                int preserveBuildSumLv_index = reader.GetIndex("preserveBuildSumLv");
                int tips1_index = reader.GetIndex("tips1");
                int tips2_index = reader.GetIndex("tips2");
                int name_index = reader.GetIndex("name");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanMainBuildConfig data = new ClanMainBuildConfig();
						data.lv = reader.getInt(i, lv_index, 0);         
						data.needGold = reader.getInt(i, needGold_index, 0);         
						data.needAct = reader.getInt(i, needAct_index, 0);         
						data.buildLvNeed = reader.getInt(i, buildLvNeed_index, 0);         
						data.levelUpNeedUse = reader.getInt(i, levelUpNeedUse_index, 0);         
						data.useTime = reader.getInt(i, useTime_index, 0);         
						data.jixianMax = reader.getInt(i, jixianMax_index, 0);         
						data.jinkuMax = reader.getInt(i, jinkuMax_index, 0);         
						data.jifangMax = reader.getInt(i, jifangMax_index, 0);         
						data.baibaoMax = reader.getInt(i, baibaoMax_index, 0);         
						data.preserveClanNeed = reader.getFloat(i, preserveClanNeed_index, 0f);         
						data.preserveAct = reader.getInt(i, preserveAct_index, 0);         
						data.preserveBuildSumLv = reader.getInt(i, preserveBuildSumLv_index, 0);         
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
                    var curType = typeof(ClanMainBuildConfig);
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


