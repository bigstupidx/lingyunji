#if MEMORY_CHECK
using System;
using System.Collections.Generic;

public class MemoryObjectMgr
{
    static MemoryInfo mMemoryInfo = new MemoryInfo();

    public static bool isDirty
    {
        get { return mMemoryInfo.isDirty; }
        set { mMemoryInfo.isDirty = value; }
    }

    public static void GetAll(MemoryInfo info)
    {
        lock (mMemoryInfo)
        {
            mMemoryInfo.CopyTo(info);
        }
    }

    public static string Text()
    {
        return string.Format("{0}/{1}", mMemoryInfo.ToString());
    }

    public static void Add(object obj)
    {
        lock(mMemoryInfo)
        {
            mMemoryInfo.Add(obj);
        }
    }

    public static void Sub(object obj)
    {
        lock (mMemoryInfo)
        {
            mMemoryInfo.Sub(obj);
        }
    }

    public static void GetInfo(System.Text.StringBuilder sb)
    {

    }
}

#endif