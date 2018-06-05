#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Wave
{
    /// <summary>
    /// MPEG Layer flags
    /// </summary>
    public enum MpegLayer
    {
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved,
        /// <summary>
        /// Layer 3
        /// </summary>
        Layer3,
        /// <summary>
        /// Layer 2
        /// </summary>
        Layer2,
        /// <summary>
        /// Layer 1
        /// </summary>
        Layer1
    }
}

#endif