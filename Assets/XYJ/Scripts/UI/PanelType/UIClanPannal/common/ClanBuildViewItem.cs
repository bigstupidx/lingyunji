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
    public class ClanBuildViewItem : ListViewItem
    {
        
        [SerializeField]
        Image m_buildIcon;
        [SerializeField]
        Text m_buildName;
        [SerializeField]
        Transform m_selectTransform;

        private ClanBuildView m_listView;
        private ClanBuildViewData m_itemData;
        public ClanBuildViewData itemdData { get { return m_itemData; } }

        public ClanBuildViewItem()
        {
            onClick.AddListener(OnClick);
        }
        public virtual void SetData(ClanBuildView listView, ClanBuildViewData newItem)
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

                m_buildName.text = ClanCommon.GetBuildName(m_itemData.id);

                if (m_itemData.isSelected)
                {
                    m_selectTransform.gameObject.SetActive(true);
                }
                else
                {
                    m_selectTransform.gameObject.SetActive(false);
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