namespace xys.UI
{
    using UIWidgets;
    using UnityEngine;
    using System.Collections.Generic;

    public class PackageView : HotTileView
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public int openGridNum { set { refType.SetProperty("openGridNum", value); } }

        public void SetDataList(List<NetProto.ItemGrid> grids)
        {
            refType.InvokeMethod("SetDataList", grids);
        }
    }
}
