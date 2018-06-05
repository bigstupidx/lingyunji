#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ResizablePackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            UIWidgets.Resizable com = component as UIWidgets.Resizable;

            has |= __CollectTexture__(ref com.CursorEWTexture, writer, mgr);

            has |= __CollectTexture__(ref com.CursorNSTexture, writer, mgr);

            has |= __CollectTexture__(ref com.CursorNESWTexture, writer, mgr);

            has |= __CollectTexture__(ref com.CursorNWSETexture, writer, mgr);

            has |= __CollectTexture__(ref com.DefaultCursorTexture, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 0});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 1});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 2});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 3});

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 4});

            return data;
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            UIWidgets.Resizable com = data.mComponent as UIWidgets.Resizable;
            switch (index)
            {

            case 0:
                com.CursorEWTexture = texture as Texture2D;
                break;

            case 1:
                com.CursorNSTexture = texture as Texture2D;
                break;

            case 2:
                com.CursorNESWTexture = texture as Texture2D;
                break;

            case 3:
                com.CursorNWSETexture = texture as Texture2D;
                break;

            case 4:
                com.DefaultCursorTexture = texture as Texture2D;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif