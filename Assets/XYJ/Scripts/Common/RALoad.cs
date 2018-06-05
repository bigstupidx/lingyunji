using XTools;
using PackTool;
using UnityEngine;

public static class RALoad
{
    public static void LoadMaterail(string name, ResourcesEnd<Material> fun, object p)
    {
#if !USE_RESOURCES
        if (!name.EndsWith(".mat"))
            name += ".mat";
#endif
#if USE_RESOURCESEXPORT
        // 使用打包加载模式
        name = ResourcesGroup.GetFullPath(ResourcesGroup.Material, name);
        MaterialLoad load = MaterialLoad.Load(name, fun, p).load as MaterialLoad;
        load.AddRef();
#elif USER_ALLRESOURCES
        Material mat = AllResources.Instance.GetMaterial(ResourcesGroup.GetFullPath(ResourcesGroup.Material, name));
        AddNextFrame(UpdateByMaterial, new object[] { mat, fun, p });
#elif USE_RESOURCES
        Material mat = AssetResoruce.Load<Material>(name);
        AddNextFrame(UpdateByMaterial, new object[]{mat, fun, p});
#elif USE_ABL
        ABL.AssetsMgr.Load(ResourcesGroup.GetFullPath(ResourcesGroup.Material, name), (Material m)=> { fun(m, p); });
#elif UNITY_EDITOR
        Material mat = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + ResourcesGroup.GetFullPath("MaterialExport", name), typeof(Material));
        AddNextFrame(UpdateByMaterial, new object[] { mat, fun, p });
#endif
    }

#if COM_DEBUG
    public static float s_delayTime = 0.0f;
#endif

    public static TimerFrame.Frame AddNextFrame(TimerFrame.UPDATE fun, object funp)
    {
#if UNITY_EDITOR
        TimerFrame.Frame fu = TimerMgrObj.Instance.AddLateUpdate(fun, funp);
#else
        TimerFrame.Frame fu = xys.App.my.frameMgr.AddLateUpdate(fun,  funp);
#endif

#if COM_DEBUG
        fu.delayTime = s_delayTime;
#endif
        return fu;
    }

    static bool UpdateByMaterial(object p)
    {
        object[] pp = p as object[];
        Material mat = pp[0] as Material;
        ResourcesEnd<Material> fun = pp[1] as ResourcesEnd<Material>;
        if (fun != null)
        {
            fun(mat, pp[2]);
        }

        return false;
    }

    // 同步资源加载
    // 如果资源存在，则会立即调用，
    // 不存在则会在加载完之后调用
    public static void LoadPrefabSync(string name, PackTool.ResourcesEnd<GameObject> fun, object para = null, bool isInit = true)
    {
#if USE_ABL
        System.Action<GameObject> endfun = (GameObject go) => 
        {
            if (go == null)
            {
                if (fun != null)
                    fun(null, para);
            }
            else
            {
                if (isInit)
                    go = Object.Instantiate(go);

                if (fun != null)
                    fun(go, para);
            }
        };

        if (!name.EndsWith(".prefab"))
            name = name + ".prefab";

        string fullpath = ResourcesGroup.GetFullPath(ResourcesGroup.Prefab, name.ToLower());
        var asset = ABL.AssetsMgr.Get(fullpath);
        if (asset != null)
        {
            endfun(asset.obj as GameObject);
        }
        else
        {
            ABL.AssetsMgr.Load(fullpath, (GameObject go) =>
            {
                endfun(go);
            });
        }
#elif USE_RESOURCESEXPORT
        PrefabLoad.LoadSync(name, fun, para, isInit, false);
#else
        GameObject asset = null;
#if USER_ALLRESOURCES
        name = ResourcesGroup.GetFullPath(ResourcesGroup.Prefab, name + ".prefab");
        if (AllResources.Instance != null)
            asset = AllResources.Instance.GetPrefab(name);
#elif USE_RESOURCES
        asset = AssetResoruce.Load<GameObject>(name);
#elif UNITY_EDITOR
        name = ResourcesGroup.GetFullPath(ResourcesGroup.Prefab, name + ".prefab");
        asset = PrefabLoadByEditor(name);
#endif
        if (asset == null)
        {
            Debuger.LogError("name:" + name + " Resources Not Find!!");
        }

        if (isInit && asset != null)
            asset = Object.Instantiate(asset);

        if (null != fun)
            fun(asset, para);
#endif
    }

#if UNITY_EDITOR
    static GameObject PrefabLoadByEditor(string name)
    {
        for (int i = 0; i < 3; ++i)
        {
            var asset = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + name, typeof(GameObject));
            if (asset != null && System.IO.Path.GetFileNameWithoutExtension(name) != asset.name)
            {
                Debuger.ErrorLog("{0})加载的资源有问题:{1} 重新导入中!", i, name);
                UnityEditor.AssetDatabase.ImportAsset("Assets/" + name, UnityEditor.ImportAssetOptions.ForceUpdate | UnityEditor.ImportAssetOptions.ForceSynchronousImport);
            }
            else
            {
                return asset;
            }
        }

        return null;
    }
#endif

    public static void LoadPrefab(string name, ResourcesEnd<GameObject> fun, object p, bool isinit = true, bool isAutoDestory = false)
    {
#if USE_ABL
        if (!name.EndsWith(".prefab"))
            name = name + ".prefab";

        ABL.AssetsMgr.Load(ResourcesGroup.GetFullPath(ResourcesGroup.Prefab, name.ToLower()), (GameObject go)=> 
        {
            if (go == null)
            {
                if (fun != null)
                    fun(null, p);
            }
            else
            {
                if (isinit)
                    go = Object.Instantiate(go);

                if (fun != null)
                    fun(go, p);
            }
        });
#elif USE_RESOURCESEXPORT
        PrefabLoad.Load(name, fun, p, isinit, isAutoDestory);
#else
        GameObject asset = null;
#if USER_ALLRESOURCES
        name = ResourcesGroup.GetFullPath(ResourcesGroup.Prefab, name + ".prefab");
        if (AllResources.Instance != null)
            asset = AllResources.Instance.GetPrefab(name);
#elif USE_RESOURCES
        asset = AssetResoruce.Load<GameObject>(name);
#elif UNITY_EDITOR
        string prefabsName = name;
        name = ResourcesGroup.GetFullPath(ResourcesGroup.Prefab, name + ".prefab");
        asset = PrefabLoadByEditor(name);
#endif
        if (asset == null)
        {
            Debuger.LogError("name:" + name + " Resources Not Find!!");
        }

#if UNITY_EDITOR
        TimerMgrObj.Instance.addFrameLateUpdate((object pp) => 
#else
        xys.App.my.frameMgr.addFrameLateUpdate((object pp) =>
#endif
        {
            if (isinit && asset != null)
                asset = Object.Instantiate(asset);

            if(null != fun)
                fun(asset, p);
            return false;
        }, 
        null);
#endif
    }
}
