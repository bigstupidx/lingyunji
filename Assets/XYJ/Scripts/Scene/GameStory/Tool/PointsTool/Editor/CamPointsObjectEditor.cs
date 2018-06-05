
namespace xys.GameStory
{
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEditor;

    using EditorExtensions;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(CamPointsObject))]
    public class CamPointsObjectEditor : PointsObjectEditor<CamPoints, CamPointsObject>
    {

        [MenuItem("Tools/剧情编辑/新建镜头动画对象")]
        static void CreateNewPointSetObject()
        {
            CamPointsObject obj = CamPointsObject.Create();

            Selection.activeGameObject = obj.gameObject;
        }

        protected override void LaterOnEnable()
        {
            m_DataObject.InitMouseInput();
        }

        protected override void DrawShowWindowBtn()
        {
            if (GUILayout.Button ("显示镜头动画编辑工具"))
            {
                CamPointsObjectEditorWindow.GetWindow();
            }
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

    public class CamPointsObjectEditorWindow : PointsObjectEditorWindow<CamPoints, CamPointsObject>
    {
        [MenuItem("Tools/剧情编辑/镜头动画编辑工具")]
        public static CamPointsObjectEditorWindow GetWindow()
        {
            var window = GetWindow<CamPointsObjectEditorWindow>();
            window.titleContent = new GUIContent("镜头动画编辑");
            window.minSize = new Vector2(400, 600);
            window.Focus();
            window.Repaint();
            return window;
        }

        public static void OpenWindow(CamPoints data, System.Action<CamPoints> fun)
        {
            var window = GetWindow<CamPointsObjectEditorWindow>();
            if (window.m_editObject != null && window.m_callbackEvent != null)
            {
                window.OnClose();
            }
            CamPointsObject obj = CamPointsObject.Create();
            obj.editData = data;
            Selection.activeGameObject = obj.gameObject;
            window.titleContent = new GUIContent("镜头动画编辑");
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

        protected override void DrawCreateObjectBtn()
        {
            if (GUILayout.Button("新建镜头动画对象"))
            {
                CamPointsObject obj = CamPointsObject.Create();

                Selection.activeGameObject = obj.gameObject;
            }
        }

        protected override void DrawButtomStatusBar()
        {
            if (m_editObject == null)
                return;

            using (new AutoEditorHorizontal(EditorStyles.toolbar))
            {
                // 播放
                if (GUILayout.Button(new GUIContent("播放"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    if (Application.isPlaying)
                    {
                        m_editObject.PlayAnim();
                    }
                }

                EditorGUILayout.Separator();
                if (m_callbackEvent != null)
                {
                    // 保存
                    if (GUILayout.Button(new GUIContent("保存"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        OnSave();
                    }
                }
            }
        }

        protected override void DrawPointItemData(int index)
        {
            base.DrawPointItemData(index);

            CameraPointData data = EditData.cameraData[index];

            using (new AutoEditorLabelWidth(60))
            {
                data.keyTime = EditorGUILayout.Slider("Time", data.keyTime, 0, 100);
                float fov = EditorGUILayout.Slider("FOV", data.fov, 1, 89);
                if (fov != data.fov)
                {
                    data.fov = fov;
                    if (Application.isPlaying)
                    {
                        m_editObject.SetCamFov(fov);
                    }
                }
            }
        }
    }

}
