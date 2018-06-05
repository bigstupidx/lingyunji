#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI.State;

namespace xys.hot.UI 
{
    [AutoILMono]
    class MainUITeamPanel
    {
        class MemberUIItem
        {
            const int STATE_HIDE = 0;
            const int STATE_FARWAY = 1;
            const int STATE_NEARBY = 2;
            public MemberUIItem(int _idx, GameObject _go)
            {
                idx = _idx;
                go = _go;
            }

            public GameObject go;
            public List<TeamMemberData> dataList;
            public int idx;
            public TeamMemberData data
            {
                get
                {
                    if (null == dataList || idx >= dataList.Count)
                        return null;
                    return dataList[idx];
                }
            }

            public void Render()
            {
                TeamMemberData info = data;

                if (null == info || info.uid == App.my.localPlayer.charid)
                {
                    go.GetComponent<StateRoot>().SetCurrentState(STATE_HIDE, true);
                    return;
                }

                long zoneId =TeamUtil.teamMgr.zoneId;
                if (info.zoneId == zoneId && info.isOnline)
                {
                    go.GetComponent<StateRoot>().SetCurrentState(STATE_NEARBY, false);
                    go.GetComponent<CanvasGroup>().alpha = 1.0f;
                }
                else
                {
                    go.GetComponent<StateRoot>().SetCurrentState(STATE_FARWAY, false);
                    go.GetComponent<CanvasGroup>().alpha = 0.5f;
                }
                go.transform.Find("name").GetComponent<Text>().text = info.name.ToString();
                go.transform.Find("levelText").GetComponent<Text>().text = info.level.ToString();
                go.transform.Find("leaderIcon").gameObject.SetActive(0 == idx);
                go.transform.Find("FollowIcon").gameObject.SetActive(info.isFollow);

                TeamProfSexResConfig cfg = TeamUtil.GetProfSexResCfg(info.prof, info.sex);
                if (null != cfg)
                {
                    Image profIcon = go.transform.Find("headIcon/icon").GetComponent<Image>();
                    xys.UI.Helper.SetSprite(profIcon, cfg.profIcon);
                }
            }
        }

        [SerializeField]
        GameObject m_self;
        [SerializeField]
        GameObject m_noMemberPage;
        [SerializeField]
        Button m_createTeamBtn;
        [SerializeField]
        Button m_fastJoinBtn;
        [SerializeField]
        GameObject m_memberPage;
        [SerializeField]
        Button m_chatBtn;
        [SerializeField]
        Button m_leaveBtn;
        [SerializeField]
        Button m_followBtn;
        [SerializeField]
        Button m_callBtn;
        [SerializeField]
        StateRoot m_btnGroup;
        [SerializeField]
        Button m_ownerTip_LeaveBtn;

        [SerializeField]
        GameObject m_teamTipPanel;
        [SerializeField]
        GameObject m_teamTipOwnerPanel;
        [SerializeField]
        GameObject m_teamTipOtherPanel;
        [SerializeField]
        GameObject m_teamTipLeaderPanel;
        [SerializeField]
        Transform m_memberList;
        [SerializeField]
        Button m_tipOther_ChatBtn;
        [SerializeField]
        Button m_tipOther_CheckInfoBtn;
        [SerializeField]
        Button m_tipOther_MakeFriendBtn;
        [SerializeField]
        Button m_tipLeader_ChatBtn;
        [SerializeField]
        Button m_tipLeader_CheckInfoBtn;
        [SerializeField]
        Button m_tipLeader_MakeFriendBtn;
        [SerializeField]
        Button m_tipLeader_SetLeaderBtn;
        [SerializeField]
        Button m_tipLeader_KickBtn;
        [SerializeField]
        Button m_inviteMsgBtn;
        [SerializeField]
        GameObject m_autoFillFlag;

        [SerializeField]
        Text m_teamTabBtnText;
        [SerializeField]
        GameObject m_unreadJoinReqFlag;

