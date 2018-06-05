#if MEMORY_CHECK
using System.Threading;

public class TimerThread : Singleton<TimerThread>
{
    public TimerThread()
    {
        mTimerMgr = new TimerMgrM();
    }

    TimerMgrM mTimerMgr; // ���̰߳汾�Ķ�ʱ��

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

    // ִ�����ڼ�ʱ���߳�
    public int register_timer(float intervalTime, int num, TimerMgrM.TimerFun fun, object p = null)
    {
        return mTimerMgr.register_timer((long)(intervalTime * 1000), num, fun, p);
    }

    public int register_timer(float starttime, float intervalTime, int num, TimerMgrM.TimerFun fun, object p = null)
    {
        return mTimerMgr.register_timer((long)(starttime * 1000), (long)(intervalTime * 1000), num, fun, p);
    }

    // ȡ��һ����ʱ��
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