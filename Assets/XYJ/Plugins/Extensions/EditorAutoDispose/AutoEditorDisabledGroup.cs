#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{
    public class AutoEditorDisabledGroup : IDisposable
    {
        // Create a group of controls that can be disabled.
        // If disabled is true, the controls inside the group will be disabled. If false, the enabled/disabled state will not be changed.
        public AutoEditorDisabledGroup(bool disabled)
        {
            EditorGUI.BeginDisabledGroup(disabled);
        }

        public void Dispose()
        {
            EditorGUI.EndDisabledGroup();
        }

    }
}
#endif
