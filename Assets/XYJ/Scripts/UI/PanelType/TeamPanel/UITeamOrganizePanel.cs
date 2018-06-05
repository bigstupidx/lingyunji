#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI;
using xys.UI.Dialog;
using xys.UI.State;
using NetProto.Hot;

namespace xys.hot.UI
{
    class UITeamOrganizePanel : HotPanelBase
    {
        [SerializeField]
        Button m_selectGoalBtn;
        [SerializeField]
        Button m_selectGoalBtns;
        [SerializeField]
        Button m_applyJoinBtn;
        [SerializeField]
        GameObject m_newApplyJoinFlag;
        [SerializeField]
        Button m_chatBtn;
        [SerializeField]
        Button m_inviteBtn;
        [SerializeField]
        Button m_goTaskBtn;
        [SerializeField]
        Button m_leaveBtn;
        [SerializeField]
        Button m_followBtn;
        [SerializeField]
        Button m_autoFillBtn;
        [SerializeField]
        Button m_closeBtn;
        [SerializeField]
        Text m_levelLimit;
        [SerializeField]
        Text m_goalName;
        [SerializeField]
        GameObject m_memberContainer;

        [SerializeField]
        Button m_chatGuildBtn;
        [SerializeField]
        Button m_chatWorldBtn;
        [SerializeField]
        Button m_chatTeamBtn;

        [SerializeField]
        StateRoot m_dialogContainer;
        [SerializeField]
        Transform m_chatDialog;
        [SerializeField]
        Transform m_applyJoinDialog;
        [SerializeField]
        Transform m_inviteDialog;
        [SerializeField]
        Transform m_goalDialog;

        [SerializeField]
        Button m_closeApplyDialogBtn;
        [SerializeField]
        Button m_closeInviteDialogBtn;
        [SerializeField]
        Button m_closeGoalDialogBtn;

        [SerializeField]
        ILMonoBehaviour m_ilMonoApplyJoinDialog;
        TeamOrganizeApplyJoinDialog m_applyJoinDialogLogic;

        [SerializeField]
        ILMonoBehaviour m_ilMonoInviteDialog;
        TeamOrganizeInviteDialog m_inviteDialogLogic;

        [SerializeField]
        ILMonoBehaviour m_ilMonoGoalDialog;
        TeamOrganizeGoalDialog m_goalDialogLogic;

        const int DIALOG_STATE_HIDE = 0;
        const int DIALOG_STATE_APPLY_JOIN = 1;
        const int DIALOG_STATE_INVITE = 2;
        const int DIALOG_STATE_GOAL = 3;
        const int DIALOG_STATE_CHAT = 4;

        const int MEMBER_MAX_COUNT = 5;
        List<TeamOrganizeHotMemberItem> m_memberItems = new List<TeamOrganizeHotMemberItem>();

        TeamAllTeamInfo m_teamAllInfo = null;
        TeamSundryData m_sundryData = null;

        TwoBtn m_twoBtn;


        RTTModelPartHandler[] m_rtts = new RTTModelPartHandler[TeamDef.MAX_MEMBER_COUNT];

