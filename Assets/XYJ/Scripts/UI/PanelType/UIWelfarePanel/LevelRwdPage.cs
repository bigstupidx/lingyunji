#if !USE_HOT
using Config;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.hot.UI;
using xys.UI;

namespace xys.hot.UI
{
    class LevelRwdPage : HotTablePageBase
    {
        int rewardTimes = 5;
        int rewardNum = 5;

        [SerializeField]
        ILMonoBehaviour m_ILScrollView;
        LVScrollView m_ScrollView;
        [SerializeField]
        ScrollRect m_Scroll;

        List<int> receivedAwdList = new List<int>();
        List<int> unreceivedAwdList = new List<int>();
        LevelRwdPage() : base(null)
        {

        }

        LevelRwdPage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
            if (m_ILScrollView != null)
                m_ScrollView = (LVScrollView)m_ILScrollView.GetObject();
        }

        protected override void OnShow(object args)
        {
            RegEvent();
            m_ScrollView.RefreshUI();
            m_Scroll.content.localPosition = new Vector3(m_Scroll.content.localPosition.x, 0, 0);
        }

        void RegEvent()
        {
            Event.Subscribe(NetProto.AttType.AT_Level, m_ScrollView.RefreshRwdStatus);
            Event.Subscribe<int>(EventID.Welfare_LVRwdReceived, m_ScrollView.DisableColumn);
            Event.Subscribe(EventID.Welfare_RefreshUI, m_ScrollView.RefreshUI);
        }
    }
}

#endif