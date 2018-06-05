using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GUIEditor;
#pragma warning disable 414

namespace PackTool
{
    public partial class FindDependence : EditorWindow
    {
        [MenuItem("PackTool/资源工具/使用情况查看", false, 9)]
        static public void OpenPackEditorWindow()
        {
            EditorWindow.GetWindow<FindDependence>(false, "FindDependence", true);
        }

        // 分析的资源
        Object resRoot = null;

        string searchPath;
        string pathPrefix = string.Empty;

        Vector2 ScrollPosition = Vector2.zero;

        ParamList AllParamList = new ParamList();

        List<Object> depList = new List<Object>();
        ResourcesUsedInfo resourcesUsedInfo { get { return ResourcesUsedInfo.my; } }

        // 重置所有资源
        void ResetAllResources(bool clear)
        {
            if (clear == true || resourcesUsedInfo.Count == 0)
            {
                resourcesUsedInfo.Init();
            }
        }

        // 依赖查找
        void DependenceFind()
        {
            resRoot = EditorGUILayout.ObjectField("资源：", resRoot, typeof(Object), true);
            //EditorGUILayout.TextField("搜索路径：", searchPath);
            //EditorGUILayout.TextField("prefix:", pathPrefix);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("检测", GUILayout.Height(50f)))
            {
                if (resRoot == null)
                    return;

                AllParamList.ReleaseAll();
                string ss = AssetDatabase.GetAssetPath(resRoot);
                ResetAllResources(false);
                var deps = resourcesUsedInfo.GetUsedInfo(ss);

                depList.Clear();
                depList.Capacity = deps.Count;
                foreach (string ap in deps)
                {
                    depList.Add(AssetDatabase.LoadAssetAtPath<Object>(ap));
                }
            }

            //if (GUILayout.Button("设置路径", GUILayout.Height(50)))
            //{
            //    searchPath = EditorUtility.OpenFolderPanel("搜索路径", searchPath, "");
            //    if (string.IsNullOrEmpty(searchPath))
            //        searchPath = Application.dataPath;

            //    if (searchPath != Application.dataPath)
            //        pathPrefix = searchPath.Substring(Application.dataPath.Length + 1);
            //    else
            //        pathPrefix = string.Empty;

            //    if (!string.IsNullOrEmpty(pathPrefix))
            //        pathPrefix = pathPrefix + "/";
            //}

            if (GUILayout.Button("重建索引", GUILayout.Height(50f)))
            {
                ResetAllResources(true);
            }

            EditorGUILayout.EndHorizontal();

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            EditorGUILayout.LabelField("引用总个数:" + depList.Count);
            GuiTools.ObjectFieldList(AllParamList, depList, false);
            EditorGUILayout.EndScrollView();
        }

        void OnGUI()
        {
            //if (string.IsNullOrEmpty(searchPath))
            //    searchPath = ResourcesPath.dataPath;

            GUILayout.Label("这个工具的作用是查找使用此份资源的预置体!");
            DependenceFind();
        }

        Vector2 ScrollPosition1 = Vector2.zero;

        EditorPageBtn mPageBtn = new EditorPageBtn();
    }
}