#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{

    class UIFacePage : HotTablePageBase
    {
        public UIFacePage() : base(null) { }
        public UIFacePage(HotTablePage parent) : base(parent)
        {
        }

        protected override void OnInit()
        {

        }
        protected override void OnShow(object p)
        {
            //UIAppearancePanel appPanel = panel as UIAppearancePanel;
            //Debug.Log(appPanel.m_roletempData.m_clothId);
            panel.tableParent.Hide(false);
            App.my.uiSystem.ShowPanel(PanelType.UIFaceMakePanel, null);
        }
    }
}
#endif