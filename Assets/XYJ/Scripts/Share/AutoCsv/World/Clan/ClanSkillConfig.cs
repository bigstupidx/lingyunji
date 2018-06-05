// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanSkillConfig 
    {
        static Dictionary<int, ClanSkillConfig> DataList = new Dictionary<int, ClanSkillConfig>();

        static public Dictionary<int, ClanSkillConfig> GetAll()
        {
            return DataList;
        }

        static public ClanSkillConfig Get(int key)
        {
            ClanSkillConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ClanSkillConfig.Get({0}) not find!", key);
            return null;
        }



        // 修炼ID
        public int id { get; set; }

        // 修炼名称
        public string name { get; set; }

        // 图标
        public string icon { get; set; }

        // 是否开放
        public int isOpen { get; set; }

        // 修炼等级
        public int skillLv { get; set; }

        // 需要工坊等级
        public int needBuildLv { get; set; }

        // 需要角色等级
        public int needRoleLv { get; set; }

        // 单次消耗帮贡
        public int contribution { get; set; }

        // 单次消耗银贝
        public int useMoney { get; set; }

        // 单次所需点数
        public int usePoint { get; set; }

        // 升级所需点数
        public int needPoint { get; set; }

        // 生命值加成
        public float addHp { get; set; }

        // 物理攻击加成
        public float AddAdActtack { get; set; }

        // 法术攻击加成
        public float AddApActtack { get; set; }

        // 金抗性率
        public float jinDefRate { get; set; }

        // 木抗性率
        public float muDefRate { get; set; }

        // 水抗性率
        public float shuiDefRate { get; set; }

        // 火抗性率
        public float huoDefRate { get; set; }

        // 土抗性率
        public float tuDefRate { get; set; }

        // 金伤害率
        public float jinAttRate { get; set; }

        // 木伤害率
        public float muAttRate { get; set; }

        // 水伤害率
        public float shuiAttRate { get; set; }

        // 火伤害率
        public float huoAttRate { get; set; }

        // 土伤害率
        public float tuAttRate { get; set; }

        // 阴伤害率
        public float yinAttRate { get; set; }

        // 阳伤害率
        public float yangAttRate { get; set; }

        // 暴击率
        public float knockRate { get; set; }

        // 暴击防御率
        public float knockDefRate { get; set; }

        // 命中率
        public float hitRate { get; set; }

        // 力量提高
        public int strengthAdd { get; set; }

        // 智慧提高
        public int intelligenceAdd { get; set; }

        // 根骨提高
        public int vitalityAdd { get; set; }

        // 体魄提高
        public int physique { get; set; }

        // 敏捷提高
        public int agilityAdd { get; set; }

        // 身法提高
        public int dexterityAdd { get; set; }

        // 物理伤害降低
        public int AdAttackDown { get; set; }

        // 法术伤害降低
        public int ApAttackDown { get; set; }

        // 修炼描述
        public string dec { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ClanSkillConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanSkillConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanSkillConfig> allDatas = new List<ClanSkillConfig>();

            {
                string file = "Clan/ClanSkillConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int icon_index = reader.GetIndex("icon");
                int isOpen_index = reader.GetIndex("isOpen");
                int skillLv_index = reader.GetIndex("skillLv");
                int needBuildLv_index = reader.GetIndex("needBuildLv");
                int needRoleLv_index = reader.GetIndex("needRoleLv");
                int contribution_index = reader.GetIndex("contribution");
                int useMoney_index = reader.GetIndex("useMoney");
                int usePoint_index = reader.GetIndex("usePoint");
                int needPoint_index = reader.GetIndex("needPoint");
                int addHp_index = reader.GetIndex("addHp");
                int AddAdActtack_index = reader.GetIndex("AddAdActtack");
                int AddApActtack_index = reader.GetIndex("AddApActtack");
                int jinDefRate_index = reader.GetIndex("jinDefRate");
                int muDefRate_index = reader.GetIndex("muDefRate");
                int shuiDefRate_index = reader.GetIndex("shuiDefRate");
                int huoDefRate_index = reader.GetIndex("huoDefRate");
                int tuDefRate_index = reader.GetIndex("tuDefRate");
                int jinAttRate_index = reader.GetIndex("jinAttRate");
                int muAttRate_index = reader.GetIndex("muAttRate");
                int shuiAttRate_index = reader.GetIndex("shuiAttRate");
                int huoAttRate_index = reader.GetIndex("huoAttRate");
                int tuAttRate_index = reader.GetIndex("tuAttRate");
                int yinAttRate_index = reader.GetIndex("yinAttRate");
                int yangAttRate_index = reader.GetIndex("yangAttRate");
                int knockRate_index = reader.GetIndex("knockRate");
                int knockDefRate_index = reader.GetIndex("knockDefRate");
                int hitRate_index = reader.GetIndex("hitRate");
                int strengthAdd_index = reader.GetIndex("strengthAdd");
                int intelligenceAdd_index = reader.GetIndex("intelligenceAdd");
                int vitalityAdd_index = reader.GetIndex("vitalityAdd");
                int physique_index = reader.GetIndex("physique");
                int agilityAdd_index = reader.GetIndex("agilityAdd");
                int dexterityAdd_index = reader.GetIndex("dexterityAdd");
                int AdAttackDown_index = reader.GetIndex("AdAttackDown");
                int ApAttackDown_index = reader.GetIndex("ApAttackDown");
                int dec_index = reader.GetIndex("dec");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanSkillConfig data = new ClanSkillConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.isOpen = reader.getInt(i, isOpen_index, 0);         
						data.skillLv = reader.getInt(i, skillLv_index, 0);         
						data.needBuildLv = reader.getInt(i, needBuildLv_index, 0);         
						data.needRoleLv = reader.getInt(i, needRoleLv_index, 0);         
						data.contribution = reader.getInt(i, contribution_index, 0);         
						data.useMoney = reader.getInt(i, useMoney_index, 0);         
						data.usePoint = reader.getInt(i, usePoint_index, 0);         
						data.needPoint = reader.getInt(i, needPoint_index, 0);         
						data.addHp = reader.getFloat(i, addHp_index, 0f);         
						data.AddAdActtack = reader.getFloat(i, AddAdActtack_index, 0f);         
						data.AddApActtack = reader.getFloat(i, AddApActtack_index, 0f);         
						data.jinDefRate = reader.getFloat(i, jinDefRate_index, 0f);         
						data.muDefRate = reader.getFloat(i, muDefRate_index, 0f);         
						data.shuiDefRate = reader.getFloat(i, shuiDefRate_index, 0f);         
						data.huoDefRate = reader.getFloat(i, huoDefRate_index, 0f);         
						data.tuDefRate = reader.getFloat(i, tuDefRate_index, 0f);         
						data.jinAttRate = reader.getFloat(i, jinAttRate_index, 0f);         
						data.muAttRate = reader.getFloat(i, muAttRate_index, 0f);         
						data.shuiAttRate = reader.getFloat(i, shuiAttRate_index, 0f);         
						data.huoAttRate = reader.getFloat(i, huoAttRate_index, 0f);         
						data.tuAttRate = reader.getFloat(i, tuAttRate_index, 0f);         
						data.yinAttRate = reader.getFloat(i, yinAttRate_index, 0f);         
						data.yangAttRate = reader.getFloat(i, yangAttRate_index, 0f);         
						data.knockRate = reader.getFloat(i, knockRate_index, 0f);         
						data.knockDefRate = reader.getFloat(i, knockDefRate_index, 0f);         
						data.hitRate = reader.getFloat(i, hitRate_index, 0f);         
						data.strengthAdd = reader.getInt(i, strengthAdd_index, 0);         
						data.intelligenceAdd = reader.getInt(i, intelligenceAdd_index, 0);         
						data.vitalityAdd = reader.getInt(i, vitalityAdd_index, 0);         
						data.physique = reader.getInt(i, physique_index, 0);         
						data.agilityAdd = reader.getInt(i, agilityAdd_index, 0);         
						data.dexterityAdd = reader.getInt(i, dexterityAdd_index, 0);         
						data.AdAttackDown = reader.getInt(i, AdAttackDown_index, 0);         
						data.ApAttackDown = reader.getInt(i, ApAttackDown_index, 0);         
						data.dec = reader.getStr(i, dec_index);         
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
                    CsvCommon.Log.Error("ClanSkillConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }


            {
                MethodInfo method = null;
                {
                    var curType = typeof(ClanSkillConfig);
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


