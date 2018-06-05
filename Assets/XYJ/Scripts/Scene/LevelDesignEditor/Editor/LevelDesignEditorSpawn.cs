using UnityEngine;
using UnityEditor;
using EditorExtensions;
using System.Collections.Generic;

public partial class LevelDesignEditor
{
    /// <summary>
    /// 创建层级面板的刷怪器对象,创建出所有对象
    /// </summary>
    public void DrawSpawnHierarchy()
    {
        if(HasLogic())
        {
            //创建每一个逻辑下的刷新器
            for (int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
            {
                LevelDesignConfig.LevelLogicData logicData = m_curEditConfig.m_levelLogicList[i];
                Transform logicTrans = GetLogicTrans(logicData.m_name);
                Transform spawnParent = FindNode("刷新点", logicTrans).transform;

                List<LevelDesignConfig.LevelSpawnData> spawnList;
                if(LogicHasSpawn(logicData, out spawnList))
                {
                    //当前逻辑下包含刷新器
                    if (null != logicTrans)
                    {
                        for (int j =0; j < spawnList.Count; ++j)
                        {
                            GameObject spawnObj = CreateLevelDesignObj("LevelDesignSpawn", spawnParent);
                            spawnObj.name = spawnList[j].m_spawnId;
                            spawnObj.AddMissingComponent<LevelDesignSpawn>().SetData(spawnList[j]);

                            //创建点集
                            for (int k = 0; k < spawnList[j].m_names.Count; ++k)
                            {
                                Vector3 pos = spawnList[j].m_postions[k];
                                Vector3 dir = spawnList[j].m_dirs[k];
                                Vector3 scale = spawnList[j].m_scales.Count > k ? spawnList[j].m_scales[k] : Vector3.one;
                                string name = spawnList[j].m_names[k];

                                GameObject go = LevelDesignTool.CreateNode(spawnObj.transform);
                                go.transform.position = pos;
                                go.transform.eulerAngles = dir;
                                go.transform.localScale = scale;
                                go.name = name;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 对应的logic是否有刷新器的资源
    /// </summary>
    /// <param name="logicData"></param>
    /// <returns></returns>
    public bool LogicHasSpawn(LevelDesignConfig.LevelLogicData logicData, out List<LevelDesignConfig.LevelSpawnData> list)
    {
        list = GetSpawnList(logicData);
        if (list != null)
        {
            return true;
        }
        return false;
    }

    public List<LevelDesignConfig.LevelSpawnData> GetSpawnList(LevelDesignConfig.LevelLogicData logicData)
    {
        if (HasLogic())
        {
            if (logicData != null && logicData.m_levelSpawnList != null && logicData.m_levelSpawnList.Count > 0)
            {
                return logicData.m_levelSpawnList;
            }
        }
        return null;
    }

    /// <summary>
    /// 绘制刷新器
    /// </summary>
    public void DrawSpawn()
    {
        GUILayout.Label("刷新点");
        LevelDesignConfig.LevelLogicData curLogicData = GetCurSelectData();
        if(null != curLogicData)
        {
            List<LevelDesignConfig.LevelSpawnData> list;
            if(LogicHasSpawn(curLogicData, out list))
            {
                //EditorGUI.BeginDisabledGroup(true);
                for (int i = 0; i < list.Count;)
                {
                    LevelDesignConfig.LevelSpawnData data = list[i];
                    using (new AutoEditorHorizontal())
                    {
                        //显示名字
                        EditorGUILayout.LabelField("刷新点id: " + data.m_spawnId);
                        EditorGUILayout.LabelField("刷新点名称: " + data.m_name);

                        //选中
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.PrefabNormal_Icon), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            GameObject obj = GetSpawnObj(data.m_spawnId, curLogicData);
                            if(null != obj)
                            {
                                Selection.activeGameObject = obj;
                            }
                        }

                        //删除
                        //if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        //{
                        //    GameObject obj = GetSpawnObj(data.m_spawnId);
                        //    if (null != obj)
                        //    {
                        //        DestroyImmediate(obj);
                        //        MinusSpawn(data);
                        //    }
                        //}
                        //else
                        {
                            ++i;
                        }
                    }
                }
                //EditorGUI.EndDisabledGroup();
            }
        }
    }

    /// <summary>
    /// 增加刷新器,需要制定增加的地方
    /// </summary>
    public bool AddSpawn(LevelDesignConfig.LevelSpawnData data, LevelDesignConfig.LevelLogicData logicData)
    {
        if (null != logicData)
        {
            if(!logicData.m_levelSpawnList.Contains(data))
            {
                logicData.m_levelSpawnList.Add(data);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 删除刷新器
    /// </summary>
    public void MinusSpawn(LevelDesignConfig.LevelSpawnData data)
    {
        LevelDesignConfig.LevelLogicData curLogicData = GetCurSelectData();
        if (null != curLogicData)
        {
            if (curLogicData.m_levelSpawnList.Contains(data))
            {
                curLogicData.m_levelSpawnList.Remove(data);
            }
        }
    }

    /// <summary>
    /// 设置刷新点对象
    /// </summary>
    public void SetSpawnData(LevelDesignConfig.LevelLogicData logic = null)
    {
        LevelDesignConfig.LevelLogicData logicData = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logicData.m_name);
        Transform spawnParent = FindNode("刷新点", logicTrans).transform;
        logicData.m_levelSpawnList.Clear();
        for (int i = 0; i < spawnParent.childCount; ++i)
        {
            Transform child = spawnParent.GetChild(i);
            LevelDesignSpawn spawn = child.GetComponent<LevelDesignSpawn>();
            if (spawn != null)
            {
                spawn.CollectData();
                LevelDesignConfig.LevelSpawnData data = spawn.m_data;
                logicData.m_levelSpawnList.Add(data);
            }
        }
    }

    public GameObject GetSpawnObj(string spawnId, LevelDesignConfig.LevelLogicData logic)
    {
        logic = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logic.m_name);
        Transform spawnParent = FindNode("刷新点", logicTrans).transform;
        Transform trans = spawnParent.Find(spawnId);
        if (null != trans)
            return trans.gameObject;
        return null;
    }
}
