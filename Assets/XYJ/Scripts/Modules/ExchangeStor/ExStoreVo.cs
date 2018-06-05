#if !USE_HOT
using Config;
using NetProto;
using System.Collections.Generic;
using xys.hot.UI;
using xys.UI;

namespace xys.hot
{
    partial class HotExStoreModule
    {
        C2AExchangeModuleRequest m_ExchangeModuleRequest;

        void Init()
        {

            this.m_ExchangeModuleRequest = new C2AExchangeModuleRequest(App.my.game.local);
            Event.Subscribe<int>(EventID.ExchangeStore_RestDay, this.Rest);
            Event.Subscribe<ExchangeItemReq>(EventID.ExchangeStore_Echange, this.Exchange);
            Event.Subscribe(ObjEventID.ChangeAttri, () =>
            {
                Event.fireEvent(EventID.ExchangeStore_RefreshUI);
            });
            Event.Subscribe(EventID.Package_UpdatePackage, () =>
            {
                Event.fireEvent(EventID.ExchangeStore_RefreshUI);
            });
            MainPanelItemListener listener = new MainPanelItemListener();
            listener.itemShowFunc = this.ItemShowConditionFunc;
            listener.arg = 1;
            MainPanel.SetItemListener((int)PanelType.UIExchangeStorePanel, listener);
        }

        void Rest(int restType)
        {
            NetProto.Int64 NrestType = new NetProto.Int64();
            NrestType.value = restType;
            this.m_ExchangeModuleRequest.ExchangeStoreRest(NrestType, (error, respone) =>
            {
                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    OnRest(respone.restType);
                    App.my.eventSet.fireEvent(EventID.ExchangeStore_RefreshUI);
                }
            });
        }

        void Exchange(ExchangeItemReq request)
        {
            this.m_ExchangeModuleRequest.Exchange(request, (error, respone) =>
            {
                if (respone.code == ReturnCode.Ex_JobError)
                {
                    return;
                }
                if (respone.code == ReturnCode.Backage_Full_Error)
                {
                    xys.UI.Utility.TipContentUtil.Show("ex_packagefull");
                    return;
                }

                if (respone.code == ReturnCode.Ex_Currency1Error)
                {
                    xys.UI.Utility.TipContentUtil.Show("ex_notengough", Item.Get(ExchangeStoreData.Get(request.exchangeItem.itemid).currency1id).name);
                    return;
                }
                if (respone.code == ReturnCode.Ex_Currency2Error)
                {
                    xys.UI.Utility.TipContentUtil.Show("ex_notengough", Item.Get(ExchangeStoreData.Get(request.exchangeItem.itemid).currency2id).name);
                    return;
                }
                if (respone.code == ReturnCode.Ex_MaterialError)
                {
                    xys.UI.Utility.TipContentUtil.Show("ex_notengough", Item.Get(ExchangeStoreData.Get(request.exchangeItem.itemid).materialid).name);
                    return;
                }

                if (respone.code == ReturnCode.Ex_LevelError)
                {
                    xys.UI.Utility.TipContentUtil.Show("ex_levelerror");
                    return;
                }

                if (respone.code == ReturnCode.Ex_UsedTimeError)
                {
                    xys.UI.Utility.TipContentUtil.Show("ex_limit");
                    return;
                }
                this.exstoreMgr.exStore.itemusedtime[request.exchangeItem.itemid] += request.exchangeItem.itemnum;
                //App.my.eventSet.FireEvent(EventID.ExchangeStore_Successful, new object[] { request.exchangeItem.itemid, request.exchangeItem.itemnum });
                App.my.eventSet.fireEvent(EventID.ExchangeStore_RefreshUI);
            });
        }

        void OnRest(ExRestType resttype)
        {
            List<int> itemList = new List<int>(this.exstoreMgr.exStore.itemusedtime.Keys);
            for (var i = 0; i < itemList.Count; i++)
            {
                if (resttype == ExRestType.ExRestDay)
                {
                    if (ExchangeStoreData.Get(itemList[i]).daylimit == 0)
                    {
                        continue;
                    }
                    else
                    {
                        this.exstoreMgr.exStore.itemusedtime[itemList[i]] = 0;
                    }
                }
                else if (resttype == ExRestType.ExRestWeek)
                {
                    if (ExchangeStoreData.Get(itemList[i]).daylimit != 0)
                    {
                        continue;
                    }
                    else
                    {
                        this.exstoreMgr.exStore.itemusedtime[itemList[i]] = 0;
                    }
                }
                else if (resttype == ExRestType.ExRestAll)
                {
                    this.exstoreMgr.exStore.itemusedtime[itemList[i]] = 0;
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