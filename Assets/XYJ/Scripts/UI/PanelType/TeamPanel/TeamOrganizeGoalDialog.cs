#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class TeamOrganizeGoalDialog
    {
        [SerializeField]
        GameObject m_lvlTextPrefab;
        [SerializeField]
        Transform m_leftLvlScroll;
        [SerializeField]
        Transform m_rightLvlScroll;
        [SerializeField]
        Transform m_lvlTextDropItems;

        [SerializeField]
        GameObject m_goalTypePrefab;
        [SerializeField]
        GameObject m_goalItemPrefab;
        [SerializeField]
        GameObject m_goalItemGroupPrefab;
        [SerializeField]
        Transform m_goalContainer;
        [SerializeField]
        Accordion m_accordion;
        [SerializeField]
        Button m_comfirmBtn;
        [SerializeField]
        StateToggle m_difficultLvlBtns;

        int m_goalId;
        int m_leftLevel = TeamDef.MIN_GOAL_LIMIT_LEVEL;
        int m_rightLevel = TeamDef.MAX_GOAL_LIMIT_LEVEL;
        Config.TeamGoal m_goalCfg = null;

        GameObject m_selectedGoalType;
        GameObject m_selectedGoalItem;

        Dictionary<GameObject, Config.TeamGoalType> m_obj2GoalTypeCfgMap = new Dictionary<GameObject, Config.TeamGoalType>();
        Dictionary<GameObject, Config.TeamGoalGroup> m_obj2GroupCfgMap = new Dictionary<GameObject, Config.TeamGoalGroup>();
        Dictionary<Config.TeamGoalGroup, GameObject> m_groupCfg2Obj = new Dictionary<Config.TeamGoalGroup, GameObject>();
        Dictionary<GameObject, GameObject> m_groupItem2TypeItemMap = new Dictionary<GameObject, GameObject>();
        Dictionary<GameObject, AccordionItem> m_typeItem2AccItemMap = new Dictionary<GameObject, AccordionItem>();

        const int GOALTYPE_STATEROOT_SINGLE_UNSELECTED = 0;
        const int GOALTYPE_STATEROOT_SINGLE_SELECTED = 1;
        const int GOALTYPE_STATEROOT_MORE_UNSELECTED = 2;
        const int GOALTYPE_STATEROOT_MORE_SELECTED = 3;

        System.Action<int, int, int> m_comfirmCb = null;

        private int GetGaolTypeStateRootVal(int goalCount, bool isSelected)
        {
            if (goalCount > 1)
                return isSelected ? GOALTYPE_STATEROOT_MORE_SELECTED : GOALTYPE_STATEROOT_MORE_UNSELECTED;
            else
                return isSelected ? GOALTYPE_STATEROOT_SINGLE_SELECTED : GOALTYPE_STATEROOT_SINGLE_UNSELECTED;
        }

        public void Init(System.Action<int, int, int> comfirmCb)
        {
            m_comfirmCb = comfirmCb;
            m_comfirmBtn.onClick.AddListenerIfNoExist(this.OnClickComfirmBtn);
            m_difficultLvlBtns.OnSelectChange = OnSelectDifficultLvl;

            m_goalId = 0;
            m_goalCfg = Config.TeamGoal.Get(m_goalId);

            Dictionary<int, Config.TeamGoalType> goalTypes = Config.TeamGoalType.GetAll();
            foreach (Config.TeamGoalType goalType in goalTypes.Values)
            {
                GameObject typeItem = GameObject.Instantiate(m_goalTypePrefab);
                GameObject itemGroup = GameObject.Instantiate(m_goalItemGroupPrefab);

                AccordionItem accItem = new AccordionItem();

                {
                    m_obj2GoalTypeCfgMap[typeItem] = goalType;
                    m_typeItem2AccItemMap[typeItem] = accItem;

                    typeItem.transform.SetParent(m_goalContainer);
                    typeItem.SetActive(true);
                    typeItem.transform.localScale = Vector3.one;
                    typeItem.transform.localPosition = Vector3.zero;
                    int srValue = this.GetGaolTypeStateRootVal(goalType.goalGroups.Count, false);
                    typeItem.GetComponent<StateRoot>().SetCurrentState(srValue, false);
                    typeItem.GetComponent<Button>().onClick.AddListener(() => { this.OnClickGoalTypeItem(typeItem); });
                    typeItem.transform.Find("Name").GetComponent<Text>().text = goalType.name;
                    typeItem.transform.Find("Triangle").gameObject.SetActive(goalType.goalGroups.Count > 1);
                }

                {
                    itemGroup.transform.SetParent(m_goalContainer);
                    itemGroup.SetActive(true);
                    itemGroup.transform.localScale = Vector3.one;
                    itemGroup.transform.localPosition = Vector3.zero;

                    foreach (Config.TeamGoalGroup goalCfg in goalType.goalGroups)
                    {
                        GameObject goalItem = GameObject.Instantiate(m_goalItemPrefab);
                        m_obj2GroupCfgMap[goalItem] = goalCfg;
                        m_groupCfg2Obj[goalCfg] = goalItem;
                        m_groupItem2TypeItemMap[goalItem] = typeItem;

                        goalItem.transform.SetParent(itemGroup.transform);
                        goalItem.SetActive((goalType.goalGroups.Count > 1));
                        goalItem.transform.localScale = Vector3.one;
                        goalItem.transform.localPosition = Vector3.zero;
                        goalItem.GetComponent<Button>().onClick.AddListenerIfNoExist(() => { this.OnClickGoalItem(goalItem); });
                        goalItem.transform.Find("Text").GetComponent<Text>().text = goalCfg.name;                       
                    }

                    accItem.ToggleObject = typeItem;
                    accItem.ContentObject = itemGroup;
                    accItem.Open = false;
                    m_accordion.Items.Add(accItem);
                }
            }

            this.ResetLevelUI();
        }

        void UpdateGoalTypeUI(GameObject obj, bool isSelected, bool isOpen)
        {
            if (null == obj)
                return;

            Config.TeamGoalType typeCfg = null;
            if (m_obj2GoalTypeCfgMap.TryGetValue(obj, out typeCfg))
            {
                obj.transform.Find("Triangle").GetComponent<StateRoot>().SetCurrentState((isOpen ? 1 : 0), false);
                int stVal = this.GetGaolTypeStateRootVal(typeCfg.goalGroups.Count, isSelected);
                obj.GetComponent<StateRoot>().SetCurrentState(stVal, false);

                AccordionItem accItem = null;
                if (m_typeItem2AccItemMap.TryGetValue(obj, out accItem))
                {
                    if (null == accItem.ContentObjectRect)
                    {
                        if (isOpen)
                            App.my.mainTimer.Register(1, 1, () => { m_accordion.Open(accItem); });
                        else
                            App.my.mainTimer.Register(1, 1, () => { m_accordion.Close(accItem); });
                    }
                    else
                    {
                        if (isOpen)
                            m_accordion.Open(accItem);
                        else
                            m_accordion.Close(accItem);
                    }
                }
            }
        }
        protected void OnSelectDifficultLvl(StateRoot sr, int idx)
        {
            Config.TeamGoal goalCfg = m_goalCfg.belongGroup.GetGoal((Config.TeamGoalDifficultyLevel)idx);
            if (null != goalCfg)
            {
                m_goalCfg = goalCfg;
                m_goalId = m_goalCfg.id;
                m_leftLevel = m_goalCfg.minLevel;
                m_rightLevel = m_goalCfg.maxLevel;

                this.ResetDifficultLvlBtns();
                this.ResetLevelUI();
            }
        }
        private void OnClickComfirmBtn()
        {
            int minLvl = (m_leftLevel < m_rightLevel ? m_leftLevel : m_rightLevel);
            int maxLvl = (m_leftLevel > m_rightLevel ? m_leftLevel : m_rightLevel);

            if (null != m_comfirmCb)
                m_comfirmCb(m_goalId, minLvl, maxLvl);
        }

        private void OnClickGoalTypeItem(GameObject obj)
        {
            if (m_selectedGoalType == obj)
            {
                AccordionItem accItem = null;
                if (m_typeItem2AccItemMap.TryGetValue(obj, out accItem))
                {
                    this.UpdateGoalTypeUI(obj, true, !accItem.Open);
                }

                return;
            }

            GameObject goalObj = null;
            foreach (var kvPair in m_groupItem2TypeItemMap)
            {
                if (kvPair.Value == obj)
                {
                    goalObj = kvPair.Key;
                    break;
                }
            }

            if (null != goalObj)
            {
                this.OnClickGoalItem(goalObj);
            }
        }

        private void OnClickGoalItem(GameObject obj)
        {
            if (obj == m_selectedGoalItem)
                return;

            GameObject typeObj = null;
            if (m_groupItem2TypeItemMap.TryGetValue(obj, out typeObj))
            {
                Config.TeamGoalGroup groupCfg = null;
                if (m_obj2GroupCfgMap.TryGetValue(obj, out groupCfg))
                {
                    if (typeObj != m_selectedGoalType)
                    {
                        if (null != m_selectedGoalType)
                            this.UpdateGoalTypeUI(m_selectedGoalType, false, false);
                        this.UpdateGoalTypeUI(typeObj, true, true);
                    }
                    typeObj.transform.Find("Text").GetComponent<Text>().text = groupCfg.name;
                    if (null != m_selectedGoalItem)
                        m_selectedGoalItem.GetComponent<StateRoot>().SetCurrentState(0, false);
                    obj.GetComponent<StateRoot>().SetCurrentState(1, false);

                    // 先默认选中 Common
                    {
                        m_goalCfg = groupCfg.GetGoal(Config.TeamGoalDifficultyLevel.Common);
                        m_goalId = m_goalCfg.id;
                        m_leftLevel = m_goalCfg.minLevel;
                        m_rightLevel = m_goalCfg.maxLevel;
                    }

                    this.ResetLevelUI();
                    this.ResetDifficultLvlBtns();

                    m_selectedGoalType = typeObj;
                    m_selectedGoalItem = obj;
                }
            }
        }
        private void LeftLvlOnCenter(GameObject centerObj)
        {
            string lvlStr = centerObj.GetComponent<Text>().text;
            Match m = Regex.Match(lvlStr, @"<.*>([0-9]+)<.*>");
            if (m.Groups.Count > 1)
            {
                m_leftLevel = int.Parse(m.Groups[1].Value);
            }
        }

        private void RightLvlOnCenter(GameObject centerObj)
        {
            string lvlStr = centerObj.GetComponent<Text>().text;
            Match m = Regex.Match(lvlStr, @"<.*>([0-9]+)<.*>");
            if (m.Groups.Count > 1)
            {
                m_rightLevel = int.Parse(m.Groups[1].Value);
            }
        }

        public void Show(int goalId, int minLevel, int maxLevel)
        {
            if (minLevel > maxLevel)
            {
                int tmpLevel = minLevel;
                minLevel = maxLevel;
                maxLevel = tmpLevel;
            }

            m_goalId = goalId;
            m_goalCfg = Config.TeamGoal.Get(m_goalId);
            m_leftLevel = (minLevel > 0 ? minLevel : TeamDef.MIN_GOAL_LIMIT_LEVEL);
            m_rightLevel = (maxLevel > 0 && maxLevel < TeamDef.MAX_GOAL_LIMIT_LEVEL ? maxLevel : TeamDef.MAX_GOAL_LIMIT_LEVEL);
            this.ResetLevelUI();
            this.ResetDifficultLvlBtns();

            if (null != m_goalCfg)
            {
                GameObject goalObj = null;
                if (m_groupCfg2Obj.TryGetValue(m_goalCfg.belongGroup, out goalObj))
                {
                    if (goalObj != m_selectedGoalItem && null != m_selectedGoalItem)
                        m_selectedGoalItem.GetComponent<StateRoot>().SetCurrentState(0, false);
                    goalObj.GetComponent<StateRoot>().SetCurrentState(1, false);
                    m_selectedGoalItem = goalObj;

                    GameObject typeObj = null;
                    if (m_groupItem2TypeItemMap.TryGetValue(goalObj, out typeObj))
                    {
                        if (null != m_selectedGoalType && typeObj != m_selectedGoalType)
                            this.UpdateGoalTypeUI(m_selectedGoalType, false, false);
                        this.UpdateGoalTypeUI(typeObj, true, false);
                        typeObj.transform.Find("Text").GetComponent<Text>().text = m_goalCfg.belongGroup.name;
                        m_selectedGoalType = typeObj;
                    }
                }
            }
        }

        private void ResetDifficultLvlBtns()
        {
            if (null != m_goalCfg)
            {
                m_difficultLvlBtns.gameObject.SetActive(m_goalCfg.belongGroup.goals.Count > 1);
                m_difficultLvlBtns.Select = (int)m_goalCfg.difficultyLevel;
            }
        }
        private void ResetLevelUI()
        {
            Config.TeamGoal goalCfg = Config.TeamGoal.Get(m_goalId);
            if (null != goalCfg)
            {
                m_leftLevel = m_leftLevel < goalCfg.minLevel ? goalCfg.minLevel : m_leftLevel;
                m_rightLevel = m_rightLevel > goalCfg.maxLevel ? goalCfg.maxLevel : m_rightLevel;
                this.ResetLevelScroll(m_leftLvlScroll, goalCfg.minLevel, goalCfg.maxLevel, m_leftLevel, this.LeftLvlOnCenter);
                this.ResetLevelScroll(m_rightLvlScroll, goalCfg.minLevel, goalCfg.maxLevel, m_rightLevel, this.RightLvlOnCenter);
            }
        }

        private void ResetLevelScroll(Transform scroll, int minLevel, int maxLevel, int selectedLevel, xys.UI.UCenterOnChild.OnCenterHandler callback)
        {
            if (minLevel > maxLevel)
            {
                int tmpLevel = minLevel;
                minLevel = maxLevel;
                maxLevel = tmpLevel;
            }
            if (selectedLevel < minLevel || selectedLevel > maxLevel)
                selectedLevel = minLevel;

            UCenterOnChild uCenterOnChild = scroll.GetComponent<UCenterOnChild>();
            uCenterOnChild.enabled = false;

            Transform container = scroll.Find("Grid");
            while (container.childCount > 0)
            {
                Transform item = container.GetChild(0);
                item.gameObject.SetActive(false);
                item.SetParent(m_lvlTextDropItems);
            }

            for (int i = minLevel; i <= maxLevel; ++i)
            {
                GameObject item = null;
                if (m_lvlTextDropItems.childCount > 0)
                    item = m_lvlTextDropItems.GetChild(0).gameObject;
                else
                    item = GameObject.Instantiate(m_lvlTextPrefab);
                item.transform.SetParent(container);
                item.SetActive(true);
                item.transform.localScale = Vector3.one;
                item.GetComponent<Text>().text = "<color=#636172>" + i.ToString() + "</color>";
            }

            uCenterOnChild.enabled = true;
            uCenterOnChild.onCenter -= callback;
            uCenterOnChild.onCenter += callback;
            uCenterOnChild.ResetItem();
            uCenterOnChild.SetBeginIndex(selectedLevel - minLevel);
        }
    }
}

#endif