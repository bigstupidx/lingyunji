using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleDisguiseConfig
{
    static List<RoleDisguiseCareer> DisguisesList = new List<RoleDisguiseCareer>();

    public static void Init(List<RoleDisguiseCareer> source)
    {
        DisguisesList.Clear();
        DisguisesList.AddRange(source);
    }

    public static List<RoleDisguiseCareer> GetAll ()
    {
        return DisguisesList;
    }

    /// <summary>
    /// 初始化配置表
    /// </summary>
    public static void Init()
    {
        DisguisesList.Clear();

        Dictionary<int, Config.RoleDisguisePrefabs> dataList = Config.RoleDisguisePrefabs.GetAll();

        List<RoleDisguiseCareer> tmpDisguiseList = new List<RoleDisguiseCareer>();
        RoleDisguiseCareer disguiseData = null;

        List<int> careerTypes = new List<int> ();
        List<int> sexTypes = new List<int>();
        List<int> faceTypes = new List<int> ();
        foreach (var item in dataList)
        {
            Config.RoleDisguisePrefabs p = item.Value;// config item
            if (p.career != -1 && !careerTypes.Contains(p.career))
            { careerTypes.Add(p.career); }
            if (p.sex != -1 && !sexTypes.Contains(p.sex))
            { sexTypes.Add(p.sex); }
            if (p.faceType != -1 && !faceTypes.Contains(p.faceType))
            { faceTypes.Add(p.faceType); }
            RoleDisguiseItem itemData = new RoleDisguiseItem(p.assetId, p.prefabName, p.iconName);// disguise item

            disguiseData = null;
            for (int i = 0; i < tmpDisguiseList.Count; ++i)
            {
                if (tmpDisguiseList[i].career == p.career && tmpDisguiseList[i].sex == p.sex && tmpDisguiseList[i].faceStyle == p.faceType)
                    disguiseData = tmpDisguiseList[i];
            }
            if (disguiseData == null)
            {
                disguiseData = new RoleDisguiseCareer(p.career, p.sex, p.faceType);
                tmpDisguiseList.Add(disguiseData);
            }
            disguiseData.AddItem(p.partType, itemData);
        }

        for (int c = 0; c < careerTypes.Count; ++c)
        {
            for (int s = 0; s < sexTypes.Count; ++s)
            {
                for (int f = 0; f < faceTypes.Count; ++f)
                {
                    disguiseData = new RoleDisguiseCareer(careerTypes[c], sexTypes[s], faceTypes[f]);
                    DisguisesList.Add(disguiseData);
                }
            }
        }

        // 把不限制的项添加到配置
        for (int i = 0; i < tmpDisguiseList.Count; ++i)
        {
            disguiseData = tmpDisguiseList[i];
            if (disguiseData.career == -1 && disguiseData.faceStyle == -1)
            {// 不限制职业和脸型

                for (int k = 0; k < DisguisesList.Count; ++k)
                {
                    if (disguiseData.sex == DisguisesList[k].sex)
                    {
                        for (int j = 0; j < disguiseData.types.Count; ++j)
                        {
                            DisguisesList[k].AddType(disguiseData.types[j]);
                        }
                    }
                }
            }
            else if (disguiseData.career == -1)
            {// 只不限制职业
                for (int k = 0; k < DisguisesList.Count; ++k)
                {
                    if (disguiseData.sex == DisguisesList[k].sex && disguiseData.faceStyle == DisguisesList[k].faceStyle)
                    {
                        for (int j = 0; j < disguiseData.types.Count; ++j)
                        {
                            DisguisesList[k].AddType(disguiseData.types[j]);
                        }

                    }
                }
            }
            else if (disguiseData.faceStyle == -1)
            {// 只不限制脸型
                for (int k = 0; k < DisguisesList.Count; ++k)
                {
                    if (disguiseData.sex == DisguisesList[k].sex && disguiseData.career == DisguisesList[k].career)
                    {
                        for (int j = 0; j < disguiseData.types.Count; ++j)
                        {
                            DisguisesList[k].AddType(disguiseData.types[j]);
                        }

                    }
                }
            }
            else
            {
                for (int k = 0; k < DisguisesList.Count; ++k)
                {
                    if (disguiseData.sex == DisguisesList[k].sex && disguiseData.career == DisguisesList[k].career && disguiseData.faceStyle == DisguisesList[k].faceStyle)
                    {
                        for (int j = 0; j < disguiseData.types.Count; ++j)
                        {
                            DisguisesList[k].AddType(disguiseData.types[j]);
                        }

                    }
                }
            }
        }
    }

    public static RoleDisguiseCareer Get(int career, int sex, int faceStyle)
    {
        if (DisguisesList.Count == 0)
            Init();
        for (int i=0; i<DisguisesList.Count; ++i)
        {
            if (DisguisesList[i].career == career && DisguisesList[i].sex == sex && DisguisesList[i].faceStyle == faceStyle)
                return DisguisesList[i];
        }

        Debuger.LogError(string.Format("找不到易容默认配置, 职业={0}, 性别={1}，基准脸={2}",career, sex, faceStyle));
        return null;
    }
}

