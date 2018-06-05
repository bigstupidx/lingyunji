#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using xys.UI;
    using xys.UI.State;
    using Config;
    using battle;
    using System;

    class UITrumpIdentifyPage : HotTablePageBase
    {
        [SerializeField]
        UITrumpIdentifyScroll m_ScrollView;
        [SerializeField]
        UITrumpIdentifyInfos m_Infos;

        UITrumpIdentifyPage() : base(null) { }
        public UITrumpIdentifyPage(HotTablePage parent) : base(parent) {  }

        protected override void OnInit()
        {
            //法宝信息表初始化
            m_Infos.OnInit();

            //法宝图鉴列表初始化
            m_ScrollView.selectedCallback = OnSelectedEvent;
            m_ScrollView.OnInit();
        }
        protected override void OnShow(object p)
        {
            m_Infos.OnShow();
            m_ScrollView.ResetSelectedTrump();
        }

        protected override void OnHide()
        {
            base.OnHide();

            m_Infos.OnHide();
        }

        public void OnDestroy()
        {
            m_Infos.OnDestroy();
        }

        #region 事件
        void OnSelectedEvent()
        {
            if (TrumpProperty.GetAll().ContainsKey(m_ScrollView.selecteTrumps))
                m_Infos.Set(TrumpProperty.Get(m_ScrollView.selecteTrumps));
        }
        #endregion
    }
}
#endif
