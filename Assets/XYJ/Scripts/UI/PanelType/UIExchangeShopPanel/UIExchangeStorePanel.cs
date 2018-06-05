#if !USE_HOT
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using UnityEngine.UI;
using Config;
using xys.UI.State;
using System.Text;

namespace xys.hot.UI
{

    enum ShopType
    {

        ExchangeShopIndependence = 0,
        ExchangeShopNonIndependence = 1,
    }
    class UIExchangeStorePanel : HotPanelBase
    {
        private ShopType sType;
        private Dictionary<int, ExchangeShopData> shopdatadict = new Dictionary<int, ExchangeShopData>();
        private Dictionary<int, Item> itemdatadict = new Dictionary<int, Item>();
        private List<ExchangeStoreData> storedatadict = new List<ExchangeStoreData>();
        private Dictionary<ItemQuality, QualitySourceConfig> qualitydatadict = new Dictionary<ItemQuality, QualitySourceConfig>();
        private List<ExchangeStoreData> storedatalist;

        private Transform infoTrans;
        private Transform proIconTrans;
        private Transform labelTrans;
        private Transform labelrootTrans;
        private Transform exitemTrans;

        private int limitNum; //限制数量
        private int preNum; //已兑换数量
        private int curNum;//当前购买数量
        private int hascurrency1num; //玩家拥有货币1数量
        private int hascurrency2num; //玩家拥有货币2数量
        private int hasmaterialnum; //玩家拥有材料数量
        private bool currency1View = true;
        private bool currency2View = true;
        private bool materialView = true;
        private ExchangeStoreData storedata;
        private Text curNumText;
        private Text curExItemlimitText;
        //private CheckItemView checkItemView;

        private int preparentlabelBtnIndex = 0;
        private int preexItemBtnIndex = 0;
        private int prechildlabelBtnIndex = 0;

        private bool isExSuccessful; //兑换成功标识
        private bool isCurView;//信息面板显示
        private Vector3 keyboardpos; //小键盘位置
        private Vector3 tipspos; //货币提示位置
        private float tipsLength; //货币提示位置间距
        private float parentLabelHeight; //父标签页高度
        private float childLabelHeight; //子标签页高度
        private float initLabelWidth; //初始标签页宽度
        private float childlabelLength = 5; //子标签页间距

        [SerializeField]
        Button ReturnBtn;
        //标签页父按钮
        [SerializeField]
        GameObject LabelBtn;
        //标签页子按钮
        [SerializeField]
        GameObject LabelRootBtn;
        //兑换物品
        [SerializeField]
        GameObject ExItem;

        [SerializeField]
        Text ItemName;
        [SerializeField]
        Text Desc;
        [SerializeField]
        ScrollRect DescRect;
        [SerializeField]
        Text GradeLimitText;
        [SerializeField]
        Image HasIcon1;
        [SerializeField]
        Text HasNum1;
        [SerializeField]
        Image CostIcon1;
        [SerializeField]
        Text CostNum1;
        [SerializeField]
        Image HasIcon2;
        [SerializeField]
        Text HasNum2;
        [SerializeField]
        Image CostIcon2;
        [SerializeField]
        Text CostNum2;
        [SerializeField]
        Image CostMetrialIcon;
        [SerializeField]
        Text CostMerialNum;
        [SerializeField]
        Button InfoBtn;
        [SerializeField]
        Button MaterialBtn;
        [SerializeField]
        Button ExchangeBtn;
        [SerializeField]
        Button AddBtn;
        [SerializeField]
        StateRoot AddSRoot;
        [SerializeField]
        Button MinBtn;
        [SerializeField]
        StateRoot MinSRoot;
        [SerializeField]
        Button InputNumBtn;
        [SerializeField]
        Text InfoTipsText;

        UIHotPanel m_Parent;
        PackageMgr m_packageMgr;

        public UIExchangeStorePanel() : base(null)
        {

        }

        public UIExchangeStorePanel(UIHotPanel parent) : base(parent)
        {
            m_Parent = parent;
        }

