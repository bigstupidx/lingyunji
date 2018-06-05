using UnityEngine;
using UnityEditor;

namespace UI
{
    [CustomEditor(typeof(TweenWidth))]
    public class TweenWidthEditor : UITweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            UIEditorTools.SetLabelWidth(120f);

            TweenWidth tw = target as TweenWidth;
            GUI.changed = false;

            int from = EditorGUILayout.IntField("From", tw.from);
            int to = EditorGUILayout.IntField("To", tw.to);

            if (from < 0) from = 0;
            if (to < 0) to = 0;

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