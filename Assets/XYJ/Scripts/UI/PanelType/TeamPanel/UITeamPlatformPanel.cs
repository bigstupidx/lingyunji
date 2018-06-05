#if !USE_HOT
using NetProto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UITeamPlatformPanel : HotPanelBase
    {
        [SerializeField]
        ILMonoBehaviour m_IlMonoGoalView;
        TeamPlatformHotGoalScrollView m_goalsView;
        [SerializeField]
        ILMonoBehaviour m_IlMonoInfoView;
        TeamPlatformHotInfoScrollView m_infoView;  
        [SerializeField]
        Button m_refreshBtn;
        [SerializeField]
        Button m_autoJoinBtn;
        [SerializeField]
        Button m_CreateTeam;
        [SerializeField]
        Button m_allTeamsBtn;
        [SerializeField]
        Button m_nearbyTeamsBtn;
        [SerializeField]
        Button m_closeBtn;

        int m_selectedGoalId = 0;
        bool m_needNearbyTeams = false;

        public UITeamPlatformPanel() :base(null) { }
        public UITeamPlatformPanel(xys.UI.UIHotPanel _parent) : base(_parent)
        {

        }

        //初始化
        protected override void OnInit()
        {
            m_infoView = m_IlMonoInfoView.GetObject() as TeamPlatformHotInfoScrollView;
            m_goalsView = m_IlMonoGoalView.GetObject() as TeamPlatformHotGoalScrollView;

            m_refreshBtn.onClick.AddListenerIfNoExist(OnClickRefreshBtn);
            m_autoJoinBtn.onClick.AddListenerIfNoExist(OnClickAutoJoinBtn);
            m_CreateTeam.onClick.AddListenerIfNoExist(OnClickCreateTeamBtn);
            m_allTeamsBtn.onClick.AddListenerIfNoExist(OnClickAllTeamBtn);
            m_nearbyTeamsBtn.onClick.AddListenerIfNoExist(OnClickNearbyTeamBtn);
            m_closeBtn.onClick.AddListenerIfNoExist(OnClickCloseBtn);
        }
        void OnClickCloseBtn()
        {
            App.my.uiSystem.HidePanel("UITeamPlatformPanel");
        }
        void OnClickRefreshBtn()
        {
            m_infoView.ClearItems();
            this.QueryTeams(0);
        }
        void OnClickAutoJoinBtn()
        {
            bool isAutoJoin = !TeamUtil.teamMgr.IsAutoJoinTeam();

            if (0 == m_selectedGoalId && isAutoJoin)
            {
                xys.UI.Utility.TipContentUtil.Show("team_select_detail_goal");
                return;
            }

            App.my.eventSet.FireEvent(EventID.Team_ApplyAutoJoinTeam, new ES_AutoJoinTeam() {
                isAuto = isAutoJoin,
                goalId = m_selectedGoalId
            });
        }
        void OnClickCreateTeamBtn()
        {
            App.my.eventSet.FireEvent(EventID.Team_ReqCreateTeam, 0);
        }
        void OnClickAllTeamBtn()
        {
            if (!m_needNearbyTeams)
                return;

            m_needNearbyTeams = false;
            m_infoView.ClearItems();
            this.QueryTeams(0);
            m_allTeamsBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
            m_nearbyTeamsBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
        }

        void OnClickNearbyTeamBtn()
        {
            if (m_needNearbyTeams)
                return;

            m_needNearbyTeams = true;
            m_infoView.ClearItems();
            this.QueryTeams(0);
            m_allTeamsBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
            m_nearbyTeamsBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
        }

        protected void OnQueryTeamsResult(TeamQueryTeamsResult ret)
        {
            if (TeamQueryTeamsReason.TeamPlatform != ret.queryReason)
                return;

            m_infoView.AddItems(ret);
        }

        protected void UpdatgeAutoJoinTeamBtnState()
        {
            if (null != m_autoJoinBtn)
                m_autoJoinBtn.GetComponent<StateRoot>().SetCurrentState((TeamUtil.teamMgr.IsAutoJoinTeam() ? 1 : 0), true);
        }

        protected void OnTeamDataChange(TeamAllTeamInfo info)
        {
            if (info.teamId > 0 && info.members.ContainsKey(App.my.localPlayer.charid))
            {
                App.my.uiSystem.HidePanel("UITeamPlatformPanel");
                TeamUtil.ShowOrganizePanel();
            }
        }

        protected void OnPlatformQueryTeamInfos(ES_QueryTeamsFilter filter)
        {
            if (TeamQueryTeamsReason.TeamPlatform != filter.queryReason)
                return;

            if (0 == filter.beginTeamId)
                m_infoView.ClearItems();

            m_selectedGoalId = filter.goalId;
            this.QueryTeams(filter.beginTeamId);
        }

        private void QueryTeams(int beginTeamId)
        {
            ES_QueryTeamsFilter filter = new ES_QueryTeamsFilter();
            filter.goalId = m_selectedGoalId;
            filter.needNearby = m_needNearbyTeams;
            filter.queryReason = TeamQueryTeamsReason.TeamPlatform;
            filter.beginTeamId = beginTeamId;
            filter.canJoinLevel = App.my.localPlayer.levelValue;
            App.my.eventSet.FireEvent<ES_QueryTeamsFilter>(EventID.Team_QueryTeamsByFilter, filter);
        }

        //显示
        protected override void OnShow(object args)
        {
            Event.Subscribe<ES_QueryTeamsFilter>(EventID.Team_PlatformQueryTeamInfos, this.OnPlatformQueryTeamInfos);
            Event.Subscribe<TeamQueryTeamsResult>(EventID.Team_QueryTeamsRet, this.OnQueryTeamsResult);
            Event.Subscribe(EventID.Team_SundryDataChange, this.UpdatgeAutoJoinTeamBtnState);
            Event.Subscribe<TeamAllTeamInfo>(EventID.Team_DataChange, this.OnTeamDataChange);

            this.UpdatgeAutoJoinTeamBtnState();
            m_goalsView.Show();
        }
    }
}

#endif