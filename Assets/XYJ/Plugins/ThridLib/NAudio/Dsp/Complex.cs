#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Dsp
{
    /// <summary>
    /// Type to represent complex number
    /// </summary>
    public struct Complex
    {
        /// <summary>
        /// Real Part
        /// </summary>
        public float X;
        /// <summary>
        /// Imaginary Part
        /// </summary>
        public float Y;
    }
}

#endif