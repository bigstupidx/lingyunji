#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProto;
using wProtobuf;
using wProtobuf.RPC;
using Int64 = NetProto.Int64;

namespace xys.hot
{
    internal partial class HotChatModule : HotModuleBase
    {
        public readonly ChatMgr ChatMgr = new ChatMgr();

        public HotChatModule(ChatModule module) : base(module) { }
        protected override void OnAwake()
        {
            Init();
        }

        protected override void OnDeserialize(IReadStream output)
        {

        }

        #region Interface

        public void AddSystemMsg(ChannelType channel,string msg)
        {
            ChatMgr.AddSystemMsg(channel,msg);
        }
        #endregion
    }
}

#endif