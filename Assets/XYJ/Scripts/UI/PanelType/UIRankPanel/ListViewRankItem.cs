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
    public class ListViewRankItem : ListViewItem
    {
        [SerializeField]
        Text field0;
        [SerializeField]
        Text field1;
        [SerializeField]
        Text field2;
        [SerializeField]
        Text field3;
        [SerializeField]
        Text field4;
        [SerializeField]
        StateRoot stateRoot;

        ListViewRankItemData m_data;
        public ListViewRankItemData data { get { return m_data; } protected set { m_data = value; } }
        ListViewRankItems m_container;

        public ListViewRankItem()
        {
            onClick.AddListener(this.OnClick);
        }

        public virtual void SetData(ListViewRankItems container, ListViewRankItemData newItem)
        {
            if (null != m_data)
                m_data.PropertyChanged -= OnPropertyChanged;
            m_data = newItem;
            m_data.PropertyChanged += OnPropertyChanged;

            m_container = container;
            this.UpdateUI();
        }
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateUI();
        }
        public void OnClick()
        {
            if (null != m_data && null != m_container)
                m_container.OnSelectItem(this);
        }

        const int UnSelectedState = 0;
        const int IsMarkState = 1;
        const int SelectedState = 2;
        protected void UpdateUI()
        {
            if (null == m_data)
                return;

            field0.text = m_data.field0;
            field1.text = m_data.field1;
            field2.text = m_data.field2;
            field3.text = m_data.field3;
            field4.text = m_data.field4;

            int state = UnSelectedState;
            if (m_data.isSelected)
                state = SelectedState;
            else if (m_data.isMark)
                state = IsMarkState;
            stateRoot.SetCurrentState(state, false);
        }
    }
}
