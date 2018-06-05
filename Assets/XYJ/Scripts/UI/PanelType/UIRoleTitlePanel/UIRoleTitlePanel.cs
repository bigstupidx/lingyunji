#if !USE_HOT
namespace xys.hot.UI
{
    using NetProto;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using xys.UI.State;
    using Config;
    using UIWidgets;

    class UIRoleTitlePanel : HotPanelBase
    {
        [SerializeField]
        Button m_Close;

        [SerializeField]
        Button m_WearTitle;

        [SerializeField]
        Accordion m_Accordion;

        [SerializeField]
        GameObject m_ToggleNode;    // 大类节点

        [SerializeField]
        GameObject m_ContentNode;   // 小类节点

        [SerializeField]
        GameObject m_ContentRoot;   // 小类容器

        [SerializeField]
        GameObject m_RootNode;      // 树根节点

        [SerializeField]
        Text m_TitleNameText;       // 称号名字文本

        [SerializeField]
        Text m_TitleTimeLimitText;  // 称号时效文本

        [SerializeField]
        Text m_TitelDescText;       // 称号描述文本

        [SerializeField]
        Text m_AttrDescText;        // 属性加成文本

        [SerializeField]
        Text m_WaysToObtainDescText;// 获得方式文本

        [SerializeField]
        ScrollRect m_ScrollRect;    // 称号滚动条

        TitleList m_TitleData;      // 称号数据

        Dictionary<int, int> m_ContentList; // 已经创建的小类节点标识列表

        Dictionary<int, GameObject> m_ToggleNodeObjects;

        Dictionary<int, GameObject> m_ContentNodeObjects;

        int m_SelectTitleId;

        AccordionItem m_SelectedItem;

        public UIRoleTitlePanel() :base(null){ }
        public UIRoleTitlePanel(xys.UI.UIHotPanel panel) : base(panel)
        {
        }

        C2ATitleRequest request_;
        C2ATitleRequest request
        {
            get
            {
                if (request_ == null)
                    request_ = ((HotTitleModule)App.my.localPlayer.GetModule<TitleModule>().refType.Instance).request;

                return request_;
            }
        }

        protected override void OnInit()
        {
            m_ContentList = new Dictionary<int, int>();
            m_ToggleNodeObjects = new Dictionary<int, GameObject>();
            m_ContentNodeObjects = new Dictionary<int, GameObject>();
            m_TitleData = App.my.localPlayer.GetModule<TitleModule>().m_TitleListData as TitleList;

            m_SelectTitleId = m_TitleData.currTitle;

            LoadAccordion();
            LoadInitTitle();
            OnUpdateContentNode();
            RegistButton();
        }

        protected override void OnShow(object args)
        {
            RegistEvent();

            // 显示当前穿戴的称号信息
            if (RoleTitle.Get(m_TitleData.currTitle) != null)
                OnShowTitleDes(m_TitleData.currTitle);

            // 展开当前穿戴的大类节点
            OpenSelectedToggle();
        }

        protected override void OnHide()
        {
            // 收起节点
            if (m_SelectedItem != null)
                m_Accordion.Close(m_SelectedItem, false);

            // 取消选中
            if (m_SelectTitleId > 0 && RoleTitle.Get(m_SelectTitleId) != null)
            {
                int typeId = RoleTitle.Get(m_SelectTitleId).type;
                if (m_ToggleNodeObjects.ContainsKey(typeId))
                {
                    m_ToggleNodeObjects[typeId].transform.GetComponent<StateRoot>().CurrentState = 0;
                }
            }
            if (m_ContentNodeObjects.ContainsKey(m_SelectTitleId))
                m_ContentNodeObjects[m_SelectTitleId].transform.GetComponent<StateRoot>().CurrentState = 0;
        }

        // 展开当前穿戴的大类节点h
        void OpenSelectedToggle()
        {
            AccordionItem accItem = null;
            int initId = 0;
            if (RoleTitle.Get(m_TitleData.currTitle) != null)
            {
                initId = m_TitleData.currTitle;
            }
            else
            {
                initId = GetDefineTitle();
            }

            int type = RoleTitle.Get(initId).type;
            if (m_ToggleNodeObjects.ContainsKey(type))
            {
                foreach (var v in m_Accordion.Items)
                {
                    if (v.ToggleObject == m_ToggleNodeObjects[type])
                    {
                        accItem = v;
                        break;
                    }
                }
            }

            if (accItem != null)
            {
               m_Accordion.Open(accItem, false);
               CallToggleItem(accItem, type, false);
               CallContentItem(initId);
            }
        }

