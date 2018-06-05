using NetProto;
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
using xys.UI.Clan;

namespace xys.UI
{
    public class ClanMemberViewItem : ListViewItem
    {
        [SerializeField]
        Image m_selectImg;
        [SerializeField]
        Image m_selfImg;
        [SerializeField]
        Text m_name;
        [SerializeField]
        Text m_post;
        [SerializeField]
        Text m_level;
        [SerializeField]
        Text m_job;
        [SerializeField]
        Text m_rightText1;
        [SerializeField]
        Text m_rightText2;
        [SerializeField]
        Text m_rightText3;

        private ClanMemberView m_listView;
        private ClanMemberViewData m_itemData;
        public ClanMemberViewData itemdData { get { return m_itemData; } }

        public ClanMemberViewItem()
        {
            onClick.AddListener(OnClick);
        }
        public virtual void SetData(ClanMemberView listView, ClanMemberViewData newItem)
        {
            if (null != m_itemData)
                m_itemData.PropertyChanged -= OnPropertyChanged;
            m_itemData = newItem;
            m_itemData.PropertyChanged += OnPropertyChanged;

            m_listView = listView;
            RefreshUI();
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshUI();
        }



        protected void RefreshUI()
        {
            if (m_itemData != null)
            {
                m_name.text = m_itemData.data.name;
                m_post.text = ClanCommon.GetPostname(m_itemData.data.post);
                m_level.text = m_itemData.data.level.ToString();
                RoleJob roleJob = RoleJob.Get((int)m_itemData.data.job);
                m_job.text = roleJob.name;

                if (m_itemData.data.charId == App.my.localPlayer.charid)
                {
                    m_selfImg.gameObject.SetActive(true);
                }
                else
                {
                    m_selfImg.gameObject.SetActive(false);
                }

                if (m_itemData.isSelected)
                {
                    m_selectImg.gameObject.SetActive(true);
                }
                else
                {
                    m_selectImg.gameObject.SetActive(false);
                }

                if (m_itemData.showType == 0)
                {
                    m_rightText1.text = "11111111111111";
                }
                else
                {
                    m_rightText1.text = string.Format("{0}/{1}/{2}", m_itemData.data.clanContributionNow, m_itemData.data.clanContributionWeek, m_itemData.data.clanContributionAll);
                   
                }
            }
        }
        public void OnClick()
        {
            m_itemData.isSelected = true;
            if (null != m_listView)
                m_listView.OnSelectItem(this);
        }
    }
}