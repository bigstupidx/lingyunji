#if !USE_HOT
using System;
using UnityEngine;
using UnityEngine.UI;
using xys.battle;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIFriendPanel : HotTablePanelBase
    {
        public enum Page
        {
            FriendPage = 0,
            MailPage = 1,
        }

        xys.UI.HotTablePanel m_Parent;
        

        [SerializeField]
        Transform m_FriendTitleControl;
        [SerializeField]
        Button m_HaoYouBtn;

        [SerializeField]
        Button m_MailBtn;
        [SerializeField]
        GameObject m_MailRedPoint;
        [SerializeField]
        StateRoot m_State;

        [SerializeField]
        Button m_CloseBtn;

        EventAgent m_EventAgent { get; set; }
        public object transform { get; private set; }

        UIFriendPanel() : base(null) { }

        UIFriendPanel(xys.UI.HotTablePanel parent) : base(parent)
        {
            this.m_Parent = parent as xys.UI.HotTablePanel;
        }

        protected override void OnInit()
        {

            this.m_EventAgent = this.m_Parent.Event;

            m_HaoYouBtn.onClick.RemoveAllListeners();
            m_MailBtn.onClick.RemoveAllListeners();
            m_CloseBtn.onClick.RemoveAllListeners();

            m_HaoYouBtn.onClick.AddListener(() => { this.OnFriendPage(); });

            m_MailBtn.onClick.AddListener(() => { this.OnMailPage(); });

            m_CloseBtn.onClick.AddListener(() => { this.OnClose(); });

            App.my.eventSet.Subscribe<int>(EventID.Friend_ShowRed_Point, this.ShowRedPoint);

        }

        
        public void OnFriendPage(object args = null)
        {
            this.m_Parent.ShowType(this.m_Parent.GetPageList()[(int)Page.FriendPage].Get().pageType, args);
            m_State.SetCurrentState(0,true);
            RefreshFrame();
        }

        public void OnMailPage(object args = null)
        {
            this.m_Parent.ShowType(this.m_Parent.GetPageList()[(int)Page.MailPage].Get().pageType, args);
            m_State.SetCurrentState(1, true);
            RefreshFrame();
        }

        protected override void OnShow(object args)
        {
            this.m_Parent.CurrentPage = this.m_Parent.GetPageList()[(int)Page.FriendPage].Get().pageType;
            m_EventAgent.fireEvent(EventID.Friend_GetFriendData);
            m_EventAgent.fireEvent(EventID.Friend_GetRecentlyInfo);
            m_EventAgent.fireEvent(EventID.Friend_GetApplyData);

            m_EventAgent.Subscribe(EventID.Mail_NewMailFlagChange, OnNewMailFlagChange);
            m_MailRedPoint.SetActive(MailDef.mailMgr.newMailFlag);
            RefreshFrame();
        }


        protected override void OnHide()
        {
           
        }

        void OnClose(object args = null)
        {
            xys.App.my.uiSystem.HidePanel(PanelType.UIFriendPanel);
        }


        public void RefreshFrame()
        {
            if (this.m_Parent.CurrentPage == this.m_Parent.GetPageList()[(int)Page.FriendPage].Get().pageType)
            {
                m_FriendTitleControl.Find("TitleBg/Title").GetComponent<Text>().text = "好友";

            }
            else if (this.m_Parent.CurrentPage == this.m_Parent.GetPageList()[(int)Page.MailPage].Get().pageType)
            {
                m_FriendTitleControl.Find("TitleBg/Title").GetComponent<Text>().text = "邮件";

            }
        }

        public void ShowRedPoint(int count)
        {
            if (m_HaoYouBtn != null)
            {
                m_HaoYouBtn.transform.Find("RedPoint").gameObject.SetActive(count > 0 ? true : false);
            }
        }

        private void OnNewMailFlagChange()
        {
            m_MailRedPoint.SetActive(MailDef.mailMgr.newMailFlag);
        }
    }
}

#endif