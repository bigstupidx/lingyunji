// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanSkillBuildConfig 
    {
        static List<ClanSkillBuildConfig> DataList = new List<ClanSkillBuildConfig>();
        static public List<ClanSkillBuildConfig> GetAll()
        {
            return DataList;
        }

        static public ClanSkillBuildConfig Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].lv == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("ClanSkillBuildConfig.Get({0}) not find!", key);
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



        // 技坊等级
        public int lv { get; set; }

        // 帮会要求等级
        public int needClanLv { get; set; }

        // 升级需求资金
        public int needGold { get; set; }

        // 升级消耗资金
        public int useGold { get; set; }

        // 升级时间
        public int time { get; set; }

        // 开启技能个数
        public int openSkillMax { get; set; }

        // 开启技能等级上限
        public int openSkillLvMax { get; set; }

        // 描述1
        public string tips1 { get; set; }

        // 描述2
        public string tips2 { get; set; }

        // 描述3
        public string tips3 { get; set; }

        // 建筑名
        public string name { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ClanSkillBuildConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanSkillBuildConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanSkillBuildConfig> allDatas = new List<ClanSkillBuildConfig>();

            {
                string file = "Clan/ClanSkillBuildConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int lv_index = reader.GetIndex("lv");
                int needClanLv_index = reader.GetIndex("needClanLv");
                int needGold_index = reader.GetIndex("needGold");
                int useGold_index = reader.GetIndex("useGold");
                int time_index = reader.GetIndex("time");
                int openSkillMax_index = reader.GetIndex("openSkillMax");
                int openSkillLvMax_index = reader.GetIndex("openSkillLvMax");
                int tips1_index = reader.GetIndex("tips1");
                int tips2_index = reader.GetIndex("tips2");
                int tips3_index = reader.GetIndex("tips3");
                int name_index = reader.GetIndex("name");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanSkillBuildConfig data = new ClanSkillBuildConfig();
						data.lv = reader.getInt(i, lv_index, 0);         
						data.needClanLv = reader.getInt(i, needClanLv_index, 0);         
						data.needGold = reader.getInt(i, needGold_index, 0);         
						data.useGold = reader.getInt(i, useGold_index, 0);         
						data.time = reader.getInt(i, time_index, 0);         
						data.openSkillMax = reader.getInt(i, openSkillMax_index, 0);         
						data.openSkillLvMax = reader.getInt(i, openSkillLvMax_index, 0);         
						data.tips1 = reader.getStr(i, tips1_index);         
						data.tips2 = reader.getStr(i, tips2_index);         
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
                    var curType = typeof(ClanSkillBuildConfig);
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