        protected override void OnInit()
        {
            this.m_packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            BeginInit();
        }

        void BeginInit()
        {
            infoTrans = m_Parent.transform.Find("Root/Info");
            proIconTrans = m_Parent.transform.Find("Root/Info/Material");
            labelTrans = LabelBtn.transform.parent;
            exitemTrans = ExItem.transform.parent;
            curNumText = InputNumBtn.GetComponent<Text>();
            InfoTipsText.transform.parent.gameObject.SetActive(false);

            tipspos = InfoTipsText.transform.parent.localPosition;
            tipsLength = HasIcon1.transform.parent.parent.GetComponent<RectTransform>().rect.height / 2;
            keyboardpos = new Vector3(400, 175);

            parentLabelHeight = LabelBtn.GetComponent<RectTransform>().rect.height;
            initLabelWidth = LabelBtn.GetComponent<RectTransform>().rect.width;
            childLabelHeight = LabelRootBtn.GetComponent<RectTransform>().rect.height + childlabelLength;

            InitBtn();
        }

        void InitBtn()
        {
            InfoBtn.onClick.AddListener(() =>
            {
                OnInfoBtnClick();
            });
            AddBtn.onClick.AddListener(() =>
            {
                OnAddBtnClick();
            });
            MinBtn.onClick.AddListener(() =>
            {
                OnMinBtnClick();
            });
            InputNumBtn.onClick.AddListener(() =>
            {
                CheckNum();
                OnInputBtnClick();

            });
            ExchangeBtn.onClick.AddListener(() =>
            {
                ExchangeBtnClick();
            });
            ReturnBtn.onClick.AddListener(() =>
            {
                App.my.uiSystem.HidePanel(PanelType.UIExchangeStorePanel);
            });
            MaterialBtn.onClick.AddListener(() =>
            {
                UICommon.ShowItemTips(storedata.materialid);
            });
        }

        protected override void OnShow(object args)
        {
            sType = (ShopType)args;
            Event.Subscribe(EventID.ExchangeStore_RefreshUI, () =>
            {
                ExStoreMgr exstoreMgr = hotApp.my.GetModule<HotExStoreModule>().exstoreMgr;
                if (exstoreMgr == null)
                    return;
                preNum = exstoreMgr.GetUsedTime(storedata.itemid);
                curNum = 1;
                isExSuccessful = true;
                CheckNum();

            });
            //Event.Subscribe(EventID.ExchangeStore_Successful, (object[] arg) =>
            //{
            //    //ExchangeSuccessfulTips((int)arg[0], (int)arg[1]);
            //});

            LeftBtnView();
        }

        protected override void OnHide()
        {

        }

