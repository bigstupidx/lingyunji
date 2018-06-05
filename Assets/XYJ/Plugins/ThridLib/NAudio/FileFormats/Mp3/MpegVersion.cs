#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Wave
{
    /// <summary>
    /// MPEG Version Flags
    /// </summary>
    public enum MpegVersion
    {
        /// <summary>
        /// Version 2.5
        /// </summary>
        Version25,
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved,
        /// <summary>
        /// Version 2
        /// </summary>
        Version2,
        /// <summary>
        /// Version 1
        /// </summary>
        Version1
    }
}

#endif