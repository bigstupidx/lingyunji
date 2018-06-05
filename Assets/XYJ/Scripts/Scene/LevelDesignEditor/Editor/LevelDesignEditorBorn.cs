using UnityEngine;
using UnityEditor;
using EditorExtensions;
using System.Collections.Generic;

public partial class LevelDesignEditor
{
    /// <summary>
    /// 刷新层次面板的出生点对象
    /// </summary>
    public void DrawBornHierarchy()
    {
        //创建每一个逻辑下的出生点
        for(int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
        {
            LevelDesignConfig.LevelLogicData logicData = m_curEditConfig.m_levelLogicList[i];
            Transform logicTrans = GetLogicTrans(logicData.m_name);
            Transform bornParent = FindNode("出生点", logicTrans).transform;

            List<LevelDesignConfig.LevelBornData> bornList;
            if(LogicHasBorn(logicData, out bornList))
            {
                if (null != logicTrans)
                {
                    for (int j = 0; j < bornList.Count; ++j)
                    {
                        GameObject bornObj = CreateLevelDesignObj("LevelDesignBorn", bornParent);
                        bornObj.name = bornList[j].m_bornId;
                        bornObj.AddMissingComponent<LevelDesignBorn>().SetData(bornList[j]);
                        bornObj.transform.position = bornList[j].m_pos;
                        bornObj.transform.eulerAngles = bornList[j].m_dir;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 对应的logic是否有出生点的资源
    /// </summary>
    /// <param name="logicData"></param>
    /// <returns></returns>
    public bool LogicHasBorn(LevelDesignConfig.LevelLogicData logicData, out List<LevelDesignConfig.LevelBornData> list)
    {
        list = GetBornList(logicData);
        if (list != null)
        {
            return true;
        }
        return false;
    }

    public List<LevelDesignConfig.LevelBornData> GetBornList(LevelDesignConfig.LevelLogicData logicData)
    {
        if (HasLogic())
        {
            if (logicData != null && logicData.m_levelBornList != null && logicData.m_levelBornList.Count > 0)
            {
                return logicData.m_levelBornList;
            }
        }
        return null;
    }

    /// <summary>
    /// 绘制出生点
    /// </summary>
    public void DrawBorn()
    {
        GUILayout.Label("出生点");
        LevelDesignConfig.LevelLogicData logic = GetCurSelectData();
        if(logic != null)
        {
            List<LevelDesignConfig.LevelBornData> bornList;
            if (LogicHasBorn(logic, out bornList))
            {
                for(int i = 0; i < bornList.Count;)
                {
                    LevelDesignConfig.LevelBornData data = bornList[i];
                    using (new AutoEditorHorizontal())
                    {
                        EditorGUILayout.LabelField("出生点id: " + data.m_bornId);

                        //选中
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.PrefabNormal_Icon), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        {
                            GameObject obj = GetBornObj(data.m_bornId, logic);
                            if (null != obj)
                            {
                                Selection.activeGameObject = obj;
                            }
                        }

                        //删除
                        //if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(50)))
                        //{
                        //    GameObject obj = GetBornObj(data.m_bornId);
                        //    if (null != obj)
                        //    {
                        //        DestroyImmediate(obj);
                        //        MinusBorn(data);
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
    /// 增加出生点
    /// </summary>
    public bool AddBorn(LevelDesignConfig.LevelBornData data, LevelDesignConfig.LevelLogicData logicData)
    {
        if (null != logicData)
        {
            if (!logicData.m_levelBornList.Contains(data))
            {
                logicData.m_levelBornList.Add(data);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 删除出生点
    /// </summary>
    public void MinusBorn(LevelDesignConfig.LevelBornData data)
    {
        LevelDesignConfig.LevelLogicData curLogicData = GetCurSelectData();
        if (null != curLogicData)
        {
            if (curLogicData.m_levelBornList.Contains(data))
            {
                curLogicData.m_levelBornList.Remove(data);
            }
        }
    }

    /// <summary>
    /// 设置出生点对象
    /// </summary>
    public void SetBornData(LevelDesignConfig.LevelLogicData logic = null)
    {
        LevelDesignConfig.LevelLogicData logicData = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logicData.m_name);
        Transform bornParent = FindNode("出生点", logicTrans).transform;
        logicData.m_levelBornList.Clear();
        for (int i = 0; i < bornParent.childCount; ++i)
        {
            Transform child = bornParent.GetChild(i);
            if(child.GetComponent<LevelDesignBorn>())
            {
                LevelDesignConfig.LevelBornData data = child.GetComponent<LevelDesignBorn>().m_data;
                data.m_bornId = child.name;
                data.m_pos = child.position;
                data.m_dir = child.eulerAngles;

                logicData.m_levelBornList.Add(data);
            }
        }
    }

    /// <summary>
    /// 获取对应的出生点对象
    /// </summary>
    /// <param name="bornId"></param>
    /// <returns></returns>
    public GameObject GetBornObj(string bornId, LevelDesignConfig.LevelLogicData logic)
    {
        logic = logic == null ? GetCurSelectData() : logic;
        Transform logicTrans = GetLogicTrans(logic.m_name);
        Transform spawnParent = FindNode("出生点", logicTrans).transform;
        Transform trans = spawnParent.Find(bornId);
        if (null != trans)
            return trans.gameObject;
        return null;
    }
}
