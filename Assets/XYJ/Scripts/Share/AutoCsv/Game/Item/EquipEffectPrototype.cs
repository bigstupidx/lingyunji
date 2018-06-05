// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class EquipEffectPrototype 
    {
        static Dictionary<int, EquipEffectPrototype> DataList = new Dictionary<int, EquipEffectPrototype>();

        static public Dictionary<int, EquipEffectPrototype> GetAll()
        {
            return DataList;
        }

        static public EquipEffectPrototype Get(int key)
        {
            EquipEffectPrototype value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("EquipEffectPrototype.Get({0}) not find!", key);
            return null;
        }



        // 特效ID
        public int id { get; set; }

        // 特效名称
        public string effectName { get; set; }

        // 特效图标
        public string icon { get; set; }

        // 克制特效
        public int restrainEffectId { get; set; }

        // 特效标识
        public int effectSign { get; set; }

        // 关联特效
        public string relatedEffectList { get; set; }

        // 物理伤害提高
        public string sPhysicalDamageEnhance { get; set; }

        // 法术伤害提高
        public string sMagicDamageEnhance { get; set; }

        // 物理伤害降低
        public string sPhysicalDamageReduce { get; set; }

        // 法术伤害降低
        public string sMagicDamageReduce { get; set; }

        // 治疗效果提高
        public string sCureAdd { get; set; }

        // 受治疗效果提高
        public string sCureAddFromOther { get; set; }

        // 暴击伤害率
        public string sCritDamageRate { get; set; }

        // 暴击伤害减免率
        public string sCritDamageReduceRate { get; set; }

        // 回避率
        public string sAvoidRate { get; set; }

        // 金抗性率
        public string sMetalResistanceRate { get; set; }

        // 木抗性率
        public string sWoodResistanceRate { get; set; }

        // 水抗性率
        public string sWaterResistanceRate { get; set; }

        // 火抗性率
        public string sFireResistanceRate { get; set; }

        // 土抗性率
        public string sEarthResistanceRate { get; set; }

        // 全属性抗性率
        public string sResistanceRate { get; set; }

        // 护体
        public string sDefendBody { get; set; }

        // 对灵兽伤害提高
        public string sDamageAddToPet { get; set; }

        // 受灵兽伤害降低
        public string sDamageReduceFromPet { get; set; }

        // 对玩家伤害提高
        public string sDamageAddToPlayer { get; set; }

        // 受玩家伤害降低
        public string sDamageReduceFromPlayer { get; set; }

        // 对怪物物理伤害提高
        public string sPhysicalDamageAddToMonster { get; set; }

        // 对怪物法术伤害提高
        public string sMagicDamageAddToMonster { get; set; }

        // 受怪物伤害降低
        public string sDamageReduceFromMonster { get; set; }

        // 生命值加成
        public string sHpAddition { get; set; }

        // 物理攻击加成
        public string sPhysicalAttackAddition { get; set; }

        // 法术攻击加成
        public string sMagicAttackAddition { get; set; }

        // 力量提高
        public string sStrengthEnhance { get; set; }

        // 智慧提高
        public string sIntelligenceEnhance { get; set; }

        // 根骨提高
        public string sBoneEnhance { get; set; }

        // 体魄提高
        public string sPhysiqueEnhance { get; set; }

        // 敏捷提高
        public string sAgilityEnhance { get; set; }

        // 身法提高
        public string sBodyWayEnhance { get; set; }

        // 当前效果描述
        public string currentEffectDes { get; set; }

        // 特效描述
        public string effectDes { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(EquipEffectPrototype);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(EquipEffectPrototype);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<EquipEffectPrototype> allDatas = new List<EquipEffectPrototype>();

            {
                string file = "Item/EquipEffectPrototype.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int effectName_index = reader.GetIndex("effectName");
                int icon_index = reader.GetIndex("icon");
                int restrainEffectId_index = reader.GetIndex("restrainEffectId");
                int effectSign_index = reader.GetIndex("effectSign");
                int relatedEffectList_index = reader.GetIndex("relatedEffectList");
                int sPhysicalDamageEnhance_index = reader.GetIndex("sPhysicalDamageEnhance");
                int sMagicDamageEnhance_index = reader.GetIndex("sMagicDamageEnhance");
                int sPhysicalDamageReduce_index = reader.GetIndex("sPhysicalDamageReduce");
                int sMagicDamageReduce_index = reader.GetIndex("sMagicDamageReduce");
                int sCureAdd_index = reader.GetIndex("sCureAdd");
                int sCureAddFromOther_index = reader.GetIndex("sCureAddFromOther");
                int sCritDamageRate_index = reader.GetIndex("sCritDamageRate");
                int sCritDamageReduceRate_index = reader.GetIndex("sCritDamageReduceRate");
                int sAvoidRate_index = reader.GetIndex("sAvoidRate");
                int sMetalResistanceRate_index = reader.GetIndex("sMetalResistanceRate");
                int sWoodResistanceRate_index = reader.GetIndex("sWoodResistanceRate");
                int sWaterResistanceRate_index = reader.GetIndex("sWaterResistanceRate");
                int sFireResistanceRate_index = reader.GetIndex("sFireResistanceRate");
                int sEarthResistanceRate_index = reader.GetIndex("sEarthResistanceRate");
                int sResistanceRate_index = reader.GetIndex("sResistanceRate");
                int sDefendBody_index = reader.GetIndex("sDefendBody");
                int sDamageAddToPet_index = reader.GetIndex("sDamageAddToPet");
                int sDamageReduceFromPet_index = reader.GetIndex("sDamageReduceFromPet");
                int sDamageAddToPlayer_index = reader.GetIndex("sDamageAddToPlayer");
                int sDamageReduceFromPlayer_index = reader.GetIndex("sDamageReduceFromPlayer");
                int sPhysicalDamageAddToMonster_index = reader.GetIndex("sPhysicalDamageAddToMonster");
                int sMagicDamageAddToMonster_index = reader.GetIndex("sMagicDamageAddToMonster");
                int sDamageReduceFromMonster_index = reader.GetIndex("sDamageReduceFromMonster");
                int sHpAddition_index = reader.GetIndex("sHpAddition");
                int sPhysicalAttackAddition_index = reader.GetIndex("sPhysicalAttackAddition");
                int sMagicAttackAddition_index = reader.GetIndex("sMagicAttackAddition");
                int sStrengthEnhance_index = reader.GetIndex("sStrengthEnhance");
                int sIntelligenceEnhance_index = reader.GetIndex("sIntelligenceEnhance");
                int sBoneEnhance_index = reader.GetIndex("sBoneEnhance");
                int sPhysiqueEnhance_index = reader.GetIndex("sPhysiqueEnhance");
                int sAgilityEnhance_index = reader.GetIndex("sAgilityEnhance");
                int sBodyWayEnhance_index = reader.GetIndex("sBodyWayEnhance");
                int currentEffectDes_index = reader.GetIndex("currentEffectDes");
                int effectDes_index = reader.GetIndex("effectDes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        EquipEffectPrototype data = new EquipEffectPrototype();
						data.id = reader.getInt(i, id_index, 0);         
						data.effectName = reader.getStr(i, effectName_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.restrainEffectId = reader.getInt(i, restrainEffectId_index, 0);         
						data.effectSign = reader.getInt(i, effectSign_index, 0);         
						data.relatedEffectList = reader.getStr(i, relatedEffectList_index);         
						data.sPhysicalDamageEnhance = reader.getStr(i, sPhysicalDamageEnhance_index);         
						data.sMagicDamageEnhance = reader.getStr(i, sMagicDamageEnhance_index);         
						data.sPhysicalDamageReduce = reader.getStr(i, sPhysicalDamageReduce_index);         
						data.sMagicDamageReduce = reader.getStr(i, sMagicDamageReduce_index);         
						data.sCureAdd = reader.getStr(i, sCureAdd_index);         
						data.sCureAddFromOther = reader.getStr(i, sCureAddFromOther_index);         
						data.sCritDamageRate = reader.getStr(i, sCritDamageRate_index);         
						data.sCritDamageReduceRate = reader.getStr(i, sCritDamageReduceRate_index);         
						data.sAvoidRate = reader.getStr(i, sAvoidRate_index);         
						data.sMetalResistanceRate = reader.getStr(i, sMetalResistanceRate_index);         
						data.sWoodResistanceRate = reader.getStr(i, sWoodResistanceRate_index);         
						data.sWaterResistanceRate = reader.getStr(i, sWaterResistanceRate_index);         
						data.sFireResistanceRate = reader.getStr(i, sFireResistanceRate_index);         
						data.sEarthResistanceRate = reader.getStr(i, sEarthResistanceRate_index);         
						data.sResistanceRate = reader.getStr(i, sResistanceRate_index);         
						data.sDefendBody = reader.getStr(i, sDefendBody_index);         
						data.sDamageAddToPet = reader.getStr(i, sDamageAddToPet_index);         
						data.sDamageReduceFromPet = reader.getStr(i, sDamageReduceFromPet_index);         
						data.sDamageAddToPlayer = reader.getStr(i, sDamageAddToPlayer_index);         
						data.sDamageReduceFromPlayer = reader.getStr(i, sDamageReduceFromPlayer_index);         
						data.sPhysicalDamageAddToMonster = reader.getStr(i, sPhysicalDamageAddToMonster_index);         
						data.sMagicDamageAddToMonster = reader.getStr(i, sMagicDamageAddToMonster_index);         
						data.sDamageReduceFromMonster = reader.getStr(i, sDamageReduceFromMonster_index);         
						data.sHpAddition = reader.getStr(i, sHpAddition_index);         
						data.sPhysicalAttackAddition = reader.getStr(i, sPhysicalAttackAddition_index);         
						data.sMagicAttackAddition = reader.getStr(i, sMagicAttackAddition_index);         
						data.sStrengthEnhance = reader.getStr(i, sStrengthEnhance_index);         
						data.sIntelligenceEnhance = reader.getStr(i, sIntelligenceEnhance_index);         
						data.sBoneEnhance = reader.getStr(i, sBoneEnhance_index);         
						data.sPhysiqueEnhance = reader.getStr(i, sPhysiqueEnhance_index);         
						data.sAgilityEnhance = reader.getStr(i, sAgilityEnhance_index);         
						data.sBodyWayEnhance = reader.getStr(i, sBodyWayEnhance_index);         
						data.currentEffectDes = reader.getStr(i, currentEffectDes_index);         
						data.effectDes = reader.getStr(i, effectDes_index);         
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
                    CsvCommon.Log.Error("EquipEffectPrototype.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(EquipEffectPrototype);
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


