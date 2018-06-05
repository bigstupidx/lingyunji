#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


using xys.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Config;
using System;
using NetProto.Hot;


namespace xys.hot.UI
{
    [AutoILMono]
    class UIActivityWeekPanel
    {
        [SerializeField]
        private UIGroup m_WeekGroup;

        [SerializeField]
        private Transform m_Transform;

        [SerializeField]
        private UIActivityWeekScrollView m_WeekScrollView;

        [SerializeField]
        private Button m_CloseBtn;

        [SerializeField]
        private Animator m_Animator;

        protected int m_ClickHandlerId = 0;

        private Dictionary<int, List<ActivityData>> m_WeekTable = new Dictionary<int, List<ActivityData>>();


        void Awake()
        {
            m_CloseBtn.onClick.AddListener(() =>
            {
                CloseEvent(null);
            });
        }

        public void SetData(Dictionary<int, ActivityData> data)
        {
            GetWeekData(data);

            CreateActivies();

            this.m_Transform.gameObject.SetActive(true);
            PlayAnimation(true);
        }

        private void GetWeekData(Dictionary<int, ActivityData> activityData)
        {
            m_WeekTable.Clear();

            foreach (ActivityData data in activityData.Values)
            {
                ActivityDefine activityConf = ActivityDefine.Get(data.activityId);
                for (int j = 0; j < activityConf.openDayOfWeek.Length; j++)
                {
                    if (!m_WeekTable.ContainsKey(activityConf.openDayOfWeek[j]))
                    {
                        m_WeekTable.Add(activityConf.openDayOfWeek[j], new List<ActivityData>());
                    }

                    m_WeekTable[activityConf.openDayOfWeek[j]].Add(data);
                }
            }

            // sort
            foreach (List<ActivityData> value in m_WeekTable.Values)
            {
                value.Sort(delegate (ActivityData activity1, ActivityData activity2)
                {
                    TimesDefine activityOpenTime1 = GetCurrentTime(activity1);
                    TimesDefine activityOpenTime2 = GetCurrentTime(activity2);

                    if (activityOpenTime1.beginHour == activityOpenTime2.beginHour)
                    {
                        if (activityOpenTime1.beginMinute == activityOpenTime2.beginMinute)
                        {
                            if (activityOpenTime1.endHour == activityOpenTime2.endHour)
                            {
                                if (activityOpenTime1.endMinute == activityOpenTime2.endMinute)
                                {
                                    return activity1.activityId.CompareTo(activity2.activityId);
                                }

                                return activityOpenTime1.endMinute.CompareTo(activityOpenTime2.endMinute);
                            }

                            return activityOpenTime1.endHour.CompareTo(activityOpenTime2.endHour);
                        }

                        return activityOpenTime1.beginMinute.CompareTo(activityOpenTime2.beginMinute);
                    }

                    return activityOpenTime1.beginHour.CompareTo(activityOpenTime2.beginHour);
                });
            }
        }

        // 有多个时间段
        private TimesDefine GetCurrentTime(ActivityData activityData)
        {
            List<Config.TimesDefine> timeStrs = GetTimesData(activityData);
            for (int i = 0; i < timeStrs.Count; i++)
            {
                float endTimes = timeStrs[i].endHour * 60 + timeStrs[i].endMinute;

                long cureentTime = hotApp.my.srvTimer.GetTime.GetCurrentTime();
                DateTime serverTime = new DateTime(cureentTime);
                float serverTimes = serverTime.Hour * 60 + serverTime.Minute;

                if (serverTimes > endTimes)
                {
                    if (i == (timeStrs.Count - 1))
                        return timeStrs[timeStrs.Count - 1];

                    continue;
                }
                else
                    return timeStrs[i];
            }

            return null;
        }

        // 获取活动所有时间段数据
        private List<Config.TimesDefine> GetTimesData(ActivityData activityData)
        {
            Dictionary<int, Config.TimesDefine> allTimeConf = Config.TimesDefine.GetAll();
            Dictionary<int, Config.TimesDefine>.Enumerator itr = allTimeConf.GetEnumerator();

            Config.ActivityDefine activityConf = Config.ActivityDefine.Get(activityData.activityId);
            List<Config.TimesDefine> timesList = new List<Config.TimesDefine>();

            while (itr.MoveNext())
            {
                if (itr.Current.Value.timeId == activityConf.timeId)
                    timesList.Add(itr.Current.Value);
            }

            return timesList;
        }

        private void CreateActivies()
        {
            long cureentTime = hotApp.my.srvTimer.GetTime.GetCurrentTime();
            DateTime serverTime = new DateTime(cureentTime);

            int today = Convert.ToInt32(serverTime.DayOfWeek.ToString("d")); // 今天是周几

            m_WeekScrollView.SetData(m_WeekTable, today == 0 ? 7 : today);
        }

        private void PlayAnimation(bool isOpen)
        {
            if (isOpen)
                AnimationHelp.PlayAnimation(m_Animator, "open", "ui_Tankuang_Activity", null);
            else
                AnimationHelp.PlayAnimation(m_Animator, "close", "ui_Tankuang_Activity_Close", this.CloseEvent);
        }

        private void CloseEvent(object obj)
        {
            this.m_Transform.gameObject.SetActive(false);
        }
    }
}
#endif