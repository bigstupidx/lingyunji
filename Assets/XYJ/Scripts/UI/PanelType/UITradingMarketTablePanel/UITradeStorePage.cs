#if !USE_HOT
using Config;
using NetProto;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using System.Collections;

namespace xys.hot.UI
{
    class UITradeStorePage : HotTablePageBase
    {
        enum TradeStorePageType
        {
            None = -1,
            BuyPage = 0,
            SellPage = 1,
        }

        [SerializeField]
        Button BuyBtn;
        [SerializeField]
        Button SellBtn;
        [SerializeField]
        Text ItemNameText;
        [SerializeField]
        Text ItemDesText;
        [SerializeField]
        Button AddBtn;
        [SerializeField]
        Button MinBtn;
        [SerializeField]
        Button InputBtn;
        [SerializeField]
        Text SellInfoText;
        [SerializeField]
        Text CostCurrencyLabelText;
        [SerializeField]
        Image CostCurrencyIconImage;
        [SerializeField]
        Text CostCurrencyNumText;
        [SerializeField]
        Text HasCurrencyLabelText;
        [SerializeField]
        Image HasCurrencyIconImage;
        [SerializeField]
        Text HasCurrencyNumText;
        [SerializeField]
        Button ConfimBtn;
        [SerializeField]
        GameObject ItemInfoObj;
        #region 购买
        [SerializeField]
        GameObject BuyPanelObj;
        [SerializeField]
        GameObject LabelBtnObj;
        [SerializeField]
        GameObject LabelBtnChildObj;
        [SerializeField]
        GameObject BuyItemObj;
        [SerializeField]
        Button BuyIntroBtn;
        [SerializeField]
        Button CurrencyAddBtn;
        [SerializeField]
        Text BuyResidueInfoText;
        #endregion
        #region 出售
        [SerializeField]
        GameObject SellPanelObj;
        [SerializeField]
        GameObject SellItemObj;
        [SerializeField]
        GameObject NoSellItemObj;
        #endregion
        private Dictionary<int, ShangHuiConfig> storeDataDict;
        private List<ShangHuiItem> storeItemDataList;
        private Dictionary<int, Item> itemDataDict;
        private Dictionary<int, StateRoot> childlabelBtnSRDic;
        private Text itemnumText;
        private Text confimBtnText;
        private Image tagIamge;
        private Image confimBtnIamge;
        private StateRoot addBtnSR;
        private StateRoot minBtnSR;
        private StateRoot buyBtnSR;
        private StateRoot sellBtnSR;
        private TradeStorePageType curPageType;
        private int buyednum;
        private int curnum;
        private Vector3 calculatorPanelPos;
        private string TagImageUpStr;
        private string TagImageDownStr;
        private string buyBtnName;
        private string sellBtnName;
        private string costCurrencyName;
        private string getCurrencyName;
        private float curSumPrice;
        private float curPrice;
        private float curUpPrice;
        private int curPriceId;
        private int curItemId;
        private int tempItemId;
        private int tempNum;
        private const int ExpressionConstant = 10000;

        #region 购买
        private Transform labelBtnParentTrans;
        private Transform buyItemParentTrans;
        private List<StateRoot> labelBtnsSRList;
        private List<StateRoot> labelBtnParentSRList;
        private List<StateRoot> buyItemSRList;
        private List<ShangHuiItem> curStoreItemDataList;
        private int preBuyBtnIndex = -1; //标签页按钮序号
        private int preBuyShopId = -1; //用于改变子标签页按钮的StateRoot
        private int preBuyItemIndex = -1; //当前物品列表序号

        #endregion

        #region 出售
        private Transform sellItemParentTrans;
        private List<StateRoot> sellItemSRList;
        private List<NetProto.ItemGrid> curSellItemList;
        private List<int> curPackageIndexList;
        private int preSellItemIndex = -1;
        #endregion

        HotTablePage m_Parent;
        TradeStoreModule m_Module;
        LocalPlayer m_LocalPlayer;
        TradeStoreMgr m_StoreMgr;
        PackageMgr m_PackageMgr;

        public UITradeStorePage() : base(null)
        {

        }

        public UITradeStorePage(HotTablePage parent) : base(parent)
        {
            m_Parent = parent;
        }


