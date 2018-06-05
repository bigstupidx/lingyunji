#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class T4MObjSCPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            T4MObjSC com = component as T4MObjSC;

            has |= __CollectMaterial__(ref com.T4MMaterial, writer, mgr);

            has |= __CollectTexture__(ref com.T4MMaskTex2d, writer, mgr);

            has |= __CollectTexture__(ref com.T4MMaskTexd, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{data, 0});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 1});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 2});

            return data;
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            T4MObjSC com = data.mComponent as T4MObjSC;
            switch (index)
            {

            case 1:
                com.T4MMaskTex2d = texture as Texture2D;
                break;

            case 2:
                com.T4MMaskTexd = texture as Texture2D;
                break;

            }
            data.OnEnd();
        }

        static void LoadMaterialEnd(Material mat, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            T4MObjSC com = data.mComponent as T4MObjSC;
            switch (index)
            {

            case 0:
                com.T4MMaterial = mat;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif