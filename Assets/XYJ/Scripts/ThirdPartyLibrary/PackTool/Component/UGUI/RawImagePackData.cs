#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class RawImagePackData : ComponentData
    {
#if UNITY_EDITOR
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            RawImage rawImage = component as RawImage;
            Texture texture = rawImage.texture;
            has |= __CollectTexture__(ref texture, writer, mgr);
            rawImage.texture = texture;

            Material mat = rawImage.material;
            has |= __CollectMaterial__(ref mat, writer, mgr);
            rawImage.material = mat;

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadTexture__(data, reader, LoadTextureEnd, data);
            __LoadMaterial__(data, reader, LoadMaterialEnd, data);

            return data;
        }

        static void LoadMaterialEnd(Material mat, object p)
        {
            Data data = p as Data;
            RawImage com = data.mComponent as RawImage;
            com.material = mat;

            data.OnEnd();
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            Data data = p as Data;
            RawImage com = data.mComponent as RawImage;
            com.texture = texture;

            data.OnEnd();
        }
    }
}
#endif