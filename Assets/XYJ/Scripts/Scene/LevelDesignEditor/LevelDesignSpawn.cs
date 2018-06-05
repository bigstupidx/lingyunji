#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 关卡刷怪器
/// </summary>
[ExecuteInEditMode]
public class LevelDesignSpawn : MonoBehaviour
{
    public LevelDesignConfig.LevelSpawnData m_data;

    void OnEnable()
    {
        if (null == m_data)
            m_data = new LevelDesignConfig.LevelSpawnData();
    }

    public void SetData(LevelDesignConfig.LevelSpawnData data)
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
        m_data.m_scales.Clear();

        for (int i = 0; i < transform.childCount; ++i)
        {
            m_data.m_names.Add(transform.GetChild(i).name);
            m_data.m_postions.Add(transform.GetChild(i).position);
            m_data.m_dirs.Add(transform.GetChild(i).eulerAngles);
            m_data.m_scales.Add(transform.GetChild(i).localScale);
        }
    }

    /// <summary>
    /// 绘制视野范围
    /// </summary>
    void OnDrawGizmos()
    {
        if (m_data == null || m_data.m_fieldOfVision <= 0 || m_data.m_postions == null || m_data.m_postions.Count == 0)
            return;

        if (!ShowFov())
            return;

        foreach (Vector3 pos in m_data.m_postions)
        {
            Gizmos.DrawWireSphere(pos, m_data.m_fieldOfVision);
        }
    }

    bool ShowFov()
    {
        bool chooseChild = false;
        if (Selection.activeObject is GameObject)
        {
            GameObject go = Selection.activeObject as GameObject;

            if (go.transform.parent == gameObject.transform)
            {
                chooseChild = true;
            }
        }
        if (Selection.activeObject == gameObject || chooseChild)
        {
            return true;
        }
        return false;
    }
}
#endif