        protected override void OnInit()
        {
            m_LocalPlayer = xys.App.my.localPlayer;
            m_Module = m_LocalPlayer.GetModule<TradeStoreModule>();
            m_PackageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;

            itemnumText = InputBtn.transform.GetComponent<Text>();
            confimBtnText = ConfimBtn.transform.Find("Text").GetComponent<Text>();
            tagIamge = SellInfoText.transform.parent.GetComponent<Image>();
            confimBtnIamge = ConfimBtn.GetComponent<Image>();
            buyBtnSR = BuyBtn.transform.GetComponent<StateRoot>();
            sellBtnSR = SellBtn.transform.GetComponent<StateRoot>();
            addBtnSR = AddBtn.transform.GetComponent<StateRoot>();
            minBtnSR = MinBtn.transform.GetComponent<StateRoot>();
            storeDataDict = ShangHuiConfig.GetAll();
            storeItemDataList = ShangHuiItem.GetAll();
            itemDataDict = Item.GetAll();

            #region 购买
            labelBtnParentTrans = LabelBtnObj.transform.parent;
            buyItemParentTrans = BuyItemObj.transform.parent;
            labelBtnsSRList = new List<StateRoot>();
            labelBtnParentSRList = new List<StateRoot>();
            buyItemSRList = new List<StateRoot>();
            curStoreItemDataList = new List<ShangHuiItem>();
            BuyResidueInfoText.transform.parent.gameObject.SetActive(false);
            InitBuyItemLeft();
            #endregion

            #region 出售
            sellItemParentTrans = SellItemObj.transform.parent;
            sellItemSRList = new List<StateRoot>();
            #endregion

            calculatorPanelPos = new Vector3(372, 73);
            curPageType = TradeStorePageType.None;
            TagImageUpStr = "ui_Common_ascension_Icon";
            TagImageDownStr = "ui_Common_Falling_Icon";
            buyBtnName = "购买";
            sellBtnName = "出售";
            costCurrencyName = "消耗";
            getCurrencyName = "收益";
            BuyResidueInfoText.text = "剩余可购：当前全服剩余总数和单人当日剩余可购数量中的较小值";

            InitBtn();
        }
        void InitBtn()
        {
            BuyBtn.onClick.AddListener(() =>
            {
                sellBtnSR.SetCurrentState(0, true);
                buyBtnSR.SetCurrentState(1, true);
                m_StoreMgr = m_Module.tradeStoreMgr as TradeStoreMgr;
                if (m_StoreMgr == null)
                    return;
                if (curPageType == TradeStorePageType.BuyPage) return;
                SellPanelObj.SetActive(false);
                BuyPanelObj.SetActive(true);
                confimBtnText.text = buyBtnName;
                CostCurrencyLabelText.text = costCurrencyName;
                CurrencyAddBtn.gameObject.SetActive(true);
                ItemInfoObj.gameObject.SetActive(true);
                if (curPageType == TradeStorePageType.None)
                {
                    curPageType = TradeStorePageType.BuyPage;
                    labelBtnParentTrans.GetChild(0).Find("Button").GetComponent<Button>().onClick.Invoke();
                    return;
                }
                curPageType = TradeStorePageType.BuyPage;
                curItemId = tempItemId;
                RefreshBuyItemUI();
                ItemChoosed(curItemId);
                //ItemChoosed(curItemId, curnum);
            });
            SellBtn.onClick.AddListener(() =>
            {
                buyBtnSR.SetCurrentState(0, true);
                sellBtnSR.SetCurrentState(1, true);
                m_StoreMgr = m_Module.tradeStoreMgr as TradeStoreMgr;
                if (m_StoreMgr == null)
                    return;
                if (curPageType == TradeStorePageType.SellPage) return;
                SellPanelObj.SetActive(true);
                BuyPanelObj.SetActive(false);
                confimBtnText.text = sellBtnName;
                CostCurrencyLabelText.text = getCurrencyName;
                CurrencyAddBtn.gameObject.SetActive(false);
                curPageType = TradeStorePageType.SellPage;
                tempItemId = curItemId;
                LoadSellItem();
            });
            InputBtn.onClick.AddListener(() =>
            {
                var param = SetParam();
                App.my.uiSystem.ShowPanel(PanelType.UICalculatorPanel, param);
            });
            AddBtn.onClick.AddListener(() =>
            {
                curnum++;
                CheckNum();
            });
            MinBtn.onClick.AddListener(() =>
            {
                curnum--;
                CheckNum();
            });
            ConfimBtn.onClick.AddListener(() =>
            {
                if (curPageType == TradeStorePageType.BuyPage)
                {
                    OnBuyBtnClick();
                }
                else if (curPageType == TradeStorePageType.SellPage)
                {
                    OnSellBtnClick();
                }
            });
            BuyIntroBtn.onClick.AddListener(() =>
            {
                BuyResidueInfoText.transform.parent.gameObject.SetActive(true);
            });
            CurrencyAddBtn.onClick.AddListener(() =>
            {
                App.my.uiSystem.systemHintMgr.ShowHintGo("目前没有货币购买");
            });
        }

        protected override void OnShow(object arg)
        {
            Event.Subscribe(EventID.TradeStore_MainRefreshUI, this.RefreshMainUI);
            Event.Subscribe(EventID.TradeStore_RefreshInfoUI, this.RefreshInfoUI);
            Event.Subscribe<int>(EventID.TradeStore_InputNum, this.ChangeNumRefreshRightUI);
            Event.fireEvent(EventID.TradeStore_Search);//获取当前服务器上数据
            if (curPageType == TradeStorePageType.SellPage)
            {
                BuyBtn.onClick.Invoke();
            }
        }

        protected override void OnHide()
        {

        }

