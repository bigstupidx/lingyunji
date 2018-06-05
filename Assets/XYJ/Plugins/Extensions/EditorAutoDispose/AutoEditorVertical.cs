#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{
    // Begin a vertical group and get its rect back.
    public class AutoEditorVertical : IDisposable
    {

        public AutoEditorVertical(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
        }

        public AutoEditorVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
        }

        public AutoEditorVertical(ref Rect rect, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginVertical(options);
        }

        public AutoEditorVertical(ref Rect rect, GUIStyle style, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginVertical(style, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }

    }
}
#endif