#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ModelPartSetting1PackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            ModelPartSetting1 com = component as ModelPartSetting1;

            has |= __CollectList__<RuntimeAnimatorController>(com.m_controllers, writer, mgr, __CollectAnimatorController__);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadAssetList__<RuntimeAnimatorController>(0, data, reader, __LoadAnimatorController__, OnRunAniConLoadEnd);

            return data;
        }

        static void OnRunAniConLoadEnd(RuntimeAnimatorController rac, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            int type = (int)pp[2];
            ModelPartSetting1 com = data.mComponent as ModelPartSetting1;
            
            switch(type)
            {

            case 0:
                com.m_controllers[index] = rac;
                break;

            }

            data.OnEnd();
        }

    }
}
#endif