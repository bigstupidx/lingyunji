using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型部件配置信息
/// </summary>
public class ModelPartConfig {

    public ModelPartType m_type;

    public string m_modelName;

    public string m_boneName;

    public string m_controllerName;
    public RuntimeAnimatorController m_controller;// 临时是用引用模式关联，如果可以建议通过名字加载方式

    // Editor Values
    public bool m_editFoldout = false;

    public ModelPartConfig()
    {
        m_type = ModelPartType.Body;

        m_editFoldout = false;
    }
    public ModelPartConfig(ModelPartType type)
    {
        m_type = type;

        m_editFoldout = false;
    }
}

/// <summary>
/// 模型的配置数据
/// </summary>
public class ModelConfigData
{
    //部件数据
}
