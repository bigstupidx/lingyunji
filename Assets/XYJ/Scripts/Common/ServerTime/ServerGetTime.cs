using NetProto;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wTimer;
using xys;

namespace xys
{
    public class ServerGetTime : IGetTime
    {
        public static void Init()
        {
            timerHandler = new TimerHandler(App.my.mainTimer);

            App.my.handler.Reg<RspServerTime>(Protoid.C2A_Ack_ServerTime, OnRspServerTime);
            App.my.eventSet.Subscribe(StateEvent.Begin, AppStateType.GameIn, (IAppState state) => { OnAppStateChange(true); });
            App.my.eventSet.Subscribe(StateEvent.End, AppStateType.GameIn, (IAppState state) => { OnAppStateChange(false); });
        }

        static public long deltaTick = 0;

        public static void ReqServerTime()
        {
            ReqServerTime msg = new ReqServerTime();
            msg.clientTimeTicks = DateTime.Now.Ticks;
            App.my.socket.SendGame(Protoid.C2A_Ack_ServerTime, msg);
        }

        public static void OnRspServerTime(IPacket packet, RspServerTime input)
        {
            long nowTicks = DateTime.Now.Ticks; // 10^-7 秒
            long delta_1 = input.serverTimeTicks - input.clientTimeTicks;
            long delta_2 = DateTime.Now.Ticks - input.serverTimeTicks;

            deltaTick = input.serverTimeTicks - input.clientTimeTicks;
        }

        static TimerHandler timerHandler;

        public static void OnAppStateChange(bool isEnter)
        {
            if (isEnter)
            {
                ReqServerTime();
                timerHandler.Register(300, -1, () => { ReqServerTime(); });
            }
            else
                timerHandler.Cannel();
        }

        public long GetCurrentTime()
        {
            return DateTime.Now.Ticks + deltaTick;
        }

        // 得到当天的hour点minute分second秒的时间戳
        public long GetDayTime(byte hour, byte minute, byte second)
        {
            DateTime now = new DateTime(DateTime.Now.Ticks + deltaTick);
            return CommonBase.Help.GetDayTime(now, hour, minute, second).Ticks;
        }

        // 得到当周的星期几，几点几分几秒的时间戳
        public long GetWeekTime(byte day, byte hour, byte minute, byte second)
        {
            DateTime now = new DateTime(DateTime.Now.Ticks + deltaTick);
            return CommonBase.Help.GetWeekTime(now, day, hour, minute, second).Ticks;
        }

        public long GetMonthTime(byte day, byte hour, byte minute, byte second)
        {
            DateTime now = new DateTime(DateTime.Now.Ticks + deltaTick);
            return CommonBase.Help.GetMonthTime(now, day, hour, minute, second).Ticks;
        }

        // 得到今年的某一个月的某天，几点几分几秒的时间戳
        public long GetMonthTime(byte month, byte day, byte hour, byte minute, byte second)
        {
            DateTime now = new DateTime(DateTime.Now.Ticks + deltaTick);
            return CommonBase.Help.GetMonthTime(now, month, day, hour, minute, second).Ticks;
        }

        // 得到下一个月的几号几点几分几秒
        public long GetNextMonthTime(byte day, byte hour, byte minute, byte second)
        {
            DateTime now = new DateTime(DateTime.Now.Ticks + deltaTick);
            return CommonBase.Help.GetNextMonthTime(now, day, hour, minute, second).Ticks;
        }
    }
}
