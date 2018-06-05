// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SoulAttributePrototype 
    {
        static Dictionary<int, SoulAttributePrototype> DataList = new Dictionary<int, SoulAttributePrototype>();

        static public Dictionary<int, SoulAttributePrototype> GetAll()
        {
            return DataList;
        }

        static public SoulAttributePrototype Get(int key)
        {
            SoulAttributePrototype value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SoulAttributePrototype.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 魂魄名称
        public string name { get; set; }

        // 魂魄系别
        public int type { get; set; }

        // 子类型
        public int childType { get; set; }

        // 强化等级
        public int inforceLevel { get; set; }

        // 魂魄品质
        public int quality { get; set; }

        // 强化所需魂能
        public int costNum { get; set; }

        // 额外材料
        public int addMaterId { get; set; }

        // 额外材料数量
        public int addMaterNum { get; set; }

        // 强化成功率
        public int inforceSuccRate { get; set; }

        // 需要角色等级
        public int useLevel { get; set; }

        // 力量
        public SoulAttr sStrength { get; set; }

        // 智慧
        public SoulAttr sIntelligence { get; set; }

        // 根骨
        public SoulAttr sBone { get; set; }

        // 体魄
        public SoulAttr sPhysique { get; set; }

        // 敏捷
        public SoulAttr sAgility { get; set; }

        // 身法
        public SoulAttr sBodyway { get; set; }

        // 额外伤害
        public SoulAttr sExtraDamage { get; set; }

        // 伤害减免
        public SoulAttr sDamageReduce { get; set; }

        // 暴击等级
        public SoulAttr sCritLevel { get; set; }

        // 暴击伤害等级
        public SoulAttr sCritDamageLevel { get; set; }

        // 暴击防御等级
        public SoulAttr sCritDefenseLevel { get; set; }

        // 命中等级
        public SoulAttr sHitLevel { get; set; }

        // 回避等级
        public SoulAttr sAvoidLevel { get; set; }

        // 招架等级
        public SoulAttr sParryLevel { get; set; }

        // 物理穿透等级
        public SoulAttr sPhysicalPenetrateLevel { get; set; }

        // 物理防御
        public SoulAttr sPhysicalDefense { get; set; }

        // 法术穿透等级
        public SoulAttr sMagicPenetrateLevel { get; set; }

        // 法术防御
        public SoulAttr sMagicDefense { get; set; }

        // 生命值
        public SoulAttr sHp { get; set; }

        // 金抗性
        public SoulAttr sMetalResistance { get; set; }

        // 木抗性
        public SoulAttr sWoodResistance { get; set; }

        // 水抗性
        public SoulAttr sWaterResistance { get; set; }

        // 火抗性
        public SoulAttr sFireResistance { get; set; }

        // 土抗性
        public SoulAttr sEarthResistance { get; set; }

        // 金伤害
        public SoulAttr sMetalDamage { get; set; }

        // 木伤害
        public SoulAttr sWoodDamage { get; set; }

        // 水伤害
        public SoulAttr sWaterDamage { get; set; }

        // 火伤害
        public SoulAttr sFireDamage { get; set; }

        // 土伤害
        public SoulAttr sEarthDamageRate { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SoulAttributePrototype);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SoulAttributePrototype);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SoulAttributePrototype> allDatas = new List<SoulAttributePrototype>();

            {
                string file = "Item/SoulAttributePrototype.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int type_index = reader.GetIndex("type");
                int childType_index = reader.GetIndex("childType");
                int inforceLevel_index = reader.GetIndex("inforceLevel");
                int quality_index = reader.GetIndex("quality");
                int costNum_index = reader.GetIndex("costNum");
                int addMaterId_index = reader.GetIndex("addMaterId");
                int addMaterNum_index = reader.GetIndex("addMaterNum");
                int inforceSuccRate_index = reader.GetIndex("inforceSuccRate");
                int useLevel_index = reader.GetIndex("useLevel");
                int sStrength_index = reader.GetIndex("sStrength");
                int sIntelligence_index = reader.GetIndex("sIntelligence");
                int sBone_index = reader.GetIndex("sBone");
                int sPhysique_index = reader.GetIndex("sPhysique");
                int sAgility_index = reader.GetIndex("sAgility");
                int sBodyway_index = reader.GetIndex("sBodyway");
                int sExtraDamage_index = reader.GetIndex("sExtraDamage");
                int sDamageReduce_index = reader.GetIndex("sDamageReduce");
                int sCritLevel_index = reader.GetIndex("sCritLevel");
                int sCritDamageLevel_index = reader.GetIndex("sCritDamageLevel");
                int sCritDefenseLevel_index = reader.GetIndex("sCritDefenseLevel");
                int sHitLevel_index = reader.GetIndex("sHitLevel");
                int sAvoidLevel_index = reader.GetIndex("sAvoidLevel");
                int sParryLevel_index = reader.GetIndex("sParryLevel");
                int sPhysicalPenetrateLevel_index = reader.GetIndex("sPhysicalPenetrateLevel");
                int sPhysicalDefense_index = reader.GetIndex("sPhysicalDefense");
                int sMagicPenetrateLevel_index = reader.GetIndex("sMagicPenetrateLevel");
                int sMagicDefense_index = reader.GetIndex("sMagicDefense");
                int sHp_index = reader.GetIndex("sHp");
                int sMetalResistance_index = reader.GetIndex("sMetalResistance");
                int sWoodResistance_index = reader.GetIndex("sWoodResistance");
                int sWaterResistance_index = reader.GetIndex("sWaterResistance");
                int sFireResistance_index = reader.GetIndex("sFireResistance");
                int sEarthResistance_index = reader.GetIndex("sEarthResistance");
                int sMetalDamage_index = reader.GetIndex("sMetalDamage");
                int sWoodDamage_index = reader.GetIndex("sWoodDamage");
                int sWaterDamage_index = reader.GetIndex("sWaterDamage");
                int sFireDamage_index = reader.GetIndex("sFireDamage");
                int sEarthDamageRate_index = reader.GetIndex("sEarthDamageRate");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SoulAttributePrototype data = new SoulAttributePrototype();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.type = reader.getInt(i, type_index, 0);         
						data.childType = reader.getInt(i, childType_index, 0);         
						data.inforceLevel = reader.getInt(i, inforceLevel_index, 0);         
						data.quality = reader.getInt(i, quality_index, 0);         
						data.costNum = reader.getInt(i, costNum_index, 0);         
						data.addMaterId = reader.getInt(i, addMaterId_index, 0);         
						data.addMaterNum = reader.getInt(i, addMaterNum_index, 0);         
						data.inforceSuccRate = reader.getInt(i, inforceSuccRate_index, 0);         
						data.useLevel = reader.getInt(i, useLevel_index, 0);         
						data.sStrength = SoulAttr.InitConfig(reader.getStr(i, sStrength_index));         
						data.sIntelligence = SoulAttr.InitConfig(reader.getStr(i, sIntelligence_index));         
						data.sBone = SoulAttr.InitConfig(reader.getStr(i, sBone_index));         
						data.sPhysique = SoulAttr.InitConfig(reader.getStr(i, sPhysique_index));         
						data.sAgility = SoulAttr.InitConfig(reader.getStr(i, sAgility_index));         
						data.sBodyway = SoulAttr.InitConfig(reader.getStr(i, sBodyway_index));         
						data.sExtraDamage = SoulAttr.InitConfig(reader.getStr(i, sExtraDamage_index));         
						data.sDamageReduce = SoulAttr.InitConfig(reader.getStr(i, sDamageReduce_index));         
						data.sCritLevel = SoulAttr.InitConfig(reader.getStr(i, sCritLevel_index));         
						data.sCritDamageLevel = SoulAttr.InitConfig(reader.getStr(i, sCritDamageLevel_index));         
						data.sCritDefenseLevel = SoulAttr.InitConfig(reader.getStr(i, sCritDefenseLevel_index));         
						data.sHitLevel = SoulAttr.InitConfig(reader.getStr(i, sHitLevel_index));         
						data.sAvoidLevel = SoulAttr.InitConfig(reader.getStr(i, sAvoidLevel_index));         
						data.sParryLevel = SoulAttr.InitConfig(reader.getStr(i, sParryLevel_index));         
						data.sPhysicalPenetrateLevel = SoulAttr.InitConfig(reader.getStr(i, sPhysicalPenetrateLevel_index));         
						data.sPhysicalDefense = SoulAttr.InitConfig(reader.getStr(i, sPhysicalDefense_index));         
						data.sMagicPenetrateLevel = SoulAttr.InitConfig(reader.getStr(i, sMagicPenetrateLevel_index));         
						data.sMagicDefense = SoulAttr.InitConfig(reader.getStr(i, sMagicDefense_index));         
						data.sHp = SoulAttr.InitConfig(reader.getStr(i, sHp_index));         
						data.sMetalResistance = SoulAttr.InitConfig(reader.getStr(i, sMetalResistance_index));         
						data.sWoodResistance = SoulAttr.InitConfig(reader.getStr(i, sWoodResistance_index));         
						data.sWaterResistance = SoulAttr.InitConfig(reader.getStr(i, sWaterResistance_index));         
						data.sFireResistance = SoulAttr.InitConfig(reader.getStr(i, sFireResistance_index));         
						data.sEarthResistance = SoulAttr.InitConfig(reader.getStr(i, sEarthResistance_index));         
						data.sMetalDamage = SoulAttr.InitConfig(reader.getStr(i, sMetalDamage_index));         
						data.sWoodDamage = SoulAttr.InitConfig(reader.getStr(i, sWoodDamage_index));         
						data.sWaterDamage = SoulAttr.InitConfig(reader.getStr(i, sWaterDamage_index));         
						data.sFireDamage = SoulAttr.InitConfig(reader.getStr(i, sFireDamage_index));         
						data.sEarthDamageRate = SoulAttr.InitConfig(reader.getStr(i, sEarthDamageRate_index));         
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
                    CsvCommon.Log.Error("SoulAttributePrototype.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(SoulAttributePrototype);
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


