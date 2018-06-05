#if !USE_HOT
namespace xys.hot
{
    using Config;
    using NetProto;
    using NetProto.Hot;
    using System;
    using xys.hot.UI;
    using xys.UI;
    using wProtobuf;

    class HotWelfareModule : HotModuleBase
    {
        WelfareModule module;
        WelfareMgr welfareMgr = new WelfareMgr();
        C2AWelfareModuleRequest m_WelfareModuleRequest;

        public static DateTime countDownDate;
        public static IntBit rwdReadyStatus;  //奖励状态，每页有奖励+1，每页无奖励-1
        public static IntBit rwdHideStatus;
        public HotWelfareModule(WelfareModule m) : base(m)
        {

        }

        protected override void OnAwake()
        {
            Init();
        }

        // 序列化
        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            welfareMgr.m_WelfareDB.MergeFrom(output);
            //当前倒数日期为登陆日期
            HotWelfareModule.countDownDate = new DateTime(welfareMgr.m_WelfareDB.playerLoginTime);
            rwdReadyStatus = new IntBit(welfareMgr.m_WelfareDB.rwdReadyStatus);
            rwdHideStatus = new IntBit(welfareMgr.m_WelfareDB.rwdHideStatus);
        }

        void Init()
        {
            this.m_WelfareModuleRequest = new C2AWelfareModuleRequest(hotApp.my.gameRPC);

            Event.Subscribe(EventID.Welfare_OnSign, GetSignRwdBySign);
            Event.Subscribe(EventID.Welfare_OnSubsign, GetSignRwdBySubsign);
            Event.Subscribe<int>(EventID.Welfare_GetOnlineRwd, ReceiveOLRwd);
            Event.Subscribe<int>(EventID.Welfare_GetLevelRwd, ReceiveLVRwd);
            Event.Subscribe<int>(EventID.Welfare_GetSevendayRwd, ReceiveDayOLRwd);
            Event.Subscribe<NetProto.Int32>(EventID.Welfare_Test, ChangeDataByGM);

            //等级和奖励由于是跨模块的数据 需要在模块同时判断
            Event.Subscribe(NetProto.AttType.AT_Level, (args) =>
            {
                if (WelfareMgr.CheckLVRwdAvailable())
                {
                    XYJLogger.LogDebug("Send LV ready:" + PanelType.UIWelfarePanel);
                    rwdReadyStatus.Set((int)WelfarePageType.TYPE_LV, true);
                    //Event.FireEvent<int>(EventID.MainPanel_UIItemReady, (int)PanelType.UIWelfarePanel);
                    MainPanel.SetItemReadyActive((int)PanelType.UIWelfarePanel,true);
                    Event.fireEvent(EventID.Welfare_RefreshUI);
                }
            });
            hotApp.my.handler.Reg<NetProto.Int32>(Protoid.A2C_OLRwdReady, OnOLRwdReady);
            hotApp.my.handler.RegHot<WelfareDB>(Protoid.A2C_ResetDay, OnResetDay);
            hotApp.my.handler.RegHot<WelfareDB>(Protoid.A2C_ResetMonth, OnResetMonth);
            MainPanelItemListener listener = new MainPanelItemListener(this.ItemShowConditionFunc, this.RedDotShowConditionFunc);
            MainPanel.SetItemListener((int)PanelType.UIWelfarePanel, listener);
        }


