#if USE_UATLAS
using UnityEditor;
using UnityEngine;

namespace UI
{
    [CustomEditor(typeof(uAtlas))]
    public class uAtlasEditor : Editor
    {
        void OnEnable()
        {
            mPage = new EditorPageBtn();
            mParamList.ReleaseAll();
        }

        EditorPageBtn mPage;
        ParamList mParamList = new ParamList();

        static public bool DrawHeader(string text, bool state)
        {
            if (!state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;

            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f)))
                state = !state;

            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            return state;
        }

        static void ASGUI(ref uAtlas.AS asd, ParamList paramList)
        {
            bool isF = DrawHeader(asd.ToString(), paramList.Get("isF", false));
            paramList.Set("isF", isF);

            if (isF)
            {
                EditorGUILayout.ObjectField("纹理", asd.texture, typeof(Texture2D), false);

                EditorPageBtn epb = paramList.Get("epb", () => { return new EditorPageBtn(); });
                epb.total = asd.sprites.Length;
                epb.pageNum = 30;
                EditorGUI.indentLevel++;
                epb.OnRender();

                uAtlas.Data[] uads = asd.sprites;
                epb.ForEach((int i) =>
                {
                    EditorGUILayout.BeginHorizontal();
                    UIEditorTools.SetLabelWidth(60);
                    EditorGUILayout.TextField("name", uads[i].name, GUILayout.Width(200));
                    EditorGUILayout.IntField("width", uads[i].width, GUILayout.Width(200));
                    EditorGUILayout.IntField("height", uads[i].height, GUILayout.Width(200));
                    if (GUILayout.Button("Select"))
                    {

                    }
                    EditorGUILayout.EndHorizontal();
                });
            }
        }
        public override bool HasPreviewGUI() { return true; }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {

        }

        class AtlasInfo
        {
            public int totalMemory = 0;
            public int totalSize = 0;
            public int totalSprites = 0;
            public int totalused = 0;

            public void Update(uAtlas.AS[] ass)
            {
                totalMemory = 0;
                totalSize = 0;
                totalSprites = 0;
                totalused = 0;

                for (int i = 0; i < ass.Length; ++i)
                {
                    var uas = ass[i];
                    totalMemory += uas.TotalMemory;
                    totalSize += uas.texture.width * uas.texture.height;
                    totalSprites += uas.sprites.Length;

                    foreach (var d in uas.sprites)
                        totalused += d.width * d.height;
                }
            }

            public override string ToString()
            {
                return string.Format("size:{0} total:{1} memory:{2} used:{3}%", GUIEditor.GuiTools.TextureSize(totalSize), totalSprites, XTools.Utility.ToMb(totalMemory), (100.0f * totalused / totalSize).ToString("0.00"));
            }
        }

        AtlasInfo m_Info = null;

        public override void OnInspectorGUI()
        {
            uAtlas atlas = target as uAtlas;
            uAtlas.AS[] ass = atlas.Atlas;
            if (m_Info == null)
            {
                m_Info = new AtlasInfo();
                m_Info.Update(ass);
            }

            EditorGUILayout.LabelField(m_Info.ToString());

            mPage.total = ass.Length;
            mPage.pageNum = 10;
            mPage.OnRender();

            mPage.ForEach((int i) => 
            {
                ASGUI(ref ass[i], mParamList.Get(i.ToString(), ()=> { return new ParamList(); }));
            });
        }
    }
}
#endif