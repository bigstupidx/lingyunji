namespace xys
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using Config;

    /// <summary>
    /// 对白管理
    /// </summary>
    public class TaskDialogMgr : Singleton<TaskDialogMgr>
    {
        // 当前任务信息数据
        int m_taskId = 0;
        List<TaskDialogDefine> m_groupDialogDefines = new List<TaskDialogDefine>();
        TaskDialogPrototype m_curDialog = null;

        // 是否做对白队列，可按顺序做多个对白组？

        /// <summary>
        /// 获取当前对白配置
        /// </summary>
        TaskDialogDefine GetDialogDefine(int index)
        {
            for (int i=0; i<m_groupDialogDefines.Count; ++i)
            {
                if (index == m_groupDialogDefines[i].numIdx)
                    return m_groupDialogDefines[i];
            }

            return null;
        }
        
        /// <summary>
        /// 整个对白组结束
        /// </summary>
        void DialogsFinished()
        {
            // 事件
            if (m_curDialog!=null)
            {
                m_curDialog = null;
            }
        }

        /// <summary>
        /// 强制关闭整个对白组
        /// </summary>
        public void StopDialogs()
        {
            if (m_curDialog!=null)
            {
                m_curDialog.Stoy();
                m_curDialog = null;
            }
        }

        /// <summary>
        /// 播放对白
        /// </summary>
        /// <param name="groupId"></param>
        public void PlayDialogs(int taskId)
        {
            if (m_curDialog!=null)
            {
                StopDialogs();
            }
            List<TaskDialogDefine> groupDefines = TaskDialogDefine.GetGroupBygroupId(taskId);
            if (groupDefines!=null && groupDefines.Count>0)
            {
                m_taskId = taskId;
                m_groupDialogDefines.AddRange(groupDefines);

                StartPlay();
            }
        }

        void StartPlay ()
        {
            TaskDialogDefine define = GetDialogDefine(1);
            if (define == null)
            {
                Debuger.LogError(string.Format("获取对白失败，对白id={0}，编号idx={1}", m_taskId, 1));
                DialogsFinished();
                return;
            }

            m_groupDialogDefines.Remove(define);

            m_curDialog = new TaskDialogPrototype();
            m_curDialog.InitByDefine(define);
            m_curDialog.Play(EndPlay);
        }

        void EndPlay(int nextIdx)
        {
            // 判断对白是否结束
            if (nextIdx == 0 || (m_curDialog != null && m_curDialog.isGroupEnd))
            {
                DialogsFinished();
                return;
            }
            m_curDialog.Finished();

            // 获取下一句对白
            TaskDialogDefine define = GetDialogDefine(nextIdx);
            if (define == null)
            {
                Debuger.LogError(string.Format("获取对白失败，对白id={0}，编号idx={1}", m_taskId, nextIdx));
                return;
            }
            m_groupDialogDefines.Remove(define);

            m_curDialog.InitByDefine(define);
            m_curDialog.Play(EndPlay);
        }

    }

}
