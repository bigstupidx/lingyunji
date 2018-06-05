#if !USE_HOT
namespace xys.hot
{
    using Network;
    using System.Collections.Generic;

    class ProtocolHandler
    {
        public ProtocolHandler(xys.ProtocolHandler parent)
        {
            this.parent = parent;
        }

        xys.ProtocolHandler parent;

        class RegHType<T> where T : RPC.IMessage, new()
        {
            public RegHType(System.Action<T> fun)
            {
                this.fun = fun;
            }

            System.Action<T> fun;

            public void OnCall(IPacket packet, wProtobuf.RealBytes bytes)
            {
                if (fun != null)
                {
                    T obj = RPCHelp.hMessageMergeFrom<T>(bytes);
                    fun(obj);
                }
            }
        }

        public void RegHot<T>(NetProto.Protoid id, System.Action<T> fun) where T : RPC.IMessage, new()
        {
            RegHot((ushort)id, fun);
        }

        public void RegHot<T>(ushort id, System.Action<T> fun) where T : RPC.IMessage, new()
        {
            RegHType<T> r = new RegHType<T>(fun);
            parent.Reg<wProtobuf.RealBytes>(id, r.OnCall);
        }

        class RegWType<T> where T : wProtobuf.IMessage, new()
        {
            public RegWType(System.Action<T> fun)
            {
                this.fun = fun;
            }

            System.Action<T> fun;

            public void OnCall(IPacket packet, wProtobuf.RealBytes bytes)
            {
                if (fun != null)
                {
                    T obj = RPCHelp.wMessageMergeFrom<T>(bytes);
                    fun(obj);
                }
            }
        }

        public void Reg<T>(NetProto.Protoid id, System.Action<T> fun) where T : wProtobuf.IMessage, new()
        {
            Reg((ushort)id, fun);
        }

        public void Reg<T>(ushort id, System.Action<T> fun) where T : wProtobuf.IMessage, new()
        {
            RegWType<T> r = new RegWType<T>(fun);
            parent.Reg<wProtobuf.RealBytes>(id, r.OnCall);
        }

        class RegType
        {
            public RegType(System.Action fun)
            {
                this.fun = fun;
            }

            System.Action fun;

            public void OnCall(IPacket packet, wProtobuf.RealBytes bytes)
            {
                if (fun != null)
                {
                    fun();
                }
            }
        }

        public void Reg(NetProto.Protoid id, System.Action fun)
        {
            Reg((ushort)id, fun);
        }

        public void Reg(ushort id, System.Action fun)
        {
            RegType r = new RegType(fun);
            parent.Reg<wProtobuf.RealBytes>(id, r.OnCall);
        }
    }
}

#endif