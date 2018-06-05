#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using NetProto;
using xys.hot.Team;
using xys.UI;

namespace xys.hot.UI
{
    [AutoILMono]
    public class TeamOrganizeHotMemberItem
    {
        [SerializeField]
        Image m_jobIcon;
        [SerializeField]
        Text m_name;
        [SerializeField]
        Text m_level;
        [SerializeField]
        public RawImage m_roleIcon;
        [SerializeField]
        Button m_roleBtn;
        [SerializeField]
        Button m_inviteBtn;
        public GameObject m_self;

        TeamMemberData m_data;
        public TeamMemberData data { get { return m_data; } }

        System.Action<TeamOrganizeHotMemberItem> m_onClickRoleIcon;
        System.Action m_onClickInviteBtn;

        public void Set(int idx, TeamMemberData data, System.Action<TeamOrganizeHotMemberItem> onClickRoleIcon, System.Action onClickInviteBtn, RTTModelPartHandler rtt)
        {
            m_onClickRoleIcon = onClickRoleIcon;
            m_onClickInviteBtn = onClickInviteBtn;
            m_inviteBtn.onClick.AddListenerIfNoExist(this.OnClickInviteBtn);
            m_roleBtn.onClick.AddListenerIfNoExist(this.OnClickRoleIcon);

            m_data = data;
            if (null != m_data)
            {
                m_name.text = m_data.name;
                m_level.text = string.Format("{0}级", m_data.level);

                TeamProfSexResConfig cfg = TeamUtil.GetProfSexResCfg(m_data.prof, m_data.sex);
                if (null != cfg)
                {
                    Helper.SetSprite(m_jobIcon, cfg.profIcon);
                    int roleId = Config.RoleJob.GetRoleID(m_data.prof, m_data.sex);
                    if (null != rtt)
                    {
                        if (rtt.modelId != roleId || !rtt.IsActive())
                            rtt.SetModel(roleId);
                    }
                }
            }
            else
            {
                if (null != rtt)
                    rtt.SetRenderActive(false);
            }
        }

        protected void OnClickRoleIcon()
        {
            if (null != m_onClickRoleIcon && null != m_data)
            {
                m_onClickRoleIcon(this);
            }
        }

        protected void OnClickInviteBtn()
        {
            if (null != m_onClickInviteBtn)
            {
                m_onClickInviteBtn();
            }
        }

        TeamMemberData GetData() { return m_data; }
    }
}

#endif