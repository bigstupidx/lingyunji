#if !USE_HOT
using NetProto;
using NetProto.Hot;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using xys.UI.State;
using Config;
using xys.UI;
using wTimer;

namespace xys.hot.UI
{
    class LoginPanel : HotPanelBase
    {
        public LoginPanel() : base(null)
        {

        }

        public LoginPanel(UIHotPanel parent) : base(parent)
        {

        }

        public static implicit operator bool (LoginPanel exists)
        {
            return exists.parent == null ? false : true;
        }

        public enum Sex : int
        {
            Male = 1,           //男
            Female = 2,         //女
        }

        public enum CharChangeType
        {
            Get,
            Add,
            Del,
        }

        //常量
        const int MAXCHARNUM = 4;           //最大角色数量
        const int MAXCHAR = 12;             //输入限制长度
        const int MAXROLE = 4;              //最大角色数量
        const int DIRECT_DEL_LEVEL = 20;    //直接删除等级
        const int WAIT_DEL_TIME = 120;      //等待删除时间

        const string ConstServerAddress = "219.135.155.90";

        [SerializeField]
        GameObject Bg1;
        [SerializeField]
        GameObject Bg2;
        [SerializeField]
        GameObject Bg3;
        [SerializeField]
        GameObject Login_account;           //用户名账户登录
        [SerializeField]
        GameObject Login_Choose;            //选服进入界面
        [SerializeField]
        GameObject Login_server_list;       //服务器列表界面
        [SerializeField]
        GameObject Login_enter_game;        //选角界面
        [SerializeField]
        GameObject Login_Choose_career;     //创角界面

        //初始界面
        [SerializeField]
        Button EnterBtn;          //进入按钮
        [SerializeField]
        Text ServerName;              //服务器名称
        [SerializeField]
        InputField ServerAddress;           //服务器地址
        [SerializeField]
        Button SerListBtn;        //服务器列表按钮

        //服务器列表界面
        [SerializeField]
        GameObject ServerListParent;    //服务器列表父节点
        [SerializeField]
        GameObject ServerItem;          //服务器列表创建对象
        //Image m_serverDragBg;        //服务器列表拖动背景

        //身份验证界面
        [SerializeField]
        InputField UserNameInput;       //用户名
        [SerializeField]
        InputField PasswordInput;       //密码
        [SerializeField]
        Button LoginBtn;                //登录按钮
        [SerializeField]
        Button SingleBtn;               //单机版按钮
        [SerializeField]
        Button WanBtn;                  //外网按钮
        [SerializeField]
        Button LanBtn;                  //内网按钮
        [SerializeField]
        Button PrivateBtn;              //私服按钮
        [SerializeField]
        InputField privateAddressInput; //私服地址
        [SerializeField]
        InputField portInput;           //端口号
        [SerializeField]
        GameObject privateAddress;      //地址的对象
        [SerializeField]
        GameObject password;            //密码对象

        //选择角色进入游戏界面
        [SerializeField]
        GameObject CharacterList;       //角色列表父节点
        [SerializeField]
        GameObject CharItem;            //角色列表创建对象
        [SerializeField]
        GameObject CreateBtn;           //创建角色按钮
        [SerializeField]
        Button SelectRoleBtn;           //选择角色进入游戏按钮
        [SerializeField]
        Button DeleteCharBtn;           //删除角色按钮
        [SerializeField]
        Button CharReturnBtn;           //选角返回按钮
        [SerializeField]
        Transform ChooseImage;          //选择展示的图片
        [SerializeField]
        StateRoot SelectCharIcon;       //选择美术字
        [SerializeField]
        Text DeleteDes;

        //创建角色界面
        [SerializeField]
        Button MaleBtn;                 //女性按钮
        [SerializeField]
        Button FemaleBtn;               //男性按钮
        [SerializeField]
        Button CreateCharBtn;           //创建角色按钮
        [SerializeField]
        InputField NameInput;           //角色名字
        [SerializeField]
        GameObject JobParent;           //职业列表父节点
        [SerializeField]
        GameObject JobItem;             //职业列表创建对象
        [SerializeField]
        Button JobRtnBtn;               //职业返回按钮
        //[SerializeField]
        //Transform ShowImage;          //用来展示的图片
        [SerializeField]
        Text Introduce;                 //职业介绍
        [SerializeField]
        StateRoot JobIconStateRoot;     //职业美术字
        [SerializeField]
        GameObject CreateLeft;          //左侧
        [SerializeField]
        GameObject CreateRight;         //右侧
        [SerializeField]
        GameObject CreateDown;          //底部
        [SerializeField]
        Button RandomNameBtn;           //随机名字按钮

        string m_port = "50001";
        string m_pwd = "";
        string m_name = "";                                             //用户名
        CreateCharVo m_createVo;                                        //当前创角对象
        private int m_selectServerId;                                   //当前选择的服务器id
        private string m_serverName = "";                               //当前服务器名称
        int charSelectIndex;                                            //当前选择的角色
        private string m_loginAddress = "";                             //登录地址
        int m_currentLoginServer = 0;                                   //当前登录服务器   外网，内网和私服 
        Sex m_currentSelectSex = Sex.Male;                              //当前选择的性别 
        Material m_grayMat;

        bool m_initSerList = false;                                     //是否初始化了服务器列表
        bool m_initJobList = false;                                     //是否初始化了职业列表
        GameObject m_curSelSer;                                         //当前选择服务器的对象
        GameObject m_curSelChar;                                        //当前选择角色的对象
        GameObject m_curSelJob;                                         //当前选择的职业对象
        int m_curSelJobId;                                              //当前选择的职业对象
        ModelPartManage m_model;                                        //模型展示
        GameObject m_createAnim;                                        //创角过场展示
        List<GameObject> m_charObjList = new List<GameObject>();        //角色对象列表
        List<GameObject> m_charCreateList = new List<GameObject>();     //角色创建按钮列表                       
        List<CharacterData> m_charList = new List<CharacterData>();     //角色列表
        ServerInfoMap m_servers;                                        //服务器map
        SimpleTimer m_timer;        //计时器
        Dictionary<long, int> m_delTimerDic = new Dictionary<long, int>();//删除角色的计时器
        int m_showDelHandler;

