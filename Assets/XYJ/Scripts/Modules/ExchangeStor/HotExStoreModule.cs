#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys.hot
{
    partial class HotExStoreModule : HotModuleBase
    {
        public ExStoreMgr exstoreMgr { get; private set; }

        public HotExStoreModule(xys.ExStoreModule m) : base(m)
        {
            exstoreMgr = new ExStoreMgr();
        }

        protected override void OnAwake()
        {
            Init();
        }

        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            this.exstoreMgr.exStore.MergeFrom(output);
        }
    }
}

#endif