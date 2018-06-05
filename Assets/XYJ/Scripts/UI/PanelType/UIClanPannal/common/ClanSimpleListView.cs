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
    public class ClanSimpleUserData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long ClanId
        {
            get { return null != data ? data.clanid : 0; }
        }

        bool m_isSelected;
        public bool isSelected
        {
            get { return m_isSelected; }
            set { m_isSelected = value; NotifyChange("isSelected"); }
        }

        ClanSimpleListViewItem.ShowType m_showType;

        public int m_isDeepColorBg;
        public ClanSimpleListViewItem.ShowType showType
        {
            get { return m_showType; }
            set { m_showType = value; }
         }

        ClanDbData m_ItemData;
        public ClanDbData data
        {
            get { return m_ItemData; }
            set { m_ItemData = value; NotifyChange("data"); }
        }

        public void NotifyChange(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ClanSimpleListView : ListViewCustom<ClanSimpleListViewItem, ClanSimpleUserData>
    {
        protected override void SetData(ClanSimpleListViewItem component, ClanSimpleUserData itemData)
        {
            component.SetData(this, itemData);
        }

        protected override void HighlightColoring(ClanSimpleListViewItem component)
        {

        }

        protected override void SelectColoring(ClanSimpleListViewItem component)
        {

        }

        protected override void DefaultColoring(ClanSimpleListViewItem component)
        {

        }

        public void OnSelectItem(ClanSimpleListViewItem component)
        {
            if (null != SelectItemHandler)
                SelectItemHandler(this, component);

            ClanSimpleUserData selectedData = component.itemdData;
            foreach (var itemData in this.DataSource)
            {
                if (itemData.ClanId == selectedData.ClanId)
                    continue;
                if (itemData.isSelected)
                    itemData.isSelected = false;
            }
        }

        public delegate void SelectItemEventHandler(ClanSimpleListView listView, ClanSimpleListViewItem item);
        public event SelectItemEventHandler SelectItemHandler;
    }
}