        #region msg
        void RegistMsg()
        {
            //注册事件
            Event.Subscribe<ServerInfoMap>(EventID.Login_ConnectLoginSucess, OnConnectedLogin);
            Event.Subscribe(EventID.Login_OnConnectedGate, OnConnectedGate);
            Event.Subscribe(EventID.Login_ShowCreateRole, ShowCreateRole);
            Event.Subscribe<List<CharacterData>>(EventID.Login_ShowRoleList, ShowRoleList);
            Event.Subscribe(EventID.Login_SelectRoleSucess, SelectRoleSucess);
            Event.Subscribe<long>(EventID.Login_DeleteRoleRet, OnDeleteRole);
            Event.Subscribe<long>(EventID.Login_WaitDeleteRoleRet, OnWaitDeleteRole);
            Event.Subscribe<long>(EventID.Login_RestoreRoleRet, OnRestoreRole);
            Event.Subscribe<object>(EventID.Login_Disguise2Login, OnDisguise2Login);
        }

        //连接登录成功，连接gate
        void OnConnectedLogin(ServerInfoMap servers)
        {
            Debug.Log("登录连接成功回调");
            m_servers = servers;
            Login_account.SetActive(false);
            Login_Choose.SetActive(true);
            Debug.Log("ShowServerUIBegin");
            ShowServerUI();
            Debug.Log("ShowServerUIEnd");
        }

        //登录成功，显示服务器列表界面
        void OnConnectedGate()
        {
            //m_loginStatus = LoginStatus.SelectSServer;
        }

        //显示创建角色界面
        void ShowCreateRole()
        {
            if (null == m_createVo)
                m_createVo = new CreateCharVo();
            SelecrServerSucess();
        }

        //显示角色列表
        void ShowRoleList(List<CharacterData> chars)
        {
            m_charList = chars;

            for (int i = m_charList.Count - 1; i >= 0; --i)
            {
                CharacterData data = m_charList[i];
                //时间超过等待删除的时间则表示已经被删除
                if (data.deleteTime > 0 && data.deleteTime <= WAIT_DEL_TIME)
                {
                    int handler = m_timer.Register<CharacterData>(1, int.MaxValue, DeleteRoleTimer, data);
                    m_delTimerDic.Add(data.charid, handler);
                }
                else if(data.deleteTime > WAIT_DEL_TIME)
                {
                    //删除角色
                    //Event.FireEvent(EventID.Login_DeleteRole, data);
                    m_charList.RemoveAt(i);
                }
            }

            SelecrServerSucess();
        }

        //选择服务器成功
        void SelecrServerSucess()
        {
            //缓存选择的服务器
            PlayerPrefs.SetInt("SwardLastServerId", m_selectServerId);

            Login_Choose.SetActive(false);
            Login_account.SetActive(false);

            if (m_charList.Count == 0 && App.my.appStateMgr.curState == AppStateType.SelectCharacter)
            {
                //若没有还存在的角色，则进入创角
                Event.fireEvent(EventID.Login_EnterCreate);
            }
            
            ShowCharacterChange();
        }

        //删除角色的计时器
        void DeleteRoleTimer(CharacterData data)
        {
            data.deleteTime++;

            if (data.deleteTime > WAIT_DEL_TIME)
            {
                //请求直接删除角色
                //Event.FireEvent(EventID.Login_DeleteRole, data);
                m_charList.Remove(data);
                //删除该计时器
                m_timer.Cannel(m_delTimerDic[data.charid]);

                //只有选角界面打开的情况下才能刷新
                if (Login_enter_game.activeSelf)
                    ShowCharacterChange();
            }

            if(Login_enter_game.activeSelf && m_charList[charSelectIndex].charid == data.charid)
            {
                string des = TipsContent.Get(3214) != null ? TipsContent.Get(3214).des : "";
                DeleteDes.text = string.Format(des, WAIT_DEL_TIME - data.deleteTime);
            }
        }

        //获取等待删除角色的剩余事件
        int GetWaiDelTime(long charId)
        {
            for (int i = 0; i < m_charList.Count; ++i)
            {
                if (m_charList[i].charid == charId)
                {
                    int passTime = m_charList[i].deleteTime;
                    return WAIT_DEL_TIME - passTime;
                }
            }
            return -1;
        }

        //选择角色成功
        void SelectRoleSucess()
        {
            OnEnterGame();
        }
        #endregion

        protected override void OnInit()
        {
            m_timer = new SimpleTimer(App.my.mainTimer);
            m_grayMat = Resources.Load("UIGray") as Material;

            m_name = PlayerPrefs.GetString("SwordAccount", "输入账号");
            m_pwd = PlayerPrefs.GetString("SwardPwd", "输入密码");
            m_loginAddress = PlayerPrefs.GetString("SwordLoginAddress", "输入地址");
            m_selectServerId = PlayerPrefs.GetInt("SwardLastServerId", -1);

            //对应组件


            //注册按钮事件
            EnterBtn.onClick.AddListener(StartOnClickEnter);       //初始界面点击进入
            SerListBtn.onClick.AddListener(StartOnClickSerList);   //点击进入服务器列表

            LoginBtn.onClick.AddListener(OnClickLogin);            //点击登录
            SingleBtn.onClick.AddListener(OnClickSingle);          //点击单机版
            WanBtn.onClick.AddListener(OnClickWan);                //点击外网
            LanBtn.onClick.AddListener(OnClickLan);                //点击内网
            PrivateBtn.onClick.AddListener(OnClickPrivate);        //点击私服

            SelectRoleBtn.onClick.AddListener(OnClickEnterGame);            //点击进入游戏
            DeleteCharBtn.onClick.AddListener(OnClickDeleteBtn);           //点击删除角色
            CharReturnBtn.onClick.AddListener(OnClickSelectReturn);
            MaleBtn.onClick.AddListener(() => { OnClickMale(); });                      //点击男
            FemaleBtn.onClick.AddListener(() => { OnClickFemale(); });                  //点击女
            CreateCharBtn.onClick.AddListener(OnClickMakeFace);             //点击创角
            JobRtnBtn.onClick.AddListener(OnClickCreateReturn);            //点击返回
            RandomNameBtn.onClick.AddListener(RandomName);

            UserNameInput.onValueChanged.AddListener(InputUserName);
            PasswordInput.onValueChanged.AddListener(InputPassword);
            privateAddressInput.onValueChanged.AddListener(InputPrivateAddress);
            portInput.onValueChanged.AddListener(InputPort);
            NameInput.onValueChanged.AddListener(InputName);

            m_model = new ModelPartManage();

            //开始，显示登录界面
            ShowAccountUI();

            RoleSkinConfig.Cache();
        }

