#if !USE_HOT
namespace xys.hot.UI
{
    using NetProto;
    using UIWidgets;
    using UnityEngine;

    class HotTempPackageView : HotTileViewBase<xys.UI.TempPackageView, HotItemGrid, NetProto.ItemGrid>
    {
        [SerializeField]
        GameObject selectItem; // 选中项

        Transform selectParent;
        xys.UI.Dialog.TwoBtn m_Screen;

        int selectIndex = -1; // 当前选中项
        int m_EventHanderId = -1;

        public HotTempPackageView() : base(null)
        {

        }

        public HotTempPackageView(xys.UI.TempPackageView parent) : base(parent)
        {
            //m_ComItemGrids = new Dictionary<int, List<HotItemGrid>>();
        }

        void Awake()
        {
            selectParent = selectItem.transform.parent;
            selectItem.SetActive(false);
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
        }

        protected override void OnSelectItem(int index)
        {
            var component = GetItem(index) as HotItemGrid;
            SetSelectItem(component);

            if (parent.SelectedItem == null)
                return;
            OpenTips((NetProto.ItemGrid)parent.SelectedItem, index);
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
            tipsData.m_BagType = Config.BagType.temp;

            App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIItemTipsPanel, tipsData);
        }
    }
}
#endif