        #region 右边界面显示
        void RefreshInfoUI()
        {
            m_StoreMgr = m_Module.tradeStoreMgr as TradeStoreMgr;
            if (m_StoreMgr == null)
                return;
            int itemid = curItemId;
            buyednum = LimitNum(itemid);
            if (curPageType == TradeStorePageType.BuyPage)
            {
                SellInfoText.gameObject.SetActive(false);
                tagIamge.gameObject.SetActive(false);
            }
            else if (curPageType == TradeStorePageType.SellPage)
            {
                SellInfoText.gameObject.SetActive(true);
                tagIamge.gameObject.SetActive(true);
                var value = CountChange(itemid);
                if (value == 0)
                {
                    tagIamge.transform.gameObject.SetActive(false);
                }
                else
                {
                    tagIamge.transform.gameObject.SetActive(true);
                    Helper.SetSprite(tagIamge, GetImageNameStr(value));
                    SellInfoText.text = Mathf.Abs((float)value) * 100 + "%";
                }
            }
            this.curnum = 1;
            CheckNum();
        }

        void ViewNumUI()
        {
            itemnumText.text = curnum.ToString();
            var costcurrencystr = GlobalSymbol.ToBef((int)curSumPrice);
            if (curPageType == TradeStorePageType.BuyPage && curSumPrice > m_PackageMgr.GetItemCount(curPriceId))
            {
                CostCurrencyNumText.text = string.Format("<color=#ef3c49>{0}</color>", costcurrencystr);
            }
            else
            {
                CostCurrencyNumText.text = string.Format("<color=#e9f1fb>{0}</color>", costcurrencystr);
            }
            HasCurrencyNumText.text = GlobalSymbol.ToBef(m_PackageMgr.GetItemCount(curPriceId));
        }

        void CheckNum(int setcurnum = -1)
        {
            if (setcurnum != -1)
            {
                curnum = setcurnum;
            }
            bool isUnLimit = false;

            if (buyednum == -1)
            {
                isUnLimit = true;
            }

            if (curnum >= 1 && curnum <= buyednum && !isUnLimit)
            {
                MinBtn.enabled = true;
                minBtnSR.SetCurrentState(1, true);
                AddBtn.enabled = true;
                addBtnSR.SetCurrentState(1, true);
                InputBtn.enabled = true;
            }
            if (curnum <= 1 && !isUnLimit)
            {
                curnum = 1;
                MinBtn.enabled = false;
                minBtnSR.SetCurrentState(0, true);
            }
            if (curnum >= buyednum && !isUnLimit)
            {
                if (buyednum == 0)
                {
                    curnum = 1;
                    InputBtn.enabled = false;
                }
                else
                {
                    curnum = buyednum;
                }
                AddBtn.enabled = false;
                addBtnSR.SetCurrentState(0, true); ;
            }
            if (buyednum == 0)
            {
                ConfimBtn.enabled = false;
                confimBtnIamge.color = Color.grey;
            }
            else
            {
                ConfimBtn.enabled = true;
                confimBtnIamge.color = Color.white;
            }
            curSumPrice = 0;
            curUpPrice = curPrice;
            GetCurPrice();
            ViewNumUI();
        }

        UICalculatorPanel.Param SetParam()
        {
            var param = new UICalculatorPanel.Param();
            param.defaultValue = curnum;
            param.minValue = 0;
            param.pos = calculatorPanelPos;
            param.maxValue = CanBuyNumCount();
            param.valueChange += CheckNum;
            return param;
        }

        int CanBuyNumCount()
        {
            int canBuyNum = 0;
            if (curPageType == TradeStorePageType.BuyPage)
            {
                var curtradeItemData = GetCurTradeItemData(curItemId);
                var costNum = BuyItemNumCount(curtradeItemData);
                var limitNum = LimitNum(curItemId);
                if (limitNum != -1)
                {
                    canBuyNum = costNum > limitNum ? limitNum : costNum;
                }
                else
                {
                    canBuyNum = costNum;
                }
            }
            else if (curPageType == TradeStorePageType.SellPage)
            {
                canBuyNum = buyednum;
            }
            return canBuyNum;
        }

