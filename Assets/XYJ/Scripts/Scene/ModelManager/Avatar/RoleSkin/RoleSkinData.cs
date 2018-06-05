using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.json;


/// <summary>
/// 妆容数据预设
/// </summary>
[System.Serializable]
public class RoleSkinDataPrefab : IJsonFile
{
    public string GetKey() { return fileKey; }
    public void SetKey(string key) { fileKey = key; }

    public string fileKey;// 文件key值

    public string describe;// 描述

    public RoleSkinPartData skinData;

}

/// <summary>
/// 部位的数据
/// </summary>
[System.Serializable]
public class RoleSkinPartData
{

    public List<RoleSkinUnitData> skinUnits = new List<RoleSkinUnitData>();

    public bool Equals(RoleSkinPartData other)
    {
        return true;
    }

    public void Clone(RoleSkinPartData source)
    {
        for (int i = 0; i < source.skinUnits.Count; ++i)
        {
            string key = source.skinUnits[i].key;
            RoleSkinUnitData data = Get(key);
            data.Clone(source.skinUnits[i]);
        }
    }

    public void CloneUnit(string key, RoleSkinUnitData source)
    {
        RoleSkinUnitData unitData = Get(key);
        unitData.Clone(source);
    }

    public RoleSkinUnitData Get(string key)
    {
        for (int i = 0; i < skinUnits.Count; ++i)
        {
            if (skinUnits[i].key.Equals(key))
                return skinUnits[i];
        }

        RoleSkinUnitData data = new RoleSkinUnitData(key);
        skinUnits.Add(data);
        return data;
    }
}

[System.Serializable]
public class RoleSkinUnitData
{
    public string key;

    public int texStyle = 0;//纹理样例下标

    public int colorStyle = 0;// 颜色样例下标

    // 默认颜色值
    public int h = 0;
    public float s = 1.0f;
    public float v = 1.0f;

    public bool Equals(RoleSkinUnitData other)
    {
        return true;
    }

    public RoleSkinUnitData() { }
    public RoleSkinUnitData (string key)
    {
        this.key = key;
    }

    public void Clone (RoleSkinUnitData source)
    {
        if (source == null)
        {
            Reset();
            return;
        }

        texStyle = source.texStyle;
        colorStyle = source.colorStyle;

        h = source.h;
        s = source.s;
        v = source.v;
    }

    public void Reset()
    {
        texStyle = 0;
        colorStyle = 0;

        h = 0;
        s = 1.0f;
        v = 1.0f;
    }

    #region Internal Implement

    //Color32[] clr;
    //bool cache = false;
    //int cacheH = 0;//色调，0~360
    //float cacheS = 1f;//饱和度，0~3？
    //float cacheV = 1f;//明度，0~3？
    //int cacheIdx = 0;

    //public Color32[] GetPixelsCache(int w, int h)
    //{
    //    int newSize = w * h + 1;
    //    if (clr == null)
    //    {
    //        clr = new Color32[newSize];
    //    }
    //    else if (clr.Length < newSize)
    //    {
    //        System.Array.Resize(ref clr, newSize);
    //    }

    //    return clr;
    //}

    //public void ClearCache()
    //{
    //    texStyle = 0;

    //    //clr = null;
    //    cache = false;
    //    cacheH = 0;
    //    cacheS = 1f;
    //    cacheIdx = 0;
    //    cacheV = 1f;
    //}

    //public bool CheckFresh()
    //{
    //    if (cache && cacheH == h && cacheS == s && cacheV == v && cacheIdx == texStyle)
    //        return false;

    //    cache = true;
    //    cacheH = h;
    //    cacheS = s;
    //    cacheV = v;
    //    cacheIdx = texStyle;
    //    return true;
    //}

    #endregion
}

