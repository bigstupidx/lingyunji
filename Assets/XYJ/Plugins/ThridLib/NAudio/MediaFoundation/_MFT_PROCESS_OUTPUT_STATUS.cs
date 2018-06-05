#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;

namespace NAudio.MediaFoundation
{
    /// <summary>
    /// Process Output Status flags
    /// </summary>
    [Flags]
    public enum _MFT_PROCESS_OUTPUT_STATUS
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0, 
        /// <summary>
        /// The Media Foundation transform (MFT) has created one or more new output streams.
        /// </summary>
        MFT_PROCESS_OUTPUT_STATUS_NEW_STREAMS = 0x00000100
    }
}
#endif