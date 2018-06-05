#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Reflection;

namespace PackTool
{
    public class ImagePackData : ComponentData
    {
#if UNITY_EDITOR
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            Image image = component as Image;
            Material mat = image.material;
            has |= __CollectMaterial__(ref mat, writer, mgr);
            image.material = mat;

            Sprite sprite = image.sprite;
            has |= __CollectSprite__(ref sprite, writer, mgr);
            image.sprite = sprite;

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);
            __LoadMaterial__(data, reader, LoadMaterial, data);

            __LoadSprite__(data, reader, LoadSpriteEnd, data);

            return data;
        }

        static FieldInfo s_sprite = typeof(Image).GetField("m_Sprite", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo s_material = typeof(Image).GetField("m_Material", BindingFlags.NonPublic | BindingFlags.Instance);

        static void LoadSpriteEnd(Sprite s, object p)
        {
            Data data = p as Data;
            Image com = data.mComponent as Image;
            s_sprite.SetValue(com, s);

            data.OnEnd();
        }

        public static void LoadMaterial(Material mat, object p)
        {
            Data data = p as Data;
            Image com = data.mComponent as Image;
            s_material.SetValue(com, mat);

            data.OnEnd();
        }
    }
}
#endif