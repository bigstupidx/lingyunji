using wRPC;
using NetProto;
using wProtobuf;
using wProtobuf.RPC;

namespace xys
{
    // 前端的协议转发RPC请求
    public class GameRPC : Procedure
    {
        GateSocket gatesocket;

        public GameRPC(IDispatcher dispatcher, ICoroutine coroutine, IRemoteCall remote, GateSocket gatesocket)
            : base(dispatcher, coroutine, remote, new LocalProcedure())
        {
            this.gatesocket = gatesocket;
        }

        protected override void OnRemoteRequestEnd(RPCRespone respone, object param)
        {
            gatesocket.SendGame(Protoid.C2A_RPC_Respone, respone);
        }

        protected override void OnSendLocalRequest(RPCRequest request)
        {
            App.my.netEvent.OnRequestRPC(ServerType.Game, request.token);
            gatesocket.SendGame(Protoid.C2A_RPC_Request, request);
        }
    }

    // 前端的协议转发RPC请求
    public class WorldRPC : Procedure
    {
        GateSocket gatesocket;
        public WorldRPC(IDispatcher dispatcher, ICoroutine coroutine, IRemoteCall remote, GateSocket gatesocket)
            : base(dispatcher, coroutine, remote, new LocalProcedure())
        {
            this.gatesocket = gatesocket;
        }

        protected override void OnRemoteRequestEnd(RPCRespone respone, object param)
        {
            gatesocket.SendWorld(Protoid.C2W_RPC_Respone, respone);
        }

        protected override void OnSendLocalRequest(RPCRequest request)
        {
            App.my.netEvent.OnRequestRPC(ServerType.World, request.token);
            gatesocket.SendWorld(Protoid.C2W_RPC_Request, request);
        }
    }
}