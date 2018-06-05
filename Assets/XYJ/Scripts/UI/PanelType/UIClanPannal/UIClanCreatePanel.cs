#if !USE_HOT
using Config;
using NetProto;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.battle;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIClanCreatePanel : HotTablePanelBase
    {

        xys.UI.HotTablePanel m_Parent;

        [SerializeField]
        Button m_joinClanBtn;
        [SerializeField]
        Button m_responeClanBtn;
        [SerializeField]
        Button m_createBtn;

        [SerializeField]
        Text m_noticeText;

        [SerializeField]
        ClanSimpleListView m_simpleListView;

        //加入面板
        [SerializeField]
        Button m_oneKeyApplyBtn;
        [SerializeField]
        Button m_applyBtn;
        [SerializeField]
        Button m_ContactLeaderBtn;

        //招募 响应页面
        [SerializeField]
        Button m_responeClan;
        [SerializeField]
        Button m_contactLeader;
        [SerializeField]
        Button m_cancelResponeBtn;
        [SerializeField]
        Button m_zhaomuBtn;

        [SerializeField]
        StateRoot m_buttonStateRoot;

        [SerializeField]
        InputField m_inPutField;
        [SerializeField]
        Button m_btnSearch;
        [SerializeField]
        Text m_clanFlagName;
        [SerializeField]
        Transform m_WuTips;
        [SerializeField]
        StateRoot m_titleStateRoot;

        private ClanAllDbData m_allData = null;

        private ClanSimpleUserData m_selectData = null;

        public ClanSimpleListView GetListView()
        {
            return m_simpleListView;
        }
        public ClanSimpleUserData GetSelectData()
        {
            return m_selectData;
        }

        public object transform { get; private set; }

        public int m_selectIdx { get; set; }    //1 为加入列表 0为响应列表

        UIClanCreatePanel() : base(null) { }

        UIClanCreatePanel(xys.UI.HotTablePanel parent) : base(parent)
        {
            this.m_Parent = parent as xys.UI.HotTablePanel;
        }

        protected override void OnInit()
        {
            m_simpleListView.SelectItemHandler += ListView_SelectItemHandler;
           
            m_joinClanBtn.onClick.AddListenerIfNoExist(()=> { this.OnClickGoToJoin(); });
            m_responeClanBtn.onClick.AddListenerIfNoExist(() => { this.OnClickGoToRespone(); });
            m_createBtn.onClick.AddListenerIfNoExist(() => { this.OnClickGoToCreate(); });


            m_oneKeyApplyBtn.onClick.AddListenerIfNoExist(() => { this.OnClickOneKeyApply(); });
            m_applyBtn.onClick.AddListenerIfNoExist(() => { this.OnClickApplyBtn(); });
            m_ContactLeaderBtn.onClick.AddListenerIfNoExist(() => { this.OnClickContactLeader(); });

            m_responeClan.onClick.AddListenerIfNoExist(() => { this.OnClickRespone(); });

            m_contactLeader.onClick.AddListenerIfNoExist(() => { this.OnClickContactLeader(); });

            m_cancelResponeBtn.onClick.AddListenerIfNoExist(() => { this.OnClickCancelRespone(); });

            m_zhaomuBtn.onClick.AddListenerIfNoExist(() => { this.OnClickZhaoMu(); });

            m_btnSearch.onClick.AddListenerIfNoExist(() => { this.OnSearchClan(); });

            m_inPutField.onValueChanged.AddListener((string str) => { this.OnSearchChange(str); });
        }

        private void ListView_SelectItemHandler(ClanSimpleListView listView, ClanSimpleListViewItem item)
        {
            ClanSimpleUserData itemData = item.itemdData;
            if (null == itemData)
                return;

            if (itemData.isSelected)
            {
                m_selectData = itemData;
                RefreshUI();
                UpdataBtnState();
            }
        }

        public void SetTextColor(int colorId)
        {
            Dictionary<int, GameClanFlagColor> allConfig = GameClanFlagColor.GetAll();
            if (colorId >= allConfig.Count)
            {
                return;
            }
            if (allConfig[colorId].mainColor.Length == 6)
            {
                float r = 0;
                float g = 0;
                float b = 0;
                StringToRgb(allConfig[colorId].mainColor, out r, out g, out b);
                m_clanFlagName.color = new Color(r / 255f, g / 255f, b / 255f);
            }

        }


        //必须是16进制颜色str  比如 FFFFFF
        public void StringToRgb(string Str, out float r, out float g, out float b)
        {
            string str1 = Str.Substring(0, 2);
            r = Convert.ToInt32(str1, 16);

            string str2 = Str.Substring(2, 2);
            g = Convert.ToInt32(str2, 16);

            string str3 = Str.Substring(4, 2);
            b = Convert.ToInt32(str3, 16);
        }

        public void ResetUI()
        {
            List<ClanSimpleUserData> clanItems = new List<ClanSimpleUserData>();
            clanItems.Clear();
            if (m_allData != null)
            {
                int idx = 0;
                foreach (var item in m_allData.allClanData)
                {
                    if (item.Value.isrespone == m_selectIdx)
                    {
                        ClanSimpleUserData clanUserData = new ClanSimpleUserData();
                        clanUserData.data = item.Value;
                        clanUserData.showType = m_selectIdx == 0 ? ClanSimpleListViewItem.ShowType.Clan_Respone : ClanSimpleListViewItem.ShowType.Clan_Join;
                        clanUserData.m_isDeepColorBg = idx % 2;
                        if (idx == 0)
                        {
                            clanUserData.isSelected = true;
                            
                            m_selectData = clanUserData;
                            if (m_noticeText != null)
                            {
                                RefreshUI();
                                UpdataBtnState();
                            }
                            
                        }
                        clanItems.Add(clanUserData);                       
                        idx++;
                    }
                }

                m_simpleListView.DataSource = new UIWidgets.ObservableList<ClanSimpleUserData>(clanItems);

                if (clanItems.Count == 0)
                {
                    m_selectData = null;
                }
            }
        }

        public void RefreshUI()
        {
            if (m_selectData != null)
            {
                m_oneKeyApplyBtn.gameObject.SetActive(true);
                m_applyBtn.gameObject.SetActive(true);
                m_ContactLeaderBtn.gameObject.SetActive(true);
                m_noticeText.text = m_selectData.data.notice;
                m_clanFlagName.text = m_selectData.data.flag;
                SetTextColor(m_selectData.data.colorid);
                m_WuTips.gameObject.SetActive(false);
            }
            else
            {
                SetRightNull();
            }
        }

        public void SetRightNull(bool issearch = false)
        {
            m_noticeText.text = "";
            m_clanFlagName.text = "";
            SetTextColor(1);

            m_oneKeyApplyBtn.gameObject.SetActive(false);
            m_applyBtn.gameObject.SetActive(false);
            m_ContactLeaderBtn.gameObject.SetActive(false);

            m_buttonStateRoot.SetCurrentState(4, true);
            if (!issearch)
            {
                m_WuTips.gameObject.SetActive(true);
            }
        }

        public void OnSearchChange(string str)
        {
            if (str == "")
            {
                this.ResetUI();
            }
        }

        protected override void OnShow(object args)
        {
            /*
            int openLv = kvCommon.Get("guildCreateLevel").value.ToInt32();
            if (App.my.localPlayer.levelValue < openLv)
            {
                SystemHintMgr.ShowHint(string.Format("打开氏族创建界面需要角色达到{0}级", openLv));
                xys.App.my.uiSystem.HidePanel("UIClanCreatePanel");
                return;
            }
            */
            Event.Subscribe<ClanAllDbData>(EventID.Clan_RecvAllClan, this.RecvALlClanData);
            Event.fireEvent(EventID.Clan_GetAllClan);
            Event.fireEvent(EventID.Clan_GetSelfClan);
            Event.Subscribe(EventID.Clan_ChangeToRespone, this.ChangeToRespone);
            Event.Subscribe(EventID.LocalAttributeChange, this.RecvAttrChange);
            Event.Subscribe<ClanDbData>(EventID.Clan_RecvSelfClan, this.RecvSelfClan);


            //已经加入了氏族不能打开界面
            if (((ClanMgr)App.my.localPlayer.GetModule<ClanModule>().clanMgr).IsInClan())
            {
                xys.App.my.uiSystem.HidePanel("UIClanCreatePanel");
                //m_joinClanBtn.enabled = false;
                //m_responeClanBtn.enabled = false;
                //m_createBtn.enabled = false;
            }

            m_selectIdx = 1;

            ChangeToRespone();

            RefreshUI();

        }

        protected void ChangeToRespone()
        {
            //创建中无法打开 创建 和 加入界面
            if (App.my.localPlayer.clanResponeIdValue > 0)
            {
                this.m_Parent.ShowType(this.m_Parent.GetPageList()[1].Get().pageType, null);
                //this.m_Parent.CurrentPage = this.m_Parent.GetPageList()[1].Get().pageType;
                m_selectIdx = 0;
                ResetUI();
            }
            UpdataBtnState();
        }

        public void ShowSearch(ClanDbData data)
        {
            if (data != null)
            {
                List<ClanSimpleUserData> clanItems = new List<ClanSimpleUserData>();
                clanItems.Clear();
                ClanSimpleUserData simpleData = new ClanSimpleUserData();
                simpleData.data = data;
                simpleData.isSelected = true;
                simpleData.showType = (this.m_Parent.CurrentPage == this.m_Parent.GetPageList()[0].Get().pageType ? ClanSimpleListViewItem.ShowType.Clan_Join : ClanSimpleListViewItem.ShowType.Clan_Respone);
                clanItems.Add(simpleData);
                m_simpleListView.DataSource = new UIWidgets.ObservableList<ClanSimpleUserData>(clanItems);
            }
            else
            {
                List<ClanSimpleUserData> clanItems = new List<ClanSimpleUserData>();
                clanItems.Clear();
                m_simpleListView.DataSource = new UIWidgets.ObservableList<ClanSimpleUserData>(clanItems);
            }
        }
        public void OnSearchClan(object args = null)
        {
            if (m_inPutField.text != "")
            {
                long clanId = 0;
                if (long.TryParse(m_inPutField.text, out clanId))
                {
                    if (m_allData.allClanData.ContainsKey(clanId))
                    {                       
                        if ((this.m_Parent.CurrentPage == this.m_Parent.GetPageList()[1].Get().pageType && m_allData.allClanData[clanId].isrespone == 0) || (this.m_Parent.CurrentPage == this.m_Parent.GetPageList()[0].Get().pageType && m_allData.allClanData[clanId].isrespone == 1))
                        {
                            ShowSearch(m_allData.allClanData[clanId]);
                        }
                        else
                        {
                            ShowSearch(null);
                            SetRightNull(true);
                            SystemHintMgr.ShowHint("查找不到氏族。");
                        }
                    }
                    else
                    {
                        ShowSearch(null);
                        SetRightNull(true);
                        SystemHintMgr.ShowHint("查找不到氏族。");
                    }
                }
                else
                {
                    ClanDbData searchData = new ClanDbData();
                    bool isHaveResult = false;
                    foreach (var item in m_allData.allClanData)
                    {
                        if (item.Value.name.Equals(m_inPutField.text))
                        {
                            if ((this.m_Parent.CurrentPage == this.m_Parent.GetPageList()[1].Get().pageType && item.Value.isrespone == 0) || (this.m_Parent.CurrentPage == this.m_Parent.GetPageList()[0].Get().pageType && item.Value.isrespone == 1))
                            {
                                ShowSearch(item.Value);
                                isHaveResult = true;
                            }
                        }
                    }
                    if (!isHaveResult)
                    {
                        ShowSearch(null);
                        SetRightNull(true);
                        SystemHintMgr.ShowHint("查找不到氏族。");
                    }  
                }
            }
        }
        public void OnClickGoToJoin(object args = null)
        {   
            if (((ClanMgr)App.my.localPlayer.GetModule<ClanModule>().clanMgr).IsCurDataInCreate())
            {
                //this.m_Parent.ShowType(this.m_Parent.GetPageList()[1].Get().pageType, null);
               // SystemHintMgr.ShowHint("氏族招募中，不可进行此操作。");
                return;
            }
            
            m_selectIdx = 1;
            m_titleStateRoot.SetCurrentState(0,true);
            ResetUI();
            RefreshUI();
        }

        public void OnClickGoToRespone(object args = null)
        {
            m_selectIdx = 0;
            m_titleStateRoot.SetCurrentState(1, true);
            ResetUI();
            RefreshUI();
        }

        public void OnClickGoToCreate(object args = null)
        {
            /*
            if (((ClanMgr)App.my.localPlayer.GetModule<ClanModule>().clanMgr).IsCurDataInCreate())
            {
                this.m_Parent.ShowType(this.m_Parent.GetPageList()[1].Get().pageType, null);
                SystemHintMgr.ShowHint("氏族招募中，不可进行此操作。");
                return;
            }
            */
        }

        protected void RecvALlClanData(ClanAllDbData data)
        {
            m_allData = null;
            m_allData = data;

            ResetUI();

            UpdataBtnState();
        }

        protected void OnClickOneKeyApply(object args = null)
        {
            
           int openLv = kvCommon.Get("guildEnterLevel").value.ToInt32();
           if (App.my.localPlayer.levelValue < openLv)
           {
               SystemHintMgr.ShowHint(string.Format("申请加入氏族需要角色达到{0}级", openLv));
               xys.App.my.uiSystem.HidePanel("UIClanCreatePanel");
               return;
           }
           

            if (m_simpleListView.GetItemsCount() > 0)
            {
                Event.fireEvent(EventID.Clan_OneKeyApply);
            }

        }

        protected void OnClickApplyBtn(object args = null)
        {
            /*
            int openLv = kvCommon.Get("guildEnterLevel").value.ToInt32();
            if (App.my.localPlayer.levelValue < openLv)
            {
                SystemHintMgr.ShowHint(string.Format("申请加入氏族需要角色达到{0}级", openLv));
                xys.App.my.uiSystem.HidePanel("UIClanCreatePanel");
                return;
            }
            */
            if (m_selectData != null)
            {
                Event.FireEvent<long>(EventID.Clan_ApplyClan, m_selectData.ClanId);
            }

        }

        protected void OnClickContactLeader(object args = null)
        {
            if (m_selectData != null && m_selectData.data.leaderid != App.my.localPlayer.charid)
            {
                Event.FireEvent<long>(EventID.Clan_ContactLeader, m_selectData.ClanId);
            }
        }

        protected void OnClickRespone(object args = null)
        {
            /*
            int openLv = kvCommon.Get("guildAnswerLevel").value.ToInt32();
            if (App.my.localPlayer.levelValue < openLv)
            {
                SystemHintMgr.ShowHint(string.Format("响应氏族需要达到{0}级", openLv));
                xys.App.my.uiSystem.HidePanel("UIClanCreatePanel");
                return;
            }
            */
            if (App.my.localPlayer.clanResponeIdValue > 0)
            {
                SystemHintMgr.ShowHint("已经创建或者响应了别的氏族。");
                return;
            }

            if (m_selectData != null)
            {
                ResponeClanMsg msg = new ResponeClanMsg();
                msg.clanid = m_selectData.ClanId;
                msg.iscancel = false;
                App.my.eventSet.FireEvent<ResponeClanMsg>(EventID.Clan_Respone, msg);
            }
            
        }

        protected void OnClickCancelRespone(object args = null)
        {
            if (App.my.localPlayer.clanResponeIdValue == 0)
            {
                SystemHintMgr.ShowHint("你并没有响应这个的氏族。");
                return;
            }
            if (m_selectData != null)
            {
                ResponeClanMsg msg = new ResponeClanMsg();
                msg.clanid = m_selectData.ClanId;
                msg.iscancel = true;
                App.my.eventSet.FireEvent<ResponeClanMsg>(EventID.Clan_Respone, msg);          
            }           
        }

        protected override bool OnPreChange(HotTablePage page)
        {
            if (((ClanMgr)App.my.localPlayer.GetModule<ClanModule>().clanMgr).IsCurDataInCreate())
            {
                //this.m_Parent.ShowType(this.m_Parent.GetPageList()[1].Get().pageType, null);
                SystemHintMgr.ShowHint("氏族招募中，不可进行此操作。");
                return false;
            }
            return true;

        }

        protected void OnClickZhaoMu(object args = null)
        {


        }

        public void UpdataBtnState()
        {
            if (m_buttonStateRoot != null)
            {
                ClanMgr clanmgr = ((ClanMgr)App.my.localPlayer.GetModule<ClanModule>().clanMgr);
                if (clanmgr != null)
                {
                    if (m_selectData != null)
                    {
                        //是否是已经响应中
                        if (App.my.localPlayer.clanResponeIdValue != 0)
                        {
                            //选择中的氏族是否已经包含了自己
                            if (m_selectData.data.member.membermap.ContainsKey(App.my.localPlayer.charid))
                            {
                                if (m_selectData.data.leaderid == App.my.localPlayer.charid)
                                {
                                    m_buttonStateRoot.CurrentState = 1;
                                }
                                else
                                {
                                    m_buttonStateRoot.CurrentState = 3;
                                }
                            }
                            else
                            {
                                if (clanmgr.IsCurDataInCreate())
                                {
                                    m_buttonStateRoot.CurrentState = 0;
                                }
                                else
                                {
                                    m_buttonStateRoot.CurrentState = 2;
                                }
                            }
                        }
                        else
                        {
                            m_buttonStateRoot.CurrentState = 2;
                        }
                    }
                }
            }
        }

        public void RecvAttrChange()
        {
            UpdataBtnState();
            ChangeToRespone();

            if (App.my.localPlayer.clanIdValue != 0)
            {
                xys.App.my.uiSystem.HidePanel("UIClanCreatePanel");
            }
        }

        public void RecvSelfClan(ClanDbData data)
        {
            this.ResetUI();
        }
    }

}
#endif