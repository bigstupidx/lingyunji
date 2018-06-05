#if MEMORY_CHECK
using System;
using System.Collections.Generic;
using Network;

public class MemoryChannel : Channel
{
    public List<MemoryInfo> mRecList = new List<MemoryInfo>();

    public void GetStream(List<MemoryInfo> l)
    {
        if (mRecList.Count == 0)
            return;

        lock(mRecList)
        {
            l.AddRange(mRecList);
            mRecList.Clear();
        }
    }

    public override void OnRecMessage(SocketClient socket, BitStream stream)
    {
        MemoryInfo info = new MemoryInfo();

        try
        {
            info.Read(stream);
        }
        catch (Exception ex)
        {
            Debuger.LogException(ex);
        }

        lock(mRecList)
        {
            mRecList.Add(info);
        }
    }
}

#endif