        //标签按钮生成
        void LeftBtnView()
        {
            shopdatadict = ExchangeShopData.GetAll();
            itemdatadict = Item.GetAll();
            storedatadict = ExchangeStoreData.GetAll();
            qualitydatadict = QualitySourceConfig.GetAll();
            //labelTrans.localPosition = new Vector3(labelTrans.localPosition.x, 0);
            List<ExchangeShopData> shopdatalist = new List<ExchangeShopData>(shopdatadict.Values);
            Dictionary<string, List<ExchangeShopData>> shopparentdict = new Dictionary<string, List<ExchangeShopData>>();
            var labelBtnindex = 0;
            for (var i = 0; i < shopdatalist.Count; i++)
            {
                if (shopdatalist[i].type == (int)sType)
                {
                    if (string.IsNullOrEmpty(shopdatalist[i].childname))
                    {
                        shopparentdict.Add(shopdatalist[i].parentname, new List<ExchangeShopData>() { shopdatalist[i] });
                        if (labelTrans.childCount <= labelBtnindex)
                        {
                            var Labelbtnclone = GameObject.Instantiate(LabelBtn);
                            SetParent(Labelbtnclone, labelTrans);
                        }
                        var index = labelBtnindex;
                        var i1 = i;
                        var labelrootTrans = labelTrans.GetChild(index).GetChild(1);
                        var labelbtnTrans = labelTrans.GetChild(index).GetChild(0);
                        labelbtnTrans.Find("Name").GetComponent<Text>().text = shopdatalist[i].parentname;
                        labelTrans.GetChild(index).GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
                        {
                            labelTrans.GetChild(preparentlabelBtnIndex).GetChild(0).GetComponent<StateRoot>().FrontState();
                            labelTrans.GetChild(preparentlabelBtnIndex).GetChild(1).gameObject.SetActive(false);
                            labelTrans.GetChild(preparentlabelBtnIndex).GetComponent<RectTransform>().sizeDelta = new Vector2(initLabelWidth, parentLabelHeight);
                            preparentlabelBtnIndex = index;
                            labelTrans.GetChild(preparentlabelBtnIndex).GetChild(0).GetComponent<StateRoot>().NextState();
                            OnLabelBtnClick(shopparentdict[shopdatalist[i1].parentname], labelrootTrans);
                        });
                        labelrootTrans.gameObject.SetActive(false);
                        labelBtnindex++;
                    }
                    else
                    {
                        shopparentdict[shopdatalist[i].parentname].Add(shopdatalist[i]);
                    }
                }
            }
            labelTrans.GetChild(0).GetChild(0).GetComponent<Button>().onClick.Invoke();
        }
        //标签按钮点击响应
        void OnLabelBtnClick(List<ExchangeShopData> shopparentlist, Transform labelrootTrans)
        {
            if (shopparentlist.Count == 1)
            {
                ShowItem(shopparentlist[0].id);
            }
            else if (shopparentlist.Count > 1)
            {
                labelrootTrans.gameObject.SetActive(true);
                if (labelrootTrans.childCount < shopparentlist.Count - 1)
                {
                    for (int i = 1; i < shopparentlist.Count; i++)
                    {
                        if (labelrootTrans.childCount <= i - 1)
                        {
                            var Labelrootbtnclone = GameObject.Instantiate(LabelRootBtn);
                            SetParent(Labelrootbtnclone, labelrootTrans);
                        }
                        labelrootTrans.GetChild(i - 1).Find("Name").GetComponent<Text>().text = shopparentlist[i].childname;
                        var i1 = i;
                        labelrootTrans.GetChild(i - 1).GetComponent<Button>().onClick.AddListener(() =>
                        {
                            labelrootTrans.GetChild(prechildlabelBtnIndex).GetComponent<StateRoot>().FrontState();
                            prechildlabelBtnIndex = i1 - 1;
                            labelrootTrans.GetChild(prechildlabelBtnIndex).GetComponent<StateRoot>().NextState();
                            ShowItem(shopparentlist[i1].id);
                        });
                    }
                }
                var chidlabelheight = labelrootTrans.childCount * childLabelHeight + parentLabelHeight;
                labelTrans.GetChild(preparentlabelBtnIndex).GetComponent<RectTransform>().sizeDelta = new Vector2(initLabelWidth, chidlabelheight);
                labelrootTrans.GetChild(0).GetComponent<StateRoot>().NextState();
                labelrootTrans.GetChild(0).GetComponent<Button>().onClick.Invoke();
            }
            else
            {

            }

        }


        //显示兑换物品
        void ShowItem(int shopid)
        {
            storedatalist = new List<ExchangeStoreData>();
            exitemTrans.localPosition = new Vector3(exitemTrans.localPosition.x, 0);
            foreach (var storedata in storedatadict)
            {
                if (storedata.id == shopid && (storedata.joblimit.Has(hotApp.my.localPlayer.job)))
                {
                    storedatalist.Add(storedata);
                }
            }
            if (exitemTrans.childCount <= storedatalist.Count)
            {
                LoadExItemBtn(storedatalist.Count, exitemTrans, storedatalist);
            }
            else
            {
                LoadExItemBtn(exitemTrans.childCount, exitemTrans, storedatalist);
            }
        }

