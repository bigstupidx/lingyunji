#if !USE_HOT
using System;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{

    class UITradingMarketTablePanel : HotTablePanelBase
    {

        enum TradeMarketType
        {
            TradeStore,   //商会
            Auctions, //拍卖行
        }

        [SerializeField]
        Button ReturnBtn;
        [SerializeField]
        Button TradeStorBtn;
        [SerializeField]
        Button AuctionsBtn;

        StateRoot tradeStoreSR;
        StateRoot auctionsSR;


        HotTablePanel m_Parent;

        public UITradingMarketTablePanel() : base(null)
        {

        }


        public UITradingMarketTablePanel(HotTablePanel parent) : base(parent)
        {
            this.m_Parent = parent;
        }

        protected override void OnInit()
        {

            tradeStoreSR = TradeStorBtn.transform.GetComponent<StateRoot>();
            auctionsSR = AuctionsBtn.transform.GetComponent<StateRoot>();

            ReturnBtn.onClick.AddListener(() =>
            {
                App.my.uiSystem.HidePanel(PanelType.UITradingMarketTablePanel);
            });
            TradeStorBtn.onClick.AddListener(() =>
            {
                this.m_Parent.ShowType(m_Parent.GetPageList()[(int)TradeMarketType.TradeStore].Get().pageType, null);
                auctionsSR.SetCurrentState(0, true);
                tradeStoreSR.SetCurrentState(1, true);
            });
            AuctionsBtn.onClick.AddListener(()=> 
            {
                this.m_Parent.ShowType(m_Parent.GetPageList()[(int)TradeMarketType.Auctions].Get().pageType, null);
                auctionsSR.SetCurrentState(1, true);
                tradeStoreSR.SetCurrentState(0, true);
            });
        }

        protected override void OnShow(object p)
        {
            
        }
    }
}
#endif