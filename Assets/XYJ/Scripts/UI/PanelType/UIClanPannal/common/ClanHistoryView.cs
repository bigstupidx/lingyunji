using NetProto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UIWidgets;
using UnityEngine;

namespace xys.UI
{
    public class ClanHistoryViewData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long HistoryId
        {
            get { return null != data ? data.id : 0; }
        }


        private ClanHistoryData m_data = null;

        public ClanHistoryData data
        {
            get { return m_data; }
            set { m_data = value; NotifyChange("data"); }
        } 

        bool m_isSelected;
        public bool isSelected
        {
            get { return m_isSelected; }
            set { m_isSelected = value; NotifyChange("isSelected"); }
        }

        public void NotifyChange(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ClanHistoryView : ListViewCustom<ClanHistoryViewItem, ClanHistoryViewData>
    {
        protected override void SetData(ClanHistoryViewItem component, ClanHistoryViewData itemData)
        {
            component.SetData(this, itemData);
        }

        public void OnSelectItem(ClanHistoryViewItem component)
        {
            if (null != SelectItemHandler)
                SelectItemHandler(this, component);

            ClanHistoryViewData selectedData = component.itemdData;
            foreach (var itemData in this.DataSource)
            {
                if (itemData.HistoryId == selectedData.HistoryId)
                    continue;
                if (itemData.isSelected)
                    itemData.isSelected = false;
            }
        }

        public delegate void SelectItemEventHandler(ClanHistoryView listView, ClanHistoryViewItem item);
        public event SelectItemEventHandler SelectItemHandler;
    }
}
