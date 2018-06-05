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
    public class ListViewMailItemDescription : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long id
        {
            get { return null != mail ? mail.id : 0; }
        }

        bool m_isSelected;
        public bool isSelected
        {
            get { return m_isSelected; }
            set { m_isSelected = value; NotifyChange("isSelected"); }
        }

        MailDbRecord m_mail;
        public MailDbRecord mail
        {
            get { return m_mail; }
            set { m_mail = value; NotifyChange("mail"); }
        }

        public void NotifyChange(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ListViewMailItem : ListViewCustom<ListViewMailItemComponent, ListViewMailItemDescription>
    {
        protected override void SetData(ListViewMailItemComponent component, ListViewMailItemDescription item)
        {
            component.SetData(this, item);
        }

        protected override void HighlightColoring(ListViewMailItemComponent component)
        {

        }

        protected override void SelectColoring(ListViewMailItemComponent component)
        {

        }

        protected override void DefaultColoring(ListViewMailItemComponent component)
        {

        }

        public void OnSelectItem(ListViewMailItemComponent component)
        {
            if (null != SelectItemHandler)
                SelectItemHandler(this, component);

            ListViewMailItemDescription selectedData = component.itemdData;
            foreach (var itemData in this.DataSource)
            {
                if (itemData.id == selectedData.id)
                    continue;
                if (itemData.isSelected)
                    itemData.isSelected = false;
            }
        }

        public delegate void SelectItemEventHandler(ListViewMailItem listView, ListViewMailItemComponent item);
        public event SelectItemEventHandler SelectItemHandler;
    }
}
