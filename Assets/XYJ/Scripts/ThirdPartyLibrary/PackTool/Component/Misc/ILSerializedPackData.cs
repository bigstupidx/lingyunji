#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ILSerializedPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            xys.ILSerialized com = component as xys.ILSerialized;
            return __CollectObjects__(com.Objs, writer, mgr);
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            xys.ILSerialized com = pd.component as xys.ILSerialized;
            __LoadObjects__(data, reader, (Object obj, int pos) => { com.Objs[pos] = obj; });

            return data;
        }
    }
}
#endif