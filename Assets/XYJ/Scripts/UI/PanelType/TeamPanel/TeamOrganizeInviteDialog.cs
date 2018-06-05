#if !USE_HOT
using NetProto;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.Team;
using xys.UI;
using xys.UI.State;
using NetProto.Hot;

namespace xys.hot.UI
{
    [AutoILMono]
    class TeamOrganizeInviteDialog
    {
        [SerializeField]
        GameObject m_prefab;
        [SerializeField]
        Button m_friendBtn;
        [SerializeField]
        Button m_guildBtn;
        [SerializeField]
        Button m_nearbyBtn;
        [SerializeField]
        GameObject m_content;
        [SerializeField]
        GameObject m_emptyView;

        System.Action<long> m_inviteBtnCb;

        const int SHOWING_TYPE_INVALID = 0;
        const int SHOWING_TYPE_FRIEND = 1;
        const int SHOWING_TYPE_GUILD = 2;
        const int SHOWING_TYPE_NEARBY = 3;
        const int SHOWING_TYPE_MAX = 4;
        int m_showingType = SHOWING_TYPE_FRIEND;

        Button []m_buttons = new Button[SHOWING_TYPE_MAX];
        List<ItemData> m_itemDatas = new List<ItemData>();

        class ItemData
        {
            public long uid;
            public int prof = 0;
            public int sex = 0;
            public int level = 0;
            public string name = null;
            public GameObject go = null;
        }

        public void Init(System.Action<long> allowBtnCb)
        {
            m_inviteBtnCb = allowBtnCb;

            m_friendBtn.onClick.AddListenerIfNoExist(()=> { this.OnClickLeftSideBtn(m_friendBtn); });
            m_guildBtn.onClick.AddListenerIfNoExist(() => { this.OnClickLeftSideBtn(m_guildBtn); });
            m_nearbyBtn.onClick.AddListenerIfNoExist(() => { this.OnClickLeftSideBtn(m_nearbyBtn); });

            m_buttons[SHOWING_TYPE_FRIEND] = m_friendBtn;
            m_buttons[SHOWING_TYPE_GUILD] = m_guildBtn;
            m_buttons[SHOWING_TYPE_NEARBY] = m_nearbyBtn;
        }

        public void Show()
        {
            this.OnClickLeftSideBtn(m_friendBtn);
        }

        public void OnClickLeftSideBtn(Button btn)
        {
            for (int i = SHOWING_TYPE_INVALID + 1; i < m_buttons.Length; ++ i)
            {
                if (btn == m_buttons[i])
                {
                    m_showingType = i;
                    m_buttons[i].GetComponent<StateRoot>().SetCurrentState(1, false);
                }
                else
                {
                    m_buttons[i].GetComponent<StateRoot>().SetCurrentState(0, false);
                }
            }

            EmptyContent();
            this.QueryData();
        }

        private void QueryData()
        {
            switch (m_showingType)
            {
                case SHOWING_TYPE_FRIEND:
                    {
                        App.my.eventSet.FireEvent<long>(EventID.Friend_GetFriendData, App.my.localPlayer.charid);
                    }
                    break;
                case SHOWING_TYPE_GUILD:
                    break;
                case SHOWING_TYPE_NEARBY:
                    {
                        ES_QueryNearbyUser evData = new ES_QueryNearbyUser();
                        evData.queryReason = QueryNearbyUserReason.TeamOrganizeInvite;
                        App.my.eventSet.FireEvent<ES_QueryNearbyUser>(EventID.Team_ReqQueryNearbyUser, evData);
                    }
                    break;
            }
        }

        public void OnQueryNearbyUserRsp(QueryNearbyUserRsp ret)
        {
            if (QueryNearbyUserReason.TeamOrganizeInvite != ret.queryReason)
                return;

            if (SHOWING_TYPE_NEARBY != m_showingType)
                return;

            List<ItemData> itemDatas = new List<ItemData>();
            foreach (NearbyUserData userData in ret.datas)
            {
                ItemData itemData = new ItemData();
                itemData.uid = userData.uid;
                itemData.sex = userData.sex;
                itemData.prof = userData.prof;
                itemData.level = userData.level;
                itemData.name = userData.name;
                itemDatas.Add(itemData);
            }

            this.SetContent(itemDatas);
        }

        public void OnQueryFriendRsp(Dictionary<long,FriendItemInfo> msg)
        {
            Dictionary<long, FriendItemInfo> friendInfos = msg;
            if (null == friendInfos)
                return;

            TeamAllTeamInfo teamAllInfo =TeamUtil.teamMgr.TeamAllInfo;
            List<ItemData> itemDatas = new List<ItemData>();
            foreach (FriendItemInfo info in friendInfos.Values)
            {
                if (!info.isOnline)
                    continue;

                if (teamAllInfo.members.ContainsKey(info.charid))
                    continue;

                ItemData itemData = new ItemData();
                itemData.uid = info.charid;
                itemData.sex = info.sex;
                itemData.prof = info.job;
                itemData.level = info.level;
                itemData.name = info.name;
                itemDatas.Add(itemData);
            }

            this.SetContent(itemDatas);
        }

        private void OnClickAllow(ItemData data)
        {
            if (null != m_inviteBtnCb)
            {
                m_inviteBtnCb(data.uid);
            }
        }

        private void EmptyContent()
        {
            for (int i = 0; i < m_content.transform.childCount; ++i)
            {
                GameObject.Destroy(m_content.transform.GetChild(i).gameObject);
            }

            m_itemDatas = new List<ItemData>();
            m_emptyView.SetActive(true);
        }
        private void SetContent(List<ItemData> datas)
        {
            this.EmptyContent();

            m_itemDatas = datas;
            foreach (ItemData data in datas)
            {
                GameObject item = GameObject.Instantiate(m_prefab);
                data.go = item;
                item.transform.SetParent(m_content.transform);
                item.SetActive(true);
                item.transform.localScale = Vector3.one;

                item.transform.Find("Name").GetComponent<Text>().text = data.name;
                item.transform.Find("Level").GetComponent<Text>().text = string.Format("{0}级", data.level);
                item.transform.Find("InviteBtn").GetComponent<Button>().onClick.AddListenerIfNoExist(() => { OnClickAllow(data); });
                TeamProfSexResConfig resCfg = TeamUtil.GetProfSexResCfg(data.prof, data.sex);
                if (null != resCfg)
                {
                    Helper.SetSprite(item.transform.Find("Head/Icon").GetComponent<Image>(), resCfg.headIcon);
                    Helper.SetSprite(item.transform.Find("Job").GetComponent<Image>(), resCfg.profIcon);
                }
            }

            m_emptyView.SetActive(0 == datas.Count);
        }

        public void OnNewMemberEnterTeam(long uid)
        {
             foreach (ItemData data in m_itemDatas)
            {
                if (data.uid == uid && null != data.go)
                    data.go.SetActive(false);
            }
        }
    }
}

#endif