        // 检查大类中是否存在小类
        bool CheckChildOfParent(int id)
        {
            foreach (var v in RoleTitle.GetAll())
            {
                if (v.Value.type == id)
                    return true;
            }
            return false;
        }

        void RegistEvent()
        {
            Event.Subscribe(EventID.Title_Change, OnUpdataTitlePanel);
        }

        void RegistButton()
        {
            m_WearTitle.onClick.AddListener(OnWearTitle);
            m_Close.onClick.AddListener(OnClosePanel);
        }

        void LoadAccordion()
        {
            AccordionItem item;
            Dictionary<int, RoleSortTitle> titleParent = RoleSortTitle.GetAll();
            foreach (var v in titleParent)
            {
                if (!CheckChildOfParent(v.Key))
                    continue;

                item = new AccordionItem();
                CreateToggleObject(item, v.Key);
                CreateContentObject(item);
                m_Accordion.Items.Add(item);
            }
        }

        // 加载默认称号
        void LoadInitTitle()
        {
            int initId = GetDefineTitle();
            OnShowTitleDes(initId);
        }

        void CreateToggleObject(AccordionItem acc, int id)
        {
            GameObject node = GameObject.Instantiate(m_ToggleNode);

            Text nodeText = node.transform.Find("Text").GetComponent<Text>();
            nodeText.text = RoleSortTitle.Get(id).name;
            node.transform.SetParent(m_RootNode.transform);
            node.SetActive(true);

            acc.ToggleObject = node;
            acc.ToggleObject.transform.localPosition = Vector3.zero;
            acc.ToggleObject.transform.localScale = new Vector3(1, 1, 1);
            acc.ContentObjectHeight = 0;
            acc.Open = false;

            Button btn = acc.ToggleObject.GetComponent<Button>();
            UnityEngine.Events.UnityAction callback = () => CallToggleItem(acc, id);

            btn.onClick.AddListener(callback);

            m_ToggleNodeObjects[id] = node;
        }

        // content容器
        void CreateContentObject(AccordionItem acc)
        {
            GameObject contentObj = GameObject.Instantiate(m_ContentRoot);
            contentObj.transform.SetParent(m_RootNode.transform);
            contentObj.transform.localPosition = Vector3.zero;
            contentObj.transform.localScale = new Vector3(1, 1, 1);
            contentObj.SetActive(true);

            acc.ContentObject = contentObj;
        }

        // content小类节点
        void CreateContentChild(AccordionItem acc, int id)
        {
            GameObject node = GameObject.Instantiate(m_ContentNode);
            Text nodeText = node.transform.Find("Text").GetComponent<Text>();
            nodeText.text = RoleTitle.Get(id).name;
            node.transform.SetParent(acc.ContentObject.transform);

            node.transform.localPosition = Vector3.zero;
            node.transform.localScale = new Vector3(1, 1, 1);
            node.SetActive(true);

            Button btn = node.GetComponent<Button>();
            UnityEngine.Events.UnityAction callback = () => CallContentItem(RoleTitle.Get(id).id);

            btn.onClick.AddListener(callback);

            m_ContentNodeObjects[id] = node;
        }

