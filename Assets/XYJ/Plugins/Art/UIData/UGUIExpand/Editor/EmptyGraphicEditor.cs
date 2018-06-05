using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace UnityEditor.UI
{

    [CanEditMultipleObjects, CustomEditor(typeof(EmptyGraphic), false)]
    public class EmptyGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(base.m_Script, new GUILayoutOption[0]);
            // skipping AppearanceControlsGUI
            base.RaycastControlsGUI();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}