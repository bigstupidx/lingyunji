#if !USE_HOT
using NetProto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono, RequireComponent(typeof(UnityEngine.UI.ScrollRect))]
    public class TeamPlatformHotGoalScrollView
    {
        [SerializeField]
        public GameObject m_scrollContent;
        public GameObject m_itemPrefab;
        public GameObject m_accItemGroupPrefab;
        public Accordion m_accordion;
        [SerializeField]
        public StateToggle m_difficultLvlBtns;

        private bool isInit = false;
        List<TeamPlatformHotGoalItem> m_goalItems = new List<TeamPlatformHotGoalItem>();
        List<TeamPlatformHotGoalAccItemGroup> m_accItemGroup = new List<TeamPlatformHotGoalAccItemGroup>();
        Dictionary<TeamPlatformHotGoalItem, TeamPlatformHotGoalAccItemGroup> m_goalItem2AccItemGroup = new Dictionary<TeamPlatformHotGoalItem, TeamPlatformHotGoalAccItemGroup>();

        TeamPlatformHotGoalItem m_selectedGoalItem;
        Config.TeamGoalGroup m_groupCfg;
        public void Show()
        {
            if (!isInit)
            {
                isInit = true;

                m_difficultLvlBtns.OnSelectChange = OnSelectDifficultLvl;

                Dictionary<int, Config.TeamGoalType> goalTypeCfgs = Config.TeamGoalType.GetAll();
                foreach (Config.TeamGoalType cfg in goalTypeCfgs.Values)
                {
                    GameObject goalItem = GameObject.Instantiate(m_itemPrefab);
                    GameObject accItemGroup = GameObject.Instantiate(m_accItemGroupPrefab);

                    AccordionItem accItem = new AccordionItem();
                    accItem.ToggleObject = goalItem;
                    accItem.ContentObject = accItemGroup;
                    accItem.Open = false;

                    goalItem.transform.SetParent(m_scrollContent.transform, false);
                    goalItem.SetActive(true);
                    goalItem.transform.localPosition = Vector3.zero;
                    TeamPlatformHotGoalItem item = goalItem.GetComponent<ILMonoBehaviour>().GetObject() as TeamPlatformHotGoalItem;
                    item.Set(cfg, m_accordion, this.OnClickGoalItem);
                    m_goalItems.Add(item);

                    accItemGroup.SetActive(true);
                    accItemGroup.transform.SetParent(m_scrollContent.transform, false);
                    accItemGroup.transform.localPosition = Vector3.zero;
                    TeamPlatformHotGoalAccItemGroup group = accItemGroup.GetComponent<ILMonoBehaviour>().GetObject() as TeamPlatformHotGoalAccItemGroup;
                    group.Set(cfg, this.OnClickAccItem);
                    accItemGroup.SetActive(false);

                    RectTransform rectTransform = accItemGroup.GetComponent<RectTransform>();
                    float objHeight = 0;
                    if (cfg.goalGroups.Count > 1)
                        objHeight = cfg.goalGroups.Count * 56;
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, objHeight);

                    m_accordion.Items.Add(accItem);
                    m_accordion.Close(accItem);
                    m_accItemGroup.Add(group);
                    m_goalItem2AccItemGroup[item] = group;

                    if (null == m_selectedGoalItem)
                        m_selectedGoalItem = item;
                }
            }

            if (null != m_selectedGoalItem)
            {
                TeamPlatformHotGoalItem chooseItem = m_selectedGoalItem;
                m_selectedGoalItem = null;
                this.OnClickGoalItem(chooseItem);
            }   
        }

        public void OnClickGoalItem(TeamPlatformHotGoalItem goalItem)
        {
            var accItem = m_accordion.Items.Find(x => x.ToggleObject == goalItem.m_self);

            if (goalItem != m_selectedGoalItem)
            {
                foreach (TeamPlatformHotGoalItem item in m_goalItems)
                {
                    item.m_btn.GetComponent<StateRoot>().SetCurrentState(0, false);
                }

                TeamPlatformHotGoalAccItemGroup accItemGroup = null;
                if (m_goalItem2AccItemGroup.TryGetValue(goalItem, out accItemGroup))
                {
                    accItemGroup.Unfold();
                }

                m_accordion.ToggleItem(accItem);
            }
            else
            {
                if (accItem.Open)
                    m_accordion.Close(accItem);
                else
                    m_accordion.Open(accItem);
            }

            m_selectedGoalItem = goalItem;
            m_selectedGoalItem.m_btn.gameObject.GetComponent<StateRoot>().SetCurrentState(accItem.Open ? 1 : 0, true);
        }

        public void OnClickAccItem(Config.TeamGoalGroup groupCfg)
        {
            if (m_groupCfg != groupCfg)
            {
                m_groupCfg = groupCfg;
                m_difficultLvlBtns.Select = 0;
                m_difficultLvlBtns.gameObject.SetActive(m_groupCfg.goals.Count > 1);
                this.QueryTeamInfos();
            }
        }
        protected void OnSelectDifficultLvl(StateRoot sr, int idx)
        {
            this.QueryTeamInfos();
        }

        int m_lastQueryGoalId = -1;
        public void QueryTeamInfos()
        {
            Config.TeamGoal goalCfg = m_groupCfg.GetGoal((Config.TeamGoalDifficultyLevel)m_difficultLvlBtns.Select);
            if (null == goalCfg && m_groupCfg.goals.Count > 0)
                goalCfg = m_groupCfg.goals[0];
            if (null == goalCfg)
                return;

            if (-1 == m_lastQueryGoalId)
                m_lastQueryGoalId = goalCfg.id;
            int oldGoalId = m_lastQueryGoalId;
            m_lastQueryGoalId = goalCfg.id;
            if (TeamUtil.teamMgr.IsAutoJoinTeam())
            {
                if (oldGoalId != m_lastQueryGoalId)
                   TeamUtil.teamMgr.AutoJoinTeam(false, 0);
            }

            ES_QueryTeamsFilter evData = new ES_QueryTeamsFilter();
            evData.queryReason = TeamQueryTeamsReason.TeamPlatform;
            evData.goalId = goalCfg.id;
            evData.beginTeamId = 0;
            App.my.eventSet.FireEvent<ES_QueryTeamsFilter>(EventID.Team_PlatformQueryTeamInfos, evData);
        }
    }
}

#endif