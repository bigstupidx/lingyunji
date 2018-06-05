#if !USE_HOT
using wProtobuf;

namespace xys.hot
{
    partial class HotTradeStoreModule : HotModuleBase
    {
        TradeStoreModule module;
        TradeStoreMgr tradeStoreMgr = new TradeStoreMgr();
        public HotTradeStoreModule(xys.TradeStoreModule m) : base(m)
        {

        }

        protected override void OnAwake()
        {
            Init();
        }

        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            this.tradeStoreMgr.tradeLimit.MergeFrom(output);
        }
    }
}
#endif