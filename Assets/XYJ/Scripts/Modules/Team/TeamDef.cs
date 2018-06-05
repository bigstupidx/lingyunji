#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using xys;
using xys.UI;

namespace xys.hot.Team
{
    public class ES_QueryTeamsFilter
    {
        public TeamQueryTeamsReason queryReason;
        public int beginTeamId;
        public int goalId;
        public int canJoinLevel;
        public bool needNearby;
    }

    public class ES_AutoJoinTeam
    {
        public bool isAuto;
        public int goalId;
    }

    public class ES_JoinReqInfoChange
    {
        public List<TeamJoinReqInfo> allInfo;
        public TeamJoinReqInfo newInfo;
    }

    public class ES_QueryNearbyUser
    {
        public QueryNearbyUserReason queryReason;
        public List<long> excludeUids;
    }

    public static class TeamDef
    {
        public const int MAX_MEMBER_COUNT = 5;
        public const int MIN_GOAL_LIMIT_LEVEL = 1;
        public const int MAX_GOAL_LIMIT_LEVEL = 100;
    }
    public class TeamProfSexResConfig
    {
        public int prof;
        // 性别(男0，女1）
        public int sex;
        // 职业小图标
        public string profIcon;
        // 中头像图标
        public string headIcon;
    }

    public static class TeamUtil
    {
        public static TeamMgr teamMgr
        {
            get
            {
                return App.my.localPlayer.GetModule<TeamModule>().teamMgr as TeamMgr;
            }
        }

        public static List<TeamMemberData> SortTeamMember(TeamAllTeamInfo info)
        {
            List<TeamMemberData> memberDatas = new List<TeamMemberData>();
            if (null != info)
            {
                foreach (var kvPair in info.members)
                {
                    if (kvPair.Key != info.leaderUid)
                    {
                        memberDatas.Add(kvPair.Value);
                    }
                }
                Comparison<TeamMemberData> comparison = new Comparison<TeamMemberData>(
                    (TeamMemberData x, TeamMemberData y)=> {
                        if (x.joinTimestamp > y.joinTimestamp)
                            return 1;
                        else if (x.joinTimestamp == y.joinTimestamp)
                            return 0;
                        return -1;
                    });
                memberDatas.Sort(comparison);

                TeamMemberData data = null;
                if (info.members.TryGetValue(info.leaderUid, out data))
                {
                    memberDatas.Insert(0, data);
                }
            }

            return memberDatas;
        }

        public static void ShowOrganizePanel()
        {
            object[] objs = new object[2];
            objs[0] =TeamUtil.teamMgr.TeamAllInfo;
            objs[1] =TeamUtil.teamMgr.SundryData;
            xys.App.my.uiSystem.ShowPanel("UITeamOrganizePanel", objs, true);
        }

        public static string GetGoalName(int goaldId)
        {
            Config.TeamGoal goal = Config.TeamGoal.Get(goaldId);
            if (null != goal)
                return goal.name;
            return String.Empty;
        }

        public static TeamProfSexResConfig GetProfSexResCfg(int prof, int sex)
        {
            TeamProfSexResConfig ret = new TeamProfSexResConfig();
            Config.RoleJob roleJobCfg = Config.RoleJob.Get(prof);
            if (null != roleJobCfg)
            {
                const int FEMALE = 0;

                ret.prof = prof;
                ret.profIcon = roleJobCfg.colorIcon;
                ret.sex = sex;
                ret.headIcon = sex == FEMALE ? roleJobCfg.femalIcon : roleJobCfg.maleIcon;
            }

            return ret;
        }

        public static void ShowRoleOperationPanel(long playerId, string name, int sex, int job, int level, Vector3 panelPos)
        {
            UIRoleOperationData obj = new UIRoleOperationData(App.my.localPlayer.charid, RoleOperShowType.Custom);
            obj.panelPos = panelPos;                              
            obj.playerId = playerId;
            obj.jobSex = sex;
            obj.job = job;
            obj.level = level;
            obj.name = name;
            App.my.uiSystem.ShowPanel(PanelType.UIRoleOperationPanel, obj, true);
        }
    }

}

#endif