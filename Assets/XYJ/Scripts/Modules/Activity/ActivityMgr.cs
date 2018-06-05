#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


namespace xys.hot
{
    using NetProto.Hot;
    using System;
    using System.Collections.Generic;

    public class ActivityMgr
    {
        public ActivityDbData m_ActivityDbData { get; set; }

        public ActivityMgr()
        {
            m_ActivityDbData = new ActivityDbData();
        }

        /// <summary>
        /// 多个时间段，得到当前时间段
        /// </summary>
        public static Config.TimesDefine GetActivityCurrentTime(ActivityData activityData)
        {
            List<Config.TimesDefine> timeList = GetTimesData(activityData);

            if (timeList.Count > 1)
            {
                long cureentTime = hotApp.my.srvTimer.GetTime.GetCurrentTime();
                DateTime t = new DateTime(cureentTime);

                for (int i = 0; i < timeList.Count; i++)
                {
                    int endTime = (timeList[i].endHour * 60) + timeList[i].endMinute;
                    int serverTime = (t.Hour * 60) + t.Minute;
                    if (serverTime > endTime)
                    {
                        if (i == (timeList.Count - 1))
                            return timeList[timeList.Count - 1];

                        continue;
                    }
                    else
                        return timeList[i];
                }
            }
            else if (timeList.Count == 1)
                return timeList[0];

            return null;
        }

        /// <summary>
        /// 获取活动所有时间段数据
        /// </summary>
        public static List<Config.TimesDefine> GetTimesData(ActivityData activityData)
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

        /// <summary>
        /// 得到具体的时间 12:00-13:00
        /// </summary>
        public static string GetTimeStr(ActivityData data, Config.ActivityDefine activityConf, bool isShowTab = false)
        {
            Config.TimesDefine timeConf = GetActivityCurrentTime(data);
            string timeStr = "";

            if (timeConf.beginYear > 0 && activityConf.activityType != 4) // 策划说节日只显示时间段
            {
                if (timeConf.beginYear != timeConf.endYear || timeConf.beginMonth != timeConf.endMonth || timeConf.beginDay != timeConf.endDay) // 多天
                {
                    timeStr = (IsShowTimeStr(timeConf) ? (isShowTab ? "全天" : "") :
                    (SetTime(timeConf.beginYear) + "-" + SetTime(timeConf.beginMonth) + "-" + SetTime(timeConf.beginDay) + "-"
                    + SetTime(timeConf.endYear) + "-" + SetTime(timeConf.endMonth) + "-" + SetTime(timeConf.endDay) + " " +
                    SetTime(timeConf.beginHour) + ":" + SetTime(timeConf.beginMinute) + "-" + SetTime(timeConf.endHour) + ":" +
                    SetTime(timeConf.endMinute)));
                }
                else // 一天
                {
                    timeStr = (IsShowTimeStr(timeConf) ? (isShowTab ? "全天" : "") :
                    (SetTime(timeConf.beginYear) + "-" + SetTime(timeConf.beginMonth) + "-" + SetTime(timeConf.beginDay) + " " +
                        SetTime(timeConf.beginHour) + ":" + SetTime(timeConf.beginMinute) + "-" + SetTime(timeConf.endHour) + ":" +
                    SetTime(timeConf.endMinute)));
                }
            }
            else // 时间段
            {
                timeStr = (IsShowTimeStr(timeConf) ? (isShowTab ? "全天" : "") : (SetTime(timeConf.beginHour) + ":" +
                    SetTime(timeConf.beginMinute) + "-" + SetTime(timeConf.endHour) + ":" + SetTime(timeConf.endMinute)));
            }

            return timeStr;
        }

        private static string SetTime(int timeNum)
        {
            string str = (timeNum <= 9 ? "0" + timeNum : timeNum.ToString());
            return str;
        }

        public static bool IsShowTimeStr(Config.TimesDefine timeConf)
        {
            bool isShow = false;
            if (timeConf.beginYear == 0 && timeConf.beginMonth == 0 && timeConf.beginDay == 0 && timeConf.beginHour == 0 &&
                timeConf.beginMinute == 0 && timeConf.endHour == 23 && timeConf.endMinute == 59)
                isShow = true;

            return isShow;
        }
    }
}
#endif