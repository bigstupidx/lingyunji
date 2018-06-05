namespace Config
{
    // 物品类型
    public enum ItemType
    {
        showItem = -1,          // 只用于图标显示
        consumables = 1,        // 消耗品
        task = 2,               // 任务道具
        money = 3,              // 货币
        equip = 4,              // 装备
    }

    // 道具子类型
    public enum ItemChildType
    {
        general = 0,                        // 通用
        petSkillBook = 1,                   // 灵兽技能书
        petSuperSkill = 2,                  // 灵兽绝技书(可和2统一)
        treasure = 3,                       // 宝图
        heroInvitation = 4,                 // 英雄帖
        playerSuperSkill = 5,               // 秘籍（主角技能书）
        moneyTree = 6,                      // 摇钱树
        moneyTreeUp = 7,                    // 摇钱树加速
        soul = 8,                           // 魂魄(宝石)
        xiuliandan = 9,                     // 修炼丹(氏族)
        collectItem = 10,                   // 采集道具
        generalMaterial = 11,               // 通用材料类型
        cure = 12,                          // 治疗类药品
        petDrug = 13,                       // 宠物药品
        bloodPool = 14,                     // 角色血池储备药

        petExpDrug = 21,                    // 灵兽经验丹
        petSoul = 22,                       // 灵兽元魂
        petResetAttrItem = 23,              // 灵兽洗炼道具
        petTrainItem = 24,                  // 灵兽培养道具
        petPersonalityResetItem = 25,       // 灵兽性格重置道具
        petTogetherItem = 26,               // 灵兽融会道具
        petLockSkillItem = 27,              // 灵兽锁定技能道具
        petOpenGridItem = 28,               // 灵兽槽位开启道具
        petAddPointResetItem = 29,          // 灵兽加点重置道具

        equipStrengthenItem = 31,           // 装备强化道具
        equipTrainAttrItem = 32,            // 炼化道具(装备炼化)
        equipResetAttrItem = 33,            // 装备重铸道具
        equipTogetherAttrItem = 34,         // 装备凝练道具
        equipBuildItem = 35,                // 装备打造道具(升级)

        treasureItem = 41,                  // 法宝道具
        treasureSoulMaterial = 42,          // 法宝注灵材料
        treasureTrainMaterial = 43,         // 法宝潜修材料
        treasureSkillUpMeterial = 44,       // 法宝技能升级材料
    }

    // 背包类型
    public enum BagType
    {
        item = 1,                           // 道具背包
        task = 2,                           // 任务背包
        temp = 3,                           // 临时背包
        guild = 4,                          // 氏族仓库
        mail = 5,                           // 邮件
    }

    // 道具品质类型
    public enum ItemQuality
    {
        white = 1,                          // 白
        green = 2,                          // 绿
        blue = 3,                           // 蓝
        purple = 4,                         // 紫
        Orange = 5,                         // 橙
        red = 6,                            // 红
    }

    // 合成类型
    public enum ItemCompositeType
    {
        common = 1,                         // 通用合成
        stone = 2,                          // 强化石
        soul = 3,                           // 魂魄
    }

    // 获得道具tips类型
    public enum GetItemTipsType
    {
        NULL = 0,
        common_reward = 1,                  // 通用奖励
        exchangeStore = 2,                  // 商城
        demonplost = 3,                     // 法宝
        activity = 4,                       // 活动
        mail = 5,                           // 邮件
        welfares = 6,                       // 福利
    }

    public class ItemCount
    {
        public int id; // ID
        public int count; // 数量

        public static ItemCount InitConfig(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            try
            {
                string[] src = text.Split('|');
                ItemCount item = new ItemCount();
                item.id = int.Parse(src[0]);
                item.count = int.Parse(src[1]);
                return item;
            }
            catch (System.Exception ex)
            {
                Log.Error("ItemCount text:{0}", text);
                Log.Exception(ex);
                throw ex;
            }
        }
    }
}
