#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ProFlareAtlasPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            ProFlareAtlas com = component as ProFlareAtlas;

            has |= __CollectTexture__(ref com.texture, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 0});

            return data;
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            ProFlareAtlas com = data.mComponent as ProFlareAtlas;
            switch (index)
            {

            case 0:
                com.texture = texture as Texture2D;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif