#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{
    // Begin a vertical group with a toggle to enable or disable all the controls within at once.
    public class AutoEditorToggleGroup : IDisposable
    {
        
        public AutoEditorToggleGroup(GUIContent label, ref bool toggle)
        {
            toggle = EditorGUILayout.BeginToggleGroup(label, toggle);
        }

        public AutoEditorToggleGroup(string label, ref bool toggle)
        {
            toggle = EditorGUILayout.BeginToggleGroup(label, toggle);
        }

        public void Dispose()
        {
            EditorGUILayout.EndToggleGroup();
        }

    }
}
#endif