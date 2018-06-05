#if !USE_HOT
namespace xys.hot
{
    class TimerMgr
    {
        public TimerMgr(wTimer.ITimerMgr mgr)
        {
            this.mgr = mgr;

            GetTime = mgr.GetTime;
        }

        readonly wTimer.ITimerMgr mgr;

        public wTimer.IGetTime GetTime { get; private set; }

        public bool Cannel(int timerid)
        {
            return mgr.Cannel(timerid);
        }

        public int Register(float intervalTime, int num, System.Action fun)
        {
            return mgr.Register(intervalTime, num, fun);
        }

        class TAction<T>
        {
            public TAction(System.Action<T> fun, T p)
            {
                this.fun = fun;
                this.p = p;
            }

            System.Action<T> fun;
            T p;

            public void OnCall()
            {
                if (fun != null)
                {
                    fun(p);
                }
            }
        }

        class TAction<T1, T2>
        {
            public TAction(System.Action<T1, T2> fun, T1 p1, T2 p2)
            {
                this.fun = fun;
                this.p1 = p1;
                this.p2 = p2;
            }

            System.Action<T1, T2> fun;
            T1 p1;
            T2 p2;

            public void OnCall()
            {
                if (fun != null)
                {
                    fun(p1, p2);
                }
            }
        }

        public int Register<T>(float intervalTime, int num, System.Action<T> fun, T p)
        {
            TAction<T> f = new TAction<T>(fun, p);
            return mgr.Register(intervalTime, num, f.OnCall);
        }

        public int Register<T1, T2>(float intervalTime, int num, System.Action<T1, T2> fun, T1 t1, T2 t2)
        {
            TAction<T1,T2> f = new TAction<T1,T2>(fun, t1, t2);
            return mgr.Register(intervalTime, num, f.OnCall);
        }

        public int Register(float firstTime, float intervalTime, int num, System.Action fun)
        {
            return mgr.Register(firstTime, intervalTime, num, fun);
        }

        public int Register<T>(float firstTime, float intervalTime, int num, System.Action<T> fun, T p)
        {
            TAction<T> f = new TAction<T>(fun, p);
            return mgr.Register(firstTime, intervalTime, num, f.OnCall);
        }

        public int Register<T1, T2>(float firstTime, float intervalTime, int num, System.Action<T1, T2> fun, T1 t1, T2 t2)
        {
            TAction<T1, T2> f = new TAction<T1, T2>(fun, t1, t2);
            return mgr.Register(firstTime, intervalTime, num, f.OnCall);
        }

        public int RegisterDay(byte hour, byte minute, byte second, int num, System.Action fun)
        {
            return mgr.RegisterDay(hour, minute, second, num, fun);
        }

        public int RegisterMonth(byte day, byte hour, byte minute, byte second, System.Action fun)
        {
            return mgr.RegisterMonth(day, hour, minute, second, fun);
        }

        public int RegisterMonth(byte month, byte day, byte hour, byte minute, byte second, System.Action fun)
        {
            return mgr.RegisterMonth(month, day, hour, minute, second, fun);
        }

        public int RegisterWeek(byte day, byte hour, byte minute, byte second, int num, System.Action fun)
        {
            return mgr.RegisterWeek(day, hour, minute, second, num, fun);
        }
    }
}
#endif