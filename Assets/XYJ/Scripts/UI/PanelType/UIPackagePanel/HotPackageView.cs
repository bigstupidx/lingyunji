#if !USE_HOT
namespace xys.hot.UI
{
    using UIWidgets;
    using UnityEngine;
    using System.Collections.Generic;
    using wProtobuf;
    using Config;

    class HotPackageView : HotTileViewBase<xys.UI.PackageView, HotItemGrid, NetProto.ItemGrid>
    {
        public HotPackageView() : base(null)
        {

        }

        public HotPackageView(xys.UI.PackageView parent) : base(parent)
        {
            //m_ComItemGrids = new Dictionary<int, List<HotItemGrid>>();
        }

        void Awake()
        {
            selectParent = selectItem.transform.parent;
            selectItem.SetActive(false);
        }

        [SerializeField]
        GameObject selectItem;

        Transform selectParent;
        xys.UI.Dialog.TwoBtn m_Screen;
        Coroutine disableCoroutine;
        //Dictionary<int, List<HotItemGrid>> m_ComItemGrids;

        int selectIndex = -1; // 当前选中项
        int openIndex = 0;  // 开放的格子数量
        int m_EventHanderId = -1;

        public int openGridNum { set { openIndex = value; } }

        NetProto.C2APackageRequest request_;
        NetProto.C2APackageRequest request
        {
            get
            {
                if (request_ == null)
                    request_ = hotApp.my.GetModule<HotPackageModule>().request;

                return request_;
            }
        }

        protected override void SetData(HotItemGrid component, NetProto.ItemGrid item)
        {
            if (component == null)
                return;

            if (selectIndex == component.Index)
            {
                SetSelectItem(component);
            }
            else if (selectItem.activeSelf && selectItem.transform.parent == component.transform)
            {
                UnSelectItem();
            }

            component.SetData(item);
            component.SetOpenGridState(openIndex);

            //if (item == null)
            //    return;
            //Item config = Item.Get(item.data.id);
            //if (config == null)
            //    return;
            //if (config.cooling == 0)
            //    return;

            //int sonType = ItemBaseAll.Get(item.data.id).sonType;

            //if (App.my.localPlayer.cdMgr.isEnd(CDType.Item, (short)sonType))
            //{
            //    if (m_ComItemGrids.ContainsKey(sonType))
            //        m_ComItemGrids.Remove(sonType);
            //    return;
            //}

            //if (!m_ComItemGrids.ContainsKey(sonType))
            //{
            //    List<xys.UI.ItemGrid> list = new List<xys.UI.ItemGrid>();
            //    m_ComItemGrids[sonType] = list;
            //}
            //m_ComItemGrids[sonType].Add(component);

            //if (disableCoroutine == null)
            //    disableCoroutine = parent.StartCoroutine(TimeLimitItem());
        }

        protected override void OnSelectItem(int index)
        {
            xys.App.my.eventSet.fireEvent(EventID.Pacakge_Selected);

            if (index >= openIndex)
            {
                OpenNewGrid(index);
                return;
            }

            var component = GetItem(index) as HotItemGrid;
            SetSelectItem(component);

            if (parent.SelectedItem == null)
                return;

            OpenTips((NetProto.ItemGrid)parent.SelectedItem, index);
        }

        void Clear()
        {
            //m_ComItemGrids.Clear();
            if (disableCoroutine != null)
            {
                parent.StopCoroutine(disableCoroutine);
                disableCoroutine = null;
            }
        }

        void SetSelectItem(HotItemGrid component)
        {
            selectIndex = component.Index;
            var rectTrans = selectItem.transform as RectTransform;
            rectTrans.SetParent(component.transform);

            rectTrans.localScale = Vector3.one;
            rectTrans.localPosition = Vector3.zero;
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;

            rectTrans.offsetMin = Vector2.zero;
            rectTrans.offsetMax = Vector2.zero;

            selectItem.SetActive(true);
        }

        void UnSelectItem()
        {
            selectItem.transform.SetParent(selectParent);
            selectItem.SetActive(false);
        }

        protected override void OnDeselectItem(int index)
        {
            xys.UI.EventHandler.pointerClickHandler.Remove(m_EventHanderId);
            UnSelectItem();
            selectIndex = -1;
        }

        void OpenTips(NetProto.ItemGrid item, int index)
        {
            xys.UI.InitItemTipsData tipsData = new xys.UI.InitItemTipsData();
            tipsData.type = xys.UI.InitItemTipsData.Type.Package;
            tipsData.itemData = item;
            tipsData.index = index;
            tipsData.m_BagType = Config.BagType.item;

            App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
        }

        void OpenNewGrid(int index)
        {
            if (m_Screen != null)
                return;
            Config.kvCommon openBagGridCost = Config.kvCommon.Get("openBagGridCost");
            if (openBagGridCost == null)
                return;
            Config.kvCommon openBagGridCostId = Config.kvCommon.Get("openBagGridCostId");
            if (openBagGridCostId == null)
                return;
            Config.ItemBase itemConfig = Config.ItemBaseAll.Get(int.Parse(openBagGridCostId.value));
            if (itemConfig == null)
                return;
            Config.TipsContent tipsConfig = Config.TipsContent.Get(3106);
            if (tipsConfig == null)
                return;


            m_Screen = xys.UI.Dialog.TwoBtn.Show(
                "",
                string.Format(tipsConfig.des, openBagGridCost.value, itemConfig.name),
                "取消", () => false,
                "确定", () =>
                {
                    NetProto.Int32 input = new NetProto.Int32();
                    input.value = index;
                    request.OpenNewGrid(input, (error, respone) =>
                    {
                        if (error != wProtobuf.RPC.Error.Success)
                            return;

                        if (respone.code == 0)
                        {
                            hotApp.my.GetModule<HotPackageModule>().packageMgr.package.ResetSize(respone.newGrid);
                            xys.App.my.eventSet.fireEvent(EventID.Package_UpdatePackage);
                        }
                        else
                        {
                            Config.ErrorCode code = Config.ErrorCode.Get((int)respone.code);
                            if (code == null)
                                return;
                            xys.UI.SystemHintMgr.ShowHint(code.desc);
                        }
                    });

                    return false;
                }, true, true, () => m_Screen = null);
        }

        //System.Collections.IEnumerator TimeLimitItem()
        //{
        //    while (true)
        //    {
        //        yield return 10;

        //        foreach (var info in m_ComItemGrids)
        //        {
        //            foreach (xys.UI.ItemGrid v in info.Value)
        //                v.SetCdImage(info.Key);
        //        }
        //    }
        //}

        void PageChange()
        {
            if (disableCoroutine != null)
            {
                parent.StopCoroutine(disableCoroutine);
                disableCoroutine = null;
            }
        }
    }
}
#endif