        void CallToggleItem(AccordionItem item, int id, bool module = true)
        {
            // 当为打开状态时，收起
            if (item.Open && module)
            {
                m_Accordion.Close(item);
                m_ToggleNodeObjects[id].transform.GetComponent<StateRoot>().CurrentState = 0;
                if (m_ContentNodeObjects.ContainsKey(m_SelectTitleId))
                    m_ContentNodeObjects[m_SelectTitleId].transform.GetComponent<StateRoot>().CurrentState = 0;
                return;
            }

            // 创建新节点
            int count = 0;
            bool isNew = false;
            foreach (var v in RoleTitle.GetAll())
            {
                if (v.Value.type == id)
                {
                    // 组件没创建节点
                    if (!m_ContentList.ContainsKey(v.Key))
                    {
                        // 能显示或已经开启
                        if ((v.Value.isShow) || m_TitleData.list.ContainsKey(v.Key))
                        {
                            // 如果不显示，且，是限时称号，且，称号过时
                            if (!v.Value.isShow && 
                                RoleTitle.Get(v.Key).timeLimt == TitleTimeLimit.limit && 
                                App.my.localPlayer.cdMgr.isEnd(CDType.Title, (short)v.Key))
                            {
                                continue;
                            }

                            CreateContentChild(item, v.Key);
                            m_ContentList[v.Key] = id;
                            isNew = true;
                            count++;
                        }
                    }
                    else if (m_TitleData.list.ContainsKey(v.Key))
                    {
                        // 已经创建节点，且，未激活不显示，且，限时称号失效，删除节点
                        if (!v.Value.isShow && 
                            RoleTitle.Get(v.Key).timeLimt == TitleTimeLimit.limit && 
                            App.my.localPlayer.cdMgr.isEnd(CDType.Title, (short)v.Key))
                        {
                            Object.Destroy(m_ContentNodeObjects[v.Key]);
                            m_ContentList.Remove(v.Key);
                            m_ContentNodeObjects.Remove(v.Key);
                            item.ContentObjectHeight -= m_ContentNode.GetComponent<RectTransform>().sizeDelta.y;
                        }
                    }
                }
            }

            if (isNew)
            {
                float y = m_ContentNode.GetComponent<RectTransform>().sizeDelta.y;
                item.ContentObjectHeight += (y + 5f) * count;
                m_Accordion.UpdateLayout();
            }

            // 展示小类节点的穿戴标识
            RoleTitle titleConfig = RoleTitle.Get(m_TitleData.currTitle);
            if (titleConfig != null && titleConfig.type == id)
            {
                OnShowWearFlag(m_TitleData.currTitle);
            }

//            App.my.mainCoroutine.StartCoroutine(SetScrollRect(id));

            App.my.mainCoroutine.StartCoroutine(OpenContentArea(id));

            m_SelectedItem = item;

            return;
        }

        void CallContentItem(int titleId)
        {
            OnShowTitleDes(titleId);
            m_SelectTitleId = titleId;
            SetContentSelectedStatus(titleId);
            return;
        }

        // 设置大类节点选中状态
        void SetToggleNodeSelectedStatus(int id)
        {
            foreach (var v in m_ToggleNodeObjects)
            {
                if (v.Key == id)
                    v.Value.transform.GetComponent<StateRoot>().CurrentState = 1;
                else
                    v.Value.transform.GetComponent<StateRoot>().CurrentState = 0;
            }
        }

        // 设置小类节点选中状态
        void SetContentSelectedStatus(int titleId)
        {
            foreach (var v in m_ContentNodeObjects)
            {
                if (v.Key == titleId)
                    v.Value.transform.GetComponent<StateRoot>().CurrentState = 1;
                else
                    v.Value.transform.GetComponent<StateRoot>().CurrentState = 0;
            }
        }

        // 设置小类节点的开启状态
        void SetContentOpenStatus(int id)
        {
            // 筛选数据
            Dictionary<int, int> list = new Dictionary<int, int>();
            RoleTitle titleConfig = null;
            foreach (var value in m_TitleData.list)
            {
                titleConfig = RoleTitle.Get(value.Key);
                if (titleConfig != null && titleConfig.type == id)
                    list.Add(value.Key, value.Key);
            }

            foreach (var obj in m_ContentNodeObjects)
            {
                if (list.ContainsKey(obj.Key))
                    if (RoleTitle.Get(obj.Key).timeLimt == TitleTimeLimit.limit)
                    {
                        if (!App.my.localPlayer.cdMgr.isEnd(CDType.Title, (short)obj.Key))
                            SetTitleShowStatus(obj.Key, 1);
                        else
                            SetTitleShowStatus(obj.Key, 0);
                    }
                    else
                        SetTitleShowStatus(obj.Key, 1);
                else
                    SetTitleShowStatus(obj.Key, 0);
            }
        }

        // 设置称号显示状态
        void SetTitleShowStatus(int titleId, int status)
        {
            RoleTitle titleConfig = RoleTitle.Get(titleId);
            m_ContentNodeObjects[titleId].transform.Find("Text").GetComponent<StateRoot>().CurrentState = status;
            Text textObj = m_ContentNodeObjects[titleId].transform.Find("Text").GetComponent<Text>();
            if (status == 1)
            {
                SetTitleTextQualiy(textObj, titleId);
            }
            else
            {
                textObj.text = titleConfig.name;
            }
        }

