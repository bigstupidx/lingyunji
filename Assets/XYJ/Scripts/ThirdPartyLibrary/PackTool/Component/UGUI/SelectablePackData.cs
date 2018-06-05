#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PackTool
{
    public class SelectablePackData : ComponentData
    {
#if UNITY_EDITOR
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            Selectable selectable = component as Selectable;

            SpriteState ss = selectable.spriteState;
            Sprite disabledSprite = ss.disabledSprite;
            Sprite highlightedSprite = ss.highlightedSprite;
            Sprite pressedSprite = ss.pressedSprite;

            has |= __CollectSprite__(ref disabledSprite, writer, mgr);
            has |= __CollectSprite__(ref highlightedSprite, writer, mgr);
            has |= __CollectSprite__(ref pressedSprite, writer, mgr);

            ss.disabledSprite = disabledSprite;
            ss.highlightedSprite = highlightedSprite;
            ss.pressedSprite = pressedSprite;

            spriteStateRefValue.SetValue(selectable, ss);

            return has;
        }
#endif
        static FieldInfo spriteStateRefValue_ = null;

        static FieldInfo spriteStateRefValue
        {
            get
            {
                if (spriteStateRefValue_ == null)
                {
                    spriteStateRefValue_ = typeof(Selectable).GetField("m_SpriteState", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding);
                }

                return spriteStateRefValue_;
            }
        }

        // ��̬ʱ��Դ����
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            //Selectable selectable = pd.component as Selectable;
            //SpriteState ss = selectable.spriteState;

            __LoadSprite__(data, reader, LoadSpriteEnd, new object[] { 0, data });
            __LoadSprite__(data, reader, LoadSpriteEnd, new object[] { 1, data });
            __LoadSprite__(data, reader, LoadSpriteEnd, new object[] { 2, data });

            return data;
        }

        static void LoadSpriteEnd(Sprite s, object p)
        {
            object[] pp = p as object[];
            int index = (int)pp[0];
            Data data = pp[1] as Data;
            Selectable com = data.mComponent as Selectable;
            SpriteState ss = com.spriteState;
            switch (index)
            {

            case 0:
                ss.disabledSprite = s;
                break;

            case 1:
                ss.highlightedSprite = s;
                break;

            case 2:
                ss.pressedSprite = s;
                break;
            }

            com.spriteState = ss;

            data.OnEnd();
        }
    }
}
#endif