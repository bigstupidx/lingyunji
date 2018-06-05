#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Config;
    using NetProto;
    using NetProto.Hot;

    enum TabType
    {
        item = 0,
        task = 1,
    }

    class PackagePage : HotTablePageBase
    {
        PackagePage() : base(null) { }
        PackagePage(xys.UI.HotTablePage p) : base(p)
        {

        }

        xys.UI.EquipGrid[] equips; // 装备列表
        xys.UI.State.StateRoot JobImage;             // 职业背景图
        RTTModelPartHandler m_rtt;                                      //角色显示
        RectTransform m_RoleImage;

        [SerializeField]
        xys.UI.PackageView packageView; // 任务包裹

        [SerializeField]
        xys.UI.State.StateToggle toggle; // 物品和任务包裹

        [SerializeField]
        xys.UI.State.StateToggle equipToggle; // 物品和任务包裹

        [SerializeField]
        Button arrangeBtn; // 整理按钮

        HotPackageModule module { get; set; }

        protected override void OnInit()
        {
            toggle.OnSelectChange = OnChange;
            toggle.OnPreChange = PreChange;

            equipToggle.OnSelectChange = OnEquipSelected;

            module = (HotPackageModule)App.my.localPlayer.GetModule<PackageModule>().refType.Instance;

            RegistButton();

            equips = parent.GetComponentsInChildren<xys.UI.EquipGrid>();
            JobImage = parent.transform.Find("EquipArea/Job").GetComponent<xys.UI.State.StateRoot>();
            m_RoleImage = parent.transform.Find("EquipArea/Role").GetComponent<RectTransform>();
        }

        void RegistEvent()
        {
            Event.Subscribe(EventID.Package_UpdatePackage, UpdataPackage);
            Event.Subscribe<int>(EventID.Equip_LoadFinish, LoadEquip);
            Event.Subscribe<int>(EventID.Equip_UnLoadFinish, UnLoadEquip);
            Event.Subscribe(EventID.Package_TipsClose, TipsClose);
            Event.Subscribe(EventID.Pacakge_Selected, SelectedEvent);
        }

        void RegistButton()
        {
            arrangeBtn.onClick.AddListener(OnArrangClick);
        }

        // 道具选中事件
        void SelectedEvent()
        {
            equipToggle.Select = -1;
        }

        // 穿装备
        void LoadEquip(int index)
        {
            var equipModule = App.my.localPlayer.GetModule<EquipModule>();
            if (equipModule == null)
                return;

            EquipMgr equipMgr = equipModule.equipMgr as EquipMgr;

            packageView.Deselect(packageView.SelectedIndex);
            equipToggle.Select = -1;

            if (index < 1 || index > 9)
                return;

            Dictionary<int, ItemData> equipList = equipMgr.GetAllEquips();

            ItemGrid grid = new ItemGrid();
            grid.data = equipList[index];
            equips[index - 1].SetData(grid);
            equips[index - 1].SetOpenGridState(grid.data);

            // 装备特效
            RALoad.LoadPrefab("fx_ui_bag_zhuangbei_effect", (go, para) =>
            {
                go.transform.SetParent(equips[index - 1].transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
            }, null);
        }

        // 脱装备
        void UnLoadEquip(int index)
        {
            packageView.Deselect(packageView.SelectedIndex);
            equipToggle.Select = -1;

            if (index < 1 || index > 9)
                return;

            equips[index - 1].SetData(null);
            equips[index - 1].SetOpenGridState(null);
        }

        // 加载装备信息
        void SetEquip()
        {
            var equipModule = App.my.localPlayer.GetModule<EquipModule>();
            if (equipModule == null)    
               return;

            EquipMgr equipMgr = equipModule.equipMgr as EquipMgr;
            ItemGrid grid = null;

            foreach (var info in equipMgr.GetAllEquips())
            {
                if (info.Key < 1 || info.Key > 9)
                    continue;

                grid = new ItemGrid();
                grid.data = info.Value;
                equips[info.Key - 1].SetData(grid);
                equips[info.Key - 1].SetOpenGridState(grid.data);
            }
        }

        // 换装特效
        void PlayEearEquipEffect()
        {

        }

        // 设置职业
        void SetPlayerJob()
        {
            m_rtt = new RTTModelPartHandler("RTTModelPart", m_RoleImage, "", true, new Vector3(1000, 0, 0));
            JobImage.SetCurrentState((int)App.my.localPlayer.job - 1 , true);
            m_rtt.SetModel(App.my.localPlayer.cfgInfo.id);
        }

        void OnArrangClick()
        {
            // 5秒冷却cd
            if (!App.my.localPlayer.cdMgr.isEnd(xys.CDType.PackageArrange))
            {
                TipsContent tipsConfig = TipsContent.Get(3109);
                if (tipsConfig == null)
                    return;
                xys.UI.SystemHintMgr.ShowHint(tipsConfig.des);
                return;
            }

            module.request.Arrange((error, respone) =>
            {
                if (error == wProtobuf.RPC.Error.Success)
                {
                    module.packageMgr.ArrangePackage();
                    UpdataPackage();
                }
            });
        }

        bool PreChange(xys.UI.State.StateRoot sr, int index)
        {
            return true;
        }

        void OnChange(xys.UI.State.StateRoot sr, int index)
        {
            if (index == (int)TabType.item)
            {
                //packageView.PageChange();
                ShowPackage();
            }
            else
            {
                //packageView.PageChange();
                ShowTaskPackage();
            }
        }

        void OnEquipSelected(xys.UI.State.StateRoot sr, int index)
        {
            var equipModule = App.my.localPlayer.GetModule<EquipModule>();
            if (equipModule == null)
                return;

            EquipMgr equipMgr = equipModule.equipMgr as EquipMgr;

            ItemGrid gridItem = new ItemGrid();
            var equipList = equipMgr.GetAllEquips();

            index = index + 1;

            packageView.Deselect(packageView.SelectedIndex);

            if (!equipList.ContainsKey(index))
                return;

            gridItem.data = equipList[index];

            xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
            tipsData.type = xys.UI.InitItemTipsData.Type.Package;
            tipsData.itemData = gridItem;
            tipsData.index = index;

            App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);

            return;
        }

        void UpdataPackage()
        {
            //packageView.PageChange();

            if (toggle.Select == (int)TabType.item)
            {
                ShowPackage();
            }
            else
            {
                ShowTaskPackage();
            }
        }

        // 显示时的回调
        protected override void OnShow(object args)
        {
            RegistEvent();
            ShowTab();
            ShowPackage();
            SetEquip();
            SetPlayerJob();
        }

        void ShowTab()
        {
            toggle.Select = (int)TabType.item;
        }

        void ShowPackage()
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            List<NetProto.ItemGrid > items = new List<NetProto.ItemGrid>();
            module.packageMgr.package.ForEach((Grid g) =>
            {
                items.Add(g.data);
            });

            // 重置选择效果
            packageView.Deselect(packageView.SelectedIndex);
            packageView.openGridNum = items.Count;
            equipToggle.Select = -1;

            var bagConfig = Bag.Get(BagType.item);
            int openIndex = items.Count + 5;
            if (openIndex > bagConfig.sum)
                openIndex = bagConfig.sum;

            while (items.Count < openIndex)
                items.Add(null);

            packageView.SetDataList(items);
        }

        void ShowTaskPackage()
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            List<NetProto.ItemGrid > items = new List<NetProto.ItemGrid>();
            module.packageMgr.taskPackage.ForEach((Grid g) =>
            {
                items.Add(g.data);
            });

            // 重置选择效果
            packageView.Deselect(packageView.SelectedIndex);
            packageView.SetDataList(items);
        }

        protected override void OnHide()
        {
            equipToggle.Select = -1;
            packageView.Deselect(packageView.SelectedIndex);
            packageView.Clear();
            if (m_rtt != null)
                m_rtt.Destroy();
        }

        void TipsClose()
        {
            equipToggle.Select = -1;
        }
    }
}
#endif