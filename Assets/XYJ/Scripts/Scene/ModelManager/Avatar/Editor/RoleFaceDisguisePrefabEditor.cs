using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using EditorExtensions;

public class RoleFaceDisguisePrefabEditor : JsonDirectoryEditorBase<RoleFaceDisguisePrefab>
{

    protected override string GetJsonDirectoryPath()
    {
        return "Data/Config/Edit/Role/RoleFaceDisguisePrefab/";
    }

    [MenuItem("Tools/角色工具/角色换脸预设配置", false, 200)]
    static void ShowRoleShapeConfigEditor()
    {
        RoleFaceDisguisePrefabEditor window = GetWindow<RoleFaceDisguisePrefabEditor>(" 换脸预设配置");
        window.minSize = new Vector2(640.0f, 480.0f);
        window.titleContent.image = EditorIconContent.GetSystem(EditorIconContentType.Prefab_Icon).image;
    }

    Vector2 m_scrollPos = Vector3.zero;

    void OnEnable()
    {
        ReloadAll();
    }

    void OnDisable()
    {

    }

    protected override void OnAddNewConfig()
    {
        if (Application.isPlaying && xys.UI.UIAvatar.Instance != null)
        {
            EditorMessageBox.Display("保存当前换脸数据", "文件名", string.Empty, (name) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogError("配置文件名不能为空！");
                    EditorUtility.DisplayDialog("添加配置失败", "配置文件名不能为空！", "确定");
                    return;
                }

                RoleFaceDisguisePrefab prefab = new RoleFaceDisguisePrefab();
                prefab.skinData.Clone(xys.UI.UIAvatar.Instance.GetSkinData());
                prefab.shapeData.Clone(xys.UI.UIAvatar.Instance.GetShapeData());
                prefab.SetKey(name);
                AddConfig(name, prefab);
            });
        }
        else
        {
            EditorUtility.DisplayDialog("添加配置失败", "请运行外观场景进行编辑添加！！", "确定");
        }
    }

    protected override void SaveCurrentConfig()
    {
        m_curEditConfig.skinData.Clone(xys.UI.UIAvatar.Instance.GetSkinData());
        m_curEditConfig.shapeData.Clone(xys.UI.UIAvatar.Instance.GetShapeData());
        base.SaveCurrentConfig();
    }

    // Implement your own editor GUI here.
    void OnGUI()
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
                DrawConfig();
            }
        }

    }

    void DrawConfig()
    {
        using (new AutoEditorLabelWidth(60))
        {
            m_curEditConfig.describe = EditorGUILayout.TextField("描述:", m_curEditConfig.describe);
        }
        using (new AutoEditorVertical(EditorStylesEx.BoxArea))
        {
            using (new AutoEditorHorizontal())
            {
                EditorGUILayout.LabelField("Skin Unit Count:" + m_curEditConfig.skinData.skinUnits.Count);
                EditorGUILayout.Separator();
                if (Application.isPlaying && xys.UI.UIAvatar.Instance != null)
                {
                    if (GUILayout.Button("应用预设", GUILayout.Width(90)))
                    {
                        xys.UI.UIAvatar.Instance.SetSkinData(m_curEditConfig.skinData);
                    }
                }
            }
            for (int i = 0; i < m_curEditConfig.skinData.skinUnits.Count; ++i)
            {
                RoleSkinUnitData unit = m_curEditConfig.skinData.skinUnits[i];
                EditorGUILayout.LabelField("key:" + unit.key, EditorStyles.boldLabel);
                using (new AutoEditorIndentLevel())
                {
                    EditorGUILayout.LabelField("Texture Style:" + unit.texStyle);
                    EditorGUILayout.LabelField("Color Style:" + unit.colorStyle);
                    using (new AutoEditorHorizontal())
                    {

                        EditorGUILayout.LabelField("H:" + unit.h, GUILayout.Width(60));
                        EditorGUILayout.LabelField("S:" + unit.s, GUILayout.Width(60));
                        EditorGUILayout.LabelField("V:" + unit.v, GUILayout.Width(60));
                    }
                }
                EditorGUILayout.Space();
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Shape Unit Count:" + m_curEditConfig.shapeData.partUnits.Count);
            for (int i = 0; i < m_curEditConfig.shapeData.partUnits.Count; ++i)
            {
                RoleShapeUnitData unit = m_curEditConfig.shapeData.partUnits[i];
                using (new AutoEditorHorizontal())
                {
                    EditorGUILayout.LabelField("key:" + unit.key, GUILayout.Width(300));
                    EditorGUILayout.LabelField("value:" + unit.value);
                }
            }
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
