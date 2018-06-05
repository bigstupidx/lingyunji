#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


using NetProto.Hot;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    enum ActivityState
    {
        enumActivityStateLvLimit = 0,    // 等级限制
        enumActivityStateGo = 1,         // 前往
        enumActivityStateAccept = 2,     // 已接
        enumActivityStateComplete = 3,   // 已完成且不显示参加按钮
        enumActivityStateNotStart = 4,   // 未开始
        enumActivityStateOver = 5,       // 活动时间结束，策划说今天时间过了也显示已完成
        enumActivityCompleteCanGo = 6,   // 已完成，仍要显示参加按钮
        enumActivityStateUnvisiable = 7, // 今天没有这个活动或者其他情况
    }

    enum TimeState
    {
        notToday,      // 今天没有
        notStart,      // 没到这个时间段
        runing,        // 活动时间段内
        over,          // 过了活动时间
    }

    enum SortState
    {
        enPriorityRuning,   // 可参加
        enPriorityNotStart, // 未开启
        enPriorityOver,     // 已结束、已完成
        enPriorityLvLimit,  // 未解锁
        enPriorityError,    // 优先级判断之外的状态
    }

    class UIActivityDailyPage : HotTablePageBase
    {
        public class UIActivityItem
        {
            #region 列表子对象

            public Transform m_Transform;

            private ActivityData activityData;
            private Config.ActivityDefine activityConf;

            public void SetData(ActivityItemData data)
            {
                activityConf = Config.ActivityDefine.Get(data.activityData.activityId);
                activityData = data.activityData;

                m_Transform.Find("Name").GetComponent<Text>().text = activityConf.name;
                m_Transform.Find("tabState").GetComponent<StateRoot>().CurrentState = activityConf.mark;

                if (activityConf.maxNum > 0)
                    m_Transform.Find("Number").GetComponent<Text>().text = data.activityData.activityNum + "/" + activityConf.maxNum;

                if (activityConf.activeness > 0)
                {
                    float maxActive = (data.activityData.activityNum * activityConf.activeness > activityConf.maxActiveness ? activityConf.maxActiveness : data.activityData.activityNum * activityConf.activeness);
                    m_Transform.Find("Active").GetComponent<Text>().text = maxActive + "/" + activityConf.maxActiveness;
                }
                else
                    m_Transform.Find("Active").GetComponent<Text>().text = "";
                m_Transform.Find("ActiveTitle").GetComponent<Text>().gameObject.SetActive(activityConf.activeness != 0);

                Helper.SetSprite(m_Transform.Find("Article/Icon").GetComponent<Image>(), activityConf.icon);

                Button showDetailBtn = m_Transform.Find("Bg").GetComponent<Button>();
                if (showDetailBtn != null)
                {
                    showDetailBtn.onClick.RemoveAllListeners();
                    showDetailBtn.onClick.AddListener(this.OnClickBtn);
                }

                m_Transform.Find("state").GetComponent<StateRoot>().CurrentState = (int)data.activityState;
                if (data.activityState == ActivityState.enumActivityStateLvLimit)
                {
                    m_Transform.Find("state/LvRequire/Number").GetComponent<Text>().text = string.Format("<color=#{0}>{1}级解锁</color>", Config.ColorConfig.Get(31).colorCode, activityConf.requireLv);
                }
                else if (data.activityState == ActivityState.enumActivityStateGo || data.activityState == ActivityState.enumActivityCompleteCanGo)
                {
                    Button goBtn = m_Transform.Find("state/btnJoin").GetComponent<Button>();
                    if (goBtn != null)
                    {
                        goBtn.onClick.RemoveAllListeners();
                        goBtn.onClick.AddListener(this.OnClickGoBtn);
                    }
                }
                else if (data.activityState == ActivityState.enumActivityStateNotStart)
                {
                    m_Transform.Find("state/Number").GetComponent<Text>().text = ActivityMgr.GetTimeStr(data.activityData, activityConf, true);
                }
            }

            private void OnClickBtn()
            {
                App.my.uiSystem.ShowPanel(PanelType.UIActivityIntroductionPanel, activityData);
            }

            // 前往
            private void OnClickGoBtn()
            {
                if (activityConf.isShowEnter == 1 && activityData.activityFirstFinish == 0 && activityData.activityNum >= activityConf.maxNum)
                {
                    string des = xys.UI.Utility.TipContentUtil.GenText("activity_remind_tip");

                    xys.UI.Dialog.TwoBtn.Show("", des, "取消", () => false, "确定", () =>
                    {
                        hotApp.my.eventSet.FireEvent(xys.EventID.Activity_SetFinishState, activityData.activityId);
                        if (CanGoing())
                            App.my.uiSystem.HidePanel("UIActivityPanel");
                        return false;
                    }, true, true);
                }
                else
                {
                    if (CanGoing())
                        App.my.uiSystem.HidePanel("UIActivityPanel");
                }
            }

            // 判断是否能前往
            private bool CanGoing()
            {
                if (activityConf == null)
                {
                    XYJLogger.LogError("活动id不存在" + activityConf.id);
                    return false;
                }

                if (activityConf.notOpen == 1 || hotApp.my.localPlayer.levelValue < activityConf.requireLv) // 策划开关
                {
                    xys.UI.Utility.TipContentUtil.Show("activity_not_open_tip");
                    return false;
                }

                // 需要切换场景
                if (activityConf.sceneId != 0 && activityConf.sceneId != App.my.localPlayer.GetModule<LevelModule>().levelId)
                {
                    return true;
                }

                // 打开界面
                if (activityConf.panelName != "")
                {
                    SystemHintMgr.ShowHint("功能未开放，敬请期待");
                    //App.my.uiSystem.ShowPanel(activityConf.panelName);
                    return true;
                }

                // 环任务
                if (activityConf.taskId > 0)
                {
                    return true;
                }

                // 如果有npcid，走到npc
                if (activityConf.npcId > 0)
                {
                    return true;
                }

                return false;
            }
            #endregion
        }

        public class UIActiveRewardItem
        {
            #region 活跃度奖励对象

            public Transform m_Transform;

            public void SetData(ActivityRewardData data, float curActive)
            {
                Config.ActiveAward activeConf = Config.ActiveAward.Get(data.rewardId);
                Config.Item itemData = Config.Item.Get(activeConf.itemId);

                m_Transform.Find("btn/Text").GetComponent<Text>().text = activeConf.activenessRequire.ToString(); // 所需活跃度
                Helper.SetSprite(m_Transform.Find("btn/Icon").GetComponent<Image>(), itemData.icon);

                if (curActive >= activeConf.activenessRequire)
                {
                    // 奖励状态，0未领，1是可领，2是已领
                    if (data.rewardStatus == 0)
                    {
                        m_Transform.Find("btn/receive").GetComponent<Image>().gameObject.SetActive(false);
                        m_Transform.Find("btn/Light").GetComponent<Image>().gameObject.SetActive(false);
                    }
                    else if (data.rewardStatus == 1)
                    {
                        m_Transform.Find("btn/receive").GetComponent<Image>().gameObject.SetActive(false);
                        m_Transform.Find("btn/Light").GetComponent<Image>().gameObject.SetActive(true);
                    }
                    else if (data.rewardStatus == 2)
                    {
                        m_Transform.Find("btn/receive").GetComponent<Image>().gameObject.SetActive(true);
                        m_Transform.Find("btn/Light").GetComponent<Image>().gameObject.SetActive(false);
                    }
                }
                else
                {
                    m_Transform.Find("btn/receive").GetComponent<Image>().gameObject.SetActive(false);
                    m_Transform.Find("btn/Light").GetComponent<Image>().gameObject.SetActive(false);
                }

                Button getBtn = m_Transform.Find("btn").GetComponent<Button>();
                if (getBtn != null)
                {
                    getBtn.onClick.RemoveAllListeners();
                    getBtn.onClick.AddListener(() =>
                    {
                        if (curActive >= activeConf.activenessRequire)
                        {
                            if (data.rewardStatus == 1) // 1可领，向后端发送协议
                                hotApp.my.eventSet.FireEvent(xys.EventID.Activity_RequestGetReward, data.rewardId);
                            else if (data.rewardStatus == 2)
                                UICommon.ShowItemTips(itemData.id);
                        }
                        else
                        {
                            // 打开物品提示框
                            UICommon.ShowItemTips(itemData.id);
                        }
                    });
                }

                Config.QualitySourceConfig qualitConfig = Config.QualitySourceConfig.Get(itemData.quality);
                if (qualitConfig == null)
                    return;

                Helper.SetSprite(m_Transform.Find("btn/Qulity").GetComponent<Image>(), qualitConfig.icon);
            }
            #endregion
        }

        public class ActivityItemData
        {
            #region 活动数据类
            public ActivityState activityState; // 活动状态
            public ActivityData activityData;   // 活动数据（后端）
            public SortState sortState;         // 只用于排序

            public ActivityItemData()
            {
                activityState = ActivityState.enumActivityStateUnvisiable;
                activityData = new ActivityData();
                sortState = SortState.enPriorityError;
            }
            #endregion
        }


        UIActivityDailyPage() : base(null) { }

        UIActivityDailyPage(HotTablePage page) : base(page)
        {
        }

        HotTablePage m_Page;

        [SerializeField]
        private Transform m_Grid;

        [SerializeField]
        private GameObject m_ItemObj;

        [SerializeField]
        private Text m_ActiveText;

        [SerializeField]
        private Text m_VitalityText;

        [SerializeField]
        private Image m_ActiveProgress;

        [SerializeField]
        private Transform m_ActiveGrid;

        [SerializeField]
        private GameObject m_ActiveObj;

        [SerializeField]
        private Image m_ProgressLisht;

        private ActivityMgr m_ActivityMgr;

        private const int VitalityId = 23; // 活力的itemId


        protected override void OnInit()
        {
            ActivityMgr activityMgr = hotApp.my.GetModule<HotActivityModule>().activityMgr;
            if (activityMgr != null)
            {
                m_ActivityMgr = activityMgr;
            }

            m_VitalityText.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 打开物品提示框
                UICommon.ShowItemTips(VitalityId);
            });
        }

        protected override void OnShow(object p)
        {
            Event.Subscribe(EventID.Activity_UpdateActiveUI, SetActiveData);
            Event.Subscribe(EventID.Activity_UpdateListData, SetListData);
            Event.Subscribe<float>(EventID.Activity_UpdateHuoliData, SetHuoLiData);

            SetListData();

            SetActiveData();

            SetHuoLiData(App.my.localPlayer.energyValue);
        }

        // 设置活动列表数据
        private void SetListData()
        {
            m_Grid.localPosition = new Vector2(m_Grid.localPosition.x, 0);
            List<ActivityItemData> listData = GetActivityData();

            int count = 0;
            int gridCount = m_Grid.childCount;
            for (int i = 0; i < listData.Count; i++)
            {
                GameObject go = null;
                if (i < gridCount)
                    go = m_Grid.GetChild(i).gameObject;
                else
                {
                    go = GameObject.Instantiate(m_ItemObj);
                    go.transform.SetParent(m_Grid);
                    go.transform.localScale = Vector3.one;
                }
                go.SetActive(true);

                UIActivityItem activityItem = new UIActivityItem();
                activityItem.m_Transform = go.transform;
                activityItem.SetData(listData[i]);
                count++;
            }
            for (int i = count; i < gridCount; i++)
            {
                m_Grid.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void SetHuoLiData(float vitNum)
        {
            // 活力
            m_VitalityText.text = vitNum.ToString();
        }

        // 设置活跃度数据
        private void SetActiveData()
        {
            float curActive = (float)m_ActivityMgr.m_ActivityDbData.activeNum;
            float maxActive = Config.ActiveAward.Get(Config.ActiveAward.GetAll().Count).activenessRequire;
            float progressNum = Mathf.Clamp01(curActive / maxActive);

            m_ActiveText.text = curActive.ToString();
            m_ActiveProgress.rectTransform.sizeDelta = new Vector2(progressNum * 730, 9);
            m_ProgressLisht.transform.localPosition = new Vector2(progressNum * 730 - 12.4f, m_ProgressLisht.transform.localPosition.y);

            int count = 0;
            int gridCount = m_ActiveGrid.childCount;
            foreach (ActivityRewardData data in m_ActivityMgr.m_ActivityDbData.listRewards.Values)
            {
                GameObject go = null;
                if (count < gridCount)
                    go = m_ActiveGrid.GetChild(count).gameObject;
                else
                {
                    go = GameObject.Instantiate(m_ActiveObj);
                    go.transform.SetParent(m_ActiveGrid);
                    go.transform.localScale = Vector3.one;
                }
                go.SetActive(true);

                UIActiveRewardItem activityItem = new UIActiveRewardItem();
                activityItem.m_Transform = go.transform;
                activityItem.SetData(data, curActive);
                count++;
            }
            for (int i = count; i < gridCount; i++)
            {
                m_ActiveGrid.GetChild(i).gameObject.SetActive(false);
            }
        }

        // 判断活动是否开放，等级是否达到解锁等级
        private bool IsReachLevel(int activityId)
        {
            Config.ActivityDefine activityConf = Config.ActivityDefine.Get(activityId);
            if (activityConf == null)
            {
                Debug.LogError("活动id不存在" + activityId);
                return false;
            }

            if (activityConf.notOpen == 1) // 策划开关
                return false;

            if (hotApp.my.localPlayer.levelValue < activityConf.requireLv)
                return false;

            return true;
        }

        // 获取活动状态
        private ActivityState GetActivityState(ActivityData activityData)
        {
            Config.ActivityDefine activityConf = Config.ActivityDefine.Get(activityData.activityId);
            if (activityConf == null)
            {
                Debug.LogError("活动id不存在" + activityData.activityId);
                return ActivityState.enumActivityStateUnvisiable;
            }

            if (activityConf.notOpen == 1) // 策划开关
                return ActivityState.enumActivityStateUnvisiable;

            if (hotApp.my.localPlayer.levelValue < activityConf.requireLv) // 等级不够
                return ActivityState.enumActivityStateLvLimit;

            if (activityConf.maxNum > 0 && activityData.activityNum >= activityConf.maxNum) // 次数已经到了，显示已经完成
            {
                if (activityConf.isShowEnter == 1) // 完成了仍可以参加
                    return ActivityState.enumActivityCompleteCanGo;
                else
                    return ActivityState.enumActivityStateComplete;
            }

            // 判断时间
            if (activityConf.timeId != 0)
            {
                return CheakIsToday(activityData, activityConf);
            }

            return ActivityState.enumActivityStateGo;
        }

        // 判断是否是今天
        private ActivityState CheakIsToday(ActivityData activityData, Config.ActivityDefine activityConf)
        {
            long cureentTime = hotApp.my.srvTimer.GetTime.GetCurrentTime();
            DateTime t = new DateTime(cureentTime);

            List<Config.TimesDefine> timeList = ActivityMgr.GetTimesData(activityData);

            for (int i = 0; i < timeList.Count; i++)
            {
                TimeState timeState = GetTimeState(timeList[i], t, activityConf);
                if (timeState == TimeState.notStart)      // 活动未开启
                    return ActivityState.enumActivityStateNotStart;
                else if (timeState == TimeState.over)     // 活动时间过了
                {
                    if (i == (timeList.Count - 1))
                        return ActivityState.enumActivityStateOver;
                    continue;
                }
                else if (timeState == TimeState.notToday) // 不是今天
                {
                    if (i == (timeList.Count - 1))
                        return ActivityState.enumActivityStateUnvisiable;
                    continue;
                }
                else if (timeState == TimeState.runing)
                    return ActivityState.enumActivityStateGo;
            }

            return ActivityState.enumActivityStateGo;
        }

        // 表中的时间段与当前时间对比
        public TimeState GetTimeState(Config.TimesDefine timeData, DateTime dateTime, Config.ActivityDefine activityConf)
        {
            // 先判断周几，在不在周数组里面(节日除外)
            if (Array.IndexOf(timeData.weekTime, (int)dateTime.DayOfWeek) == -1 && activityConf.activityType != 4)
                return TimeState.notToday;

            // 再判断年月日
            if (timeData.beginYear != 0 || timeData.beginMonth != 0 || timeData.beginDay != 0)
            {
                var beginData = new DateTime(timeData.beginYear == 0 ? dateTime.Year : timeData.beginYear, timeData.beginMonth, timeData.beginDay);
                var endDate = new DateTime(timeData.endYear == 0 ? dateTime.Year : timeData.endYear, timeData.endMonth, timeData.endDay) + new TimeSpan(TimeSpan.TicksPerDay);//这里要加多一天
                if (dateTime < beginData || dateTime >= endDate)
                    return TimeState.notToday;
            }

            // 再判断时段
            var beginDayTime = timeData.beginHour * 3600 + timeData.beginMinute * 60;
            var endDayTime = timeData.endHour * 3600 + timeData.endMinute * 60;
            var nowDayTime = dateTime.Hour * 3600 + dateTime.Minute * 60 + dateTime.Second;
            if (nowDayTime < beginDayTime)
                return TimeState.notStart;
            else if (nowDayTime >= endDayTime)
                return TimeState.over;
            else
                return TimeState.runing;
        }

        // 活动列表数据
        private List<ActivityItemData> GetActivityData()
        {
            List<ActivityItemData> itemsData = new List<ActivityItemData>();
            foreach (ActivityData data in m_ActivityMgr.m_ActivityDbData.listActivies.Values)
            {
                Config.ActivityDefine activityConf = Config.ActivityDefine.Get(data.activityId);
                if (hotApp.my.localPlayer.levelValue + 10 >= activityConf.requireLv) // 解锁等级不差10级
                {
                    ActivityItemData itemData = new ActivityItemData();
                    itemData.activityData = data;

                    itemData.activityState = GetActivityState(itemData.activityData);

                    if (itemData.activityState == ActivityState.enumActivityStateGo ||
                        itemData.activityState == ActivityState.enumActivityStateAccept ||
                        itemData.activityState == ActivityState.enumActivityCompleteCanGo)
                        itemData.sortState = SortState.enPriorityRuning;
                    else if (itemData.activityState == ActivityState.enumActivityStateNotStart)
                        itemData.sortState = SortState.enPriorityNotStart;
                    else if (itemData.activityState == ActivityState.enumActivityStateComplete)
                        itemData.sortState = SortState.enPriorityOver;
                    else if (itemData.activityState == ActivityState.enumActivityStateLvLimit)
                        itemData.sortState = SortState.enPriorityLvLimit;
                    else
                        itemData.sortState = SortState.enPriorityError;

                    if (itemData.activityState != ActivityState.enumActivityStateUnvisiable) // 不是今天的不要显示在活动列表中
                    {
                        ActivityState state = CheakIsToday(data, activityConf); // 当玩家等级不够、任务完成时，还要加多一层判断是否是今天
                        if (state != ActivityState.enumActivityStateUnvisiable)
                            itemsData.Add(itemData);
                    }
                }
            }

            // 排序规则,可参加 > 未开启 > 已结束(已完成) > 未解锁
            // 1，如果都是可参加，推荐排前面，然后按id排
            // 2，如果都是未开启，开始时间先排前面，然后按id
            // 3，如果都是已结束，结束时间先排前面，然后按id
            // 4，如果都是未解锁，开启等级小排前面，然后按id
            itemsData.Sort(delegate (ActivityItemData item1, ActivityItemData item2)
            {
                if (item1.sortState != item2.sortState)
                    return item1.sortState.CompareTo(item2.sortState);

                Config.ActivityDefine activityConf1 = Config.ActivityDefine.Get(item1.activityData.activityId);
                Config.ActivityDefine activityConf2 = Config.ActivityDefine.Get(item2.activityData.activityId);

                Config.TimesDefine timeConf1 = ActivityMgr.GetActivityCurrentTime(item1.activityData);
                Config.TimesDefine timeConf2 = ActivityMgr.GetActivityCurrentTime(item2.activityData);

                if (item1.sortState == SortState.enPriorityRuning)
                {
                    if (activityConf1.mark != activityConf2.mark)
                        return activityConf1.mark == 1 ? -1 : 1;
                }
                else if (item1.sortState == SortState.enPriorityNotStart)
                {
                    float time1 = (timeConf1.beginHour * 60) + timeConf1.beginMinute;
                    float time2 = (timeConf2.beginHour * 60) + timeConf2.beginMinute;

                    if (time1 != time2)
                        return time1.CompareTo(time2);
                }
                else if (item1.sortState == SortState.enPriorityOver)
                {
                    float time1 = (timeConf1.endHour * 60) + timeConf1.endMinute;
                    float time2 = (timeConf2.endHour * 60) + timeConf2.endMinute;

                    if (time1 != time2)
                        return time1.CompareTo(time2);
                }
                else if (item1.sortState == SortState.enPriorityLvLimit)
                {
                    if (activityConf1.requireLv != activityConf2.requireLv)
                        return activityConf1.requireLv.CompareTo(activityConf2.requireLv);
                }

                return activityConf1.id.CompareTo(activityConf2.id);
            });

            return itemsData;
        }
    }
}
#endif