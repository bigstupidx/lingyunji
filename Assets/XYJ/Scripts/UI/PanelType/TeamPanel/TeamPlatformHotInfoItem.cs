#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    public class TeamPlatformHotInfoItem
    {
        [SerializeField]
        public Image m_headIcon;
        [SerializeField]
        public Image m_jobIcon;
        [SerializeField]
        public Text m_name;
        [SerializeField]
        public Text m_level;
        [SerializeField]
        public Button m_applyBtn;
        [SerializeField]
        public Text m_goalName;
        [SerializeField]
        public Text m_levelLimit;
        [SerializeField]
        public Transform m_root;

        TeamAllTeamInfo m_info;
        System.Action<TeamPlatformHotInfoItem> m_onClickItem;

        public TeamAllTeamInfo Info { get { return m_info; } }

        public void Set(TeamAllTeamInfo info, System.Action<TeamPlatformHotInfoItem> onClickItem)
        {
            m_info = info;
            m_onClickItem = onClickItem;

            TeamMemberData leaderData = info.members[info.leaderUid];
            m_goalName.text = TeamUtil.GetGoalName(m_info.goalId);
            m_name.text = leaderData.name;
            m_levelLimit.text = string.Format("{0}-{1}级", info.limit.minLevel, info.limit.maxLevel);
            m_level.text = string.Format("{0}级", leaderData.level);
            TeamProfSexResConfig Leadercfg = TeamUtil.GetProfSexResCfg(leaderData.prof, leaderData.sex);
            if (null != Leadercfg)
            {
                Helper.SetSprite(m_headIcon, Leadercfg.headIcon);
                Helper.SetSprite(m_jobIcon, Leadercfg.profIcon);
            }

            List<TeamMemberData> memberDatas = TeamUtil.SortTeamMember(info);
            for (int i = 0; i < TeamDef.MAX_MEMBER_COUNT; ++ i)
            {
                string childPath = string.Format("Members/{0}", i);
                Transform memberObj = m_root.Find(childPath);
                if (i < memberDatas.Count)
                {
                    memberObj.GetComponent<StateRoot>().SetCurrentState(1, true);
                    TeamMemberData memberData = memberDatas[i];
                    Text memberLevelText = memberObj.Find("Level/Text").GetComponent<Text>();
                    memberLevelText.text = memberData.level.ToString();
                    Image memberJobIcon = memberObj.Find("Icon").GetComponent<Image>();
                    TeamProfSexResConfig cfg = TeamUtil.GetProfSexResCfg(memberData.prof, memberData.sex);
                    if (null != cfg)
                        Helper.SetSprite(memberJobIcon, cfg.profIcon);
                }
                else
                {
                    memberObj.GetComponent<StateRoot>().SetCurrentState(0, true);
                }
            }

            m_applyBtn.onClick.AddListenerIfNoExist(this.OnClickBtn);
            m_root.GetComponent<Button>().onClick.AddListenerIfNoExist(this.OnClick);
        }

        private void OnClick()
        {
            if (null != m_onClickItem)
            {
                m_onClickItem(this);
            }
        }

        private void OnClickBtn()
        {
            App.my.eventSet.FireEvent<int>(EventID.Team_ApplyJoinTeam, m_info.teamId);
            this.OnClick();
        }

        public int GetTeamId()
        {
            if (null != m_info)
                return m_info.teamId;
            return 0;
        }
    }
}

#endif