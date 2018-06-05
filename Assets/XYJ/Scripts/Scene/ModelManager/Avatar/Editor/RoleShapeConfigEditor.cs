

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using EditorExtensions;

public class RoleShapeConfigEditor : JsonDirectoryEditorBase<RoleShapeConfig>
{

    protected override string GetJsonDirectoryPath()
    {
        return "Data/Config/Edit/Role/RoleShapeConfig/";
    }

    [MenuItem("Tools/角色工具/角色外形键值配置", false, 200)]
    static void ShowRoleShapeConfigEditor()
    {
        RoleShapeConfigEditor window = GetWindow<RoleShapeConfigEditor>(" 外形键值配置");
        window.minSize = new Vector2(640.0f, 480.0f);
        window.titleContent.image = EditorIconContent.GetSystem(EditorIconContentType.AvatarInspector_BodyPartPicker).image;
    }

    Vector2 m_scrollPos = Vector3.zero;
    Color m_bgColor = Color.white;

    void OnEnable ()
    {
        ReloadAll();
    }

    void OnDisable()
    {
        RecordLastConfigIndex();
        CheckSaveOnClose();
    }

    // Implement your own editor GUI here.
    void OnGUI ()
    {
        using (new AutoEditorVertical())
        {
            using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
            {
                // 工具栏
                DrawToolbarView();
            }

            // 是否有编辑内容提示
            if (!HasCurEditConfig())
            {
                EditorGUILayout.HelpBox("请选择或添加配置进行编辑！", MessageType.Warning);
                return;
            }

            // 显示编辑内容
            using (new AutoEditorScrollView(ref m_scrollPos))
            {
                DrawRoleShape();
            }
        }
        
    }

