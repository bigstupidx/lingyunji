#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 关卡编辑器   点集
/// </summary>
[ExecuteInEditMode]
public class LevelDesignPoint : MonoBehaviour
{
    public LevelDesignConfig.LevelPointData m_data;

    void OnEnable()
    {
        if (null == m_data)
            m_data = new LevelDesignConfig.LevelPointData();
    }

    public void SetData(LevelDesignConfig.LevelPointData data)
    {
        m_data = data;
    }

    public void AddPointToSet(bool isFirst = false)
    {
        GameObject child = LevelDesignTool.CreateNode(transform);
        Selection.activeGameObject = gameObject;

        if (isFirst)
        {
            gameObject.transform.position = child.transform.position;
            child.transform.localPosition = Vector3.zero;
        }

        child.gameObject.name = transform.childCount.ToString();
    }

    void Update()
    {
        CollectData();
    }
    
    public void CollectData()
    {
        m_data.m_names.Clear();
        m_data.m_postions.Clear();
        m_data.m_dirs.Clear();

        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            m_data.m_names.Add(child.name);
            m_data.m_postions.Add(child.position);
            m_data.m_dirs.Add(child.eulerAngles);
        }
    }
}
#endif