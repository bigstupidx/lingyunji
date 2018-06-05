#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI.State;

namespace xys.hot.UI
{
    class TaskTeamPanel
    {
        enum TabName
        {
            Task,
            Team,
        }

        GameObject m_self;
        GameObject m_panel;

        Button m_switchShowBtn;
        StateRoot m_switcTabSR;
        Button m_taskTabBtn;
        Button m_teamTabBtn;

        GameObject m_teamTab;
        GameObject m_taskTab;

        [SerializeField]
        ILMonoBehaviour m_ILTeamPanel;
        MainUITeamPanel m_teamPanel;

        [SerializeField]
        ILMonoBehaviour m_ILTaskTracPanel;
        UITaskTracPanel m_taskTracPanel;

        bool m_isShowPanel = false;
        TabName m_activeTab = TabName.Team;

        private T FindComponent<T>(string path, GameObject go = null)
        {
            if (null == go)
                go = m_self;
            return go.transform.Find(path).GetComponent<T>();
        }

        private GameObject FindGameObject(string path, GameObject go = null)
        {
            if (null == go)
                go = m_self;
            return go.transform.Find(path).gameObject;
        }

        public TaskTeamPanel(GameObject go)
        {
            m_self = go.transform.Find("Offset/TaskTeam").gameObject;
            m_panel = this.FindGameObject("Panel");

            m_taskTab = this.FindGameObject("Panel/TaskTab/offset");
            m_teamTab = this.FindGameObject("Panel/TeamTab");

            m_ILTaskTracPanel = m_taskTab.GetComponent<ILMonoBehaviour>();
            m_taskTracPanel = m_ILTaskTracPanel.GetObject() as UITaskTracPanel;

            ILMonoBehaviour ilMono = m_teamTab.GetComponent<ILMonoBehaviour>();
            m_teamPanel = ilMono.GetObject() as MainUITeamPanel;

            m_switchShowBtn = this.FindComponent<Button>("ShowBtn", m_panel);
            m_switchShowBtn.onClick.AddListenerIfNoExist(OnClickSwitchShowBtn);
            m_switcTabSR = this.FindComponent<StateRoot>("TabButton", m_panel);

            m_taskTabBtn = this.FindComponent<Button>("TabButton/TaskBtn", m_panel);
            m_taskTabBtn.onClick.AddListenerIfNoExist(OnClickTaskTabBtn);

            m_teamTabBtn = this.FindComponent<Button>("TabButton/TeamBtn", m_panel);
            m_teamTabBtn.onClick.AddListenerIfNoExist(OnClickTeamTabBtn);

            m_activeTab = TabName.Task;// …Ë÷√ƒ¨»œTab
            this.UpdateUI();
        }

        private void OnClickSwitchShowBtn()
        {
            m_isShowPanel = !m_isShowPanel;
            this.UpdateUI();
        }
        private void OnClickTaskTabBtn()
        {
            if (TabName.Task != m_activeTab)
            {
                m_activeTab = TabName.Task;
                this.UpdateUI();
                List<TaskDbRecord> records = hotApp.my.GetModule<HotGameTaskModule>().GetCurTaskRecords();
                m_taskTracPanel.SetTasks(records);
            }
            else
            {
                App.my.uiSystem.ShowPanel("UIGameTaskPanel", null, true);
            }
        }
        private void OnClickTeamTabBtn()
        {
            if (TabName.Team != m_activeTab)
            {
                m_activeTab = TabName.Team;
                this.UpdateUI();
            }
            else
            {
                if (TeamUtil.teamMgr.InTeam())
                {
                    TeamUtil.ShowOrganizePanel();
                }
                else
                {
                    xys.App.my.uiSystem.ShowPanel("UITeamPlatformPanel", new object() { }, true);
                }
            }
        }

        private void UpdateUI()
        {
            m_switcTabSR.CurrentState = TabName.Task == m_activeTab ? 0 : 1;
//             m_teamTab.SetActive(TabName.Team == m_activeTab && m_isShowPanel);
//             m_taskTab.SetActive(TabName.Task == m_activeTab && m_isShowPanel);
//             m_switcTabSR.gameObject.SetActive(m_isShowPanel);
//             m_switcTabSR.SetCurrentState((int)m_activeTab, true);
           // m_switchShowBtn.gameObject.GetComponent<StateRoot>().SetCurrentState((m_isShowPanel ? 1 : 0), true);
        }
    }
}

#endif