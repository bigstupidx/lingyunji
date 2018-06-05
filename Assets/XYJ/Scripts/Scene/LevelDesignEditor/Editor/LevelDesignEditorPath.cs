using UnityEngine;
using UnityEditor;
using EditorExtensions;
using System.Collections.Generic;

public partial class LevelDesignEditor
{
    /// <summary>
    /// 绘制路径层次面板
    /// </summary>
    public void DrawPathHierarchy()
    {
        //创建每一个逻辑下的区域集
        for (int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
        {
            LevelDesignConfig.LevelLogicData logicData = m_curEditConfig.m_levelLogicList[i];
            Transform logicTrans = GetLogicTrans(logicData.m_name);
            Transform pathParent = FindNode("路径", logicTrans).transform;

            List<LevelDesignConfig.LevelPathData> pathList;
            if (LogicHasPath(logicData, out pathList))
            {
                if (null != logicTrans)
                {
                    for (int j = 0; j < pathList.Count; ++j)
                    {
                        GameObject pathObj = CreateLevelDesignObj("LevelDesignPath", pathParent);
                        pathObj.name = pathList[j].m_pathId;
                        LevelDesignPath levelDesignPath = pathObj.AddMissingComponent<LevelDesignPath>();
                        levelDesignPath.SetData(pathList[j]);
                        //设置每一个点
                        for (int k = 0; k < pathList[j].m_names.Count; ++k)
                        {
                            GameObject go = LevelDesignTool.CreateNode(pathObj.transform);
                            DestroyImmediate(go.GetComponent<Renderer>());
                            DestroyImmediate(go.GetComponent<MeshFilter>());
                            DestroyImmediate(go.GetComponent<SphereCollider>());

                            go.transform.position = pathList[j].m_postions[k]; ;
                            go.transform.rotation = Quaternion.Euler(pathList[j].m_dirs[k]);
                            go.name = pathList[j].m_names[k];

                            LevelDesignPathItem item = go.AddMissingComponent<LevelDesignPathItem>();
                            levelDesignPath.SetEvent(go, pathList[j].m_events[k]);
                            levelDesignPath.SetStayTime(go, pathList[j].m_stayTimes[k]);
                            levelDesignPath.SetSpeed(go, pathList[j].m_speeds.Count > k ? pathList[j].m_speeds[k] : 0);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 对应的logic是否有路径的资源
    /// </summary>
    /// <param name="logicData"></param>
    /// <returns></returns>
    public bool LogicHasPath(LevelDesignConfig.LevelLogicData logicData, out List<LevelDesignConfig.LevelPathData> list)
    {
        list = GetPathList(logicData);
        if (list != null)
        {
            return true;
        }
        return false;
    }

    public List<LevelDesignConfig.LevelPathData> GetPathList(LevelDesignConfig.LevelLogicData logicData)
    {
        if (HasLogic())
        {
            if (logicData != null && logicData.m_levelPathList != null && logicData.m_levelPathList.Count > 0)
            {
                return logicData.m_levelPathList;
            }
        }
        return null;
    }

    /// <summary>
    /// 绘制路径
    /// </summary>
    public void DrawPath()
    {
        GUILayout.Label("路径");
        LevelDesignConfig.LevelLogicData curLogicData = GetCurSelectData();
        if (curLogicData != null)
        {
            List<LevelDesignConfig.LevelPathData> pathList;
            if (LogicHasPath(curLogicData, out pathList))
            {
                for (int i = 0; i < pathList.Count;)
                {
                    LevelDesignConfig.LevelPathData data = pathList[i];
                    using (new AutoEditorHorizontal())
                    {
                        EditorGUILayout.LabelField("路径id: " + data.m_pathId);

                        //选中
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.PrefabNormal_Icon), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            GameObject obj = GetPathObj(data.m_pathId);
                            if (null != obj)
                            {
                                Selection.activeGameObject = obj;
                            }
                        }
                        {
                            ++i;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 增加路径
    /// </summary>
    public bool AddPath(LevelDesignConfig.LevelPathData data, LevelDesignConfig.LevelLogicData logicData)
    {
        if (null != logicData)
        {
            if (!logicData.m_levelPathList.Contains(data))
            {
                logicData.m_levelPathList.Add(data);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 设置区域对象
    /// </summary>
    public void SetPathData(LevelDesignConfig.LevelLogicData logic = null)
    {
        LevelDesignConfig.LevelLogicData logicData = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logicData.m_name);
        Transform pathParent = FindNode("路径", logicTrans).transform;
        logicData.m_levelPathList.Clear();
        for (int i = 0; i < pathParent.childCount; ++i)
        {
            Transform child = pathParent.GetChild(i);
            LevelDesignPath path = child.GetComponent<LevelDesignPath>();
            if (path != null)
            {
                path.CollectData();
                LevelDesignConfig.LevelPathData data = path.m_data;
                logicData.m_levelPathList.Add(data);
            }
        }
    }

    public GameObject GetPathObj(string pathId, LevelDesignConfig.LevelLogicData logic = null)
    {
        logic = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(GetCurSelectData().m_name);
        Transform pathParent = FindNode("路径", logicTrans).transform;
        Transform trans = pathParent.Find(pathId);
        if (null != trans)
            return trans.gameObject;
        return null;
    }
}
