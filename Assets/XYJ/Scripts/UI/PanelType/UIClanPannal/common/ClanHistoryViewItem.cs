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

namespace xys.UI
{
    public class ClanHistoryViewItem : ListViewItem
    {
        [SerializeField]
        Text m_timeTxt;
        [SerializeField]
        Text m_dataTxt;

        private ClanHistoryView m_listView;
        private ClanHistoryViewData m_itemData;
        public ClanHistoryViewData itemdData { get { return m_itemData; } }

        public ClanHistoryViewItem()
        {
            onClick.AddListener(OnClick);
        }
        public virtual void SetData(ClanHistoryView listView, ClanHistoryViewData newItem)
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
                DateTime itemTime = new DateTime(m_itemData.data.time);
                m_timeTxt.text = string.Format("{0}-{1}-{2}", itemTime.Year, itemTime.Month, itemTime.Day);
                m_dataTxt.text = m_itemData.data.data;
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