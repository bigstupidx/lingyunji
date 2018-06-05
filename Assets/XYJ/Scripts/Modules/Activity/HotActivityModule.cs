#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


namespace xys.hot
{
    using NetProto;
    using NetProto.Hot;
    using UI;
    using wProtobuf;
    using xys.UI;
    using System.Collections.Generic;

    class HotActivityModule : HotModuleBase
    {
        public ActivityMgr activityMgr = new ActivityMgr();

        public ActivityModuleRequest m_request { get; private set; }

        private const float DelayTime = 3.5f;
        private const float DelayOpenTime = 35f;
        private const int ActiveMul = 6;     // 活力值=活跃度*6
        private const int LevelMul = 25;     // 活力的上限为 玩家等级*25

        private Queue<ActivityData> m_UnlockData; // 未解锁活动数据

        private Queue<ActivityData> m_OpenData; // 活动开启，前两分钟提示

        private int openNum = 0;

        public HotActivityModule(ActivityModule module) : base(module)
        {
        }

        protected override void OnAwake()
        {
            Init();
            RegistMsg();

            m_UnlockData = new Queue<ActivityData>();
            m_OpenData = new Queue<ActivityData>();
        }

        // 推送协议注册
        private void RegistMsg()
        {
            hotApp.my.handler.RegHot<ActivityData>(Protoid.A2C_ActivityDataChange, OnRefreshData);
            hotApp.my.handler.RegHot<LevelTmp>(Protoid.A2C_NewActivityPush, OnShowOpenActivityPanel);
            hotApp.my.handler.RegHot<ActivityData>(Protoid.A2C_ActivityStartPush, OnShowStartActivityPanel);
            hotApp.my.handler.RegHot<ActivityData>(Protoid.A2C_ActivityStartingPush, OnShowStartingActivityPanel);
        }

        protected override void OnDeserialize(IReadStream output)
        {
            this.activityMgr.m_ActivityDbData.MergeFrom(output);
        }

        private void Init()
        {
            this.m_request = new ActivityModuleRequest(hotApp.my.gameRPC);

            Event.Subscribe<ActivityData>(EventID.Activity_OpenPush, RequestSetPush);
            Event.Subscribe<int>(xys.EventID.Activity_RequestGetReward, RequestGetActiveReward);
            Event.Subscribe<int>(xys.EventID.Activity_SetFinishState, RequestSetFinishState);
            MainPanel.SetItemListener((int)PanelType.UIActivityPanel, new MainPanelItemListener(this.ItemShowConditionFunc));
        }

        public bool ItemShowConditionFunc()
        {
            return true;
        }

        private void RequestSetPush(ActivityData data)
        {
            // 向后端发送开启协议
            ActivityRemindRequest request = new ActivityRemindRequest();
            request.activityId = data.activityId;
            request.activityIsPush = data.activityOpen;
            request.activityType = 0; // 0是线上，1是线下

            this.m_request.SetPushRemind(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (activityMgr.m_ActivityDbData.listActivies.ContainsKey(data.activityId))
                    activityMgr.m_ActivityDbData.listActivies[data.activityId].activityOpen = data.activityOpen;

                Event.FireEvent(EventID.Activity_UpdatePushUI, activityMgr.m_ActivityDbData.listActivies); // 刷新ui
            });
        }

