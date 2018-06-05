#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class USCreatePrefabsPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            WellFired.USCreatePrefabs com = component as WellFired.USCreatePrefabs;

            has |= __CollectPrefab__(ref com.spawnPrefab, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadPrefab__(data, reader, PrefabLoadEnd, new object[]{data, 0});

            return data;
        }

        static void PrefabLoadEnd(GameObject go, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            WellFired.USCreatePrefabs com = data.mComponent as WellFired.USCreatePrefabs;
            switch (index)
            {

            case 0:
                com.spawnPrefab = go;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif