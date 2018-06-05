using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public enum GameTaskType
    {
        Main    = 1,// 主线章节任务
        Branch  = 2,// 分支任务
        Guide   = 3,// 指引任务
        Loop    = 4,// 环任务，主要是活动用
    }

    public partial class TaskDefine
    {

        // 额外处理的字段

        // 接任务条件：个性差值

        // 任务完成条件：物品


        // 额外提供的方法
        #region 任务定义额外提供的方法

        /// <summary>
        /// 是否有后置任务
        /// </summary>
        /// <returns></returns>
        public bool HasNextTask()
        {
            if (nextTaskId == 0 && (nextTaskIds==null || nextTaskIds.Length==0))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 是否有多个后置任务
        /// </summary>
        /// <returns></returns>
        public bool HasMultiNextTasks()
        {
            if (nextTaskIds!=null && nextTaskIds.Length>0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 接任务条件：是否等级
        /// </summary>
        /// <returns></returns>
        public bool IsAcceptNeedLevel(int playerLevel)
        {
            if (acceptLevelMin == 0)
                return true;

            if (playerLevel >= acceptLevelMin && playerLevel <= acceptLevelMax)
                return true;

            return false;
        }

        /// <summary>
        /// 接任务条件：是否个性值
        /// </summary>
        /// <returns></returns>
        public bool IsAcceptNeedPersonality()
        {
            if (string.IsNullOrEmpty(acceptPersonal))
                return true;
            else
                return true;
        }

        /// <summary>
        /// 接任务条件：是否门派
        /// </summary>
        /// <returns></returns>
        public bool IsAcceptNeedCareer(int career)
        {
            if (acceptCareer == 0)
                return true;
            else
            {
                if (acceptCareer == career)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 接任务条件：是否需要氏族
        /// </summary>
        /// <returns></returns>
        public bool IsAcceptNeedGuild(bool playerHasGuild)
        {
            if (!acceptHasGuild)
                return true;
            else
            {
                return playerHasGuild;
            }
        }

        /// <summary>
        /// 是否有完成条件
        /// </summary>
        /// <returns></returns>
        public bool HasCompletedCondition()
        {
            return false;
        }

        /// <summary>
        /// 任务是否限时完成
        /// </summary>
        /// <returns></returns>
        public bool IsTimeLimit()
        {
            return false;
        }

        /// <summary>
        /// 获取奖励物品
        /// </summary>
        /// <returns></returns>
        public ItemCount[] GetRewardItemList()
        {
            return RewardDefine.InitItems(rewardItems, ';');
        }

        #endregion

        // 处理一些字段
        public static void OnLoadEndLine(TaskDefine data, CsvCommon.ICsvLoad reader, int i)
        {
            if (data.acceptLevelMax == 0)
                data.acceptLevelMax = int.MaxValue;
        }

        // 管理任务定义表
        #region 任务定义管理

        // 任务组字典，key为任务组id
        static Dictionary<int, TaskGroupDefine> m_groupDataList = new Dictionary<int, TaskGroupDefine>();
        // 任务章节字典,key为章节id
        static Dictionary<int, TaskChapterConfig> m_chapterDataList = new Dictionary<int, TaskChapterConfig>();
        // 环任务字典,key为环任务id
        static Dictionary<int, TaskLoopConfig> m_loopDataList = new Dictionary<int, TaskLoopConfig>();

        /// <summary>
        /// 整理该表格
        /// </summary>
        static void OnLoadEnd ()
        {
            m_groupDataList.Clear();
            m_chapterDataList.Clear();
            m_loopDataList.Clear();
            foreach (var item in DataList)
            {
                TaskDefine taskItem = item.Value;
                int groupId = taskItem.groupId;
                if (!m_groupDataList.ContainsKey (groupId))
                {
                    m_groupDataList.Add(groupId, new TaskGroupDefine());
                }
                m_groupDataList[groupId].m_dataList.Add(taskItem);
            }

            // 按类型分类存放
            foreach (var item in m_groupDataList)
            {
                TaskGroupDefine group = item.Value;
                group.Init();

                if (group.firstTask.taskType == 1)//主线任务
                {
                    if (!m_chapterDataList.ContainsKey(group.firstTask.chapterOrder))
                    {
                        TaskChapterConfig chapter = new TaskChapterConfig();
                        chapter.chapterOrder = group.firstTask.chapterOrder;
                        chapter.chapterName = group.firstTask.chapterName;
                        m_chapterDataList.Add(group.firstTask.chapterOrder, new TaskChapterConfig());
                    }
                    m_chapterDataList[group.firstTask.chapterOrder].m_groupDataList.Add(group);
                }
                else if (group.firstTask.taskType == 4)//环任务
                {

                    if (!m_loopDataList.ContainsKey(group.firstTask.loopId))
                    {
                        TaskLoopConfig loop = new TaskLoopConfig();
                        loop.loopId = group.firstTask.loopId;
                        m_loopDataList.Add(group.firstTask.loopId, loop);
                    }
                    m_loopDataList[group.firstTask.chapterOrder].m_groupDataList.Add(group);
                }
            }
        }

        /// <summary>
        /// 获取后置任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static List<TaskDefine> GetNextTasks(int taskId)
        {
            List<TaskDefine> nextTaskList = new List<TaskDefine>();
            TaskDefine curTask = Get(taskId);
            TaskDefine nextTask;
            if (curTask != null)
            {
                if (curTask.nextTaskId != 0)
                {
                    nextTask = Get(curTask.nextTaskId);
                    if (nextTask != null)
                        nextTaskList.Add(nextTask);
                }
                else if (curTask.nextTaskIds != null && curTask.nextTaskIds.Length > 0)
                {
                    for (int i = 0; i < curTask.nextTaskIds.Length; ++i)
                    {
                        nextTask = Get(curTask.nextTaskIds[i]);
                        if (nextTask != null)
                            nextTaskList.Add(nextTask);
                    }
                }
            }

            return nextTaskList;
        }

        /// <summary>
        /// 获取任务组
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static TaskGroupDefine GetTaskGroup(int groupId)
        {
            TaskGroupDefine group;
            if (m_groupDataList.TryGetValue(groupId, out group))
            {
                return group;
            }
            CsvCommon.Log.Error("TaskDefine.GetTaskGroup({0}) not find!", groupId);
            return null;
        }

        #endregion

    }
}