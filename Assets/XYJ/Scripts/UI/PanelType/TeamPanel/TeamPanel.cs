#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using xys.UI;

namespace xys.hot.UI
{
    class TeamPanel : HotPanelBase
    {
        xys.UI.HotTablePanel m_Parent;

        public TeamPanel() : base(null) { }
        public TeamPanel(xys.UI.UIHotPanel parent) : base(parent)
        {
        }

        //初始化
        protected override void OnInit()
        {
            Debuger.Log(this.GetType() + "Oninit");
            //

        }

        protected override IEnumerator OnInitSync()
        {
            Debuger.Log(this.GetType() + "OnInitSync");
            yield return 0;
        }
        //显示
        protected override void OnShow(object args)
        {
            Debuger.Log(this.GetType() + "OnShow");
        }

        protected override IEnumerator OnShowSync(object args)
        {
            Debuger.Log(this.GetType() + "OnShowSync");
            yield return 0;
        }
        //隐藏
        protected override void OnHide()
        {
            Debuger.Log(this.GetType() + "OnHide");
        }
    }
}
#endif