        protected override void OnShow(object args)
        {
            RegistMsg();

            App.my.eventSet.Subscribe(EventID.Login_FinishCg, OnFinishCg);
            //Utils.EventDispatcher.Instance.AddEventListener(EventDefine.SceneEvents.FinishCg, OnFinishCg);
            //Utils.EventDispatcher.Instance.AddEventListener<List<characterInfo>, LoginSystem.CharChangeType>(LoginSystem.Event.CharacterListChange, OnCharacterListChange);
            //Utils.EventDispatcher.Instance.AddEventListener(LoginSystem.Event.EnterGame, OnEnterGame);
            //数据初始化
            m_charList = new List<CharacterData>();
            m_charObjList = new List<GameObject>();
            m_charCreateList = new List<GameObject>();
            //m_jobList = new List<GameObject>();

            //初始化界面
            Login_account.SetActive(true);
            Login_Choose.SetActive(false);
            Login_server_list.SetActive(false);
            Login_Choose_career.SetActive(false);
            Login_enter_game.SetActive(false);

            //m_rtt = new RttView("RTT", new Vector3(-1000, 0, 0), null, ChooseImage);
            //m_createPlr = new PrefabsLoadReference();

            //if (m_createPlr.IsLoad() || m_createPlr.IsLoading())
            //{
            //    m_createPlr.SetDestroy();
            //}
            //m_createRtt = new RttView("", m_showImage);

            //Main.Instance.cameraManager.m_defaultCamera.gameObject.SetActive(false);

            m_currentSelectSex = Sex.Male;

            //设置背景
            SetBg(1);

            //Joystick.instance.gameObject.SetActive(false);

            DeleteCamera();
        }

        protected override void OnHide()
        {
            //Utils.EventDispatcher.Instance.RemoveEventListener(EventDefine.SceneEvents.FinishCg, OnFinishCg);
            //Utils.EventDispatcher.Instance.RemoveEventListener<List<characterInfo>, LoginSystem.CharChangeType>(LoginSystem.Event.CharacterListChange, OnCharacterListChange);
            //Utils.EventDispatcher.Instance.RemoveEventListener(LoginSystem.Event.EnterGame, OnEnterGame);

            m_curSelJob = null;
            m_currentSelectSex = Sex.Male;
            m_initJobList = false;
            m_initSerList = false;
            ChooseImage.gameObject.SetActive(false);
            //m_rtt.SetDestroy();
            //m_createPlr.SetDestroy();
            //m_createRtt.SetDestroy();
            GameObject.Destroy(m_createAnim);
            ClearChooseChar();
            ClearJobList();
            //Joystick.instance.gameObject.SetActive(true);;
        }

        protected override void OnDestroy()
        {
            if (parent != null)
            {
                parent.DestroySelf();
            }
            m_timer.Release();
        }

        //设置背景
        void SetBg(int index)
        {
            if (1 == index)
            {
                Bg1.SetActive(true);
                Bg2.SetActive(false);
                Bg3.SetActive(false);
            }
            else if (2 == index)
            {
                Bg1.SetActive(false);
                Bg2.SetActive(true);
                Bg3.SetActive(false);
            }
            else if (3 == index)
            {
                Bg1.SetActive(false);
                Bg2.SetActive(false);
                Bg3.SetActive(true);
            }
            else
            {
                Bg1.SetActive(false);
                Bg2.SetActive(false);
                Bg3.SetActive(false);
            }
        }

        #region 服务器界面
        //初始界面
        void ShowServerUI()
        {
            SetBg(3);
            if (null == m_servers)
                return;

            if (!m_servers.servers.ContainsKey(m_selectServerId))
            {
                //当前没有选择服务器
                if (m_servers != null && m_servers.servers != null)
                {
                    foreach (var itor in m_servers.servers)
                    {
                        //默认选择第一个
                        OnClickServerItem(itor.Value);
                        break;
                    }
                }
            }
            else
            {
                OnClickServerItem(m_servers.servers[m_selectServerId]);
            }

            ServerName.text = m_serverName;
        }

        //初始界面点击进入
        void StartOnClickEnter()
        {
            //m_selectCharacterUI.SetActive(true);
            //ShowAccount();
            if (-1 == m_selectServerId)
            {
                Debuger.LogError("当前没有选择服务器");
                return;
            }
            //选择服务器
            //m_controller.RequestSelectServer(m_selectServerId);
            Event.FireEvent(EventID.Login_SelectServer, m_selectServerId);
            //MainNet.ConnectToServer(m_address, m_serverDic[m_address].gates[0].gateport, OnConnedtedGame, OnConnedtGameFail);
        }

        //游戏服务器连接失败
        void OnConnedtGameFail(string msg, System.Net.Sockets.SocketError error)
        {
            Debuger.LogError("GameServer OnConnedtFail:" + (string.IsNullOrEmpty(msg) ? "" : msg));
            SystemHintMgr.ShowTipsHint(3211);
        }

        // 游戏登录服务器成功
        //void OnConnedtedGame()
        //{
        //    this.StartCoroutine(SysClientTime());
        //}

        ////在连接游戏服的时候对时
        //IEnumerator SysClientTime()
        //{
        //    yield return BattleSystem.SysClientTime();

        //    Debuger.DebugLog("GameServer OnConnedted()");
        //    //请求角色列表
        //    Utils.EventDispatcher.Instance.TriggerEvent(LoginSystem.Event.QuestChar);
        //    //ShowEnter();
        //    Login_Choose.SetActive(false);
        //}


        //点击进入服务器列表界面
        void StartOnClickSerList()
        {
            Login_Choose.SetActive(false);
            Login_server_list.SetActive(true);
            ShowServerListUI();
        }

        //输入ip地址
        void InputAddress(string text)
        {
        }
        #endregion

        #region 服务器列表界面
        //服务器列表界面
        void ShowServerListUI()
        {
            SetBg(3);
            if (m_initSerList)
                return;

            if (null != m_servers)
            {
                foreach (var itor in m_servers.servers)
                {
                    //if (GUILayout.Button(itor.Value.name))
                    //{
                    //    m_controller.RequestSelectServer(itor.Value.id);
                    //}
                    int serverId = itor.Value.id;
                    string serverName = itor.Value.name;

                    GameObject go = Object.Instantiate(ServerItem) as GameObject;
                    go.transform.SetParent(ServerListParent.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    go.SetActive(true);
                    go.name = serverId.ToString();
#if COPYRIGHT_EDITION
            go.transform.FindChild("Label").GetComponent<Text>().text = "剑之源";
#else
                    go.transform.Find("Label").GetComponent<Text>().text = serverName;
#endif
                    go.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Login_Choose.SetActive(true);
                        Login_server_list.SetActive(false);
                        if (null != m_curSelSer)
                        {
                            m_curSelSer.GetComponent<StateRoot>().SetCurrentState(0, true);
                        }
                        go.GetComponent<StateRoot>().SetCurrentState(1, true);
                        m_curSelSer = go;

                        OnClickServerItem(itor.Value);
                        ShowServerUI();
                    });
                }
            }

