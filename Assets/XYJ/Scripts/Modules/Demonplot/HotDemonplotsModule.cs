#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto;
    partial class HotDemonplotsModule : HotModuleBase
    {
        DemonplotsMgr demonplotsMgr;

        public HotDemonplotsModule(xys.DemonplotsModule m) : base(m)
        {
            demonplotsMgr = new DemonplotsMgr();
        }

        protected override void OnAwake()
        {
            this.Init();
        }

        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            this.demonplotsMgr.m_Tables.MergeFrom(output);
        }
    }
}
#endif