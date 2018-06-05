using UnityEngine;
using UnityEditor;
using EditorExtensions;
using System.Collections.Generic;

public partial class LevelDesignEditor
{
    /// <summary>
    /// 绘制点集的层次面板
    /// </summary>
    public void DrawPointHierarchy()
    {
        //创建每一个逻辑下的点集
        for (int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
        {
            LevelDesignConfig.LevelLogicData logicData = m_curEditConfig.m_levelLogicList[i];
            Transform logicTrans = GetLogicTrans(logicData.m_name);
            Transform pointParent = FindNode("点集", logicTrans).transform;

            List<LevelDesignConfig.LevelPointData> pointList;
            if (LogicHasPoint(logicData, out pointList))
            {
                if (null != logicTrans)
                {
                    for (int j = 0; j < pointList.Count; ++j)
                    {
                        GameObject pointObj = CreateLevelDesignObj("LevelDesignPoint", pointParent);
                        pointObj.name = pointList[j].m_pointSetId;
                        pointObj.AddMissingComponent<LevelDesignPoint>().SetData(pointList[j]);

                        //设置每一个点
                        for (int k = 0; k < pointList[j].m_names.Count; ++k)
                        {
                            Vector3 pos = pointList[j].m_postions[k];
                            Vector3 dir = pointList[j].m_dirs[k];
                            string name = pointList[j].m_names[k];

                            GameObject go = LevelDesignTool.CreateNode(pointObj.transform);
                            go.transform.position = pos;
                            go.transform.eulerAngles = dir;
                            go.name = name;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 对应的logic是否有点集的资源
    /// </summary>
    /// <param name="logicData"></param>
    /// <returns></returns>
    public bool LogicHasPoint(LevelDesignConfig.LevelLogicData logicData, out List<LevelDesignConfig.LevelPointData> list)
    {
        list = GetPointList(logicData);
        if (list != null)
        {
            return true;
        }
        return false;
    }

    public List<LevelDesignConfig.LevelPointData> GetPointList(LevelDesignConfig.LevelLogicData logicData)
    {
        if (HasLogic())
        {
            if (logicData != null && logicData.m_levelPointList != null && logicData.m_levelPointList.Count > 0)
            {
                return logicData.m_levelPointList;
            }
        }
        return null;
    }

    /// <summary>
    /// 绘制点集
    /// </summary>
    public void DrawPoint()
    {
        GUILayout.Label("点集");
        LevelDesignConfig.LevelLogicData logic = GetCurSelectData();
        if (logic != null)
        {
            List<LevelDesignConfig.LevelPointData> pointList;
            if (LogicHasPoint(logic, out pointList))
            {
                for (int i = 0; i < pointList.Count;)
                {
                    LevelDesignConfig.LevelPointData data = pointList[i];
                    using (new AutoEditorHorizontal())
                    {
                        EditorGUILayout.LabelField("点集id: " + data.m_pointSetId);

                        //选中
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.PrefabNormal_Icon), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            GameObject obj = GetPointObj(data.m_pointSetId, logic);
                            if (null != obj)
                            {
                                Selection.activeGameObject = obj;
                            }
                        }

                        //删除
                        //if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        //{
                        //    GameObject obj = GetPointObj(data.m_pointSetId);
                        //    if (null != obj)
                        //    {
                        //        DestroyImmediate(obj);
                        //        MinusPoint(data);
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
    /// 增加点集
    /// </summary>
    public bool AddPoint(LevelDesignConfig.LevelPointData data, LevelDesignConfig.LevelLogicData logicData)
    {
        if (null != logicData)
        {
            if (!logicData.m_levelPointList.Contains(data))
            {
                logicData.m_levelPointList.Add(data);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 设置点集对象
    /// </summary>
    public void SetPointData(LevelDesignConfig.LevelLogicData logic = null)
    {
        LevelDesignConfig.LevelLogicData logicData = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logicData.m_name);
        Transform pointParent = FindNode("点集", logicTrans).transform;
        logicData.m_levelPointList.Clear();
        for (int i = 0; i < pointParent.childCount; ++i)
        {
            Transform child = pointParent.GetChild(i);
            LevelDesignPoint point = child.GetComponent<LevelDesignPoint>();
            if (point != null)
            {
                point.CollectData();
                LevelDesignConfig.LevelPointData data = point.m_data;
                logicData.m_levelPointList.Add(data);
            }
        }
    }

    /// <summary>
    /// 删除点集
    /// </summary>
    public void MinusPoint(LevelDesignConfig.LevelPointData data)
    {
        LevelDesignConfig.LevelLogicData curLogicData = GetCurSelectData();
        if (null != curLogicData)
        {
            if (curLogicData.m_levelPointList.Contains(data))
            {
                curLogicData.m_levelPointList.Remove(data);
            }
        }
    }

    public GameObject GetPointObj(string pointId, LevelDesignConfig.LevelLogicData logic)
    {
        logic = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logic.m_name);
        Transform areaParent = FindNode("点集", logicTrans).transform;
        Transform trans = areaParent.Find(pointId);
        if (null != trans)
            return trans.gameObject;
        return null;
    }
}
