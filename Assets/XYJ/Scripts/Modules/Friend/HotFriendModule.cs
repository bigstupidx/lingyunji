#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto;
    using xys;

    class HotFriendModule : HotModuleBase
    {
        FriendModule module;
        public HotFriendModule(FriendModule m) : base(m)
        {
            module = m;
        }
  
        public FriendDbData m_friendDbData;

        protected override void OnAwake()
        {
            if (friendMgr == null)
            {
                friendMgr = new FriendMgr(App.my.localPlayer);
            }
            if (m_friendDbData == null)
            {
                m_friendDbData = new FriendDbData();
            }
        }

        public FriendMgr friendMgr = null;

        // 序列化
        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            if (output != null && m_friendDbData != null)
            {
                m_friendDbData.MergeFrom(output);

                if (friendMgr != null)
                {
                    friendMgr.SetFriendDbData(m_friendDbData);
                }
            }
        }
    }
}
#endif