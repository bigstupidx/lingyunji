using Config;
using NetProto;
using System.Collections.Generic;

public static class ItemHelp
{
    static ItemHelp()
    {
        MoneyItems = new Dictionary<int, AttType>();
        ItemMoneys = new Dictionary<AttType, int>();

        // 货币对应的属性ID
        MoneyItems.Add(1, AttType.AT_SilverShell); // 银贝
        MoneyItems.Add(2, AttType.AT_GoldShell); // 金贝
        MoneyItems.Add(3, AttType.AT_JasperJade); // 碧玉
        MoneyItems.Add(7, AttType.AT_XiuWei); // 修为

        MoneyItems.Add(15, AttType.AT_Organization); // 门派贡献
        MoneyItems.Add(16, AttType.AT_GongXun); // 功勋
        MoneyItems.Add(17, AttType.AT_Honor); // 荣誉
        MoneyItems.Add(18, AttType.AT_Chivalrous); // 侠义值
        MoneyItems.Add(19, AttType.AT_TianPing); // 天平战积分
        MoneyItems.Add(20, AttType.AT_Family); // 氏族贡献
        MoneyItems.Add(21, AttType.AT_FairyJade); // 仙玉
        MoneyItems.Add(23, AttType.AT_Energy); // 活力
        MoneyItems.Add(24, AttType.AT_XinHuo); // 薪火值

        // 属性ID对应货币
        ItemMoneys.Add(AttType.AT_SilverShell, 1); // 银贝
        ItemMoneys.Add(AttType.AT_GoldShell, 2); // 金贝
        ItemMoneys.Add(AttType.AT_JasperJade, 3); // 碧玉
        ItemMoneys.Add(AttType.AT_XiuWei, 7); // 修为

        ItemMoneys.Add(AttType.AT_Organization, 15); // 门派贡献
        ItemMoneys.Add(AttType.AT_GongXun, 16); // 功勋
        ItemMoneys.Add(AttType.AT_Honor, 17); // 荣誉
        ItemMoneys.Add(AttType.AT_Chivalrous, 18); // 侠义值
        ItemMoneys.Add(AttType.AT_TianPing, 18); // 天平战积分
        ItemMoneys.Add(AttType.AT_Family, 20); // 氏族贡献
        ItemMoneys.Add(AttType.AT_FairyJade, 21); // 仙玉
        ItemMoneys.Add(AttType.AT_Energy, 23); // 活力
        ItemMoneys.Add(AttType.AT_XinHuo, 24); // 薪火值
    }

    static Dictionary<int, AttType> MoneyItems;
    static Dictionary<AttType, int> ItemMoneys;

    public static int GetCount(int total, int maxNum)
    {
        return (total / maxNum) + (total % maxNum == 0 ? 0 : 1);
    }

    // 道具对应属性类型
    public static AttType ItemToMoneyType(int id)
    {
        AttType at = AttType.AT_Null;
        if (MoneyItems.TryGetValue(id, out at))
            return at;
        return AttType.AT_Null;
    }

    // 属性类型对应道具Id
    public static int MoneyTypeToItem(AttType attr)
    {
        int id = 0;
        if (ItemMoneys.TryGetValue(attr, out id))
            return id;
        return 0;
    }

    public static void SetFlag(this ItemData data, ItemData.Flag f, bool value)
    {
        if (value == true)
        {
            data.flags = data.flags | (1 << (int)f);
        }
        else
        {
            data.flags = data.flags & (~(1 << (int)f));
        }
    }

    public static bool GetFlag(this ItemData data, ItemData.Flag f)
    {
        return (data.flags & (1 << (int)f)) == 0 ? false : true;
    }

    class TempData
    {
        public ItemBase config = null;
        public List<ItemGrid> grids = new List<ItemGrid>();

        // 合并
        public void Combine(List<PackData> datas)
        {
            ItemGrid item = grids[0];
            int stackNum = config.stackNum;
            if (stackNum <= 0)
                stackNum = 1;
            for (int i = 1; i < grids.Count; ++i)
            {
                var grid = grids[i];

                int total = grid.count + item.count;
                if (total == stackNum)
                {
                    item.count = total;
                    datas.Add(new PackData(config, item));
                    if (i >= grids.Count - 1)
                    {
                        item = null;
                        break;
                    }
                }
                else if (total > stackNum)
                {
                    item.count = stackNum; // 已经满了
                    datas.Add(new PackData(config, item));

                    item = grids[i];
                    item.count = total - stackNum;
                }
                else
                {
                    // 还没有满，还可以再加
                    item.count = total;
                }
            }

            if (item != null)
            {
                datas.Add(new PackData(config, item));
            }
        }
    }