        private void RequestGetActiveReward(int activityId)
        {
            NetProto.Int32 requestData = new NetProto.Int32();
            requestData.value = activityId;

            this.m_request.GetActiveReward(requestData, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Activity_Cannot_Get_Reward)
                {
                    xys.UI.Utility.TipContentUtil.Show("activity_get_error_tip");
                    return;
                }
                else if (respone.code == ReturnCode.Backage_Full_Error)
                {
                    xys.UI.Utility.TipContentUtil.Show("activity_bag_full_tip");
                    return;
                }

                if (activityMgr.m_ActivityDbData.listRewards.ContainsKey(activityId))
                    activityMgr.m_ActivityDbData.listRewards[activityId].rewardStatus = 2;

                Event.fireEvent(EventID.Activity_UpdateActiveUI); // 刷新ui
            });
        }

        private void RequestSetUnlockState(ActivityData data)
        {
            NetProto.Int32 requestData = new NetProto.Int32();
            requestData.value = data.activityId; // 传活动id

            this.m_request.SetActivityUnlockState(requestData, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Activity_Id_Error)
                {
                    xys.UI.Utility.TipContentUtil.Show("活动不存在！");
                    return;
                }

                if (activityMgr.m_ActivityDbData.listActivies.ContainsKey(data.activityId))
                    activityMgr.m_ActivityDbData.listActivies[data.activityId].activityIsShow = 1;
            });
        }

        private void RequestSetFinishState(int activityId)
        {
            NetProto.Int32 requestData = new NetProto.Int32();
            requestData.value = activityId; // 传活动id

            this.m_request.SetActivityFinishState(requestData, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.code == ReturnCode.Activity_Id_Error)
                {
                    xys.UI.Utility.TipContentUtil.Show("活动不存在！");
                    return;
                }

                if (activityMgr.m_ActivityDbData.listActivies.ContainsKey(activityId))
                    activityMgr.m_ActivityDbData.listActivies[activityId].activityFirstFinish = 1;
            });
        }

        // 新活动逐个推送
        private void OnShowOpenActivityPanel(LevelTmp levelData)
        {
            m_UnlockData.Clear();

            foreach (ActivityData data in activityMgr.m_ActivityDbData.listActivies.Values)
            {
                Config.ActivityDefine activityConf = Config.ActivityDefine.Get(data.activityId);
                if (data.activityIsShow == 0 && activityConf.requireLv <= levelData.level)
                {
                    m_UnlockData.Enqueue(data);
                }
            }
            ShowUnlockPanel();
        }

        private void ShowUnlockPanel()
        {
            ActivityData[] acitiviesData = m_UnlockData.ToArray();
            if (acitiviesData.Length <= 0) return;

            ActivityData currentData = m_UnlockData.Dequeue();
            App.my.uiSystem.ShowPanel("UIActivityUnlockedPanel", currentData, true);
            RequestSetUnlockState(currentData);

            ActivityData[] acitivies = m_UnlockData.ToArray();
            if (acitivies.Length <= 0) return;

            hotApp.my.mainTimer.Register(DelayTime, 1, ShowUnlockPanel);
        }

        // 活动开启前两分钟推送
        private void OnShowStartActivityPanel(ActivityData data)
        {
            m_OpenData.Enqueue(data);

            if (openNum == 0) // 隔几秒后再调用
                hotApp.my.mainTimer.Register(2, 1, ShowOpenActivityPanel);
            else
                hotApp.my.mainTimer.Register(5, 1, ShowOpenActivityPanel);

            openNum++;
        }

        // 活动真正开启推送
        private void OnShowStartingActivityPanel(ActivityData data)
        {
            if (activityMgr.m_ActivityDbData.listActivies.ContainsKey(data.activityId))
            {
                Event.fireEvent(EventID.Activity_UpdateListData); // 刷新ui
            }
        }

        private void ShowOpenActivityPanel()
        {
            var panel = UISystem.Get("UIActivityOpenPanel");
            if (panel != null && panel.state == UIPanelBase.State.Show)
                return;

            ActivityData currentData = m_OpenData.Dequeue();
            App.my.uiSystem.ShowPanel("UIActivityOpenPanel", currentData, true);

            ActivityData[] acitivies = m_OpenData.ToArray();
            if (acitivies.Length <= 0)
            {
                openNum = 0;
                return;
            }

            hotApp.my.mainTimer.Register(DelayOpenTime, 1, ShowOpenActivityPanel);
        }

        //某个活动次数发生变化
        private void OnRefreshData(ActivityData data)
        {
            if (activityMgr.m_ActivityDbData.listActivies.ContainsKey(data.activityId))
            {
                int oldNum = activityMgr.m_ActivityDbData.listActivies[data.activityId].activityNum;
                activityMgr.m_ActivityDbData.listActivies[data.activityId] = data;
                Event.fireEvent(EventID.Activity_UpdateListData); // 刷新ui

                SetActiveData(data, oldNum);
            }
        }

        // 设置活跃度数据(当活动次数发生变化的时候更新，并向前端推送)
        private void SetActiveData(ActivityData activityData, int oldNum)
        {
            Config.ActivityDefine activityConf = Config.ActivityDefine.Get(activityData.activityId);
            if (activityConf.maxActiveness == 0)
                return;

            float activeNum = activityConf.activeness * activityData.activityNum;
            int addNum = activityData.activityNum - oldNum;
            float subActiveNum = 0; // 记录添加的活跃度差值

            if (activeNum < activityConf.maxActiveness) // 活动次数不超过最大次数才添加活跃度和活力
            {
                activityMgr.m_ActivityDbData.activeNum += activityConf.activeness * addNum;
                subActiveNum = activityConf.activeness * addNum;
            }
            else
            {
                float lastActive = oldNum * activityConf.activeness;
                if (lastActive < activityConf.maxActiveness)
                {
                    activityMgr.m_ActivityDbData.activeNum += activityConf.maxActiveness - lastActive;
                    subActiveNum = activityConf.maxActiveness - lastActive;
                }
            }

            float addFreshNum = subActiveNum * ActiveMul;
            int freshNum = hotApp.my.localPlayer.energyValue;
            if (freshNum < hotApp.my.localPlayer.levelValue * LevelMul) // 设置活力
            {
                if (freshNum + addFreshNum >= hotApp.my.localPlayer.levelValue * LevelMul)
                    Event.FireEvent(EventID.Activity_UpdateHuoliData, hotApp.my.localPlayer.levelValue * LevelMul);
                else
                    Event.FireEvent(EventID.Activity_UpdateHuoliData, addFreshNum + freshNum);
            }

            foreach (ActivityRewardData data in activityMgr.m_ActivityDbData.listRewards.Values)
            {
                if (data.rewardStatus == 0)
                {
                    Config.ActiveAward activeConf = Config.ActiveAward.Get(data.rewardId);
                    data.rewardStatus = ((activityMgr.m_ActivityDbData.activeNum >= activeConf.activenessRequire) ? 1 : 0);
                }
            }
            Event.fireEvent(EventID.Activity_UpdateActiveUI); // 刷新ui
        }
    }
}
#endif