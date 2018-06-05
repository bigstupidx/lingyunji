#if !USE_HOT
using Network;
using NetProto;
using NetProto.Hot;
using System.Net.Sockets;

namespace xys.hot
{
    interface ILoginEvent
    {
        void OnConnectedError(); // 连接失败
        LoginRequest OnConnected(); // 连接成功
        void OnVerifyError(); // 验证出错
        void OnSuccess(LoginRespone respone, ServerInfoMap servers); // 验证成功，下发服务器列表
    }

    class HotLoginSocket
    {
        public HotLoginSocket(LoginSocket parent, ILoginEvent loginEvent)
        {
            this.parent = parent;
            this.loginEvent = loginEvent;
        }

        LoginSocket parent;
        ILoginEvent loginEvent { get; set; }

        // 连接失败
        void OnConnectedError()
        {
            loginEvent.OnConnectedError();
        }

        LoginRequest OnConnected()
        {
            return loginEvent.OnConnected();
        }

        // 验证出错
        void OnVerifyError()
        {
            loginEvent.OnVerifyError();
        }

        // 验证成功，下发服务器列表
        void OnSuccess(LoginRespone respone, ServerInfoMap servers)
        {
            loginEvent.OnSuccess(respone, servers);
        }

        public ServerInfoMap servers { get; private set; }

        void OnMessage(LoginRespone respone)
        {
            Log.Debug("token:{0} code:{1} result:{2}", respone.token, respone.code, respone.result);
            if (respone.result == LoginResult.LR_OK)
            {
                parent.SetState(LoginSocket.State.Success);
                wProtobuf.MessageStream ms = new wProtobuf.MessageStream(respone.servers.buffer);
                ms.WritePos = respone.servers.Length;
                servers = new ServerInfoMap();
                try
                {
                    servers.MergeFrom(ms);
                }
                catch (System.Exception ex)
                {
                    Log.Exception(ex);
                }

                loginEvent.OnSuccess(respone, servers);
            }
            else
            {
                parent.SetState(LoginSocket.State.VerifyError);
                loginEvent.OnVerifyError();
            }
        }

        // 有协议过来了
        void OnRecMessage(SocketClient socket, Network.BitStream stream)
        {
            IPacket packet = socket.CreatePacket();
            packet.Read(stream);
            Protoid pid = (Protoid)packet.protoid;
            switch (pid)
            {
            case Protoid.C2L_Ack_Login:
                {
                    LoginRespone respone = new LoginRespone();
                    try
                    {
                        respone.MergeFrom(stream);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Exception(ex);
                    }

                    OnMessage(respone);
                }
                break;
            }
        }
    }
}
#endif