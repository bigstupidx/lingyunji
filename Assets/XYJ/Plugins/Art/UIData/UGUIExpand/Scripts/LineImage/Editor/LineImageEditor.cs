using UnityEditor;
using UnityEditor.UI;

namespace UI
{
    [CustomEditor(typeof(LineImage), true)]
    [CanEditMultipleObjects]
    public class LineImageEditor : GraphicEditor
    {
        SerializedProperty m_Sprite;
        SerializedProperty m_LSprite;
        SerializedProperty m_TSprite;
        SerializedProperty m_XSprite;
        SerializedProperty m_Width;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Sprite = serializedObject.FindProperty("m_Sprite");
            m_LSprite = serializedObject.FindProperty("m_LSprite");
            m_TSprite = serializedObject.FindProperty("m_TSprite");
            m_XSprite = serializedObject.FindProperty("m_XSprite");
            m_Width = serializedObject.FindProperty("m_Width");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Width);
            EditorGUILayout.PropertyField(m_Sprite);
            EditorGUILayout.PropertyField(m_LSprite);
            EditorGUILayout.PropertyField(m_TSprite);
            EditorGUILayout.PropertyField(m_XSprite);
            serializedObject.ApplyModifiedProperties();
        }
    }
}