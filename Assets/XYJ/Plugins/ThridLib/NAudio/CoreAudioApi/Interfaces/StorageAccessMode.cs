#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.CoreAudioApi.Interfaces
{
    /// <summary>
    /// MMDevice STGM enumeration
    /// </summary>
    enum StorageAccessMode
    {
        Read,
        Write,
        ReadWrite
    }
}

#endif