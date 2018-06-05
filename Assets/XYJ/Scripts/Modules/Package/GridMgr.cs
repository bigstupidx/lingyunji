namespace xys
{
    using NetProto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Config;
    public struct Grid
    {
        public Grid(int pos, ItemGrid data)
        {
            this.pos = pos;
            this.data = data;
        }

        public int pos; // 位置索引
        public ItemGrid data; // 物品数据

        public bool isEmpty { get { return data == null ? true : false; } }

        // 注意下，以下接口，前提是要先判断下是否为空
        public bool isID(int id) { return data.data.id == id ? true : false; }
        public int id { get { return data.data.id; } }
        public bool isBind { get { return ItemHelp.GetFlag(data.data, ItemData.Flag.isBind); } }
        public int Count
        {
            get { return data.count; }
        }
        public void SetEmpty()
        {
            data = null;
        }
    }

    public class GridMgr
    {
        public void Reset(ItemGrids items)
        {
            grids = new Grid[items.count];
            for (int i = 0; i < grids.Length; ++i)
                grids[i].pos = i;

            foreach (var itor in items.items)
            {
                grids[itor.Key].data = itor.Value;
            }
        }

        Grid[] grids;

        public int Count { get { return grids.Length; } }

        // 重置大小
        public void ResetSize(int count)
        {
            System.Array.Resize(ref grids, count);
            for (int i = 0; i < grids.Length; ++i)
                grids[i].pos = i;
        }

        // 得到某个物品的数量
        public int GetItemCount(int id)
        {
            int associateId = 0;    // 关联绑定关系道具id
            Item config = Item.Get(id);
            if (config != null)
            {
                if (config.bindId != 0)
                    associateId = config.bindId;
                else if (config.unbindId != 0)
                    associateId = config.unbindId;
            }
            int total = 0;
            foreach (var itor in grids)
            {
                if (itor.data == null)
                    continue;
                if (itor.isID(id))
                    total += itor.Count;
                if (itor.isID(associateId))
                    total += itor.Count;
            }

            return total;
        }

        // 获得某个物品的真实数量，不包含绑定关系道具
        public int GetItemRealCount(int id)
        {
            int total = 0;
            foreach (var itor in grids)
            {
                if (itor.data == null)
                    continue;
                if (itor.isID(id))
                    total += itor.Count;
            }

            return total;
        }

        // 根据格子索引获取格子物品信息(不提供给背包以外系统使用)
        public ItemGrid GetItemInfo(int index)
        {
            return grids[index].data;
        }

        // 通过物品ID，返回此物品ID的位置信息
        public bool GetItemPosition(int id, int count, List<int> pos)
        {
            pos.Clear();
            List<int> normal = new List<int>();
            for (int i = 0; i < grids.Length; ++i)
            {
                if (grids[i].data == null)
                    continue;

                if (!grids[i].isID(id))
                    continue;

                if (!grids[i].isBind)
                {
                    normal.Add(i);
                    continue;
                }

                // 优先查找绑定的
                pos.Add(i);
                count -= grids[i].Count;
                if (count <= 0)
                    return true;
            }

            foreach (int i in normal)
            {
                pos.Add(i);
                count -= grids[i].Count;
                if (count <= 0)
                    return true;
            }

            pos.Clear();
            return false;
        }

        public void SetItemInfo(int index, ItemGrid data)
        {
            grids[index].data = data;
        }

        // 是否满
        public bool isFull()
        {
            foreach (var itor in grids)
            {
                if (itor.isEmpty)
                    return false;
            }

            return true;
        }

        // 是否空
        public bool isEmpty
        {
            get
            {
                foreach (var itor in grids)
                {
                    if (!itor.isEmpty)
                        return false;
                }

                return true;
            }
        }

        public void ForEach(Action<Grid> fun)
        {
            foreach (var itor in grids)
            {
                fun(itor);
            }
        }

        // 整理包裹
        public void Arrange()
        {
            List<ItemGrid> datas = new List<ItemGrid>();
            for (int i = 0; i < grids.Length; ++i)
            {
                if (!grids[i].isEmpty)
                {
                    datas.Add(grids[i].data);
                    grids[i].SetEmpty();
                }
            }

            ItemHelp.Sort(datas);

            for (int i = 0; i < datas.Count; ++i)
                grids[i].data = datas[i];
        }

        // 合并道具
        public bool CombineItem(int bindId, int unBindId)
        {
            foreach (var v in grids)
            {
                if (v.data != null && v.data.data.id == unBindId)
                {
                    v.data.data.id = bindId;
                    v.data.data.SetFlag(ItemData.Flag.isBind, true);
                }
            }

            Arrange();
            return true;
        }
    }
}
