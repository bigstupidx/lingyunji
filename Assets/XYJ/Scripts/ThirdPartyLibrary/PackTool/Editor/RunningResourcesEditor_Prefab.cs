using UnityEngine;
using UnityEditor;
using PackTool;
using GUIEditor;
using System.Collections.Generic;
#pragma warning disable 414, 649
#if USE_RESOURCESEXPORT
partial class RunningResourcesEditor : BaseEditorWindow
{
    public class PrefabLoadShow
    {
        public Vector2 PrefabScrollPosition;
        public Vector2 DeleteScrollPosition;

        // 排序
        enum PrefabSortType
        {
            [Enum2Type.EnumValue("创建次数")]
            CreateNum, // 创建次数
            [Enum2Type.EnumValue("存活个数")]
            LiftNum, // 存活个数
            [Enum2Type.EnumValue("加载次数")]
            LoadNum, // 存活个数
        }

        PrefabSortType sortType;
        bool isDasc = false;

        string search_key = "";

        public void ShowPrefabList(ParamList paramlist, Dictionary<string, AssetLoadObject> objs)
        {
            search_key = EditorGUILayout.TextField("搜索key", search_key);

            // 查看下预置体的使用情况
            List<PrefabLoad> prefabs = new List<PrefabLoad>();
            {
                foreach (KeyValuePair<string, AssetLoadObject> itor in objs)
                {
                    if (itor.Value == null)
                        continue;

                    if (!string.IsNullOrEmpty(search_key) && !itor.Key.Contains(search_key))
                        continue;

                    PrefabLoad pl = itor.Value as PrefabLoad;
                    if (!pl.isDone)
                        continue;

                    prefabs.Add(pl);
                }
            }

            GuiTools.HorizontalField(true,
                () =>
                {
                    sortType = (PrefabSortType)GuiTools.EnumPopup(false, "排序类型", sortType);//不导出中文
                    isDasc = EditorGUILayout.Toggle("升序", isDasc);
                });

            switch (sortType)
            {
            case PrefabSortType.CreateNum:
                prefabs.Sort((PrefabLoad x, PrefabLoad y) => 
                {
                    if (x == null || y == null)
                        return 0;

                    if (x.prefabBeh == null || y.prefabBeh == null)
                        return 0;

                    return x.prefabBeh.Get().create_num.CompareTo(y.prefabBeh.Get().create_num); 
                });
                break;
            case PrefabSortType.LiftNum:
                prefabs.Sort((PrefabLoad x, PrefabLoad y) => 
                {
                    if (x == null || y == null)
                        return 0;

                    if (x.prefabBeh == null || y.prefabBeh == null)
                        return 0;

                    return x.prefabBeh.Get().lift_num.CompareTo(y.prefabBeh.Get().lift_num); 
                });
                break;
            case PrefabSortType.LoadNum:
                prefabs.Sort((PrefabLoad x, PrefabLoad y) => 
                {
                    if (x == null || y == null)
                        return 0;

                    if (x.prefabBeh == null || y.prefabBeh == null)
                        return 0;

                    return x.prefabBeh.Get().load_num.CompareTo(y.prefabBeh.Get().load_num);
                });
                break;
            }

            if (!isDasc)
            {
                prefabs.Reverse();
            }

            ParamList paramList = paramlist.Get<ParamList>("预置体");//不导出中文

            EditorGUILayout.LabelField("源预置体");//不导出中文
            PrefabScrollPosition = EditorGUILayout.BeginScrollView(PrefabScrollPosition);
            GuiTools.ObjectFieldList<PrefabLoad>(
                paramList,
                prefabs,
                (PrefabLoad pl) => { return pl.asset; },
                false,
                null,
                (PrefabLoad pl) =>
                {
                },
                (PrefabLoad pl) =>
                {
                    // 当前实例化对象
                    PrefabBeh.Data d = PrefabBeh.Get(pl.url);
                    GUILayout.Label(d.Text);
                    ParamList ppp = paramList.Get<ParamList>(pl.url);

                    bool isshow = ppp.Get<bool>("showlift", false);
                    EditorGUILayout.LabelField("引用", GUILayout.Width(30f));
                    isshow = EditorGUILayout.Toggle(isshow, GUILayout.ExpandWidth(false), GUILayout.Width(20f));
                    ppp.Set("showlift", isshow);

                    bool isDep = ppp.Get<bool>("deplift", false);
                    EditorGUILayout.LabelField("依赖", GUILayout.Width(30f));
                    isDep = EditorGUILayout.Toggle(isDep, GUILayout.ExpandWidth(false), GUILayout.Width(20f));
                    ppp.Set("deplift", isDep);

                    bool isShowDep = ppp.Get<bool>("deplift_prefab", false);
                    EditorGUILayout.LabelField("预置体依赖", GUILayout.Width(60f));
                    isShowDep = EditorGUILayout.Toggle(isShowDep, GUILayout.ExpandWidth(false), GUILayout.Width(20f));
                    ppp.Set("deplift_prefab", isShowDep);

                    if (isshow)
                    {
                        GUILayout.EndHorizontal();
                        GuiTools.ObjectFieldList<GameObject>(ppp.Get<ParamList>("show-showlift"), d.LiftList, true);
                        GUILayout.BeginHorizontal();
                    }

                    if (isDep)
                    {
                        if (pl.DependenceList != null)
                        {
                            GUILayout.EndHorizontal();
                            GuiTools.ObjectFieldList(ppp.Get<ParamList>("show-deplift"), new List<Object>(pl.DependenceList), true);
                            GUILayout.BeginHorizontal();
                        }
                        else
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.Label("没有依赖!");
                            GUILayout.BeginHorizontal();
                        }
                    }

                    if (isShowDep)
                    {
                        List<PrefabLoad> pls = GetPrefabDep(pl);
                        GUILayout.EndHorizontal();
                        GuiTools.ObjectFieldList<PrefabLoad>(
                            ppp.Get<ParamList>("prefab_dep"),
                            pls,
                            (PrefabLoad xs) => { return xs.asset; },
                            true,
                            null,
                            null,
                            null);

                        GUILayout.BeginHorizontal();
                    }
                });
            EditorGUILayout.EndScrollView();
        }
    }

    PrefabLoadShow mPrefabShow = new PrefabLoadShow();

    void ShowPrefabList()
    {
        mPrefabShow.ShowPrefabList(mParamList, PrefabLoad.GetAllList());
    }

    static List<PrefabLoad> GetPrefabDep(PrefabLoad prefab)
    {
        List<PrefabLoad> pls = new List<PrefabLoad>();
        foreach (KeyValuePair<string, AssetLoadObject> itor in PrefabLoad.GetAllList())
        {
            PrefabLoad pl = itor.Value as PrefabLoad;
            if (pl.GetDependences().Contains(prefab.url))
            {
                pls.Add(pl);
            }
        }

        return pls;
    }
}
#endif