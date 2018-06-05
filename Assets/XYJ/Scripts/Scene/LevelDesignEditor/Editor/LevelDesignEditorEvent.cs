using UnityEngine;
using UnityEditor;
using EditorExtensions;
using System.Collections.Generic;

public partial class LevelDesignEditor
{
    /// <summary>
    /// 绘制事件的层次面板，事件由全局事件和房间内事件组成
    /// </summary>
    public void DrawOverallEventHierarchy()
    {
        //绘制全局事件
        GameObject logicParent = RootFind(GetCurEditConfigName());
        GameObject eventParent = FindNode("全局事件", logicParent.transform);
        if (HasOverallEvent())
        {
            for (int i = 0; i < m_curEditConfig.m_overalEventList.Count; ++i)
            {
                LevelDesignConfig.LevelEventObjData eventData = m_curEditConfig.m_overalEventList[i];
                GameObject eventObj = CreateLevelDesignObj("LevelDesignEventObj", eventParent.transform);
                eventObj.name = eventData.m_eventId;
                eventObj.AddMissingComponent<LevelDesignEventObj>().SetData(eventData);
            }
        }
    }

    public void DrawEventHierarchy()
    {
        //绘制每一个逻辑下的事件
        for (int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
        {
            LevelDesignConfig.LevelLogicData logicData = m_curEditConfig.m_levelLogicList[i];
            Transform logicTrans = GetLogicTrans(logicData.m_name);
            Transform parent = FindNode("局部事件", logicTrans).transform;

            List<LevelDesignConfig.LevelEventObjData> eventList;
            if (LogicHasEvent(logicData, out eventList))
            {
                if (null != logicTrans)
                {
                    for (int j = 0; j < eventList.Count; ++j)
                    {
                        GameObject eventObj = CreateLevelDesignObj("LevelDesignEventObj", parent);
                        eventObj.name = eventList[j].m_eventId;
                        eventObj.AddMissingComponent<LevelDesignEventObj>().SetData(eventList[j]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 事件也是全局的
    /// </summary>
    /// <returns></returns>
    public bool HasOverallEvent()
    {
        if (m_curEditConfig.m_overalEventList != null && m_curEditConfig.m_overalEventList.Count > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 对应的logic是否有事件
    /// </summary>
    /// <param name="logicData"></param>
    /// <returns></returns>
    public bool LogicHasEvent(LevelDesignConfig.LevelLogicData logicData, out List<LevelDesignConfig.LevelEventObjData> list)
    {
        list = GetEventList(logicData);
        if (list != null)
        {
            return true;
        }
        return false;
    }

    public List<LevelDesignConfig.LevelEventObjData> GetEventList(LevelDesignConfig.LevelLogicData logicData)
    {
        if (HasLogic())
        {
            if (logicData != null && logicData.m_roomEventList != null && logicData.m_roomEventList.Count > 0)
            {
                return logicData.m_roomEventList;
            }
        }
        return null;
    }

    /// <summary>
    /// 绘制全局事件
    /// </summary>
    public void DrawOverallEvent()
    {
        GUILayout.Label("全局事件");
        if(HasOverallEvent())
        {
            for(int i = 0; i < m_curEditConfig.m_overalEventList.Count;)
            {
                LevelDesignConfig.LevelEventObjData data = m_curEditConfig.m_overalEventList[i];
                using (new AutoEditorHorizontal())
                {
                    EditorGUILayout.LabelField("事件id: " + data.m_eventId);

                    //选中
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.PrefabNormal_Icon), EditorStyles.toolbarButton, GUILayout.Width(50)))
                    {
                        GameObject obj = GetOverallEventObj(data.m_eventId);
                        if (null != obj)
                        {
                            Selection.activeGameObject = obj;
                        }
                    }

                    //删除
                    //if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(50)))
                    //{
                    //    GameObject obj = GetOverallEventObj(data.m_eventId);
                    //    if (null != obj)
                    //    {
                    //        DestroyImmediate(obj);
                    //        MinusOverallEvent(data);
                    //    }
                    //}
                    //else
                    {
                        ++i;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 绘制局部事件
    /// </summary>
    public void DrawEvent()
    {
        GUILayout.Label("局部事件");
        LevelDesignConfig.LevelLogicData logic = GetCurSelectData();
        if (logic != null)
        {
            List<LevelDesignConfig.LevelEventObjData> eventList;
            if (LogicHasEvent(logic, out eventList))
            {
                for(int i = 0; i < eventList.Count;)
                {
                    LevelDesignConfig.LevelEventObjData data = eventList[i];
                    using (new AutoEditorHorizontal())
                    {
                        EditorGUILayout.LabelField("事件id: " + data.m_eventId);

                        //选中
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.PrefabNormal_Icon), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            GameObject obj = GetEventObj(data.m_eventId, logic);
                            if (null != obj)
                            {
                                Selection.activeGameObject = obj;
                            }
                        }

                        //删除
                        //if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        //{
                        //    GameObject obj = GetEventObj(data.m_eventId);
                        //    if (null != obj)
                        //    {
                        //        DestroyImmediate(obj);
                        //        MinusEventObj(data);
                        //    }
                        //}
                        //else
                        {
                            ++i;
                        }
                    }
                }
            }
        }
    } 

    /// <summary>
    /// 增加全局事件
    /// </summary>
    public bool AddOverallEvent(LevelDesignConfig.LevelEventObjData data)
    {
        if (!m_curEditConfig.m_overalEventList.Contains(data))
        {
            m_curEditConfig.m_overalEventList.Add(data);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 删除全局事件集
    /// </summary>
    public void MinusOverallEvent(LevelDesignConfig.LevelEventObjData data)
    {
        if (m_curEditConfig.m_overalEventList.Contains(data))
        {
            m_curEditConfig.m_overalEventList.Remove(data);
        }
    }

    /// <summary>
    /// 获取全局对象
    /// </summary>
    public void SetOverallEventData()
    {
        GameObject logicParent = RootFind(GetCurEditConfigName());
        GameObject eventParent = FindNode("全局事件", logicParent.transform);
        m_curEditConfig.m_overalEventList.Clear();

        for(int i = 0; i < eventParent.transform.childCount; ++i)
        {
            Transform child = eventParent.transform.GetChild(i);
            if(child.GetComponent<LevelDesignEventObj>() != null)
            {
                LevelDesignConfig.LevelEventObjData data = child.GetComponent<LevelDesignEventObj>().m_data;
                m_curEditConfig.m_overalEventList.Add(data);
            }
        }
    }

    public GameObject GetOverallEventObj(string eventId)
    {
        Transform eventTrans = GetOverallEventTrans(eventId);
        if (null != eventTrans)
            return eventTrans.gameObject;
        return null;
    }

    public GameObject GetEventObj(string eventId, LevelDesignConfig.LevelLogicData logic)
    {
        logic = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logic.m_name);
        Transform eventParent = FindNode("局部事件", logicTrans).transform;
        Transform trans = eventParent.Find(eventId);
        if (null != trans)
            return trans.gameObject;
        return null;
    }

    /// <summary>
    /// 添加事件对象
    /// </summary>
    /// <param name="data"></param>
    /// <param name="eventData"></param>
    public bool AddEventObj(LevelDesignConfig.LevelEventObjData data, LevelDesignConfig.LevelLogicData logicData)
    {
        if(null != logicData && data != null)
        {
            if(!logicData.m_roomEventList.Contains(data))
            {
                logicData.m_roomEventList.Add(data);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 删除事件对象
    /// </summary>
    /// <param name="data"></param>
    public void MinusEventObj(LevelDesignConfig.LevelEventObjData data)
    {
        LevelDesignConfig.LevelLogicData curLogicData = GetCurSelectData();
        if (null != curLogicData)
        {
            if (curLogicData.m_roomEventList.Contains(data))
            {
                curLogicData.m_roomEventList.Remove(data);
            }
        }
    }

    /// <summary>
    /// 设置事件对象
    /// </summary>
    public void SetEventData(LevelDesignConfig.LevelLogicData logic = null)
    {
        LevelDesignConfig.LevelLogicData logicData = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logicData.m_name);
        Transform eventParent = FindNode("局部事件", logicTrans).transform;
        logicData.m_roomEventList.Clear();
        for (int i = 0; i < eventParent.childCount; ++i)
        {
            Transform child = eventParent.GetChild(i);
            if (child.GetComponent<LevelDesignEventObj>())
            {
                LevelDesignConfig.LevelEventObjData data = child.GetComponent<LevelDesignEventObj>().m_data;
                logicData.m_roomEventList.Add(data);
            }
        }
    }
}
