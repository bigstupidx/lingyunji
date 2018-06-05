

namespace xys.GameStory
{
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEditor;

    using EditorExtensions;
    

    /// <summary>
    /// 点集合对象模版编辑器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PointsObjectEditor<T, U> : Editor where T : Points, new() where U : PointsObject<T>
    {
        // 点集合对象和数据
        protected U m_DataObject;
        protected T m_Data;

        protected bool ShowGizmos
        {
            get { return m_DataObject.showGizmos; }
            set { m_DataObject.showGizmos = value; }
        }

        protected bool EditPosition
        {
            get { return m_DataObject.editPosition; }
            set { m_DataObject.editPosition = value; }
        }

        protected bool EditRotation
        {
            get { return m_DataObject.editRotation; }
            set { m_DataObject.editRotation = value; }
        }

        void OnEnable()
        {
            m_DataObject = target as U;
            m_Data = m_DataObject.editData;
            m_DataObject.CheckToggles();

            if (Application.isPlaying)
                m_DataObject.m_hasSelected = true;
            //if (!m_DataObject.m_hasSelected && Selection.activeGameObject == m_DataObject.gameObject)
            //    m_DataObject.m_hasSelected = true;
            //else if (m_DataObject.m_hasSelected && Selection.activeGameObject != m_DataObject.gameObject)
            //    m_DataObject.m_hasSelected = false;

            LaterOnEnable();
        }

        void OnDisable()
        {
            if (Application.isPlaying)
                m_DataObject.m_hasSelected = false;

            LaterOnDisable();
        }

        /// <summary>
        /// OnSceneGUI
        /// </summary>
        protected virtual void OnSceneGUI()
        {
            using (new AutoEditorChangeCheck(() =>
            {
                Undo.RecordObject(target, string.Format("{0}:{1}", typeof(T).Name, m_DataObject.objectName));
                EditorUtility.SetDirty(target);
                SceneView.RepaintAll();
            }))
            {
                DrawPointsSceneGUI();

                LaterDrawSceneGUI();
            }
        }

        /// <summary>
        /// 绘制Scene视窗的UI
        /// </summary>
        protected void DrawPointsSceneGUI()
        {

            if (m_Data.Count > 0)
            {
                for (int i = 0; i < m_Data.Count; ++i)
                {
                    Vector3 pointPos = m_Data.GetPosition(i);
                    Quaternion pointRot = m_Data.GetRotation(i);

                    // 显示位置信息Label
                    using (new AutoHandlesColor(Color.green))
                    {
                        Handles.Label(pointPos + Vector3.up, string.Format("{0} ({1})", m_Data.id, i));
                    }
                    DrawPointHandles(i, pointPos, pointRot, 0.2f);

                    if (EditPosition)
                    {
                        Vector3 pos = Handles.PositionHandle(pointPos, pointRot);
                        if (pointPos != pos)
                        {
                            // Set position
                            m_Data.SetPosition(i, pos);
                        }
                    }

                    if (EditRotation)
                    {
                        Quaternion rot = Handles.RotationHandle(pointRot, pointPos);
                        if (pointRot != rot)
                        {
                            // Set Rotation
                            m_Data.SetRotation(i, rot);
                        }
                    }
                }

                // draw way line
                if (m_Data.Count > 1)
                {
                    Vector3[] lineSegments = new Vector3[m_Data.Count * 2];
                    int pointIndex = 0;
                    for (int i = 0; i < m_Data.Count - 1; i++)
                    {
                        lineSegments[pointIndex++] = m_Data.GetPosition(i);
                        lineSegments[pointIndex++] = m_Data.GetPosition(i + 1);
                    }
                    Handles.DrawDottedLines(lineSegments, 2.0f);
                }
            }

            // Display paths manager's edit window on scene
            Handles.BeginGUI();
            GUILayout.Window(0, new Rect(Screen.width - 128, Screen.height - 120, 120, 100), DrawSceneWindowGUI, "PointSet");
            Handles.EndGUI();
        }

        /// <summary>
        /// Cylinders the handles.
        /// </summary>
        protected void DrawPointHandles(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            // Center
            using (new AutoHandlesColor(Handles.selectedColor))//Color.yellow;
            {
                Handles.CubeHandleCap(controlID, position, rotation, size, EventType.Repaint);
            }


            if (!EditPosition)
            {
                Vector3 capPos = position + rotation * Vector3.forward * size * 3;
                using (new AutoHandlesColor(Handles.selectedColor))
                {
                    Handles.DrawLine(position, capPos);
                    Handles.ArrowHandleCap(controlID, capPos, rotation, size * 6, EventType.Repaint);
                }
            }

            /*
            if (!EditPosition && !EditRotation)
            {
                // Forward
                Vector3 capPos = position + rotation * Vector3.forward * size * 3;
                Handles.color = Color.blue;
                Handles.DrawLine(position, capPos);
                Handles.CylinderCap(controlID, capPos, rotation, size);

                // Up
                capPos = position + rotation * Vector3.up * size * 3;
                Handles.color = Color.green;
                Handles.DrawLine(position, capPos);
                Handles.CylinderCap(controlID, capPos, rotation * Quaternion.Euler(Vector3.left * 90.0f), size);

                // Right
                capPos = position + rotation * Vector3.right * size * 3;
                Handles.color = Color.red;
                Handles.DrawLine(position, capPos);
                Handles.CylinderCap(controlID, capPos, rotation * Quaternion.Euler(Vector3.up * 90.0f), size);

                Handles.color = Color.white;
            }*/
        }

        protected virtual void DrawShowWindowBtn()
        {

        }

        /// <summary>
        /// 显示一些基本操作到InspectorGUI
        /// </summary>
        protected void DrawInspectorGUI()
        {
            EditorGUILayout.Space();

            DrawShowWindowBtn();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            // 添加
            if (GUILayout.Button("添加点"))
            {
                m_DataObject.Add();
                SceneView.RepaintAll();
            }

            // 插入
            if (GUILayout.Button("插入点"))
            {
                if (m_Data.Count == 0)
                {
                    m_DataObject.Add();
                    SceneView.RepaintAll();
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < m_Data.Count; ++i)
                    {
                        menu.AddItem(new GUIContent("插入点位置：" + i), false, (obj) =>
                        {
                            m_DataObject.Insert((int)obj);
                            SceneView.RepaintAll();
                        }, i);
                    }

                    menu.ShowAsContext();
                }

            }

            // 移除
            if (GUILayout.Button("移除点"))
            {
                if (m_Data.Count > 0)
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < m_Data.Count; ++i)
                    {
                        menu.AddItem(new GUIContent("移除点位置：" + i), false, (obj) =>
                        {
                            m_DataObject.RemoveAt((int)obj);
                            SceneView.RepaintAll();
                        }, i);
                    }

                    menu.ShowAsContext();
                }
            }

            // 清空
            if (GUILayout.Button("清空点"))
            {
                m_DataObject.Clear();
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制Scene视窗的WindowGUI
        /// </summary>
        /// <param name="winID"></param>
        protected virtual void DrawSceneWindowGUI(int winID)
        {
            bool toggleResult = false;

            // Show Gizmos Toggle
            toggleResult = GUILayout.Toggle(ShowGizmos, "Show Gizmos");
            if (toggleResult != ShowGizmos)
            {
                ShowGizmos = toggleResult;
            }

            // Edit Positions Toggle
            toggleResult = GUILayout.Toggle(EditPosition, "Edit Position");
            if (toggleResult != EditPosition)
            {
                EditPosition = toggleResult;

                SceneView.RepaintAll();
            }

            // Edit Rotations Toggle
            toggleResult = GUILayout.Toggle(EditRotation, "Edit Rotation");
            if (toggleResult != EditRotation)
            {
                EditRotation = toggleResult;

                SceneView.RepaintAll();
            }

            // reset rotation
            if (GUILayout.Button("Reset Rotations"))
            {
                for (int i = 0; i < m_Data.Count; ++i)
                {
                    m_Data.SetRotation(i, Quaternion.identity);
                }
                SceneView.RepaintAll();
            }
        }

        protected virtual void LaterOnEnable() { }

        protected virtual void LaterOnDisable() { }

        protected virtual void LaterDrawSceneGUI() { }

    }


    /// <summary>
    /// 编辑窗口，支持两种模式
    /// 1. 编辑选择的点集对象
    /// 2. 只处理显示调用需要编辑的点集对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PointsObjectEditorWindow<T, U> : EditorWindow where T : Points, new() where U : PointsObject<T>
    {
        protected U m_editObject;
        protected T EditData
        {
            get { return m_editObject.editData; }
        }

        // 如果该窗口需要返回编辑信息，需要传回调事件
        protected System.Action<T> m_callbackEvent;

        Vector2 m_pointsScrollPos = Vector2.zero;
        bool m_pointListToggle = true;

        public virtual void OnShow(U dataObj, System.Action<T> fun)
        {
            m_editObject = dataObj;
            m_callbackEvent = fun;
        }

        public virtual void OnSave()
        {
            if (m_callbackEvent != null)
            {

                if (EditorUtility.DisplayDialog("保存", "是否保存编辑数据？", "确定", "取消"))
                {
                    m_callbackEvent(EditData);
                    m_callbackEvent = null;
                    if (m_editObject!=null)
                        GameObject.DestroyImmediate(m_editObject.gameObject);

                    Close();
                }
            }
        }

        public virtual void OnClose()
        {
            if (m_callbackEvent != null)
            {
                m_callbackEvent = null;
                if (m_editObject!=null)
                    GameObject.DestroyImmediate(m_editObject.gameObject);
            }
        }

        protected virtual void OnEditObjectChange()
        {
            if (m_callbackEvent == null)
            {
                m_editObject = null;
                if (Selection.activeGameObject != null)
                {
                    m_editObject = Selection.activeGameObject.GetComponent<U>();
                }
                Repaint();
            }
        }

        protected virtual void DrawHeaderInfo()
        {

            EditorGUILayout.LabelField("基本信息");
            using (new AutoEditorLabelWidth(60))
            {
                EditData.id = EditorGUILayout.TextField("ID", EditData.id);
                EditData.describe = EditorGUILayout.TextField("描述", EditData.describe);
            }

        }

        protected virtual void DrawCreateObjectBtn()
        {

        }

        protected virtual void DrawPointList()
        {
            if (m_editObject == null)
            {
                // 提示没有可编辑的对象
                EditorGUILayout.HelpBox("当前没有可编辑的路点对象，请选择需要编辑的路点对象或新建路点对象！", MessageType.Warning);

                DrawCreateObjectBtn();
                return;
            }

            using (new AutoEditorVertical(EditorStylesEx.FrameArea))
            {
                DrawHeaderInfo();
            }

            EditorGUILayout.Separator();

            using (new AutoEditorVertical(EditorStylesEx.PreferencesSectionBox))
            {
                using (new AutoEditorHorizontal(EditorStyles.toolbar))
                {

                    bool tmpToggle = EditorGUILayout.Toggle(m_pointListToggle, GUILayout.Width(15));
                    if (tmpToggle != m_pointListToggle)
                    {
                        m_pointListToggle = tmpToggle;
                        m_editObject.SetToggles(tmpToggle);
                    }

                    GUILayout.Label(new GUIContent("点集列表"), EditorStyles.toolbarButton, GUILayout.Width(80));

                    EditorGUILayout.Separator();

                    // 添加
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.toolbarButton, GUILayout.Width(30)))
                    {
                        m_editObject.Add();
                        SceneView.RepaintAll();
                        Repaint();
                    }
                    // 插入
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus_More), EditorStyles.toolbarButton, GUILayout.Width(30)))
                    {
                        if (EditData.Count == 0)
                        {
                            m_editObject.Add();
                            SceneView.RepaintAll();
                        }
                        else
                        {
                            GenericMenu menu = new GenericMenu();
                            for (int i = 0; i < EditData.Count; ++i)
                            {
                                menu.AddItem(new GUIContent("插入点位置：" + i), false, (obj) =>
                                {
                                    m_editObject.Insert((int)obj);
                                    SceneView.RepaintAll();
                                }, i);
                            }

                            menu.ShowAsContext();
                        }
                    }
                    // 删除
                    //if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(30)))
                    //{
                    //    GenericMenu menu = new GenericMenu();
                    //    for (int i = 0; i < EditData.Count; ++i)
                    //    {
                    //        menu.AddItem(new GUIContent("移除点位置：" + i), false, (obj) =>
                    //        {
                    //            m_editObject.RemoveAt((int)obj);
                    //            SceneView.RepaintAll();
                    //        }, i);
                    //    }

                    //    menu.ShowAsContext();
                    //}
                    // 清空
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Trash), EditorStyles.toolbarButton, GUILayout.Width(30)))
                    {
                        m_editObject.Clear();
                        SceneView.RepaintAll();
                        Repaint();
                    }
                }

                if (EditData.Count == 0)
                {
                    EditorGUILayout.HelpBox("没有路点数据，请添加！", MessageType.Warning);
                    return;
                }

                using (new AutoEditorScrollView(ref m_pointsScrollPos))
                {
                    for (int i = 0; i < EditData.Count; ++i)
                    {
                        using (new AutoEditorVertical(EditorStylesEx.FrameArea))
                        {
                            using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
                            {
                                m_editObject.selectedToggles[i] = EditorGUILayout.Toggle(m_editObject.selectedToggles[i], GUILayout.Width(15));

                                EditorGUILayout.LabelField("点" + i);

                                // 删除
                                if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(30)))
                                {
                                    m_editObject.RemoveAt(i);
                                    SceneView.RepaintAll();
                                }
                                
                                // 移动
                                if (GUILayout.Button(new GUIContent("↕"), EditorStyles.toolbarButton, GUILayout.Width(30)))
                                {
                                    GenericMenu moveMenu = new GenericMenu();
                                    for (int tmp = 0; tmp < EditData.Count; ++tmp)
                                    {
                                        if (tmp == i)
                                            continue;
                                        int movePos = tmp;
                                        if (tmp > i)
                                            movePos += 1;
                                        moveMenu.AddItem(new GUIContent(string.Format("移动到{0}位置", tmp)), false, (obj) =>
                                        {
                                            object[] objArr = (object[])obj;
                                            m_editObject.Move((int)objArr[1], (int)objArr[0]);
                                            SceneView.RepaintAll();

                                        }, new object[] { movePos, i });
                                    }
                                    moveMenu.ShowAsContext();
                                }
                                // 贴地
                                if (GUILayout.Button(new GUIContent("贴地"), EditorStyles.toolbarButton, GUILayout.Width(50)))
                                {
                                     RaycastHit hit;
                                     if (Physics.Raycast(EditData.GetPosition(i) + Vector3.up * 2.0f, Vector3.down, out hit, 1000, ComLayer.Layer_GroundMask))
                                     {
                                         if (hit.collider != null && hit.collider.gameObject != null)
                                         {
                                             Vector3 hitPos = EditData.GetPosition(i);
                                             hitPos.y = hit.point.y;
                                             EditData.SetPosition(i, hitPos);
                                         }
                                     }
                                }
                            }

                            if (m_editObject.selectedToggles[i])
                            {
                                DrawPointItemData(i);
                            }
                        }
                    }
                }// end AutoEditorScrollView

            }
        }

        protected virtual void DrawPointItemData(int index)
        {
            Vector3 pos = EditData.GetPosition(index);
            Quaternion rot = EditData.GetRotation(index);

            using (new AutoEditorLabelWidth(120))
            {
                Vector3 tmpPos = EditorGUILayout.Vector3Field("Position", pos, GUILayout.Width(240));
                if (tmpPos!=pos)
                {
                    EditData.SetPosition(index, tmpPos);
                    SceneView.RepaintAll();
                }

                EditorGUILayout.Vector3Field("Rotation", rot.eulerAngles, GUILayout.Width(240));
            }
        }

        protected virtual void DrawButtomStatusBar()
        {
            if (m_callbackEvent == null || m_editObject == null)
                return;

            using (new AutoEditorHorizontal(EditorStyles.toolbar))
            {

                EditorGUILayout.Separator();

                // 保存
                if (GUILayout.Button(new GUIContent("保存"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    OnSave();
                }
            }
        }

    }

}
