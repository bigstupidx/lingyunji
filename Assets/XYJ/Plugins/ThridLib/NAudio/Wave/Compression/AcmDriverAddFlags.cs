#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Wave.Compression
{
    /// <summary>
    /// Flags for use with acmDriverAdd
    /// </summary>
    enum AcmDriverAddFlags
    {
        // also ACM_DRIVERADDF_TYPEMASK   = 0x00000007;

        /// <summary>
        /// ACM_DRIVERADDF_LOCAL
        /// </summary>
        Local = 0,
        /// <summary>
        /// ACM_DRIVERADDF_GLOBAL
        /// </summary>
        Global = 8,
        /// <summary>
        /// ACM_DRIVERADDF_FUNCTION
        /// </summary>
        Function = 3,
        /// <summary>
        /// ACM_DRIVERADDF_NOTIFYHWND
        /// </summary>
        NotifyWindowHandle = 4,
    }
}

#endif