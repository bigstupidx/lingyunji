#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{
    // The indent level of the field labels.
    public class AutoEditorIndentLevel : IDisposable
    {
        int preValue = 0;

        public AutoEditorIndentLevel(int addLevel=1)
        {
            preValue = EditorGUI.indentLevel;
            EditorGUI.indentLevel = preValue + addLevel;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = preValue;
        }
    }
}
#endif