            m_initSerList = true;
        }

        void OnClickServerItem(ServerInfoMap.Server server)
        {
#if COPYRIGHT_EDITION
            m_serverName = "剑之源";
#else
            m_serverName = server.name;
#endif
            m_selectServerId = server.id;
        }
        #endregion

        #region 身份验证界面
        //身份验证界面
        void ShowAccountUI()
        {
            SetBg(1);
#if COPYRIGHT_EDITION
        //版署宏
        UserNameInput.text = m_name;
        privateAddress.SetActive(false);
        password.SetActive(false);
        OnClickWan();
#else
            UserNameInput.text = m_name;
            privateAddressInput.text = m_loginAddress;
            portInput.text = m_port;
            PasswordInput.text = m_pwd;
            //OnClickPrivate();
#endif
        }

        //输入用户名
        void InputUserName(string name)
        {
            m_name = name;
            Debuger.Log("m_name : " + m_name);
        }

        //输入密码
        void InputPassword(string text)
        {
            m_pwd = text;
        }

        //输入私服地址
        void InputPrivateAddress(string text)
        {
            m_loginAddress = text;
        }

        void InputPort(string text)
        {
            m_port = text;
        }

        //点击登录
        void OnClickLogin()
        {
            PlayerPrefs.SetString("SwordAccount", m_name);
            PlayerPrefs.SetString("SwordLoginAddress", m_loginAddress);
            PlayerPrefs.SetString("SwardPwd", m_pwd);

            //连接EBLogin服务器  TODO
            string address = m_loginAddress;
            int port = int.Parse(m_port);

            if (string.IsNullOrEmpty(address))
            {
                Debuger.LogError("地址为空，请先填写地址");
            }
            else if (string.IsNullOrEmpty(m_name))
            {
                Debuger.LogError("请填写用户名");
                SystemHintMgr.ShowTipsHint(3212);
            }
            else if (string.IsNullOrEmpty(m_pwd))
            {
                Debuger.LogError("密码不能为空");
            }
            else
            {
                LoginData ld = new LoginData();
                ld.ip = address;
                ld.port = port;
                ld.user = m_name;
                ld.pass = m_pwd;
                Event.FireEvent(EventID.BeginLogin, ld);
                //m_controller.ConnectToLoginSer(m_name, m_pwd, address, port);
                //MainNet.ConnectToServer(address, port, OnConnedted, OnConnedtFail, ServerType.Login);
            }
        }

        //点击单机版
        void OnClickSingle()
        {
            //MainNet.SetTestNoNet(true);
            //MainNet.SetLevelType(true);
            ////Hide(true);
            //UI.UISystem.Instance.HidePanel<UILoginPanel>(true);

            //LevelSystem.Instance.BackToCity();
            ////单人也要发送选人消息
            //Utils.EventDispatcher.Instance.TriggerEvent<int>(LoginSystem.Event.SelectCharacter, 0);
            ////创建单人角色
            ////吧自己先添加到周围玩家表里面去
            //ObjectDataManage.instance.OnSelectCharacter(ObjectDataManage.instance.mainPlayerID);
            //Battle.ObjectCreateFactory.InitObjectTableByLocal(ObjectDataManage.instance.mainPlayerID, 1);
        }

        //点击外网
        void OnClickWan()
        {
            privateAddressInput.text = ConstServerAddress;
            m_currentLoginServer = 0;
            m_loginAddress = privateAddressInput.text;
            PrivateBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
            WanBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
            LanBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
        }

        //点击内网
        void OnClickLan()
        {
            privateAddressInput.text = "192.168.1.29";
            m_currentLoginServer = 1;
            PrivateBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
            WanBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
            LanBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
        }

        //点击私服
        void OnClickPrivate()
        {
            privateAddressInput.text = m_loginAddress;
            m_currentLoginServer = 2;
            PrivateBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
            WanBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
            LanBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
        }

        #endregion

        #region 创角界面
        void ClearJobList()
        {
            //for(int i = 0; i < m_jobList.Count; ++i)
            //{
            //    Destroy(m_jobList[i]);
            //    m_jobList[i] = null;
            //}
            //m_jobList.Remove(null);
            //m_jobList.Clear();
            //m_jobList = null;
        }

        //创角界面
        void ShowCreateCharUI()
        {
            //m_rtt.SetRenderActive(false);
            SetBg(0);
            App.my.mainCoroutine.StartCoroutine(ShowCreateCharUIYield());
        }

