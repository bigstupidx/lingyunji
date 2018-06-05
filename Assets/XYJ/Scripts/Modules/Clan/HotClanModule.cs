#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto;

    class HotClanModule : HotModuleBase
    {
        public HotClanModule(ClanModule m) : base(m)
        {

        }

        public ClanModule module;

        protected override void OnAwake()
        {
            if (clanMgr == null)
            {
                clanMgr = new ClanMgr(App.my.localPlayer);
            }
        }

        public ClanMgr clanMgr = null;

        // 序列化
        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            
        }
    }
}
#endif