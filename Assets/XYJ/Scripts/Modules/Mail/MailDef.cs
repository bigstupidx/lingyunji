#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys.hot
{
    static class MailDef
    {
        public const long NANOSECONDS_PER_SECOND = 1000000000;
        public const long HUNDRED_NANOSECONDS_PER_SECOND = 10000000;
        public const long MAIL_SAVE_TICKS = 7 * 24 * 60 * 60 * MailDef.HUNDRED_NANOSECONDS_PER_SECOND;
        public const int MAIL_SAVE_MAX_COUNT = 256;
        public static bool IsMailExpired(long nowTick, long createTick)
        {
            return createTick < nowTick - MailDef.MAIL_SAVE_TICKS;
        }
        public static long GetNowSecond()
        { 
            return App.my.srvTimer.GetTime.GetCurrentTime() / HUNDRED_NANOSECONDS_PER_SECOND;
        } 

        public static long GetNowTick()
        {
            return App.my.srvTimer.GetTime.GetCurrentTime();
        }

        public static MailMgr mailMgr { get { return hotApp.my.GetModule<HotMailModule>().mailMgr; } }

        public static bool HasAttachement(MailDbRecord mail)
        {
            if (null != mail && null != mail.attachments && mail.attachments.Count > 0)
                return true;
            return false;
        }
    }
}
#endif