        MemberUIItem m_tipPanelCurrTarget;

        List<MemberUIItem> m_memberUIItems = new List<MemberUIItem>();
        public MainUITeamPanel()
        {
            
        }

        void Awake()
        {
            m_createTeamBtn.onClick.AddListenerIfNoExist(OnClickCreateTeamBtn);
            m_fastJoinBtn.onClick.AddListenerIfNoExist(OnClicFastJoinBtn);

            for (int i = 0; i < TeamDef.MAX_MEMBER_COUNT; ++i)
            {
                Transform item = m_memberList.Find(i.ToString());
                MemberUIItem memberUIItem = new MemberUIItem(m_memberUIItems.Count, item.gameObject);
                m_memberUIItems.Add(memberUIItem);
                Button itemBtn = item.GetComponent<Button>();
                itemBtn.onClick.AddListenerIfNoExist(() => { OnClickMemberItem(memberUIItem); });
            }

            m_chatBtn.onClick.AddListenerIfNoExist(OnClickChatBtn);
            m_leaveBtn.onClick.AddListenerIfNoExist(OnClickLeaveTeamBtn);
            m_followBtn.onClick.AddListenerIfNoExist(OnClickFollowBtn);
            m_callBtn.onClick.AddListenerIfNoExist(OnClickCallBtn);
            m_ownerTip_LeaveBtn.onClick.AddListenerIfNoExist(OnClickLeaveTeamBtn);
            m_teamTipOwnerPanel.SetActive(false);
            m_teamTipOtherPanel.SetActive(false);
            m_teamTipLeaderPanel.SetActive(false);


            m_tipOther_ChatBtn.onClick.AddListenerIfNoExist(this.OnClickTipPanelChatBtn);
            m_tipOther_CheckInfoBtn.onClick.AddListenerIfNoExist(this.OnClickTipPanelCheckInfoBtn);
            m_tipOther_MakeFriendBtn.onClick.AddListenerIfNoExist(this.OnClickTipPanelMakeFriendBtn);
            m_tipLeader_ChatBtn.onClick.AddListenerIfNoExist(this.OnClickTipPanelChatBtn);
            m_tipLeader_CheckInfoBtn.onClick.AddListenerIfNoExist(this.OnClickTipPanelCheckInfoBtn);
            m_tipLeader_MakeFriendBtn.onClick.AddListenerIfNoExist(this.OnClickTipPanelMakeFriendBtn);
            m_tipLeader_SetLeaderBtn.onClick.AddListenerIfNoExist(this.OnClickTipPanelSetLeaderBtn);
            m_tipLeader_KickBtn.onClick.AddListenerIfNoExist(this.OnClickPanelKickBtn);
            m_inviteMsgBtn.onClick.AddListenerIfNoExist(this.OnClickInviteMsgBtn);
            m_inviteMsgBtn.GetComponent<StateRoot>().SetCurrentState(1, false);
            m_inviteMsgBtn.gameObject.SetActive(false);

            App.my.eventSet.Subscribe<TeamAllTeamInfo>(EventID.Team_DataChange, OnDataChange);
            App.my.eventSet.Subscribe<TeamSundryData>(EventID.Team_SundryDataChange, OnSundryDataChange);
            App.my.eventSet.Subscribe<Dictionary<int, TeamInviteJoinInfo>>(EventID.Team_InviteJoinInfoChange, OnInviteJoinInfoChange);
            App.my.eventSet.Subscribe(EventID.Team_UnreadJoinReqInfoFlagChange, OnUnreadJoinReqInfoFlagChange);

            this.UpdateUI();
        }

        TeamAllTeamInfo m_teamAllInfo = new TeamAllTeamInfo();
        TeamSundryData m_sundryData = new TeamSundryData();

        bool IsInTeam() { return m_teamAllInfo.members.Count > 0; }
        bool IsLeader() { return App.my.localPlayer.charid == m_teamAllInfo.leaderUid; }

