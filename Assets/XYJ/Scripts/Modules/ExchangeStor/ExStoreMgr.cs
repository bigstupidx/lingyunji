#if !USE_HOT
using NetProto;

namespace xys.hot
{
    class ExStoreMgr
    {
        public ExchangeStore exStore { get; private set; }
        public ExStoreMgr() {
            exStore = new ExchangeStore();
        }

        public int GetUsedTime(int itemid)
        {

            if (!exStore.itemusedtime.ContainsKey(itemid))
            {
                exStore.itemusedtime.Add(itemid, 0);
                return 0;
            }
            else
            {
                return exStore.itemusedtime[itemid];
            }
        }
    }
}

#endif
