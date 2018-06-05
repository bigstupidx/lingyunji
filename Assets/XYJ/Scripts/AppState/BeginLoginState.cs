#if !USE_HOT
using NetProto.Hot;
using System.Collections;

namespace xys.hot
{
    public class LoginData
    {
        public string user; // 用户名
        public string pass; // 密码
        public string ip; // IP地址
        public int port; // 端口号
    }

    class BeginLoginState : AppStateBase, ILoginEvent
    {
        public BeginLoginState(HotAppStateBase parent) : base(parent)
        {

        }

        LoginSocket loginSocket;

        // 进入此状态
        protected override void OnEnter(object p)
        {
            loginSocket = new LoginSocket();
            loginSocket.loginEvent = new RefType(new HotLoginSocket(loginSocket, this));

            Event.Release();
            Event.Subscribe<LoginData>(EventID.BeginLogin, BeginLoginBtn);

            App.my.mainCoroutine.StartCoroutine(YieldChangeScene());
        }

        IEnumerator YieldChangeScene()
        {
            //切换到登录场景
            //if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "login")
            //{
            //    PackTool.SceneLoad sl = PackTool.SceneLoad.Load("login", null);
            //    while (!sl.isDone)
            //        yield return 0;
            //}

            yield return 0;

            App.my.uiSystem.ShowPanel("UILoginPanel", null, true);

            loginSocket.SetState(LoginSocket.State.Null);
        }

        LoginData loginData;

        void BeginLoginBtn(LoginData ld)
        {
            loginData = ld;
            loginSocket.Connect(loginData.ip, loginData.port);
        }

        // 连接失败
        public void OnConnectedError()
        {
            Debuger.ErrorLog("OnConnectedError");
            loginSocket.SetState(LoginSocket.State.Null);
        }

        // 连接成功
        public LoginRequest OnConnected()
        {
            LoginRequest lr = new LoginRequest();
            lr.username = loginData.user;
            lr.pass = loginData.pass;

            return lr;
        }

        // 验证出错
        public void OnVerifyError()
        {
            Debuger.ErrorLog("OnVerifyError");
            loginSocket.SetState(LoginSocket.State.Null);
        }

        // 验证成功，下发服务器列表
        public void OnSuccess(LoginRespone respone, ServerInfoMap servers)
        {
            // 这里开始显示下服务器列表
            parent.Level();

            SelectServerData ssd = new SelectServerData() { respone = respone, servers = servers };
            App.my.appStateMgr.Enter(AppStateType.SelectServer, ssd);

            hotApp.my.eventSet.FireEvent(EventID.Login_ConnectLoginSucess, servers);
        }
    }
}
#endif