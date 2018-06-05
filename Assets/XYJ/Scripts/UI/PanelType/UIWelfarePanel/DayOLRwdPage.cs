#if !USE_HOT
using System.Collections.Generic;
using UnityEngine;
using xys.hot.UI;
using xys.UI;
using Config;
using UnityEngine.UI;

namespace xys.hot.UI
{
    class DayOLRwdPage : HotTablePageBase
    {
        int rewardTimes = 5;
        int rewardNum = 5;

        [SerializeField]
        ILMonoBehaviour m_ILScrollView;
        DayOLScrollView m_ScrollView;
        [SerializeField]
        ScrollRect m_Scroll;

        HotTablePage m_Page;
        DayOLRwdPage() : base(null) { }
        DayOLRwdPage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
            if (m_ILScrollView != null)
                m_ScrollView = (DayOLScrollView)m_ILScrollView.GetObject();
            
        }
        protected override void OnShow(object args)
        {
            Event.Subscribe<int>(EventID.Welfare_DayOLRwdReceived, m_ScrollView.DisableColumn);
            m_Scroll.content.localPosition =new Vector3 (m_Scroll.content.localPosition.x,0,0);
            Event.Subscribe(EventID.Welfare_RefreshUI, m_ScrollView.RefreshUI);
            m_ScrollView.RefreshUI();
        }
    }
}

#endif