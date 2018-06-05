#if !USE_HOT
using Config;
using NetProto;
using System;
using System.Collections;
using wProtobuf.RPC;
using xys.hot.UI;
using xys.UI;

namespace xys.hot
{
    partial class HotTradeStoreModule
    {
        C2ATradeStoreModuleRequest m_C2ATradeStoreModuleRequest;

        void Init()
        {
            this.m_C2ATradeStoreModuleRequest = new C2ATradeStoreModuleRequest(App.my.game.local);
            Event.Subscribe<TradeStoreBuyRequest>(EventID.TradeStore_Buy, this.Buy);
            Event.Subscribe<TradeStoreSellRequest>(EventID.TradeStore_Sell, this.Sell);
            Event.Subscribe(EventID.TradeStore_Search, this.SearchTradeItemData);
            Event.Subscribe(EventID.TradeStore_QuickSellSearch, this.SearchTradeItemDataSellPanel);
            Event.Subscribe(ObjEventID.ChangeAttri, () =>
            {
                Event.fireEvent(EventID.TradeStore_RefreshInfoUI);
            });
            Event.Subscribe(EventID.Package_UpdatePackage, () =>
            {
                App.my.eventSet.fireEvent(EventID.TradeStore_MainRefreshUI);
            });
            Event.Subscribe(EventID.TradeStore_Rest, this.Rest);
            Event.Subscribe(EventID.TradeStore_Recover, this.Recover);
            MainPanelItemListener listener = new MainPanelItemListener();
            listener.itemShowFunc = this.ItemShowConditionFunc;
            listener.arg = 1;
            MainPanel.SetItemListener((int)PanelType.UITradingMarketTablePanel, listener);
        }

        void Buy(TradeStoreBuyRequest request)
        {            
            App.my.mainCoroutine.StartCoroutine(BuyItemYield(request));
        }

        IEnumerator BuyItemYield(TradeStoreBuyRequest request)
        {
            var buyResult = this.m_C2ATradeStoreModuleRequest.TradeBuyItemYield(request);
            yield return buyResult;
            if (buyResult.code == Error.Success && buyResult.result != null)
            {
                var respone = buyResult.result;
                if (respone.code == ReturnCode.Tr_BuyItemNumError)
                {
                    xys.UI.Utility.TipContentUtil.Show("tr_buy_num_error");
                }
                else if (respone.code == ReturnCode.Tr_BuyItemLimitError)
                {
                    xys.UI.Utility.TipContentUtil.Show("tr_buy_limit");
                }
                else if (respone.code == ReturnCode.Backage_Full_Error)
                {
                    xys.UI.Utility.TipContentUtil.Show("tr_package_full");
                    App.my.eventSet.FireEvent(EventID.TradeStore_InputNum, request.itemnum);
                    yield break;
                }
                else if (respone.code == ReturnCode.Tr_CurPriceError)
                {
                    xys.UI.Utility.TipContentUtil.Show("tr_price_error");
                }
                else if(respone.code == ReturnCode.ReturnCode_OK)
                {
                    xys.UI.Utility.TipContentUtil.Show("tr_trade_suc");
                    tradeStoreMgr.tradeLimit.buyedTimedic[request.itemid] += request.itemnum;
                }
                tradeStoreMgr.tradeItemData.data = respone.curtradeItemData.data;
                App.my.eventSet.fireEvent(EventID.TradeStore_MainRefreshUI);
                yield break;
            }
        }

        void Sell(TradeStoreSellRequest request)
        {
            App.my.mainCoroutine.StartCoroutine(SellItemYield(request));
        }