        // 设置称号字体品质
        void SetTitleTextQualiy(Text obj, int titleId)
        {
            RoleTitle titleConfig = RoleTitle.Get(titleId);
            if (titleConfig == null)
                return;
            QualitySourceConfig qualityConfig = QualitySourceConfig.Get(titleConfig.quality);
            if (titleConfig.specialEqulity != "")
            {
                obj.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}#n", titleConfig.specialEqulity, titleConfig.name));
            }
            else
            {
                if (qualityConfig != null)
                {
                    obj.text = string.Format("<color=#{0}>{1}</color>", qualityConfig.color, titleConfig.name);
                }
                else
                {
                    obj.text = titleConfig.name;
                }
            }
        }

        System.Collections.IEnumerator OpenContentArea(int id)
        {
            yield return new WaitForSecondsRealtime(0.01f);

            // 设置状态
            SetToggleNodeSelectedStatus(id);
            // 设置开启状态
            SetContentOpenStatus(id);
            // 小类节点排序
            OnContentSort(id);
        }

            // 设置滚动区域
        System.Collections.IEnumerator SetScrollRect(int id)
        {
            yield return new WaitForSecondsRealtime(0.08f);

            int count = 0;
            foreach (var v in m_ContentNodeObjects)
            {
                if (RoleTitle.Get(v.Key).type == id)
                    count++;
            }

            float toggleHeight = m_ToggleNode.GetComponent<RectTransform>().sizeDelta.y;
            float contentHeight = m_ContentNode.GetComponent<RectTransform>().sizeDelta.y;
            float scrollHeight = m_ScrollRect.GetComponent<RectTransform>().sizeDelta.y + (contentHeight + 5) * count;
            float value = 1 - ((id - 1f) * (toggleHeight + 40f) / scrollHeight);
            m_ScrollRect.verticalNormalizedPosition = value;
        }

        // 展示称号信息
        void OnShowTitleDes(int titleId)
        {
            // 按钮状态
            m_WearTitle.transform.gameObject.SetActive(true);

            if ((m_TitleData.list.ContainsKey(titleId) && titleId != m_TitleData.currTitle) || titleId == 0)
                if (titleId != 0 && RoleTitle.Get(titleId).timeLimt == TitleTimeLimit.limit && App.my.localPlayer.cdMgr.isEnd(CDType.Title, (short)titleId))
                    m_WearTitle.transform.gameObject.SetActive(false);
                else
                    m_WearTitle.transform.Find("text").GetComponent<Text>().text = "佩戴称号";
            else if (m_TitleData.list.ContainsKey(titleId) && titleId == m_TitleData.currTitle)
                m_WearTitle.transform.Find("text").GetComponent<Text>().text = "取消称号";
            else
                m_WearTitle.transform.gameObject.SetActive(false);

            RoleTitle titleConfig = RoleTitle.Get(titleId);
            if (titleConfig == null)
                return;

            SetTitleTextQualiy(m_TitleNameText, titleId);

            if (titleConfig.timeLimt == TitleTimeLimit.forever)
            {
                m_TitleTimeLimitText.text = "永久称号";
            }
            else
            {
                if (RoleTitle.Get(titleId).timeLimt == TitleTimeLimit.limit && !App.my.localPlayer.cdMgr.isEnd(CDType.Title, (short)titleId))
                {
                    OnShowLimitTimes(titleId);
                }
                else
                {
                    m_TitleTimeLimitText.text = "限时称号";
                }
            }

            m_TitelDescText.text = GlobalSymbol.ToUT(titleConfig.titleDesc);

            if (titleConfig.attrDesc == "")
                m_AttrDescText.text = "无加成";
            else
                m_AttrDescText.text = GlobalSymbol.ToUT(titleConfig.attrDesc);

            m_WaysToObtainDescText.text = GlobalSymbol.ToUT(titleConfig.waysToObtainDesc);
            return;
        }

