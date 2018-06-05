#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using Config;
using NetProto;
using System;
using xys.UI.State;
using System.Collections;

namespace xys.hot.UI
{
    class UIQuickSellPanel : HotPanelBase
    {

        [SerializeField]
        Image QualityBgImage;
        [SerializeField]
        Image ItemIconImage;
        [SerializeField]
        Text ItemTypeText;
        [SerializeField]
        Text ItemNameText;
        [SerializeField]
        Text ItemFunText;
        [SerializeField]
        Text ItemDesText;
        [SerializeField]
        Image CoefficientTagImage;
        [SerializeField]
        Text CoefficientTagText;
        [SerializeField]
        Button AddBtn;
        [SerializeField]
        Button MinBtn;
        [SerializeField]
        Button InputNumBtn;
        [SerializeField]
        Text InputNumText;
        [SerializeField]
        Image GetCurrencyImage;
        [SerializeField]
        Text GetCurrencyText;
        [SerializeField]
        Image HasCurrencyImage;
        [SerializeField]
        Text HasCurrencyText;
        [SerializeField]
        Button SellBtn;

        private int curPackageIndex;
        private NetProto.ItemGrid curItemGrid;
        private int curNum;
        private float curPrice;
        private string[] UpOrDownImageStr;
        private int buyedNum;
        private StateRoot minBtnSR;
        private StateRoot addBtnSR;
        private TradeItemAtt curtradeItemAtt;
        private Item itemData;
        private ShangHuiItem tradeItem;
        private RectTransform itemdescTrans;
        private const int ExpressionConstant = 10000;
        private Vector3 curUICalculatorPanelPos = new Vector3(20, 110);

        UIHotPanel m_Parent;
        PackageMgr m_PackageMgr;
        TradeStoreMgr m_TradeStoreMgr;


        public UIQuickSellPanel() : base(null) { }

        public UIQuickSellPanel(UIHotPanel parent) : base(parent)
        {
            m_Parent = parent;
        }

        protected override void OnInit()
        {
            m_PackageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            m_TradeStoreMgr = App.my.localPlayer.GetModule<TradeStoreModule>().tradeStoreMgr as TradeStoreMgr;
            UpOrDownImageStr = new string[] { "ui_Common_ascension_Icon", "ui_Common_Falling_Icon" };
            minBtnSR = MinBtn.transform.GetComponent<StateRoot>();
            addBtnSR = AddBtn.transform.GetComponent<StateRoot>();
            itemdescTrans = ItemDesText.transform.GetComponent<RectTransform>();
            InitBtn();
        }

        protected override void OnShow(object p)
        {
            curPackageIndex = (int)p;
            curItemGrid = m_PackageMgr.package.GetItemInfo(curPackageIndex);
            tradeItem = ShangHuiItem.Get(curItemGrid.data.id);
            buyedNum = curItemGrid.count;
            Event.Subscribe(EventID.TradeStore_QuickSellRefreshUI, InitView);
            Event.fireEvent(EventID.TradeStore_QuickSellSearch);
        }


        protected override void OnHide()
        {

        }

        void InitBtn()
        {
            AddBtn.onClick.AddListener(() =>
            {
                curNum++;
                CheckView();
            });
            MinBtn.onClick.AddListener(() =>
            {
                curNum--;
                CheckView();
            });
            InputNumBtn.onClick.AddListener(() =>
            {
                xys.App.my.uiSystem.ShowPanel(PanelType.UICalculatorPanel, SetParam());
            });

            SellBtn.onClick.AddListener(() =>
            {
                TradeStoreSellRequest request = new TradeStoreSellRequest();
                request.itemid = curItemGrid.data.id;
                request.itemnum = curNum;
                request.curprice = curPrice;
                request.itemindex = curPackageIndex;
                Event.FireEvent(EventID.TradeStore_Sell, request);
                App.my.uiSystem.HidePanel(PanelType.UIQuickSellPanel);
            });
        }

