#if !USE_HOT
namespace xys.hot.UI
{
    using Config;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using xys.UI;
    using xys.UI.State;

    class FashionPage : HotTablePageBase
    {
        [SerializeField]
        UIDPInfos m_DpInfos;
        [SerializeField]
        UIDPScroll m_ScrollView;

        FashionPage() : base(null) { }
        FashionPage(HotTablePage page) : base(page) { }

        protected override void OnInit()
        {
            m_DpInfos.OnInit();

            m_ScrollView.selectedCallback = this.ResetInfos;
        }

        protected override void OnShow(object args)
        {
            m_ScrollView.OnInit();
            Event.Subscribe(EventID.Demonplot_RefleashUI, this.Refreash);
            Event.Subscribe(EventID.Package_UpdatePackage, this.Refreash);
            Event.Subscribe<AttributeChange>(EventID.LocalAttributeChange, this.AttriChange);
        }

        protected override void OnHide()
        {
            m_DpInfos.OnHide();
            m_ScrollView.OnHide();
        }

        void ResetInfos()
        {
            this.Refreash();
        }

        void Refreash()
        {
            m_DpInfos.ResetPage(m_ScrollView.skilltype);
        }

        void AttriChange(AttributeChange data)
        {
            this.Refreash();
        }
    }
}
#endif