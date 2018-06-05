#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace GUIEditor
{
    public class GUIIndent : IDisposable
    {
        bool isIndentLevel = false;
        static int indentLevel;

        public GUIIndent(bool isIndentLevel)
        {
            this.isIndentLevel = isIndentLevel;
            if (isIndentLevel)
            {
                indentLevel = ++EditorGUI.indentLevel;
            }
        }

        public void Dispose()
        {
            if (isIndentLevel)
            {
                indentLevel = --EditorGUI.indentLevel;
            }
        }
    }
}

#endif