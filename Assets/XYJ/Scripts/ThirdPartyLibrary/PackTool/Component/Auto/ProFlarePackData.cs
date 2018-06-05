#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ProFlarePackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            ProFlare com = component as ProFlare;

            has |= __CollectMono__<ProFlareAtlas>(ref com._Atlas, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadMono__(data, reader, PrefabLoadEnd, new object[]{data, 0});

            return data;
        }

        static void PrefabLoadEnd(GameObject go, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            ProFlare com = data.mComponent as ProFlare;
            switch (index)
            {

            case 0:
                com._Atlas = go.GetComponent<ProFlareAtlas>();
                break;

            }
            data.OnEnd();
        }

    }
}
#endif