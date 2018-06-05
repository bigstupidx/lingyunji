#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;

namespace xys.hot.UI
{

    [AutoILMono]
    public class TeamPlatformHotGoalItem
    {
        public Text m_text;
        public Image m_icon;
        public Button m_btn;
        public GameObject m_self;

        private Config.TeamGoalType m_goalTypeCfg = null;
        protected System.Action<TeamPlatformHotGoalItem> m_onClickItem;
        private Accordion m_accordion;

        public int SelectedGoalId { get { return -1;} }
        void Awake()
        {

        }

        public void Set(Config.TeamGoalType cfg, Accordion accordion, System.Action<TeamPlatformHotGoalItem> onClickItem)
        {
            m_goalTypeCfg = cfg;
            m_onClickItem = onClickItem;
            m_accordion = accordion;
            m_text.text = m_goalTypeCfg.name;
            m_btn.onClick.AddListenerIfNoExist(OnClick);
            m_icon.gameObject.SetActive(cfg.goalGroups.Count > 1);
        }

        private void OnClick()
        {
            if (null != m_onClickItem)
            {
                m_onClickItem(this);
            }
        }
    }
}

#endif