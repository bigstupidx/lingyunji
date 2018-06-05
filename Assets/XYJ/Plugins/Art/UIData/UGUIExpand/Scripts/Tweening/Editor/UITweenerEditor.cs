using UnityEngine;
using UnityEditor;

namespace UI
{
    [CustomEditor(typeof(UITweener), true)]
    public class UITweenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(110f);
            base.OnInspectorGUI();
            DrawCommonProperties();
        }

        SerializedProperty onFinishedProperty;

        void OnEnable()
        {
            onFinishedProperty = serializedObject.FindProperty("onFinished");
        }

        protected void DrawCommonProperties()
        {
            UITweener tw = target as UITweener;

            if (UIEditorTools.DrawHeader("Tweener"))
            {
                UIEditorTools.BeginContents();
                UIEditorTools.SetLabelWidth(110f);

                GUI.changed = false;

                UITweener.Style style = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", tw.style);
                AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.animationCurve, GUILayout.Width(170f), GUILayout.Height(62f));
                //UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);

                GUILayout.BeginHorizontal();
                float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(170f));
                GUILayout.Label("seconds");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
                GUILayout.Label("seconds");
                GUILayout.EndHorizontal();

                int tg = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(170f));
                bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);

                if (GUI.changed)
                {
                    UIEditorTools.RegisterUndo("Tween Change", tw);
                    tw.animationCurve = curve;
                    //tw.method = method;
                    tw.style = style;
                    tw.ignoreTimeScale = ts;
                    tw.tweenGroup = tg;
                    tw.duration = dur;
                    tw.delay = del;
                    EditorUtility.SetDirty(tw);
                }
                UIEditorTools.EndContents();
            }

            UIEditorTools.SetLabelWidth(80f);

            EditorGUILayout.PropertyField(onFinishedProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}