using NetProto;
using System.Collections.Generic;

namespace xys
{
    public class ChatModule : HotModule
    {
        public ChatModule() : base("xys.hot.HotChatModule")
        {
            var v = typeof(Dictionary<long, ItemData>);
            v = typeof(Dictionary<long, PetsAttribute>);
            v = typeof(Dictionary<ChannelType, int>);
            v = typeof(Dictionary<ChannelType, Dictionary<int, ChatMsgRspone>>);
            v = typeof(Dictionary<int, ChatMsgRspone>);
            v = typeof(Queue<ChatMsgRspone>);
            v = typeof(Dictionary<long, ChatMsgRspone>);
        }

        private object chatMgr;
        public object ChatMgr
        {
            get
            {
                if (null == chatMgr)
                {
                    chatMgr = refType.GetField("ChatMgr");
                }
                return chatMgr;
            }
        }

        public void AddSystemMsg(ChannelType channel, string msg)
        {
            refType.InvokeMethod("AddSystemMsg",channel,msg);
        }
    }
} 

