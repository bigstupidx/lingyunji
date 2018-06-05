#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using PackTool;
using GUIEditor;
using UnityEngine.Profiling;

partial class RunningResourcesEditor : BaseEditorWindow
{
    class RACShow
    {
        string search_key = "";

        Vector2 ScrollPosition = Vector2.zero;

        Dictionary<int, int> racMemorys = new Dictionary<int, int>();

        int GetTotalMemory(RuntimeAnimatorController rac)
        {
            int v = 0;
            if (racMemorys.TryGetValue(rac.GetInstanceID(), out v))
                return v;

            v = GetR(rac);
            racMemorys.Add(rac.GetInstanceID(), v);
            return v;
        }

        public void Show(ParamList paramList)
        {
            search_key = EditorGUILayout.TextField("搜索key", search_key);

            // 查看下纹理的使用情况
            List<RunAnimContLoad> racs = new List<RunAnimContLoad>();
            {
                foreach (var itor in RunAnimContLoad.GetAllList())
                {
                    if (!string.IsNullOrEmpty(search_key) && !itor.Key.Contains(search_key))
                        continue;

                    racs.Add(itor.Value as RunAnimContLoad);
                }
            }

            EditorGUILayout.LabelField("动画控制器");//不导出中文
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            GuiTools.ObjectFieldList<RunAnimContLoad>(
                paramList,
                racs,
                (RunAnimContLoad rac) => { return rac.asset; },
                false,
                (List<RunAnimContLoad> items, ParamList pl)=> 
                {
                    items.Sort((RunAnimContLoad x, RunAnimContLoad y)=> { return GetTotalMemory(y.asset).CompareTo(GetTotalMemory(x.asset)); });
                },
                (RunAnimContLoad rac) =>
                {
                    EditorGUILayout.LabelField("内存:" + XTools.Utility.ToMb(GetR(rac.asset)), GUILayout.Width(90f));
                },
                (RunAnimContLoad rac) =>
                {
                    ParamList pl = paramList.Get<ParamList>("rac-" + rac.asset.GetInstanceID());

                    bool isshow = pl.Get<bool>("show", false);
                    EditorGUILayout.LabelField("显示", GUILayout.Width(30f));
                    isshow = EditorGUILayout.Toggle(isshow, GUILayout.Width(20f));
                    pl.Set("show", isshow);
                    if (isshow)
                    {
                        List<AnimationClip> clips = new List<AnimationClip>();
                        clips.AddRange(rac.asset.animationClips);
                        EditorGUILayout.EndHorizontal();
                        GuiTools.ObjectFieldList<AnimationClip>(
                            pl.Get<ParamList>("animationClips"),
                            clips,
                            (AnimationClip xs) => { return xs; },
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

    static int GetR(RuntimeAnimatorController rac)
    {
        int total = Profiler.GetRuntimeMemorySize(rac);
        foreach (var clip in rac.animationClips)
            total += Profiler.GetRuntimeMemorySize(clip);

        return total;
    }

    RACShow mRACShow = new RACShow();

    void ShowRACShow()
    {
        mRACShow.Show(mParamList.Get("RACShow", ()=> { return new ParamList(); }));
    }
}
#endif