        IEnumerator SellItemYield(TradeStoreSellRequest request)
        {
            var sellItemResult = this.m_C2ATradeStoreModuleRequest.TradeSellItemYield(request);
            yield return sellItemResult;
            if (sellItemResult.code == Error.Success && sellItemResult.result != null)
            {
                var respone = sellItemResult.result;
                if (respone.code == ReturnCode.Tr_CurPriceError)
                {
                    xys.UI.Utility.TipContentUtil.Show("tr_price_error");
                }
                else if (respone.code == ReturnCode.Tr_SellItemError)
                {
                    xys.UI.Utility.TipContentUtil.Show("tr_sell_error");
                }
                else if (respone.code == ReturnCode.Tr_SellLimit)
                {
                    xys.UI.Utility.TipContentUtil.Show("tr_sell_limit", Item.Get((int)respone.curtradeItemData.data[request.itemid].priceType).name);
                }
                else if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    //App.my.eventSet.FireEvent<int>(EventID.TradeStore_SellSuccessful, request.itemnum);
                    xys.UI.Utility.TipContentUtil.Show("tr_sell_suc");
                }
                tradeStoreMgr.tradeItemData.data = respone.curtradeItemData.data;
                App.my.eventSet.fireEvent(EventID.TradeStore_MainRefreshUI);
                yield break;
            }
        }

        void SearchTradeItemData()
        {
            App.my.mainCoroutine.StartCoroutine(SearchItemData());
        }

        IEnumerator SearchItemData()
        {
            var tradeData = this.m_C2ATradeStoreModuleRequest.TradeStroeItemDataYield(new NetProto.None() {});
            yield return tradeData;
            if (tradeData.code == Error.Success && tradeData.result != null)
            {
                if (tradeData.result.code == ReturnCode.ReturnCode_OK)
                {
                    tradeStoreMgr.tradeItemData.data = tradeData.result.data.data;
                    tradeStoreMgr.tradeLimit.buyedTimedic = tradeData.result.curTradeItemLimitTime.buyedTimedic;
                    Event.fireEvent(EventID.TradeStore_MainRefreshUI);
                    yield break;
                }
            }
        }

        void SearchTradeItemDataSellPanel()
        {
            App.my.mainCoroutine.StartCoroutine(SearchItemDataSellPanel());
        }

        IEnumerator SearchItemDataSellPanel()
        {
            var tradeData = this.m_C2ATradeStoreModuleRequest.TradeStroeItemDataYield(new NetProto.None() { });
            yield return tradeData;
            if (tradeData.code == Error.Success && tradeData.result != null)
            {
                if (tradeData.result.code == ReturnCode.ReturnCode_OK)
                {
                    tradeStoreMgr.tradeItemData.data = tradeData.result.data.data;
                    tradeStoreMgr.tradeLimit.buyedTimedic = tradeData.result.curTradeItemLimitTime.buyedTimedic;
                    Event.fireEvent(EventID.TradeStore_QuickSellRefreshUI);
                    yield break;
                }
            }
        }

        void Rest()
        {
            App.my.mainCoroutine.StartCoroutine(RestYield());
        }

        IEnumerator RestYield()
        {
            var restData = this.m_C2ATradeStoreModuleRequest.RestTradeStoreYield(new NetProto.None());
            yield return restData;
            if (restData.code == Error.Success && restData.result != null)
            {

                if (restData.result.code == ReturnCode.ReturnCode_OK)
                {
                    tradeStoreMgr.tradeItemData.data = restData.result.data.data;
                    tradeStoreMgr.tradeLimit.buyedTimedic = restData.result.curTradeItemLimitTime.buyedTimedic;
                }
            }
        }

        void Recover()
        {
            App.my.mainCoroutine.StartCoroutine(RecoverYield());
        }

        IEnumerator RecoverYield()
        {
            var recoverData = this.m_C2ATradeStoreModuleRequest.RecoverTradeStoreYield(new NetProto.None());
            yield return recoverData;
            if (recoverData.code == Error.Success && recoverData.result != null)
            {
                if (recoverData.result.code == ReturnCode.ReturnCode_OK)
                {
                    tradeStoreMgr.tradeItemData.data = recoverData.result.curtradeItemData.data;
                }
            }
        }

        bool ItemShowConditionFunc()
        {
            return true;
        }
    }
}
#endif