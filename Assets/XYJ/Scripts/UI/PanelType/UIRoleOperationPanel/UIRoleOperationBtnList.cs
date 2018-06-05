#if !USE_HOT
using NetProto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.hot.Team;
using xys.UI;
using NetProto.Hot;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIRoleOperationBtnList
    {
        [SerializeField]
        Transform m_BtnList;

        private long m_PlayerId = 0;    //当前信息面板玩家ID

        private bool m_IsFriend = false;
        private bool m_IsBlack = false;
        private bool m_IsEnemy = false;
        private int m_PlayerTeamId = 0;
        private bool m_IsHasLeaderRoot = false;
        public int m_ActiveItemCount { get; private set; }
        List<UIRoleOperationBtnHandle> m_HandleList;

        UIRoleOperationPanel m_Panel;
        public UIRoleOperationPanel panel { set { m_Panel = value; } }

        void Awake()
        {
            m_ActiveItemCount = 0;
        }

        private void OnDisable()
        {
            m_PlayerId = 0;
            m_IsFriend = false;
            m_IsBlack = false;
            m_IsEnemy = false;
            m_PlayerTeamId = 0;
            m_IsHasLeaderRoot = false;
        }

        public void SetBtnShowList(List<UIRoleOperationBtnHandle> handleList, RoleOperShowType showType, long playerID)
        {
            m_PlayerId = playerID;
            RefreshPlayerData();

            switch (showType)
            {
                case RoleOperShowType.Custom:
                case RoleOperShowType.Rank:
                    handleList = GetCustomBtn(new List<UIRoleOperationBtnHandle>());
                    break;
                case RoleOperShowType.FriendEnemyPanel:
                    handleList = GetFriendEnemyPanelBtn(new List<UIRoleOperationBtnHandle>());
                    break;
                case RoleOperShowType.RecentlyListPanel:
                    handleList = GetRecentlyListPanelBtn(new List<UIRoleOperationBtnHandle>());
                    break;
            }

            m_HandleList = handleList;

            for (int i = 0; i < m_HandleList.Count; i++)
                m_HandleList[i].SetBtn(m_BtnList, playerID);

            ResetShowList();
            RefreshShowList();
        }

        void RefreshShowList()
        {
            m_ActiveItemCount = 0;

            for (int i = 0; i < m_HandleList.Count; i++)
            {
                m_HandleList[i].RefreshShowCondition();
                if (m_HandleList[i].gameObject.activeSelf)
                    m_ActiveItemCount++;
            }
        }

        void RefreshPlayerData()
        {
            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
            if (friendModule != null && friendModule.friendMgr != null)
            {
                m_IsFriend = ((FriendMgr)friendModule.friendMgr).IsFriend(m_PlayerId);
                m_IsBlack = ((FriendMgr)friendModule.friendMgr).IsBlack(m_PlayerId);
                m_IsEnemy = ((FriendMgr)friendModule.friendMgr).IsEnemy(m_PlayerId);
            }

            C2WTeamRequest request = new C2WTeamRequest(App.my.world.local);
            request.QueryUserTeamInfo(new NetProto.Int64() { value = m_PlayerId }, (wProtobuf.RPC.Error code, TeamUserTeamInfo ret) =>
            {
                if (wProtobuf.RPC.Error.Success != code)
                    return;

                if (!m_BtnList.gameObject.activeSelf)
                    return;

                if (m_PlayerId == ret.uid)
                    m_PlayerTeamId = ret.teamid;

                m_IsHasLeaderRoot =TeamUtil.teamMgr.IsLeader() && TeamUtil.teamMgr.teamId == m_PlayerTeamId && m_PlayerTeamId != 0;
                m_Panel.RefreshTeamInfo(ret.teamid == 0 ? "0/0" : ret.memberCount + "/" + TeamDef.MAX_MEMBER_COUNT);

                TeamApplyJoinRefresh();
                TeamInviteJoinRefresh();
                TeamTransferLeaderRefresh();
                TeamForceLeaveRefresh();

                ResetShowList();
                RefreshShowList();
                m_Panel.SetBGHeight();
            });

        }

        void ResetShowList()
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform item in m_BtnList)
            {
                children.Add(item);
            }

            for (int i = 0; i < children.Count; i++)
            {
                children[i].gameObject.SetActive(false);
            }
        }



        #region 预设按钮列表类型
        public List<UIRoleOperationBtnHandle> GetCustomBtn(List<UIRoleOperationBtnHandle> handleList)
        {
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.Chat, Chat, ChatRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.CheckInfo, CheckInfo, CheckInfoRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.FriendAdd, FriendAdd, FriendAddRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.FriendDelete, FriendDelete, FriendDeleteRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.TeamApplyJoin, TeamApplyJoin, TeamApplyJoinRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.TeamInviteJoin, TeamInviteJoin, TeamInviteJoinRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.TeamTransferLeader, TeamTransferLeader, TeamTransferLeaderRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.TeamForceLeave, TeamForceLeave, TeamForceLeaveRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.TeamLeave, TeamLeave, TeamLeaveRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.ClanInviteJoin, ClanInviteJoin, ClanInviteJoinRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.ClanApplyJoin, ClanApplyJoin, ClanApplyJoinRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.ClanLeaderTransfer, ClanApplyJoin, ClanApplyJoinRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.ClanLeaderImpeach, ClanApplyJoin, ClanApplyJoinRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.ClanKick, ClanKick, ClanKickRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.BlacklistDelete, BlacklistDelete, BlacklistDeleteRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.BlacklistAdd, BlacklistAdd, BlacklistAddRefresh));
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.Report, Report, ReportRefresh));

            return handleList;
        }

        public List<UIRoleOperationBtnHandle> GetFriendEnemyPanelBtn(List<UIRoleOperationBtnHandle> handleList)
        {
            handleList = GetCustomBtn(handleList);
            handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.EnemyDelete, EnemyDelete, EnemyDeleteRefresh));

            return handleList;
        }

        public List<UIRoleOperationBtnHandle> GetRecentlyListPanelBtn(List<UIRoleOperationBtnHandle> handleList)
        {
            handleList = GetCustomBtn(handleList);
            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
            if (friendModule != null && friendModule.friendMgr != null)
            {
                if (((FriendMgr)friendModule.friendMgr).m_RecentlyType == 1)
                {
                    handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.RecentlyListRemove, RecentlyChatListRemove, RecentlyListRemoveRefresh));
                }
                else
                {
                    handleList.Add(new UIRoleOperationBtnHandle(RoleOperBtnType.RecentlyListRemove, RecentlyTeamListRemove, RecentlyListRemoveRefresh));
                }
            }

            return handleList;
        }

        //通用显示条件
        bool CommonShowCondition()
        {
            //点击自己时不出现
            //return !IsLocalPlayer();
            return true;
        }

        bool IsLocalPlayer()
        {
            return App.my.localPlayer.charid == m_PlayerId;
        }

        //通用点击条件
        void CommonClick()
        {
            SystemHintMgr.ShowHint("功能未开放，敬请期待");
        }

        //聊天
        bool Chat()
        {
            if (m_IsBlack)
            {
                SystemHintMgr.ShowHint("您在该玩家的黑名单列表中，无法发送信息");
                return true;
            }
            CommonClick();
            return false;
        }
        bool ChatRefresh()
        {
            return CommonShowCondition();
        }

        //查看信息
        bool CheckInfo()
        {
            CommonClick();
            return false;
        }
        bool CheckInfoRefresh()
        {
            return CommonShowCondition();
        }

        //邀请入队	
        bool TeamInviteJoin()
        {
           TeamUtil.teamMgr.InviteJoinTeam(m_PlayerId);
            return false;
        }
        bool TeamInviteJoinRefresh()
        {
            return CommonShowCondition() &&TeamUtil.teamMgr.teamId != 0 && m_PlayerTeamId == 0;
        }

        //申请入队
        bool TeamApplyJoin()
        {
            App.my.eventSet.FireEvent<int>(EventID.Team_ApplyJoinTeam, m_PlayerTeamId);
            return false;
        }
        bool TeamApplyJoinRefresh()
        {
            return CommonShowCondition() &&TeamUtil.teamMgr.teamId == 0 && m_PlayerTeamId != 0;
        }

        //移交队长
        bool TeamTransferLeader()
        {
           TeamUtil.teamMgr.SetTeamLeader(m_PlayerId);
            return false;
        }
        bool TeamTransferLeaderRefresh()
        {
            return CommonShowCondition() && m_IsHasLeaderRoot;
        }

        //请离队伍
        bool TeamForceLeave()
        {
           TeamUtil.teamMgr.KickMember(m_PlayerId);
            return false;
        }
        bool TeamForceLeaveRefresh()
        {
            return CommonShowCondition() && m_IsHasLeaderRoot;
        }

        //离开队伍
        bool TeamLeave()
        {
           TeamUtil.teamMgr.LeaveTeam();
            return false;
        }
        bool TeamLeaveRefresh()
        {
            return IsLocalPlayer() &&TeamUtil.teamMgr.teamId != 0;
        }

        //添加好友
        bool FriendAdd()
        {
            //CommonClick();
            App.my.eventSet.FireEvent<long>(EventID.Friend_Apply, m_PlayerId);
            return false;
        }
        bool FriendAddRefresh()
        {
            return CommonShowCondition() && !m_IsFriend;
        }

        //删除好友
        bool FriendDelete()
        {

            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
            if (friendModule != null && friendModule.friendMgr != null)
            {
                ((FriendMgr)friendModule.friendMgr).DeleteRecondFromData(m_PlayerId, FriendListType.FD_Friend);
            }

            return false;
        }
        bool FriendDeleteRefresh()
        {
            return CommonShowCondition() && m_IsFriend;
        }

        //加入黑名单
        bool BlacklistAdd()
        {
            App.my.eventSet.FireEvent<long>(EventID.Friend_BlakSomeOne, m_PlayerId);
            return false;
        }
        bool BlacklistAddRefresh()
        {
            return CommonShowCondition() && !m_IsBlack;
        }

        //从黑名单中删除
        bool BlacklistDelete()
        {
            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
            if (friendModule != null && friendModule.friendMgr != null)
            {
                ((FriendMgr)friendModule.friendMgr).DeleteRecondFromData(m_PlayerId, FriendListType.FD_Black);
            }
            return false;
        }
        bool BlacklistDeleteRefresh()
        {
            return CommonShowCondition() && m_IsBlack;
        }

        //举报
        bool Report()
        {
            CommonClick();
            return false;
        }
        bool ReportRefresh()
        {
            return CommonShowCondition();
        }

        //邀请入族
        bool ClanInviteJoin()
        {
            CommonClick();
            return false;
        }
        bool ClanInviteJoinRefresh()
        {
            return CommonShowCondition();
        }

        //请离氏族
        bool ClanKick()
        {
            CommonClick();
            return false;
        }
        bool ClanKickRefresh()
        {
            return CommonShowCondition();
        }

        //氏族申请加入
        bool ClanApplyJoin()
        {
            CommonClick();
            return false;
        }
        bool ClanApplyJoinRefresh()
        {
            return CommonShowCondition();
        }

        //氏族族长禅让
        bool ClanLeaderTransfer()
        {
            CommonClick();
            return false;
        }
        bool ClanLeaderTransferRefresh()
        {
            return CommonShowCondition();
        }

        //氏族弹劾族长
        bool ClanLeaderImpeach()
        {
            CommonClick();
            return false;
        }
        bool ClanLeaderImpeachRefresh()
        {
            return CommonShowCondition();
        }

        //删除仇敌
        bool EnemyDelete()
        {
            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
            if (friendModule != null && friendModule.friendMgr != null)
            {
                ((FriendMgr)friendModule.friendMgr).DeleteRecondFromData(m_PlayerId, FriendListType.FD_Enemy);
            }
            return false;
        }
        bool EnemyDeleteRefresh()
        {
            return CommonShowCondition() && m_IsEnemy;
        }

        //最近聊天-移除列表
        bool RecentlyChatListRemove()
        {
            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
            if (friendModule != null && friendModule.friendMgr != null)
            {
                ((FriendMgr)friendModule.friendMgr).DeleteRecondFromData(m_PlayerId, FriendListType.FD_Chat);
            }
            return false;
        }

        //最近组队-移除列表
        bool RecentlyTeamListRemove()
        {
            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
            if (friendModule != null && friendModule.friendMgr != null)
            {
                ((FriendMgr)friendModule.friendMgr).DeleteRecondFromData(m_PlayerId, FriendListType.FD_Team);
            }
            return false;
        }

        bool RecentlyListRemoveRefresh()
        {
            return CommonShowCondition();
        }
        #endregion

    }
}
#endif