        private void GetCurPrice()
        {
            if (curPageType == TradeStorePageType.BuyPage)
            {
                var curTradeStoreItem = GetCurTradeItemData(curItemId);
                var curItemAtt = m_StoreMgr.tradeItemData.data[curItemId];
                for (int i = 0; i < curnum; i++)
                {
                    curSumPrice += curUpPrice;
                    curUpPrice = (float)Math.Ceiling(Math.Round(curUpPrice * curTradeStoreItem.upcoefficient * curTradeStoreItem.normalcoefficient / ExpressionConstant,5)) + (float)Math.Ceiling(curUpPrice);
                    var maxPrice = (float)Math.Ceiling(Math.Round(curItemAtt.defaultprice * curTradeStoreItem.uplimit,2));
                    if (curUpPrice > maxPrice)
                    {
                        curUpPrice = maxPrice;
                    }
                }
            }
            else if (curPageType == TradeStorePageType.SellPage)
            {
                var curItemAtt = m_StoreMgr.GetItemDataDic(curItemId);
                if (curItemAtt != null)
                {
                    var curTradeStoreItem = ShangHuiItem.Get(curItemId);
                    var discount = DisCountSellPrice();
                    for (int i = 0; i < curnum; i++)
                    {
                        curSumPrice += (float)Math.Ceiling(Math.Round(curUpPrice * discount,2));
                        curUpPrice = (float)Math.Ceiling(curUpPrice) - (float)Math.Ceiling(Math.Round(curTradeStoreItem.normalcoefficient * curUpPrice / ExpressionConstant,5));
                        var minPrice = (float)Math.Ceiling(Math.Round(curItemAtt.defaultprice * curTradeStoreItem.minlimit, 2));
                        if (curUpPrice < minPrice)
                        {
                            curUpPrice = minPrice;
                        }
                    }
                }
                else
                {
                    curSumPrice = curUpPrice * curnum;
                }
            }
            curSumPrice = (float)Math.Ceiling(curSumPrice);
        }

        float DisCountSellPrice()
        {
            if (itemDataDict[curItemId].storeId == 0)
            {
                return 1;
            }
            else
            {
                return 0.8f;
            }
        }


        //计算无限制物品可购买的最大数量
        int BuyItemNumCount(ShangHuiItem curTradeStoreItem)
        {
            float playerHasCount = m_PackageMgr.GetItemCount(curPriceId);
            float curprice = curPrice;
            int canBuyNum = 0;
            while (playerHasCount > 0)
            {
                playerHasCount -= curprice;
                if (playerHasCount < 0)
                {
                    break;
                }
                curprice = (float)Math.Ceiling(Math.Round(curprice * curTradeStoreItem.normalcoefficient * curTradeStoreItem.upcoefficient / ExpressionConstant,5)) + curprice;
                var maxPrice = (float)Math.Ceiling(Math.Round(m_StoreMgr.GetItemDataDic(curItemId).defaultprice * curTradeStoreItem.uplimit, 2));
                if (curprice > maxPrice)
                {
                    curprice = maxPrice;
                }
                canBuyNum++;
            }
            return canBuyNum;
        }

        #endregion

        //购买功能
        void OnBuyBtnClick()
        {
            if (curSumPrice > m_PackageMgr.GetItemCount(curPriceId))
            {
                xys.App.my.uiSystem.systemHintMgr.ShowHintGo("货币不足，购买功能暂未开发");
                //string des = string.Format("你的{0}货币不足，是否前往购买", Item.Get(curPriceId).name);
                //UICommon.ShowConfirmPannel(des, OkAction, CancelAction);
                return;
            }
            else
            {
                TradeStoreBuyRequest request = new TradeStoreBuyRequest();
                request.itemid = curItemId;
                request.itemnum = curnum;
                request.curprice = curPrice;
                request.curupprice = curUpPrice;
                Event.FireEvent(EventID.TradeStore_Buy, request);
            }
        }

        //出售功能
        void OnSellBtnClick()
        {
            if (m_PackageMgr.GetItemCount(curItemId) == 0)
            {
                xys.UI.Utility.TipContentUtil.Show("tr_noitem");
                return;
            }
            else
            {
                TradeStoreSellRequest request = new TradeStoreSellRequest();
                request.curprice = curPrice;
                request.itemid = curItemId;
                request.itemnum = curnum;
                request.itemindex = curPackageIndexList[preSellItemIndex];
                Event.FireEvent(EventID.TradeStore_Sell, request);
            }
        }

        #region 公用

        //货币不足确定功能
        private void OkAction()
        {

        }

        //货币不足取消功能
        private void CancelAction()
        {

        }


        //物品列表获取货币信息
        TradeItemAtt GetTradeItemData(int itemid)
        {
            return m_StoreMgr.tradeItemData.data[itemid];
        }

        ShangHuiItem GetCurTradeItemData(int itemid)
        {
            for (var i = 0; i < curStoreItemDataList.Count; i++)
            {
                if (curStoreItemDataList[i].itemid == itemid)
                {
                    return curStoreItemDataList[i];
                }
            }
            return null;
        }


        double CountChange(int itemid)
        {
            var tradeItemAtt = m_StoreMgr.GetItemDataDic(itemid);
            double Value = 0;
            if (tradeItemAtt != null)
            {
                Value = Math.Round((tradeItemAtt.curprice - tradeItemAtt.defaultprice) / tradeItemAtt.defaultprice, 4);
            }
            return Value;
        }

        string GetImageNameStr(double value)
        {
            string imageNameStr = value > 0 ? TagImageUpStr : TagImageDownStr;
            return imageNameStr;
        }

