using FMOD;
using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class BankFileLoad
{
    public string filename;

    System.IO.FileStream fileStream;

    public static string dataPath = Application.dataPath;

    public RESULT Open(StringWrapper name, ref uint filesize, ref IntPtr handle, IntPtr userdata)
    {
        try
        {
            fileStream = new System.IO.FileStream(string.Format("{0}/Data/sounds/Desktop/{1}.bank", dataPath, filename), System.IO.FileMode.Open, System.IO.FileAccess.Read);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
            return RESULT.ERR_ALREADY_LOCKED;
        }

        filesize = (uint)fileStream.Length;

        UnityEngine.Debug.LogFormat("BankFileLoad name:{0} filesize:{1}", filename, filesize);
        handle = IntPtr.Zero;

        return RESULT.OK;
    }

    public RESULT Close(IntPtr handle, IntPtr userdata)
    {
        if (fileStream != null)
            fileStream.Close();

        return RESULT.OK;
    }

    public RESULT Read(IntPtr handle, IntPtr buffer, uint sizebytes, ref uint bytesread, IntPtr userdata)
    {
        byte[] bytes = new byte[1024 * 10]; // 10K个字节的数据

        int readsizelenght = Mathf.Min((int)sizebytes, bytes.Length);
        int readsize;
        while ((readsize = fileStream.Read(bytes, 0, readsizelenght)) != 0)
        {
            bytesread += (uint)readsize;
            sizebytes -= (uint)readsize;

            Marshal.Copy(bytes, 0, buffer, readsize);
            buffer = (IntPtr)(((int)buffer) + readsize);

            if (sizebytes <= 0)
                break;

            readsizelenght = Mathf.Min((int)sizebytes, bytes.Length);
        }

        return RESULT.OK;
    }

    public RESULT Seek(IntPtr handle, uint pos, IntPtr userdata)
    {
        fileStream.Seek(pos, System.IO.SeekOrigin.Begin);

        return RESULT.OK;
    }
}