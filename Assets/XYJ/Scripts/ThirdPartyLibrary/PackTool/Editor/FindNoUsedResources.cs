using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GUIEditor;

namespace PackTool
{
    public partial class FindNoUsedResources : EditorWindow
    {
        [MenuItem("PackTool/资源工具/未使用的资源", false, 9)]
        static public void OpenPackEditorWindow()
        {
            EditorWindow.GetWindow<FindNoUsedResources>(false, "FindNoUsedResources", true);
        }

        // 查找未被使用的资源,主要是动画和模型
        List<Object> FindNoUseRes()
        {
            List<Object> allList = CurrentUsedResources.instance.GetAllList();

            HashSet<string> allMapList = new HashSet<string>();
            foreach (Object o in allList)
            {
                string assetPath = AssetDatabase.GetAssetPath(o);
                if (allMapList.Contains(assetPath))
                    continue;
                DependenceList.DepList dl = GlobalDependenceList.instance.GetDepList(assetPath);
                foreach (string key in dl.Dic)
                {
                    allMapList.Add(key);
                }
            }
            Debug.Log("使用的资源总个数:" + allMapList.Count);

            Dictionary<Object, int> noMap = new Dictionary<Object, int>();
            List<Object> notUseList = new List<Object>();
            {
                List<string> currentList = GetCurrentAllResources();
                Debug.Log("资源总个数:" + currentList.Count);
                foreach (string obj in currentList)
                {
                    if (allMapList.Contains(obj))
                        continue;

                    Object o = AssetDatabase.LoadAssetAtPath(obj, typeof(Object));
                    if (o == null)
                    {
                        Debug.Log("null:" + obj);
                        continue;
                    }
                    if (noMap.ContainsKey(o))
                        continue;
                    noMap.Add(o, 1);
                    notUseList.Add(o);
                }
            }

            return notUseList;
        }

        List<string> GetCurrentAllResources()
        {
            List<string> allList = new List<string>();
            string[] ss = AssetDatabase.FindAssets("*.*", new string[] { "Assets/Scene", "Assets/Fbxs", "Assets/UIData" });
            foreach (string s in ss)
            {
                string p = AssetDatabase.GUIDToAssetPath(s);
                string lp = p.ToLower().Replace("\\", "/");
                if (lp.Contains("/editor/") ||
                    lp.EndsWith(".cs") ||
                    lp.EndsWith(".txt") ||
                    lp.EndsWith(".xml") ||
                    lp.EndsWith(".js"))
                    continue;

                if (p.Contains("."))
                    allList.Add(p);
            }

            Debug.Log(allList.Count);
            return allList;
        }

        List<Object> NoUseList = new List<Object>();

        Vector2 ScrollPosition = Vector2.zero;

        ParamList AllParamList = new ParamList();

        void OnGUI()
        {
            OnFindOnUseGUI();
        }

        void OnFindOnUseGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("查找未使用的资源", GUILayout.Height(50f)))
            {
                AllParamList.ReleaseAll();
                NoUseList = FindNoUseRes();
            }

            if (GUILayout.Button("重新检测使用的资源", GUILayout.Height(50f)))
            {
                CurrentUsedResources.instance.FindAllResources();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("未使用资源总个数:" + NoUseList.Count);
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            int index = 0;
            while ((index = NoUseList.IndexOf(null)) != -1)
                NoUseList.RemoveAt(index);

            GuiTools.ObjectFieldList(AllParamList, NoUseList, false);
            EditorGUILayout.EndScrollView();
        }
    }
}