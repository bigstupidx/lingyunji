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
    public class ClanMemberViewData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ClanUser m_data = null;

        public long id
        {
            get { return data != null ? data.charId : 0; }
        }
        public ClanUser data
        {
            get { return m_data; }
            set { m_data = value; NotifyChange("data"); }
        }

        private int m_type = 0;
        public int showType
        {
            get { return m_type; }
            set { m_type = value; NotifyChange("showType"); }
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

    public class ClanMemberView : ListViewCustom<ClanMemberViewItem, ClanMemberViewData>
    {
        protected override void SetData(ClanMemberViewItem component, ClanMemberViewData itemData)
        {
            component.SetData(this, itemData);
        }

        public void OnSelectItem(ClanMemberViewItem component)
        {
            if (null != SelectItemHandler)
                SelectItemHandler(this, component);

            ClanMemberViewData selectedData = component.itemdData;
            foreach (var itemData in this.DataSource)
            {

                if (itemData.id == selectedData.id)
                    continue;
                if (itemData.isSelected)
                    itemData.isSelected = false;

            }
        }

        public delegate void SelectItemEventHandler(ClanMemberView listView, ClanMemberViewItem item);
        public event SelectItemEventHandler SelectItemHandler;
    }
}
