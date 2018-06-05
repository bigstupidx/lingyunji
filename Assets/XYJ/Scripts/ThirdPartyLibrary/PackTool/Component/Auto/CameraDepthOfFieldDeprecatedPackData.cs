#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class CameraDepthOfFieldDeprecatedPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            UnityStandardAssets.ImageEffects.CameraDepthOfFieldDeprecated com = component as UnityStandardAssets.ImageEffects.CameraDepthOfFieldDeprecated;

            has |= __CollectShader__(ref com.dofBlurShader, writer, mgr);

            has |= __CollectShader__(ref com.dofShader, writer, mgr);

            has |= __CollectShader__(ref com.bokehShader, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 0});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 1});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 2});

            return data;
        }

        static void LoadShaderEnd(Shader shader, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            UnityStandardAssets.ImageEffects.CameraDepthOfFieldDeprecated com = data.mComponent as UnityStandardAssets.ImageEffects.CameraDepthOfFieldDeprecated;
            switch (index)
            {

            case 0:
                com.dofBlurShader = shader;
                break;

            case 1:
                com.dofShader = shader;
                break;

            case 2:
                com.bokehShader = shader;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif