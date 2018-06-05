#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class LineImagePackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            UI.LineImage com = component as UI.LineImage;

            has |= __CollectSprite__(ref com.m_Sprite, writer, mgr);

            has |= __CollectSprite__(ref com.m_LSprite, writer, mgr);

            has |= __CollectSprite__(ref com.m_TSprite, writer, mgr);

            has |= __CollectSprite__(ref com.m_XSprite, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadSprite__(data, reader, LoadSpriteEnd, new object[]{data, 0});

            __LoadSprite__(data, reader, LoadSpriteEnd, new object[]{data, 1});

            __LoadSprite__(data, reader, LoadSpriteEnd, new object[]{data, 2});

            __LoadSprite__(data, reader, LoadSpriteEnd, new object[]{data, 3});

            return data;
        }

        static void LoadSpriteEnd(Sprite sprite, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            UI.LineImage com = data.mComponent as UI.LineImage;
            switch (index)
            {

            case 0:
                com.m_Sprite = sprite;
                break;

            case 1:
                com.m_LSprite = sprite;
                break;

            case 2:
                com.m_TSprite = sprite;
                break;

            case 3:
                com.m_XSprite = sprite;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif