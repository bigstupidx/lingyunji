using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config
{

    public partial class TeamGoalType
    {
        public List<TeamGoalGroup> goalGroups = new List<TeamGoalGroup>();
        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(TeamGoalGroup.ManageAfterAllFinish);
            CsvLoadAdapter.AddCallAfterAllLoad(TeamGoalType.ManageAfterAllFinish);
        }

        static void ManageAfterAllFinish()
        {
            HashSet<int> emptyIds = new HashSet<int>();
            foreach (int goalTypeId in DataList.Keys)
            {
                TeamGoalType goalType = DataList[goalTypeId];
                foreach (TeamGoalGroup goalGroup in TeamGoalGroup.GetAll().Values)
                {
                    if (goalGroup.goalTypeId == goalTypeId)
                    {
                        if (null != goalGroup.belongType)
                        {
                            Log.Error(string.Format("TeamGoalType: GoalGroup id %d is repeated used", goalGroup.id));
                        }
                        else
                        {
                            goalGroup.belongType = DataList[goalTypeId];
                            goalType.goalGroups.Add(goalGroup);
                        }
                    }
                }
                if (goalType.goalGroups.Count <= 0)
                    emptyIds.Add(goalType.id);
            }

            foreach (int id in emptyIds)
            {
                DataList.Remove(id);
            }
        }
    }

    public enum TeamGoalDifficultyLevel
    {
        Common = 0,
        Elit,

        Count,
    }

    public partial class TeamGoal
    {
        public TeamGoalDifficultyLevel difficultyLevel = TeamGoalDifficultyLevel.Count;
        public TeamGoalGroup belongGroup;
    }

    public partial class TeamGoalGroup
    {
        public List<TeamGoal> goals = new List<TeamGoal>();
        public TeamGoalType belongType;

        public TeamGoal GetGoal(TeamGoalDifficultyLevel lvl)
        {
            foreach (TeamGoal goal in goals)
            {
                if (goal.difficultyLevel == lvl)
                    return goal;
            }
            return null;
        }
        public static void ManageAfterAllFinish()
        {
            List<int> emptyGroupIds = new List<int>();

            foreach (int groupId in DataList.Keys)
            {
                TeamGoalGroup groupCfg = DataList[groupId];
                Config.TeamGoal goal = null;

                if (1 == groupCfg.id) // 1 这个组id特殊处理
                {
                    goal = Config.TeamGoal.Get(0);
                    if (null == goal)
                    {
                        Log.Error("TeamGoalGroup: missing Goal id 0");
                    }
                    else
                    {
                        groupCfg.goals.Add(goal);
                        goal.belongGroup = groupCfg;
                        goal.difficultyLevel = TeamGoalDifficultyLevel.Common;
                    }
                    
                    continue;
                }

                if (groupCfg.commenGoalId > 0)
                {
                    goal = Config.TeamGoal.Get(groupCfg.commenGoalId);
                    if (null == goal)
                    {
                        Log.Error(string.Format("TeamGoalGroup: missing Goal id %d", groupCfg.commenGoalId));
                    }
                    else
                    {
                        if (null != goal.belongGroup)
                        {
                            Log.Error(string.Format("TeamGoalGroup: Goal id %d is repeated used", groupCfg.commenGoalId));
                            groupCfg.commenGoalId = 0;
                        }
                        else
                        {
                            goal.belongGroup = groupCfg;
                            goal.difficultyLevel = TeamGoalDifficultyLevel.Common;
                            groupCfg.goals.Add(goal);
                        }
                    }
                }
                if (groupCfg.elitGoalId > 0)
                {
                    goal = Config.TeamGoal.Get(groupCfg.elitGoalId);
                    if (null == goal)
                    {
                        Log.Error(string.Format("TeamGoalGroup: missing Goal id %d", groupCfg.elitGoalId));
                    }
                    else
                    {
                        if (null != goal.belongGroup)
                        {
                            Log.Error(string.Format("TeamGoalGroup: Goal id %d is repeated used", groupCfg.elitGoalId));
                            groupCfg.elitGoalId = 0;
                        }
                        else
                        {
                            goal.belongGroup = groupCfg;
                            goal.difficultyLevel = TeamGoalDifficultyLevel.Elit;
                            groupCfg.goals.Add(goal);
                        }
                    }
                }

                if (groupCfg.goals.Count <= 0)
                    emptyGroupIds.Add(groupCfg.id);
            }

            Dictionary<int, TeamGoalGroup> allGroupCfgs = Config.TeamGoalGroup.GetAll();
            foreach (int groupId in emptyGroupIds)
            {
                allGroupCfgs.Remove(groupId);
            }
        }
    }
}
