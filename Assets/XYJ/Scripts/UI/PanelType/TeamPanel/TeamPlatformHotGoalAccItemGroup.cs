#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace xys.hot.UI
{
    [AutoILMono]
    class TeamPlatformHotGoalAccItemGroup
    {
        public GameObject m_accItemPrefab;
        [SerializeField]
        Transform m_self;
        Config.TeamGoalType m_cfg;
        System.Action<Config.TeamGoalGroup> m_clickAccItemCb;

        List<TeamPlatformHotGoalAccItem> m_items = new List<TeamPlatformHotGoalAccItem>();
        public void Set(Config.TeamGoalType cfg, System.Action<Config.TeamGoalGroup> accItemClickCb)
        {
            m_cfg = cfg;
            m_clickAccItemCb = accItemClickCb;

            if (m_cfg.goalGroups.Count <= 1)
                return;

            foreach (Config.TeamGoalGroup goal in m_cfg.goalGroups)
            {
                GameObject obj = GameObject.Instantiate(m_accItemPrefab);
                obj.SetActive(true);
                obj.transform.SetParent(m_self, false);

                TeamPlatformHotGoalAccItem accItem = obj.GetComponent<ILMonoBehaviour>().GetObject() as TeamPlatformHotGoalAccItem;
                accItem.Set(goal, this.OnClickAccItem);
                m_items.Add(accItem);
            }
        }

        public void Unfold()
        {
            if (m_cfg.goalGroups.Count <= 0)
                return;

            if (null != m_clickAccItemCb)
                this.m_clickAccItemCb(m_cfg.goalGroups[0]);
            if (m_items.Count > 0)
                this.HightLightItem(m_items[0]);
        }

        public void HightLightItem(TeamPlatformHotGoalAccItem hlItem)
        {
            foreach (TeamPlatformHotGoalAccItem item in m_items)
                item.m_state.SetCurrentState(0, false);
            hlItem.m_state.SetCurrentState(1, true);
        }

        private void OnClickAccItem(TeamPlatformHotGoalAccItem item)
        {
            this.HightLightItem(item);
            Config.TeamGoalGroup cfg = item.GetCfg();
            if (null != cfg && null != m_clickAccItemCb)
            {
                m_clickAccItemCb(cfg);
            }
        }
    }
}

#endif