using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.json;

/// <summary>
/// 易容的整体数据
/// 包含数据内容：1发型，2发色，3肤色，4妆容，5捏脸
/// 资源通过编号来
/// </summary>
[System.Serializable]
public class RoleDisguiseOverallData
{
    public int faceType = 0;// 基准脸

    // 发型
    public int hairStyle = 0;// 发饰的也根据这个字段保存数据

    // 发色
    public int hairColorIdx = 0;

    // 肤色
    public int skinColorIdx = 0;

    // 妆容
    public RoleSkinPartData skinData = new RoleSkinPartData (); 
    // 捏脸
    public RoleShapePartData shapeData = new RoleShapePartData ();

    public bool Equals(RoleDisguiseOverallData other)
    {
        if (!faceType.Equals(other.faceType))
            return false;
        if (!hairStyle.Equals(other.hairStyle))
            return false;
        if (!hairColorIdx.Equals(other.hairColorIdx))
            return false;


        return true;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void FromJson(string json)
    {
        try
        {
            RoleDisguiseOverallData data = JsonUtility.FromJson<RoleDisguiseOverallData>(json);
            if (data != null)
                this.Clone(data);
        }
        catch(System.Exception ex)
        {
            Debuger.LogException(ex);
        }
    }

    public string ToFaceJson()
    {
        RoleFaceDisguiseData data = new RoleFaceDisguiseData(skinData, shapeData);
        return data.ToJson();
    }

    public void FromFaceJson(string faceJson)
    {
        if (string.IsNullOrEmpty(faceJson))
        {
            Debuger.LogError("faceJosn==null");
            return;
        }
        RoleFaceDisguiseData data = JsonUtility.FromJson<RoleFaceDisguiseData>(faceJson);
        if (data!=null)
        {
            this.skinData.Clone(data.skinData);
            this.shapeData.Clone(data.shapeData);
        }
    }

    /// <summary>
    /// 复制数据
    /// </summary>
    /// <param name="source"></param>
    public void Clone(RoleDisguiseOverallData source)
    {
        if (source == null)
            return;
        faceType = source.faceType;
        hairStyle = source.hairStyle;
        hairColorIdx = source.hairColorIdx;
        skinColorIdx = source.skinColorIdx;

        skinData.Clone(source.skinData);
        shapeData.Clone(source.shapeData);
    }

    /// <summary>
    /// 获取类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetType(int type)
    {
        if (type == 1)
        {
            return hairStyle;
        }
        else if (type == 2)
        {
            return hairColorIdx;
        }
        else if (type == 3)
        {
            return skinColorIdx;
        }
        else
        {
            return 0;
        }
    }

}

[System.Serializable]
public class RoleDisguisePrefab : IJsonFile
{
    public static bool TryGet(string name, out RoleDisguisePrefab config)
    {
        return JsonConfigMgr.RoleDisguisePrefabs.TryGet(name, out config);
    }

    public static RoleDisguiseOverallData GetPrefabData(string name)
    {
        RoleDisguisePrefab prefab;
        if (JsonConfigMgr.RoleDisguisePrefabs.TryGet(name, out prefab))
            return prefab.m_data;
        return null;
    }

    public string GetKey() { return fileKey; }
    public void SetKey(string key) { fileKey = key; }

    public string fileKey;// 方案键值,数据的唯一id
    public string describe;// 描述

    //public int career;// 适用角色职业
    //public int sex; // 适用性别

    public RoleDisguiseOverallData m_data = new RoleDisguiseOverallData ();
}

[System.Serializable]
public class RoleFaceDisguisePrefab : IJsonFile
{
    public static bool TryGet(string name, out RoleFaceDisguisePrefab config)
    {
        return JsonConfigMgr.RoleFaceDisguisePrefabs.TryGet(name, out config);
    }

    public string GetKey() { return fileKey; }
    public void SetKey(string key) { fileKey = key; }

    public string fileKey;// 方案键值
    public string describe;// 描述

    // 妆容
    public RoleSkinPartData skinData = new RoleSkinPartData ();
    // 捏脸
    public RoleShapePartData shapeData = new RoleShapePartData ();

}

[System.Serializable]
public class RoleFaceDisguiseData
{
    // 妆容
    public RoleSkinPartData skinData;
    // 捏脸
    public RoleShapePartData shapeData;

    public RoleFaceDisguiseData(RoleSkinPartData skin, RoleShapePartData shape)
    {
        skinData = skin;
        shapeData = shape;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

}

/// <summary>
/// 衣服颜色
/// </summary>
public class RoleClothData
{
    public int clothId = 0;
    public string clothModelName;
    // 衣服颜色

    public string GetModelName()
    {
        return string.Empty;
    }
}

/// <summary>
/// 武器数据
/// </summary>
public class RoleWeaponData
{
    public int weaponId = 0;
    public string weaponModelName0;// 左手武器
    public string weaponModelName1;// 右手武器
    // 武器品阶

    public string GetModelName0()
    {
        return string.Empty;
    }

    public string GetModelName1()
    {
        return string.Empty;
    }
}

/// <summary>
/// 坐骑数据
/// </summary>
public class RoleRideData
{
    public int rideId = 0;
    public string rideModelName;
    // 坐骑颜色

    public string GetModelName()
    {
        return string.Empty;
    }
}

