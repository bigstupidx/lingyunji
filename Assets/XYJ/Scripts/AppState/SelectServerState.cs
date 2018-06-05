#if !USE_HOT
using NetProto;
using NetProto.Hot;
using System.Collections;

namespace xys.hot
{
    class SelectServerData
    {
        public LoginRespone respone;
        public ServerInfoMap servers;
    }

    class SelectServerState : AppStateBase
    {
        public SelectServerState(HotAppStateBase parent) : base(parent)
        {

        }

        C2GLoginRequest request = null;

        LoginRespone respone;
        ServerInfoMap servers;

        int m_selectServerId = 0;

        // 进入此状态
        protected override void OnEnter(object p)
        {
            SelectServerData ssd = p as SelectServerData;
            respone = ssd.respone;
            servers = ssd.servers;

            request = new C2GLoginRequest(hotApp.my.gateRPC);

            // 注释事件
            Event.Subscribe(EventID.ConnectedGate, () => 
            {
                OnConnected();
            });
            Event.Subscribe<int>(EventID.Login_SelectServer, OnSelectServer);
        }

        IEnumerator EnterYield(int serverId)
        {
            SelectServerRequest ssr = new SelectServerRequest();
            ssr.code = respone.code;
            ssr.token = respone.token;
            ssr.serverid = serverId;

            //SelectServerRespone ssrd = null;
            //request.SelectServer(ssr, (error, data) => { ssrd = data; });
            //while (ssrd == null)
            //    yield return 0;

            //var chars = ssrd.chars;

            var yyd = request.SelectServerYield(ssr);
            yield return yyd.yield;

            if (yyd.code != wProtobuf.RPC.Error.Success)
                yield break;

            if (yyd.result.result != SelectServerRespone.Result.SSRR_OK)
            {
                Debuger.ErrorLog("SelectServerYield result:{0}", yyd.result.result);
                yield break;
            }

            var chars = yyd.result.chars;
            App.my.localPlayer.BeginLogin(respone.token);
            parent.Level();
            if (chars.Count == 0)
            {
                //还没有角色
                App.my.appStateMgr.Enter(AppStateType.CreateCharacter, request);
                Event.fireEvent(EventID.Login_ShowCreateRole);
            }
            else
            {
                //已经有角色
                EnterSelectCharacter esc = new EnterSelectCharacter();
                esc.chars = chars;
                esc.request = request;
                App.my.appStateMgr.Enter(AppStateType.SelectCharacter, esc);
                Event.FireEvent(EventID.Login_ShowRoleList, chars);
            }
        }

        public void OnConnected()
        {
            //选择相应的服务器
            App.my.mainCoroutine.StartCoroutine(EnterYield(m_selectServerId));
            //App.my.mainCoroutine.StartCoroutine(EnterYield());
            //通知ui，连接上网关服,用于显示服务器列表
            App.my.eventSet.fireEvent(EventID.Login_OnConnectedGate);
        }

        //点击选择服务器
        void OnSelectServer(int serverId)
        {
            m_selectServerId = serverId;
            //连接网关服
            App.my.socket.Connect(respone.ip, respone.port);
        }
    }
}
#endif