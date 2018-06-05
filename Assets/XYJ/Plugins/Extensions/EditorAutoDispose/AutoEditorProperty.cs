#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{
    // Create a Property wrapper, useful for making regular GUI controls work with SerializedProperty.
    public class AutoEditorProperty : IDisposable
    {

        public AutoEditorProperty(ref GUIContent label, Rect totalPosition, SerializedProperty property)
        {
            label = EditorGUI.BeginProperty(totalPosition, label, property);
        }

        public void Dispose()
        {
            EditorGUI.EndProperty();
        }
    }
}
#endif