        //初始化Item
        void LoadExItemBtn(int Count, Transform itemTrans, List<ExchangeStoreData> storedatalist)
        {
            ExStoreMgr exstoreMgr = hotApp.my.GetModule<HotExStoreModule>().exstoreMgr;
            if (exstoreMgr == null)
                return;
            for (var i = 0; i < Count; i++)
            {
                var currency1flag = true;
                var currency2flag = true;
                if (itemTrans.childCount <= i)
                {
                    var exitemclone = GameObject.Instantiate(ExItem);
                    exitemclone.GetComponent<StateRoot>().SetCurrentState(0, true);
                    SetParent(exitemclone, itemTrans);
                }
                else if (storedatalist.Count <= i)
                {
                    itemTrans.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
                else if (!itemTrans.GetChild(i).gameObject.activeSelf)
                {
                    itemTrans.GetChild(i).gameObject.SetActive(true);
                }
                var exitemdata = storedatalist[i];
                //var itemIconBtn = itemTrans.GetChild(i).Find("Icon").GetComponent<Button>();
                Helper.SetSprite(itemTrans.GetChild(i).Find("Icon").Find("Icon").GetComponent<Image>(), itemdatadict[exitemdata.itemid].icon);
                //itemIconBtn.onClick.RemoveAllListeners();
                //itemIconBtn.onClick.AddListener(() =>
                //{
                //    UICommon.ShowItemTips(exitemdata.itemid);
                //});
                Helper.SetSprite(itemTrans.GetChild(i).Find("Icon/Quality").GetComponent<Image>(), qualitydatadict[itemdatadict[exitemdata.itemid].quality].icon);
                itemTrans.GetChild(i).Find("Name/Grid/Label").GetComponent<Text>().text = ColorTextView(itemdatadict[exitemdata.itemid].quality, itemdatadict[exitemdata.itemid].name);
                var textView = itemTrans.GetChild(i).Find("Level").GetComponent<Text>();
                ExItemTextView(textView, exitemdata.itemid, exstoreMgr);
                var costitem1Trans = itemTrans.GetChild(i).Find("JiFen1");
                if (exitemdata.currency1id == 0)
                {
                    costitem1Trans.gameObject.SetActive(false);
                    currency1flag = false;
                }
                else
                {
                    costitem1Trans.gameObject.SetActive(true);
                    Helper.SetSprite(costitem1Trans.Find("icon").GetComponent<Image>(), itemdatadict[exitemdata.currency1id].icon);
                    costitem1Trans.Find("Text").GetComponent<Text>().text = exitemdata.currency1num.ToString();

                }
                var costitem2Trans = itemTrans.GetChild(i).Find("JiFen2");
                if (exitemdata.currency2id == 0)
                {
                    costitem2Trans.gameObject.SetActive(false);
                    currency2flag = false;
                }
                else
                {
                    costitem2Trans.gameObject.SetActive(true);
                    Helper.SetSprite(costitem2Trans.Find("icon").GetComponent<Image>(), itemdatadict[exitemdata.currency2id].icon);
                    costitem2Trans.Find("Text").GetComponent<Text>().text = exitemdata.currency2num.ToString();
                }
                var exitembtn = itemTrans.GetChild(i).GetComponent<Button>();
                exitembtn.onClick.RemoveAllListeners();
                var i1 = i;
                exitembtn.onClick.AddListener(() =>
                {
                    itemTrans.GetChild(preexItemBtnIndex).GetComponent<StateRoot>().FrontState();
                    curExItemlimitText = itemTrans.GetChild(i1).Find("Level").GetComponent<Text>();
                    preexItemBtnIndex = i1;
                    itemTrans.GetChild(preexItemBtnIndex).GetComponent<StateRoot>().NextState();
                    ExchangeInfo(exitemdata.itemid, currency1flag, currency2flag);

                });
            }
            itemTrans.GetChild(0).GetComponent<Button>().onClick.Invoke();
        }

        //兑换信息
        void ExchangeInfo(int itemid, bool currency1flag, bool currency2flag)
        {
            ExStoreMgr exstoreMgr = hotApp.my.GetModule<HotExStoreModule>().exstoreMgr;
            if (exstoreMgr == null)
                return;
            if (storedata != null && storedata.itemid == itemid)
            {
                preNum = exstoreMgr.GetUsedTime(storedata.itemid);
                CheckNum();
                return;
            }
            storedata = ExchangeStoreData.Get(itemid);
            preNum = exstoreMgr.GetUsedTime(storedata.itemid);
            curNum = 1;
            curNumText.text = curNum.ToString();
            ItemName.text = ColorTextView(itemdatadict[storedata.itemid].quality, itemdatadict[storedata.itemid].name);
            var desctext = GlobalSymbol.ToUT(itemdatadict[itemid].desc);
            Desc.text = desctext;
            if (storedata.level == 0)
            {
                GradeLimitText.transform.gameObject.SetActive(false);
            }
            else
            {
                GradeLimitText.transform.gameObject.SetActive(true);
                if (storedata.level <= hotApp.my.localPlayer.levelValue)
                {
                    GradeLimitText.text = string.Format("<color=#e9f1fb>兑换等级:{0}</color>", storedata.level);
                }
                else
                {
                    GradeLimitText.text = string.Format("<color=#ef3c49>兑换等级:{0}</color>", storedata.level);
                }
            }
            limitNum = storedata.daylimit;
            if (limitNum == 0)
            {
                limitNum = storedata.weeklimit;
            }
            if (!currency1flag)
            {
                CostIcon1.transform.parent.parent.gameObject.SetActive(false);
                currency1View = false;
            }
            else
            {
                CostIcon1.transform.parent.parent.gameObject.SetActive(true);
                Helper.SetSprite(HasIcon1, itemdatadict[storedata.currency1id].icon);
                Helper.SetSprite(CostIcon1, itemdatadict[storedata.currency1id].icon);
                currency1View = true;
            }
            if (!currency2flag)
            {
                CostIcon2.transform.parent.parent.gameObject.SetActive(false);
                currency2View = false;
            }
            else
            {
                CostIcon2.transform.parent.parent.gameObject.SetActive(true);
                Helper.SetSprite(HasIcon2, itemdatadict[storedata.currency2id].icon);
                Helper.SetSprite(CostIcon2, itemdatadict[storedata.currency2id].icon);
                currency2View = true;
            }
            if (storedata.materialid != 0)
            {
                proIconTrans.gameObject.SetActive(true);
                Helper.SetSprite(CostMetrialIcon, itemdatadict[storedata.materialid].icon);
                materialView = true;
            }
            else
            {
                proIconTrans.gameObject.SetActive(false);
                materialView = false;
            }
            CheckNum();
        }

        //检测判断数量
        void CheckNum()
        {
            AddSRoot.SetCurrentState(1, false);
            AddBtn.enabled = true;
            MinSRoot.SetCurrentState(1, false);
            MinBtn.enabled = true;
            if (curNum >= limitNum - preNum && limitNum != 0)
            {
                AddSRoot.SetCurrentState(0, false);
                AddBtn.enabled = false;
                curNum = limitNum - preNum;
            }
            if (curNum <= 1)
            {
                MinSRoot.SetCurrentState(0, false);
                MinBtn.enabled = false;
                //curNum = 1;
            }
            if (limitNum == preNum && limitNum != 0)
            {
                curNum = 1;
            }
            ViewNum();
        }

        void ViewNum()
        {
            curNumText.text = curNum.ToString();
            if (currency1View)
            {
                hascurrency1num = m_packageMgr.GetItemCount(storedata.currency1id);
                var costcurrency1str = GlobalSymbol.ToBef(curNum * storedata.currency1num);
                HasNum1.text = GlobalSymbol.ToBef(hascurrency1num);//玩家拥有数量
                if (hascurrency1num >= curNum * storedata.currency1num)
                {
                    CostNum1.text = string.Format("<color=#e9f1fb>{0}</color>", costcurrency1str);
                }
                else
                {
                    CostNum1.text = string.Format("<color=#ef3c49>{0}</color>", costcurrency1str);
                }
            }
            if (currency2View)
            {
                hascurrency2num = m_packageMgr.GetItemCount(storedata.currency2id);
                var costcurrency2str = GlobalSymbol.ToBef(curNum * storedata.currency2num);
                HasNum2.text = GlobalSymbol.ToBef(hascurrency2num);//玩家拥有数量
                if (hascurrency2num >= curNum * storedata.currency2num)
                {
                    CostNum2.text = string.Format("<color=#e9f1fb>{0}</color>", costcurrency2str);
                }
                else
                {
                    CostNum2.text = string.Format("<color=#ef3c49>{0}</color>", costcurrency2str);
                }
            }
            if (materialView)
            {
                hasmaterialnum = m_packageMgr.GetItemCount(storedata.materialid);
                if (hasmaterialnum >= storedata.materialnum * curNum)
                {
                    CostMerialNum.text = string.Format("<color=#61e171>{0}/{1}</color>", hasmaterialnum, storedata.materialnum * curNum);
                }
                else
                {
                    CostMerialNum.text = string.Format("<color=#ef3c49>{0}/{1}</color>", hasmaterialnum, storedata.materialnum * curNum);
                }
            }
            if (curExItemlimitText != null && isExSuccessful)
            {
                isExSuccessful = false;
                ExItemTextView(curExItemlimitText);
            }

        }

        #region 信息面板按钮
        //信息按钮
        void OnInfoBtnClick()
        {

            //打开tips
            InfoTipsText.transform.parent.gameObject.SetActive(true);
            InfoTipsText.transform.parent.localPosition = currency2View ? tipspos : tipspos + tipsLength * Vector3.down;
            InfoTipsText.text = itemdatadict[storedata.currency1id].desc;

        }

        //添加按钮
        void OnAddBtnClick()
        {
            curNum++;
            CheckNum();
        }

        //减少
        void OnMinBtnClick()
        {
            curNum--;
            CheckNum();
        }

        void OnInputBtnClick()
        {
            var param = SetParam();
            App.my.uiSystem.ShowPanel(PanelType.UICalculatorPanel, param);
        }

        void ExchangeBtnClick()
        {

            if (preNum == limitNum && limitNum != 0)
            {
                xys.UI.Utility.TipContentUtil.Show("ex_limit");
                return;
            }

            if (curNum == 0)
            {
                //弹出提示
                xys.UI.Utility.TipContentUtil.Show("ex_null");
                return;
            }

            if (hascurrency1num < storedata.currency1num * curNum && currency1View)
            {
                //弹出积分不足提示
                xys.UI.Utility.TipContentUtil.Show("ex_notengough", itemdatadict[storedata.currency1id].name);
                return;
            }
            if (hascurrency2num < storedata.currency2num * curNum && currency2View)
            {
                //弹出货币不足提示
                xys.UI.Utility.TipContentUtil.Show("ex_notengough", itemdatadict[storedata.currency2id].name);
                return;
            }
            if (materialView && (hasmaterialnum < storedata.materialnum * curNum))
            {
                //弹出材料不足提示
                UICommon.ShowItemTips(storedata.materialid);
                return;
            }

            if (storedata.level > hotApp.my.localPlayer.levelValue)
            {
                xys.UI.Utility.TipContentUtil.Show("ex_levelerror");
                return;
            }

            if (storedata == null) return;
            NetProto.ExchangeItemReq request = new NetProto.ExchangeItemReq();
            request.exchangeItem.itemid = storedata.itemid;
            request.exchangeItem.itemnum = curNum;
            request.exchangeItem.currency1num = storedata.currency1num * curNum;
            request.exchangeItem.currency2num = storedata.currency2num * curNum;
            request.exchangeItem.materialnum = storedata.materialnum * curNum;
            Event.FireEvent<NetProto.ExchangeItemReq>(EventID.ExchangeStore_Echange, request);

        }
        #endregion


        #region 部分方法实现
        //小键盘
        UICalculatorPanel.Param SetParam()
        {

            UICalculatorPanel.Param param = new UICalculatorPanel.Param();
            param.minValue = 0;
            param.defaultValue = curNum;
            param.pos = keyboardpos;
            param.valueChange += ValueChange;
            if (limitNum != 0)
            {
                param.maxValue = limitNum;
            }
            else
            {
                var maxvalue = 0;
                if (currency1View)
                {
                    maxvalue = (int)(hascurrency1num / storedata.currency1num);
                    if (currency2View)
                    {
                        maxvalue = (int)(hascurrency2num / storedata.currency2num) > maxvalue ? maxvalue : (int)(hascurrency2num / storedata.currency2num);
                    }
                    if (materialView)
                    {
                        maxvalue = (int)(hasmaterialnum / storedata.materialnum) > maxvalue ? maxvalue : (int)(hasmaterialnum / storedata.materialnum);
                    }
                }
                if (currency2View)
                {
                    maxvalue = (int)(hascurrency2num / storedata.currency2num);
                    if (materialView)
                    {
                        maxvalue = (int)(hasmaterialnum / storedata.materialnum) < maxvalue ? (int)(hasmaterialnum / storedata.materialnum) : maxvalue;
                    }
                }
                if (materialView)
                {
                    maxvalue = (int)(hasmaterialnum / storedata.materialnum);
                }
                param.maxValue = maxvalue;
            }
            return param;
        }

        void ValueChange(int index)
        {

            curNumText.text = index.ToString();
            curNum = index;
            CheckNum();
        }

        //ExItem上面限购显示
        void ExItemTextView(Text text, int itemid, ExStoreMgr m_ExStoreMgr)
        {

            var exusedtime = m_ExStoreMgr.GetUsedTime(itemid);
            var daylimit = ExchangeStoreData.Get(itemid).daylimit;
            var weeklimit = ExchangeStoreData.Get(itemid).weeklimit;
            var canextime = "";
            if (daylimit == 0 && weeklimit == 0)
            {
                canextime = "-";
            }
            else if (daylimit != 0)
            {
                canextime = (daylimit - exusedtime).ToString();
            }
            else if (weeklimit != 0)
            {
                canextime = (weeklimit - exusedtime).ToString();
            }
            text.text = string.Format("<color=#ACCEF4FF>限购:</color>{0}", canextime);
        }

        void ExItemTextView(Text text)
        {
            if (limitNum != 0)
                text.text = string.Format("<color=#ACCEF4FF>限购:</color>{0}", limitNum - preNum);
            //CheckNum();
        }

        //字体颜色
        string ColorTextView(ItemQuality qualitycolor, string name)
        {
            var colorcode = qualitydatadict[qualitycolor].color;
            return string.Format("<color=#{0}>{1}</color>", colorcode, name);
        }

        //初始化prefab
        void SetParent(GameObject gobject, Transform parent)
        {
            gobject.transform.parent = parent;
            gobject.transform.localPosition = Vector3.zero;
            gobject.transform.localScale = Vector3.one;
        }

        //兑换成功显示tips
        //void ExchangeSuccessfulTips(int itemid, int count)
        //{
        //    List<Obtain> obainList = new List<Obtain>();
        //    Obtain obtain = new Obtain(itemid, count);
        //    obainList.Add(obtain);
        //    ObtainItemShowMgr.ShowObtain(obainList);
        //}

        //获取颜色名字
        string GetColorName(string colorcode)
        {
            Dictionary<int, ColorConfig> datalist = ColorConfig.GetAll();
            foreach (var colordata in datalist)
            {
                if (colordata.Value.colorCode == colorcode)
                {
                    return colordata.Value.colorName;
                }
            }
            return "";
        }
        #endregion
    }
}


#endif