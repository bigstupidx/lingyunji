#if !USE_HOT
using System;
using xys.UI;

namespace xys.hot.UI
{
    class PackagePanel : HotTablePanelBase
    {
        public PackagePanel() : base(null)
        {

        }

        public PackagePanel(xys.UI.HotTablePanel parent) : base(parent)
        {

        }

        protected override bool OnPreChange(HotTablePage page)
        {
            if(page.pageType == "FashionPage")
            {
                int lv = int.Parse(Config.kvCommon.Get("DemonplotOpenLv").value);
                if (App.my.localPlayer.levelValue < lv)
                {
                    SystemHintMgr.ShowHint(string.Format(Config.TipsContent.GetByName("demonplot_playerLv_error").des, lv));
                    return false;
                }
            }
            return base.OnPreChange(page);
        }

        protected override void OnShow(object p)
        {

        }

        protected override void OnInit()
        {

        }
    }
}
#endif