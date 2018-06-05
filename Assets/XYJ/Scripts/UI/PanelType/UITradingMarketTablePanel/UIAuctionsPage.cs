#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;

namespace xys.hot.UI
{
    class UIAuctionsPage : HotTablePageBase
    {

        HotTablePage m_Parent;


        public UIAuctionsPage() : base(null)
        {

        }

        public UIAuctionsPage(HotTablePage parent) : base(parent)
        {
            this.m_Parent = parent;
        }

        protected override void OnInit()
        {
            
        }

        protected override void OnShow(object p)
        {
            
        }
    }

}
#endif