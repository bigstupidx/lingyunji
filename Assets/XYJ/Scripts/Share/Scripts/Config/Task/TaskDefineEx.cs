using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{

    /// <summary>
    /// 环任务
    /// </summary>
    public class TaskLoopConfig
    {
        public int loopId { get; set; }

        // 环任务的所有任务
        public List<TaskGroupDefine> m_groupDataList = new List<TaskGroupDefine>();
    }

    /// <summary>
    /// 章节任务，主任务
    /// </summary>
    public class TaskChapterConfig
    {
        public int chapterOrder { get; set; }
        public string chapterName { get; set; }

        public List<TaskGroupDefine> m_groupDataList = new List<TaskGroupDefine>();
    }

    /// <summary>
    /// 任务组
    /// </summary>
    public class TaskGroupDefine
    {
        public int groupId { get; set; }
        public int firstTaskId { get; set; }
        public TaskDefine firstTask { get; set; }
        public TaskDefine lastTask { get; set; }
        public List<TaskDefine> m_dataList = new List<TaskDefine>();

        public List<TaskDefine> GetNextTasks(int curTaskId)
        {
            return TaskDefine.GetNextTasks(curTaskId);
        }

        public void Init()
        {
            if (m_dataList.Count > 0)
            {
                firstTask = m_dataList[0];
                lastTask = m_dataList[m_dataList.Count - 1];
                firstTaskId = firstTask.id;
                groupId = firstTask.groupId;
            }
        }

        /// <summary>
        /// 任务目标描述
        /// </summary>
        /// <returns></returns>
        public List<string> GetTargetDescs(NetProto.TaskDbRecord dbRecord)
        {
            List<string> targetDescs = new List<string>();
            bool isFinished = true;
            string desc = string.Empty;
            for (int i=0; i<m_dataList.Count; ++i)
            {
                TaskDefine define = m_dataList[i];
                if (define.id == dbRecord.taskId)
                    isFinished = false;

                if (isFinished)
                    desc = define.completedTraceDesc;
                else
                    desc = define.acceptableTraceDesc;

                if (desc.Contains("&"))
                {
                    string[] cs = define.competedCountId.Split(';');
                    for (int k = 0; k < cs.Length; ++k)
                    {
                        int countId = 0;
                        if (!string.IsNullOrEmpty(cs[k]) && int.TryParse(cs[k], out countId))
                        {
                            TaskCountDefine cd = Config.TaskCountDefine.Get(countId);
                            if (cd != null)
                            {
                                string content = cd.countDesc;
                                if (isFinished)
                                {
                                    if (content.Contains("#["))
                                    {
                                        int startpos = cd.countDesc.IndexOf('#');
                                        int endpos = cd.countDesc.IndexOf(']') + 1;
                                        content = cd.countDesc.Substring(0, startpos) + cd.countDesc.Substring(endpos);
                                    }
                                    if (content.Contains("#n"))
                                        content = content.Replace("#n", "");
                                    targetDescs.Add(string.Format("#e{0}#n", content));
                                }
                                else
                                {
                                    targetDescs.Add(string.Format("{0}({1}/{2})", content, 0, cd.count));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (isFinished)
                    {
                        if (desc.Contains("#["))
                        {
                            int startpos = desc.IndexOf('#');
                            int endpos = desc.IndexOf(']') + 1;
                            desc = desc.Substring(0, startpos) + desc.Substring(endpos);
                        }
                        if (desc.Contains("#n"))
                            desc = desc.Replace("#n", "");
                        targetDescs.Add(string.Format("#e{0}#n", desc));
                    }
                    else
                    {
                        targetDescs.Add(desc);
                    }
                }

                if (!isFinished && i < (m_dataList.Count-1))
                {
                    targetDescs.Add("未完待续");
                    break;
                }
            }

            return targetDescs;
        }

    }
}