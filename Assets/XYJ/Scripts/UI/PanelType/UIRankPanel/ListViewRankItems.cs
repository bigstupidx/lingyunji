using NetProto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UIWidgets;

namespace xys.UI
{
    public class ListViewRankItemData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Action<ListViewRankItemData> onClickCb { get; set; }

        bool m_isSelected;
        public bool isSelected
        {
            get { return m_isSelected; }
            set { m_isSelected = value; NotifyChange("isSelected"); }
        }

        public string field0 { get; set; }
        public string field1 { get; set; }
        public string field2 { get; set; }
        public string field3 { get; set; }
        public string field4 { get; set; }
        public bool isMark { get; set; }
        public RankType rankType { get; set; }
        public long id { get; set; }
        public long keyParam { get; set; }
        public long rankOrder { get; set; }
        public RankDbPlayerData playerRankData { get; set; }
        public RankDbGuildData guildRankData { get; set; }

    public void NotifyChange(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ListViewRankItems : ListViewCustom<ListViewRankItem, ListViewRankItemData>
    {
        protected override void SetData(ListViewRankItem comp, ListViewRankItemData item)
        {
            comp.SetData(this, item);
        }
        public void OnSelectItem(ListViewRankItem comp)
        {
            foreach (ListViewRankItemData data in this.DataSource)
            {
                if (!ReferenceEquals(comp.data, data))
                {
                    data.isSelected = false;
                }
            }
            comp.data.isSelected = true;
            if (null != comp.data.onClickCb)
            {
                comp.data.onClickCb(comp.data);
            }
        }

        protected override void HighlightColoring(ListViewRankItem component) { }
        protected override void SelectColoring(ListViewRankItem component) { }
        protected override void DefaultColoring(ListViewRankItem component) { }
    }
}
