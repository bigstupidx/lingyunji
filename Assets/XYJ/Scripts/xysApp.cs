namespace xys
{
    using wRPC;
    using Network;
    using NetProto;
    using wProtobuf;
    using UnityEngine;
    using wProtobuf.RPC;
    using System.Collections;
    using System.Collections.Generic;
    using Config;
    using xys.battle;

#if USE_HOT
    using ILRuntime.Runtime.Enviorment;
    using Config;
#endif

    public class App
    {
        public static App my { get; private set; }

        public App(Main m)
        {
            my = this;
            main = m;
        }

        public wTimer.ITimerMgr srvTimer { get; protected set; } // 按照服务器时间运行的定时器
        public wTimer.ITimerMgr mainTimer { get; protected set; } // 逻辑主线程定时器
        public wThread.Coroutine mainCoroutine { get; protected set; } // 逻辑主线程协程

        public MsgDispatcher mainDispatcher { get; protected set; } // 逻辑主线程分配器
        public ThreadDispatcher threadDispatcher { get; private set; } 

#if COM_DEBUG
        public DebugNetDispatcher debugNetDispatcher { get; private set; } // 模拟网络延迟的逻辑处理
#endif

        public ModuleFactoryMgr moduleMgr { get; private set; }

        public ProtocolHandler handler { get; private set; }

        public EventSet eventSet { get; private set; }

        public SceneMgr sceneMgr { get; private set; }

        //public LevelLogicMgr levelLogicMgr { get; private set; }

        public BattleProtocol battleProtocol { get; private set; }

        public LocalPlayer localPlayer { get; private set; }

        public AppStateMgr appStateMgr { get; private set; }

        public UI.UISystem uiSystem { get; private set; }

        public XTools.TimerMgrObj frameMgr { get; private set; }

        // 调用热更新中的接口，所有C#层调用热更DLL中的接口都需要通过此类来进行
        public hotAppAgent hotAgent { get; private set; }

        public GameRPC gameRPC { get { return socket.game; } }
        public WorldRPC worldRPC { get { return socket.world; } }

        public static MessageStream GetStream(ByteString bytes)
        {
            MessageStream ms = new MessageStream(bytes.buffer);
            ms.WritePos = bytes.Length;
            return ms;
        }

        public Main main { get; private set; }
        public GateSocket socket { get; private set; }
        public GameRPC game { get { return socket.game; } }
        public WorldRPC world { get { return socket.world; } }
        public IRemoteCall remote { get; private set; } // 响应服务器RPC的请求
        public CameraManager cameraMgr { get; private set; }
        public battle.InputManager input { get; private set; }

        public NetEvent netEvent { get; private set; }
#if USE_HOT
        public AppDomain appdomain { get; private set; }
#endif
        public IEnumerator Init()
        {
            global::XYJLogger.CreateInstance();

            // 加载热更模块
            InitHotModule();

            mainCoroutine = new wThread.Coroutine();
            srvTimer = new wTimer.TimerMgr(new ServerGetTime());
            mainTimer = new wTimer.TimerMgr();

            mainDispatcher = new MsgDispatcher();
            threadDispatcher = new ThreadDispatcher(1);

#if COM_DEBUG
            debugNetDispatcher = new DebugNetDispatcher();
#endif
            handler = new ProtocolHandler();

            eventSet = new EventSet();
            sceneMgr = new SceneMgr();
            //levelLogicMgr = new LevelLogicMgr();
            battleProtocol = new BattleProtocol();
            remote = new MTRemoteCall();
            socket = new GateSocket();

            appStateMgr = new AppStateMgr();
            localPlayer = new LocalPlayer();

            hotAgent = new hotAppAgent();

            Log.Set(new CommonLog());
            CsvCommon.Log.Set(new CsvLog());
            CsvLoadAdapter.All();

            InitModule();
            localPlayer.Start(); // 在这里，初始化下本地玩家的模块数据

            cameraMgr = GameObject.FindObjectOfType<CameraManager>();
            input = new battle.InputManager();

            YieldFactory.SetFactory(new RPC.YieldFactory());

            netEvent = new NetEvent();

            XTools.TimerMgrObj.CreateInstance();
            frameMgr = XTools.TimerMgrObj.Instance;

            PackTool.MagicThread.CreateInstance();
            PackTool.ResourcesGroup.Init();

            TaskDialogMgr.CreateInstance();

            //面板初始化
            bool isLoad = false;
            GameObject uiRoot = null;
            RALoad.LoadPrefab("UIRoot", (GameObject go, object p) => { uiRoot = go; isLoad = true; }, null, true, true);
            while (!isLoad)
                yield return 0;
            uiRoot.name = "UIRoot";
            uiRoot.SetActive(true);
            Object.DontDestroyOnLoad(uiRoot);
            uiSystem = uiRoot.GetComponentInChildren<UI.UISystem>();

            uiSystem.systemHintMgr.Init();
            uiSystem.obtainItemShowMgr.Init();
            uiSystem.pureShowTipsMgr.Init();

            yield return uiSystem.dialogMgr.Init();

            xys.gm.GM_UI.Create();

            //游戏初始化完成
            eventSet.fireEvent(EventID.FinishAppInit);

            appStateMgr.Enter(AppStateType.BeginLogin, null);

            ServerGetTime.Init();
            SoundMgr.Init();

            behaviac.BehaviacWorkspace.Behavic_Init("Data/Config/behaviorXml");
        }

        private void InitHotModule()
        {
#if USE_HOT
            appdomain = new AppDomain();
            appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
            try
            {
                using(var fs = PackTool.TextLoad.GetStream("Data/DyncDll.dll"))
                {
#if USE_PDB
                    using (var p = PackTool.TextLoad.GetStream("Data/DyncDll.pdb"))
                    {
                        appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
                    }
#else
                    appdomain.LoadAssembly(fs);
#endif
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() => { ((System.Action)act)(); });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.String>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.String>((arg0) =>
                {
                    ((System.Action<System.String>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<xys.UI.UCenterOnChild.OnCenterHandler>((act) =>
            {
                return new xys.UI.UCenterOnChild.OnCenterHandler((centerChild) =>
                {
                    ((System.Action<UnityEngine.GameObject>)act)(centerChild);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
                {
                    ((System.Action<System.Single>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<PackTool.ResourcesEnd<GameObject>>((act) => { return new PackTool.ResourcesEnd<GameObject>((obj, p) => { ((System.Action<GameObject, object>)act)(obj, p); }); });
            appdomain.DelegateManager.RegisterDelegateConvertor<PackTool.ResourcesEnd<Texture2D>>((act) => { return new PackTool.ResourcesEnd<Texture2D>((obj, p) => { ((System.Action<Texture2D, object>)act)(obj, p); }); });
            appdomain.DelegateManager.RegisterDelegateConvertor<PackTool.ResourcesEnd<Material>>((act) => { return new PackTool.ResourcesEnd<Material>((obj, p) => { ((System.Action<Material, object>)act)(obj, p); }); });
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.CharacterData>();            appdomain.DelegateManager.RegisterMethodDelegate<global::Network.IPacket, NetProto.Int64>();            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.MailRecords>();            appdomain.DelegateManager.RegisterMethodDelegate<IPacket, wProtobuf.RealBytes>();            appdomain.DelegateManager.RegisterMethodDelegate<IPacket, AllPackageChange>();            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, System.Int32, System.Object>();            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.SkillSchemeName>();            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.SkillComprehend>();
            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, wProtobuf.RealBytes>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::Network.IPacket, NetProto.PackageList>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.PointerEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<string>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::Network.IPacket, ItemGrids>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.TeamAllTeamInfo>();
            appdomain.DelegateManager.RegisterMethodDelegate<object[]>();
            appdomain.DelegateManager.RegisterMethodDelegate<int>();
            appdomain.DelegateManager.RegisterMethodDelegate<xys.UI.State.StateRoot, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<xys.Grid>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.TeamSundryData>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::Network.IPacket, NetProto.AllPackageChange>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::Network.IPacket, NetProto.PetsAttribute>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::Network.IPacket, NetProto.TitleDataChange>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.ExchangeItemReq>();
            appdomain.DelegateManager.RegisterMethodDelegate<global::Network.IPacket>();
            appdomain.DelegateManager.RegisterMethodDelegate<List<NetProto.CharacterData>>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int64>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<Dictionary<int, NetProto.TeamInviteJoinInfo>>();
            appdomain.DelegateManager.RegisterMethodDelegate<xys.IObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject, System.Object>();
            appdomain.DelegateManager.RegisterMethodDelegate<CommonBase.IAttribute<NetProto.AttType>>();
            appdomain.DelegateManager.RegisterMethodDelegate<xys.UI.PanelType>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.QueryNearbyUserRsp>();
            appdomain.DelegateManager.RegisterMethodDelegate<xys.AttributeChange>();            appdomain.DelegateManager.RegisterFunctionDelegate<NetProto.TeamMemberData, NetProto.TeamMemberData, System.Int32>();            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.RankQueryRankResult>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.WelfarePageType>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.WelfareResult>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.ReceiveOLRwdResponse>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.WelfareDB>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendsApplyInfos>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendSearchInfo>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendError>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendItemInfo>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendDbData>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendDeleteMsg>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendSearchInfo>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendContactInfo>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendRecentlyInfos>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendsApplyInfos>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendTips>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendErrorRet>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.FriendDeleteReturnMsg>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.DemonplotRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.DemonplotSkillRequest>();
            //appdomain.DelegateManager.RegisterMethodDelegate<NetProto.DemonplotCurrencyRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Single>();
            appdomain.DelegateManager.RegisterMethodDelegate<double>();
            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.ExchangeItemRep>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.RefineryEquipMsg>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();            appdomain.DelegateManager.RegisterMethodDelegate<xys.CreateCharVo>();            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.SkillSchemeData>();
            //appdomain.DelegateManager.RegisterMethodDelegate<IMessageAdapter>();
            //appdomain.DelegateManager.RegisterMethodDelegate<IMessageAdapter.Adaptor>();
            appdomain.DelegateManager.RegisterMethodDelegate<IPacket, MailFetchAttachmentResult>();
            appdomain.DelegateManager.RegisterMethodDelegate<RealBytes>();            appdomain.DelegateManager.RegisterFunctionDelegate<IEnumerator>();            appdomain.DelegateManager.RegisterFunctionDelegate<RealBytes, OutValue<RealBytes>, IEnumerator>();
            appdomain.DelegateManager.RegisterFunctionDelegate<RealBytes, IEnumerator>();            appdomain.DelegateManager.RegisterFunctionDelegate<UIWidgets.AccordionItem, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<RealBytes, RealBytes>();            appdomain.DelegateManager.RegisterFunctionDelegate<List<CharacterData>, bool>();
            appdomain.DelegateManager.RegisterFunctionDelegate<bool>();
            appdomain.DelegateManager.RegisterFunctionDelegate<xys.UI.State.StateRoot, System.Int32, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.GameObject, UnityEngine.EventSystems.BaseEventData, System.Boolean>();
            #region 福利相关
            //appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.WelfareResult>();
            //appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.ReceiveOLRwdResponse>();
            #endregion
            #region 宠物模块相关
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.RefineryPetRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.LearnSkillPetRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.LockSkillPetRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.PetItemRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.SetPetPotentialPointRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.SetPetPlayRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.SetPetPotentialSliderRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.PetsNickNameRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.PetsAIRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<NetProto.PetQualificationRequest>();
            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.PetAttributeRespone>();
            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.PetsRespone>();
            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.WashPetRespone>();
            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.RefineryPetRespone>();
            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.PetsNickNameRespone>();
            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.PetAIRespone>();
            appdomain.DelegateManager.RegisterMethodDelegate<wProtobuf.RPC.Error, NetProto.PetItemRespone>();

            appdomain.DelegateManager.RegisterFunctionDelegate<Config.AttributeDefine, Config.AttributeDefine, System.Int32>();
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<Config.AttributeDefine>>((act) =>
            {
                return new System.Comparison<Config.AttributeDefine>((x, y) =>
                {
                    return ((System.Func<Config.AttributeDefine, Config.AttributeDefine, int>)act)(x, y);
                });
            });
            #endregion

            appdomain.DelegateManager.RegisterDelegateConvertor<System.Action<NetProto.TitleDataChange>>((act) =>
            {
                return new System.Action<NetProto.TitleDataChange>((obj) =>
                {
                    ((System.Action<NetProto.TitleDataChange>)act)(obj);
                });
            });            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<TeamMemberData>>((act) =>
            {
                return new System.Comparison<TeamMemberData>((x, y) =>
                {
                    return ((System.Func<TeamMemberData, TeamMemberData, int>)act)(x, y);
                });
            });#else

#endif
        }

        private void InitModule()
        {
            moduleMgr = new ModuleFactoryMgr();

            moduleMgr.Reg<AttributeModule>(ModuleType.MT_Attribute);
            moduleMgr.Reg<ActivityModule>(ModuleType.MT_Activity);
            moduleMgr.Reg<PackageModule>(ModuleType.MT_Package);
            moduleMgr.Reg<FriendModule>(ModuleType.MT_Friend);
            moduleMgr.Reg<PetsModule>(ModuleType.MT_Pets);
            moduleMgr.Reg<WelfareModule>(ModuleType.MT_Welfare);
            moduleMgr.Reg<EquipModule>(ModuleType.MT_Equip);
            moduleMgr.Reg<LevelModule>(ModuleType.MT_Level);
            moduleMgr.Reg<CDModule>(ModuleType.MT_CDTimer);
            moduleMgr.Reg<TitleModule>(ModuleType.MT_Title);
            moduleMgr.Reg<ExStoreModule>(ModuleType.MT_ExchangeStore);
            moduleMgr.Reg<DemonplotsModule>(ModuleType.MT_Demonplot);
            moduleMgr.Reg<MailModule>(ModuleType.MT_Mail);
            moduleMgr.Reg<AppearanceModule>(ModuleType.MT_Appearance);
            moduleMgr.Reg<ChatModule>(ModuleType.MT_Chat);
            moduleMgr.Reg<TrumpsModule>(ModuleType.MT_Trump);
            moduleMgr.Reg<TeamModule>(ModuleType.MT_Team);
            moduleMgr.Reg<ClanModule>(ModuleType.MT_Clan);
            moduleMgr.Reg<SkillModule>(ModuleType.MT_Skill);
            moduleMgr.Reg<TradeStoreModule>(ModuleType.MT_TradeStore);
            moduleMgr.Reg<GameTaskModule>(ModuleType.MT_GameTask);
            moduleMgr.Reg<RankModule>(ModuleType.MT_Rank);
            moduleMgr.Reg<MoneyTreeModule>(ModuleType.MT_MoneyTree);

            // 初始化热更模块
            hotAgent.InitModule(moduleMgr);
        }

        public void Update()
        {
            mainCoroutine.Update();
            mainDispatcher.Update();

#if COM_DEBUG
            debugNetDispatcher.Update();
#endif
            srvTimer.CheckTimer();
            mainTimer.CheckTimer();
            input.Update();
            sceneMgr.Update();
            // Assets.Scripts.Common.ServerTime.ServerGetTime.ReqServerTime() ;

#if UNITY_EDITOR
#else
            frameMgr.Update();
#endif
            behaviac.BehaviacWorkspace.Behavic_Update();
        }

        public void LateUpdate()
        {
#if UNITY_EDITOR
#else
            frameMgr.LateUpdate();
#endif
        }

        public void OnApplicationQuit()
        {
            threadDispatcher.Release();
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }

            behaviac.BehaviacWorkspace.Behavic_Cleanup();

            my = null;
        }

        //重回登录界面,一些全局数据需要复位
        public void BackToLogin()
        {
            eventSet.fireEvent(EventID.BackToLogin);
            App.my.uiSystem.HideAllPanel();
            App.my.socket.Close();
            App.my.appStateMgr.Enter(AppStateType.BeginLogin, null);

        }
    }
}