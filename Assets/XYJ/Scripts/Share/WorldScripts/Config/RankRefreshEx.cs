using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config
{
    public partial class RankRefresh
    {
        public static void OnLoadEndLine(RankRefresh data, CsvCommon.ICsvLoad reader, int i)
        {
            Array.Sort(data.weekDays);
        }
        public static long CalRefreshTick(int rankType, long inputTick)
        {
            long retTick = 0;
            RankRefresh cfg = RankRefresh.Get(rankType);
            if (null != cfg && cfg.weekDays.Length > 0)
            {
                DateTime dt = new DateTime(inputTick);

                int fitDayOfWeek = cfg.weekDays[0];
                bool dayInSameWeek = false;
                foreach (int dayOfWeek in cfg.weekDays)
                {
                    if (dayOfWeek != (int)dt.DayOfWeek)
                    {
                        if (dayOfWeek < (int)dt.DayOfWeek)
                            continue;
                        dayInSameWeek = true;
                        fitDayOfWeek = dayOfWeek;
                        break;
                    }
                    if (cfg.hour != dt.Hour)
                    {
                        if (cfg.hour < dt.Hour)
                            continue;
                        dayInSameWeek = true;
                        fitDayOfWeek = dayOfWeek;
                        break;
                    }
                    if (cfg.min != dt.Minute)
                    {
                        if (cfg.min < dt.Minute)
                            continue;
                        dayInSameWeek = true;
                        fitDayOfWeek = dayOfWeek;
                        break;
                    }
                }

                int daySpan = (dayInSameWeek ? 0 : 7) + fitDayOfWeek - (int)dt.DayOfWeek;
                DateTime retDt = new DateTime(dt.Year, dt.Month, dt.Day, cfg.hour, cfg.min, 0);
                retDt = retDt.AddDays(daySpan);
                retTick = retDt.Ticks;
            }
            return retTick;
        }
    }
}