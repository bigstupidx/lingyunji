using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 每个blendshape值范围是0-100
/// </summary>
public class RoleShapeHandle
{

    SkinnedMeshRenderer m_faceMesh;// 模型
    ModelPartManage m_modelManager;

    RoleShapeConfig m_config;// 外形配置，只读
    RoleShapePartData m_data;// 自定义数据
    RoleShapePartData m_defaultData;// 基准脸的默认数据,只读

    /// <summary>
    /// 只读
    /// </summary>
    /// <returns></returns>
    public RoleShapeConfig GetConfig()
    {
        return m_config;
    }

    /// <summary>
    /// 从模型获取数据
    /// </summary>
    /// <returns></returns>
    public RoleShapePartData GetData()
    {
        if (m_faceMesh != null)
            GetValues();

        return m_data;
    }

    [System.Obsolete("废弃方法，用InitInfo和InitModel替换")]
    public void InitConfig (SkinnedMeshRenderer renderer, RoleShapeConfig config)
    {
        m_faceMesh = renderer;
        m_config = config;
        m_config.InitKeys();
        m_data = new RoleShapePartData();
        m_defaultData = m_data;
    }

    /// <summary>
    /// 初始化配置和数据
    /// </summary>
    /// <param name="config"></param>
    /// <param name="data"></param>
    /// <param name="defaultData"></param>
    public void InitInfo(RoleShapeConfig config, RoleShapePartData data, RoleShapePartData defaultData)
    {
        m_config = config;
        m_config.InitKeys();
        m_data = data;
        if (m_data==null)
            m_data = new RoleShapePartData();

        if (m_defaultData == null)
            m_defaultData = new RoleShapePartData();
        m_defaultData.Clone(defaultData);
    }

    /// <summary>
    /// 初始化模型
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="model"></param>
    public void InitModel(SkinnedMeshRenderer renderer, ModelPartManage model)
    {
        m_faceMesh = renderer;
        m_modelManager = model;

        SetValues(m_data);
    }

    // 默认数据
    public void SetDefaultData(RoleShapePartData data, bool withRefresh = true)
    {
        m_defaultData = data;
        m_data.Clone(m_defaultData);

        if (withRefresh)
            SetValues (m_defaultData);
    }

    // 自定义数据
    public void SetData(RoleShapePartData data, bool withRefresh = true)
    {
        m_data.Clone(data);

        if (withRefresh)
            SetValues(m_data);
    }

    /// <summary>
    /// 设置基准脸配置
    /// </summary>
    /// <param name="index"></param>
    public void SetBaseFace(int index, bool setScale=false)
    {
        if (m_config.baseFaces.Count>0)
        {
            List<string> faceTypeKeys = m_config.GetFaceTypeKeys();
            for (int i = 0; i < faceTypeKeys.Count; ++i)
            {
                string key = faceTypeKeys[i];
                if (i == index)
                {
                    SetValue(key, 100);// 设置基准脸值
                    // 设置身高
                    if (setScale && m_modelManager != null)
                        m_modelManager.SetModelScale(m_config.baseFaces[i].scale);
                }
                else
                {
                    SetValue(key, 0);
                }
            }
        }
    }

    public float GetModelScale(int index)
    {
        if (m_config.baseFaces.Count > 0)
        {
            for (int i = 0; i < m_config.baseFaces.Count; ++i)
            {
                if (index == i)
                {
                    return m_config.baseFaces[i].scale;
                }
            }
        }
        return 1f;
    }
    

    /// <summary>
    /// 重置所有外观值
    /// </summary>
    public void Reset()
    {
        m_data.Clone(m_defaultData);
        SetValues(m_defaultData);
    }

    /// <summary>
    /// 重置某个部位
    /// </summary>
    public void ResetPart(int index)
    {
        if (m_defaultData == null || m_config == null)
            return;

        if (index >= 0 && index < m_config.faceParts.Count)
        {
            RoleShapePart part = m_config.faceParts[index];
            string key = string.Empty;
            float value = 0.0f;
            for (int j = 0; j < part.subParts.Count; ++j)
            {
                RoleShapeSubPart subPart = part.subParts[j];
                for (int k = 0; k < subPart.units.Count; ++k)
                {
                    // key0
                    key = subPart.units[k].GetKey0();
                    if (!string.IsNullOrEmpty(key))
                    {
                        value = m_defaultData.GetKeyValue(key);
                        SetValue(key, value);
                    }

                    // key1
                    key = subPart.units[k].GetKey1();
                    if (!string.IsNullOrEmpty(key))
                    {
                        value = m_defaultData.GetKeyValue(key);
                        SetValue(key, value);
                    }

                }
            }
        }
    }

    #region UI调用的方法

    public float GetValue01(RoleShapePartUnit unit)
    {
        if (string.IsNullOrEmpty(unit.key0) || string.IsNullOrEmpty(unit.key1))
        {

            string keyName = unit.GetKey0();
            if (string.IsNullOrEmpty(keyName))
                keyName = unit.GetKey1();
            float value = GetValue(keyName);
            return Mathf.Clamp(value / 100.0f, 0.0f, 1.0f);
        }

        // 两个参数的值
        string leftName = unit.GetKey0();
        int leftIdx = m_faceMesh.sharedMesh.GetBlendShapeIndex(leftName);
        if (leftIdx == -1)
        {
            Debug.LogError("模型找不到脸型上对应的blendShapes:" + leftName);
            return 0.5f;
        }

        string rightName = unit.GetKey1();
        int rightIdx = m_faceMesh.sharedMesh.GetBlendShapeIndex(rightName);
        if (rightIdx == -1)
        {
            Debug.LogError("模型找不到脸型上对应的blendShapes:" + rightName);
            return 0.5f;
        }

        var leftWeight = m_faceMesh.GetBlendShapeWeight(leftIdx);
        if (leftWeight > 0)
            return 0.5f - Mathf.Clamp(leftWeight / 200f, 0f, 0.5f);

        return 0.5f + Mathf.Clamp(m_faceMesh.GetBlendShapeWeight(rightIdx) / 200f, 0f, 0.5f);
    }

