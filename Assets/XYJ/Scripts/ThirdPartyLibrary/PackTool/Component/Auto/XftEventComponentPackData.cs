#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class XftEventComponentPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            Xft.XftEventComponent com = component as Xft.XftEventComponent;

            has |= __CollectShader__(ref com.RadialBlurShader, writer, mgr);

            has |= __CollectShader__(ref com.RadialBlurTexAddShader, writer, mgr);

            has |= __CollectTexture__(ref com.RadialBlurMask, writer, mgr);

            has |= __CollectShader__(ref com.GlowCompositeShader, writer, mgr);

            has |= __CollectShader__(ref com.GlowBlurShader, writer, mgr);

            has |= __CollectShader__(ref com.GlowDownSampleShader, writer, mgr);

            has |= __CollectShader__(ref com.GlowPerObjReplacementShader, writer, mgr);

            has |= __CollectShader__(ref com.GlowPerObjBlendShader, writer, mgr);

            has |= __CollectShader__(ref com.ColorInverseShader, writer, mgr);

            has |= __CollectShader__(ref com.GlitchShader, writer, mgr);

            has |= __CollectTexture__(ref com.GlitchMask, writer, mgr);

            has |= __CollectShader__(ref com.BlurryDistortionShader, writer, mgr);

            has |= __CollectTexture__(ref com.BDDistortTexture, writer, mgr);

            has |= __CollectAudioClip__(ref com.Clip, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 0});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 1});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 2});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 3});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 4});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 5});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 6});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 7});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 8});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 9});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 10});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 11});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 12});

            __LoadAudioClip__(data, reader, LoadAudioClipEnd, new object[]{data, 13});

            return data;
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            Xft.XftEventComponent com = data.mComponent as Xft.XftEventComponent;
            switch (index)
            {

            case 2:
                com.RadialBlurMask = texture as Texture2D;
                break;

            case 10:
                com.GlitchMask = texture as Texture2D;
                break;

            case 12:
                com.BDDistortTexture = texture as Texture2D;
                break;

            }
            data.OnEnd();
        }

        static void LoadShaderEnd(Shader shader, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            Xft.XftEventComponent com = data.mComponent as Xft.XftEventComponent;
            switch (index)
            {

            case 0:
                com.RadialBlurShader = shader;
                break;

            case 1:
                com.RadialBlurTexAddShader = shader;
                break;

            case 3:
                com.GlowCompositeShader = shader;
                break;

            case 4:
                com.GlowBlurShader = shader;
                break;

            case 5:
                com.GlowDownSampleShader = shader;
                break;

            case 6:
                com.GlowPerObjReplacementShader = shader;
                break;

            case 7:
                com.GlowPerObjBlendShader = shader;
                break;

            case 8:
                com.ColorInverseShader = shader;
                break;

            case 9:
                com.GlitchShader = shader;
                break;

            case 11:
                com.BlurryDistortionShader = shader;
                break;

            }
            data.OnEnd();
        }

        static void LoadAudioClipEnd(AudioClip clip, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            Xft.XftEventComponent com = data.mComponent as Xft.XftEventComponent;
            switch (index)
            {

            case 13:
                com.Clip = clip;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif