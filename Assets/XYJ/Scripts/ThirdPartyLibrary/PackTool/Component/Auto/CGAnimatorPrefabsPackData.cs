#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class CGAnimatorPrefabsPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            CGAnimatorPrefabs com = component as CGAnimatorPrefabs;

            has |= __CollectPrefab__(ref com.m_prefabs, writer, mgr);

            has |= __CollectAnimatorController__(ref com.m_controller, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadPrefab__(data, reader, PrefabLoadEnd, new object[]{data, 0});

            __LoadAnimatorController__(data, reader, LoadRunAnimConEnd, new object[]{data, 1});

            return data;
        }

        static void PrefabLoadEnd(GameObject go, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            CGAnimatorPrefabs com = data.mComponent as CGAnimatorPrefabs;
            switch (index)
            {

            case 0:
                com.m_prefabs = go;
                break;

            }
            data.OnEnd();
        }

        static void LoadRunAnimConEnd(RuntimeAnimatorController rac, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            CGAnimatorPrefabs com = data.mComponent as CGAnimatorPrefabs;
            switch (index)
            {

            case 1:
                com.m_controller = rac;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif