#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class BlurPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            UnityStandardAssets.ImageEffects.Blur com = component as UnityStandardAssets.ImageEffects.Blur;

            bool has = false;
            has |= __CollectShader__(ref com.blurShader, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 0});

            return data;
        }

        static void LoadShaderEnd(Shader shader, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            UnityStandardAssets.ImageEffects.Blur com = data.mComponent as UnityStandardAssets.ImageEffects.Blur;
            switch (index)
            {

            case 0:
                com.blurShader = shader;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif