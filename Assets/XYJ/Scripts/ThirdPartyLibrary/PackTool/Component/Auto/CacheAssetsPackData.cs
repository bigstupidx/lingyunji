#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class CacheAssetsPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            CacheAssets com = component as CacheAssets;

            has |= __CollectList__<Texture>(com.textures, writer, mgr, __CollectTexture__<Texture>);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadAssetList__<Texture>(0, data, reader, __LoadTexture__, LoadTextureEndList);

            return data;
        }

        static void LoadTextureEndList(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            int type = (int)pp[2];
            CacheAssets com = data.mComponent as CacheAssets;
            
            switch(type)
            {

            case 0:
                com.textures[index] = texture;
                break;

            }

            data.OnEnd();
        }

    }
}
#endif