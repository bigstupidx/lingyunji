using Config;
using NetProto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;

namespace xys.UI
{
    public class ClanSimpleListViewItem : ListViewItem
    {
        public enum ShowType
        {         
            Clan_Respone = 0,
            Clan_Join = 1,
        }

        
        [SerializeField]
        Text m_clanId;
        [SerializeField]
        Text m_clanName;
        [SerializeField]
        Text m_clanLevel;
        [SerializeField]
        Text m_clanCount;
        [SerializeField]
        Text m_clanLeader;
        [SerializeField]
        StateRoot m_stateRoot;
        [SerializeField]
        StateRoot m_showTypeStateRoot;

        [SerializeField]
        StateRoot m_BgStateRoot;

        ClanSimpleUserData m_itemData = null;
        ClanSimpleListView m_listView = null;


         

        public ClanSimpleUserData itemdData { get { return m_itemData; } }

        public ClanSimpleListViewItem()
        {
            onClick.AddListener(OnClick);
        }
        public virtual void SetData(ClanSimpleListView listView, ClanSimpleUserData newItem)
        {
            if (null != m_itemData)
                m_itemData.PropertyChanged -= OnPropertyChanged;
            m_itemData = newItem;
            m_itemData.PropertyChanged += OnPropertyChanged;

            m_listView = listView;
            this.UpdateUI();
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateUI();
        }

        protected void UpdateUI()
        {
            if (m_itemData != null)
            {
                m_clanId.text = m_itemData.data.clanid.ToString();
                m_clanName.text = m_itemData.data.name;
                
                if (m_itemData.isSelected)
                {
                    m_stateRoot.SetCurrentState(1, true);
                }
                else
                {
                    m_stateRoot.SetCurrentState(0, true);
                }

                if (m_itemData.showType == ShowType.Clan_Join)
                {
                    m_showTypeStateRoot.SetCurrentState(0, true);
                    m_clanLevel.text = m_itemData.data.clanlevel.ToString();
                    m_clanCount.text = string.Format("{0}/{1}", m_itemData.data.membercount, 100);
                    ClanMgr clanMgr = (ClanMgr)App.my.localPlayer.GetModule<ClanModule>().clanMgr;
                    m_clanLeader.text = clanMgr.GetLeaderName(m_itemData.data.clanid);
                }
                else
                {
                    m_showTypeStateRoot.SetCurrentState(1, true);
                    ClanMgr clanMgr = (ClanMgr)App.my.localPlayer.GetModule<ClanModule>().clanMgr;
                    m_clanLevel.text = clanMgr.GetLeaderName(m_itemData.data.clanid);        //响应招募列表中 level 表示族长
                    int responeLimit = kvCommon.Get("guildFoundMembers").value.ToInt32();
                    m_clanCount.text = string.Format("{0}/{1}", m_itemData.data.membercount, responeLimit);

                    if (m_itemData.data.createtime > 0)
                    {
                        int timeLimit = kvCommon.Get("guildAnswerDissolveTime").value.ToInt32();
                        DateTime createTime = new DateTime(m_itemData.data.createtime);
                        DateTime finishiTime = createTime.AddHours(timeLimit);
                        m_clanLeader.text = (finishiTime - DateTime.Now).ToString().Substring(0, 8);                 //m_itemData.data.createtime.ToString();
                    }
                }
                
                ClanDbData data = ((ClanMgr)App.my.localPlayer.GetModule<ClanModule>().clanMgr).GetSelfCurClan();
                if (data != null)
                {
                    if (data.clanid == m_itemData.ClanId)
                    {
                        m_BgStateRoot.SetCurrentState(2, true);
                    }
                }
                else
                {
                    m_BgStateRoot.SetCurrentState(m_itemData.m_isDeepColorBg, true);
                }
                
            }     
        }

        public void OnClick()
        {
            m_itemData.isSelected = true;           
            if (null != m_listView)
                m_listView.OnSelectItem(this);
        }

        public void RecvSelfClanData(ClanDbData data )
        {
            if (data != null)
            {
                if (data.clanid == m_itemData.ClanId)
                {
                    m_BgStateRoot.SetCurrentState(2, true);
                }
            }
            else
            {
                m_BgStateRoot.SetCurrentState(m_itemData.m_isDeepColorBg, true);
            }
        }
    }
}
