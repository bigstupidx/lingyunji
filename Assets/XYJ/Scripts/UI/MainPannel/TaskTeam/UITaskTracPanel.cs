#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Config;
    using NetProto;
    using xys.UI;
    using xys.UI.State;

    [AutoILMono]
    class UITaskTracPanel
    {
        [SerializeField]
        UIGroup m_taskGroup;

        public void SetTasks(List<TaskDbRecord> records)
        {
            for (int i=0;i<records.Count; ++i)
            {
                GameObject go = m_taskGroup.Get(i);
                // 监听追踪
                Button tracBtn = go.GetComponent<Button>();
                tracBtn.onClick.RemoveAllListeners();
                tracBtn.onClick.AddListener(() => { });

                // 设置信息
                ILMonoBehaviour ilMono = go.GetComponent<ILMonoBehaviour>();
                UITaskTracItem item = ilMono.GetObject() as UITaskTracItem;
                item.SetData(records[i]);
            }
        }
    }

    [AutoILMono]
    class UITaskTracItem
    {
        [SerializeField]
        Text m_titleText;

        [SerializeField]
        GameObject m_stateObj;

        [SerializeField]
        StateRoot m_finishSate;

        [SerializeField]
        UIGroup m_taskList;

        public void SetData(TaskDbRecord dbRecord)
        {
            TaskDefine define = TaskDefine.Get(dbRecord.taskId);
            TaskGroupDefine groupDefine = TaskDefine.GetTaskGroup(dbRecord.groupId);
            List<string> taskTargets = groupDefine.GetTargetDescs(dbRecord);
            // 标题
            if (define.taskType == 1)
                m_titleText.text = string.Format("【{0}】 {1}", define.taskChapterName, define.taskName);
            else
                m_titleText.text = string.Format("【环】 {1}", define.taskName);

            // 完成状态
            if (define.showCompletedTips == 0)
                m_stateObj.SetActive(false);
            else
                m_stateObj.SetActive(true);

            // 任务追踪列表
            m_taskList.SetCount(taskTargets.Count);
            for (int i=0;i<taskTargets.Count; ++i)
            {
                GameObject go = m_taskList.Get(i);
                WXB.SymbolText text = go.GetComponentInChildren<WXB.SymbolText>();
                text.text = taskTargets[i];
            }
        }
    }
}

#endif