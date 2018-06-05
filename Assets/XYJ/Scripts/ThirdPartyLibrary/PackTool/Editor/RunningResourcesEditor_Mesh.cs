#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PackTool;
using GUIEditor;

partial class RunningResourcesEditor : BaseEditorWindow
{
    static List<Object> FindDepResoruces(Mesh mesh)
    {
        List<Object> res = new List<Object>();
        {
            MeshFilter[] meshFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();
            foreach (MeshFilter r in meshFilters)
            {
                if (r.sharedMesh == mesh)
                    res.Add(r);
            }
        }

        {
            SkinnedMeshRenderer[] smrs = Resources.FindObjectsOfTypeAll<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer r in smrs)
            {
                if (r.sharedMesh == mesh)
                    res.Add(r);
            }
        }

        return res;
    }

    class MeshShow
    {
        string search_key = "";

        Vector2 ScrollPosition = Vector2.zero;

        public void Show(ParamList paramList)
        {
            search_key = EditorGUILayout.TextField("搜索key", search_key);

            // 查看下纹理的使用情况
            List<MeshLoad> atlasList = new List<MeshLoad>();
            {
                Dictionary<string, AssetLoadObject> objs = MeshLoad.GetAllList();
                foreach (KeyValuePair<string, AssetLoadObject> itor in objs)
                {
                    MeshLoad ml = itor.Value as MeshLoad;
                    if (ml == null || !ml.isDone)
                        continue;

                    if (!string.IsNullOrEmpty(search_key) && !ml.url.Contains(search_key))
                        continue;

                    atlasList.Add(ml);
                }
            }

            EditorGUILayout.LabelField("网格");//不导出中文
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            GuiTools.ObjectFieldList<MeshLoad>(
                paramList,
                atlasList,
                (MeshLoad pl) => { return pl.asset; },
                false,
                null,
                (MeshLoad pl) =>
                {
                    //string ik = "Instance:" + pl.asset.GetInstanceID();
                    {

                    }
                },
                (MeshLoad pl) =>
                {
                    string ik = "Instance:" + pl.asset.GetInstanceID();
                    {
                        ParamList tpl = paramList.Get<ParamList>(ik);
                        bool v = tpl.Get<bool>("showdeplist", false);
                        if (GUILayout.Button(v == false ? "搜索引用" : "隐藏引用"))
                        {
                            tpl.Set("showdeplist", !v);
                            if (v)
                                tpl.Set("deplist", null);
                        }
                    }

                    {
                        ParamList tpl = paramList.Get<ParamList>(ik);
                        bool v = tpl.Get<bool>("showdeplist", false);
                        if (v == false)
                            return;

                        EditorGUILayout.EndHorizontal();
                        List<Object> fi = tpl.Get<List<Object>>("deplist");
                        if (fi == null || fi.Count == 0)
                        {
                            fi = FindDepResoruces(pl.asset);
                            tpl.Set("deplist", fi);
                        }

                        GuiTools.ObjectFieldList<Object>(tpl.Get<ParamList>("MeshLoad"), fi, true);
                        EditorGUILayout.BeginHorizontal();
                    }
                });
            EditorGUILayout.EndScrollView();
        }
    }

    MeshShow mMeshShow = new MeshShow();

    void ShowMeshList()
    {
        mMeshShow.Show(mParamList.Get("MeshShow", () => { return new ParamList(); }));
    }
}
#endif