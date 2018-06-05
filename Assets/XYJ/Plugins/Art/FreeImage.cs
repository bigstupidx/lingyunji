#if UNITY_EDITOR
using System;
using UnityEditor;
using System.Runtime.InteropServices;

public static class FreeImage
{
    public static int GetFileType(string file)
    {
        return FreeImage_GetFileType(file);
    }

    [DllImport("FreeImage")]
    static extern int FreeImage_GetFileType(string file);

    [DllImport("FreeImage")]
    static extern int FreeImage_FIFSupportsReading(int type);

    public static int FIFSupportsReading(int type)
    {
        return FreeImage_FIFSupportsReading(type);
    }

    [DllImport("FreeImage")]
    static extern IntPtr FreeImage_Load(int type, string file, int flags);
    public static IntPtr Load(int type, string file, int flags)
    {
        return FreeImage_Load(type, file, flags);
    }

    [DllImport("FreeImage")]
    static extern void FreeImage_Unload(IntPtr ptr);
    static public void Unload(IntPtr ptr)
    {
        FreeImage_Unload(ptr);
    }

    [DllImport("FreeImage")]
    static extern void FreeImage_Save(int type, IntPtr ptr, string file, int flags);
    static public void Save(int type, IntPtr ptr, string file, int flags)
    {
        FreeImage_Save(type, ptr, file, flags);
    }

    [DllImport("FreeImage")]
    static extern int FreeImage_GetBPP(IntPtr ptr);
    static public int GetBPP(IntPtr ptr)
    {
        return FreeImage_GetBPP(ptr);
    }

    [DllImport("FreeImage")]
    static extern int FreeImage_GetWidth(IntPtr ptr);

    static public int GetWidth(IntPtr ptr)
    {
        return FreeImage_GetWidth(ptr);
    }

    [DllImport("FreeImage")]
    static extern int FreeImage_GetHeight(IntPtr ptr);

    static public int GetHeight(IntPtr ptr)
    {
        return FreeImage_GetHeight(ptr);
    }

    [DllImport("FreeImage")]
    static extern int FreeImage_GetPitch(IntPtr ptr);

    static public int GetPitch(IntPtr ptr)
    {
        return FreeImage_GetPitch(ptr);
    }

    [DllImport("FreeImage")]
    static extern IntPtr FreeImage_GetBits(IntPtr ptr);

    static public IntPtr GetBits(IntPtr ptr)
    {
        return FreeImage_GetBits(ptr);
    }


    [DllImport("FreeImage")]
    static extern IntPtr FreeImage_GetScanLine(IntPtr ptr, int y);

    static public IntPtr GetScanLine(IntPtr ptr, int y)
    {
        return FreeImage_GetScanLine(ptr, y);
    }
}
#endif