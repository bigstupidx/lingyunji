#if MEMORY_CHECK
using System.Threading;

public class TimerThread : Singleton<TimerThread>
{
    public TimerThread()
    {
        mTimerMgr = new TimerMgrM();
    }

    TimerMgrM mTimerMgr; // 多线程版本的定时器

    public void Start()
    {
        if (isRuning)
            return;

        isRuning = true;
        ThreadPool.QueueUserWorkItem(Src);

        MonoQuit.CreateInstance();
        MonoQuit.AddQuit(() => { Release(); });
    }

    bool isRuning = false;

    void Src(object p)
    {
        while (isRuning)
        {
            mTimerMgr.check_timer();
            Thread.Sleep(33);
        }
    }

    // 执行是在计时器线程
    public int register_timer(float intervalTime, int num, TimerMgrM.TimerFun fun, object p = null)
    {
        return mTimerMgr.register_timer((long)(intervalTime * 1000), num, fun, p);
    }

    public int register_timer(float starttime, float intervalTime, int num, TimerMgrM.TimerFun fun, object p = null)
    {
        return mTimerMgr.register_timer((long)(starttime * 1000), (long)(intervalTime * 1000), num, fun, p);
    }

    // 取消一个定时器
    public bool cannel_timer(int timerid)
    {
        return mTimerMgr.cannel_timer(timerid);
    }

    public void Release()
    {
        isRuning = false;
        mTimerMgr.Release();
    }
}
#endif