        public UITeamOrganizePanel() : base(null) { }
        public UITeamOrganizePanel(xys.UI.UIHotPanel _parent) : base(_parent)
        {

        }
        protected override void OnInit()
        {
            for(int i = 0 ; i < MEMBER_MAX_COUNT ; ++i)
            {
                Transform obj = m_memberContainer.transform.Find(i.ToString());
                TeamOrganizeHotMemberItem item = obj.GetComponent<ILMonoBehaviour>().GetObject() as TeamOrganizeHotMemberItem;
                m_memberItems.Add(item);
            }

            m_closeBtn.onClick.AddListenerIfNoExist(OnClickCloseBtn);
            m_leaveBtn.onClick.AddListenerIfNoExist(OnClickLeaveBtn);
            m_autoFillBtn.onClick.AddListenerIfNoExist(OnClickAutoFillBtn);
            m_followBtn.onClick.AddListenerIfNoExist(OnClickFollowBtn);
            m_chatBtn.onClick.AddListenerIfNoExist(() => { OnClickChatBtn(m_chatBtn); });
            m_chatWorldBtn.onClick.AddListenerIfNoExist(() => { OnClickChatBtn(m_chatWorldBtn); });
            m_chatGuildBtn.onClick.AddListenerIfNoExist(() => { OnClickChatBtn(m_chatGuildBtn); });
            m_chatTeamBtn.onClick.AddListenerIfNoExist(() => { OnClickChatBtn(m_chatTeamBtn); });
            m_closeApplyDialogBtn.onClick.AddListenerIfNoExist(() => { OnClickCloseDialogBtn(m_closeApplyDialogBtn); });
            m_closeInviteDialogBtn.onClick.AddListenerIfNoExist(() => { OnClickCloseDialogBtn(m_closeInviteDialogBtn); });
            m_closeGoalDialogBtn.onClick.AddListenerIfNoExist(() => { OnClickCloseDialogBtn(m_closeGoalDialogBtn); });
            m_applyJoinBtn.onClick.AddListenerIfNoExist(OnClickApplyJoinBtn);
            m_inviteBtn.onClick.AddListenerIfNoExist(OnClickInviteJoinBtn);
            m_selectGoalBtn.onClick.AddListenerIfNoExist(() => { OnClickSelectGoalBtn(m_selectGoalBtn); });
            m_selectGoalBtns.onClick.AddListenerIfNoExist(() => { OnClickSelectGoalBtn(m_selectGoalBtns); });
            m_goTaskBtn.onClick.AddListenerIfNoExist(OnClickGoTaskBtn);

            parent.gameObject.transform.Find("Dialogs/ApplyJoinDialog/Shade").GetComponent<Button>().onClick.AddListenerIfNoExist(() => { OnClickCloseDialogBtn(m_closeApplyDialogBtn); });
            parent.gameObject.transform.Find("Dialogs/InviteDialog/Shade").GetComponent<Button>().onClick.AddListenerIfNoExist(() => { OnClickCloseDialogBtn(m_closeInviteDialogBtn); });
            parent.gameObject.transform.Find("Dialogs/GoalDialog/Shade").GetComponent<Button>().onClick.AddListenerIfNoExist(() => { OnClickCloseDialogBtn(m_closeGoalDialogBtn); });

            m_applyJoinDialogLogic = m_ilMonoApplyJoinDialog.GetObject() as TeamOrganizeApplyJoinDialog;
            m_applyJoinDialogLogic.Init(DoAllowJoinTeam);

            m_inviteDialogLogic = m_ilMonoInviteDialog.GetObject() as TeamOrganizeInviteDialog;
            m_inviteDialogLogic.Init(DoInviteJoin);

            m_goalDialogLogic = m_ilMonoGoalDialog.GetObject() as TeamOrganizeGoalDialog;
            m_goalDialogLogic.Init(OnClickGoalConfirmBtn);
        }

        protected void DoInviteJoin(long uid)
        {
            TeamUtil.teamMgr.InviteJoinTeam(uid);
        }
        protected void DoAllowJoinTeam(TeamJoinReqInfo info)
        {
            TeamUtil.teamMgr.RspReqJoinTeam(info.teamId, info.uid, true);
        }
        protected override void OnShow(object args)
        {
            object[] objs = (object[])args;
            m_teamAllInfo = objs[0] as TeamAllTeamInfo;
            m_sundryData = objs[1] as TeamSundryData;

            for(int i = 0 ; i < m_rtts.Length ; ++i)
            {
                m_rtts[i] = new RTTModelPartHandler("RTTModelPart", m_memberItems[i].m_roleIcon.GetComponent<RectTransform>(), "", true, new Vector3(5 * i, 0, 0));
            }

            Event.Subscribe<ES_JoinReqInfoChange>(EventID.Team_JoinReqInfoChange, OnJoinReqInfoChange);
            Event.Subscribe(EventID.Team_UnreadJoinReqInfoFlagChange, OnUnreadJoinReqFlagChange);
            Event.Subscribe<TeamAllTeamInfo>(EventID.Team_DataChange, OnDataChange);
            Event.Subscribe<TeamSundryData>(EventID.Team_SundryDataChange, OnSundryDataChange);
            Event.Subscribe<long>(EventID.Team_EnterTeam, m_inviteDialogLogic.OnNewMemberEnterTeam);
            Event.Subscribe<QueryNearbyUserRsp>(EventID.Team_RspQueryNearbyUser, m_inviteDialogLogic.OnQueryNearbyUserRsp);
            Event.Subscribe<Dictionary<long, FriendItemInfo>>(EventID.Friend_PushFriendData, m_inviteDialogLogic.OnQueryFriendRsp);


            this.UpdateUI();
            m_newApplyJoinFlag.SetActive(TeamUtil.teamMgr.hasUnreadJoinReqInfo);
        }

        protected override void OnHide()
        {
            for(int i = 0 ; i < m_rtts.Length ; ++i)
            {
                m_rtts[i].Destroy();
                m_rtts[i] = null;
            }
        }
        protected void OnClickCloseBtn()
        {
            App.my.uiSystem.HidePanel("UITeamOrganizePanel");
        }

