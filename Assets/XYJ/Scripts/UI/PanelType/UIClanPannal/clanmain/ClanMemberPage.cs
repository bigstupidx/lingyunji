#if !USE_HOT
using NetProto;
using NetProto.Hot;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class ClanMemberPage : HotTablePageBase
    {
        [SerializeField]
        Button m_applyListBtn;
        [SerializeField]
        Button m_sendMsgGroupBtn;
        [SerializeField]
        Button m_clanListBtn;
        [SerializeField]
        Button m_outClanBtn;
        [SerializeField]
        Button m_rightBtn;
        [SerializeField]
        Text m_tipsTxt;
        [SerializeField]
        ClanMemberView m_memberView;

        [SerializeField]
        StateRoot m_titleRoot;

        private int m_type = 1;

        private ClanDbData m_curData = null;

        private ClanMemberViewData m_curMemberData = null;
        ClanMemberPage() : base(null) { }
        public ClanMemberPage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
            m_applyListBtn.onClick.AddListenerIfNoExist(()=> {


            });

            m_sendMsgGroupBtn.onClick.AddListenerIfNoExist(()=> {


            });

            m_clanListBtn.onClick.AddListenerIfNoExist(()=> {

            });

            m_outClanBtn.onClick.AddListenerIfNoExist(()=> {

            });

            m_rightBtn.onClick.AddListenerIfNoExist(()=> {
                if (m_type == 0)
                {
                    m_type = 1;                 
                }
                else
                {
                    m_type = 0;               
                }
                m_titleRoot.SetCurrentState(m_type,true);
                SetMemberListShowType(m_type);
            });
        }

        public void SetMemberListShowType(int showType)
        {
            foreach (var item in m_memberView.DataSource)
            {
                item.showType = showType;
            }
        }

        public void ResetMemberList()
        {
            if (m_curData != null && m_memberView != null)
            {
                List<ClanMemberViewData> clanItems = new List<ClanMemberViewData>();
                clanItems.Clear();
                if (m_curData != null)
                {
                    int idx = 0;
                    foreach (var item in m_curData.member.membermap)
                    {
                        ClanMemberViewData memberData = new ClanMemberViewData();
                        memberData.data = item.Value;
                        if (idx == 0)
                        {
                            memberData.isSelected = true;
                            m_curMemberData = memberData;
                         
                        }
                        clanItems.Add(memberData);
                        idx++;
                    }
                    m_memberView.DataSource = new UIWidgets.ObservableList<ClanMemberViewData>(clanItems);
                }
            }

        }

        protected override void OnShow(object args)
        {
            Event.Subscribe<ClanDbData>(EventID.Clan_RecvSelfClan, this.RefreshUI);
            Event.fireEvent(EventID.Clan_GetSelfClan);
        }

        public void RefreshUI(ClanDbData data)
        {
            if (data != null)
            {
                m_curData = null;
                m_curData = data;
                ResetMemberList();
            }
        }

        protected override void OnHide()
        {

        }
    }
}

#endif