using UnityEngine;
using UnityEditor;

namespace UI
{
    [CustomEditor(typeof(TweenColor))]
    public class TweenColorEditor : UITweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(120f);

            TweenColor tw = target as TweenColor;
            GUI.changed = false;

            Color from = EditorGUILayout.ColorField("From", tw.from);
            Color to = EditorGUILayout.ColorField("To", tw.to);

            if (GUI.changed)
            {
                UIEditorTools.RegisterUndo("Tween Change", tw);
                tw.from = from;
                tw.to = to;
                EditorUtility.SetDirty(tw);
            }

            DrawCommonProperties();
        }
    }
}