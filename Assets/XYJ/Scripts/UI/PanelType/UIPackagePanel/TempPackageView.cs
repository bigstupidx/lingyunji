namespace xys.UI
{
    using UIWidgets;
    using UnityEngine;
    using System.Collections.Generic;

    public class TempPackageView : HotTileView
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public void SetDataList(List<NetProto.ItemGrid> grids)
        {
            refType.InvokeMethod("SetDataList", grids);
        }
    }
}
