// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class Item : ItemBase
    {
        static Dictionary<int, Item> DataList = new Dictionary<int, Item>();

        static public Dictionary<int, Item> GetAll()
        {
            return DataList;
        }

        static public Item Get(int key)
        {
            Item value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("Item.Get({0}) not find!", key);
            return null;
        }



        // 对应绑定物品ID
        public int bindId { get; set; }

        // 对应不绑定物品ID
        public int unbindId { get; set; }

        // 是否可使用
        public bool isCanUse { get; set; }

        // 全部使用
        public bool allUse { get; set; }

        // 不可丢弃
        public bool unLost { get; set; }

        // 不可出售
        public bool isCanSell { get; set; }

        // 售价-银贝
        public int priceSilver { get; set; }

        // 售价-金贝
        public int priceGold { get; set; }

        // 售价-碧玉
        public int priceBiyu { get; set; }

        // 关联商会道具ID
        public int storeId { get; set; }

        // 贵重物品
        public bool expenItem { get; set; }

        // 是否可寄售
        public bool isConsign { get; set; }

        // 获得后自动使用
        public bool isAutoUse { get; set; }

        // 获得后提示使用
        public bool tipUse { get; set; }

        // 使用后冷却
        public int cooling { get; set; }

        // 合成ID
        public int comId { get; set; }

        // 分解产物
        public int decProduc { get; set; }

        // 道具耐久
        public int enduLevel { get; set; }

        // 触发事件
        public int eventId { get; set; }

        // 资质提升下限
        public int qualMin { get; set; }

        // 资质提升上限
        public int qualMax { get; set; }

        // 成长提升下限
        public float growMin { get; set; }

        // 成长提升上限
        public float growMax { get; set; }

        // 悟性提升
        public PetSavvy perceImpro { get; set; }

        // 灵兽资质要求
        public int[] petQuali { get; set; }

        // 使用次数限制
        public int limitNum { get; set; }

        // 转化值下限
        public int bound { get; set; }

        // 转化值上限
        public int upper { get; set; }

        // 吟唱时间
        public int singTimer { get; set; }

        // 奖励ID
        public int rewardId { get; set; }

        // 灵兽技能
        public int petSkill { get; set; }

        // 激活宠物
        public int actPet { get; set; }

        // 激活法宝
        public int actWeapon { get; set; }

        // 附魂ID
        public int soulId { get; set; }

        // 增加\n血量
        public int addHp { get; set; }

        // 获得状态
        public int status { get; set; }

        // 法宝潜修经验
        public int weaponExp { get; set; }

        // 激活称号ID
        public int ActTitleId { get; set; }

        // 获得时装ID
        public int fashionId { get; set; }

        // 增加时装期限
        public int addFashTimer { get; set; }

        // 获得坐骑ID
        public int mountId { get; set; }

        // 增加坐骑期限
        public int addMouTimer { get; set; }

        // 摇钱树id
        public int cowId { get; set; }

        // 摇钱树加速时间
        public int cowAccTimer { get; set; }

        // 宝图-点集
        public MapPoints map { get; set; }

        // 奖励生活经验
        public int liveskillexp { get; set; }

        // 生活技能使用等级限制
        public int liveskilllimit { get; set; }

        // 用途
        public string use { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(Item);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(Item);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<Item> allDatas = new List<Item>();

            {
                string file = "Item/Item-Item.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int type_index = reader.GetIndex("type");
                int sonType_index = reader.GetIndex("sonType");
                int quality_index = reader.GetIndex("quality");
                int job_index = reader.GetIndex("job");
                int bindId_index = reader.GetIndex("bindId");
                int unbindId_index = reader.GetIndex("unbindId");
                int isCanUse_index = reader.GetIndex("isCanUse");
                int allUse_index = reader.GetIndex("allUse");
                int unLost_index = reader.GetIndex("unLost");
                int isCanSell_index = reader.GetIndex("isCanSell");
                int priceSilver_index = reader.GetIndex("priceSilver");
                int priceGold_index = reader.GetIndex("priceGold");
                int priceBiyu_index = reader.GetIndex("priceBiyu");
                int storeId_index = reader.GetIndex("storeId");
                int expenItem_index = reader.GetIndex("expenItem");
                int isBind_index = reader.GetIndex("isBind");
                int isConsign_index = reader.GetIndex("isConsign");
                int isAutoUse_index = reader.GetIndex("isAutoUse");
                int tipUse_index = reader.GetIndex("tipUse");
                int useLevel_index = reader.GetIndex("useLevel");
                int stackNum_index = reader.GetIndex("stackNum");
                int cooling_index = reader.GetIndex("cooling");
                int comId_index = reader.GetIndex("comId");
                int decProduc_index = reader.GetIndex("decProduc");
                int enduLevel_index = reader.GetIndex("enduLevel");
                int eventId_index = reader.GetIndex("eventId");
                int qualMin_index = reader.GetIndex("qualMin");
                int qualMax_index = reader.GetIndex("qualMax");
                int growMin_index = reader.GetIndex("growMin");
                int growMax_index = reader.GetIndex("growMax");
                int perceImpro_index = reader.GetIndex("perceImpro");
                int petQuali_index = reader.GetIndex("petQuali");
                int limitNum_index = reader.GetIndex("limitNum");
                int bound_index = reader.GetIndex("bound");
                int upper_index = reader.GetIndex("upper");
                int singTimer_index = reader.GetIndex("singTimer");
                int rewardId_index = reader.GetIndex("rewardId");
                int petSkill_index = reader.GetIndex("petSkill");
                int actPet_index = reader.GetIndex("actPet");
                int actWeapon_index = reader.GetIndex("actWeapon");
                int soulId_index = reader.GetIndex("soulId");
                int addHp_index = reader.GetIndex("addHp");
                int status_index = reader.GetIndex("status");
                int weaponExp_index = reader.GetIndex("weaponExp");
                int ActTitleId_index = reader.GetIndex("ActTitleId");
                int fashionId_index = reader.GetIndex("fashionId");
                int addFashTimer_index = reader.GetIndex("addFashTimer");
                int mountId_index = reader.GetIndex("mountId");
                int addMouTimer_index = reader.GetIndex("addMouTimer");
                int cowId_index = reader.GetIndex("cowId");
                int cowAccTimer_index = reader.GetIndex("cowAccTimer");
                int map_index = reader.GetIndex("map");
                int liveskillexp_index = reader.GetIndex("liveskillexp");
                int liveskilllimit_index = reader.GetIndex("liveskilllimit");
                int icon_index = reader.GetIndex("icon");
                int use_index = reader.GetIndex("use");
                int  desc_index = reader.GetIndex(" desc");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        Item data = new Item();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.type = ((ItemType)reader.getInt(i, type_index, 0));         
						data.sonType = reader.getInt(i, sonType_index, 0);         
						data.quality = ((ItemQuality)reader.getInt(i, quality_index, 0));         
						data.job = JobMask.InitConfig(reader.getStr(i, job_index));         
						data.bindId = reader.getInt(i, bindId_index, 0);         
						data.unbindId = reader.getInt(i, unbindId_index, 0);         
						data.isCanUse = reader.getBool(i, isCanUse_index, false);         
						data.allUse = reader.getBool(i, allUse_index, false);         
						data.unLost = reader.getBool(i, unLost_index, false);         
						data.isCanSell = reader.getBool(i, isCanSell_index, false);         
						data.priceSilver = reader.getInt(i, priceSilver_index, 0);         
						data.priceGold = reader.getInt(i, priceGold_index, 0);         
						data.priceBiyu = reader.getInt(i, priceBiyu_index, 0);         
						data.storeId = reader.getInt(i, storeId_index, 0);         
						data.expenItem = reader.getBool(i, expenItem_index, false);         
						data.isBind = reader.getBool(i, isBind_index, false);         
						data.isConsign = reader.getBool(i, isConsign_index, false);         
						data.isAutoUse = reader.getBool(i, isAutoUse_index, false);         
						data.tipUse = reader.getBool(i, tipUse_index, false);         
						data.useLevel = reader.getInt(i, useLevel_index, 0);         
						data.stackNum = reader.getInt(i, stackNum_index, 0);         
						data.cooling = reader.getInt(i, cooling_index, 0);         
						data.comId = reader.getInt(i, comId_index, 0);         
						data.decProduc = reader.getInt(i, decProduc_index, 0);         
						data.enduLevel = reader.getInt(i, enduLevel_index, 0);         
						data.eventId = reader.getInt(i, eventId_index, 0);         
						data.qualMin = reader.getInt(i, qualMin_index, 0);         
						data.qualMax = reader.getInt(i, qualMax_index, 0);         
						data.growMin = reader.getFloat(i, growMin_index, 0f);         
						data.growMax = reader.getFloat(i, growMax_index, 0f);         
						data.perceImpro = PetSavvy.InitConfig(reader.getStr(i, perceImpro_index));         
						data.petQuali = reader.getInts(i, petQuali_index, ';');         
						data.limitNum = reader.getInt(i, limitNum_index, 0);         
						data.bound = reader.getInt(i, bound_index, 0);         
						data.upper = reader.getInt(i, upper_index, 0);         
						data.singTimer = reader.getInt(i, singTimer_index, 0);         
						data.rewardId = reader.getInt(i, rewardId_index, 0);         
						data.petSkill = reader.getInt(i, petSkill_index, 0);         
						data.actPet = reader.getInt(i, actPet_index, 0);         
						data.actWeapon = reader.getInt(i, actWeapon_index, 0);         
						data.soulId = reader.getInt(i, soulId_index, 0);         
						data.addHp = reader.getInt(i, addHp_index, 0);         
						data.status = reader.getInt(i, status_index, 0);         
						data.weaponExp = reader.getInt(i, weaponExp_index, 0);         
						data.ActTitleId = reader.getInt(i, ActTitleId_index, 0);         
						data.fashionId = reader.getInt(i, fashionId_index, 0);         
						data.addFashTimer = reader.getInt(i, addFashTimer_index, 0);         
						data.mountId = reader.getInt(i, mountId_index, 0);         
						data.addMouTimer = reader.getInt(i, addMouTimer_index, 0);         
						data.cowId = reader.getInt(i, cowId_index, 0);         
						data.cowAccTimer = reader.getInt(i, cowAccTimer_index, 0);         
						data.map = MapPoints.InitConfig(reader.getStr(i, map_index));         
						data.liveskillexp = reader.getInt(i, liveskillexp_index, 0);         
						data.liveskilllimit = reader.getInt(i, liveskilllimit_index, 0);         
						data.icon = reader.getStr(i, icon_index);         
						data.use = reader.getStr(i, use_index);         
						data. desc = reader.getStr(i,  desc_index);         
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
                    CsvCommon.Log.Error("Item.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(Item);
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


