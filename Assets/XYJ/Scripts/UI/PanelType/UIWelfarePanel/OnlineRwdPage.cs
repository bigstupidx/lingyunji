#if !USE_HOT
using xys.UI;
using UnityEngine;
using UnityEngine.UI;

namespace xys.hot.UI
{
    class OnlineRwdPage : HotTablePageBase
    {
        int totalCountDown = 10; //in seconds
        int rewardTimes = 5;
        int rewardNum = 5;

        [SerializeField]
        ScrollRect m_Scroll;
        [SerializeField]
        ILMonoBehaviour m_ILScrollView;
        OLScrollView m_ScrollView;
        OnlineRwdPage() : base(null) { }
        OnlineRwdPage(HotTablePage page) : base(page) { }

        protected override void OnInit()
        {
            if (m_ILScrollView != null)
                m_ScrollView = (OLScrollView)m_ILScrollView.GetObject();
        }

        protected override void OnShow(object args)
        {
            Event.Subscribe(EventID.Welfare_OLRwdReceived, m_ScrollView.DisableColumn);
            Event.Subscribe(EventID.Welfare_RefreshUI, m_ScrollView.RefreshUI);
            m_ScrollView.RefreshUI();
            m_Scroll.content.localPosition = new Vector3(m_Scroll.content.localPosition.x, 0, 0);
        }
    }
}

#endif