[System.Serializable]
public class RoleDisguiseCareer
{
    public int career = 1;// 1天剑
    public int sex = 0;
    public int faceStyle = 0;
    public List<RoleDisguiseType> types;

    public RoleDisguiseCareer(int c, int s, int f)
    {
        this.career = c;
        this.sex = s;
        this.faceStyle = f;

        types = new List<RoleDisguiseType>();
    }

    /// <summary>
    /// 1=发型
    /// 2=发色
    /// 3=皮肤
    /// 4=换脸
    /// 5=默认
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public RoleDisguiseType GetType(int id)
    {
        for (int i=0; i<types.Count; ++i)
        {
            if (types[i].typeId == id)
                return types[i];
        }

        Debuger.LogError(string.Format("找不到易容默认配置类型：{3}, 职业={0}, 性别={1}，基准脸={2}", career, sex, faceStyle, id));
        return null;
    }

    public void AddType(RoleDisguiseType typeData)
    {
        RoleDisguiseType tmpType = null;
        for (int i = 0; i < types.Count; ++i)
        {
            if (types[i].typeId == typeData.typeId)
                tmpType = types[i];
        }
        if (tmpType == null)
        {
            types.Add(typeData);
        }
        else
        {
            tmpType.items.AddRange(typeData.items);
        }
    }

    public void AddItem (int type, RoleDisguiseItem item)
    {
        RoleDisguiseType typeData = null;
        for (int i = 0; i < types.Count; ++i)
        {
            if (types[i].typeId == type)
                typeData = types[i];
        }
        if (typeData == null)
        {
            typeData = new RoleDisguiseType(type);
            types.Add(typeData);
        }
        typeData.AddItem(item);
    }
}

[System.Serializable]
public class RoleDisguiseType
{
    public int typeId = 0;
    public string name;

    public List<RoleDisguiseItem> items;

    public RoleDisguiseType(int type)
    {
        typeId = type;
        items = new List<RoleDisguiseItem>();
    }

    public void AddItem (RoleDisguiseItem item)
    {
        items.Add(item);
    }
}

[System.Serializable]
public class RoleDisguiseItem
{
    public int itemId = 0;// 资源编号

    public string modelName;
    public string perfabName;
    public string iconName;

    public RoleDisguiseColorStyle color;

    public RoleDisguiseItem(int id, string prefab, string icon)
    {
        itemId = id;
        perfabName = prefab;
        iconName = icon;

        modelName = string.Empty;
        color = new RoleDisguiseColorStyle();
        if (itemId == 0)
            return;
        Config.RoleDisguiseAsset asset = Config.RoleDisguiseAsset.Get(itemId);
        if (asset != null)
        {
            modelName = asset.modelName;
            color = new RoleDisguiseColorStyle(asset.HSV);
        }
    }

}

/// <summary>
/// 颜色样式
/// 默认值分别为：0，1，1
/// </summary>
[System.Serializable]
public class RoleDisguiseColorStyle
{
    [Range(0, 360)]
    public int h = 0;//色调，0~360
    [Range(0, 3f)]
    public float s = 1f;//饱和度，0~3
    [Range(0, 3f)]
    public float v = 1f;//明度，0~3

    public Color RBGFloatColor
    {
        get { return Color.HSVToRGB(h / 360f, s / 3f, v / 3f); }
    }

    public RoleDisguiseColorStyle() { }

    public RoleDisguiseColorStyle(string hsvStr)
    {
        h = 0;
        s = 1f;
        v = 1f;
        if (!string.IsNullOrEmpty(hsvStr))
        {
            string[] hsv = hsvStr.Split('|');
            if (hsv!=null && hsv.Length==3)
            {
                int.TryParse(hsv[0], out h);
                float.TryParse(hsv[1], out s);
                float.TryParse(hsv[2], out v);
            }
        }
    }
}