        void GetSignRwdBySign()
        {
            this.m_WelfareModuleRequest.GetRwdBySign((error, respone) =>

            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Welfare_Signed_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Welfare_Date_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    XYJLogger.LogDebug("Get signed:" + welfareMgr.m_WelfareDB.signDay);
                    welfareMgr.m_WelfareDB.isTodaySigned = true;
                    welfareMgr.m_WelfareDB.signDay++;
                    Event.fireEvent(EventID.Welfare_DaySignRwdReceived);
                    Event.fireEvent(EventID.Welfare_RefreshPageInfo);
                }
            }
            );
        }
        void GetSignRwdBySubsign()
        {
            this.m_WelfareModuleRequest.GetRwdBySubsign((error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Welfare_SubSignTimes_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Welfare_Signed_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Welfare_Date_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    XYJLogger.LogDebug("Get Subsigned:" + welfareMgr.m_WelfareDB.signDay);
                    welfareMgr.m_WelfareDB.signDay++;
                    Event.FireEvent<int>(EventID.Welfare_DaySignRwdReceived, welfareMgr.m_WelfareDB.signDay - 1);
                    Event.fireEvent(EventID.Welfare_RefreshPageInfo);
                }
            }
            );
        }
        void ReceiveOLRwd(int rwdID)
        {
            NetProto.Int32 request = new NetProto.Int32();
            request.value = rwdID;
            this.m_WelfareModuleRequest.ReceiveOLRwd(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Welfare_RwdReceived_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Welfare_RwdID_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    XYJLogger.LogDebug("Get OlRwd:" + welfareMgr.m_WelfareDB.onlineRwdID);
                    welfareMgr.m_WelfareDB.onlineRwdID++;
                    welfareMgr.m_WelfareDB.onlineAwardTime = 0;  //reset onlineTime
                     //重置当前倒数日期
                    HotWelfareModule.countDownDate = DateTime.Now;
                    Event.fireEvent(EventID.Welfare_OLRwdReceived);
                }
            }
            );
        }

        void ReceiveLVRwd(int rwdID)
        {
            NetProto.Int32 request = new NetProto.Int32();
            request.value = rwdID;
            this.m_WelfareModuleRequest.ReceiveLVRwd(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Welfare_RwdReceived_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Welfare_Date_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    XYJLogger.LogDebug("Get LvRwd:" + rwdID);
                    Event.FireEvent<int>(EventID.Welfare_LVRwdReceived, rwdID - 1);
                }
            }
            );
        }

        void ReceiveDayOLRwd(int rwdID)
        {
            NetProto.Int32 request = new NetProto.Int32();
            request.value = rwdID;
            this.m_WelfareModuleRequest.ReceiveDayOLRwd(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Welfare_RwdReceived_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Welfare_Date_Error)
                {
                    return;
                }
                if (respone.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    XYJLogger.LogDebug("Get DayOLRwd:" + rwdID);
                    Event.FireEvent<int>(EventID.Welfare_DayOLRwdReceived, rwdID - 1);
                }
            }
            );
        }

        void ChangeDataByGM(NetProto.Int32 msg)
        {
            this.m_WelfareModuleRequest.ReceiveGmCmd(msg, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Welfare_Date_Error)
                {
                    return;
                }

                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    Log.Debug("Change success");

                    welfareMgr.m_WelfareDB.loginDays = msg.value;
                }
                if (respone.code == ReturnCode.Welfare_RwdReceived_Error)
                {
                    return;
                }
            }
            );
        }
        void OnOLRwdReady(IMessage obj)
        {
            XYJLogger.LogDebug("ServerOLRwdCountDownEnd");
            rwdReadyStatus.Set((int)WelfarePageType.TYPE_OL, true);
            Event.FireEvent<int>(EventID.Welfare_PageRewardReady, (int)WelfarePageType.TYPE_OL);
            MainPanel.SetItemReadyActive((int)PanelType.UIWelfarePanel, true);
        }

        void OnResetDay(WelfareDB data)
        {
            welfareMgr.SetData(data);
            rwdReadyStatus = new IntBit(welfareMgr.m_WelfareDB.rwdReadyStatus);
            rwdHideStatus = new IntBit(welfareMgr.m_WelfareDB.rwdHideStatus);
            MainPanel.SetItemReadyActive((int)PanelType.UIWelfarePanel, true);
            Event.fireEvent(EventID.Welfare_RefreshUI);
        }
        void OnResetMonth(WelfareDB data)
        {
            welfareMgr.SetData(data);
            rwdReadyStatus = new IntBit(welfareMgr.m_WelfareDB.rwdReadyStatus);
            rwdHideStatus = new IntBit(welfareMgr.m_WelfareDB.rwdHideStatus);
            MainPanel.SetItemReadyActive((int)PanelType.UIWelfarePanel, true);
            Event.fireEvent(EventID.Welfare_RefreshUI);
        }
        public bool ItemShowConditionFunc()
        {
            return true;
        }
        public bool RedDotShowConditionFunc()
        {
            if (rwdReadyStatus.value != 0)
                return true;
            else
                return false;
        }
    }
}
#endif