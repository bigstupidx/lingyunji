#if !USE_HOT
using NetProto;

namespace xys.hot
{
    class TradeStoreMgr
    {
        public TradeItemData tradeItemData { get; private set; }
        public TradeItemLimitTime tradeLimit { get; private set; }

        public TradeStoreMgr()
        {
            tradeItemData = new TradeItemData();
            tradeLimit = new TradeItemLimitTime();
        }

        public int GetBuyedNum(int itemid)
        {
            if (!tradeLimit.buyedTimedic.ContainsKey(itemid))
            {
                tradeLimit.buyedTimedic.Add(itemid, 0);
                return 0;
            }
            else
            {
                return tradeLimit.buyedTimedic[itemid];
            }
        }

        public TradeItemAtt GetItemDataDic(int itemid)
        {
            if (tradeItemData.data.ContainsKey(itemid))
            {
                return tradeItemData.data[itemid];
            }
            else
            {
                return null;
            }
        }
    }

}
#endif
