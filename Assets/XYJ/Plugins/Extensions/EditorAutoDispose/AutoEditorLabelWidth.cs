#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{
    public class AutoEditorLabelWidth : IDisposable
    {
        float preWidth = 0.0f;

        public AutoEditorLabelWidth(float labelWidth)
        {
            preWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        public void Dispose()
        {
            EditorGUIUtility.labelWidth = preWidth;
        }
    }
}
#endif