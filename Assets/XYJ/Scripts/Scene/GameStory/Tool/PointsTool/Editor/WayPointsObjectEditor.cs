using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using EditorExtensions;

namespace xys.GameStory
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(WayPointsObject))]
    public class WayPointsObjectEditor : PointsObjectEditor<WayPoints, WayPointsObject>
    {

        [MenuItem("Tools/剧情编辑/新建路点对象")]
        static void CreateNewWayPointsObject()
        {
            WayPointsObject obj = WayPointsObject.Create();

            Selection.activeGameObject = obj.gameObject;
        }

        protected override void DrawShowWindowBtn()
        {
            if (GUILayout.Button("显示路点编辑工具"))
            {
                WayPointsObjectEditorWindow.GetWindow();
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

    public class WayPointsObjectEditorWindow : PointsObjectEditorWindow<WayPoints, WayPointsObject>
    {
        [MenuItem("Tools/剧情编辑/路点编辑工具")]
        public static WayPointsObjectEditorWindow GetWindow()
        {
            var window = GetWindow<WayPointsObjectEditorWindow>();
            window.titleContent = new GUIContent("路点编辑");
            window.minSize = new Vector2(400, 600);
            window.Focus();
            window.Repaint();
            return window;
        }

        public static void OpenWindow(WayPoints data, System.Action<WayPoints> fun)
        {
            var window = GetWindow<WayPointsObjectEditorWindow>();
            if (window.m_editObject != null && window.m_callbackEvent != null)
            {
                window.OnClose();
            }
            WayPointsObject obj = WayPointsObject.Create();
            obj.editData = data;
            Selection.activeGameObject = obj.gameObject;
            window.titleContent = new GUIContent("路点编辑");
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
            if (GUILayout.Button("新建路点对象"))
            {
                WayPointsObject obj = WayPointsObject.Create();

                Selection.activeGameObject = obj.gameObject;
            }
        }

        protected override void DrawButtomStatusBar()
        {
            if (m_editObject == null)
                return;

            using (new AutoEditorHorizontal(EditorStyles.toolbar))
            {

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

        protected override void DrawHeaderInfo()
        {
            base.DrawHeaderInfo();

            EditData.useDefaultSpeed = EditorGUILayout.Toggle("使用默认速度", EditData.useDefaultSpeed);
            EditData.repeatMode = (WayRepeatMode)EditorGUILayout.EnumPopup("RepeatMode", EditData.repeatMode);
            
        }

        protected override void DrawPointItemData(int index)
        {
            base.DrawPointItemData(index);

            WayPointData data = EditData.wayDatas[index];

            using (new AutoEditorLabelWidth(60))
            {
                data.waitTime = EditorGUILayout.Slider("等待时间", data.waitTime, 0, 100);
                data.moveSpeed = EditorGUILayout.Slider("移动速度", data.moveSpeed, 0, 100);
            }

            EditorGUILayout.Separator();

            using (new AutoEditorVertical(EditorStylesEx.FrameArea))
            {

                using (new AutoEditorHorizontal(EditorStylesEx.FrameArea))
                {
                    EditorGUILayout.LabelField("路点事件:", GUILayout.Width(60));

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("添加事件", GUILayout.Width(60)))
                    {
                        GenericMenu menu = new GenericMenu();
                        foreach (WayPointEventType type in System.Enum.GetValues(typeof(WayPointEventType)))
                        {
                            menu.AddItem(new GUIContent("添加：" + type.ToString()), false, (obj) =>
                            {
                                data.Add((WayPointEventType)obj);
                            }, type);
                        }

                        menu.ShowAsContext();
                    }

                    if (GUILayout.Button("清空", GUILayout.Width(60)))
                    {
                        data.Clear();
                    }
                }

                // 路点事件列表
                for (int i = 0; i < data.pointEvents.Count; ++i)
                {
                    WayPointEvent pointEvent = data.pointEvents[i];
                    using (new AutoEditorHorizontal(EditorStylesEx.FrameArea))
                    {
                        EditorGUILayout.LabelField(pointEvent.eventType.ToString(), EditorStyles.foldout, GUILayout.Width(80));

                        using (new AutoEditorLabelWidth(40))
                        {
                            pointEvent.eventParam = EditorGUILayout.TextField("|参数：", pointEvent.eventParam);
                        }

                        if (GUILayout.Button ("删除", GUILayout.Width(60)))
                        {
                            data.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }

}
