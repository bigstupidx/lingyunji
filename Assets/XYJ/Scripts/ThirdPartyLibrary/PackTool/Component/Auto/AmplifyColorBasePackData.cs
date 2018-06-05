#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class AmplifyColorBasePackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            AmplifyColorBase com = component as AmplifyColorBase;

            has |= __CollectTexture__(ref com.LutTexture, writer, mgr);

            has |= __CollectTexture__(ref com.LutBlendTexture, writer, mgr);

            has |= __CollectTexture__(ref com.MaskTexture, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 0});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 1});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 2});

            return data;
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            AmplifyColorBase com = data.mComponent as AmplifyColorBase;
            switch (index)
            {

            case 0:
                com.LutTexture = texture;
                break;

            case 1:
                com.LutBlendTexture = texture;
                break;

            case 2:
                com.MaskTexture = texture;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif