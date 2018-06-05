using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.json;

/// <summary>
/// 角色外形各个部位的设置数据，可编辑保存为配置
/// RoleShapeConfig用来指定保存数据的key值
/// GameObject是用来获取对应key值的数据
/// </summary>
[System.Serializable]
public class RoleShapeData
{

    public string keyName;// 键值名称
    public string roleId;// 角色id
    public long uuid = 0;// 数据的唯一id

    // 各个部位的外形数据
    public List<RoleShapePartData> shapeParts = new List<RoleShapePartData>();
}

/// <summary>
/// 外形的预设数据
/// </summary>
[System.Serializable]
public class RoleShapeDataPrefab : IJsonFile
{
    public string GetKey() { return fileKey; }
    public void SetKey(string key) { fileKey = key; }

    public string fileKey;// 文件key值

    public string describe;// 描述

    public RoleShapePartData shapeData;
}

/// <summary>
/// 每个形变值数据
/// </summary>
[System.Serializable]
public class RoleShapePartData
{
    public List<RoleShapeUnitData> partUnits = new List<RoleShapeUnitData>();

    /// <summary>
    /// 还原所有值
    /// </summary>
    public void ResetValues()
    {
        for (int i = 0; i < partUnits.Count; ++i)
        {
            partUnits[i].value = 0.0f;
        }
    }

    public void Clone(RoleShapePartData source)
    {
        for (int i = 0; i < source.partUnits.Count; ++i)
        {
            SetKeyValue(source.partUnits[i].key, source.partUnits[i].value);
        }
    }

    public void CloneWithConfig(RoleShapeConfig config, RoleShapePartData source)
    {
        List<string> configKeys = config.GetAllFaceKeys();
        for (int i=0; i<configKeys.Count; ++i)
        {
            SetKeyValue(configKeys[i], source.GetKeyValue(configKeys[i]));
        }
    }

    /// <summary>
    /// 获取key值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public float GetKeyValue(string key)
    {
        if (string.IsNullOrEmpty(key))
            return 0.0f;

        for (int i = 0; i < partUnits.Count; ++i)
        {
            if (key.Equals(partUnits[i].key))
                return partUnits[i].value;
        }

        partUnits.Add(new RoleShapeUnitData(key, 0.0f));
        return 0.0f;
    }

    /// <summary>
    /// 设置key值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetKeyValue(string key, float value)
    {
        if (string.IsNullOrEmpty(key))
            return;

        for (int i = 0; i < partUnits.Count; ++i)
        {
            if (key.Equals(partUnits[i].key))
            {
                partUnits[i].value = value;
                return;
            }
        }

        partUnits.Add(new RoleShapeUnitData(key, value));
    }

}

/// <summary>
/// 部位部件的数据
/// </summary>
[System.Serializable]
public class RoleShapeUnitData
{
    public string key;
    public float value = 0.0f;//每个blendshape值范围是0-100

    public RoleShapeUnitData() { }

    public RoleShapeUnitData(string key, float value)
    {
        this.key = key;
        this.value = value;
    }

    public RoleShapeUnitData(RoleShapeUnitData source)
    {
        this.key = source.key;
        this.value = source.value;
    }

}
