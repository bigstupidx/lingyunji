using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GUIEditor;

namespace PackTool
{
    public class SeeResourcesDependence : EditorWindow
    {
        [MenuItem("PackTool/资源工具/资源依赖查看", false, 9)]
        static public void OpenPackEditorWindow()
        {
            EditorWindow.GetWindow<SeeResourcesDependence>(false, "SeeResourcesDependence", true);
        }

        [MenuItem("PackTool/资源工具/查找RE同名文件", false, 9)]
        static public void FindRESameFile()
        {
            List<string> files = XTools.Utility.GetAllFileList(Application.dataPath);
            ResourcesGroup rg = new ResourcesGroup();
            foreach (string file in files)
                rg.addFile(file);

            rg.clear();
        }

        List<Object> SeeRoots = new List<Object>();

        List<Object> depObjsList = null;
        Vector2 SeeScrollPosition = Vector2.zero;
        bool isShow = false;
        ParamList AllParamList = new ParamList();

        void OnGUI()
        {
            OnSeeDependence();
        }

        // 查看当前依赖
        void OnSeeDependence()
        {
            if (SeeRoots == null || SeeRoots.Count < 1)
            {
                SeeRoots = new List<Object>();
                SeeRoots.Add(null);
                depObjsList = null;
                AllParamList.ReleaseAll();
            }

            EditorGUILayout.BeginHorizontal();
            Object so = EditorGUILayout.ObjectField("查看依赖", SeeRoots[0], typeof(Object), true);
            if (SeeRoots.Count == 1)
            {
                if (SeeRoots[0] != so)
                {
                    SeeRoots.Clear();
                    SeeRoots.Add(so);
                    depObjsList = null;
                    AllParamList.ReleaseAll();
                }
            }

            if (GUILayout.Button("查找选中"))
            {
                SeeRoots.Clear();
                SeeRoots.AddRange(Selection.objects);
                depObjsList = null;
                AllParamList.ReleaseAll();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            isShow = EditorGUILayout.Toggle("显示", isShow);
            EditorGUILayout.LabelField("查找总个数:" + SeeRoots.Count);
            EditorGUILayout.EndHorizontal();

            if (isShow)
            {
                GuiTools.ObjectFieldList(AllParamList.Get<ParamList>("SeeRoots"), SeeRoots, true);
            }

            if (SeeRoots.Count == 0 || (SeeRoots.Count == 1 && SeeRoots[0] == null))
                return;

            if (depObjsList == null)
            {
                depObjsList = new List<Object>();
                DependenceList.DepList depsList = GlobalDependenceList.instance.GetDepsList(SeeRoots.ToArray());
                foreach (string key in depsList.Dic)
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(key, typeof(Object));
                    if (obj == null)
                    {
                        Debug.LogError("null:" + key);
                        continue;
                    }

                    depObjsList.Add(obj);
                }
            }

            if (depObjsList != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(60f);
                EditorGUILayout.LabelField("依赖资源列表:" + depObjsList.Count);
                EditorGUILayout.EndHorizontal();

                SeeScrollPosition = EditorGUILayout.BeginScrollView(SeeScrollPosition);
                GuiTools.ObjectFieldList(AllParamList.Get<ParamList>("depsList"), depObjsList, true);
                EditorGUILayout.EndScrollView();
            }
        }
    }
}