        //数量限制
        int LimitNum(int itemid)
        {
            int limitNum = 0;
            if (curPageType == TradeStorePageType.BuyPage)
            {
                ShangHuiItem tempTradeItemData = ShangHuiItem.Get(itemid);
                ShangHuiConfig tempTradeStoreData = ShangHuiConfig.Get(tempTradeItemData.id);
                if (!m_StoreMgr.tradeLimit.buyedTimedic.ContainsKey(tempTradeItemData.itemid))
                {
                    m_StoreMgr.tradeLimit.buyedTimedic.Add(itemid, 0);
                }
                if (tempTradeStoreData.type == (int)TradeStoreType.NoStockStore)
                {
                    if (tempTradeItemData.buylimit == 0)
                    {
                        limitNum = -1;
                    }
                    else
                    {
                        limitNum = tempTradeItemData.buylimit - m_StoreMgr.tradeLimit.buyedTimedic[tempTradeItemData.itemid];
                    }
                }
                else
                {
                    int tempStockStoreNum = m_StoreMgr.tradeItemData.data[tempTradeItemData.itemid].daystocknumcustom + m_StoreMgr.tradeItemData.data[tempTradeItemData.itemid].daystocknumserver;
                    if (tempTradeItemData.buylimit == 0)
                    {
                        limitNum = tempStockStoreNum;
                    }
                    else
                    {
                        int tempLimitNum = tempTradeItemData.buylimit - m_StoreMgr.tradeLimit.buyedTimedic[tempTradeItemData.itemid];
                        if (tempStockStoreNum > tempLimitNum)
                        {
                            limitNum = tempLimitNum;
                        }
                        else
                        {
                            limitNum = tempStockStoreNum;
                        }
                    }
                }
            }
            else if (curPageType == TradeStorePageType.SellPage)
            {
                limitNum = curSellItemList[preSellItemIndex].count;
            }
            return limitNum;
        }

        //当前选择的物品
        void ItemChoosed(int itemid, int num = 1)
        {
            curItemId = itemid;
            curPriceId = (int)GetCurrencyIdNum(itemid)[0];
            curPrice = GetCurrencyIdNum(itemid)[1];
            Item curItemData = itemDataDict[itemid];
            ItemNameText.text = GlobalSymbol.ToUT (string.Format("#[{0}]{1}#n",QualitySourceConfig.Get(curItemData.quality).colorname,curItemData.name));
            ItemDesText.text = GlobalSymbol.ToUT(curItemData.desc);
            Item curCurrencyData = itemDataDict[curPriceId];
            Helper.SetSprite(CostCurrencyIconImage, curCurrencyData.icon);
            Helper.SetSprite(HasCurrencyIconImage, curCurrencyData.icon);
            this.curnum = num;
            RefreshInfoUI();
        }

        //获取货币ID和数量
        float[] GetCurrencyIdNum(int itemid)
        {
            float[] value = new float[2];
            var tradeItemAtt = m_StoreMgr.GetItemDataDic(itemid); ;
            if (tradeItemAtt != null)
            {
                value[0] = (int)tradeItemAtt.priceType;
                value[1] = tradeItemAtt.curprice;
            }
            else
            {
                var curItemData = itemDataDict[itemid];
                if (curItemData.priceBiyu != 0)
                {
                    value[0] = (int)TradeStorePriceType.PriceBiyu;
                    value[1] = curItemData.priceBiyu;
                }
                else if (curItemData.priceGold != 0)
                {
                    value[0] = (int)TradeStorePriceType.PriceGold;
                    value[1] = curItemData.priceGold;
                }
                else if (curItemData.priceSilver != 0)
                {
                    value[0] = (int)TradeStorePriceType.PriceSilver;
                    value[1] = curItemData.priceSilver;
                }
                else
                {
                    value[0] = (int)TradeStorePriceType.PriceSilver;
                    value[1] = 0;
                }
            }
            return value;
        }

        //刷新主界面UI
        void RefreshMainUI()
        {
            if (curPageType == TradeStorePageType.None)
            {
                BuyBtn.onClick.Invoke();
                return;
            }

            else if (curPageType == TradeStorePageType.BuyPage)
            {
                RefreshBuyItemUI();
            }
            else
            {
                RefreshSellItemUI();
            }
            curPrice = GetCurrencyIdNum(curItemId)[1];
            RefreshInfoUI();
        }

        //改变数量刷新右边界面
        void ChangeNumRefreshRightUI(int inputNum)
        {
            App.my.mainCoroutine.StartCoroutine(ChangeNumRefreshUIYield(inputNum));
        }

        IEnumerator ChangeNumRefreshUIYield(int inputNum)
        {
            yield return new WaitForSeconds(0.1f);
            curnum = inputNum;
            CheckNum();
        }

        //初始化
        void SetParent(GameObject gobj, Transform parent)
        {
            gobj.transform.parent = parent;
            gobj.transform.localRotation = Quaternion.identity;
            gobj.transform.localPosition = Vector3.zero;
            gobj.transform.localScale = Vector3.one;
            gobj.SetActive(true);
        }

