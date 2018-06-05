using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace xys.UI
{
    [CustomEditor(typeof(ContentSizeFitter), true)]
    [CanEditMultipleObjects]
    public class ContentSizeFitterEditor : SelfControllerEditor
    {
        SerializedProperty m_HorizontalFit;
        SerializedProperty m_VerticalFit;

        protected virtual void OnEnable()
        {
            if (serializedObject == null)
                return;

            m_HorizontalFit = serializedObject.FindProperty("m_HorizontalFit");
            m_VerticalFit = serializedObject.FindProperty("m_VerticalFit");
        }

        static void ShowVector2(ContentSizeFitter fitter, ref Vector2 src)
        {
            Vector2 rangSize = src;
            rangSize.x = EditorGUILayout.FloatField("min", rangSize.x);
            rangSize.y = EditorGUILayout.FloatField("max", rangSize.y);

            rangSize.x = Mathf.Max(0, rangSize.x);
            rangSize.y = Mathf.Max(rangSize.y, rangSize.x);

            if (rangSize != src)
            {
                src = rangSize;
                fitter.SetDirty();
            }
        }

        public override void OnInspectorGUI()
        {
            ContentSizeFitter fitter = target as ContentSizeFitter;

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_HorizontalFit, true);
            if (fitter.horizontalFit == ContentSizeFitter.FitMode.RangeSize)
            {
                ShowVector2(fitter, ref fitter.m_RangeSize[0]);
            }

            EditorGUILayout.PropertyField(m_VerticalFit, true);

            if (fitter.verticalFit == ContentSizeFitter.FitMode.RangeSize)
            {
                ShowVector2(fitter, ref fitter.m_RangeSize[1]);
            }

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