        private void OnClickCreateTeamBtn()
        {
            TeamUtil.ShowOrganizePanel();
           TeamUtil.teamMgr.CreateTeam(0);
        }

        private void OnClicFastJoinBtn()
        {
            xys.App.my.uiSystem.ShowPanel("UITeamPlatformPanel", new object() { }, true);
        }
        private void OnClickChatBtn()
        {

        }

        xys.UI.Dialog.TwoBtn m_twoBtn;
        private void OnClickLeaveTeamBtn()
        {
            m_twoBtn = xys.UI.Dialog.TwoBtn.Show(
                "", "确定离开队伍？",
                "取消", () => false,
                "确定", () =>
                {
                   TeamUtil.teamMgr.LeaveTeam();
                    App.my.uiSystem.HidePanel("UITeamOrganizePanel");
                    xys.App.my.uiSystem.ShowPanel("UITeamPlatformPanel", new object() { }, true);
                    return false;
                }, true, true, () => { m_twoBtn = null; });
        }

        private void OnClickFollowBtn()
        {
           TeamUtil.teamMgr.SetFollow(!TeamUtil.teamMgr.IsFollow());
        }

        private void OnClickCallBtn()
        {
            this.HideTipPanel();
           TeamUtil.teamMgr.CallFollow();
        }

        private void OnClickTipPanelChatBtn()
        {
            this.HideTipPanel();
        }

        private void OnClickTipPanelCheckInfoBtn()
        {
            this.HideTipPanel();
            TeamMemberData data = m_tipPanelCurrTarget.data;
            TeamUtil.ShowRoleOperationPanel(data.uid, data.name, data.sex, data.prof, data.level, Vector3.zero);
        }

        private void OnClickTipPanelMakeFriendBtn()
        {
            this.HideTipPanel();
            App.my.eventSet.FireEvent<long>(EventID.Friend_Apply, m_tipPanelCurrTarget.data.uid);
        }

        private void OnClickTipPanelSetLeaderBtn()
        {
            this.HideTipPanel();

            if (null == m_tipPanelCurrTarget || null == m_tipPanelCurrTarget.data)
                return;

           TeamUtil.teamMgr.SetTeamLeader(m_tipPanelCurrTarget.data.uid);
        }

        private void OnClickPanelKickBtn()
        {
            this.HideTipPanel();

            if (null == m_tipPanelCurrTarget || null == m_tipPanelCurrTarget.data)
                return;

           TeamUtil.teamMgr.KickMember(m_tipPanelCurrTarget.data.uid);
        }

        private void OnClickInviteMsgBtn()
        {
            xys.App.my.uiSystem.ShowPanel("UITeamRspInvitePanel",TeamUtil.teamMgr.GetInviteJoinInfos(), true);
        }


        int m_testClickTimes = 0;
        private void OnClickMemberItem(MemberUIItem item)
        {
            m_tipPanelCurrTarget = item;
            m_teamTipPanel.SetActive(false);
            m_teamTipOwnerPanel.SetActive(false);
            m_teamTipOtherPanel.SetActive(false);
            m_teamTipLeaderPanel.SetActive(false);

            GameObject go = null;
            if (item.data.uid == App.my.localPlayer.charid)
            {
                go = m_teamTipOwnerPanel;
            }
            else if (this.IsLeader())
            {
                go = m_teamTipLeaderPanel;
            }
            else
            {
                go = m_teamTipOtherPanel;
            }

            if (null != go)
            {
                const int AdjustX = -15;
                const int AdjustY = -13;
                RectTransform rectTransform = item.go.transform as RectTransform;
                Vector3 scrPos = App.my.uiSystem.uguiCamera.WorldToScreenPoint(item.go.transform.position);
                Vector3 tarScrPos = new Vector3(scrPos.x + rectTransform.rect.width / 4 + AdjustX, scrPos.y + rectTransform.rect.height / 2 + AdjustY, scrPos.z);
                Vector3 retPos = App.my.uiSystem.uguiCamera.ScreenToWorldPoint(tarScrPos);
                m_teamTipPanel.transform.position = retPos;
                m_teamTipPanel.SetActive(true);
                go.SetActive(true);
                this.AddTipPanelEventHandle();
            }
        }

