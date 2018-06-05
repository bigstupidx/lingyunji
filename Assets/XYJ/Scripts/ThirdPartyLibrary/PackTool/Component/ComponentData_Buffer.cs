using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
#if USE_RESOURCESEXPORT
    // 组件所需要保存的数据
    public abstract partial class ComponentData
    {
        protected static Data CreateData(ParamData pd)
        {
            Data data = Buff<Data>.Get();
            data.Reset(pd);
            return data;
        }

        public static ParamData CreateParamData(Component c, OnComponentEnd e, object ep)
        {
            ParamData data = Buff<ParamData>.Get();
            data.Reset(c, e, ep);
            return data;
        }

        public static void FreeParamData(ParamData pd)
        {
            pd.Reset(null, null, null);
            Buff<ParamData>.Free(pd);
        }
    }
#endif
}