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
    public class ListViewMailItemComponent : ListViewItem
    {
        [SerializeField]
        Text m_title;
        [SerializeField]
        StateRoot m_selectedState;
        [SerializeField]
        Text m_time;
        [SerializeField]
        StateRoot m_iconState;
        [SerializeField]
        GameObject m_box;

        ListViewMailItemDescription m_itemData = null;
        ListViewMailItem m_listView = null;

        public ListViewMailItemDescription itemdData { get { return m_itemData; } }

        public ListViewMailItemComponent()
        {
            onClick.AddListener(OnClick);
        }
        public virtual void SetData(ListViewMailItem listView, ListViewMailItemDescription newItem)
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
            if (null == m_itemData || null == m_itemData.mail)
                return;

            MailDbRecord mail = m_itemData.mail;
            m_title.text = mail.title;
            DateTime createDate = new DateTime(mail.createTick);
            m_time.text = createDate.ToString("yyyy-MM-dd");
            m_selectedState.SetCurrentState(m_itemData.isSelected ? 1 : 0, false);
            m_iconState.SetCurrentState(mail.isRead ? 1 : 0, false);
            bool isShowBox = (null != mail.attachments && mail.attachments.Count > 0 && !mail.isFetchedAttachments);
            m_box.SetActive(isShowBox);
        }

        public void OnClick()
        {
            m_itemData.isSelected = true;
            if (null != m_listView)
                m_listView.OnSelectItem(this);
        }
    }
}