    // 显示某个角色的外形配置
    void DrawRoleShape()
    {
        using (new AutoEditorVertical(EditorStylesEx.BoxArea))
        {
            using (new AutoEditorLabelWidth(60))
            {
                // 角色外形基本信息
                using (new AutoEditorHorizontal())
                {
                    //m_curEditConfig.shapeKey = EditorGUILayout.TextField("文件", m_curEditConfig.shapeKey, GUILayout.Width(200));
                    //HandleEnterRenameEvent();
                    m_curEditConfig.shapeName = EditorGUILayout.TextField("描述:", m_curEditConfig.shapeName, GUILayout.Width(200));

                    EditorGUILayout.Separator();

                    if (GUILayout.Button("复制", GUILayout.Width(60)))
                    {
                        SaveCurrentConfig();
                        string newFile = m_curEditConfig.shapeKey + "(clone)";
                        if (!AddConfig(newFile, new RoleShapeConfig(m_curEditConfig)))
                        {
                            EditorUtility.DisplayDialog("复制配置失败", "复制文件失败！文件名："+ newFile, "确定");
                        }
                    }
                }
                
                m_curEditConfig.faceKey = EditorGUILayout.TextField("RootKey:", m_curEditConfig.faceKey, GUILayout.Width(200));


                using (new AutoEditorVertical(EditorStylesEx.BoxArea))
                {
                    using (new AutoEditorHorizontal())
                    {
                        EditorGUILayout.LabelField("基准脸键值配置");
                        EditorGUILayout.Separator();
                        if (GUILayout.Button("添加基准脸键值", GUILayout.Width(120)))
                        {
                            m_curEditConfig.baseFaces.Add(new RoleFaceBaseSet());
                        }
                    }
                    for (int i = 0; i < m_curEditConfig.baseFaces.Count; ++i)
                    {
                        using (new AutoEditorHorizontal())
                        {
                            m_curEditConfig.baseFaces[i].key = EditorGUILayout.TextField(string.Format("基准脸({0})：", i), m_curEditConfig.baseFaces[i].key, GUILayout.Width(300));
                            m_curEditConfig.baseFaces[i].scale = EditorGUILayout.Slider("身高缩放：", m_curEditConfig.baseFaces[i].scale, 0.5f, 2.0f, GUILayout.Width(200));
                            if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.miniButton, GUILayout.Width(30)))
                            {
                                m_curEditConfig.baseFaces.RemoveAt(i);
                            }
                        }
                    }
                }

                //m_bgColor = EditorGUILayout.ColorField(m_bgColor);
            }
        }

        EditorGUILayout.Space();
        using (new AutoEditorHorizontal(EditorStyles.toolbar))
        {
            EditorGUILayout.LabelField(new GUIContent("Part List"), EditorStyles.boldLabel, new GUILayoutOption[0]);

            // 添加部位
            if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                m_curEditConfig.faceParts.Add(new RoleShapePart());
            }

            GUILayout.Space(10);

            // 清空
            if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Trash), EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                m_curEditConfig.faceParts.Clear();
            }
        }
        if (m_curEditConfig.faceParts.Count == 0)
        {
            EditorGUILayout.HelpBox("当前没有任何部位配置，请添加！", MessageType.Warning);
        }


        // 部位列表
        for (int i = 0; i < m_curEditConfig.faceParts.Count; ++i)
        {
            RoleShapePart shapePart = m_curEditConfig.faceParts[i];

            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                using (new AutoEditorHorizontal())
                {
                    EditorGUILayout.Toggle(true, EditorStyles.foldout, GUILayout.Width(10));
                    using (new AutoEditorLabelWidth(60))
                    {
                        shapePart.name = EditorGUILayout.TextField("类型名称", shapePart.name, GUILayout.Width(160));
                    }

                    // 删除自己
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.miniButton, GUILayout.Width(30)))
                    {
                        m_curEditConfig.faceParts.Remove(shapePart);
                    }

                    // 添加
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.miniButton, GUILayout.Width(30)))
                    {
                        m_curEditConfig.faceParts.Add(new RoleShapePart());
                    }

                    // 复制
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Duplicate), EditorStyles.miniButton, GUILayout.Width(30)))
                    {
                        m_curEditConfig.faceParts.Add(new RoleShapePart(shapePart));
                    }
                    

                    // 移动
                    if (m_curEditConfig.faceParts.Count > 1)
                    {
                        if (GUILayout.Button(new GUIContent("↕"), GUILayout.Width(30)))
                        {
                            GenericMenu addMenu = new GenericMenu();
                            for (int tmp = 0; tmp < m_curEditConfig.faceParts.Count; ++tmp)
                            {
                                if (tmp == i)
                                    continue;
                                int movePos = tmp;
                                if (tmp > i)
                                    movePos += 1;
                                addMenu.AddItem(new GUIContent(string.Format("移动{0}位置", tmp)), false, (obj) =>
                                {
                                    m_curEditConfig.faceParts.Insert((int)obj, new RoleShapePart(shapePart));
                                    m_curEditConfig.faceParts.Remove(shapePart);
                                }, movePos);
                            }
                            addMenu.ShowAsContext();
                        }
                    }

                    EditorGUILayout.Space();

                    // 添加
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.miniButton, GUILayout.Width(30)))
                    {
                        shapePart.subParts.Add(new RoleShapeSubPart());
                    }

                    // 清空
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Trash), EditorStyles.miniButton, GUILayout.Width(30)))
                    {
                        shapePart.subParts.Clear();
                    }
                    
                }

                if (shapePart.subParts.Count == 0)
                {
                    EditorGUILayout.HelpBox("该类型没有任何部件，请添加！", MessageType.Warning);
                }

                // 每个小部位的键值配置
                for (int p = 0; p < shapePart.subParts.Count; ++p)
                {
                    RoleShapeSubPart subPart = shapePart.subParts[p];
                    Color col = Color.white;
                    if (p % 2 == 0)
                    {
                        col = col * 0.8f;
                        col.a = 1.0f;
                    }
                    using (new AutoGUIColor(col))
                    {
                        using (new AutoEditorHorizontal(EditorStylesEx.BoxArea))
                        {
                            EditorGUILayout.LabelField("", EditorStyles.radioButton, GUILayout.Width(10));
                            using (new AutoEditorLabelWidth(60))
                            {
                                subPart.name = EditorGUILayout.TextField("部位名称", subPart.name);
                                subPart.key = EditorGUILayout.TextField("部位键值", subPart.key);
                            }

                            // 删除自己
                            if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.miniButton, GUILayout.Width(30)))
                            {
                                shapePart.subParts.Remove(subPart);
                            }

                            // 添加
                            if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.miniButton, GUILayout.Width(30)))
                            {
                                shapePart.subParts.Add(new RoleShapeSubPart());
                            }

                            // 复制
                            if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Duplicate), EditorStyles.miniButton, GUILayout.Width(30)))
                            {
                                shapePart.subParts.Add(new RoleShapeSubPart(subPart));
                            }

                            // 移动
                            if (shapePart.subParts.Count > 1)
                            {
                                if (GUILayout.Button(new GUIContent("↕"), GUILayout.Width(30)))
                                {
                                    GenericMenu addMenu = new GenericMenu();
                                    for (int tmp = 0; tmp < shapePart.subParts.Count; ++tmp)
                                    {
                                        if (tmp == p)
                                            continue;
                                        int movePos = tmp;
                                        if (tmp > p)
                                            movePos += 1;
                                        addMenu.AddItem(new GUIContent(string.Format("移动{0}位置", tmp)), false, (obj) =>
                                        {
                                            shapePart.subParts.Insert((int)obj, new RoleShapeSubPart(subPart));
                                            shapePart.subParts.Remove(subPart);
                                        }, movePos);
                                    }
                                    addMenu.ShowAsContext();
                                }
                            }

                            EditorGUILayout.Space();


                            // 添加
                            if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.miniButton, GUILayout.Width(30)))
                            {
                                subPart.units.Add(new RoleShapePartUnit());
                            }

                            // 清空
                            if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Trash), EditorStyles.miniButton, GUILayout.Width(30)))
                            {
                                subPart.units.Clear();
                            }
                            
                        }
                        if (subPart.units.Count == 0)
                        {
                            EditorGUILayout.HelpBox("该部件没有配置任何成员，请添加！", MessageType.Warning);
                        }

                        for (int u = 0; u < subPart.units.Count; ++u)
                        {
                            // 各个部位的成员单位
                            RoleShapePartUnit unit = subPart.units[u];

                            using (new AutoEditorHorizontal())
                            {
                                using (new AutoEditorIndentLevel())
                                {
                                    using (new AutoEditorLabelWidth(60))
                                    {
                                        unit.name = EditorGUILayout.TextField("名称", unit.name);
                                        unit.key0 = EditorGUILayout.TextField("Key0", unit.key0);
                                        unit.key1 = EditorGUILayout.TextField("key1", unit.key1);
                                    }

                                    // 删除自己
                                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.miniButton, GUILayout.Width(30)))
                                    {
                                        subPart.units.Remove(unit);
                                    }

                                    // 添加单位
                                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.miniButton, GUILayout.Width(30)))
                                    {
                                        subPart.units.Add(new RoleShapePartUnit());
                                    }

                                    // 复制
                                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Duplicate), EditorStyles.miniButton, GUILayout.Width(30)))
                                    {
                                        subPart.units.Add(new RoleShapePartUnit(unit));
                                    }

                                    // 移动
                                    if (subPart.units.Count > 1)
                                    {
                                        if (GUILayout.Button(new GUIContent("↕"), GUILayout.Width(30)))
                                        {
                                            GenericMenu addMenu = new GenericMenu();
                                            for (int tmp = 0; tmp < subPart.units.Count; ++tmp)
                                            {
                                                if (tmp == u)
                                                    continue;
                                                int movePos = tmp;
                                                if (tmp > u)
                                                    movePos += 1;
                                                addMenu.AddItem(new GUIContent(string.Format("移动{0}位置", tmp)), false, (obj) =>
                                                {
                                                    subPart.units.Insert((int)obj, new RoleShapePartUnit(unit));
                                                    subPart.units.Remove(unit);
                                                }, movePos);
                                            }
                                            addMenu.ShowAsContext();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }// end部位列表
        
    }

    // Called as the new window is opened.
	void Awake () {
		
	}

    // Called multiple times per second on all visible windows.
	void Update () {
        
    }
}
