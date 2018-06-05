#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;

namespace NAudio.CoreAudioApi
{
    /// <summary>
    /// AUDCLNT_SHAREMODE
    /// </summary>
    public enum AudioClientShareMode
    {
        /// <summary>
        /// AUDCLNT_SHAREMODE_SHARED,
        /// </summary>
        Shared,
        /// <summary>
        /// AUDCLNT_SHAREMODE_EXCLUSIVE
        /// </summary>
        Exclusive,
    }
}

#endif