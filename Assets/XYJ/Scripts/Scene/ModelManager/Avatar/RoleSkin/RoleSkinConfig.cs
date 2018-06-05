using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.json;

/// <summary>
/// 角色妆容的键值资源配置
/// 任何职业都只用两套脸部纹理
/// </summary>
[System.Serializable]
[ExecuteInEditMode]
public class RoleSkinConfig : MonoBehaviour
{    
    static GameObject _prefab = null;
    static bool isLoading = false;

    static RoleSkinConfig _instance = null;
    public static RoleSkinConfig instance
    {
        get { return _instance; }
    }

#if USE_RESOURCESEXPORT
    static PackTool.PrefabLoad.PrefabData prefabData;
#endif

    public static void Cache()
    {
        if (_instance != null || isLoading)
            return;
        isLoading = true;
#if USE_RESOURCESEXPORT
        prefabData = PackTool.PrefabLoad.LoadAsync("RoleSkinConfig", (GameObject go, object p) =>
#else
        ArtResLoad.LoadResSync("RoleSkinConfig", (GameObject go, object p) =>
#endif
        {
#if USE_RESOURCESEXPORT
            prefabData.load.AddRef();
#endif
            isLoading = false;
            if (go == null)
            {
                Debug.LogErrorFormat("加载RoleSkinConfig失败");
                return;
            }
            _prefab = go;
            _instance = go.GetComponent<RoleSkinConfig>();//存着预制体就好了
        }, null, false);
    }

    // 不同脸型的配置
    public List<RoleSkinPart> parts = new List<RoleSkinPart>();

    void Awake()
    {
        _instance = this;

//#if UNITY_EDITOR
//        foreach (var d in parts)
//        {
//            d.Set();
//        }
//#endif
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public RoleSkinPart Get(string key)
    {
        Cache();
        foreach (var part in parts)
        {
            if (part.skinKey.Equals(key))
                return part;
        }
        Debuger.LogError("不能找到妆容配置的部位：" + key);
        return null;
    }

#region 编辑器方法

    public List<string> GetKeys()
    {
        List<string> keys = new List<string>();
        foreach (var part in parts)
            keys.Add(part.skinKey);

        return keys;
    }

    public void DeletePart(string key)
    {
        RoleSkinPart part = Get(key);
        if (part != null)
        {
            parts.Remove(part);
        }
    }

    public void ClonePart(string key)
    {
        RoleSkinPart part = Get(key);
        if (part!=null)
        {
            parts.Add(new RoleSkinPart(part, string.Format("{0}(Clone)", key)));
        }
    }

#endregion

}

/// <summary>
/// 每个部位的配置
/// </summary>
[System.Serializable]
public class RoleSkinPart
{
    public string skinKey;//代码里用来找到这个配置的唯一标识

    //中文名,用于界面显示
    public string[] tabsName;
    public string[] subTabsKeys;

    //public Texture2D orgTexture;// 原始纹理
    public Texture2DAsset orgTexture;// 原始纹理
    public List<RoleSkinUnit> units = new List<RoleSkinUnit>();

//#if UNITY_EDITOR
//    public void Set()
//    {
//        orgTextureAsset = Texture2DAsset.Get(orgTexture);
//        foreach (var d in units)
//            d.Set();
//    }
//#endif

    public RoleSkinPart() { }

    public RoleSkinPart(RoleSkinPart source, string newKey)
    {
        skinKey = newKey;
        int len = source.tabsName.Length;
        tabsName = new string[len];
        System.Array.Copy(source.tabsName, tabsName, source.tabsName.Length);
        len = source.subTabsKeys.Length;
        subTabsKeys = new string[len];
        System.Array.Copy(source.subTabsKeys, subTabsKeys, source.subTabsKeys.Length);

        orgTexture = source.orgTexture;
        units = new List<RoleSkinUnit>();
        foreach (var item in source.units)
            units.Add(new RoleSkinUnit(item));
    }

    public string[] GetTabsKeys(int index)
    {
        string str = subTabsKeys[index];
        return str.Split(';');
    }

    public RoleSkinUnit Get(string key)
    {
        foreach (var unit in units)
        {
            if (key.Equals(unit.key))
                return unit;
        }

        Debuger.LogError("找不到妆容配置的部位单元：" + key);
        return null;
    }

    public string GetKeyName(int index)
    {
        if (index>=0 && index < units.Count)
        {
            return units[index].key;
        }
        Debuger.LogError("找不到妆容配置的部位单元下标：" + index);
        return string.Empty;
    }
}

/// <summary>
/// 每个部位单元
/// </summary>
[System.Serializable]
public class RoleSkinUnit
{
    public string key = string.Empty;
    public string name = string.Empty;// 界面展示名

    //public Texture2D shareMask;//共享遮罩图，如果texStyles的mask没有填则用这个
    public Texture2DAsset shareMask;//共享遮罩图，如果texStyles的mask没有填则用这个
    public Vector2 shareOffset = Vector2.zero;// 共享偏移值

    public List<RoleSkinTexStyle> texStyles;// 贴图样式方案
    public List<RoleSkinColorStyle> colorStyles;// 贴图颜色颜色方案

//#if UNITY_EDITOR
//    public void Set()
//    {
//        shareMaskAsset = Texture2DAsset.Get(shareMask);
//        foreach (var d in texStyles)
//            d.Set();
//    }
//#endif

    /// <summary>
    /// 是否分别支持对色相，饱和度和明暗调整
    /// </summary>
    public bool h = false;
    public bool s = false;
    public bool v = false;

    public RoleSkinUnit() { }

    public RoleSkinUnit(RoleSkinUnit source)
    {
        key = source.key;
        name = source.name;

        shareMask = source.shareMask;
        shareOffset = source.shareOffset;

        texStyles = new List<RoleSkinTexStyle>();
        foreach (var item in source.texStyles)
            texStyles.Add(item);

        colorStyles = new List<RoleSkinColorStyle>();
        foreach (var item in source.colorStyles)
            colorStyles.Add(item);

        h = source.h;
        s = source.s;
        v = source.v;
    }

}

/// <summary>
/// 颜色样式
/// 默认值分别为：0，1，1
/// </summary>
[System.Serializable]
public class RoleSkinColorStyle
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
}

/// <summary>
/// 贴图样式
/// </summary>
[System.Serializable]
public class RoleSkinTexStyle
{
    public Texture2DAsset texName;//贴图名称
    public Texture2DAsset maskName;//遮罩图名称

    //public Texture2D texName;//贴图名称
    //public Texture2D maskName;//遮罩图名称
    public string previewUIName;//预览图的UI名
    public Vector2 offset = Vector2.zero;// 贴图偏移
    public bool mirror = false;// 是否镜像

//#if UNITY_EDITOR
//    public void Set()
//    {
//        texNameAsset = Texture2DAsset.Get(texName);
//        maskNameAsset = Texture2DAsset.Get(maskName);
//    }
//#endif
}

