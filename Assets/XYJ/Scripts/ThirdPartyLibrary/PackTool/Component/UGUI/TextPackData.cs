#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Reflection;

namespace PackTool
{
    public class TextPackData : ComponentData
    {
#if UNITY_EDITOR
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            Text text = component as Text;
            Font font = text.font;
            has |= __CollectFont__(ref font, writer, mgr);

            Material mat = text.material;
            has |= __CollectMaterial__(ref mat, writer, mgr);

            text.font = font;
            text.material = mat;

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadFontlib__(data, reader, LoadFontEnd, data);
            __LoadMaterial__(data, reader, LoadMatEnd, data);

            return data;
        }

        static FieldInfo s_font_data = typeof(Text).GetField("m_FontData", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void LoadMatEnd(Material mat, object p)
        {
            Data data = p as Data;
            Text com = data.mComponent as Text;
            com.material = mat;
            FontUpdateTracker.UntrackText(com);

            data.OnEnd();
        }

        public static void LoadFontEnd(Font font, object p)
        {
            Data data = p as Data;
            Text com = data.mComponent as Text;
            FontData fd = s_font_data.GetValue(com) as FontData;
            fd.font = font;
            FontUpdateTracker.UntrackText(com);

            data.OnEnd();
        }
    }
}
#endif