        System.Collections.IEnumerator ShowCreateCharUIYield()
        {
            PackTool.SceneLoad sl = PackTool.SceneLoad.Load("Level_Chuangjue", null);
            while (!sl.isDone)
                yield return 0;

           Event.FireEvent(EventID.FinishLoadSceneParam, false);

            //Login_Choose_career.SetActive(true);
            Debuger.Log("进入创建角色界面");
            m_createVo = new CreateCharVo();
            m_createVo.name = "";

            if (m_initJobList)
            {
                int id = m_curSelJobId != 0 ? m_curSelJobId : 1;
                GameObject go = JobParent.transform.Find(id.ToString()).gameObject;
                OnClickJob(go);
                yield break; ;
            }

            int index = 0;
            //获取职业列表
            Dictionary<int, RoleJob> dataList = RoleJob.GetAll();
            if (null != dataList)
            {
                foreach (var itor in dataList)
                {
                    GameObject go = JobParent.transform.Find(itor.Value.id.ToString()).gameObject;
                    //GameObject go = Instantiate(m_jobItem) as GameObject;
                    //go.transform.SetParent(m_jobParent.transform);
                    //go.transform.localScale = Vector3.one;
                    //go.transform.localPosition = m_jobPos[index];
                    //go.SetActive(true);
                    //go.name = pair.Key.ToString();

                    SetJobItem(go, itor.Value, false);

                    //UIEventListener.Get(go).onClick = OnClickJob;
                    //绑定方法
                    go.AddMissingComponent<Button>().onClick.RemoveAllListeners();
                    go.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        OnClickJob(go);
                    });

                    if (0 == index)
                    {
                        OnClickJob(go);
                    }

                    index++;
                    //m_jobList.Add(go);
                }
            }

            m_initJobList = true;
        }

        void HideCreateUI()
        {
            Login_Choose_career.SetActive(false);
        }

        void ShowCreateUI()
        {
            if(App.my.appStateMgr.curState == AppStateType.CreateCharacter)
                Login_Choose_career.SetActive(true);
        }

        //点击职业
        void OnClickJob(GameObject go)
        {
            //判断职业是否开放
            int jobId = int.Parse(go.name);

            RoleJob data = RoleJob.Get(jobId);
            if (!data.isOpen)
            {
                SystemHintMgr.ShowTipsHint(3208);
                return;
            }

            if (null != m_curSelJob)
            {
                if (go.GetInstanceID() == m_curSelJob.GetInstanceID())
                {
                    return;
                }

                int id = int.Parse(m_curSelJob.name);
                RoleJob curData = RoleJob.Get(id);

                SetJobItem(m_curSelJob, curData, false);
            }

            //选择角色
            m_createVo.jobId = int.Parse(go.name);
            SetJobItem(go, data, true);

            m_curSelJob = go;

            //判断性别是否开放
            if (data.maleId != 0)
            {
                MaleBtn.GetComponent<Image>().material = null;
            }
            else
            {
                MaleBtn.GetComponent<Image>().material = m_grayMat;
            }

            if (data.felmaleId != 0)
            {
                FemaleBtn.GetComponent<Image>().material = null;
            }
            else
            {
                FemaleBtn.GetComponent<Image>().material = m_grayMat;
            }

            if (m_currentSelectSex == Sex.Male)
            {
                //当前选择的性别是男性，切换角色的时候，优先男性
                if (0 != data.maleId)
                    OnClickMale(true);
                else if (0 != data.felmaleId)
                    OnClickFemale(true);
            }
            else
            {
                //当前选择的性别是女性，切换角色的时候，优先女性
                if (0 != data.felmaleId)
                    OnClickFemale(true);
                else if (0 != data.maleId)
                    OnClickMale(true);
            }
        }

        //点击男
        void OnClickMale(bool changeRole = false)
        {
            if (null == m_curSelJob)
            {
                Debuger.LogError("点击男 当前选择职业为空");
                return;
            }
            int jobId = int.Parse(m_curSelJob.name);
            RoleJob data = RoleJob.Get(jobId);
            if (null == data)
            {
                Debuger.LogError("点击男 职业表为空 id : " + jobId);
                return;
            }

            if (m_createVo.sex == (int)Sex.Male && !changeRole)
            {
                return;
            }

            if (data.maleId <= 0)
            {
                Debuger.LogError("该职业没有男性");
                SystemHintMgr.ShowTipsHint(3209);
                return;
            }

            MaleBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
            FemaleBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
            m_createVo.sex = (int)Sex.Male;

            Introduce.text = data.maleIntroduce;
            SetRandomName(true);
            //int objId = data.maleId;
            //ShowJob(objId);
            m_currentSelectSex = Sex.Male;
            ShowCreateJob(data);
        }

        //点击女
        void OnClickFemale(bool changeRole = false)
        {
            if (null == m_curSelJob)
            {
                Debuger.LogError("点击男 当前选择职业为空");
                return;
            }
            int jobId = int.Parse(m_curSelJob.name);

            RoleJob data = RoleJob.Get(jobId);
            if (null == data)
            {
                Debuger.LogError("点击男 职业表为空 id : " + jobId);
                return;
            }

            if (m_createVo.sex == (int)Sex.Female && !changeRole)
            {
                return;
            }

            if (data.felmaleId <= 0)
            {
                Debuger.LogError("该职业没有女性");
                SystemHintMgr.ShowTipsHint(3210);
                return;
            }

            MaleBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
            FemaleBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
            m_createVo.sex = (int)Sex.Female;

            Introduce.text = data.femaleIntroduce;
            SetRandomName(false);
            //int objId = data.felmaleId;
            //ShowJob(objId);
            m_currentSelectSex = Sex.Female;
            ShowCreateJob(data);
        }

        void ShowCreateJob(RoleJob data)
        {
            //先隐藏ui
            HideCreateUI();
            JobIconStateRoot.SetCurrentState(data.id - 1, true);
            GameObject.Destroy(m_createAnim);
            string anim = m_currentSelectSex == Sex.Male ? data.maleCreateAnim : data.femaleCreateAnim;
            ArtResLoad.LoadCG(anim, LoadCgFinish);
        }

        void LoadCgFinish(GameObject go, object para)
        {
            m_createAnim = go;
            go.transform.localPosition = new Vector3(0, 0, 0);
        }
        
        //点击捏脸
        void OnClickMakeFace()
        {
            m_curSelJob = null;
            m_curSelJobId = 0;
            App.my.mainCoroutine.StartCoroutine(ShowMakeFaceYield());
        }

        System.Collections.IEnumerator ShowMakeFaceYield()
        {
            PackTool.SceneLoad sl = PackTool.SceneLoad.Load("Level_Nielianjiemian", null);
            while (!sl.isDone)
                yield return 0;

            Event.FireEvent(EventID.FinishLoadScene, false);

            RoleJob jobCfg = RoleJob.Get(m_createVo.jobId);
            int id = 1 == m_createVo.sex ? jobCfg.maleId : jobCfg.felmaleId;
            m_createVo.roleId = id;
            App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIFaceMakePanel, m_createVo);
        }

        //点击创角
        void OnClickCreateChar()
        {
            if (string.IsNullOrEmpty(m_createVo.name))
            {
                SystemHintMgr.ShowTipsHint(3204);
                return;
            }

            //需要经过字库筛选
            //TODO 屏蔽字检查
            //if (ChatTextParser.IsInputContainsSensitiveWord(m_createVo.name))
            //{
            //    UIHintManage.Instance.ShowHint("含有屏蔽词");
            //    return;
            //}

            //不可单独使用数字或特殊符号作为角色名，单纯为数字与符号组合也不行
            bool check = false;
            char[] charList = m_createVo.name.ToCharArray();
            for (int i = 0; i < charList.Length; ++i)
            {
                //必须包含中文或者字母
                if (Helper.CheckStringChineseReg(charList[i]) || Helper.CheckStringCharacterReg(charList[i]))
                {
                    check = true;
                }
            }
            if (!check)
            {
                SystemHintMgr.ShowTipsHint(3204);
                return;
            }

            if (m_charList.Count > MAXROLE)
            {
                Debuger.LogError("已经满" + MAXROLE + "个角色");
                return;
            }

            /*m_createVo.name = string.IsNullOrEmpty(m_createVo.name) ? m_name + Random.Range(1, 99999) : m_createVo.name;*/
            Debuger.Log("发送创建角色 昵称 : " + m_createVo.name + " 性别 : " + m_createVo.sex + " 职业 : " + m_createVo.jobId);
            if (0 == m_createVo.sex)
            {
                Debuger.LogError("数据不正确");
                return;
            }
            //请求创建角色
            //m_controller.RequestCreateChar(m_createVo.jobId, m_createVo.name);
            Event.FireEvent(EventID.Login_CreateRole, m_createVo);
            //Utils.EventDispatcher.Instance.TriggerEvent<string, int, int>(LoginSystem.Event.CreateCharacter,
            //    m_createVo.name, m_createVo.gender, m_createVo.jobId);c
        }

        //点击返回
        void OnClickCreateReturn()
        {
            //判断当前有没有角色
            if (0 == m_charList.Count)
            {
                //没有角色返回选服界面，断开游戏服
                Login_Choose_career.SetActive(false);
                Login_account.SetActive(true);

                SetBg(1);
                App.my.socket.Close();
                App.my.appStateMgr.Enter(AppStateType.BeginLogin, null);
            }
            else
            {
                //有角色返回角色列表界面
                //Login_enter_game.SetActive(true);
                Login_Choose_career.SetActive(false);
                ShowChooseCharUI();

                Event.fireEvent(EventID.Login_EnterChoose);
            }

            if (null != m_curSelJob)
            {
                int id = int.Parse(m_curSelJob.name);
                RoleJob curData = RoleJob.Get(id);

                SetJobItem(m_curSelJob, curData, false);
                m_curSelJob = null;
                m_curSelJobId = 0;
            }
            GameObject.Destroy(m_createAnim);
        }

        void RandomName()
        {
            bool isMale = m_currentSelectSex == Sex.Male;
            SetRandomName(isMale);
        }

        //随机名字
        void SetRandomName(bool isMale)
        {
            string name = GetRandomName(isMale);
            NameInput.text = name;
            m_createVo.name = name;
        }

        string GetRandomName(bool isMale)
        {
            Dictionary<int, RoleRandomName> dataList = RoleRandomName.GetAll();

            string name = "";
            int index = 0;
            List<RoleRandomName> list = new List<RoleRandomName>();
            foreach (var itor in dataList)
            {
                list.Add(itor.Value);
            }

            if (null != list && list.Count > 0)
            {
                index = Random.Range(0, list.Count - 1);

                while (string.IsNullOrEmpty(list[index].firstName))
                {
                    index = Random.Range(0, list.Count - 1);
                }
                name += list[index].firstName;
                if (isMale)
                {
                    //男角色
                    while (string.IsNullOrEmpty(list[index].maleName))
                    {
                        index = Random.Range(0, list.Count - 1);
                    }
                    name += list[index].maleName;
                }
                else
                {
                    //女角色
                    while (string.IsNullOrEmpty(list[index].femaleName))
                    {
                        index = Random.Range(0, list.Count - 1);
                    }
                    name += list[index].femaleName;
                }
            }

            return name;
        }

        //输入名字
        void InputName(string text)
        {
            //字符数限制
            if (Helper.TextLimit(ref text, MAXCHAR))
            {
                SystemHintMgr.ShowTipsHint(3201);
            }

            NameInput.text = text;
            m_createVo.name = text;
        }

        //职业
        void SetJobItem(GameObject go, RoleJob data, bool select, bool show = false)
        {
            go.GetComponent<Image>().SetNativeSize();
            GameObject icon = go.transform.Find("icon").gameObject;
            int state = data.id - 1;
            //if (!select)
            //    state += 6;
            icon.GetComponent<StateRoot>().SetCurrentState(state, true);
            if (data.isOpen)
            {
                //激活状态
                go.GetComponent<StateRoot>().SetCurrentState(0, true);
                icon.GetComponent<Image>().material = null;

            }
            else
            {
                //未激活状态
                go.GetComponent<StateRoot>().SetCurrentState(2, true);
                icon.GetComponent<Image>().material = m_grayMat;

            }
            go.GetComponent<StateRoot>().SetCurrentState(select ? 1 : 0, true);

            go.transform.Find("icon").GetComponent<Image>().SetNativeSize();
            //go.transform.FindChild("icon").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(70, 70);
        }
        #endregion

        #region 选角进入界面
        //选角进入界面
        void ClearChooseChar()
        {
            //m_rtt.SetRenderActive(true);
            for (int i = m_charObjList.Count - 1; i >= 0; --i)
            {
                Object.Destroy(m_charObjList[i]);
                m_charObjList[i] = null;
            }
            m_charObjList.Remove(null);
            m_charObjList.Clear();

            for (int i = m_charCreateList.Count - 1; i >= 0; --i)
            {
                Object.Destroy(m_charCreateList[i]);
                m_charCreateList[i] = null;
            }
            m_charCreateList.Remove(null);
            m_charCreateList.Clear();
        }

        void ShowChooseCharUI()
        {
            SetBg(0);
            App.my.mainCoroutine.StartCoroutine(ShowChooseYield());
        }

        System.Collections.IEnumerator ShowChooseYield()
        {
            var loadingMgr = App.my.uiSystem.loadingMgr;
            //判断是否需要切换场景
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "level_renwujiemian")
            {
                loadingMgr.Show();
                //切换到登录场景
                PackTool.SceneLoad sl = PackTool.SceneLoad.Load("level_renwujiemian", null);
                while (!sl.isDone)
                {
                    loadingMgr.progress = sl.progress;
                    yield return 0;
                }

                Event.FireEvent(EventID.FinishLoadSceneParam, false);
            }

            Debuger.Log("选角进入界面 角色列表长度 ： " + m_charList.Count);
            m_curSelChar = null;
            Login_enter_game.SetActive(true);
            ClearChooseChar();

            int index = 0;
            bool hasLoginChar = false;
            //创建角色列表
            for (int i = 0; i < m_charList.Count; ++i)
            {
                if (i > 3)
                    continue;
                GameObject go = Object.Instantiate(CharItem) as GameObject;
                go.transform.SetParent(CharacterList.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.SetActive(true);
                go.name = i.ToString();

                int jobId = m_charList[i].career;
                int gender = m_charList[i].sex;
                RoleJob data = RoleJob.Get(jobId);

                SetCharItem(go, data, m_charList[i], false);

                //UIEventListener.Get(go).onClick = OnClickChooseChar;
                go.AddMissingComponent<Button>().onClick.RemoveAllListeners();
                go.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickChooseChar(go);
                });

                m_charObjList.Add(go);

                long charId = 0;
                string character = PlayerPrefs.GetString("loginCharId");
                if (long.TryParse(character, out charId))
                {
                    if (charId == m_charList[i].charid)
                    {
                        hasLoginChar = true;
                        OnClickChooseChar(go);
                    }
                }
                if (m_charList.Count - 1 == i && !hasLoginChar)
                {
                    OnClickChooseChar(go);
                }
                index++;
            }

            //创建角色创建按钮，  数量为4-角色个数
            for (int i = 0; i < MAXCHARNUM - m_charList.Count; ++i)
            {
                GameObject go = Object.Instantiate(CreateBtn) as GameObject;
                go.transform.SetParent(CharacterList.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.SetActive(true);

                //UIEventListener.Get(go).onClick = OnClickEnterCreateChar;
                go.AddMissingComponent<Button>().onClick.RemoveAllListeners();
                go.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickEnterCreateChar(go);
                });
                m_charCreateList.Add(go);
                index++;
            }
            loadingMgr.Hide();
        }

        //职业
        void SetCharItem(GameObject go, RoleJob data, CharacterData info, bool select)
        {
            if (null == data /*|| null == info*/)
            {
                Debuger.LogError("职业显示错误 null == data || null == info ");
                return;
            }
            go.GetComponent<StateRoot>().SetCurrentState(select ? 1 : 0, true);
            go.GetComponent<Image>().SetNativeSize();
            StateRoot iconStateRoot = go.transform.Find("icon").GetComponent<StateRoot>();
            if (null != iconStateRoot)
            {
                int state = data.id - 1;
                if (!select)
                    state += 6;
                iconStateRoot.SetCurrentState(state, true);
            }
            go.transform.Find("icon").GetComponent<Image>().SetNativeSize();
            //go.transform.FindChild("icon").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(70, 70);
            string head = info.sex == 1 ? data.maleIcon : data.femalIcon;
            Helper.SetSprite(go.transform.Find("Head").GetComponent<Image>(), head);

            GameObject selectObj = go.transform.Find("Selected").gameObject;
            selectObj.SetActive(true);
            //if(select)
            {
                selectObj.transform.Find("Name").GetComponent<Text>().text = info.name;
                selectObj.transform.Find("Level").GetComponent<Text>().text = info.level.ToString() + "级";
            }

            //float alpha = select ? 1 : 0.7f;
            //Graphic[] graphics = go.transform.GetComponentsInChildren<Graphic>();
            //for (int i = 0; i < graphics.Length; ++i)
            //{
            //    graphics[i].color = new Color(graphics[i].color.r, graphics[i].color.g, graphics[i].color.b, alpha);
            //}
        }

        void OnClickChooseChar(GameObject go)
        {
            if (m_curSelChar == go)
                return;


            //重置当前对象
            if (null != m_curSelChar)
            {
                int index = int.Parse(m_curSelChar.name);
                int curJobId = m_charList[index].career;
                RoleJob curData = RoleJob.Get(curJobId);
                SetCharItem(m_curSelChar, curData, m_charList[index], false);
            }

            //获取数据
            charSelectIndex = int.Parse(go.name);
            var charInfo = m_charList[charSelectIndex];
            int jobId = charInfo.career;
            int gender = charInfo.sex;

            RoleJob jobCfg = RoleJob.Get(jobId);

            if (null == jobCfg)
            {
                Debuger.LogError("取不到职业数据 id : " + jobId);
                return;
            }
            SetCharItem(go, jobCfg, charInfo, true);
            m_curSelChar = go;
            SelectCharIcon.SetCurrentState(jobCfg.id - 1, true);

            //判断角色是否正在删除的过程中
            if (charInfo.deleteTime > 0 && charInfo.deleteTime <= WAIT_DEL_TIME)
            {
                //提示
                SystemHintMgr.ShowTipsHint(3206);
                DeleteDes.gameObject.SetActive(true);

                //显示恢复按钮
                DeleteCharBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
                //m_timer.Cannel(m_showDelHandler);
                //m_showDelHandler = m_timer.Register<long>(1, int.MaxValue, ShowDeleteTimer, charInfo.charid);
            }
            else
            {
                m_timer.Cannel(m_showDelHandler);
                DeleteDes.gameObject.SetActive(false);

                //显示删除按钮
                DeleteCharBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
            }

            //角色表id
            int id = 1 == gender ? jobCfg.maleId : jobCfg.felmaleId;
            Debuger.Log("选角进入  职业id : " + jobId + ", 性别 : " + gender + ", 角色表id : " + id + ", characterId: " + charInfo.charid);

            RoleDefine config = RoleDefine.Get(id);

            RoleDisguiseHandle disguiseHandle = new RoleDisguiseHandle ();

            disguiseHandle.SetRoleAppearance(id, charInfo.career, charInfo.sex, charInfo.appearance);
            m_model.LoadModelWithAppearance (disguiseHandle, ModelLoadEnd);
            m_model.PlayAnim("idle_1");
            //m_model.LoadModel(config.model, ModelLoadEnd);
        }

        void ShowDeleteTimer(long id)
        {
            string des = TipsContent.Get(3214) != null ? TipsContent.Get(3214).des : "";
            DeleteDes.text = string.Format(des, GetWaiDelTime(id));
        }

        void ModelLoadEnd(GameObject go)
        {
            go.transform.position = new Vector3(523.15f, 4.58f, -111.25f);
            GameObject map = GameObject.Find("MapScene");
            if (null != map)
            {
                Transform camera = map.transform.Find("[Camera]/MainCamera");
                if (null != camera)
                    go.transform.eulerAngles = new Vector3(0, -camera.eulerAngles.y, 0);
            }

            ChooseImage.GetComponent<RTTDrag>().SetDragObject(go.transform);
            //Quaternion.LookRotation
        }

        //点击创建角色
        void OnClickEnterCreateChar(GameObject go)
        {
            if (m_charList.Count >= MAXCHARNUM)
            {
                Debuger.LogError("已经超过创角上限");
                return;
            }
            Login_enter_game.SetActive(false);
            //Login_Choose_career.SetActive(true);
            ShowCreateCharUI();
            //SetBg(3);

            Event.fireEvent(EventID.Login_EnterCreate);
        }

        //点击进入游戏
        void OnClickEnterGame()
        {
            if (-1 == charSelectIndex)
            {
                Debuger.LogError("当前没有选中对象");
                return;
            }
            if (charSelectIndex >= m_charList.Count)
            {
                Debuger.LogError("点击进入游戏 index错误 : " + charSelectIndex);
                return;
            }

            CharacterData data = m_charList[charSelectIndex];
            if (data.deleteTime > 0 && data.deleteTime <= WAIT_DEL_TIME)
            {
                SystemHintMgr.ShowTipsHint(3206);
                return;
            }

            Debuger.Log("选择角色进入游戏 : " + data.charid);
            string loginCharId = (data.charid).ToString();
            PlayerPrefs.SetString("loginCharId", loginCharId);
            Event.FireEvent(EventID.Login_SelectRole, data);
        }

        //点击删除角色
        void OnClickDeleteBtn()
        {
            if (-1 == charSelectIndex)
            {
                Debuger.LogError("当前没有选中对象");
                return;
            }
            if (charSelectIndex >= m_charList.Count)
            {
                Debuger.LogError("点击删除角色 index错误 : " + charSelectIndex);
                return;
            }
            CharacterData data = m_charList[charSelectIndex];
            Debuger.Log("点击删除角色 : " + data.charid);

            //需要二次确认，20级以下确认直接删除，20级以上确认进入删除缓存
            if (data.level < DIRECT_DEL_LEVEL)
            {
                string des = TipsContent.Get(3205) != null ? TipsContent.Get(3205).des : "";
                des = string.Format(des, data.name);
                xys.UI.Dialog.TwoBtn.Show("", des, "取消", () => false, "确定", () =>
                {
                    Event.FireEvent(EventID.Login_DeleteRole, m_charList[charSelectIndex]);
                    return false;
                }, true, true);
            }
            else
            {
                //大于20级，判断当前是否正在删除中，正在删除则点击恢复，否则确认后删除
                if (data.deleteTime > 0 && data.deleteTime <= WAIT_DEL_TIME)
                {
                    //恢复角色
                    string des = TipsContent.Get(3207) != null ? TipsContent.Get(3207).des : "";
                    des = string.Format(des, GetWaiDelTime(data.charid) + "秒", data.name);
                    xys.UI.Dialog.TwoBtn.Show("", des, "取消", () => false, "确定", () =>
                    {
                        Event.FireEvent(EventID.Login_RestoreRole, m_charList[charSelectIndex]);
                        return false;
                    }, true, true);
                }
                else
                {
                    //删除角色
                    string des = TipsContent.Get(3205) != null ? TipsContent.Get(3205).des : "";
                    des = string.Format(des, data.name);
                    xys.UI.Dialog.TwoBtn.Show("", des, "取消", () => false, "确定", () =>
                    {
                        //延迟删除角色
                        Event.FireEvent(EventID.Login_WaitDeleteRole, m_charList[charSelectIndex]);
                        return false;
                    }, true, true);
                }
            }
        }

        //选角界面点击返回
        void OnClickSelectReturn()
        {
            //直接返回到登录界面，断开socket
            Login_enter_game.SetActive(false);
            Login_account.SetActive(true);

            //ShowServerUI();
            SetBg(1);
            App.my.socket.Close();
            App.my.appStateMgr.Enter(AppStateType.BeginLogin, null);

            m_model.Destroy();
            //断开游戏服
            //NetManage.Instance.Close(ServerType.Game);
        }
        #endregion


        #region 事件回调
        //角色列表变更回调
        void OnDeleteRole(long charId)
        {
            for (int i = m_charList.Count - 1; i >= 0; --i)
            {
                if (m_charList[i].charid == charId)
                    m_charList.RemoveAt(i);
            }
            if(m_charList.Count == 0)
            {
                Event.fireEvent(EventID.Login_EnterCreate);
            }
            ShowCharacterChange();
        }

        void OnWaitDeleteRole(long charId)
        {
            for (int i = m_charList.Count - 1; i >= 0; --i)
            {
                if (m_charList[i].charid == charId)
                {
                    m_charList[i].deleteTime = 1;

                    int handler = m_timer.Register<CharacterData>(1, int.MaxValue, DeleteRoleTimer, m_charList[i]);
                    m_delTimerDic.Add(m_charList[i].charid, handler);
                }
            }
            ShowCharacterChange();
        }

        //恢复角色
        void OnRestoreRole(long charId)
        {
            for (int i = m_charList.Count - 1; i >= 0; --i)
            {
                if (m_charList[i].charid == charId)
                {
                    m_charList[i].deleteTime = 0;
                    if (m_delTimerDic.ContainsKey(charId))
                    {
                        m_timer.Cannel(m_delTimerDic[charId]);
                    }
                }
            }
            ShowCharacterChange();
        }

        /// <summary>
        /// 从捏脸返回
        /// </summary>
        void OnDisguise2Login(object param)
        {
            HideCreateUI();
            object[] pList = (object[])param;
            m_curSelJobId = (int)pList[0];
            m_currentSelectSex = (Sex)((int)pList[1]);
            ShowCreateCharUI();
        }

        //角色列表改变
        void ShowCharacterChange()
        {
            if (null == m_charList || 0 == m_charList.Count)
            {
                Login_enter_game.SetActive(false);
                ShowCreateCharUI();
            }
            else
            {
                Login_Choose_career.SetActive(false);
                ShowChooseCharUI();
            }
        }

        //选择角色进入游戏成功
        void OnEnterGame()
        {
            //Hide(true);
            //Main.Instance.uiManager.HidePannel(ModuleDefine.Login);
        }

        //cg播放完
        void OnFinishCg()
        {
            ShowCreateUI();
        }
        #endregion

        public void DeleteCamera()
        {
            //删除多余相机
            GameObject MapScene = GameObject.Find("MapScene");
            if (MapScene == null)
                return;
            Camera[] cameras = MapScene.GetComponentsInChildren<Camera>();
            for (int i = 0; i < cameras.Length; ++i)
            {
                cameras[i].gameObject.SetActive(false);
            }
        }
    }
}
#endif