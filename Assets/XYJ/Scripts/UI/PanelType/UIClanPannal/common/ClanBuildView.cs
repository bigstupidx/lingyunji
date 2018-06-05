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
    public class ClanBuildViewData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ClanBuildDbData m_data = null;

        public int id
        {
            get { return data != null? data.type:0; }
        }
        public ClanBuildDbData data
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

    public class ClanBuildView : ListViewCustom<ClanBuildViewItem, ClanBuildViewData>
    {
        protected override void SetData(ClanBuildViewItem component, ClanBuildViewData itemData)
        {
            component.SetData(this, itemData);
        }

        public void OnSelectItem(ClanBuildViewItem component)
        {
            if (null != SelectItemHandler)
                SelectItemHandler(this, component);

            ClanBuildViewData selectedData = component.itemdData;
            foreach (var itemData in this.DataSource)
            {
                
                if (itemData.id == selectedData.id)
                    continue;
                if (itemData.isSelected)
                    itemData.isSelected = false;
                    
            }
        }

        public delegate void SelectItemEventHandler(ClanBuildView listView, ClanBuildViewItem item);
        public event SelectItemEventHandler SelectItemHandler;
    }
}
