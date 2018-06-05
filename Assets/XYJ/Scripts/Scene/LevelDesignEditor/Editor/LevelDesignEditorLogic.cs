using UnityEngine;
using UnityEditor;
using EditorExtensions;

public partial class LevelDesignEditor
{
    /// <summary>
    /// 创建层级面板对象
    /// </summary>
    public void DrawLogicHierarchy()
    {
        if(HasLogic())
        {
            for (int i = 0; i < m_curEditConfig.m_levelLogicList.Count; ++i)
            {
                GameObject logicParent = RootFind(GetCurEditConfigName());
                GameObject logicObj = CreateLevelDesignObj("LevelDesignLogic", logicParent.transform);
                logicObj.name = m_curEditConfig.m_levelLogicList[i].m_name;
                logicObj.AddMissingComponent<LevelDesignLogic>().SetData(m_curEditConfig.m_levelLogicList[i]);
            }
        }
    }

    /// <summary>
    /// 绘制逻辑相关
    /// </summary>
    public void DrawLogic()
    {
        using (new AutoEditorHorizontal())
        {
            GUILayout.Label("关卡逻辑");
            if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                //AddLogic();
                GameObject logicParent = RootFind(GetCurEditConfigName());
                GameObject logicObj = CreateLevelDesignObj("LevelDesignLogic", logicParent.transform);
                Selection.activeObject = logicObj;
                logicObj.AddMissingComponent<LevelDesignLogic>();
            }
        }
        if (HasLogic())
        {
            for(int i = 0; i < m_curEditConfig.m_levelLogicList.Count;)
            {
                LevelDesignConfig.LevelLogicData data = m_curEditConfig.m_levelLogicList[i];
                using (new AutoEditorHorizontal())
                {
                    //设置当前的逻辑
                    if(GUILayout.Button(IsCurData(data) ? EditorIconContent.GetSystem(EditorIconContentType.lightMeter_greenLight) : 
                        EditorIconContent.GetSystem(EditorIconContentType.lightMeter_lightRim), EditorStyles.toolbarButton, GUILayout.Width(50)))
                    {
                        m_curEditConfig.m_curSelectLogic = data.m_name;
                    }

                    //显示逻辑的名称
                    GUILayout.Label(data.m_name);

                    //选中
                    if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.PrefabNormal_Icon), EditorStyles.toolbarButton, GUILayout.Width(50)))
                    {
                        GameObject obj = GetLogicObj(data.m_name);
                        if (null != obj)
                        {
                            Selection.activeGameObject = obj;
                        }
                    }

                    //删除
                    //if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(50)))
                    //{
                    //    GameObject obj = GetLogicObj(data.m_name);
                    //    if (null != obj)
                    //    {
                    //        DestroyImmediate(obj);
                    //        MinusLogic(data);
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
    /// 当前是否有逻辑
    /// </summary>
    /// <returns></returns>
    public bool HasLogic()
    {
        if(m_curEditConfig.m_levelLogicList != null && m_curEditConfig.m_levelLogicList.Count > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 增加逻辑
    /// </summary>
    public void AddLogic(LevelDesignConfig.LevelLogicData data)
    {
        if (!m_curEditConfig.m_levelLogicList.Contains(data))
        {
            m_curEditConfig.m_levelLogicList.Add(data);

            //绘制出生点
            DrawBornHierarchy();
            //绘制刷新器
            DrawSpawnHierarchy();
            //绘制点集
            DrawPointHierarchy();
            //绘制区域集
            DrawAreaHierarchy();
            //绘制路径
            DrawPathHierarchy();
            //绘制事件
            DrawEventHierarchy();
        }

        if (m_curEditConfig.m_levelLogicList.Count == 1)
        {
            m_curEditConfig.m_curSelectLogic = data.m_name;
        }
    }

    /// <summary>
    /// 删除一个逻辑
    /// </summary>
    public void MinusLogic(LevelDesignConfig.LevelLogicData data)
    {
        if (m_curEditConfig.m_levelLogicList.Contains(data))
        {
            m_curEditConfig.m_levelLogicList.Remove(data);
        }
    }

    /// <summary>
    /// 设置房间逻辑的数据
    /// </summary>
    public void SetLogicData()
    {
        GameObject logicParent = RootFind(GetCurEditConfigName());
        m_curEditConfig.m_levelLogicList.Clear();
        for(int i = 0; i < logicParent.transform.childCount; ++i)
        {
            Transform child = logicParent.transform.GetChild(i);
            if(child.GetComponent<LevelDesignLogic>() != null)
            {
                LevelDesignConfig.LevelLogicData data = child.GetComponent<LevelDesignLogic>().m_data;
                m_curEditConfig.m_levelLogicList.Add(data);
            }
        }

        if (m_curEditConfig.m_levelLogicList.Count == 1)
        {
            m_curEditConfig.m_curSelectLogic = m_curEditConfig.m_levelLogicList[0].m_name;
        }
    }

    /// <summary>
    /// 判断是否是当前选中的逻辑
    /// </summary>
    /// <param name="logic"></param>
    /// <returns></returns>
    public bool IsCurData(LevelDesignConfig.LevelLogicData logic)
    {
        if(m_curEditConfig.m_curSelectLogic != null)
        {
            return m_curEditConfig.m_curSelectLogic.Equals(logic.m_name);
        }
        return false;
    }
    
    /// <summary>
    /// 判断是否有重名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsLogicNameContain(string name)
    {
        return false;
    }

    public GameObject GetLogicObj(string name)
    {
        Transform logicTrans = GetLogicTrans(name);
        if (null != logicTrans)
            return logicTrans.gameObject;
        return null;
    }
}
