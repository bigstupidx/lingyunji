#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class MaterialAnimation2PackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            MaterialAnimation2 com = component as MaterialAnimation2;

            has |= __CollectMaterial__(ref com.srcMaterial, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{data, 0});

            return data;
        }

        static void LoadMaterialEnd(Material mat, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            MaterialAnimation2 com = data.mComponent as MaterialAnimation2;
            switch (index)
            {

            case 0:
                com.srcMaterial = mat;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif