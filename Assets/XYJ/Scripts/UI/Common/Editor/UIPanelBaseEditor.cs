using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

namespace xys.UI
{
    [CustomEditor(typeof(UIPanelBase), true)]
    public class UIPanelBaseEditor : UnityEditor.Editor
    {
        static List<Text> s_texts = new List<Text>();
        FontConfig select_font;
        bool isOpenFont = false;

        public struct FontConfig
        {
            public Font font;

            public FontConfig(Font f)
            {
                font = f;
            }

            public override int GetHashCode()
            {
                return font == null ? 0 : font.GetInstanceID();
            }

            public override bool Equals(object obj)
            {
                return font == ((FontConfig)obj).font;
            }
        }

        Font default_font;

        //[MenuItem("Assets/UI/遍历面板", false, 0)]
        //static public void ForEachPanel()
        //{
        //    foreach (Object o in Selection.objects)
        //    {
        //        XTools.Utility.ForEach(AssetDatabase.GetAssetPath(o),
        //            (AssetImporter assetImporter) =>
        //            {
        //                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetImporter.assetPath);
        //                UIPanelBase pb = go.GetComponent<UIPanelBase>();
        //                if (pb == null || (!pb.isShowGravity) || (pb.isShowGravity && pb.isShowPanelHead))
        //                    return;

        //                pb.isShowPanelHead = true;
        //                EditorUtility.SetDirty(pb);
        //                AssetDatabase.ImportAsset(assetImporter.assetPath);
        //            }, 
        //            (string assetPath, string root) => { return assetPath.EndsWith(".prefab"); });
        //    }
        //}

        static List<string> PanelHead_StateNames;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UIPanelBase panel = target as UIPanelBase;
            panel.isShowGravity = EditorGUILayout.Toggle(new GUIContent("重力背景", ""), panel.isShowGravity);
            panel.isShowPanelHead = EditorGUILayout.Toggle(new GUIContent("面板头", ""), panel.isShowPanelHead);
            if (panel.isShowPanelHead)
            {
                ++EditorGUI.indentLevel;
                panel.panelName = EditorGUILayout.TextField(new GUIContent("面板名称", ""), panel.panelName);

                if (PanelHead_StateNames == null)
                {
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/UIData/Data/Prefabs/Template/ResourcesExport/UIRoot.prefab");
                    PanelHead_StateNames = new List<string>(go.transform.Find("Default/PanelHead").GetComponent<UI.State.StateRoot>().StateNames);
                }

                panel.panelHeadState = GUIEditor.GuiTools.StringPopup("面板头状态", panel.panelHeadState, PanelHead_StateNames);

                --EditorGUI.indentLevel;
            }

            panel.startBlurExcep = EditorGUILayout.Toggle(new GUIContent("模糊效果", "此界面显示时，场景以及其他界面模糊处理!"), panel.startBlurExcep);
            panel.isExclusive = EditorGUILayout.Toggle(new GUIContent("独占模式", "此面板显示时，当前打开的界面临时隐藏，此面板隐藏时，临时隐藏的界面恢复显示!"), panel.isExclusive);
            panel.isRecordingPanel = EditorGUILayout.Toggle(new GUIContent("显示链", "勾选此项，在面板显示时，会有类似A->B->C的显示链，假如C隐藏了，会显示B，B隐藏了会显示A!"), panel.isRecordingPanel);
            panel.isHideMainPanel = EditorGUILayout.Toggle(new GUIContent("隐藏主界面", "此界面显示时，主界面播放隐藏，此面板隐藏时，主界面恢复显示!"), panel.isHideMainPanel);

            panel.isClickSpaceClose = EditorGUILayout.Toggle("点击空白隐藏", panel.isClickSpaceClose);

            if (Application.isPlaying)
            {
                panel.isBlurExcep = EditorGUILayout.Toggle("isBlurExcep", panel.isBlurExcep);
                panel.temporaryHide = EditorGUILayout.Toggle("临时隐藏", panel.temporaryHide);

                panel.animScaler = EditorGUILayout.FloatField("动画缩放比", panel.animScaler);
            }

            isOpenFont = EditorGUILayout.Toggle("字库使用情况", isOpenFont);
            if (isOpenFont)
            {
                EditorGUI.indentLevel++;

                if (default_font == null)
                    default_font = Resources.Load<Font>("msyh");

                default_font = EditorGUILayout.ObjectField("默认字库", default_font, typeof(Font), false) as Font;

                s_texts.Clear();
                panel.GetComponentsInChildren<Text>(s_texts);

                Dictionary<FontConfig, List<Text>> font_to_texts = new Dictionary<FontConfig, List<Text>>();
                foreach (Text t in s_texts)
                {
                    Font f = t.font;
                    FontConfig fc = new FontConfig(f);
                    List<Text> texts = null;
                    if (!font_to_texts.TryGetValue(fc, out texts))
                    {
                        texts = new List<Text>();
                        font_to_texts.Add(fc, texts);
                    }

                    texts.Add(t);
                }

                bool isreplace = false;
                select_font = GUIEditor.GuiTools.MapStringPopup(
                    "字体",
                    select_font,
                    font_to_texts,
                    (List<KeyValuePair<FontConfig, List<Text>>> itor) =>
                    {

                    },
                    (KeyValuePair<FontConfig, List<Text>> itor) =>
                    {
                        if (null == itor.Key.font)
                            return "无字体 " + itor.Value.Count;

                        Font font = itor.Key.font;
                        return font.name;
                    },
                    () => 
                    {
                        EditorGUILayout.BeginHorizontal();
                    },
                    (FontConfig fc) => 
                    {
                        if (fc.font != default_font)
                        {
                            if (GUILayout.Button("设置为默认字体"))
                                isreplace = true;
                        }

                        EditorGUILayout.EndHorizontal();
                    },
                    (KeyValuePair<FontConfig, List<Text>> itor) =>
                    {
                        EditorGUI.indentLevel++;
                        // 选中的
                        List<Text> value = itor.Value;

                        if (isreplace)
                        {
                            for (int i = 0; i < value.Count; ++i)
                            {
                                value[i].font = default_font;
                            }

                            EditorUtility.SetDirty(target);
                        }

                        for (int i = 0; i < value.Count; ++i)
                        {
                            EditorGUILayout.ObjectField(string.Format("{0}):", i), value[i], typeof(Text), true);
                        }
                        EditorGUI.indentLevel--;
                    });

                EditorGUI.indentLevel--;
            }
        }
    }
}