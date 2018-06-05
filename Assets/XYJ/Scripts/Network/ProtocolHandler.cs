namespace xys
{
    public class ProtocolHandler : wRPC.ProtocolHandler
    {
        public ProtocolHandler() : base(
#if COM_DEBUG
            App.my.debugNetDispatcher
#else
            App.my.mainDispatcher
#endif
            )
        {

        }

        public void Reg<T>(NetProto.Protoid id, System.Action<T> fun) where T : wProtobuf.IMessage, new()
        {
            Reg((ushort)id, (Network.IPacket p, T t)=> { fun(t); });
        }

        public void Reg<T>(NetProto.Protoid id, System.Action<Network.IPacket, T> fun) where T : wProtobuf.IMessage, new()
        {
            Reg((ushort)id, fun);
        }

        public void Reg(NetProto.Protoid id, System.Action<Network.IPacket> fun)
        {
            Reg((ushort)id, fun);
        }
    }
}
