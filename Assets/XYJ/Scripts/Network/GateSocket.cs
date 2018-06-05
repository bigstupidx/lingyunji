using wRPC;
using Network;
using NetProto;
using wProtobuf;
using wProtobuf.RPC;
using System.Net.Sockets;
using System.Collections;

namespace xys
{
    public class GateSocket : SocketClientEvent
    {
        public GateSocket()
        {
            App.my.handler.Reg<TransferPack>(Protoid.A2C_Transfer, OnMessage);
            App.my.handler.Reg<TransferPack>(Protoid.W2C_Transfer, OnMessage);

            state = State.Null;

            local = new LocalProcedure();

            game = new GameRPC(App.my.mainDispatcher, App.my.mainCoroutine, App.my.remote, this);
            world = new WorldRPC(App.my.mainDispatcher, App.my.mainCoroutine, App.my.remote, this);
        }

        SocketClient socket;

        public enum State
        {
            Null, 
            Connecting, // 连接中
            Connected, // 连接成功
            ConnectError, // 连接错误
            Offline, // 掉线了
        }

        public LocalProcedure local { get; private set; }

        public State state { get; private set; }
        public SocketProcedure socketProcedure { get; private set; }
        public GameRPC game { get; private set; }
        public WorldRPC world { get; private set; }

        public void SendGame(int id)
        {
            TransferPack tp = new TransferPack();
            tp.pid = id;

            socket.SendMessageSync((ushort)Protoid.C2A_Transfer, tp);
            App.my.netEvent.OnSend(ServerType.World, (Protoid)id);
        }

        public void SendGame(Protoid id)
        {
            SendGame((int)id);
        }

        // 发送协议到逻辑服
        public void SendGame<T>(int id, T msg) where T : IMessage
        {
            byte[] bytes = new byte[msg.CalculateSize()];
            MessageStream ms = new MessageStream(bytes);
            msg.WriteTo(ms);

            TransferPack tp = new TransferPack();
            tp.pid = id;
            tp.data = ByteString.AttachBytes(bytes);

            socket.SendMessageSync((ushort)Protoid.C2A_Transfer, tp);
            App.my.netEvent.OnSend(ServerType.Game, (Protoid)id);
        }

        public void SendGame<T>(Protoid id, T msg) where T : IMessage
        {
            SendGame((int)id, msg);
        }

        public void SendWorld(int id)
        {
            TransferPack tp = new TransferPack();
            tp.pid = id;

            socket.SendMessageSync((ushort)Protoid.C2W_Transfer, tp);
            App.my.netEvent.OnSend(ServerType.World, (Protoid)id);
        }

        public void SendWorld(Protoid id)
        {
            SendWorld((int)id);
        }

        public void SendWorld<T>(int id, T msg) where T : IMessage
        {
            byte[] bytes = new byte[msg.CalculateSize()];
            MessageStream ms = new MessageStream(bytes);
            msg.WriteTo(ms);

            TransferPack tp = new TransferPack();
            tp.pid = id;
            tp.data = ByteString.AttachBytes(bytes);

            socket.SendMessageSync((ushort)Protoid.C2W_Transfer, tp);
            App.my.netEvent.OnSend(ServerType.World, (Protoid)id);
        }

        // 发送协议到世界服
        public void SendWorld<T>(Protoid id, T msg) where T : IMessage
        {
            SendWorld((int)id, msg);
        }

        public void Connect(string ip, int port)
        {
            if (state != State.Null)
            {
                throw new System.Exception("Connect is not State.Null!");
            }

            socket = new SocketClient(ip, port, this);
            socket.Connect();
            state = State.Connecting;
        }

        public void Close()
        {
            state = State.Null;
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
        }    

        bool SocketClientEvent.OnConnected(SocketClient socket)
        {
            state = State.Connected;
            Log.Debug("连接网关服成功");
            socketProcedure = new SocketProcedure(socket,
#if COM_DEBUG
                App.my.debugNetDispatcher,
#else
                App.my.mainDispatcher,
#endif
                App.my.mainCoroutine, 
                App.my.remote, 
                local);

            socketProcedure.protocolHandler = App.my.handler;

            App.my.mainDispatcher.Message(() => 
            {
                App.my.eventSet.fireEvent(EventID.ConnectedGate);
            });

            return true;
        }

        void OnMessage(IPacket packet, TransferPack tg)
        {
            if (tg == null)
            {
                Log.Error("tg == null pid:{0}", packet.protoid);
                return;
            }

            BitStream ms = new BitStream(tg.data == null ? new byte[0] : tg.data.buffer);
            ms.WritePos = tg.data == null ? 0 : tg.data.Length;
            switch ((Protoid)tg.pid)
            {
            case Protoid.C2A_RPC_Request: // 执行一个RPC
                {
                    RPCRequest request = new RPCRequest();
                    request.MergeFrom(ms);
                    game.OnRequest(request, null);
                }
                break;

            case Protoid.C2A_RPC_Respone: // 请求远程执行RPC的返回
                {
                    RPCRespone respone = new RPCRespone();
                    respone.MergeFrom(ms);
                    App.my.netEvent.OnResponeRPC(ServerType.Game, respone.token);
                    game.OnRespone(respone);
                }
                break;

            case Protoid.C2W_RPC_Request: // 执行一个RPC
                {
                    RPCRequest request = new RPCRequest();
                    CommonBase.Help.MergeFrom(request, tg.data.buffer);
                    world.OnRequest(request, null);
                }
                break;

            case Protoid.C2W_RPC_Respone: // 请求远程执行RPC的返回
                {
                    RPCRespone respone = new RPCRespone();
                    CommonBase.Help.MergeFrom(respone, tg.data.buffer);
                    App.my.netEvent.OnResponeRPC(ServerType.World, respone.token);
                    world.OnRespone(respone);
                }
                break;
            default:
                {
                    switch ((Protoid)packet.protoid)
                    {
                    case Protoid.A2C_Transfer:
                        {
                            App.my.netEvent.OnRec(ServerType.Game, (Protoid)tg.pid);
                        }
                        break;
                    case Protoid.W2C_Transfer:
                        {
                            App.my.netEvent.OnRec(ServerType.World, (NetProto.Protoid)tg.pid);
                        }
                        break;
                    }

                    IPacket p = socket.CreatePacket((ushort)tg.pid, null);
                    App.my.handler.OnMessage(p, ms);
                }
                break;
            }
        }

        void SocketClientEvent.OnConnectedError(SocketClient socket, string message, SocketError error)
        {
            state = State.ConnectError;
            App.my.mainDispatcher.Message(() => 
            {
                App.my.eventSet.fireEvent(EventID.ConnectedGateError);
            });
        }

        IPacket SocketClientEvent.CreatePacket()
        {
            return PacketFactory.CreateSafe();
        }

        void SocketClientEvent.OnOffline(SocketClient socket, SocketError error)
        {
            if(state == State.Connected)
                state = State.Offline;
            if (App.my == null)
                return;

            App.my.mainDispatcher.Message(() => 
            {
                App.my.eventSet.fireEvent(EventID.OfflineGate);
            });
        }

        void SocketClientEvent.OnRecMessage(SocketClient socket, BitStream stream)
        {
            socketProcedure.OnRecMessage(socket, stream);
        }
    }
}