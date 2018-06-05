#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using System.Collections.Generic;
    using Config;

    public class PackageMgr
    {
        public PackageMgr()
        {
            package = new GridMgr();
            tempPackage = new GridMgr();
            taskPackage = new GridMgr();
        }

        public void Reset(PackageList pl)
        {
            package.Reset(pl.package);
            tempPackage.Reset(pl.tempPackage);
            taskPackage.Reset(pl.taskPackage);
        }

        public GridMgr package { get; private set; }
        public GridMgr tempPackage { get; private set; }
        public GridMgr taskPackage { get; private set; }

        // 获取物品数量
        public int GetItemCount(int id)
        {
            ItemBase itemConfig = ItemBaseAll.Get(id);
            if (itemConfig == null)
            {
                Log.Debug("GetItemCount, item id is invalid id = {0}", id);
                return 0;
            }

            if (itemConfig.type == ItemType.money)
            {
                // 货币类型
                var attType = ItemHelp.ItemToMoneyType(id);
                if (attType == AttType.AT_Null)
                {
                    Log.Error("SubItem item id = {0} money is null!", id);
                    return 0;
                }

                return (int)App.my.localPlayer.GetMoney(attType);
            }

            if (itemConfig.type == ItemType.task)
                return taskPackage.GetItemCount(id);
            else
                return package.GetItemCount(id);
        }

        // 获得某个物品的真实数量，不包含绑定关系道具
        public int GetItemRealCount(int id)
        {
            ItemBase itemConfig = ItemBaseAll.Get(id);
            if (itemConfig == null)
            {
                Log.Debug("GetItemCount, item id is invalid id = {0}", id);
                return 0;
            }

            if (itemConfig.type == ItemType.money)
            {
                // 货币类型
                var attType = ItemHelp.ItemToMoneyType(id);
                if (attType == AttType.AT_Null)
                {
                    Log.Error("SubItem item id = {0} money is null!", id);
                    return 0;
                }

                return (int)App.my.localPlayer.GetMoney(attType);
            }

            if (itemConfig.type == ItemType.task)
                return taskPackage.GetItemRealCount(id);
            else
                return package.GetItemRealCount(id);
        }

        // 根据格子索引获取格子物品信息
        public ItemGrid GetItemInfo(int index)
        {
            return package.GetItemInfo(index);
        }

        // 判断背包满
        public bool isFullOfPackage()
        {
            // 背包格子满
            if (package.isFull())
                return true;
            return false;
        }

        // 判断任务背包满
        public bool isFullOfTaskPackage()
        {
            // 背包格子满
            if (taskPackage.isFull())
                return true;
            return false;
        }

        // 判断临时背包满
        public bool isFullOfTempPackage()
        {
            // 背包格子满
            if (tempPackage.isFull())
                return true;
            return false;
        }
        
        // 注意：该接口不提供给包裹模块以外的模块使用
        public void ArrangePackage()
        {
            package.Arrange();
        }

        // 是否能合并道具
        public bool CanCombineItem(int itemId)
        {
            // 找出绑定道具
            int bindId = 0;
            int unBindId = 0;
            ItemBase itemConfig = ItemBaseAll.Get(itemId);
            if (itemConfig == null)
                return false;
            if (itemConfig.isBind)
                bindId = itemId;
            else
            {
                bindId = Item.Get(itemId).bindId;
                if (ItemBaseAll.Get(bindId) == null)
                    return false;
            }

            // 找出非绑定道具
            unBindId = Item.Get(bindId).unbindId;
            if (ItemBaseAll.Get(unBindId) == null)
                return false;

            if (package.GetItemRealCount(bindId) == 0)
                return false;
            if (package.GetItemRealCount(unBindId) == 0)
                return false;

            return true;
        }

        // 合并道具
        public bool CombineItem(int bindId, int unBindId)
        {
            return package.CombineItem(bindId, unBindId);
        }

        // 使用道具
        public bool UseItemById(int id, int count)
        {
            List<int> list = new List<int>();
            package.GetItemPosition(id, count, list);
            if (list.Count == 0)
                return false;

            ItemGrid gridInfo = GetItemInfo(list[0]);
            if (gridInfo == null)
                return false;

            return UseItemByIndex(list[0], count);
        }

        // 使用道具
        public bool UseItemByIndex (int index, int count)
        {
            ItemGrid gridInfo = GetItemInfo(index);
            if (gridInfo == null)
                return false;

            xys.hot.ItemFunction itemFunc = new xys.hot.ItemFunction();
            xys.hot.ItemFuncObject obj = new xys.hot.ItemFuncObject();

            obj.GridIndex = index;
            obj.itemId = gridInfo.data.id;
            obj.itemCount = count;

            itemFunc.ItemUseFuncBtn(obj);

            return true;
        }
    }
}
#endif