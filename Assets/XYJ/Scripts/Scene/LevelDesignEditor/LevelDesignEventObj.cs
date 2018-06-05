#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 关卡编辑器   事件对象
/// </summary>
[ExecuteInEditMode]
public class LevelDesignEventObj : MonoBehaviour
{
    public LevelDesignConfig.LevelEventObjData m_data;

    void OnEnable()
    {
        if (null == m_data)
            m_data = new LevelDesignConfig.LevelEventObjData();
    }

    public void SetData(LevelDesignConfig.LevelEventObjData data)
    {
        m_data = data;
    }

    public void SetConditionType(int index, int conditionType)
    {
        if(index >= 0 && index < m_data.m_conditions.Count)
        {
            m_data.m_conditions[index].m_conditionType = (LevelDesignConfig.ConditionType)conditionType;
        }
    }

    public void SetActionType(int index, int actionType)
    {
        if(index >= 0 && index < m_data.m_actions.Count)
        {
            m_data.m_actions[index].m_actionType = (LevelDesignConfig.ActionType)actionType;
        }
    }
}
#endif