#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using NetProto.Hot;
    using Config;
    using System.Collections.Generic;

    public class ShowPetPanelObject
    {
        public ShowPetPanelObject()
        {

        }

        public int itemId {set; get;}
    }

    public class ItemFuncObject
    {
        int m_Index;
        BagType m_BagType;

        public int GridIndex { get { return m_Index; } set { m_Index = value; } }
        public BagType PackageType { get { return m_BagType; } set { m_BagType = value; } }
        public int retIndex { get; set; }
        public int itemId { get; set; }
        public int itemCount { get; set; }
    }

    public class ItemFunction
    {
        xys.UI.Dialog.TwoBtn m_Screen;

        PackageMgr packageMgr;

        HotEquipModule m_HotEquipModule;

        C2APackageRequest request_;
        C2APackageRequest request
        {
            get
            {
                if (request_ == null)
                    request_ = hotApp.my.GetModule<HotPackageModule>().request;

                return request_;
            }
        }

        public ItemFunction()
        {
            packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            m_HotEquipModule = hotApp.my.GetModule<HotEquipModule>();
        }

        // 使用
        public void ItemUseFuncBtn(ItemFuncObject useOjb)
        {
            CloseTips();

            ItemGrid itemData = packageMgr.GetItemInfo(useOjb.GridIndex);
            if (itemData == null || itemData.data == null)
                return;
            if (!ItemUseCondition(itemData.data.id))
                return;

            Item config = Item.Get(itemData.data.id);
            if (config == null)
                return;

            if (config.singTimer != 0)
            {
                xys.UI.ProgressData barData = new xys.UI.ProgressData();
                barData.timeLenght = config.singTimer;
                barData.timeBegin = 0;
                barData.rightToLeft = true;
                barData.titleName = "使用中";
                barData.finishEvent = () =>
                {
                    ProgessbarAction(useOjb);
                };

                App.my.eventSet.FireEvent<xys.UI.ProgressData>(EventID.Package_ProgessBar, barData);
            }
            else
            {
                ProgessbarAction(useOjb);
            }
        }

        void ProgessbarAction(ItemFuncObject useOjb)
        {
            ItemGrid itemData = packageMgr.GetItemInfo(useOjb.GridIndex);
            if (itemData == null || itemData.data == null)
                return;

            ItemBase itemConfig = ItemBaseAll.Get(itemData.data.id);
            if (itemConfig == null)
                return;
            if (itemConfig.type == ItemType.consumables)
                UseConsumables(useOjb.GridIndex, useOjb.itemId, useOjb.itemCount);
            else if (itemConfig.type == ItemType.task)
                UseTaskItem(useOjb.GridIndex);
        }

        // 使用全部
        public void ItemUseAll(ItemFuncObject useObj)
        {
            CloseTips();

            request.UseAll(new Int32() { value = useObj.GridIndex }, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;
             });
        }

        // 装备血瓶
        public void EquipCureItem(ItemFuncObject obj)
        {
            CloseTips();

            request.WearBloodBottle(new Int32() { value = obj.itemId }, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.value)
                {
                    App.my.eventSet.FireEvent<int>(EventID.BloodPoolWearHp, obj.itemId);

                    if (obj.itemId == 0)
                        return;

                    ItemBase config = ItemBaseAll.Get(obj.itemId);
                    if (config == null)
                        return;
                    Config.TipsContent tipsConfig = Config.TipsContent.Get(3101);
                    if (tipsConfig == null)
                        return;

                    QualitySourceConfig qualityConfig = QualitySourceConfig.Get(config.quality);
                    string strName = string.Format("#[{0}]{1}#n", qualityConfig.colorname, config.name);
                    xys.UI.SystemHintMgr.ShowHint(string.Format(tipsConfig.des, strName));
                }
            });
        }

        // 分解道具
        public void DecomposeItem(ItemFuncObject obj)
        {
            CloseTips();

            if (m_Screen != null)
                return;

            TipsContent tipsConfig = TipsContent.Get(3121);
            if (tipsConfig == null)
                return;

            m_Screen = xys.UI.Dialog.TwoBtn.Show(
                "",
                string.Format(GlobalSymbol.ToUT(tipsConfig.des)),
                "取消", () => false,
                "确定", () =>
                {
                    DecomposeItemReq input = new DecomposeItemReq();
                    input.index.Add(obj.GridIndex);
                    request.DecomposeItem(input, (error, respone) =>
                    {
                        if (error != wProtobuf.RPC.Error.Success)
                            return;

                        if (respone.code == ReturnCode.Package_NotEnoughSpace)
                        {
                            Config.TipsContent config = Config.TipsContent.Get(3110);
                            if (config == null)
                                return;

                            xys.UI.SystemHintMgr.ShowHint(config.des);
                        }
                    });

                    return false;
                }, true, true, () => m_Screen = null);
        }

        // 批量分解
        public void DecomposeEquips(ItemFuncObject obj)
        {
            CloseTips();

            App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIDecomposePanel, obj);
        }

        // 合成
        public void CompoundItem(ItemFuncObject obj)
        {
            CloseTips();

            App.my.uiSystem.ShowPanel(xys.UI.PanelType.UICompoundPanel, obj);
        }

        // 从临时背包提取物品
        public void GetItemFromTempPackage(ItemFuncObject useObj)
        {


            request.GetItemFromTemp(new Int32() { value = useObj.GridIndex }, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
            });
        }

        // 丢弃
        public void ItemRejectFuncBtn(ItemFuncObject reject)
        {
            CloseTips();

            if (m_Screen != null)
                return;

            TipsContent tipsConfig = TipsContent.Get(3107);
            if (tipsConfig == null)
                return;

            m_Screen = xys.UI.Dialog.TwoBtn.Show(
                "",
                string.Format(GlobalSymbol.ToUT(tipsConfig.des)),
                "取消", () => false,
                "确定", () =>
                {
                    if (reject.PackageType == BagType.item)
                    {
                        request.RejectItem(new Int32() { value = reject.GridIndex }, (error, respone) =>
                                   {
                                       if (error != wProtobuf.RPC.Error.Success)
                                           return;
                                   });
                    }
                    else if (reject.PackageType == BagType.temp)
                    {
                        request.RejectTempItem(new Int32() { value = reject.GridIndex }, (error, respone) =>
                        {
                            if (error != wProtobuf.RPC.Error.Success)
                                return;
                        });
                    }

                    return false;
                }, true, true, () => m_Screen = null);
        }

        public void SellToStore(ItemFuncObject obj)
        {
            CloseTips();

            xys.UI.SystemHintMgr.ShowHint("功能暂未开放");
        }

        // 道具出售
        public void SellItem(ItemFuncObject funcObj)
        {
            CloseTips();

            ShangHuiItem config = ShangHuiItem.Get(funcObj.itemId);
            if (config == null)
            {// 出售给系统
                request.SellItem(new Int32() { value = funcObj.GridIndex }, (error, respone) =>
                {
                    if (error != wProtobuf.RPC.Error.Success)
                        return;
                    if (respone.value)
                    {
                        TipsContent tipsConfig = TipsContent.Get(3114);
                        if (tipsConfig == null)
                            return;
                        xys.UI.SystemHintMgr.ShowHint(tipsConfig.des);
                    }
                });
            }
            else
            {
                App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIQuickSellPanel, funcObj.GridIndex);
            }
        }

        // 装备
        public void EquipEquipment(ItemFuncObject funcObj)
        {
            CloseTips();

            m_HotEquipModule.LoadEquip(funcObj.GridIndex);
        }

        // 卸 下
        public void RemoveEquioment(ItemFuncObject funcObj)
        {
            CloseTips();

            EquipPrototype equipConfig = EquipPrototype.Get(funcObj.itemId);
            if (equipConfig == null)
                return;

            m_HotEquipModule.UnLoadEquip(equipConfig.sonType);
        }

        // 装备出售
        public void SellEquiment(ItemFuncObject funcObj)
        {
            CloseTips();

            Int32 intput = new Int32();
            intput.value = funcObj.GridIndex;
            request.SellItem(intput, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
            });
        }

        // 装备炼化
        public void RefinEquiment(ItemFuncObject funcObj)
        {
            CloseTips();
            //App.my.uiSystem.HidePanel(xys.UI.PanelType.UIPackagePanel);
            App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIEquipPanel, funcObj.GridIndex);
        }

        // 合并
        public void RelationItem(ItemFuncObject funcObj)
        {
            CloseTips();

            if (m_Screen != null)
                return;

            TipsContent tipsConfig = TipsContent.Get(3108);
            if (tipsConfig == null)
                return;

            ItemBase itemConfig = ItemBaseAll.Get(funcObj.itemId);
            if (itemConfig == null)
                return;
            QualitySourceConfig qualityConfig = QualitySourceConfig.Get(itemConfig.quality);

            string nameStr = string.Format("#[{0}]{1}#n", qualityConfig.colorname, itemConfig.name);
 
            m_Screen = xys.UI.Dialog.TwoBtn.Show(
                "",
                string.Format(GlobalSymbol.ToUT(tipsConfig.des), nameStr),
                "取消", () => false,
                "确定", () =>
                {
                    Int32 input = new Int32();
                    input.value = funcObj.itemId;
                    request.CombineRelationItem(input, (error, respone) =>
                    {
                        if (error != wProtobuf.RPC.Error.Success)
                            return;
                        if (respone.ret == 0)
                        {
                            packageMgr.CombineItem(respone.bindId, respone.unBindId);
                            App.my.eventSet.fireEvent(EventID.Package_UpdatePackage);
                        }
                    });

                    return false;
                }, true, true, () => m_Screen = null);
        }

        // 

        // 使用条件
        bool ItemUseCondition(int id)
        {
            ItemBase baseConfig = ItemBaseAll.Get(id);
            if (baseConfig == null)
                return false;

            return baseConfig.isCanUseCondition();
        }

        // 使用消耗品
        void UseConsumables(int index, int itemId, int itemCount)
        {
            Item itemConfig = Item.Get(itemId);
            if (itemConfig == null)
                return;
            if (itemConfig.type != ItemType.consumables)
                return;

            switch (itemConfig.sonType)
            {
                case (int)ItemChildType.petExpDrug:
                case (int)ItemChildType.petResetAttrItem:
                case (int)ItemChildType.petTrainItem:
                case (int)ItemChildType.petPersonalityResetItem:
                case (int)ItemChildType.petTogetherItem:
                case (int)ItemChildType.petLockSkillItem:
                case (int)ItemChildType.petOpenGridItem:
                case (int)ItemChildType.petAddPointResetItem:
                    int petCount = xys.App.my.localPlayer.GetModule<xys.PetsModule>().GetPetCount();
                    if (petCount == 0)
                        return;
                    ShowPetPanelObject obj = new ShowPetPanelObject();
                    obj.itemId = itemId;
                    xys.App.my.uiSystem.HidePanel(xys.UI.PanelType.UIPackagePanel, false);
                    xys.App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIPetsPanel, obj);
                    break;
                case (int)ItemChildType.moneyTree: // 使用摇钱树种子
                    if(MoneyTreeDef.HasMoneyTreeBelongToMe()) // 已经种树
                    {
                        xys.UI.Utility.TipContentUtil.Show("moneyTree_has_plant_tree");
                    }
                    else // 申请种树
                    {
                        MoneyTreeMgr moneyTreeMgr = App.my.localPlayer.GetModule<MoneyTreeModule>().moneyTreeMgr as MoneyTreeMgr;
                        moneyTreeMgr.UseSeedToPlantTree(itemConfig.cowId, itemId);
                        xys.App.my.uiSystem.HidePanel(xys.UI.PanelType.UIPackagePanel, false);
                    }
                    return;
            }

            // 返回false为纯客户端操作
            if (!itemConfig.Use())
                return;

            // 服务器操作
            UseItemRet input = new UseItemRet();
            input.index = index;
            input.count = itemCount;
            request.UseItem(input, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
            });
        }

        // 使用任务道具
        void UseTaskItem(int index)
        {
            //C2GPackageRequest packageReq = new C2GPackageRequest(App.my.game.local);
            //Int32 request = new Int32();
            //request.value = index;
            //packageReq.UseTaskItem(request, (error, respone) =>
            //{
            //    if (error != wProtobuf.RPC.Error.Success)
            //        return;
            //});
        }

        void CloseTips()
        {
            App.my.uiSystem.HidePanel(xys.UI.PanelType.UIItemTipsPanel);
        }
    }
}
#endif