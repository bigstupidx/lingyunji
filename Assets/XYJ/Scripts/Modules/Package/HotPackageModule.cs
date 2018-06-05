#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using NetProto.Hot;
    using Network;
    using System.Collections.Generic;
    using wTimer;

    class HotPackageModule : HotModuleBase
    {
        public HotPackageModule(PackageModule m) : base(m)
        {

        }

        // 背包管理器
        public PackageMgr packageMgr { get; private set; }

        public C2APackageRequest request { get; private set; }

        SimpleTimer m_timer;        //计时器

        protected override void OnAwake()
        {
            packageMgr = new PackageMgr();

            // 注册协议
            this.request = new C2APackageRequest(App.my.socket.game.local);

            hotApp.my.handler.Reg<AllPackageChange>(Protoid.A2C_AllPackageChange, OnPackageChange);

            xys.hot.UI.MainPanel.SetItemListener((int)xys.UI.PanelType.UITempPackagePanel,new xys.hot.UI.MainPanelItemListener(ShowTempPackageBtn));

            Event.Subscribe<AttributeChange>(EventID.LocalAttributeChange, ActionFunc);
            Event.Subscribe<LoginData>(EventID.BeginLogin, BeginLoginBtn);

            m_timer = new SimpleTimer(App.my.mainTimer);
        }

        void BeginLoginBtn(LoginData ld)
        {
            string[] str = Config.kvCommon.Get("BloodPoolRecover").value.Split('|');
            float tick = float.Parse(str[0]);
            m_timer.Register(tick, int.MaxValue, BloodPool);
        }

        static void ChangeGrid(Dictionary<int, int> changeItems, int id, int idCount)
        {
            int count = 0;
            if (changeItems.TryGetValue(id, out count))
                changeItems[id] = count + idCount;
            else
                changeItems.Add(id, idCount);
        }

        static void Alter(GridMgr mgr, PackageChange change, Dictionary<int, int> changeItems)
        {
            if (change == null)
                return;

            bool isCountChange = false;
            bool isAddChange = false;
            bool isRemoveChange = false;

            foreach (var itor in change.Counts)
            {
                // 数量变化的
                var grid = mgr.GetItemInfo(itor.Key);
                if (grid == null)
                {
                    Log.Error("grid == null");
                    continue;
                }
                if (grid.count == itor.Value)
                {
                    Log.Error("count==");
                    continue;
                }

                ChangeGrid(changeItems, grid.data.id, itor.Value - grid.count);
                grid.count = itor.Value;

                isCountChange = true;
            }

            // 道具变化了
            foreach (var itor in change.Datas)
            {
                var grid = mgr.GetItemInfo(itor.Key);
                if (grid != null)
                    ChangeGrid(changeItems, grid.data.id, -grid.count);

                mgr.SetItemInfo(itor.Key, itor.Value);                
                ChangeGrid(changeItems, itor.Value.data.id, itor.Value.count);

                isAddChange = true;
            }

            // 置空的
            foreach (var itor in change.Emptys)
            {
                var grid = mgr.GetItemInfo(itor);
                if (grid == null)
                {
                    Log.Error("grid == null");
                    continue;
                }

                mgr.SetItemInfo(itor, null);
                ChangeGrid(changeItems, grid.data.id, -grid.count);

                isRemoveChange = true;
            }

            if (isCountChange)
                App.my.eventSet.fireEvent(EventID.Package_CountChange);
            if (isAddChange)
                App.my.eventSet.fireEvent(EventID.Package_AddChange);
            if (isRemoveChange)
                App.my.eventSet.fireEvent(EventID.Package_ReomveChange);
        }

        static void Alter(GridMgr mgr, ItemGrids newV, Dictionary<int, int> changeItems)
        {
            for (int i = 0; i < mgr.Count; ++i)
            {
                var data = mgr.GetItemInfo(i);
                if (data != null)
                {
                    ChangeGrid(changeItems, data.data.id, -data.count);
                    mgr.SetItemInfo(i, null);
                }
            }

            foreach (var itor in newV.items)
            {
                mgr.SetItemInfo(itor.Key, itor.Value);
                ChangeGrid(changeItems, itor.Value.data.id, itor.Value.count);
            }
        }

        void OnPackageChange(AllPackageChange data)
        {
            Dictionary<int, int> alter = new Dictionary<int, int>(); // 变化的道具
            if (data.package != null) Alter(packageMgr.package, data.package, alter);
            if (data.task != null) Alter(packageMgr.taskPackage, data.task, alter);
            if (data.temp != null) Alter(packageMgr.tempPackage, data.temp, alter);
            if (data.tempAll != null) Alter(packageMgr.tempPackage, data.tempAll, alter);

#if COM_DEBUG
            foreach (var itor in alter)
            {
                if (itor.Value > 0)
                {
                    Debuger.DebugLog("得到道具:{0} Count:{1}", itor.Key, itor.Value);
                }
                else
                {
                    Debuger.DebugLog("失去道具:{0} Count:{1}", itor.Key, -itor.Value);
                }
            }
#endif


            App.my.eventSet.fireEvent(EventID.Package_UpdatePackage);
            ShowTempPackageBtn();
            OnShowTips(alter, (Config.GetItemTipsType)data.TipsType);
        }

        // 序列化
        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            PackageList pl = new PackageList();
            pl.MergeFrom(output);
            packageMgr.Reset(pl);
        }

        // 限时或隐藏临时背包按钮
        public bool ShowTempPackageBtn()
        {
            if (packageMgr.tempPackage.isEmpty)
            {
                xys.hot.UI.MainPanel.SetItemActive((int)xys.UI.PanelType.UITempPackagePanel, false);
                return false;
            }
            else
            {
                xys.hot.UI.MainPanel.SetItemActive((int)xys.UI.PanelType.UITempPackagePanel, true);
                return true;
            }
        }

        public bool UseItemByIndex(int index, int count)
        {
            return packageMgr.UseItemByIndex(index, count);
        }

        public bool UserItemById(int id, int count)
        {
            return packageMgr.UseItemById(id, count);
        }

        public int GetItemCount(int id)
        {
            return packageMgr.GetItemCount(id);
        }

        void ActionFunc(AttributeChange attr)
        {   
            long value = 0;
            int itemId = 0;
            List<xys.UI.Obtain> obainList = new List<xys.UI.Obtain>();
            switch (attr.id)
            {
                case AttType.AT_SilverShell:
                case AttType.AT_GoldShell:
                case AttType.AT_FairyJade:
                case AttType.AT_JasperJade:
                case AttType.AT_XiuWei:
                    value = attr.currentValue.longValue - attr.oldValue.longValue;
                    itemId = ItemHelp.MoneyTypeToItem(attr.id);
                    obainList.Add(new xys.UI.Obtain(itemId, (int)value));
                    break;
            }
            if (obainList.Count > 0)
                xys.UI.ObtainItemShowMgr.ShowObtain(obainList);
        }

        // 获得道具tips展示
        void OnShowTips(Dictionary<int, int> alter, Config.GetItemTipsType tipsType)
        {
            switch (tipsType)
            {
                case Config.GetItemTipsType.common_reward:
                case Config.GetItemTipsType.activity:
                case Config.GetItemTipsType.demonplost:
                case Config.GetItemTipsType.exchangeStore:
                case Config.GetItemTipsType.mail:
                case Config.GetItemTipsType.welfares:
                    List<xys.UI.Obtain> obainList = new List<xys.UI.Obtain>();
                    foreach (var v in alter)
                    {
                        if (v.Value > 0)
                            obainList.Add(new xys.UI.Obtain(v.Key, v.Value));
                    }
                    if (obainList.Count > 0)
                        xys.UI.ObtainItemShowMgr.ShowObtain(obainList);
                    break;
            }

        }

        // 血池
        void BloodPool()
        {
            if (App.my.localPlayer.attributes.Get(AttType.AT_HP) == null)
                return;

            if (App.my.localPlayer.hpValue == App.my.localPlayer.maxHpValue)
                return;

            if (!App.my.localPlayer.cdMgr.isEnd(CDType.BloodPool))
                return;

            if (App.my.localPlayer.battle == null || App.my.localPlayer.battle.m_attrMgr.battleState)
                return;

            if (App.my.localPlayer.bloodPoolValue == 0)
                return;

            request.BloodPoolRenewHp((error) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
            });
        }
    }
}
#endif