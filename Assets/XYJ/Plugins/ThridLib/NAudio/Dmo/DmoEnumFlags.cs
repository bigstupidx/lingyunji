#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;
using System.Collections.Generic;
using System.Text;

namespace NAudio.Dmo
{
    [Flags]
    enum DmoEnumFlags
    {
        None,
        DMO_ENUMF_INCLUDE_KEYED = 0x00000001
    }
}

#endif