#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{
    // Begin a horizontal group and get its rect back.
    public class AutoEditorHorizontal : IDisposable
    {

        public AutoEditorHorizontal(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
        }

        public AutoEditorHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
        }

        public AutoEditorHorizontal(ref Rect rect, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginHorizontal(options);
        }

        public AutoEditorHorizontal(ref Rect rect, GUIStyle style, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginHorizontal(style, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }

    }
}
#endif