        int m_teamTipEventId = 0;
        protected void AddTipPanelEventHandle()
        {
            this.RemoveTipPanelEventHandle();
            m_teamTipEventId = xys.UI.EventHandler.pointerClickHandler.Add(OnGlobalClick);
        }
        protected void RemoveTipPanelEventHandle()
        {
            if (0 != m_teamTipEventId)
            {
                xys.UI.EventHandler.pointerClickHandler.Remove(m_teamTipEventId);
                m_teamTipEventId = 0;
            }
        }
        protected bool OnGlobalClick(GameObject go, UnityEngine.EventSystems.BaseEventData bed)
        {
            if (go == null || !go.transform.IsChildOf(this.m_teamTipPanel.GetComponent<Transform>()))
            {
                this.m_teamTipPanel.SetActive(false);
                this.RemoveTipPanelEventHandle();
            }

            return true;
        }

        private void OnDataChange(TeamAllTeamInfo info)
        {
            m_teamAllInfo = info;
            this.UpdateUI();
        }
        protected void OnSundryDataChange(TeamSundryData sundryData)
        {
            m_sundryData = sundryData;
            this.UpdateUI();
        }
        protected void OnUnreadJoinReqInfoFlagChange()
        {
            if (null != m_unreadJoinReqFlag)
                m_unreadJoinReqFlag.SetActive(this.IsInTeam() &&TeamUtil.teamMgr.hasUnreadJoinReqInfo);
        }
        protected void OnInviteJoinInfoChange(Dictionary<int, TeamInviteJoinInfo> infos)
        {
            m_inviteMsgBtn.gameObject.SetActive(infos.Count > 0);
        }
        
        private void UpdateUI()
        {
            if (null == m_memberPage || null == m_noMemberPage)
                return;

            bool isInTeam = this.IsInTeam();
            bool isShowMemberPage = (isInTeam || m_sundryData.isAutoJoinTeam);
            m_memberPage.SetActive(isShowMemberPage);
            m_noMemberPage.SetActive(!isShowMemberPage);

            bool isLeader = this.IsLeader();
            m_btnGroup.SetCurrentState((isLeader ? 1 : 0), true);
            m_btnGroup.gameObject.SetActive(m_teamAllInfo.members.Count >= 2);

            if (!isInTeam)
            {
                m_teamTipPanel.SetActive(false);
            }

            List<TeamMemberData> dataList = TeamUtil.SortTeamMember(m_teamAllInfo);
            for (int i = 0; i < m_memberUIItems.Count; ++ i)
            {
                MemberUIItem item = m_memberUIItems[i];
                item.dataList = dataList;
                item.Render();
            }

            bool isFollow =TeamUtil.teamMgr.IsFollow();
            m_followBtn.GetComponent<StateRoot>().SetCurrentState((isFollow ? 1 : 0), true);
            m_autoFillFlag.SetActive(m_sundryData.isAutoFillTeam || m_sundryData.isAutoJoinTeam);

            if (m_teamAllInfo.members.Count <= 0)
                m_teamTabBtnText.text = "组\n队";
            else
                m_teamTabBtnText.text = "组\n队\n" + m_teamAllInfo.members.Count;
            m_unreadJoinReqFlag.SetActive(this.IsInTeam() &&TeamUtil.teamMgr.hasUnreadJoinReqInfo);
        }

        private void HideTipPanel()
        {
            m_teamTipPanel.SetActive(false);
            this.RemoveTipPanelEventHandle();
        }
    }
}

#endif