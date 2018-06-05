namespace xys
{
    public class NetEvent
    {
        // 发送一个RPC
        public void OnRequestRPC(NetProto.ServerType st, int token)
        {

        }

        // 接收一个RPC
        public void OnResponeRPC(NetProto.ServerType st, int token)
        {

        }

        // 发送一个协议请求
        public void OnSend(NetProto.ServerType st, NetProto.Protoid pid)
        {

        }

        // 接收一个协议
        public void OnRec(NetProto.ServerType st, NetProto.Protoid pid)
        {

        }
    }
}
