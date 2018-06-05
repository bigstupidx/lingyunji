#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using PackTool;
using GUIEditor;
using UnityEngine.UI;

partial class RunningResourcesEditor : BaseEditorWindow
{
    static void GetMaterialTextures(Material mat, System.Func<Texture, bool> fun)
    {
        if (mat == null)
            return;
        MaterialProperty[] mps = UnityEditor.MaterialEditor.GetMaterialProperties(new Object[] { mat });
        foreach (MaterialProperty mp in mps)
        {
            if (mp.type == MaterialProperty.PropType.Texture)
            {
                if (mp.textureValue != null)
                {
                    if (fun(mp.textureValue))
                        break;
                }
            }
        }
    }

    static List<Object> FindDepResoruces(Texture texture)
    {
        List<Object> res = new List<Object>();
        Renderer[] renderers = Resources.FindObjectsOfTypeAll<Renderer>();
        bool isfind = false;
        foreach (Renderer r in renderers)
        {
            isfind = false;
            Material[] mats = r.sharedMaterials;
            foreach (Material mat in mats)
            {
                GetMaterialTextures(mat, 
                    (Texture t) => 
                    { 
                        if (t == texture) 
                        {
                            isfind = true; res.Add(r); 
                        }
                    });

                if (isfind)
                    break;
            }
        }

        foreach (UnityEngine.UI.Graphic graphic in Resources.FindObjectsOfTypeAll<UnityEngine.UI.Graphic>())
        {
            if (graphic != null)
            {
                GetMaterialTextures(graphic.materialForRendering,
                    (Texture t) =>
                    {
                        if (t == texture)
                        {
                            isfind = true;
                            res.Add(graphic);
                        }
                    });
            }
        }

        return res;
    }

    static void GetMaterialTextures(Material mat, System.Action<Texture> fun)
    {
        if (mat != null)
        {
            MaterialProperty[] mps = UnityEditor.MaterialEditor.GetMaterialProperties(new Object[] { mat });
            Texture t = null;
            foreach (MaterialProperty mp in mps)
            {
                if (mp.type == MaterialProperty.PropType.Texture)
                {
                    if ((t = mp.textureValue) != null)
                    {
                        fun(t);
                    }
                }
            }
        }
    }

    // 查找当前正在使用的纹理
    static HashSet<int> FindAllUsedTexture(bool findunactive)
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>(findunactive);
        HashSet<int> usedlist = new HashSet<int>();
        foreach (Renderer r in renderers)
        {
            Material[] mats = r.sharedMaterials;
            foreach (Material mat in mats)
            {
                GetMaterialTextures(mat, (Texture t) => { usedlist.Add(t.GetInstanceID()); });
            }
        }

        foreach (Graphic widget in FindObjectsOfType<Graphic>(findunactive))
        {
            if (widget.materialForRendering != null)
            {
                GetMaterialTextures(widget.materialForRendering, (Texture t) => { usedlist.Add(t.GetInstanceID()); });
            }
            else if (widget.mainTexture != null)
            {
                usedlist.Add(widget.mainTexture.GetInstanceID());
            }
        }

        return usedlist;
    }

    // 是否查找未激活对象
    bool isFindUnactiveResources = false;

    // 查看未使用的资源
    void ShowNoUseResources(List<AssetLoad<Texture>> texs)
    {
        isFindUnactiveResources = GUILayout.Toggle(isFindUnactiveResources, "查找未激活");

        ParamList noUseParamList = mParamList.Get<ParamList>("NoUsedRsources");
        List<AssetLoad<Texture>> nouse = null;
        if (GUILayout.Button("查找未使用的资源"))
        {
            noUseParamList.ReleaseAll();
            HashSet<int> useList = FindAllUsedTexture(isFindUnactiveResources);
            nouse = new List<AssetLoad<Texture>>();
            foreach (KeyValuePair<string, AssetLoadObject> itor in AllAsset.AlreadLoadList)
            {
                AssetLoad<Texture> at = itor.Value as AssetLoad<Texture>;
                if (at == null)
                    continue;

                if (at.isDone && at.asset != null)
                {
                    if (!useList.Contains(at.asset.GetInstanceID()))
                        nouse.Add(at);
                }
            }

            noUseParamList.Set("nouse", nouse);
        }

        if ((nouse = noUseParamList.Get<List<AssetLoad<Texture>>>("nouse")) == null)
            return;

        OnShowTextureList(noUseParamList, "未使用的资源", nouse);//不导出中文
    }

    static void OnShowTextureList(ParamList pl, string name, List<AssetLoad<Texture>> texs)
    {
        texs.RemoveAll((AssetLoad<Texture> t) => { return t.asset == null; });

        GuiTools.TextureListField<AssetLoad<Texture>>(
            pl,
            false,
            "",
            texs,
            (AssetLoad<Texture> l) => { return l.asset; },
            (AssetLoad<Texture> l) =>
            {
                string ik = "Instance:" + l.asset.GetInstanceID();

                {
                    ParamList tpl = pl.Get<ParamList>(ik);
                    bool v = tpl.Get<bool>("showdeplist", false);
                    if (v == false)
                        return;
                    
                    List<Object> fi = tpl.Get<List<Object>>("deplist");
                    if (fi == null)
                    {
                        fi = FindDepResoruces(l.asset);
                        tpl.Set("deplist", fi);
                    }
                    
                    GuiTools.ObjectFieldList<Object>(tpl.Get<ParamList>("Renderers"), fi, true);
                }
            },
            null,
            (AssetLoad<Texture> l) =>
            {
                if (GUILayout.Button("删除" + l.Refcount))
                {
                    TextureLoad.Destroy(l.url);
                }

                string ik = "Instance:" + l.asset.GetInstanceID();
                {
                    ParamList tpl = pl.Get<ParamList>(ik);
                    bool v = tpl.Get<bool>("showdeplist", false);
                    if (GUILayout.Button(v == false ? "搜索引用" : "隐藏引用"))
                    {
                        tpl.Set("showdeplist", !v);
                        if (v)
                            tpl.Set("deplist", null);
                    }
                }
            });
    }

    void ShowTexture()
    {
        // 查看下纹理的使用情况
        List<AssetLoad<Texture>> texs = new List<AssetLoad<Texture>>();
        {
            Dictionary<string, AssetLoadObject> objs = AllAsset.AlreadLoadList;
            foreach (KeyValuePair<string, AssetLoadObject> itor in objs)
            {
                if (itor.Value as AssetLoad<Texture> != null)
                    texs.Add(itor.Value as AssetLoad<Texture>);
            }
        }

        OnShowTextureList(mParamList.Get<ParamList>("Texture"), "", texs);
        ShowNoUseResources(texs);
    }
}
#endif