        void InitView()
        {
            itemData = Item.Get(curItemGrid.data.id);
            curtradeItemAtt = m_TradeStoreMgr.GetItemDataDic(curItemGrid.data.id);
            var priceData = Item.Get((int)curtradeItemAtt.priceType);
            curPrice = curtradeItemAtt.curprice;
            var qualitysource = QualitySourceConfig.Get(itemData.quality);
            Helper.SetSprite(QualityBgImage, qualitysource.tips);
            Helper.SetSprite(ItemIconImage, itemData.icon);
            ItemNameText.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", qualitysource.colorname, itemData.name));
            ItemFunText.text = itemData.use;
            ItemDesText.text = GlobalSymbol.ToUT(itemData.desc);
            Helper.SetSprite(GetCurrencyImage, priceData.icon);
            Helper.SetSprite(HasCurrencyImage, priceData.icon);
            var tempPrice = curtradeItemAtt.curprice - curtradeItemAtt.defaultprice;
            if (tempPrice > 0)
            {
                Helper.SetSprite(CoefficientTagImage, UpOrDownImageStr[0]);
                CoefficientTagImage.transform.parent.gameObject.SetActive(true);
            }
            else if (tempPrice < 0)
            {
                Helper.SetSprite(CoefficientTagImage, UpOrDownImageStr[1]);
                CoefficientTagImage.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                CoefficientTagImage.transform.parent.gameObject.SetActive(false);
            }
            CoefficientTagText.text = Math.Round(Mathf.Abs(tempPrice) / curtradeItemAtt.defaultprice, 4) * 100 + "%";
            curNum = 1;
            CheckView();
            App.my.mainCoroutine.StartCoroutine(ToTheTop());
        }

        void CheckView()
        {
            if (curNum >= 1 && curNum <= buyedNum)
            {
                AddBtn.enabled = true;
                addBtnSR.SetCurrentState(1, true);
                MinBtn.enabled = true;
                minBtnSR.SetCurrentState(1, true);
            }
            if (curNum <= 1)
            {
                curNum = 1;
                MinBtn.enabled = false;
                minBtnSR.SetCurrentState(0, true);
            }
            if (curNum >= buyedNum)
            {
                curNum = buyedNum;
                AddBtn.enabled = false;
                addBtnSR.SetCurrentState(0, true);
            }
            InputNumText.text = curNum.ToString();
            var sumPrice = GetSumPrice();
            GetCurrencyText.text = GlobalSymbol.ToBef(sumPrice);
            HasCurrencyText.text = GlobalSymbol.ToBef(m_PackageMgr.GetItemCount((int)curtradeItemAtt.priceType));
        }

        void ChangeView(int num)
        {
            curNum = num;
            CheckView();
        }

        float GetSumPrice()
        {
            var tempPrice = curPrice;
            float sumprice = 0;
            for (int i = 0; i < curNum; i++)
            {
                var discount = DisCountSellPrice();
                sumprice += (float)Math.Ceiling(Math.Round(tempPrice * discount, 2));
                tempPrice = (float)Math.Ceiling(Math.Round(tempPrice - tempPrice * curtradeItemAtt.normalcoefficient / ExpressionConstant, 5));
                var minPrice = (float)Math.Ceiling(Math.Round(curtradeItemAtt.defaultprice * tradeItem.minlimit, 2));
                if (tempPrice < minPrice)
                {
                    tempPrice = minPrice;
                }
            }
            return (float)Math.Ceiling(sumprice);
        }

        float DisCountSellPrice()
        {
            if (itemData.storeId == 0)
            {
                return 1;
            }
            else
            {
                return 0.8f;
            }
        }

        struct Param
        {
            public int itemid;
        }

        IEnumerator ToTheTop()
        {
            yield return new WaitForEndOfFrame();
            itemdescTrans.localPosition = itemdescTrans.rect.height / 2 * Vector3.down;
        }


        UICalculatorPanel.Param SetParam()
        {
            UICalculatorPanel.Param param = new UICalculatorPanel.Param();
            param.defaultValue = 1;
            param.maxValue = curItemGrid.count;
            param.pos = curUICalculatorPanelPos;
            param.valueChange += ChangeView;
            return param;
        }
    }

}
#endif