#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PackTool
{
    public class TMTextPackData : ComponentData
    {
        static FieldInfo m_fontAsset_ref;

#if UNITY_EDITOR
        static FieldInfo m_fontMaterial_ref;
#endif

        static void Init()
        {
            if (m_fontAsset_ref == null)
                m_fontAsset_ref = typeof(TMPro.TMP_Text).GetField("m_fontAsset", BindingFlags.Instance | BindingFlags.NonPublic);

#if UNITY_EDITOR
            if (m_fontMaterial_ref == null)
                m_fontMaterial_ref = typeof(TMPro.TMP_Text).GetField("m_fontMaterial", BindingFlags.Instance | BindingFlags.NonPublic);
#endif
        }

#if UNITY_EDITOR
        private static TMPro.TMP_FontAsset s_empty;
        public static TMPro.TMP_FontAsset empty
        {
            get
            {
                if (s_empty == null)
                {
                    s_empty = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/Empty");
                }

                return s_empty;
            }
        }

        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            // 网格渲染器，需要保存下材质数据
            TMPro.TMP_Text text = component as TMPro.TMP_Text;
            TMPro.TMP_FontAsset font = text.font;
            __CollectTMPFont__(ref font, writer, mgr);

            Init();

            m_fontAsset_ref.SetValue(text, null);
            m_fontMaterial_ref.SetValue(text, null);

            text.font = empty;

            return true;
        }
#endif
        // 动态时资源加载
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadTMPFont__(data, reader, LoadTMPFont, data);

            return data;
        }

        public static void LoadTMPFont(TMPro.TMP_FontAsset font, object p)
        {
            Data data = p as Data;
            TMPro.TMP_Text text = data.mComponent as TMPro.TMP_Text;

            Init();
            m_fontAsset_ref.SetValue(text, font);

            data.OnEnd();
        }
    }
}
#endif