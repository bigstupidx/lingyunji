#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{
    public class AutoEditorFadeGroup : IDisposable
    {
        private bool m_withIndent = false;
        /// <summary>
        /// Begins a group that can be be hidden/shown and the transition will be animated.
        /// </summary>
        /// <param name="value">A value between 0 and 1, 0 being hidden, and 1 being fully visible.</param>
        /// <param name="withIndent">Default is without indent.</param>
        public AutoEditorFadeGroup(float value, bool withIndent=false)
        {
            EditorGUILayout.BeginFadeGroup(value);
            m_withIndent = withIndent;
            if (m_withIndent)
                EditorGUI.indentLevel++;
        }

        public void Dispose()
        {
            EditorGUILayout.EndFadeGroup();
            if (m_withIndent)
                EditorGUI.indentLevel--;
        }
    }
}
#endif