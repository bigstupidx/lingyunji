using UnityEngine;
using UnityEditor;
using EditorExtensions;
using System.Collections.Generic;

public partial class LevelDesignEditor
{
    /// <summary>
    /// 绘制区域集的层次面板
    /// </summary>
    public void DrawAreaHierarchy()
    {
        //创建每一个逻辑下的区域集
        for (int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
        {
            LevelDesignConfig.LevelLogicData logicData = m_curEditConfig.m_levelLogicList[i];
            Transform logicTrans = GetLogicTrans(logicData.m_name);
            Transform pointParent = FindNode("区域集", logicTrans).transform;

            List<LevelDesignConfig.LevelAreaData> areaList;
            if (LogicHasArea(logicData, out areaList))
            {
                if (null != logicTrans)
                {
                    for (int j = 0; j < areaList.Count; ++j)
                    {
                        GameObject areaObj = CreateLevelDesignObj("LevelDesignArea", pointParent);
                        areaObj.name = areaList[j].m_areaSetId;
                        LevelDesignArea levelDesignArea = areaObj.AddMissingComponent<LevelDesignArea>();
                        levelDesignArea.SetData(areaList[j]);
                        if(!areaList[j].m_isInitOpen)
                        {
                            areaObj.SetActive(false);
                        }
                        //设置每一个点
                        for (int k = 0; k < areaList[j].m_names.Count; ++k)
                        {
                            LevelDesignConfig.LevelAreaData.AreaType areaType = areaList[j].m_types[k];

                            GameObject go = LevelDesignTool.CreateNode(areaObj.transform);
                            DestroyImmediate(go.GetComponent<Renderer>());
                            DestroyImmediate(go.GetComponent<MeshFilter>());

                            go.transform.position = areaList[j].m_postions[k]; ;
                            go.transform.rotation = Quaternion.Euler(areaList[j].m_dirs[k]);
                            go.transform.localScale = areaList[j].m_scales[k];
                            go.name = areaList[j].m_names[k]; ;
                            levelDesignArea.SetColliderType(go, (int)areaType);

                            if (areaType == LevelDesignConfig.LevelAreaData.AreaType.Rect)
                            {
                                BoxCollider collider = go.GetComponent<BoxCollider>();
                                collider.center = areaList[j].m_centers[k];
                                collider.size = areaList[j].m_sizes[k];
                            }
                            else
                            {
                                SphereCollider collider = go.GetComponent<SphereCollider>();
                                collider.center = areaList[j].m_centers[k];
                                collider.radius = areaList[j].m_radiuses[k];
                            }
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
    public bool LogicHasArea(LevelDesignConfig.LevelLogicData logicData, out List<LevelDesignConfig.LevelAreaData> list)
    {
        list = GetAreaList(logicData);
        if (list != null)
        {
            return true;
        }
        return false;
    }

    public List<LevelDesignConfig.LevelAreaData> GetAreaList(LevelDesignConfig.LevelLogicData logicData)
    {
        if (HasLogic())
        {
            if (logicData != null && logicData.m_levelAreaList != null && logicData.m_levelAreaList.Count > 0)
            {
                return logicData.m_levelAreaList;
            }
        }
        return null;
    }

    public void DrawArea()
    {
        GUILayout.Label("区域集");
        LevelDesignConfig.LevelLogicData curLogicData = GetCurSelectData();
        if (curLogicData != null)
        {
            List<LevelDesignConfig.LevelAreaData> areaList;
            if (LogicHasArea(curLogicData, out areaList))
            {
                for (int i = 0; i < areaList.Count;)
                {
                    LevelDesignConfig.LevelAreaData data = areaList[i];
                    using (new AutoEditorHorizontal())
                    {
                        EditorGUILayout.LabelField("区域集id: " + data.m_areaSetId);

                        //选中
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.PrefabNormal_Icon), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            GameObject obj = GetAreaObj(data.m_areaSetId);
                            if (null != obj)
                            {
                                Selection.activeGameObject = obj;
                            }
                        }

                        //删除
                        //if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        //{
                        //    GameObject obj = GetAreaObj(data.m_areaSetId);
                        //    if (null != obj)
                        //    {
                        //        DestroyImmediate(obj);
                        //        MinusArea(data);
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
    public bool AddArea(LevelDesignConfig.LevelAreaData data, LevelDesignConfig.LevelLogicData logicData)
    {
        if (null != logicData)
        {
            if (!logicData.m_levelAreaList.Contains(data))
            {
                logicData.m_levelAreaList.Add(data);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 删除点集
    /// </summary>
    public void MinusArea(LevelDesignConfig.LevelAreaData data)
    {
        LevelDesignConfig.LevelLogicData curLogicData = GetCurSelectData();
        if (null != curLogicData)
        {
            if (curLogicData.m_levelAreaList.Contains(data))
            {
                curLogicData.m_levelAreaList.Remove(data);
            }
        }
    }

    /// <summary>
    /// 设置区域对象
    /// </summary>
    public void SetAreaData(LevelDesignConfig.LevelLogicData logic = null)
    {
        LevelDesignConfig.LevelLogicData logicData = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logicData.m_name);
        Transform areaParent = FindNode("区域集", logicTrans).transform;
        logicData.m_levelAreaList.Clear();
        for (int i = 0; i < areaParent.childCount; ++i)
        {
            Transform child = areaParent.GetChild(i);
            LevelDesignArea area = child.GetComponent<LevelDesignArea>();
            if (area != null)
            {
                area.CollectData();
                LevelDesignConfig.LevelAreaData data = area.m_data;
                data.m_isInitOpen = child.gameObject.activeSelf;
                logicData.m_levelAreaList.Add(data);
            }
        }
    }

    public GameObject GetAreaObj(string areaId, LevelDesignConfig.LevelLogicData logic = null)
    {
        logic = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(GetCurSelectData().m_name);
        Transform areaParent = FindNode("区域集", logicTrans).transform;
        Transform trans = areaParent.Find(areaId);
        if (null != trans)
            return trans.gameObject;
        return null;
    }
}
