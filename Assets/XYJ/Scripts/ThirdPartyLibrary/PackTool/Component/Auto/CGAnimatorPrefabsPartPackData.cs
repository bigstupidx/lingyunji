#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class CGAnimatorPrefabsPartPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            CGAnimatorPrefabsPart com = component as CGAnimatorPrefabsPart;

            has |= __CollectPrefab__(ref com.m_player, writer, mgr);

            has |= __CollectList__<RuntimeAnimatorController>(com.m_controllers, writer, mgr, __CollectAnimatorController__);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadPrefab__(data, reader, PrefabLoadEnd, new object[]{data, 0});

            __LoadAssetList__<RuntimeAnimatorController>(1, data, reader, __LoadAnimatorController__, OnRunAniConLoadEnd);

            return data;
        }

        static void PrefabLoadEnd(GameObject go, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            CGAnimatorPrefabsPart com = data.mComponent as CGAnimatorPrefabsPart;
            switch (index)
            {

            case 0:
                com.m_player = go;
                break;

            }
            data.OnEnd();
        }

        static void OnRunAniConLoadEnd(RuntimeAnimatorController rac, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            int type = (int)pp[2];
            CGAnimatorPrefabsPart com = data.mComponent as CGAnimatorPrefabsPart;
            
            switch(type)
            {

            case 1:
                com.m_controllers[index] = rac;
                break;

            }

            data.OnEnd();
        }

    }
}
#endif