        // 穿戴称号
        void OnWearTitle()
        {
            Int32 input = new Int32();
            input.value = m_SelectTitleId;
            request.WearTitle(input, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.ret != 0)
                {
                    ErrorCode code = ErrorCode.Get(respone.ret);
                    if (code == null)
                        return;
                    xys.UI.SystemHintMgr.ShowHint(code.desc);
                    return;
                }
                else
                {
                    TipsContent tipsConfig = TipsContent.Get(respone.tipsCode);
                    if (tipsConfig == null)
                        return;
                    xys.UI.SystemHintMgr.ShowHint(tipsConfig.des);
                }
            });
            return;
        }

        // 关闭面板
        void OnClosePanel()
        {
            App.my.uiSystem.HidePanel(xys.UI.PanelType.UIRoleTitlePanel);
            return;
        }

        // 重置佩戴标识
        void OnResetWearFlag()
        {
            foreach (var v in m_ToggleNodeObjects)
                v.Value.transform.Find("Tag").GetComponent<Image>().gameObject.SetActive(false);

            foreach (var v in m_ContentNodeObjects)
                v.Value.transform.Find("Tag").GetComponent<Image>().gameObject.SetActive(false);
        }

        // 展示佩戴标识
        void OnShowWearFlag(int titleId)
        {
            RoleTitle titleConfig = RoleTitle.Get(titleId);
            if (titleConfig == null)
                return;

            if (m_ToggleNodeObjects.ContainsKey(titleConfig.type))
                m_ToggleNodeObjects[titleConfig.type].transform.Find("Tag").GetComponent<Image>().gameObject.SetActive(true);

            if (m_ContentNodeObjects.ContainsKey(titleId))
                m_ContentNodeObjects[titleId].transform.Find("Tag").GetComponent<Image>().gameObject.SetActive(true);

        }

        // 更新小类节点
        void OnUpdateContentNode()
        {
            OnResetWearFlag();
            OnShowWearFlag(m_TitleData.currTitle);
        }

        // 刷新面板
        void OnUpdataTitlePanel()
        {
            OnShowTitleDes(m_TitleData.currTitle);
            OnUpdateContentNode();
        }

        // 更新限时称号时间
        void OnShowLimitTimes(int titleId)
        {
            CDEventData data = App.my.localPlayer.cdMgr.GetData(CDType.Title, (short)titleId);

            string text = "有效时间：";

            int unitDay = 60 * 60 * 24;
            int unitHour = 60 * 60;
            int unitMin = 60;
            float invalid = data.total - data.elapse;
            int day = (int)(invalid / unitDay);
            int hour = (int)((invalid - day * unitDay) / unitHour);
            int minute = (int)((invalid - day * unitDay - hour * unitHour )/ unitMin);
            if (day > 0)
                text = text + string.Format("{0}天", day);
            else if (hour > 0)
                text = text + string.Format("{0}小时", hour);
            else if (minute == 0)
                text = text + string.Format("{0}分钟", 1);
            else
                text = text + string.Format("{0}分钟", minute + 1);

            m_TitleTimeLimitText.text = text;
        }

        // 获得默认称号
        int GetDefineTitle()
        {
            int initId = m_TitleData.currTitle;
            RoleTitle titleConfig = RoleTitle.Get(m_TitleData.currTitle);
            if (titleConfig == null)
            {
                foreach (var v in RoleTitle.GetAll())
                {
                    initId = v.Value.id;
                    break;
                }
            }

            return initId;
        }

        // 小类节点排序
        void OnContentSort(int id)
        {
            // 称号id列表
            List<int> sortTemp = new List<int>();
            List<int> sortTemp2 = new List<int>();
            foreach (var v in m_ContentNodeObjects)
            {
                if (RoleTitle.Get(v.Key).type == id)
                {
                    if (m_TitleData.list.ContainsKey(v.Key))
                        if (RoleTitle.Get(v.Key).timeLimt == TitleTimeLimit.limit && App.my.localPlayer.cdMgr.isEnd(CDType.Title, (short)v.Key))
                            sortTemp2.Add(v.Key);
                        else
                            sortTemp.Add(v.Key);
                    else
                        sortTemp2.Add(v.Key);
                }
            }

            // 对id排序
            sortTemp.Sort(cmp);
            sortTemp2.Sort(cmp);

            foreach (int v in sortTemp2)
            {
                sortTemp.Add(v);
            }

            // 整理层次下标
            for (int i = 0; i < sortTemp.Count; ++i)
            {
                m_ContentNodeObjects[sortTemp[i]].transform.SetSiblingIndex(i);
            }

            m_Accordion.UpdateLayout();
        }

        int cmp(int x, int y)
        {
            return x.CompareTo(y);
        }
    }
}
#endif