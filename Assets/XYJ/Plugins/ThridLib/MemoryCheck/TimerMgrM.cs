#if MEMORY_CHECK
using System.Collections.Generic;
using System.Threading;

// 定时器管理,支持多线程版本
public class TimerMgrM
{
    // 得到当前时间,返回单位为毫秒
    public delegate long GetCurrentTime();

    static long GetDefaultCurrentTime()
    {
        return System.Environment.TickCount;
    }

    public TimerMgrM(GetCurrentTime time = null)
    {
        if (time == null)
            mCurrentTime = GetDefaultCurrentTime;
        else
            mCurrentTime = time;
    }

    GetCurrentTime mCurrentTime;

    public delegate void TimerFun(System.Object p);

    public const int invalid = 0;

    // 定时器
    public class Timer
    {
        public int id; // 定时器ID
        public long lastTime; // 调用的时间,毫秒
        public long intervalTime; // 间隔时间,毫秒
        public bool bCannel; // 是否取消定时
        public int num; // 回调的次数

        public TimerFun fun;
        public System.Object p;

        public void Release()
        {
            id = invalid; // 定时器ID
            lastTime = 0; // 调用的时间,毫秒
            intervalTime = 0; // 间隔时间,毫秒
            num = 0; // 回调的次数

            fun = null;
            p = null;
        }
    }

    private int m_timerid = invalid;
    private SortedDictionaryM<long, Timer> m_timerList = new SortedDictionaryM<long,Timer>(); // 定时器回调时间戳做key
    private Dictionary<int, Timer> m_timerMap = new Dictionary<int, Timer>(); // 映射
    private object object_lock = new object();

    // 注册一个定时器回调
    public int register_timer(long intervalTime, int num, TimerFun fun, object p)
    {
        return register_timer(mCurrentTime() + intervalTime, intervalTime, num, fun, p);
    }

    public int register_timer(long starttime, long intervalTime, int num, TimerFun fun, object p)
    {
        if (fun == null)
            return invalid;

        Timer timer = new Timer();
        timer.intervalTime = intervalTime;
        timer.lastTime = starttime;
        timer.num = num;
        timer.fun = fun;
        timer.p = p;
        timer.bCannel = false;
        timer.id = System.Threading.Interlocked.Increment(ref m_timerid);

        addTimer(timer);

        return timer.id;
    }

    // 取消一个定时器
    public bool cannel_timer(int timerid)
    {
        if (timerid == invalid)
            return false;

        Timer timer = null;
        lock (object_lock)
        {
            if (m_timerMap.TryGetValue(timerid, out timer) == false)
                return false;

            m_timerList.Remove(timer.lastTime, timer);
            m_timerMap.Remove(timerid);
        }

        RelaseTimer(timer);
        return true;
    }

    void RelaseTimer(Timer timer)
    {
        timer.bCannel = true;
        timer.fun = null;
        timer.p = null;

        timer.Release();
        timer = null;
    }

    public void Release()
    {
        lock (object_lock)
        {
            foreach (KeyValuePair<int, Timer> itor in m_timerMap)
            {
                RelaseTimer(itor.Value);
            }

            m_timerMap.Clear();
        }
    }

    Timer[] checkList = new Timer[10]; // 一次最多回调十个定时器

    int temp_start_index = 0;
    int temp_count = 0;
    long temp_cur_time = 0;

    // 检测定时器
    public void check_timer()
    {
        temp_start_index = 0;
        temp_count = m_timerList.Count;
        if (temp_count == 0)
            return;

        temp_cur_time = mCurrentTime();
        Timer timer = null;
        lock (object_lock)
        {
            foreach (KeyValuePair<long, Timer> itor in m_timerList)
            {
                temp_count--;
                timer = itor.Value;
                if (temp_cur_time >= itor.Key)
                {
                    // 到回调时间了
                    checkList[temp_start_index] = timer;
                    ++temp_start_index;
                    if (temp_start_index >= 10)
                        break;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < temp_start_index; ++i)
            {
                timer = checkList[i];
                if (!m_timerList.Remove(timer.lastTime, timer))
                {
                    Logger.LogError("TimeRemove Error!");
                }

                Logger.LogDebug("CallBack:{0}", timer.id);
            }
        }

        for (int i = 0; i < temp_start_index; ++i)
        {
            timer = checkList[i];
            checkList[i] = null;
            if (timer.bCannel == true)
            {
                bool isremove = false;
                lock (object_lock)
                {
                    isremove = m_timerMap.Remove(timer.id);
                }

                if (isremove)
                {
                    RelaseTimer(timer);
                }
            }
            else
            {
                try
                {
                    timer.num--;

                    if (timer.fun == null)
                        Logger.LogError("timer.fun == null");
                    else
                    {
                        timer.fun(timer.p);
                    }
                }
                catch (System.Exception e)
                {
                    Logger.LogException(e);
                }

                if (timer.num > 0 && timer.bCannel == false)
                {
                    timer.lastTime = temp_cur_time + timer.intervalTime;
                    lock (object_lock)
                    {
                        m_timerList.Add(timer.lastTime, timer);
                    }
                }
                else
                {
                    bool isremove = false;
                    lock (object_lock)
                    {
                        isremove = m_timerMap.Remove(timer.id);
                    }

                    if (isremove)
                    {
                        RelaseTimer(timer);
                    }
                }
            }
        }
    }

    void addTimer(Timer timer)
    {
        lock (object_lock)
        {
            m_timerList.Add(timer.lastTime, timer);
            if (m_timerMap.ContainsKey(timer.id))
                Logger.LogError(string.Format("timerid:{0} repeat!", timer.id));
            else
                m_timerMap.Add(timer.id, timer);

            Logger.LogDebug("addTimer:{0}", timer.id);
        }
    }
}
#endif