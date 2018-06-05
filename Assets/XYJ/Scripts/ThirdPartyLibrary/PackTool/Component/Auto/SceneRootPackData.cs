#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class SceneRootPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            PackTool.SceneRoot com = component as PackTool.SceneRoot;

            has |= __CollectMaterial__(ref com.skybox, writer, mgr);

            has |= __CollectTexture__(ref com.customReflection, writer, mgr);

            has |= __CollectLightProbes__(ref com.lightProbes, writer, mgr);

            has |= __CollectList__<Texture2D>(com.lightmapLights, writer, mgr, __CollectTexture__<Texture2D>);

            has |= __CollectList__<Texture2D>(com.lightmapDirs, writer, mgr, __CollectTexture__<Texture2D>);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{data, 0});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 1});

            __LoadLightProbes__(data, reader, LoadLightProbesEnd, new object[]{data, 2});

            __LoadAssetList__<Texture>(3, data, reader, __LoadTexture__, LoadTextureEndList);

            __LoadAssetList__<Texture>(4, data, reader, __LoadTexture__, LoadTextureEndList);

            return data;
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            PackTool.SceneRoot com = data.mComponent as PackTool.SceneRoot;
            switch (index)
            {

            case 1:
                com.customReflection = texture as Cubemap;
                break;

            }
            data.OnEnd();
        }

        static void LoadMaterialEnd(Material mat, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            PackTool.SceneRoot com = data.mComponent as PackTool.SceneRoot;
            switch (index)
            {

            case 0:
                com.skybox = mat;
                break;

            }
            data.OnEnd();
        }

        static void LoadTextureEndList(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            int type = (int)pp[2];
            SceneRoot com = data.mComponent as SceneRoot;
            
            switch(type)
            {

            case 3:
                com.lightmapLights[index] = texture as Texture2D;
                break;

            case 4:
                com.lightmapDirs[index] = texture as Texture2D;
                break;

            }

            data.OnEnd();
        }

        static void LoadLightProbesEnd(LightProbes lightProbes, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            PackTool.SceneRoot com = data.mComponent as PackTool.SceneRoot;
            switch (index)
            {

            case 2:
                com.lightProbes = lightProbes;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif