#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{
    /// <summary>
    /// Begin an automatically layouted scrollview.
    /// These work just like GUILayout.BeginScrollView but feel more application-like and should be used in the editor
    /// </summary>
    public class AutoEditorScrollView : IDisposable
    {

        public AutoEditorScrollView(ref Vector2 scrollPosition, params GUILayoutOption[] options)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
        }

        public AutoEditorScrollView(ref Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, style, options);
        }

        public AutoEditorScrollView(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options);
        }

        public AutoEditorScrollView(ref Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, horizontalScrollbar, verticalScrollbar, options);
        }

        public AutoEditorScrollView(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }

    }
}
#endif