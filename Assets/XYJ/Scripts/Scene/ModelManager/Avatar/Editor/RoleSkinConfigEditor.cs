using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorExtensions;

[CustomEditor(typeof(RoleSkinConfig))]
public class RoleSkinConfigEditor : Editor
{

    RoleSkinConfig m_config;

    bool m_toggle = false;

    void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        if (m_config == null)
            m_config = target as RoleSkinConfig;

        if (m_config!=null && m_config.parts.Count>0)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("选择复制配置"))
            {
                GenericMenu gmenu = new GenericMenu();
                List<string> keys = m_config.GetKeys();
                foreach (var key in keys)
                {
                    gmenu.AddItem(new GUIContent(key), false, (obj) =>
                    {
                        m_config.ClonePart((string)key);
                    }, key);
                }
                gmenu.ShowAsContext();
            }
            if (GUILayout.Button("删除配置"))
            {
                GenericMenu gmenu = new GenericMenu();
                List<string> keys = m_config.GetKeys();
                foreach (var key in keys)
                {
                    gmenu.AddItem(new GUIContent(key), false, (obj) =>
                    {
                        m_config.DeletePart((string)key);
                    }, key);
                }
                gmenu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();

            // 处理部位顺序
            m_toggle = EditorGUILayout.Foldout(m_toggle, "编辑部位顺序");
            if (m_toggle)
            {
                using (new AutoEditorIndentLevel())
                {
                    for (int i = 0; i < m_config.parts.Count; ++i)
                    {
                        RoleSkinPart skinPart = m_config.parts[i];
                        EditorGUILayout.LabelField("化妆Key:" + skinPart.skinKey);
                        for (int j = 0; j < skinPart.units.Count; ++j)
                        {
                            RoleSkinUnit unit = skinPart.units[j];
                            if (GUILayout.Button(string.Format("{0}-{1}", unit.key, unit.name)))
                            {
                                skinPart.units.Add(unit);
                                skinPart.units.RemoveAt(j);
                                EditorUtility.SetDirty(target);
                                break;
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Separator();

        }
        
        base.OnInspectorGUI();
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
