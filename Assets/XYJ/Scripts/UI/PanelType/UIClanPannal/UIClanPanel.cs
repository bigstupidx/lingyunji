#if !USE_HOT
using System;
using UnityEngine;
using UnityEngine.UI;
using xys.battle;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class UIClanPanel : HotTablePanelBase
    {    
        xys.UI.HotTablePanel m_Parent;

        EventAgent m_EventAgent { get; set; }
        public object transform { get; private set; }

        UIClanPanel() : base(null) { }

        UIClanPanel(xys.UI.HotTablePanel parent) : base(parent)
        {
            this.m_Parent = parent as xys.UI.HotTablePanel;
        }
        protected override void OnInit()
        {
            this.m_EventAgent = this.m_Parent.Event;
        }

        protected override void OnShow(object args)
        {

        }
        protected override void OnHide()
        {

        }
    }

}
#endif