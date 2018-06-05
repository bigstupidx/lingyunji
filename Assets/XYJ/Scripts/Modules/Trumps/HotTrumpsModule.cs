#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto;
    using wProtobuf;

    partial class HotTrumpsModule : HotModuleBase
    {
        TrumpsMgr trumpsMgr = new TrumpsMgr();
        public TrumpsMgr trumpMgr { get { return trumpsMgr; } }

        public HotTrumpsModule(xys.TrumpsModule m) : base(m)
        {
        }

        protected override void OnAwake()
        {
            this.Init();
        }

        protected override void OnDeserialize(IReadStream output)
        {
            this.trumpsMgr.table.MergeFrom(output);
        }

    }
}

#endif