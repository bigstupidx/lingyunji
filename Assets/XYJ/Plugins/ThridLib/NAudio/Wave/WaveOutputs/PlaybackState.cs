#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Wave
{
    /// <summary>
    /// Playback State
    /// </summary>
    public enum PlaybackState
    {
        /// <summary>
        /// Stopped
        /// </summary>
        Stopped,
        /// <summary>
        /// Playing
        /// </summary>
        Playing,
        /// <summary>
        /// Paused
        /// </summary>
        Paused
    }
}

#endif