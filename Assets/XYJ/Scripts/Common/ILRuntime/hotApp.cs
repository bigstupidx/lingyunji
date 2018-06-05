#if !USE_HOT
namespace xys.hot
{
    using Network;
    using System.Collections.Generic;

    class hotApp
    {
        public static hotApp my { get; private set; }

        // 协议注册
        public ProtocolHandler handler { get; private set; }
        public HotLocalProcedure gateRPC { get; private set; }
        public HotLocalProcedure gameRPC { get; private set; }
        public HotLocalProcedure worldRPC { get; private set; }
        public RPC.IRemoteCall remote { get; private set; }
        public TimerMgr mainTimer { get; private set; }
        public TimerMgr srvTimer { get; private set; }

        public LocalPlayer localPlayer { get { return App.my.localPlayer; } }

        // 当初初始化的模块列表
        class ModuleMgr
        {
            public ModuleMgr()
            {
                idsList = new Dictionary<int, HotModuleBase>();
                typeList = new Dictionary<string, HotModuleBase>();
            }

            Dictionary<int, HotModuleBase> idsList { get; set; }
            Dictionary<string, HotModuleBase> typeList { get; set; }

            public void OnCreate(HotModuleBase module)
            {
                idsList.Add(module.parent.id, module);
                typeList.Add(module.GetType().FullName, module);
            }

            public T Get<T>() where T : HotModuleBase
            {
                string fullName = typeof(T).FullName;
                HotModuleBase v = null;
                if (typeList.TryGetValue(fullName, out v))
                    return (T)v;

                return null;
            }

            public HotModuleBase Get(int id)
            {
                HotModuleBase v = null;
                if (idsList.TryGetValue(id, out v))
                    return v;

                return null;
            }
        }

        class PanelMgr
        {
            public PanelMgr()
            {
                panelTypeList = new Dictionary<string, UI.HotPanelBase>();
                typeList = new Dictionary<string, UI.HotPanelBase>();
            }

            // 所有的面板基类
            Dictionary<string, UI.HotPanelBase> panelTypeList { get; set; }
            Dictionary<string, UI.HotPanelBase> typeList { get; set; }

            public void OnPanelCreate(UI.HotPanelBase panel)
            {
                panelTypeList.Add(panel.parent.panelType, panel);
                typeList.Add(panel.GetType().FullName, panel);

                Debuger.DebugLog("Panel Create:{0}", panel);
            }

            public void OnPanelDestroy(UI.HotPanelBase panel)
            {
                Debuger.DebugLog("Panel Destroy:{0}", panel.parent.panelType);
                if (!panelTypeList.Remove(panel.parent.panelType))
                {
                    Debuger.ErrorLog("Panel Destroy:{0} not find!", panel.parent.panelType);
                }

                if (!typeList.Remove(panel.GetType().FullName))
                {
                    Debuger.ErrorLog("Panel Destroy:{0} not find!", panel.GetType().FullName);
                }
            }

            UI.HotPanelBase GetByPanelType(string panelType)
            {
                UI.HotPanelBase panel = null;
                if (panelTypeList.TryGetValue(panelType, out panel))
                    return panel;
                return null;
            }

            public T Get<T>() where T : UI.HotPanelBase
            {
                UI.HotPanelBase panel = null;
                if (typeList.TryGetValue(typeof(T).Name, out panel))
                    return (T)panel;
                return null;
            }
        }

        PanelMgr panelMgr { get; set; }
        ModuleMgr moduleMgr { get; set; }

        // 全局事件
        public Event.HotEventSet eventSet { get; private set; }

        // 本地玩家的事件
        public Event.HotObjectEventSet localEvent { get; private set; }

        GateSocket socket { get; set; }

        public hotApp()
        {
            my = this;

            socket = App.my.socket;
            handler = new ProtocolHandler(App.my.handler);
            gateRPC = new HotLocalProcedure(socket.local);
            gameRPC = new HotLocalProcedure(App.my.game.local);
            worldRPC = new HotLocalProcedure(App.my.world.local);
            remote = new RPC.Remote(App.my.remote);

            eventSet = new Event.HotEventSet();
            localEvent = new Event.HotObjectEventSet(App.my.localPlayer.eventSet);

            moduleMgr = new ModuleMgr();
            panelMgr = new PanelMgr();

            mainTimer = new TimerMgr(App.my.mainTimer);
            srvTimer = new TimerMgr(App.my.srvTimer);
            VoiceMisc.Init();
        }

        public void OnPanelCreate(UI.HotPanelBase panel)
        {
            panelMgr.OnPanelCreate(panel);
        }

        public void OnPanelDestroy(UI.HotPanelBase panel)
        {
            panelMgr.OnPanelDestroy(panel);
        }

        public T GetPanel<T>() where T : UI.HotPanelBase
        {
            return panelMgr.Get<T>();
        }

        public void OnModuleCreate(HotModuleBase mb)
        {
            moduleMgr.OnCreate(mb);
        }

        public HotModuleBase GetModule(NetProto.ModuleType mt)
        {
            return GetModule((int)mt);
        }

        public T GetModule<T>() where T : HotModuleBase
        {
            return moduleMgr.Get<T>();
        }

        public HotModuleBase GetModule(int id)
        {
            return moduleMgr.Get(id);
        }

        // 初始化所有热更模块
        void InitModule(ModuleFactoryMgr mgr)
        {

        }

        void CsvLoad(CsvCommon.CsvLoadKey csv)
        {

        }

        public void SendGame(NetProto.Protoid id)
        {
            SendGame((int)id);
        }

        public void SendGame(int id)
        {
            socket.SendGame(id);
        }

        public void SendGame<T>(NetProto.Protoid id, T msg) where T : RPC.IMessage, new()
        {
            SendGame((int)id, msg);
        }

        public void SendGame<T>(int id, T msg) where T : RPC.IMessage, new()
        {
            wProtobuf.RealBytes bytes = new wProtobuf.RealBytes() { bytes = RPCHelp.WriteTo(msg) };
            socket.SendGame(id, bytes);
        }

        public void hSendGame<T>(NetProto.Protoid id, T msg) where T : RPC.IMessage, new()
        {
            hSendGame((int)id, msg);
        }

        public void hSendGame<T>(int id, T msg) where T : RPC.IMessage, new()
        {
            wProtobuf.RealBytes bytes = new wProtobuf.RealBytes() { bytes = RPCHelp.WriteTo(msg) };
            socket.SendGame(id, bytes);
        }

        public void SendWorld(NetProto.Protoid id)
        {
            SendWorld((int)id);
        }

        public void SendWorld(int id)
        {
            socket.SendWorld(id);
        }

        public void SendWorld<T>(NetProto.Protoid id, T msg) where T : RPC.IMessage, new()
        {
            SendWorld((int)id, msg);
        }

        public void SendWorld<T>(int id, T msg) where T : RPC.IMessage, new()
        {
            wProtobuf.RealBytes bytes = new wProtobuf.RealBytes() { bytes = RPCHelp.WriteTo(msg) };
            socket.SendWorld(id, bytes);
        }

        public void hSendWorld<T>(NetProto.Protoid id, T msg) where T : RPC.IMessage, new()
        {
            hSendWorld((int)id, msg);
        }

        public void hSendWorld<T>(int id, T msg) where T : RPC.IMessage, new()
        {
            wProtobuf.RealBytes bytes = new wProtobuf.RealBytes() { bytes = RPCHelp.WriteTo(msg) };
            socket.SendWorld(id, bytes);
        }
    }
}
#endif