    //1）子类型升序排
    //2）装备子类型升序排
    //3）装备等级降序排
    //4）品质降序排
    //5）绑定装备和非绑定装备同类型和子类型排在非绑定后边
    static long SortScoreEquip(ItemGrid grid, EquipPrototype config)
    {
        long v = 1 << 63; // 装备的评分最高
        v |= (long)(((byte)config.sonType) << 55); // 8位，最多255种,子类型
        v |= (long)(((byte)config.equipType) << 47); // 8位，最多255种,装备子类型
        v |= (long)((byte)(255 - config.leve) << 39); // 8位，最多255级
        v |= (long)((byte)(255 - config.quality) << 31); // 8位，最多255级
        v |=  (long)((byte)(config.isBind ? 1 : 0) << 30); // 绑定，与非绑定
        v |= (long)((ushort)config.id); // 再物品ID

        return v;
    }

    // 1）按照子类型排序，子类型升序排列
    // 2）按照品质排序，品质越高，则越靠前
    // 3）按照 绑定物品ID 排序，有对应关系，则排列在一起
    // 4）其次按照ID大小进行排列
    static long SortScoreItem(ItemGrid grid, Item config)
    {
        long v = 0 << 63; // 装备的评分最高
        v |= (long)(((byte)config.sonType) << 55); // 8位，最多255种,子类型
        //v |= (long)(((byte)config.equipType) << 47); // 8位，最多255种,装备子类型
        //v |= (long)((byte)(255 - config.leve) << 39); // 8位，最多255级
        v |= (long)((byte)(255 - config.quality) << 47); // 8位，最多255级
        v |= (long)((byte)(config.isBind ? 1 : 0) << 46); // 绑定，与非绑定
        v |= (long)((byte)(grid.count) << 38);
        v |= (long)((ushort)config.id); // 再物品ID

        return v;
    }

    static long SortScore(ItemGrid grid, ItemBase config)
    {
        if (config.type == ItemType.equip)
        {
            // 装备评分最高
            return SortScoreEquip(grid, (EquipPrototype)config);
        }
        else if (config.type != ItemType.task)
        {
            return SortScoreItem(grid, (Item)config);
        }
        else
        {
            return config.id;
        }
    }

    class PackData
    {
        public ItemBase config;
        public ItemGrid grid;
        public long score;

        public PackData(ItemBase config, ItemGrid grid)
        {
            this.config = config;
            this.grid = grid;
        }

        public void Score()
        {
            score = SortScore(grid, config);
        }
    }

    // 整理
    public static void Sort(List<ItemGrid> grids)
    {
        // 先做合并操作
        Dictionary<int, TempData> bindsCount = new Dictionary<int, TempData>();
        Dictionary<int, TempData> nobindsCount = new Dictionary<int, TempData>();
        List<PackData> datas = new List<PackData>();
        for (int i = 0; i < grids.Count; ++i)
        {
            var config = ItemBaseAll.Get(grids[i].data.id);
            if (config.stackNum == 1)
            {
                // 不可叠加
                datas.Add(new PackData(config, grids[i]));
            }
            else
            {
                Dictionary<int, TempData> d = grids[i].data.GetFlag(ItemData.Flag.isBind) ? bindsCount : nobindsCount;
                TempData td = null;
                if (!d.TryGetValue(config.id, out td))
                {
                    td = new TempData();
                    td.config = config;
                    d.Add(config.id, td);
                }

                td.grids.Add(grids[i]);
            }
        }

        foreach (var itor in bindsCount)
            itor.Value.Combine(datas);

        foreach (var itor in nobindsCount)
            itor.Value.Combine(datas);

        for (int i = 0; i < datas.Count; ++i)
            datas[i].Score();

        // 优先装备排序
        datas.Sort((PackData x, PackData y) => { return x.score.CompareTo(y.score); });

        // 再排序下
        grids.Clear();
        for (int i = 0; i < datas.Count; ++i)
            grids.Add(datas[i].grid);
    }

    // 合并
}