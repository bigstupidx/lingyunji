

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.json;

/// <summary>
/// 角色外形键值配置
/// </summary>
[System.Serializable]
public class RoleShapeConfig : IJsonFile
{
    public static bool TryGet(string name, out RoleShapeConfig config)
    {
        return JsonConfigMgr.RoleShapeConfigs.TryGet(name, out config);
    }

    public string GetKey() { return shapeKey; }
    public void SetKey(string key) { shapeKey = key; }

    
    public string shapeKey;// 外观配置唯一值

    public string shapeName;// 外形配置名称
    public int sex = 0;// 可能没必要，需要shapeKey

    // face key
    public string faceKey;// 角色脸部的键名配置

    // 几个基准脸的特殊值，另外需要设置身高（实际是一个缩放值）
    public List<RoleFaceBaseSet> baseFaces = new List<RoleFaceBaseSet>();
    
    // 修容配置
    public List<RoleShapePart> faceParts = new List<RoleShapePart>();


    // 内部键名列表
    private List<string> m_allFaceKeys = new List<string>();
    private List<string> m_faceTypeKeys = new List<string>();
    /// <summary>
    /// 初始化所有键名
    /// </summary>
    public void InitKeys()
    {
        m_allFaceKeys.Clear();
        for (int i = 0; i < faceParts.Count; ++i)
        {
            // 初始化所有key
            faceParts[i].InitKeys(faceKey);
            // 获取所有key
            m_allFaceKeys.AddRange(faceParts[i].GetPartKeys());
        }
        
        // 基准脸的keys
        m_faceTypeKeys.Clear();
        for (int i=0; i<baseFaces.Count; ++i)
        {
            baseFaces[i].InitKey(faceKey);
            m_faceTypeKeys.Add(baseFaces[i].GetKey());
        }
        
    }

    /// <summary>
    /// 获取配置的所有键名
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllFaceKeys()
    {
        return m_allFaceKeys;
    }

    /// <summary>
    /// 获取基准脸keys
    /// </summary>
    /// <returns></returns>
    public List<string> GetFaceTypeKeys()
    {
        return m_faceTypeKeys;
    }

    public RoleShapeConfig() { }
    public RoleShapeConfig(RoleShapeConfig source)
    {
        this.shapeKey = source.shapeKey;
        this.shapeName = source.shapeName;
        this.sex = source.sex;

        this.faceKey = source.faceKey;
        for (int i=0; i<source.faceParts.Count; ++i)
        {
            this.faceParts.Add(new RoleShapePart(source.faceParts[i]));
        }
    }
	
}

[System.Serializable]
public class RoleFaceBaseSet
{
    public string key;
    public float scale = 1.0f;

    public string RootKey
    {
        get;
        private set;
    }

    // 初始化键名
    public void InitKey(string rootKey)
    {
        RootKey = rootKey;
    }

    public string GetKey()
    {
        return string.Format("{0}{1}", RootKey, key);
    }
}

/// <summary>
/// 外观总部位的键名
/// </summary>
[System.Serializable]
public class RoleShapePart
{
    public string name;//外观部位名称

    public List<RoleShapeSubPart> subParts = new List<RoleShapeSubPart>();

    public RoleShapePart() { }
    public RoleShapePart(RoleShapePart source)
    {
        this.name = source.name;
        for (int i = 0; i < source.subParts.Count; ++i)
        {
            this.subParts.Add(new RoleShapeSubPart(source.subParts[i]));
        }
    }

    // 初始化所有键名
    public void InitKeys(string rootKey)
    {
        for (int i = 0; i < subParts.Count; ++i)
        {
            subParts[i].InitKeys(rootKey);
        }
    }

    /// <summary>
    /// 获取部位的所有键名
    /// </summary>
    /// <param name="rootKey"></param>
    /// <returns></returns>
    public List<string> GetPartKeys()
    {
        List<string> partKeys = new List<string>();
        for (int i = 0; i < subParts.Count; ++i)
        {
            partKeys.AddRange(subParts[i].GetUnitKeys());
        }

        return partKeys;
    }

}


/// <summary>
/// 每个子部位的键名
/// </summary>
[System.Serializable]
public class RoleShapeSubPart
{
    public string name;
    public string key;

    public List<RoleShapePartUnit> units = new List<RoleShapePartUnit>();

    public RoleShapeSubPart() { }
    public RoleShapeSubPart(RoleShapeSubPart source)
    {
        this.name = source.name;
        this.key = source.key;

        for (int i = 0; i < source.units.Count; ++i)
        {
            this.units.Add(new RoleShapePartUnit(source.units[i]));
        }
    }

    // 初始化所有键名
    public void InitKeys(string rootKey)
    {
        for (int i = 0; i < units.Count; ++i)
        {
            units[i].InitKey (rootKey+key);
        }
    }

    /// <summary>
    /// 获取部位的键名
    /// </summary>
    /// <param name="rootKey"></param>
    /// <returns></returns>
    public List<string> GetUnitKeys()
    {
        List<string> unitKeys = new List<string>();
        for (int i = 0; i < units.Count; ++i)
        {
            string fullKey = units[i].GetKey0();
            if (!string.IsNullOrEmpty(fullKey))
                unitKeys.Add(fullKey);
            fullKey = units[i].GetKey1();
            if (!string.IsNullOrEmpty(fullKey))
                unitKeys.Add(fullKey);
        }

        return unitKeys;
    }
}


/// <summary>
/// 具体部位部件的键名
/// 每个blendshape值范围是0-100
/// </summary>
[System.Serializable]
public class RoleShapePartUnit
{
    public string name;
    public string key0;
    public string key1;

    public string RootKey
    {
        get;
        private set;
    }

    public RoleShapePartUnit() { }
    public RoleShapePartUnit(RoleShapePartUnit source)
    {
        this.name = source.name;
        this.key0 = source.key0;
        this.key1 = source.key1;
    }

    // 初始化键名
    public void InitKey(string rootKey)
    {
        RootKey = rootKey;
    }

    /// <summary>
    /// 完整key值
    /// </summary>
    /// <returns></returns>
    public string GetKey0()
    {
        if (string.IsNullOrEmpty(key0))
            return string.Empty;

        return string.Format("{0}{1}", RootKey, key0);
    }

    /// <summary>
    /// 完整key值
    /// </summary>
    /// <returns></returns>
    public string GetKey1()
    {
        if (string.IsNullOrEmpty(key1))
            return string.Empty;

        return string.Format("{0}{1}", RootKey, key1);
    }

}
