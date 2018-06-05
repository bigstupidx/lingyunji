
using System;
using NetProto;
using System.Reflection;

namespace xys
{
    public partial class TradeStoreModule : HotModule
    {
        public TradeStoreModule() : base("xys.hot.HotTradeStoreModule")
        {
            
        }

        public object tradeStoreMgr
        {
            get
            {
                return refType.GetField("tradeStoreMgr");
            }
        }
    }
}
