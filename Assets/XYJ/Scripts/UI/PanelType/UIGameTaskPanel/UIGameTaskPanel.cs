#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UIWidgets;
    using xys.UI;
    using xys.UI.State;

    using Config;
    using NetProto;
    using NetProto.Hot;

    class UIGameTaskPanel : HotPanelBase
    {
        // 是否可接
        [SerializeField]
        StateRoot m_rootState;

        // 是否有任务
        [SerializeField]
        StateRoot m_hasTaskState;

        // 已接按钮
        [SerializeField]
        Button m_acceptedBtn;

        // 可接按钮
        [SerializeField]
        Button m_acceptableBtn;

        // 任务目录
        [SerializeField]
        Accordion m_Accordion;

        [SerializeField]
        Transform m_menuRoot;

        [SerializeField]
        GameObject m_toggleObject;// 折叠对象

        [SerializeField]
        GameObject m_contentObject;// 折叠内容对象

        // 任务信息========================

        // 任务目标
        [SerializeField]
        UIGroup m_taskTarget;

        // 任务描述
        [SerializeField]
        Text m_taskDesc;

        // 任务奖励
        [SerializeField]
        ILMonoBehaviour m_ILRewardInfo;
        UITaskRewardInfo m_rewardInfo;

        // 放弃
        [SerializeField]
        Button m_giveupBtn;

        // 前往
        [SerializeField]
        Button m_gotoBtn;

        #region 内部方法

        GameTaskMgr m_gameTaskMgr;
        int m_curSelectTaskId = 0;
        AccordionItem m_selectedItem = null;
        GameObject m_selectedChildObject = null;

        void ResetData()
        {
            m_curSelectTaskId = 0;
            m_selectedItem = null;
            m_selectedChildObject = null;
        }

        /// <summary>
        /// 获取折叠对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        AccordionItem GetAccordionItem(int index)
        {
            AccordionItem item = null;
            if (index >= 0 && index < m_Accordion.Items.Count)
            {
                item = m_Accordion.Items[index];
                item.ContentObject.SetActive(false);
            }
            else
            {
                item = new AccordionItem();
                item.ToggleObject = GameObject.Instantiate(m_toggleObject);
                item.ToggleObject.SetActive(true);
                item.ToggleObject.transform.parent = m_menuRoot;

                StateRoot state = item.ToggleObject.GetComponent<StateRoot>();
                state.CurrentState = 0;

                item.ContentObject = GameObject.Instantiate(m_contentObject);
                item.ContentObject.transform.parent = m_menuRoot;
                item.ContentObject.SetActive(false);

                m_Accordion.Items.Add(item);
            }

            item.Open = false;
            return item;
                
        }

        /// <summary>
        /// 生成折叠目录
        /// </summary>
        void GenerateAccordionMenu()
        {
            ResetData();
            AccordionItem item = null;
            AccordionItem mainItem = null;
            List<TaskDbRecord> dbRecords = new List<TaskDbRecord> ();
            
            // 主线任务
            int index = 0;
            TaskDbRecord dbRecord = m_gameTaskMgr.m_taskData.main.curTask;
            if (dbRecord != null && dbRecord.taskId != 0)
            {
                item = GetAccordionItem(index);
                index++;

                dbRecords.Add(dbRecord);
                Button btn = item.ToggleObject.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(()=>{
                    OnCallToggleItem(item, dbRecords);
                });

                mainItem = item;
            }

            // 环任务
            for (int i=0; i<m_gameTaskMgr.m_taskData.loops.Count; ++i)
            {
                TaskLoopData loopData = m_gameTaskMgr.m_taskData.loops[i];
                if (loopData.curTask != null && loopData.curTask.taskId != 0)
                {
                    item = GetAccordionItem(index);
                    index++;

                    dbRecords.Clear();
                    dbRecords.Add(loopData.curTask);
                    Button btn = item.ToggleObject.GetComponent<Button>();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        OnCallToggleItem(item, dbRecords);
                    });
                }
            }

            // 默认打开主线任务
            if (mainItem != null)
            {
                dbRecord = m_gameTaskMgr.m_taskData.main.curTask;
                dbRecords.Clear();
                dbRecords.Add (dbRecord);
                OnCallToggleItem(mainItem, dbRecords);
                UIGroup group = mainItem.ContentObject.GetComponent<UIGroup>();
                group.SetCount(dbRecords.Count);
                OnSetAcceptedTaskInfo(dbRecord, group.Get(0));
            }
        }

        void OnCallToggleItem (AccordionItem item, List<TaskDbRecord> dbRecords)
        {
            StateRoot state = null;
            if (item.Open)
            {
                m_Accordion.Close(item);
                state = item.ToggleObject.GetComponent<StateRoot>();
                state.CurrentState = 0;
                m_selectedItem = null;
                return;
            }

            m_Accordion.Open(item);
            m_selectedItem = item;
            state = item.ToggleObject.GetComponent<StateRoot>();
            state.CurrentState = 1;
            UIGroup group = item.ContentObject.GetComponent<UIGroup>();
            group.SetCount(dbRecords.Count);
            for (int i = 0; i < dbRecords.Count; ++i)
            {
                TaskDbRecord dbRecord = dbRecords[i];
                TaskDefine define = TaskDefine.Get(dbRecord.taskId);
                GameObject childObject = group.Get(i);
                state = childObject.GetComponent<StateRoot>();
                if (m_selectedChildObject == childObject)
                    state.CurrentState = 1;
                else
                    state.CurrentState = 0;
                Text text = childObject.GetComponentInChildren<Text>();
                text.text = string.Format("{0} {1}", define.taskChapterName, define.taskName);
                Button btn = childObject.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    OnSetAcceptedTaskInfo(dbRecord, childObject);
                });
            }
        }

        void OnSetAcceptedTaskInfo(TaskDbRecord dbRecord, GameObject obj)
        {
            if (m_selectedChildObject!=obj)
            {
                m_selectedChildObject = obj;
                StateRoot state = obj.GetComponent<StateRoot>();
                state.CurrentState = 1;
            }
            if (dbRecord.taskId != m_curSelectTaskId)
            {
                // 设置任务信息
                m_curSelectTaskId = dbRecord.taskId;
                // 任务目标
                TaskGroupDefine groupDefine = TaskDefine.GetTaskGroup(dbRecord.groupId);
                List<string> targetDescs = groupDefine.GetTargetDescs(dbRecord);
                m_taskTarget.SetCount(targetDescs.Count);
                for (int i=0; i<targetDescs.Count; ++i)
                {
                    GameObject go = m_taskTarget.Get(i);
                    WXB.SymbolText text = go.GetComponentInChildren<WXB.SymbolText>();
                    text.text = targetDescs[i];
                }

                TaskDefine define = TaskDefine.Get(dbRecord.taskId);
                // 任务描述
                m_taskDesc.text = define.taskDesc;

                // 任务奖励
                m_rewardInfo.SetInfo(groupDefine.lastTask);

                // 注册前往和放弃
                // 放弃
                m_giveupBtn.onClick.RemoveAllListeners();
                m_giveupBtn.onClick.AddListener(() => { m_gameTaskMgr.GiveupTaskReq(define.id); });
                // 前往
                m_gotoBtn.onClick.RemoveAllListeners();
                m_gotoBtn.onClick.AddListener(() => { });
            }
        }

        void DestoryAccordionMenu()
        {
            if (m_selectedItem!=null)
            {
                m_Accordion.Close(m_selectedItem);
                m_selectedItem = null;
            }

            ResetData();
        }

        #endregion

        void OnAccepted()
        {
            m_rootState.CurrentState = 0;
            m_hasTaskState.CurrentState = 1;
            GenerateAccordionMenu();
        }

        void OnAcceptable()
        {
            m_rootState.CurrentState = 1;
            m_hasTaskState.CurrentState = 0;
            
            DestoryAccordionMenu();
        }

        UIGameTaskPanel() : base(null) { }
        UIGameTaskPanel(UIHotPanel parent) : base(parent) { }

        protected override void OnInit()
        {
            if (m_ILRewardInfo!=null)
            {
                m_rewardInfo = (UITaskRewardInfo)m_ILRewardInfo.GetObject();
            }

            m_acceptedBtn.onClick.AddListener(OnAccepted);
            m_acceptableBtn.onClick.AddListener(OnAcceptable);

            m_Accordion.Animate = false;
            m_toggleObject.SetActive(false);
            m_contentObject.SetActive(false);
        }

        protected override void OnShow(object p)
        {
            m_gameTaskMgr = hotApp.my.GetModule<HotGameTaskModule>().gameTaskMgr;
            // 默认限时已接任务的状态
            OnAccepted();
        }

        protected override void OnHide()
        {
            base.OnHide();
            DestoryAccordionMenu();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
#endif