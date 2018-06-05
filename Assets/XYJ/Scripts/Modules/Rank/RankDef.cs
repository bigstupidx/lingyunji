#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys.hot
{
    public static class RankDef
    {
    }

    public static class RankUtil
    {
        public const long NANOSECONDS_PER_SECOND = 1000000000;
        public const long HUNDRED_NANOSECONDS_PER_SECOND = 10000000;
        public static long GetNowSecond()
        {
            return App.my.mainTimer.GetTime.GetCurrentTime() / HUNDRED_NANOSECONDS_PER_SECOND;
        }
        public static long GetNowTick()
        {
            return App.my.mainTimer.GetTime.GetCurrentTime();
        }
    }

}

#endif