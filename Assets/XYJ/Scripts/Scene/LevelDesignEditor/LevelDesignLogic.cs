#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 每个关卡编辑里面可以实现多套逻辑，每个逻辑都是独立的
/// </summary>

[ExecuteInEditMode]
public class LevelDesignLogic : MonoBehaviour
{
    public LevelDesignConfig.LevelLogicData m_data;
    public Object m_scene;
    public Transform m_eventSet;

    void OnEnable()
    {
        if(null == m_data)
            m_data = new LevelDesignConfig.LevelLogicData();
    }

    public void SetData(LevelDesignConfig.LevelLogicData data)
    {
        m_data = data;
        //事件集
        //GameObject eventGo = gameObject.transform.FindChild("事件集").gameObject;
        //GameObject[] gos = eventGo.GetComponentsInChildren<GameObject>();
        //for(int i = 0; i < gos.Length; ++i)
        //{
        //    if(gos[i].name == data.m_eventSet)
        //    {
        //        m_eventSet = gos[i];
        //    }
        //}
    }

    //设置场景
    public void SetScene(object scene)
    {
        m_scene = scene as Object;
        m_data.m_scene = m_scene.name;
    }

    //设置场景风格
    public void SetSceneStyle(string style)
    {
        m_data.m_sceneStyle = style;
    }

    //设置事件集
    //public void SetEventSet(object set)
    //{
    //    m_eventSet = set as Transform;
    //    m_data.m_eventSet = m_eventSet.name;
    //}
}
#endif