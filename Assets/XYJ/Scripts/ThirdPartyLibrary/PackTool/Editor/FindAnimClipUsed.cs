using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GUIEditor;
#pragma warning disable 414

namespace PackTool
{
    public partial class FindAnimClipUsed : EditorWindow
    {
        [MenuItem("PackTool/资源工具/模型动画使用情况", false, 9)]
        static public void OpenPackEditorWindow()
        {
            EditorWindow.GetWindow<FindAnimClipUsed>(false, "FindAnimClipUsed", true);
        }

        // 分析的资源
        Object resRoot = null;

        Vector2 ScrollPosition = Vector2.zero;

        List<Object> depList = new List<Object>();

        // 当前所有的控制器
        List<RuntimeAnimatorController> RuntimeAnimatorControllers = new List<RuntimeAnimatorController>();

        ParamList AllParamList = new ParamList();

        List<XTools.Pair<AnimationClip, List<RuntimeAnimatorController>>> AllList = new List<XTools.Pair<AnimationClip, List<RuntimeAnimatorController>>>();

        // 重置所有资源
        void ResetAllResources(bool clear)
        {
            if (clear == true)
                RuntimeAnimatorControllers.Clear();

            if (RuntimeAnimatorControllers.Count == 0)
            {
                AllParamList.ReleaseAll();
                AllList.Clear();
                HashSet<string> assets = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { "Assets" }));
                Dictionary<AnimationClip, List<RuntimeAnimatorController>> AnimationClips = new Dictionary<AnimationClip, List<RuntimeAnimatorController>>();
                foreach (string asset in assets)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(asset);
                    if (!assetPath.EndsWith(".controller"))
                        continue;

                    RuntimeAnimatorController rac = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(assetPath);
                    if (rac == null)
                        continue;

                    foreach (AnimationClip clip in rac.animationClips)
                    {
                        List<RuntimeAnimatorController> racs = null;
                        if (!AnimationClips.TryGetValue(clip, out racs))
                        {
                            racs = new List<RuntimeAnimatorController>();
                            AnimationClips.Add(clip, racs);
                            AllList.Add(new XTools.Pair<AnimationClip, List<RuntimeAnimatorController>>(clip, racs));
                        }

                        racs.Add(rac);
                    }
                }

                AllList.Sort((XTools.Pair<AnimationClip, List<RuntimeAnimatorController>> x, XTools.Pair<AnimationClip, List<RuntimeAnimatorController>> y) => 
                {
                    return x.second.Count.CompareTo(y.second.Count);
                });
            }
        }

        bool isShowMut = false;

        // 依赖查找
        void DependenceFind()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("检测", GUILayout.Height(50f)))
            {
                if (resRoot == null)
                    return;

                AllParamList.ReleaseAll();
                depList.Clear();
                AllList.Clear();
                //string ss = AssetDatabase.GetAssetPath(resRoot);
                ResetAllResources(false);
            }

            if (GUILayout.Button("重建索引", GUILayout.Height(50f)))
            {
                ResetAllResources(true);
            }

            EditorGUILayout.EndHorizontal();

            isShowMut = EditorGUILayout.Toggle("只显示多个", isShowMut);
            List<XTools.Pair<AnimationClip, List<RuntimeAnimatorController>>> shows = AllList;
            if (isShowMut)
            {
                shows = new List<XTools.Pair<AnimationClip, List<RuntimeAnimatorController>>>();
                for (int i = 0; i < AllList.Count; ++i)
                {
                    if (AllList[i].second.Count >= 2)
                        shows.Add(AllList[i]);
                }
            }

            mPageBtn.total = shows.Count;
            mPageBtn.pageNum = 30;
            mPageBtn.OnRender();
            mPageBtn.ForEach((int index)=> 
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(shows[index].first, typeof(AnimationClip), false);
                EditorGUILayout.LabelField("Count:" + shows[index].second.Count);
                bool isshow = AllParamList.Get(index.ToString(), false);
                isshow = EditorGUILayout.Toggle("显示", isshow);
                AllParamList.Set(index.ToString(), isshow);

                EditorGUILayout.EndHorizontal();

                if (isshow)
                {
                    for (int i = 0; i < shows[index].second.Count; ++i)
                    {
                        Color c = GUI.color;
                        GUI.color = Color.yellow;
                        EditorGUILayout.ObjectField(shows[index].second[i], typeof(RuntimeAnimatorController), false);
                        GUI.color = c;
                    }
                }
            });
        }

        void OnGUI()
        {
            GUILayout.Label("FindAnimClipUsed!");
            DependenceFind();
        }

        Vector2 ScrollPosition1 = Vector2.zero;

        EditorPageBtn mPageBtn = new EditorPageBtn();
    }
}