        //初始或刷新物品
        void InitOrRefreshItem(GameObject initObj, Transform parentTrans, int needInitCount, bool isRefresh, Action<int> initAction, Action<int> refreshAction)
        {
            int itemListCount = 0;
            if (isRefresh)
            {
                itemListCount = needInitCount;
            }
            else
            {
                itemListCount = needInitCount > parentTrans.childCount ? needInitCount : parentTrans.childCount;
            }
            for (var i = 0; i < itemListCount; i++)
            {
                if (!isRefresh)
                {
                    if (i >= parentTrans.childCount)
                    {
                        GameObject initObjClone = GameObject.Instantiate(initObj);
                        SetParent(initObjClone, parentTrans);
                    }
                    else if (i >= needInitCount)
                    {
                        parentTrans.GetChild(i).gameObject.SetActive(false);
                        continue;
                    }
                    parentTrans.GetChild(i).gameObject.SetActive(true);
                    initAction(i);
                }
                refreshAction(i);
            }
        }

        #endregion

        #region 购买
        void InitBuyItemLeft()
        {
            List<ShangHuiConfig> storedatalist = new List<ShangHuiConfig>(storeDataDict.Values);
            List<string> storeParentNameList = new List<string>();
            childlabelBtnSRDic = new Dictionary<int, StateRoot>();
            int labelBtnIndex = 0;
            for (var i = 0; i < storedatalist.Count; i++)
            {
                if (string.IsNullOrEmpty(storedatalist[i].childName))
                {
                    InstantiateParentLabelBtn(labelBtnIndex, storedatalist[i],false,LoadStoreItem);
                    storeParentNameList.Add(storedatalist[i].parentName);
                    labelBtnIndex++;
                }
                else
                {
                    if (!storeParentNameList.Contains(storedatalist[i].parentName))
                    {
                        InstantiateParentLabelBtn(labelBtnIndex, storedatalist[i],true,OpenChildLabelBtn);
                        storeParentNameList.Add(storedatalist[i].parentName);
                        labelBtnIndex++;
                    }
                    Transform childBtnTrans = labelBtnParentTrans.GetChild(labelBtnIndex - 1).Find("subGroup");
                    GameObject childBtnObjClone = GameObject.Instantiate(LabelBtnChildObj);
                    SetParent(childBtnObjClone, childBtnTrans);
                    childBtnObjClone.transform.Find("Text").GetComponent<Text>().text = storedatalist[i].childName;
                    childlabelBtnSRDic.Add(storedatalist[i].id, childBtnObjClone.GetComponent<StateRoot>());
                    var i1 = i;
                    childBtnObjClone.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (childlabelBtnSRDic.ContainsKey(preBuyShopId))
                        {
                            childlabelBtnSRDic[preBuyShopId].SetCurrentState(0, true);
                        }
                        childlabelBtnSRDic[storedatalist[i1].id].SetCurrentState(1, true);
                        LoadStoreItem(storedatalist[i1].id);
                    });
                }
            }
        }

        void LoadStoreItem(int storeid)
        {
            var isRefresh = storeid == preBuyShopId ? true : false;
            preBuyShopId = storeid;
            if (!isRefresh)
            {
                curStoreItemDataList.Clear();
                for (var i = 0; i < storeItemDataList.Count; i++)
                {
                    if (storeItemDataList[i].id == storeid)
                    {
                        curStoreItemDataList.Add(storeItemDataList[i]);
                    }
                }
            }
            InitOrRefreshItem(BuyItemObj, buyItemParentTrans, curStoreItemDataList.Count, isRefresh, InitBuyItemView, RefreshBuyItemValue);
            if (!isRefresh)
            {
                buyItemParentTrans.GetChild(0).GetComponent<Button>().onClick.Invoke();
            }
        }

        void InitBuyItemView(int j)
        {
            var buyitem = buyItemParentTrans.GetChild(j);
            var itemdata = itemDataDict[curStoreItemDataList[j].itemid];
            var currencyicon = itemDataDict[(int)GetTradeItemData(curStoreItemDataList[j].itemid).priceType].icon;
            buyitem.gameObject.SetActive(true);
            buyitem.GetComponent<StateRoot>().SetCurrentState(0, true);
            if (!buyItemSRList.Contains(buyitem.GetComponent<StateRoot>()))
                buyItemSRList.Add(buyitem.GetComponent<StateRoot>());
            //初始化物品列表显示
            var buyItemIconBtn = buyitem.Find("Icon").GetComponent<Button>();
            Helper.SetSprite(buyItemIconBtn.transform.Find("Icon").GetComponent<Image>(), itemdata.icon);
            //buyItemIconBtn.onClick.RemoveAllListeners();
            //buyItemIconBtn.onClick.AddListener(() =>
            //{
            //    UICommon.ShowItemTips(itemdata.id);
            //});
            var qualitysource = QualitySourceConfig.Get(itemdata.quality);
            Helper.SetSprite(buyitem.Find("Icon/Quality").GetComponent<Image>(), qualitysource.icon);
            buyitem.Find("Name").GetComponent<Text>().text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", qualitysource.colorname, itemdata.name));
            //if (itemdata.useLevel != 0)
            //{
            //    buyitem.Find("Name/Level").gameObject.SetActive(true);
            //    buyitem.Find("Name/Level").GetComponent<Text>().text = itemdata.useLevel + "级";
            //}
            //else
            //{
            //    buyitem.Find("Name/Level").gameObject.SetActive(false);
            //}
            Helper.SetSprite(buyitem.transform.Find("JiFen1/icon").GetComponent<Image>(), currencyicon);
            var j1 = j;
            buyitem.GetComponent<Button>().onClick.RemoveAllListeners();
            buyitem.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (preBuyItemIndex >= 0 && buyItemSRList.Count > preBuyItemIndex)
                {
                    buyItemSRList[preBuyItemIndex].SetCurrentState(0, true);
                }
                buyItemSRList[j1].SetCurrentState(1, true);
                preBuyItemIndex = j1;
                ItemChoosed(curStoreItemDataList[j1].itemid);
            });
        }

        void RefreshBuyItemValue(int i)
        {
            ShangHuiItem curItemData = curStoreItemDataList[i];
            Transform curItemTrans = buyItemParentTrans.GetChild(i);
            var currencyprice = (int)GetTradeItemData(curItemData.itemid).curprice;
            var value = CountChange(curItemData.itemid);
            var limitNum = LimitNum(curItemData.itemid);
            var limitNumText = "";
            if (limitNum >= 100 || limitNum == -1)
            {
                limitNumText = "--";
            }
            else if (limitNum == 0)
            {
                limitNumText = "售罄";
            }
            else
            {
                limitNumText = limitNum.ToString();
            }

            curItemTrans.Find("JiFen1/Text").GetComponent<Text>().text = currencyprice.ToString();
            curItemTrans.Find("ShuLiang/Liang").GetComponent<Text>().text = limitNumText;
            if (value == 0)
            {
                curItemTrans.Find("Tag").gameObject.SetActive(false);
            }
            else
            {
                curItemTrans.Find("Tag").gameObject.SetActive(true);
                Helper.SetSprite(curItemTrans.Find("Tag").GetComponent<Image>(), GetImageNameStr(value));
                curItemTrans.Find("Tag/Text").GetComponent<Text>().text = Mathf.Abs((float)value) * 100 + "%";
            }
        }

        //显示子标签页
        void OpenChildLabelBtn(int storeid)
        {
            var childTrans = labelBtnParentTrans.GetChild(preBuyBtnIndex).Find("subGroup");
            if (labelBtnsSRList[preBuyBtnIndex].CurrentState == 1)
            {
                labelBtnsSRList[preBuyBtnIndex].SetCurrentState(2, true);
                childTrans.gameObject.SetActive(true);
                if (storeid != preBuyShopId)
                {
                    childTrans.GetChild(1).GetComponent<Button>().onClick.Invoke();
                }
            }
            else if (labelBtnsSRList[preBuyBtnIndex].CurrentState == 2)
            {
                labelBtnsSRList[preBuyBtnIndex].SetCurrentState(1, true);
                childTrans.gameObject.SetActive(false);
            }
        }

        //生成父标签页
        void InstantiateParentLabelBtn(int labelBtnIndex, ShangHuiConfig storedata,bool ishasChild,Action<int> action)
        {
            if (labelBtnParentTrans.childCount <= labelBtnIndex)
            {
                GameObject labelBtanClone = GameObject.Instantiate(LabelBtnObj);
                SetParent(labelBtanClone, labelBtnParentTrans);
            }
            var labelBtn = labelBtnParentTrans.GetChild(labelBtnIndex);
            var button = labelBtn.Find("Button");
            labelBtnsSRList.Add(labelBtn.GetComponent<StateRoot>());
            labelBtnParentSRList.Add(button.GetComponent<StateRoot>());
            if (ishasChild)
            {
                labelBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
            }
            var i1 = labelBtnIndex;
            var ishas = ishasChild;
            button.Find("Text").GetComponent<Text>().text = storedata.parentName;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {

                if (preBuyBtnIndex == i1 && ishas)
                {
                    action(storedata.id);
                    return;
                }
                else if (preBuyBtnIndex == i1)
                {
                    return;
                }
                if (childlabelBtnSRDic.ContainsKey(preBuyShopId))
                {
                    childlabelBtnSRDic[preBuyShopId].SetCurrentState(0, true);
                }
                if (preBuyBtnIndex >= 0 && preBuyBtnIndex < labelBtnParentSRList.Count)
                {
                    if (labelBtnsSRList[preBuyBtnIndex].CurrentState == 2)
                    {
                        labelBtnsSRList[preBuyBtnIndex].SetCurrentState(1, true);
                    }
                    labelBtnParentSRList[preBuyBtnIndex].SetCurrentState(0, true);
                    labelBtnParentTrans.GetChild(preBuyBtnIndex).Find("subGroup").gameObject.SetActive(false);
                }
                labelBtnParentSRList[i1].SetCurrentState(1, true);
                //labelBtnParentTrans.GetChild(i1).Find("subGroup").gameObject.SetActive(true);
                preBuyBtnIndex = i1;
                action(storedata.id);
            });
        }

        void RefreshBuyItemUI()
        {
            int stid = preBuyShopId;
            LoadStoreItem(stid);
        }
        #endregion

        #region 出售

        void LoadSellItem()
        {
            GetCurPackageItemList();
            if (curSellItemList.Count == 0)
            {
                ItemInfoObj.SetActive(false);
                NoSellItemObj.SetActive(true);
                if (!NoSellItemObj.activeSelf)
                {
                    NoSellItemObj.SetActive(true);
                }
                return;
            }
            NoSellItemObj.SetActive(false);
            ItemInfoObj.SetActive(true);

            InitOrRefreshItem(SellItemObj, sellItemParentTrans, curSellItemList.Count, false, InitSellItemView, RefreshItemSellValue);
            sellItemParentTrans.GetChild(0).Find("Bg").GetComponent<Button>().onClick.Invoke();
        }

        void InitSellItemView(int i)
        {
            var sellItemTrans = sellItemParentTrans.GetChild(i);
            var sellItemData = curSellItemList[i];
            var sellItemSR = sellItemTrans.GetComponent<StateRoot>();
            sellItemSR.SetCurrentState(0, true);
            if (!sellItemSRList.Contains(sellItemSR))
            {
                sellItemSRList.Add(sellItemSR);
            }
            Helper.SetSprite(sellItemTrans.Find("Quality").GetComponent<Image>(), QualitySourceConfig.Get(Item.Get(sellItemData.data.id).quality).icon);
            Helper.SetSprite(sellItemTrans.Find("Icon").GetComponent<Image>(), Item.Get(sellItemData.data.id).icon);
            var i1 = i;
            sellItemTrans.Find("Bg").GetComponent<Button>().onClick.RemoveAllListeners();
            sellItemTrans.Find("Bg").GetComponent<Button>().onClick.AddListener(() =>
            {
                //if (preSellItemIndex == i1) return;
                if (preSellItemIndex >= 0 && preSellItemIndex < sellItemSRList.Count)
                {
                    sellItemSRList[preSellItemIndex].SetCurrentState(0, true);
                }
                preSellItemIndex = i1;
                sellItemSRList[preSellItemIndex].SetCurrentState(1, true);
                ItemChoosed(curSellItemList[preSellItemIndex].data.id);
            });
        }

        void RefreshItemSellValue(int i)
        {
            var sellItemTrans = sellItemParentTrans.GetChild(i);
            var cursellItemData = m_PackageMgr.GetItemInfo(curPackageIndexList[i]);
            if (cursellItemData == null)
            {
                curSellItemList[i].count = 0;
            }
            var sellItemData = curSellItemList[i];
            if (sellItemData.count == 0)
            {
                sellItemTrans.gameObject.SetActive(false);
                for (var j = i; j < sellItemParentTrans.childCount; j++)
                {
                    if (j + 1 >= sellItemParentTrans.childCount)
                    {
                        break;
                    }
                    if (sellItemParentTrans.GetChild(j + 1).gameObject.activeSelf)
                    {
                        sellItemParentTrans.GetChild(j + 1).Find("Bg").GetComponent<Button>().onClick.Invoke();
                        return;
                    }
                }
                for (var w = 0; w < i; w++)
                {
                    if (sellItemParentTrans.GetChild(w).gameObject.activeSelf)
                    {
                        sellItemParentTrans.GetChild(w).Find("Bg").GetComponent<Button>().onClick.Invoke();
                        return;
                    }
                }
                NoSellItemObj.SetActive(true);
                if (!NoSellItemObj.activeSelf)
                {
                    NoSellItemObj.SetActive(true);
                }
                ItemInfoObj.SetActive(false);
                return;
            }
            else if (sellItemData.count == 1)
            {
                sellItemTrans.Find("Text").gameObject.SetActive(false);
            }
            else
            {
                sellItemTrans.Find("Text").gameObject.SetActive(true);
                sellItemTrans.Find("Text").GetComponent<Text>().text = sellItemData.count.ToString();
            }
        }

        void GetCurPackageItemList()
        {
            curSellItemList = new List<NetProto.ItemGrid>();
            curPackageIndexList = new List<int>();
            for (var i = 0; i < m_PackageMgr.package.Count; i++)
            {
                if (m_PackageMgr.GetItemInfo(i) == null || !itemDataDict.ContainsKey(m_PackageMgr.GetItemInfo(i).data.id))
                {
                    continue;
                }
                if (!itemDataDict[m_PackageMgr.GetItemInfo(i).data.id].IsCanSell && m_StoreMgr.GetItemDataDic(m_PackageMgr.GetItemInfo(i).data.id) != null)
                {
                    curSellItemList.Add(m_PackageMgr.GetItemInfo(i));
                    curPackageIndexList.Add(i);
                }
            }
            //ItemHelp.Sort(curSellItemList);
        }

        void RefreshSellItemUI()
        {
            RefreshItemSellValue(preSellItemIndex);
        }

        #endregion
    }
}
#endif