using Network;
using NetProto;
using System.Net.Sockets;

namespace xys
{
    public class HotMessage : wProtobuf.IMessage
    {
        public HotMessage()
        {

        }

        public HotMessage(object instance)
        {
            refType = new RefType(instance);
        }

        RefType refType;

        public int CalculateSize()
        {
            return (int)refType.InvokeMethodReturn("CalculateSize");
        }

        public void MergeFrom(wProtobuf.IReadStream input)
        {
            refType.InvokeMethod("MergeFrom", input);
        }

        public void WriteTo(wProtobuf.IWriteStream output)
        {
            refType.InvokeMethod("WriteTo", output);
        }
    }

    public class LoginSocket : SocketClientEvent
    {
        public enum State
        {
            Null,
            ConnectingLogin, // 连接登陆服中
            VerifyLogin, // 验证账号和密码中
            VerifyError, // 验证账号和密码中
            LoginEnd, // 登陆结束
            ConnectError,// 连接错误
            Success,
        }

        public LoginSocket()
        {

        }

        public State state { get; private set; }

        public SocketClient socket { get; private set; }

        public void Connect(string ip, int port)
        {
            if (state != State.Null)
                throw new System.Exception("Connect State is not Null!");

            state = State.ConnectingLogin;
            socket = new SocketClient(ip, port, this);
            socket.Connect();
        }

        public RefType loginEvent { get; set; }

        bool SocketClientEvent.OnConnected(SocketClient socket)
        {
            App.my.mainDispatcher.Message(() => 
            {
                socket.SendMessageSync((int)Protoid.C2L_Ack_Login, new HotMessage(loginEvent.InvokeMethodReturn("OnConnected")));
                state = State.VerifyLogin;
            });

            return true;
        }

        void SocketClientEvent.OnConnectedError(SocketClient socket, string message, SocketError error)
        {
            App.my.mainDispatcher.Message(() =>
            {
                state = State.ConnectError;
                loginEvent.InvokeMethod("OnConnectedError");
            });
        }

        IPacket SocketClientEvent.CreatePacket()
        {
            return PacketFactory.CreateSafe();
        }

        public void Close()
        {
            if (socket != null)
            {
                var t = socket;
                socket = null;
                t.Close();
            }

            state = State.Null;
        }

        void SocketClientEvent.OnOffline(SocketClient socket, SocketError error)
        {
            App.my.mainDispatcher.Message(() =>
            {
                if (state != State.Success)
                {
                    loginEvent.InvokeMethod("OnVerifyError");
                }
            });
        }

        //void OnMessage(LoginRespone respone)
        //{
        //    Log.Debug("token:{0} code:{1} result:{2}", respone.token, respone.code, respone.result);
        //    if (respone.result == LoginResult.LR_OK)
        //    {
        //        state = State.Success;
        //        wProtobuf.MessageStream ms = new wProtobuf.MessageStream(respone.servers.buffer);
        //        ms.WritePos = respone.servers.Length;
        //        servers = new ServerInfoMap();
        //        try
        //        {
        //            servers.MergeFrom(ms);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            Log.Exception(ex);
        //        }

        //        loginEvent.OnSuccess(respone, servers);
        //    }
        //    else
        //    {
        //        state = State.VerifyError;
        //        loginEvent.OnVerifyError();
        //    }
        //}

        BitStream NetRecStream = new BitStream(1024);

        void SocketClientEvent.OnRecMessage(SocketClient socket, BitStream stream)
        {
            lock (NetRecStream)
            {
                NetRecStream.Write(stream.Buffer, stream.ReadPos, stream.ReadSize);
            }

            App.my.mainDispatcher.Message(() => 
            {
                lock (NetRecStream)
                {
                    loginEvent.InvokeMethod("OnRecMessage", socket, NetRecStream);
                }
            });

            //IPacket packet = socket.CreatePacket();
            //packet.Read(stream);
            //Protoid pid = (Protoid)packet.protoid;
            //switch (pid)
            //{
            //case Protoid.C2L_Ack_Login:
            //    {
            //        LoginRespone respone = new LoginRespone();
            //        respone.MergeFrom(stream);
            //        App.my.mainDispatcher.Message(OnMessage, respone);
            //    }
            //    break;
            //}
        }

        public void SetState(State state)
        {
            this.state = state;
        }
    }
}
