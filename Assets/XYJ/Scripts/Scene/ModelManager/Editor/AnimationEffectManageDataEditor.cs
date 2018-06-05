

#if SCENE_DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationEffectManageData))]
public class AnimationEffectManageDataEditor : Editor
{
    AnimationEffectManageData managerData = null;
    string[] m_animList;
    int m_selectIndex = -1;

    SerializedProperty testAnim;

    private void OnEnable()
    {
        testAnim = serializedObject.FindProperty("m_testPlayAnimation");
    }

    public override void OnInspectorGUI()
    {
        if (managerData==null)
        {
            managerData = target as AnimationEffectManageData;
            m_animList = managerData.GetAnimList();
            m_selectIndex = -1;
        }

        if (m_animList != null && m_animList.Length > 0)
        {
            EditorGUILayout.BeginHorizontal();

            int tmpI = EditorGUILayout.Popup("选择动画", m_selectIndex, m_animList);
            if (tmpI != m_selectIndex)
            {
                m_selectIndex = tmpI;
                testAnim.stringValue = m_animList[m_selectIndex];
                serializedObject.ApplyModifiedProperties();
                managerData.PlayTestAnim();
            }

            if (GUILayout.Button("播放", GUILayout.Width(50)))
                managerData.PlayTestAnim();

            EditorGUILayout.EndHorizontal();
        }

        base.OnInspectorGUI();
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update () {
		
	}

}
#endif
