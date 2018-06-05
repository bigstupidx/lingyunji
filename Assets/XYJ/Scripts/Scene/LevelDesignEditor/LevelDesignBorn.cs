using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡编辑器   出生点
/// </summary>
[ExecuteInEditMode]
public class LevelDesignBorn : MonoBehaviour
{
    public LevelDesignConfig.LevelBornData m_data;

    void OnEnable()
    {
        if (null == m_data)
            m_data = new LevelDesignConfig.LevelBornData();
    }

    public void SetData(LevelDesignConfig.LevelBornData data)
    {
        m_data = data;
    }

    public void SetPos(Vector3 pos)
    {
        m_data.m_pos = pos;
    }

    public void SetDir(Vector3 dir)
    {
        m_data.m_dir = dir;
    }
}
