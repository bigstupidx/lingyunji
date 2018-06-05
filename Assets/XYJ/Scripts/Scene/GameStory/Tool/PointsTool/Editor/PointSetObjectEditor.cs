using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace xys.GameStory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PointSetObject))]
    public class PointSetObjectEditor : PointsObjectEditor<Points, PointSetObject>
    {

        [MenuItem("Tools/剧情编辑/新建点集对象")]
        static void CreateNewPointSetObject()
        {
            PointSetObject obj = PointSetObject.Create();

            Selection.activeGameObject = obj.gameObject;
        }


        public override void OnInspectorGUI()
        {
            DrawInspectorGUI();

            base.OnInspectorGUI();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

    }

    public class PointSetObjectEditorWindow : PointsObjectEditorWindow<Points, PointSetObject>
    {
        [MenuItem("Tools/剧情编辑/点集编辑工具")]
        public static PointSetObjectEditorWindow GetWindow()
        {
            var window = GetWindow<PointSetObjectEditorWindow>();
            window.titleContent = new GUIContent("点集编辑");
            window.Focus();
            window.Repaint();
            return window;
        }

        public static void OpenWindow(Points data, System.Action<Points> fun)
        {
            var window = GetWindow<PointSetObjectEditorWindow>();
            if (window.m_editObject != null && window.m_callbackEvent != null)
            {
                window.OnClose();
            }
            PointSetObject obj = PointSetObject.Create();
            obj.editData = data;
            Selection.activeGameObject = obj.gameObject;
            window.titleContent = new GUIContent("点集编辑");
            window.OnShow(obj, fun);
            window.Focus();
            window.Repaint();
        }

        private void OnDestroy()
        {
            OnClose();
        }

        private void OnFocus()
        {
            if (m_editObject == null)
                OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            OnEditObjectChange();
        }

        private void OnGUI()
        {
            DrawPointList();

            DrawButtomStatusBar();
        }
    }

}
