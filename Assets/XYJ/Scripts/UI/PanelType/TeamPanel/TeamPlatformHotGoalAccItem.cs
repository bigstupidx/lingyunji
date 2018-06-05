#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class TeamPlatformHotGoalAccItem
    {
        public Text m_text;
        public StateRoot m_state;
        public Button m_btn;
        public Transform m_self;

        Config.TeamGoalGroup m_cfg;
        System.Action<TeamPlatformHotGoalAccItem> m_onClick;
        public void Set(Config.TeamGoalGroup cfg, System.Action<TeamPlatformHotGoalAccItem> onClick)
        {
            m_cfg = cfg;
            m_onClick = onClick;
            m_text.text = m_cfg.name;
            m_state.SetCurrentState(0, true);

            m_self.GetComponent<Button>().onClick.AddListenerIfNoExist(OnClick);
        }

        private void OnClick()
        {
            if (null != m_onClick)
                m_onClick(this);
        }

        public Config.TeamGoalGroup GetCfg()
        {
            return m_cfg;
        }
    }
}

#endif