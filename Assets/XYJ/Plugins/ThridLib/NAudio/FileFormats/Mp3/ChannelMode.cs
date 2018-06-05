#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Wave
{
    /// <summary>
    /// Channel Mode
    /// </summary>
    public enum ChannelMode
    {
        /// <summary>
        /// Stereo
        /// </summary>
        Stereo,
        /// <summary>
        /// Joint Stereo
        /// </summary>
        JointStereo,
        /// <summary>
        /// Dual Channel
        /// </summary>
        DualChannel,
        /// <summary>
        /// Mono
        /// </summary>
        Mono
    }

}

#endif