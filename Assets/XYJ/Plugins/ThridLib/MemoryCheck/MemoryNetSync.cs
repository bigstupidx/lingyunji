#if MEMORY_CHECK
using Network;

public class MemoryNetSync : Singleton<MemoryNetSync>
{
    ~MemoryNetSync()
    {
        Release();
    }

    int timerid = 0;

    public void Release()
    {
        if (timerid != 0 && TimerThread.IsCreate)
        {
            TimerThread.Instance.cannel_timer(timerid);
            timerid = 0;
        }
    }

    public void Start()
    {
        // 每隔一秒就发送一次内存数据
        if (timerid == 0)
            timerid = TimerThread.Instance.register_timer(1f, int.MaxValue, ThreadUpdate);
    }

    MemoryInfo mMemoryInfo = new MemoryInfo();

    void ThreadUpdate(object p)
    {
        if (!MemoryObjectMgr.isDirty)
            return;

        MemoryObjectMgr.GetAll(mMemoryInfo);
        MemoryObjectMgr.isDirty = false;
        DebugNet.Instance.Send(DebugProtocol.ObjectMemory, (BitStream bitStream) =>
        {
            mMemoryInfo.Write(bitStream);
        });
    }
}

#endif