    public void SetValue01(RoleShapePartUnit unit, float value)
    {
        if (string.IsNullOrEmpty(unit.key0) || string.IsNullOrEmpty(unit.key1))
        {

            string keyName = unit.GetKey0();
            if (string.IsNullOrEmpty(keyName))
                keyName = unit.GetKey1();

            SetValue(keyName, value * 100.0f);
            return;
        }
        value = Mathf.Clamp01(value);
        if (value == 0.5f)
        {
            SetValue(unit.GetKey0(), 0);
            SetValue(unit.GetKey1(), 0);
        }
        else if (value < 0.5f)
        {
            SetValue(unit.GetKey0(), (0.5f - value) * 200f);
            SetValue(unit.GetKey1(), 0);
        }
        else
        {
            SetValue(unit.GetKey0(), 0);
            SetValue(unit.GetKey1(), (value - 0.5f) * 200f);
        }
    }

    #endregion

    /// <summary>
    /// 把Data值全设置到mesh上
    /// </summary>
    void SetValues(RoleShapePartData data)
    {
        if (data == null || m_config == null || m_config.faceParts.Count==0)
            return;

        string key = string.Empty;
        float value = 0.0f;
        List<string> configKeys = m_config.GetAllFaceKeys();
        for (int i = 0; i < configKeys.Count; ++i)
        {
            key = configKeys[i];
            value = m_data.GetKeyValue(key);
            SetValue(key, value);
        }
        //for (int i=0; i<m_config.faceParts.Count; ++i)
        //{
        //    RoleShapePart part = m_config.faceParts[i];
        //    for (int j=0; j<part.subParts.Count; ++j)
        //    {
        //        RoleShapeSubPart subPart = part.subParts[j];
        //        for (int k=0; k<subPart.units.Count; ++k)
        //        {
        //            // key0
        //            key = subPart.units[k].GetKey0();
        //            if (!string.IsNullOrEmpty(key))
        //            {
        //                value = data.GetKeyValue(key);
        //                SetValue(key, value);
        //            }

        //            // key1
        //            key = subPart.units[k].GetKey1();
        //            if (!string.IsNullOrEmpty(key))
        //            {
        //                value = data.GetKeyValue(key);
        //                SetValue(key, value);
        //            }
                    
        //        }
        //    }
        //}
    }

    /// <summary>
    /// 把mesh上的值都设置到Data
    /// </summary>
    void GetValues()
    {
        if (m_data == null || m_config == null || m_config.faceParts.Count==0)
            return;

        string key = string.Empty;
        float value = 0.0f;
        List<string> configKeys = m_config.GetAllFaceKeys();
        for (int i = 0; i < configKeys.Count; ++i)
        {
            key = configKeys[i];
            value = GetValue(key);
            m_data.SetKeyValue(key, value);
        }
        configKeys = m_config.GetFaceTypeKeys();
        for (int i = 0; i < configKeys.Count; ++i)
        {
            key = configKeys[i];
            value = GetValue(key);
            m_data.SetKeyValue(key, value);
        }
        //for (int i = 0; i < m_config.faceParts.Count; ++i)
        //{
        //    RoleShapePart part = m_config.faceParts[i];
        //    for (int j = 0; j < part.subParts.Count; ++j)
        //    {
        //        RoleShapeSubPart subPart = part.subParts[j];
        //        for (int k = 0; k < subPart.units.Count; ++k)
        //        {
        //            // key0
        //            key = subPart.units[k].GetKey0();
        //            if (!string.IsNullOrEmpty(key))
        //            {
        //                value = GetValue(key);
        //                m_data.SetKeyValue(key, value);
        //            }

        //            // key1
        //            key = subPart.units[k].GetKey1();
        //            if (!string.IsNullOrEmpty(key))
        //            {
        //                value = GetValue(key);
        //                m_data.SetKeyValue(key, value);
        //            }

        //        }
        //    }
        //}
    }

    float GetValue(string key)
    {
        if (string.IsNullOrEmpty(key))
            return 0.0f;
        int keyIndex = m_faceMesh.sharedMesh.GetBlendShapeIndex(key);
        if (keyIndex == -1)
        {
            Debug.LogError("模型找不到脸型上对应的blendShape:" + key);
            return 0.0f;
        }
        return m_faceMesh.GetBlendShapeWeight(keyIndex);
    }

    void SetValue(string key, float value)
    {
        if (string.IsNullOrEmpty(key))
            return;
        int keyIndex = m_faceMesh.sharedMesh.GetBlendShapeIndex(key);
        if (keyIndex == -1)
        {
            Debug.LogError("模型找不到脸型上对应的blendShape:" + key);
            return;
        }
        m_data.SetKeyValue(key, value);
        m_faceMesh.SetBlendShapeWeight(keyIndex, value);
    }

}
