#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.hot
{
    using Network;
    using NetProto;
    using Team;

    public class TeamMgr 
    {
        public TeamMgr() { }

        public void OnInit()
        {
            request = new C2WTeamRequest(App.my.world.local);
            localPlayer = App.my.localPlayer;

            hotApp.my.handler.Reg<TeamMemberLeave>(Protoid.W2C_TeamMemberLeave, OnMemberLeave);                                // 队员离开队伍
            hotApp.my.handler.Reg<TeamChangeLeader>(Protoid.W2C_TeamChangeLeader, OnChangeLeader);                             // 队长改变
            hotApp.my.handler.Reg<TeamBeKicked>(Protoid.W2C_TeamBeKicked, OnMemberKicked);                                     // 队员被踢出
            hotApp.my.handler.Reg<TeamAllTeamInfo>(Protoid.W2C_TeamSyncTeamInfo, OnSyncTeamAllInfo);                           // 同步队伍信息
            hotApp.my.handler.Reg<TeamDismiss>(Protoid.W2C_TeamDismiss, OnDismiss);                                            // 队伍解散
            hotApp.my.handler.Reg<None>(Protoid.W2C_TeamNotifyInfoChange, OnTeamInfoChange);                                   // 通知队伍信息有改变
            hotApp.my.handler.Reg<TeamInviteCreateInfo>(Protoid.W2C_TeamInviteCreateInfo, OnTeamInviteCreateInfo);             // 被邀创建队伍
            hotApp.my.handler.Reg<TeamInviteCreateResult>(Protoid.W2C_TeamInviteCreateResult, OnTeamInviteCreateResult);       // 邀请创建队伍结果
            // hotApp.my.handler.Reg<TeamJoinReqInfos>(Protoid.W2C_TeamJoinReqInfos, OnTeamJoinReqInfos);                      // 申请入队信息（队长才收到）
            hotApp.my.handler.Reg<TeamJoinReqInfo>(Protoid.W2C_TeamJoinReqInfo, OnTeamJoinReqInfo);                            // 申请入队信息（队长才收到）
            hotApp.my.handler.Reg<TeamRspReqJoinResult>(Protoid.W2C_TeamRspReqJoinResult, OnTeamRspReqJoinResult);             // 申请入队结果
            hotApp.my.handler.Reg<TeamInviteJoinInfo>(Protoid.W2C_TeamInviteJoinInfo, OnTeamInviteJoinInfo);                   // 被邀加入队伍
            hotApp.my.handler.Reg<TeamInviteJoinResult>(Protoid.W2C_TeamInviteJoinResult, OnTeamInviteJoinResult);             // 邀请加入队伍结果（队长才收到）
            hotApp.my.handler.Reg<TeamSundryData>(Protoid.W2C_TeamSundryData, OnSundryDataChange);                             // 队伍杂项信息
            hotApp.my.handler.Reg<TeamNewMember>(Protoid.W2C_TeamNewMember, OnNewMember);                                      // 新队员入队
            hotApp.my.handler.Reg<None>(Protoid.W2C_TeamCallFollow, OnCallFollow);                                             // 召唤跟随

            hotApp.my.eventSet.Subscribe(EventID.FinishLoadScene, this.OnFinishLoadScene);
            hotApp.my.eventSet.Subscribe<ES_QueryTeamsFilter>(EventID.Team_QueryTeamsByFilter, this.OnQueryTeamsByFilter);
            hotApp.my.eventSet.Subscribe<int>(EventID.Team_ApplyJoinTeam, this.OnApplyJoinTeam);
            hotApp.my.eventSet.Subscribe<ES_AutoJoinTeam>(EventID.Team_ApplyAutoJoinTeam, this.OnApplyAutoJoinTeam);
            hotApp.my.eventSet.Subscribe<int>(EventID.Team_ReqCreateTeam, this.CreateTeam);
            App.my.eventSet.Subscribe<ES_QueryNearbyUser>(EventID.Team_ReqQueryNearbyUser, this.OnQueryNearbyUser);
            App.my.eventSet.Subscribe(EventID.Team_ReadedJoinReqInfo, this.OnReadedJoinReq);
        }

        public void OnAwake()
        {

        }

        #region fields
        public LocalPlayer localPlayer { get; private set; }
        public C2WTeamRequest request { get; private set; }

        private long m_zoneId;
        public long zoneId { get { return m_zoneId; } }
        int m_lastTeamId = 0;
        TeamAllTeamInfo m_teamAllInfo = new TeamAllTeamInfo();
        TeamSundryData m_sundryData = new TeamSundryData();
        public int teamId { get { return m_teamAllInfo.teamId; } }
        public long leaderUid { get { return m_teamAllInfo.leaderUid; } }
        public TeamAllTeamInfo TeamAllInfo { get { return m_teamAllInfo; } }
        public TeamSundryData SundryData { get { return m_sundryData; } }
        public bool InTeam() { return m_teamAllInfo.teamId > 0; }
        public bool IsLeader() { return m_teamAllInfo.leaderUid == localPlayer.charid; }
        public int TeamNum() { return m_teamAllInfo.members.Count; }
        public bool IsAutoJoinTeam() { return m_sundryData.isAutoJoinTeam; }
        public TeamAllTeamInfo GetTeamAllInfo()
        {
            return m_teamAllInfo;
        }
        public bool IsFollow()
        {
            TeamMemberData data = null;
            if (m_teamAllInfo.members.TryGetValue(localPlayer.charid, out data))
                return data.isFollow;
            return false;
        }

        TeamQueryTeamsResult queryTeamsRet = new TeamQueryTeamsResult();
        Dictionary<long, TeamInviteCreateInfo> inviteCreateTeamInfos = new Dictionary<long, TeamInviteCreateInfo>();

        private int m_inviteJoinTimerId = 0;
        private Dictionary<int, TeamInviteJoinInfo> m_inviteJoinInfos = new Dictionary<int, TeamInviteJoinInfo>();
        public Dictionary<int, TeamInviteJoinInfo> GetInviteJoinInfos()
        {
            return m_inviteJoinInfos;
        }

        private List<TeamJoinReqInfo> m_joinTeamReqInfos = new List<TeamJoinReqInfo>();
        public List<TeamJoinReqInfo> GetJoinReqInfos()
        {
            this.CheckJoinReqInfo();
            return m_joinTeamReqInfos;
        }

        #endregion

        #region proto handler
        void OnSundryDataChange(TeamSundryData input)
        {
            m_sundryData = input;
            App.my.eventSet.FireEvent<TeamSundryData>(EventID.Team_SundryDataChange, m_sundryData);
        }

        void OnNewMember(TeamNewMember input)
        {
            if (input.uid == App.my.localPlayer.charid)
                GenTeamActionNotification(TeamError.TE_None, ActionId.SelfJoinSucc);
            else
                GenTeamActionNotification(TeamError.TE_None, ActionId.NewMemberJoin, input.name);

            App.my.eventSet.FireEvent<long>(EventID.Team_EnterTeam, input.uid);
        }

        xys.UI.Dialog.TwoBtn m_twoBtn;
        void OnCallFollow(None input)
        {
            if (!this.IsLeader() && !this.IsFollow())
            {
                if (null == m_twoBtn)
                {
                    m_twoBtn = xys.UI.Dialog.TwoBtn.Show(
                        "", "队长召唤跟随，是否跟随？",
                        "取消", () => false,
                        "确定", () =>
                        {
                            SetFollow(true);
                            return false;
                        }, true, true, () => { m_twoBtn = null; }, 15);
                }

            }
        }

        void OnTeamInfoChange(None input)
        {
            this.UpdateTeamInfo();
            this.QuerySundryData();
        }

        void OnDismiss(TeamDismiss input)
        {
            int teamId = input.teamId;
            if (teamId == m_teamAllInfo.teamId)
            {
                m_teamAllInfo = new TeamAllTeamInfo();
                GenTeamActionNotification(TeamError.TE_None, ActionId.BeDismissTeam);
            }
        }

        void OnSyncTeamAllInfo(TeamAllTeamInfo msg)
        {
            m_lastTeamId = msg.teamId;
            m_teamAllInfo = msg;

            App.my.eventSet.FireEvent<TeamAllTeamInfo>(EventID.Team_DataChange, m_teamAllInfo);
        }

        void OnChangeLeader(TeamChangeLeader input)
        {
            long newLeaderUid = input.leaderUid;
            m_teamAllInfo.leaderUid = newLeaderUid;

            TeamMemberData data = null;
            if (m_teamAllInfo.members.TryGetValue(newLeaderUid, out data))
            {
                if (newLeaderUid == App.my.localPlayer.charid)
                    this.GenTeamActionNotification(TeamError.TE_None, ActionId.SelfBecomeNewLeader);
            }
        }

        void OnMemberLeave(TeamMemberLeave input)
        {
            long memberUid = input.uid;
            TeamMemberData data = null;
            m_teamAllInfo.members.TryGetValue(memberUid, out data);
            m_teamAllInfo.members.Remove(memberUid);
            if (memberUid == localPlayer.charid)
                m_teamAllInfo = new TeamAllTeamInfo();
            if (null != data && memberUid != localPlayer.charid)
                this.GenTeamActionNotification(TeamError.TE_None, ActionId.LeaveTeam, data.name);
            this.OnSyncTeamAllInfo(m_teamAllInfo);
        }
        void OnMemberKicked(TeamBeKicked input)
        {
            long memberUid = input.uid;
            TeamMemberData data = null;
            m_teamAllInfo.members.TryGetValue(memberUid, out data);
            m_teamAllInfo.members.Remove(memberUid);
            if (memberUid == localPlayer.charid)
                m_teamAllInfo = new TeamAllTeamInfo();
            if (null != data && !this.IsLeader())
            {
                if (data.uid != App.my.localPlayer.charid)
                    this.GenTeamActionNotification(TeamError.TE_None, ActionId.BeKickMember, data.name);
                else
                    this.GenTeamActionNotification(TeamError.TE_None, ActionId.SelfBeKickNotify, data.name);
            }
            this.OnSyncTeamAllInfo(m_teamAllInfo);
        }
        void OnTeamInviteCreateInfo(TeamInviteCreateInfo input)
        {
            inviteCreateTeamInfos.Add(input.inviteReqId, input);
        }

        void OnTeamInviteCreateResult(TeamInviteCreateResult input)
        {
            XYJLogger.LogDebug("OnTeamInviteCreateResult");
        }

        private bool m_hasUnreadJoinReqInfo = false;
        public bool hasUnreadJoinReqInfo { get { return m_hasUnreadJoinReqInfo; } }
        void OnTeamJoinReqInfo(TeamJoinReqInfo input)
        {
            if (input.teamId != this.teamId)
                return;

            this.CheckJoinReqInfo();
            bool isExist = false;
            for (int i = 0; i < m_joinTeamReqInfos.Count; ++ i)
            {
                TeamJoinReqInfo info = m_joinTeamReqInfos[i];
                if (info.teamId == input.teamId && info.uid == input.uid)
                {
                    m_joinTeamReqInfos[i] = input;
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                m_joinTeamReqInfos.Add(input);
                m_hasUnreadJoinReqInfo = true;
                App.my.eventSet.fireEvent(EventID.Team_UnreadJoinReqInfoFlagChange);
            }
            ES_JoinReqInfoChange evData = new ES_JoinReqInfoChange();
            evData.allInfo = m_joinTeamReqInfos;
            evData.newInfo = isExist ? null : input;
            App.my.eventSet.FireEvent<ES_JoinReqInfoChange>(EventID.Team_JoinReqInfoChange, evData);
        }

        void CheckJoinReqInfo()
        {
            if (!this.IsLeader())
                m_joinTeamReqInfos.Clear();

            long nowSec = App.my.srvTimer.GetTime.GetCurrentTime() / 10000000;
            List<TeamJoinReqInfo> removeItems = new List<TeamJoinReqInfo>();
            for (int i = 0; i < m_joinTeamReqInfos.Count; ++i)
            {
                TeamJoinReqInfo info = m_joinTeamReqInfos[i];
                // if (nowSec >= info.timeoutTS || info.teamId != this.teamId)
                if (info.teamId != this.teamId)
                {
                    removeItems.Add(info);
                }
            }
            foreach (TeamJoinReqInfo item in removeItems)
            {
                m_joinTeamReqInfos.Remove(item);
            }

            if (m_joinTeamReqInfos.Count <= 0)
            {
                m_hasUnreadJoinReqInfo = false;
                App.my.eventSet.fireEvent(EventID.Team_UnreadJoinReqInfoFlagChange);
            }
        }

        void RemoveJoinReqInfo(int teamId, long uid)
        {
            int removeIdx = -1;
            for (int i = 0; i < m_joinTeamReqInfos.Count; ++i)
            {
                TeamJoinReqInfo info = m_joinTeamReqInfos[i];
                if (info.teamId == teamId && info.uid == uid)
                {
                    removeIdx = i;
                    break;
                }
            }

            if (removeIdx >= 0)
            {
                ES_JoinReqInfoChange evData = new ES_JoinReqInfoChange();
                evData.allInfo = m_joinTeamReqInfos;
                m_joinTeamReqInfos.RemoveAt(removeIdx);
                App.my.eventSet.FireEvent<ES_JoinReqInfoChange>(EventID.Team_JoinReqInfoChange, evData);
            }
        }

        void OnTeamRspReqJoinResult(TeamRspReqJoinResult input)
        {
            GenTeamActionNotification(TeamError.TE_None, input.isAccept ? ActionId.JoinTeamReqBeAccepted : ActionId.JoinTeamReqBeRefused, input.leaderName);
        }

        void OnTeamInviteJoinInfo(TeamInviteJoinInfo input)  
        {
            XYJLogger.LogDebug("OnTeamInviteJoinInfo");

            bool isExist = m_inviteJoinInfos.ContainsKey(input.teamId);
            m_inviteJoinInfos[input.teamId] = input;
            if (!isExist)
            {
                App.my.eventSet.FireEvent<TeamInviteJoinInfo>(EventID.Team_NewInviteJoinInfo, input);
            }

            App.my.eventSet.FireEvent<Dictionary<int, TeamInviteJoinInfo>>(EventID.Team_InviteJoinInfoChange, m_inviteJoinInfos);
        }

        void OnTeamInviteJoinResult(TeamInviteJoinResult input)
        {
            XYJLogger.LogDebug("OnTeamInviteJoinResult");

            if (!input.isAccept)
                GenTeamActionNotification(TeamError.TE_None, ActionId.InvitedJoinBeRefused, input.userName);
        }

        #endregion

        #region RPC
        public void CallFollow()
        {
            request.CallFollowYield();
        }

        public void CreateTeam(int goalId)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteCreateTeam(goalId));
        }
        protected IEnumerator ExecuteCreateTeam(int goalId)
        {
            var yyd = request.CreateTeamYield(new NetProto.Int32() { value = goalId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc CreateTeamYield fail");
            }
            else
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.CreateTeam);
            }

            yield break;
        }

        public void DisissTeam()
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteDisissTeam());
        }
        protected IEnumerator ExecuteDisissTeam()
        {
            var yyd = request.DismissTeamYield(new NetProto.Int32() { value = teamId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc DismissTeamYield fail");
            }
            else
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.DismissTeam);

                if (TeamError.TE_None == ret)
                {
                    m_teamAllInfo.teamId = 0;
                    m_teamAllInfo.leaderUid = 0;
                    m_teamAllInfo.members.Clear();
                }
            }
        }

        public void UpdateTeamInfo()
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteUpdateTeamInfo());
        }
        protected IEnumerator ExecuteUpdateTeamInfo()
        {
            var yyd = request.QueryTeamInfoYield(new Int32() { value = 0 });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                OnSyncTeamAllInfo(yyd.result);
            }
        }

        public void QueryTeamsByFilter(int beginTeamId, int goalId, bool needNearby, int userLevel,TeamQueryTeamsReason queryReason)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteQueryTeamsByFilter(beginTeamId, goalId, needNearby, userLevel, queryReason));
        }
        protected IEnumerator ExecuteQueryTeamsByFilter(int beginTeamId, int goalId, bool needNearby, int userLevel, TeamQueryTeamsReason queryReason)
        {
            var yyd = request.QueryTeamsByFilterYield(new TeamQueryTeamsFilter() {
                beginTeamId = beginTeamId,
                goalId = goalId,
                queryReason = queryReason,
                needNearby = needNearby,
                userLevel = userLevel
            });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                queryTeamsRet = yyd.result;
                App.my.eventSet.FireEvent<TeamQueryTeamsResult>(EventID.Team_QueryTeamsRet, queryTeamsRet);
            }
        }

        public void LeaveTeam()
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteLeaveTeam());
        }
        protected IEnumerator ExecuteLeaveTeam()
        {
            var yyd = request.LeaveTeamYield(new Int32() { value = teamId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                if (TeamError.TE_None == yyd.result.value)
                {
                    m_teamAllInfo = new TeamAllTeamInfo();
                    this.OnSyncTeamAllInfo(m_teamAllInfo);
                }
            }
        }

        public void InviteCreateTeam(int goaldId, long inviteeUid)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteInviteCreateTeam(goaldId, inviteeUid));
        }
        protected IEnumerator ExecuteInviteCreateTeam(int goaldId, long inviteeUid)
        {
            var yyd = request.InviteCreateTeamYield(new TeamInviteCreateReq() { inviteeUid = inviteeUid, goalId = goaldId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.InviteCreateTeam);
            }
        }

        public void RspInviteCreateTeam(long inviterUid, long inviteeUid, int goalId, bool isAccept)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteRspInviteCreateTeam(inviterUid, inviteeUid, goalId, isAccept));
        }
        protected IEnumerator ExecuteRspInviteCreateTeam(long inviterUid, long inviteeUid, int goalId, bool isAccept)
        {
            var yyd = request.RspInviteCreateTeamYield(new TeamInviteCreateRsp() { inviterUid=inviterUid, inviteeUid=inviteeUid, goalId=goalId, isAccept = isAccept });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.RspInviteCreateTeam);
            }
        }

        public void ReqJoinTeam(int teamId)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteJoinTeam(teamId));
        }
        protected IEnumerator ExecuteJoinTeam(int teamId)
        {
            var yyd = request.ReqJoinTeamYield(new Int32() { value = teamId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.ReqJoinTeam);
            }
        }

        public void RspReqJoinTeam(int teamId, long aimUid, bool isAccept)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteRspReqJoinTeam(teamId, aimUid, isAccept));
            RemoveJoinReqInfo(teamId, aimUid);
        }
        protected IEnumerator ExecuteRspReqJoinTeam(int teamId, long aimUid, bool isAccept)
        {
            var yyd = request.RspReqJoinTeamYield(new TeamRspReqJoin() { teamId = teamId, aimUid = aimUid, isAccept = isAccept });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.RspReqJoinTeam);
            }
        }

        public void KickMember(long aimUid)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteKickMember(aimUid));
        }
        protected IEnumerator ExecuteKickMember(long aimUid)
        {
            TeamMemberData memberData = null;
            string aimUserName = "";
            if (!m_teamAllInfo.members.TryGetValue(aimUid, out memberData))
                yield break;

            aimUserName = memberData.name;
            var yyd = request.KickMemberYield(new Int64() { value = aimUid });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.KickMember, aimUserName);
            }
        }

        public void QueryTeamInfo(int teamId)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteQueryTeamInfo(teamId));
        }
        protected IEnumerator ExecuteQueryTeamInfo(int teamId)
        {
            var yyd = request.QueryTeamInfoYield(new Int32() { value = teamId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                this.OnSyncTeamAllInfo(yyd.result);
            }
        }

        public void SetTeamLeader(long aimUid)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteSetTeamLeader(aimUid));
        }
        protected IEnumerator ExecuteSetTeamLeader(long aimUid)
        {
            var yyd = request.SetTeamLeaderYield(new Int64() { value = aimUid });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.SetTeamLeader);
            }
        }

        public void InviteJoinTeam(long aimUid)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteInviteJoinTeam(aimUid));
        }
        protected IEnumerator ExecuteInviteJoinTeam(long aimUid)
        {
            var yyd = request.InviteJoinTeamYield(new Int64() { value = aimUid });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.InviteJoinTeam);
            }
        }

        public void RspInviteJoinTeam(int teamId, bool isAccept)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteRspInviteJoinTeam(teamId, isAccept));
            m_inviteJoinInfos.Remove(teamId);
            App.my.eventSet.FireEvent<Dictionary<int, TeamInviteJoinInfo>>(EventID.Team_InviteJoinInfoChange, m_inviteJoinInfos);
        }
        protected IEnumerator ExecuteRspInviteJoinTeam(int teamId, bool isAccept)
        {
            var yyd = request.RspInviteJoinTeamYield(new TeamRspInviteJoin() { teamId = teamId, isAccept = isAccept });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;

                GenTeamActionNotification(ret, ActionId.RspInviteJoinTeam);
                if (isAccept)
                    xys.App.my.uiSystem.HidePanel("UITeamRspInvitePanel");
            }
        }

        public void AutoJoinTeam(bool isAuto, int goalId)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteAutoJoinTeam(isAuto, goalId));
        }
        protected IEnumerator ExecuteAutoJoinTeam(bool isAuto, int goalId)
        {
            var yyd = request.AutoJoinTeamYield(new TeamAutoJoin() { isAuto = isAuto, goalId = goalId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
                GenTeamActionNotification(ret, ActionId.AutoJoinTeam);

                if (TeamError.TE_None == ret)
                {
                    m_sundryData.isAutoJoinTeam = isAuto;
                    App.my.eventSet.FireEvent<TeamSundryData>(EventID.Team_SundryDataChange, m_sundryData);
                    GenTeamActionNotification(TeamError.TE_None, (isAuto ? ActionId.EnterAutoJoinTeamState : ActionId.LeaveAutoJoinTeamState));
                }
            }
        }

        public void AutoFillTeam(bool isAuto)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteAutoFillTeam(isAuto));
        }
        protected IEnumerator ExecuteAutoFillTeam(bool isAuto)
        {
            var yyd = request.AutoFillTeamYield(new Bool() { value = isAuto });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
                if ((TeamError.TE_None != ret || isAuto))
                    GenTeamActionNotification(ret, ActionId.AutoFillTeam);
            }
        }
        
        public void QuerySundryData()
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteQuerySundryData());
        }
        protected IEnumerator ExecuteQuerySundryData()
        {
            var yyd = request.QuerySundryDataYield();
            yield return yyd;
        }
        public void SetFollow(bool isFollow)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteSetFollow(isFollow));
        }
        protected IEnumerator ExecuteSetFollow(bool isFollow)
        {
            var yyd = request.SetFollowYield(new Bool() { value = isFollow });
            yield return yyd;
        }

        public void QueryNearByUser(QueryNearbyUserReason queryReason, List<long> excludeUids)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteQueryNearByUser(queryReason, excludeUids));
        }
        protected IEnumerator ExecuteQueryNearByUser(QueryNearbyUserReason queryReason, List<long> excludeUids)
        {
            QueryNearbyUserReq req = new QueryNearbyUserReq();
            req.queryReason = queryReason;
            if (null != req.excludeUids)
                req.excludeUids = excludeUids;
            else
                req.excludeUids = new List<long>();
            var yyd = request.QueryNearbyUserYield(req);
            yield return yyd;

            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                QueryNearbyUserRsp ret = yyd.result;
                App.my.eventSet.FireEvent<QueryNearbyUserRsp>(EventID.Team_RspQueryNearbyUser, ret);
            }
        }

        public void SetJoinLimit(int goalId, int minLvl, int maxLvl)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteSetJoinLimit(goalId, minLvl, maxLvl));
        }
        protected IEnumerator ExecuteSetJoinLimit(int goalId, int minLvl, int maxLvl)
        {
            TeamJoinLimit limit = new TeamJoinLimit();
            limit.goalId = goalId;
            limit.minLevel = minLvl;
            limit.maxLevel = maxLvl;
            var yyd = request.SetLimitYield(limit);
            yield return yyd;

            if (wProtobuf.RPC.Error.Success == yyd.code)
            {
                TeamError ret = yyd.result.value;
            }
        }

        #endregion

        protected void OnReadedJoinReq()
        {
            m_hasUnreadJoinReqInfo = false;
            App.my.eventSet.fireEvent(EventID.Team_UnreadJoinReqInfoFlagChange);
        }
        protected void OnQueryNearbyUser(ES_QueryNearbyUser evData)
        {
            if (null == evData.excludeUids)
                evData.excludeUids = new List<long>();

            if (!this.InTeam())
            {
                evData.excludeUids.Add(localPlayer.charid);
            }
            else
            {
                foreach (long uid in m_teamAllInfo.members.Keys)
                {
                    evData.excludeUids.Add(uid);
                }
            }

            this.QueryNearByUser(evData.queryReason, evData.excludeUids);
        }

        protected void OnQueryTeamsByFilter(ES_QueryTeamsFilter filter)
        {
            this.QueryTeamsByFilter(filter.beginTeamId, filter.goalId, filter.needNearby, filter.canJoinLevel, filter.queryReason);
        }

        protected void OnApplyJoinTeam(int teamId)
        {
            this.ReqJoinTeam(teamId);
        }

        protected void OnApplyAutoJoinTeam(ES_AutoJoinTeam item)
        {
            this.AutoJoinTeam(item.isAuto, item.goalId);
        }

        enum ActionId
        {
            CreateTeam = 1,         // 创建队伍
            DismissTeam,            // 解散队伍
            InviteCreateTeam,       // 邀请组队
            RspInviteCreateTeam,    // 响应邀请组队
            ReqJoinTeam,            // 请求入队
            RspReqJoinTeam,         // 队长审批入队请求
            BeKickMember,           // 队员被踢出队伍
            SetTeamLeader,          // 队长请求设置新队长
            InviteJoinTeam,         // 角色请求加入队伍
            AutoJoinTeam,           // 角色请求自动加入队伍
            LeaveTeam,              // 离开队伍
            JoinTeamReqBeAccepted,  // 入队申请被接受
            JoinTeamReqBeRefused,   // 入队申请被拒绝
            BeDismissTeam,          // 队员所在队伍被解散
            NewMemberJoin,          // 新成员入队
            AutoFillTeam,           // 队长请求自动填充队伍
            RspInviteJoinTeam,      // 被邀请者接受入队邀请
            KickMember,             // 队长踢出队员
            SelfJoinSucc,           // 自己成功加入队伍
            InvitedJoinBeRefused,   // 邀请入队被拒绝
            SelfBecomeNewLeader,    // 你成为了新队长
            EnterAutoJoinTeamState, // 进入自动组队模式
            LeaveAutoJoinTeamState, // 离开自动组队模式
            SelfBeKickNotify,       // 通知自己被踢
        }

        void GenTeamActionNotification(TeamError err, ActionId actionId, params object[] objs)
        {
            GenTeamActionNotification((int)err, (int)actionId, objs);
        }

        static Dictionary<long, Config.TeamActionNotification> ActionTeamError2NotificationMap = null;
        static long GenActionNotificationId(int actionId, int teamError)
        {
            long ret = teamError + ((long)actionId << 32);
            return ret;
        }
        void GenTeamActionNotification(int err, int actionId, params object[] objs)
        {
            if (null == ActionTeamError2NotificationMap)
            {
                ActionTeamError2NotificationMap = new Dictionary<long, Config.TeamActionNotification>();
                foreach (Config.TeamActionNotification cfgItem in Config.TeamActionNotification.GetAll())
                {
                    long notificationId = GenActionNotificationId(cfgItem.ActionId, cfgItem.teamError);
                    ActionTeamError2NotificationMap[notificationId] = cfgItem;
                }
            }

            long key = GenActionNotificationId(actionId, err);
            Config.TeamActionNotification cfg = null;
            if (ActionTeamError2NotificationMap.TryGetValue(key, out cfg))
            {
                if (cfg.isOpen && !string.IsNullOrEmpty(cfg.tipKey))
                {
                    xys.UI.Utility.TipContentUtil.Show(cfg.tipKey, objs);
                   // string retMsg = string.Format(cfg.msg, objs);
                   // UI.SystemHintMgr.ShowHint(retMsg);
                }
            }
        }

        private void OnFinishLoadScene()
        {
            m_zoneId = App.my.localPlayer.GetModule<LevelModule>().zoneId;
            this.QueryTeamInfo(0);
            this.QuerySundryData();
        }
    }
}

#endif