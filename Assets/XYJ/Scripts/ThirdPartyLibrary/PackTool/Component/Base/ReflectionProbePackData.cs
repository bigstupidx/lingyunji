#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ReflectionProbePackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            // 网格渲染器，需要保存下材质数据
            bool has = false;

            ReflectionProbe com = component as ReflectionProbe;

            Texture bakedTexture = com.bakedTexture;
            Texture customBakedTexture = com.customBakedTexture;

            has |= __CollectTexture__(ref bakedTexture, writer, mgr);
            has |= __CollectTexture__(ref customBakedTexture, writer, mgr);

            com.bakedTexture = bakedTexture;
            com.customBakedTexture = customBakedTexture;

            return has;
        }
#endif

        // 动态时资源加载
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadTexture__(data, reader, LoadTextureEnd, new object[] { data, 0 });

            __LoadTexture__(data, reader, LoadTextureEnd, new object[] { data, 1 });

            return data;
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            ReflectionProbe com = data.mComponent as ReflectionProbe;
            switch (index)
            {

            case 0:
                com.bakedTexture = texture;
                break;

            case 1:
                com.customBakedTexture = texture;
                break;

            }
            data.OnEnd();
        }
    }
}
#endif