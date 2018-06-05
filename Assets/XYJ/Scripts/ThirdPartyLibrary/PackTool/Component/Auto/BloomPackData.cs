#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class BloomPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            UnityStandardAssets.ImageEffects.Bloom com = component as UnityStandardAssets.ImageEffects.Bloom;

            has |= __CollectTexture__(ref com.lensFlareVignetteMask, writer, mgr);

            has |= __CollectShader__(ref com.lensFlareShader, writer, mgr);

            has |= __CollectShader__(ref com.screenBlendShader, writer, mgr);

            has |= __CollectShader__(ref com.blurAndFlaresShader, writer, mgr);

            has |= __CollectShader__(ref com.brightPassFilterShader, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 0});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 1});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 2});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 3});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 4});

            return data;
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            UnityStandardAssets.ImageEffects.Bloom com = data.mComponent as UnityStandardAssets.ImageEffects.Bloom;
            switch (index)
            {

            case 0:
                com.lensFlareVignetteMask = texture as Texture2D;
                break;

            }
            data.OnEnd();
        }

        static void LoadShaderEnd(Shader shader, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            UnityStandardAssets.ImageEffects.Bloom com = data.mComponent as UnityStandardAssets.ImageEffects.Bloom;
            switch (index)
            {

            case 1:
                com.lensFlareShader = shader;
                break;

            case 2:
                com.screenBlendShader = shader;
                break;

            case 3:
                com.blurAndFlaresShader = shader;
                break;

            case 4:
                com.brightPassFilterShader = shader;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif