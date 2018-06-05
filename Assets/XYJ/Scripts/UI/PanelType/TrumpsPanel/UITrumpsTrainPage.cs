#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using xys.UI;
    using xys.UI.State;
    using Config;
    using System;

    class UITrumpsTrainPage : HotTablePageBase
    {
        protected enum PageType
        {
            Property = 0,
            Infused,
        }
        [SerializeField]
        StateToggle m_StateTogle;
        [SerializeField]
        UITrumpsTrainScroll m_ScrollView;
        [SerializeField]
        UITrumpsTrainInfos m_Infos;
        [SerializeField]
        UITrumpsTrainProperty m_Property;
        [SerializeField]
        UITrumpsInfused m_Infused;
        UITrumpsTrainPage() : base(null) { }
        public UITrumpsTrainPage(HotTablePage parent) : base(parent) { }

        protected override void OnInit()
        {
            m_Infos.OnInit(this);
            m_Property.OnInit(this);
            m_Infused.OnInit(this);

            m_ScrollView.OnInit();
            m_ScrollView.selectedCallback = this.OnSelectedTrump;

            m_StateTogle.OnSelectChange = this.OnSelectedChange;
        }

        protected override void OnShow(object p)
        {
            int selectTrump = 0;
            if(p != null)
                selectTrump = (int)p;

            m_Infos.OnShow();
            m_ScrollView.Create(selectTrump);
            //
            Event.Subscribe(EventID.Trumps_RefleashUI, this.OnRefresh);
            Event.Subscribe(EventID.Package_UpdatePackage, this.OnRefreshTips);
            //
            m_StateTogle.Select = (int)PageType.Property;
        }

        void OnRefresh()
        {
            this.m_ScrollView.Refresh();
            this.OnSelectedTrump();
        }

       void OnRefreshTips()
        {
            if (m_StateTogle.Select == (int)PageType.Property)
            {
                m_Property.OnRefreshUpgrade();
            }
            else if (m_StateTogle.Select == (int)PageType.Infused)
            {
                m_Infused.Set(m_ScrollView.selectedTrump);
            }
        }

        protected override void OnHide()
        {
            m_ScrollView.Clear();
            m_Infos.OnHide();
            m_Infused.OnHide();
        }

        public void OnDestroy()
        {
            m_Infos.OnDestroy();
        }

        void OnSelectedTrump()
        {
            m_Infos.Set(m_ScrollView.selectedTrump);
            if (m_StateTogle.Select == (int)PageType.Property)
            {
                m_Property.Set(m_ScrollView.selectedTrump);
            }
            else if (m_StateTogle.Select == (int)PageType.Infused)
            {
                m_Infused.Set(m_ScrollView.selectedTrump);
            }
        }

        void OnSelectedChange(StateRoot sr,int index)
        {
            if(index == (int)PageType.Property)
            {
                m_Property.Set(m_ScrollView.selectedTrump);
            }
            else if(index == (int)PageType.Infused)
            {
                m_Infused.Set(m_ScrollView.selectedTrump);
            }
        }

    }
}
#endif