        protected void OnClickLeaveBtn()
        {
            if(null != m_twoBtn)
                return;

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

        protected void OnClickAutoFillBtn()
        {
            if(0 == m_teamAllInfo.limit.goalId)
                xys.UI.Utility.TipContentUtil.Show("team_select_goal_first");
            else
                TeamUtil.teamMgr.AutoFillTeam(!m_sundryData.isAutoFillTeam);
        }

        protected void OnClickFollowBtn()
        {
            TeamUtil.teamMgr.SetFollow(!TeamUtil.teamMgr.IsFollow());
        }

        protected void OnClickChatBtn(Button btn)
        {
            int dialogState = DIALOG_STATE_HIDE;
            ChannelType channelType = ChannelType.Channel_None;

            if(btn == m_chatBtn)
            {
                dialogState = DIALOG_STATE_CHAT;
                this.AddDialogEventHandle(m_chatDialog);
            }
            if(btn == m_chatGuildBtn)
            {
                channelType = ChannelType.Channel_Family;
                xys.UI.Utility.TipContentUtil.Show("team_send_chat_msg_to_guild");
            }
            if(btn == m_chatWorldBtn)
            {
                channelType = ChannelType.Channel_Global;
                xys.UI.Utility.TipContentUtil.Show("team_send_msg_to_world");
            }
            if(btn == m_chatTeamBtn)
            {
                channelType = ChannelType.Channel_GlobalTeam;
                xys.UI.Utility.TipContentUtil.Show("team_send_chat_msg_to_team");
            }

            m_dialogContainer.SetCurrentState(dialogState, true);
            if(ChannelType.Channel_None != channelType)
            {
                // 组队招募测试
                ChatUtil.SendTeamRecruit(channelType, "team_test", TeamUtil.teamMgr.teamId);
            }
        }

        protected void OnClickCloseDialogBtn(Button btn)
        {
            if(btn == m_closeGoalDialogBtn)
            {

            }
            if(btn == m_closeInviteDialogBtn)
            {

            }
            if(btn == m_closeApplyDialogBtn)
            {

            }

            m_dialogContainer.SetCurrentState(DIALOG_STATE_HIDE, true);
        }

        protected void OnClickGoalConfirmBtn(int goalId, int minLvl, int maxLvl)
        {
            m_dialogContainer.SetCurrentState(DIALOG_STATE_HIDE, true);
            TeamUtil.teamMgr.SetJoinLimit(goalId, minLvl, maxLvl);

            if(0 == goalId)
            {
                TeamUtil.teamMgr.AutoFillTeam(false);
            }
            else if(goalId > 0 && m_teamAllInfo.members.Count < TeamDef.MAX_MEMBER_COUNT)
            {
                TeamUtil.teamMgr.AutoFillTeam(true);
            }
        }

        protected void OnClickRoleIcon(TeamOrganizeHotMemberItem item)
        {
            TeamMemberData data = item.data;
            TeamUtil.ShowRoleOperationPanel(data.uid, data.name, data.sex, data.prof, data.level, Vector3.zero);
        }

        protected void OnClickInviteJoinBtn()
        {
            m_inviteDialogLogic.Show();
            m_dialogContainer.SetCurrentState(DIALOG_STATE_INVITE, true);
        }

        protected void OnClickApplyJoinBtn()
        {
            Event.fireEvent(EventID.Team_ReadedJoinReqInfo);

            List<TeamJoinReqInfo> joinReqInfoList = TeamUtil.teamMgr.GetJoinReqInfos();
            if(joinReqInfoList.Count <= 0)
            {
                xys.UI.Utility.TipContentUtil.Show("team_no_join_request_info");
            }
            else
            {
                m_dialogContainer.SetCurrentState(DIALOG_STATE_APPLY_JOIN, true);
                m_applyJoinDialogLogic.Show(joinReqInfoList);
            }
        }

        protected void OnClickSelectGoalBtn(Button btn)
        {
            if(!TeamUtil.teamMgr.IsLeader())
            {
                if(btn == m_selectGoalBtn)
                    xys.UI.Utility.TipContentUtil.Show("team_only_leader_set_goal");
                return;
            }

            m_dialogContainer.SetCurrentState(DIALOG_STATE_GOAL, true);
            m_goalDialogLogic.Show(m_teamAllInfo.limit.goalId, m_teamAllInfo.limit.minLevel, m_teamAllInfo.limit.maxLevel);
        }

        protected void OnClickGoTaskBtn()
        {
            App.my.uiSystem.HidePanel("UITeamOrganizePanel");
        }

        protected void UpdateUI()
        {
            bool isLeader = TeamUtil.teamMgr.IsLeader();
            parent.gameObject.GetComponent<StateRoot>().SetCurrentState(( isLeader ? 1 : 0 ), true);

            m_levelLimit.text = string.Format("等级：{0}-{1}级", m_teamAllInfo.limit.minLevel, m_teamAllInfo.limit.maxLevel);
            m_goalName.text = string.Format("目标：{0}", TeamUtil.GetGoalName(m_teamAllInfo.goalId));

            List<TeamMemberData> memberDatas = TeamUtil.SortTeamMember(m_teamAllInfo);
            for(int i = 0 ; i < MEMBER_MAX_COUNT ; ++i)
            {
                TeamOrganizeHotMemberItem item = m_memberItems[i];
                GameObject memberObj = item.m_self;

                if(i < memberDatas.Count)
                {
                    item.Set(i, memberDatas[i], this.OnClickRoleIcon, this.OnClickInviteJoinBtn, m_rtts[i]);
                    item.m_self.GetComponent<StateRoot>().SetCurrentState(( 0 == i ? 2 : 1 ), true);
                }
                else
                {
                    item.Set(i, null, this.OnClickRoleIcon, this.OnClickInviteJoinBtn, m_rtts[i]);
                    item.m_self.GetComponent<StateRoot>().SetCurrentState(0, true);
                }
            }

            m_autoFillBtn.GetComponent<StateRoot>().SetCurrentState(( m_sundryData.isAutoFillTeam ? 1 : 0 ), true);
            bool isFollow = TeamUtil.teamMgr.IsFollow();
            m_followBtn.GetComponent<StateRoot>().SetCurrentState(( isFollow ? 1 : 0 ), true);

            bool isShowGoTaskBtn = true;
            isShowGoTaskBtn &= TeamUtil.teamMgr.IsLeader();
            Config.TeamGoal goalCfg = Config.TeamGoal.Get(m_teamAllInfo.goalId);
            if(null != goalCfg)
                isShowGoTaskBtn &= goalCfg.isShowGoTaskBtn;
            m_goTaskBtn.gameObject.SetActive(isShowGoTaskBtn);
        }

        protected void OnDataChange(TeamAllTeamInfo teamAllInfo)
        {
            m_teamAllInfo = teamAllInfo;
            this.UpdateUI();
        }
        protected void OnSundryDataChange(TeamSundryData sundryData)
        {
            m_sundryData = sundryData;
            this.UpdateUI();
        }

        protected void OnUnreadJoinReqFlagChange()
        {
            bool unreadFlag = TeamUtil.teamMgr.hasUnreadJoinReqInfo;
            m_newApplyJoinFlag.SetActive(unreadFlag);

            if(unreadFlag)
            {
                if(DIALOG_STATE_APPLY_JOIN == m_dialogContainer.GetComponent<StateRoot>().CurrentState)
                {
                    Event.fireEvent(EventID.Team_ReadedJoinReqInfo);
                }
            }
        }
        protected void OnJoinReqInfoChange(ES_JoinReqInfoChange data)
        {
            if(DIALOG_STATE_APPLY_JOIN != m_dialogContainer.GetComponent<StateRoot>().CurrentState)
            {
                if(null != data.newInfo)
                {
                    m_newApplyJoinFlag.SetActive(true);
                }
                if(data.allInfo.Count <= 0)
                {
                    m_newApplyJoinFlag.SetActive(false);
                }
                return;
            }

            m_applyJoinDialogLogic.OnDataChange(data.allInfo);
        }

        Transform m_showingDialog;
        int m_dialogClickEventHandId = 0;
        protected void AddDialogEventHandle(Transform dialog)
        {
            this.RemoveDialogEventHandle();
            m_showingDialog = dialog;
            m_dialogClickEventHandId = xys.UI.EventHandler.pointerClickHandler.Add(OnGlobalClick);
        }

        protected void RemoveDialogEventHandle()
        {
            m_showingDialog = null;
            if(0 != m_dialogClickEventHandId)
            {
                xys.UI.EventHandler.pointerClickHandler.Remove(m_dialogClickEventHandId);
                m_dialogClickEventHandId = 0;
            }
        }
        protected bool OnGlobalClick(GameObject go, UnityEngine.EventSystems.BaseEventData bed)
        {
            if(go == null || !m_showingDialog || !go.transform.IsChildOf(m_showingDialog))
            {
                m_dialogContainer.SetCurrentState(DIALOG_STATE_HIDE, true);
                this.RemoveDialogEventHandle();
                return false;
            }

            return true;
        }
    }
}

#endif