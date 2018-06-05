// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class EquipPrototype : ItemBase
    {
        static Dictionary<int, EquipPrototype> DataList = new Dictionary<int, EquipPrototype>();

        static public Dictionary<int, EquipPrototype> GetAll()
        {
            return DataList;
        }

        static public EquipPrototype Get(int key)
        {
            EquipPrototype value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("EquipPrototype.Get({0}) not find!", key);
            return null;
        }



        // 装备子类型
        public int equipType { get; set; }

        // 检索值
        public int index { get; set; }

        // 装备等级
        public int leve { get; set; }

        // 绑定类型
        public bool bindType { get; set; }

        // 不可出售
        public bool isCanSell { get; set; }

        // 出售价格
        public int priceSilver { get; set; }

        // 耐久度
        public int enduLevel { get; set; }

        // 装备生成类型
        public int iCreateType { get; set; }

        // 分解产物
        public int decProduc { get; set; }

        // 是否可以重铸
        public bool isCanRecast { get; set; }

        // 重铸材料1
        public ItemCount recastMaterialId1 { get; set; }

        // 重铸材料2
        public ItemCount recastMaterialId2 { get; set; }

        // 重铸银贝数量
        public int recastCoinCost { get; set; }

        // 是否可以洗炼
        public bool isCanWash { get; set; }

        // 凝炼材料1
        public ItemCount consiceMaterialId1 { get; set; }

        // 凝炼材料2
        public ItemCount consiceMaterialId2 { get; set; }

        // 凝炼银贝数量
        public int consiceCoinCost { get; set; }

        // 是否可炼化
        public bool isCanRefine { get; set; }

        // 基础属性随机范围
        public RandomRange basicAttributeRandomRange { get; set; }

        // 定制属性库id
        public int customAttributeId { get; set; }

        // 定制属性随机范围
        public RandomRange customAttributeRandomRange { get; set; }

        // 随机属性库id
        public int randomAttributeId { get; set; }

        // 随机属性条目
        public ProbabilityTable randomAttributeNum { get; set; }

        // 随机属性等级
        public int randAttrLv { get; set; }

        // 随机属性范围
        public RandomRange randAttriRange { get; set; }

        // 特效库1id
        public int effecId1 { get; set; }

        // 特效库1几率
        public int effecPro1 { get; set; }

        // 特效库2id
        public int effecId2 { get; set; }

        // 特效库2几率
        public int effecPro2 { get; set; }

        // 特技1id
        public int stuntId1 { get; set; }

        // 特技1几率
        public int stuntId1Probability { get; set; }

        // 特技2id
        public int stuntId2 { get; set; }

        // 特技2几率
        public int stuntId2Probability { get; set; }

        // 强化值
        public int InforceValue { get; set; }

        // 觉醒值
        public int awake { get; set; }

        // 觉醒材料
        public string awakeMater { get; set; }

        // 觉醒属性加成
        public float awakePropAdd { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(EquipPrototype);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(EquipPrototype);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<EquipPrototype> allDatas = new List<EquipPrototype>();

            {
                string file = "Item/Item-EquipPrototype@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int type_index = reader.GetIndex("type");
                int sonType_index = reader.GetIndex("sonType");
                int equipType_index = reader.GetIndex("equipType");
                int index_index = reader.GetIndex("index");
                int quality_index = reader.GetIndex("quality");
                int useLevel_index = reader.GetIndex("useLevel");
                int leve_index = reader.GetIndex("leve");
                int job_index = reader.GetIndex("job");
                int icon_index = reader.GetIndex("icon");
                int stackNum_index = reader.GetIndex("stackNum");
                int isBind_index = reader.GetIndex("isBind");
                int bindType_index = reader.GetIndex("bindType");
                int isCanSell_index = reader.GetIndex("isCanSell");
                int priceSilver_index = reader.GetIndex("priceSilver");
                int enduLevel_index = reader.GetIndex("enduLevel");
                int iCreateType_index = reader.GetIndex("iCreateType");
                int decProduc_index = reader.GetIndex("decProduc");
                int isCanRecast_index = reader.GetIndex("isCanRecast");
                int recastMaterialId1_index = reader.GetIndex("recastMaterialId1");
                int recastMaterialId2_index = reader.GetIndex("recastMaterialId2");
                int recastCoinCost_index = reader.GetIndex("recastCoinCost");
                int isCanWash_index = reader.GetIndex("isCanWash");
                int consiceMaterialId1_index = reader.GetIndex("consiceMaterialId1");
                int consiceMaterialId2_index = reader.GetIndex("consiceMaterialId2");
                int consiceCoinCost_index = reader.GetIndex("consiceCoinCost");
                int isCanRefine_index = reader.GetIndex("isCanRefine");
                int basicAttributeRandomRange_index = reader.GetIndex("basicAttributeRandomRange");
                int customAttributeId_index = reader.GetIndex("customAttributeId");
                int customAttributeRandomRange_index = reader.GetIndex("customAttributeRandomRange");
                int randomAttributeId_index = reader.GetIndex("randomAttributeId");
                int randomAttributeNum_index = reader.GetIndex("randomAttributeNum");
                int randAttrLv_index = reader.GetIndex("randAttrLv");
                int randAttriRange_index = reader.GetIndex("randAttriRange");
                int effecId1_index = reader.GetIndex("effecId1");
                int effecPro1_index = reader.GetIndex("effecPro1");
                int effecId2_index = reader.GetIndex("effecId2");
                int effecPro2_index = reader.GetIndex("effecPro2");
                int stuntId1_index = reader.GetIndex("stuntId1");
                int stuntId1Probability_index = reader.GetIndex("stuntId1Probability");
                int stuntId2_index = reader.GetIndex("stuntId2");
                int stuntId2Probability_index = reader.GetIndex("stuntId2Probability");
                int InforceValue_index = reader.GetIndex("InforceValue");
                int awake_index = reader.GetIndex("awake");
                int awakeMater_index = reader.GetIndex("awakeMater");
                int awakePropAdd_index = reader.GetIndex("awakePropAdd");
                int  desc_index = reader.GetIndex(" desc");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        EquipPrototype data = new EquipPrototype();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.type = ((ItemType)reader.getInt(i, type_index, 0));         
						data.sonType = reader.getInt(i, sonType_index, 0);         
						data.equipType = reader.getInt(i, equipType_index, 0);         
						data.index = reader.getInt(i, index_index, 0);         
						data.quality = ((ItemQuality)reader.getInt(i, quality_index, 0));         
						data.useLevel = reader.getInt(i, useLevel_index, 0);         
						data.leve = reader.getInt(i, leve_index, 0);         
						data.job = JobMask.InitConfig(reader.getStr(i, job_index));         
						data.icon = reader.getStr(i, icon_index);         
						data.stackNum = reader.getInt(i, stackNum_index, 0);         
						data.isBind = reader.getBool(i, isBind_index, false);         
						data.bindType = reader.getBool(i, bindType_index, false);         
						data.isCanSell = reader.getBool(i, isCanSell_index, false);         
						data.priceSilver = reader.getInt(i, priceSilver_index, 0);         
						data.enduLevel = reader.getInt(i, enduLevel_index, 0);         
						data.iCreateType = reader.getInt(i, iCreateType_index, 0);         
						data.decProduc = reader.getInt(i, decProduc_index, 0);         
						data.isCanRecast = reader.getBool(i, isCanRecast_index, false);         
						data.recastMaterialId1 = ItemCount.InitConfig(reader.getStr(i, recastMaterialId1_index));         
						data.recastMaterialId2 = ItemCount.InitConfig(reader.getStr(i, recastMaterialId2_index));         
						data.recastCoinCost = reader.getInt(i, recastCoinCost_index, 0);         
						data.isCanWash = reader.getBool(i, isCanWash_index, false);         
						data.consiceMaterialId1 = ItemCount.InitConfig(reader.getStr(i, consiceMaterialId1_index));         
						data.consiceMaterialId2 = ItemCount.InitConfig(reader.getStr(i, consiceMaterialId2_index));         
						data.consiceCoinCost = reader.getInt(i, consiceCoinCost_index, 0);         
						data.isCanRefine = reader.getBool(i, isCanRefine_index, false);         
						data.basicAttributeRandomRange = RandomRange.InitConfig(reader.getStr(i, basicAttributeRandomRange_index));         
						data.customAttributeId = reader.getInt(i, customAttributeId_index, 0);         
						data.customAttributeRandomRange = RandomRange.InitConfig(reader.getStr(i, customAttributeRandomRange_index));         
						data.randomAttributeId = reader.getInt(i, randomAttributeId_index, 0);         
						data.randomAttributeNum = ProbabilityTable.InitConfig(reader.getStr(i, randomAttributeNum_index));         
						data.randAttrLv = reader.getInt(i, randAttrLv_index, 0);         
						data.randAttriRange = RandomRange.InitConfig(reader.getStr(i, randAttriRange_index));         
						data.effecId1 = reader.getInt(i, effecId1_index, 0);         
						data.effecPro1 = reader.getInt(i, effecPro1_index, 0);         
						data.effecId2 = reader.getInt(i, effecId2_index, 0);         
						data.effecPro2 = reader.getInt(i, effecPro2_index, 0);         
						data.stuntId1 = reader.getInt(i, stuntId1_index, 0);         
						data.stuntId1Probability = reader.getInt(i, stuntId1Probability_index, 0);         
						data.stuntId2 = reader.getInt(i, stuntId2_index, 0);         
						data.stuntId2Probability = reader.getInt(i, stuntId2Probability_index, 0);         
						data.InforceValue = reader.getInt(i, InforceValue_index, 0);         
						data.awake = reader.getInt(i, awake_index, 0);         
						data.awakeMater = reader.getStr(i, awakeMater_index);         
						data.awakePropAdd = reader.getFloat(i, awakePropAdd_index, 0f);         
						data. desc = reader.getStr(i,  desc_index);         
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
                    CsvCommon.Log.Error("EquipPrototype.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(EquipPrototype);
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


