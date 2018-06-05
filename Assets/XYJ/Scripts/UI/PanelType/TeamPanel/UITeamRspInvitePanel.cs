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
    class UITeamRspInvitePanel : HotPanelBase
    {
        UIHotPanel m_parent;

        [SerializeField]
        Button m_closeBtn;
        [SerializeField]
        Button m_allRefuseBtn;
        [SerializeField]
        GameObject m_prefab;
        [SerializeField]
        GameObject m_container;

        Dictionary<GameObject, int> m_obj2TeamIdMap = new Dictionary<GameObject, int>();
        bool m_isOPen = false;

        public UITeamRspInvitePanel() :base(null) { }
        public UITeamRspInvitePanel(xys.UI.UIHotPanel _parent) : base(_parent)
        {

        }

        protected override void OnInit()
        {
            m_closeBtn.onClick.AddListenerIfNoExist(OnClickCloseBtn);
            m_allRefuseBtn.onClick.AddListenerIfNoExist(OnClickAllRefuseBtn);

            App.my.eventSet.Subscribe<TeamInviteJoinInfo>(EventID.Team_NewInviteJoinInfo, OnNewInviteJoinInfo);
        }

        private void OnNewInviteJoinInfo(TeamInviteJoinInfo info)
        {
            if (m_isOPen)
                this.AddItem(info);
        }

        private void OnClickAllRefuseBtn()
        {
            foreach (var kvPair in m_obj2TeamIdMap)
            {
                kvPair.Key.SetActive(false);
               TeamUtil.teamMgr.RspInviteJoinTeam(kvPair.Value, false);
            }

            m_obj2TeamIdMap.Clear();
            xys.App.my.uiSystem.HidePanel("UITeamRspInvitePanel");
        }

        private void OnClickAcceptBtn(int teamId, GameObject go)
        {
            go.SetActive(false);
           TeamUtil.teamMgr.RspInviteJoinTeam(teamId, true);
            m_obj2TeamIdMap.Remove(go);
        }

        private void OnClickCloseBtn()
        {
            App.my.uiSystem.HidePanel("UITeamRspInvitePanel");
        }

        protected override void OnShow(object args)
        {
            m_isOPen = true;

            Dictionary<int, TeamInviteJoinInfo> datas = args as Dictionary<int, TeamInviteJoinInfo>;
            if (null == datas)
                return;

            m_obj2TeamIdMap.Clear();
            for (int i = 0; i < m_container.transform.childCount; ++ i)
            {
                GameObject.Destroy(m_container.transform.GetChild(i).gameObject);
            }

            foreach (TeamInviteJoinInfo  data in datas.Values)
            {
                this.AddItem(data);
            }
        }

        private void AddItem(TeamInviteJoinInfo info)
        {
            GameObject obj = GameObject.Instantiate(m_prefab);
            obj.transform.SetParent(m_container.transform);
            obj.SetActive(true);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.Find("AcceptBtn").GetComponent<Button>().onClick.AddListenerIfNoExist(() => { this.OnClickAcceptBtn(info.teamId, obj); });
            m_obj2TeamIdMap[obj] = info.teamId;

            List<TeamMemberData> memberDatas = TeamUtil.SortTeamMember(info.teamInfo);
            if (memberDatas.Count <= 0)
                return;
            TeamMemberData leaderData = memberDatas[0];
            obj.transform.Find("ContentText/Name").GetComponent<Text>().text = leaderData.name;
            obj.transform.Find("ContentText/Level").GetComponent<Text>().text = string.Format("{0}级", leaderData.level);
            obj.transform.Find("ContentText/HuoDongName").GetComponent<Text>().text = TeamUtil.GetGoalName(info.teamInfo.goalId);
            obj.transform.Find("ContentText/HuoDongLevel").GetComponent<Text>().text = string.Format("{0}-{1}级", info.teamInfo.limit.minLevel, info.teamInfo.limit.maxLevel);

            TeamProfSexResConfig Leadercfg = TeamUtil.GetProfSexResCfg(leaderData.prof, leaderData.sex);
            if (null != Leadercfg)
            {
                Image headIcon = obj.transform.Find("Head/Icon").GetComponent<Image>();
                Helper.SetSprite(headIcon, Leadercfg.headIcon);
                Image jobIcon = obj.transform.Find("Job").GetComponent<Image>();
                Helper.SetSprite(jobIcon, Leadercfg.profIcon);
            }

            for (int i = 0; i < TeamDef.MAX_MEMBER_COUNT; ++i)
            {
                string childPath = string.Format("TeamList/{0}", i);
                Transform memberObj = obj.transform.Find(childPath);
                if (i < memberDatas.Count)
                {
                    memberObj.GetComponent<StateRoot>().SetCurrentState(1, true);
                    TeamMemberData memberData = memberDatas[i];
                    Text memberLevelText = memberObj.Find("Level/Text").GetComponent<Text>();
                    memberLevelText.text = memberData.level.ToString();
                    Image memberJobIcon = memberObj.Find("Icon").GetComponent<Image>();
                    TeamProfSexResConfig cfg = TeamUtil.GetProfSexResCfg(leaderData.prof, leaderData.sex);
                    if (null != cfg)
                        Helper.SetSprite(memberJobIcon, cfg.profIcon);
                }
                else
                {
                    memberObj.GetComponent<StateRoot>().SetCurrentState(0, true);
                }
            }
        }

        protected override void OnHide()
        {
            m_isOPen = false;
        }
    }
}

#endif