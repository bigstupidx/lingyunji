#if !USE_HOT
namespace xys.hot.UI
{
    using xys.UI;
    using UnityEngine;
    using System.Collections;
    using Config;
    using System;
    using battle;

    class UITrumpsPanel : HotTablePanelBase
    {
        public enum Page
        {
            Equip = 0,
            Train,
            Identify,
        }
        UITrumpsPanel(): base(null) { }
        UITrumpsPanel(HotTablePanel parent) : base(parent)  {  }

        TrumpsMgr m_TrumpsMgr;
        public TrumpsMgr mgr { get { return m_TrumpsMgr; } }

        protected override void OnInit()
        {
            m_TrumpsMgr = App.my.localPlayer.GetModule<TrumpsModule>().trumpMgr as TrumpsMgr;
        }

        protected override bool OnPreChange(HotTablePage page)
        {
            if(page.pageType == "UITrumpsTrainPage")
            {
                if(m_TrumpsMgr.table.attributes.Count == 0)
                {
                    SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_None").des);
                    return false;
                }
            }
            return base.OnPreChange(page);
        }
        protected override void OnShow(object p)
        {
            if(p != null)
            {
                int trumpId = (int)p;
                if (trumpId != 0)
                    this.tableParent.ShowType((int)Page.Train, trumpId);
            }

            //界面事件注册
            Event.Subscribe(EventID.Trumps_RefleashUI, this.Refresh);
            Event.Subscribe(EventID.Trumps_RefreshEquips, this.EquipRefresh);
        }

        protected override void OnHide()
        {
            UITrumpIdentifyPage identifyPage = this.GetPage((int)Page.Identify) as UITrumpIdentifyPage;
            identifyPage.OnDestroy();

            UITrumpsTrainPage trainPage = this.GetPage((int)Page.Train) as UITrumpsTrainPage;
            trainPage.OnDestroy();

            base.OnHide();
        }

        void Refresh()
        {
        }

        void EquipRefresh()
        {

        }
    }
}

#endif
