using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GUIEditor;
using XTools;

namespace PackTool
{
#if !SCENE_DEBUG
    public partial class FindNoUsedPrefab : EditorWindow
    {
        [MenuItem("PackTool/资源工具/预置体", false, 9)]
        static public void OpenPackEditorWindow()
        {
            EditorWindow.GetWindow<FindNoUsedPrefab>(false, "FindNoUsedPrefab", true);
        }

        // 得到当前所有的预置体
        static public HashSet<string> GetAllPrefabs()
        {
            HashSet<string> all_files = new HashSet<string>();
            //PackResources uiprefabs = new PackResources(); // 预置体资源
            List<string> files = XTools.Utility.GetAllFileList(Application.dataPath);
            PackResources Prefabs = new PackResources();
            PackResources.GetResources<GameObject>(Prefabs, files, null, new string[] { "/ResourcesExport/" }, new string[] { ".prefab" });

            ResourcesGroup.Clear();
            for (int i = 0; i < Prefabs.Keys.Count; ++i)
            {
                ResourcesGroup.AddFile(Prefabs.Keys[i]);
                all_files.Add(Prefabs.Keys[i]);
            }

            return all_files;
        }

        // 当前正在使用当中的预置体
        HashSet<string> usedPrefabs = new HashSet<string>();

        // 未使用的预置体
        List<GameObject> nousedPrefabs = new List<GameObject>();

        // 未使用的模型
        List<GameObject> nousedFbxs = new List<GameObject>();

        ParamList mParamList = new ParamList();
        ParamList mParamList1 = new ParamList();

        void Release()
        {
            mParamList.ReleaseAll();
            mParamList1.ReleaseAll();
            usedPrefabs.Clear();
            nousedPrefabs.Clear();
            nousedFbxs.Clear();
        }

        void OnGUI()
        {
            if (GUILayout.Button("查找资源", GUILayout.Width(100f), GUILayout.Height(50f)))
            {
                Release();

                HashSet<string> useds = new HashSet<string>();
                HashSet<string> all_files = GetAllPrefabs();
                usedPrefabs = GetScenePrefab();
                DependenceList deplist = new DependenceList();
                HashSet<string> all_used_files = new HashSet<string>(); // 所有使用的文件

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (string path in useds)
                {
                    string full_path = ResourcesGroup.GetFullPath(ResourcesGroup.Prefab, path + ".prefab");
                    if (string.IsNullOrEmpty(full_path))
                    {
                        Debug.LogError(path + "查找失败!");
                        sb.AppendLine(path);
                    }
                    else
                    {
                        all_used_files.Add(full_path);
                        usedPrefabs.Add(full_path);
                        foreach (string s in deplist.GetDepList("Assets/" + full_path).Dic)
                        {
                            all_used_files.Add(s.Substring(7));
                            if (s.EndsWith(".prefab"))
                                usedPrefabs.Add(s.Substring(7));
                        }
                    }
                }

                Debug.Log("资源查找失败:\n\r" + sb.ToString());

                foreach (string file in all_files)
                {
                    if (IsFixResourcesGroup(file))
                    {
                        usedPrefabs.Add(file);
                        all_used_files.Add(file);
                        foreach (string s in deplist.GetDepList("Assets/" + file).Dic)
                        {
                            all_used_files.Add(s.Substring(7));
                            if (s.EndsWith(".prefab"))
                                usedPrefabs.Add(s.Substring(7));
                        }
                    }
                }

                foreach (string file in all_files)
                {
                    if (!usedPrefabs.Contains(file))
                        nousedPrefabs.Add(AssetDatabase.LoadAssetAtPath("Assets/" + file, typeof(GameObject)) as GameObject);
                }

                HashSet<string> all_fbx_assets = FindFbxAssets();
                sb.Length = 0;
                foreach (string file in all_fbx_assets)
                {
                    if (!all_used_files.Contains(file))
                    {
                        nousedFbxs.Add(AssetDatabase.LoadAssetAtPath("Assets/" + file, typeof(GameObject)) as GameObject);
                        sb.AppendLine(file);
                    }
                }

                Debug.Log("未使用的模型:\n\r" + sb.ToString());
            }

            GuiTools.ObjectFieldList(mParamList, nousedPrefabs, false);
            GuiTools.ObjectFieldList(mParamList1, nousedFbxs, false);
        }
        
        // 查找所有的模型
        static HashSet<string> FindFbxAssets()
        {
            // 查找所有的fbx模型下的资源
            string[] res = AssetDatabase.FindAssets("*.*", new string[] { "Assets/Fbxs/roles" });
            HashSet<string> files = new HashSet<string>();
            foreach (string s in res)
            {
                string f = AssetDatabase.GUIDToAssetPath(s);
                if (f.StartsWith("Assets/Fbxs/roles/player/") || f.StartsWith("Assets/Fbxs/roles/ResourcesExport/"))
                    continue;

                if (!f.EndsWith(".fbx", true, null))
                    continue;

                files.Add(f.Substring(7));
            }

            return files;
        }

        static HashSet<string> GetScenePrefab()
        {
            HashSet<string> files = new HashSet<string>();
            List<string> sceneslist = BuildSceneList.GetAllSceneList();
            DependenceList deplist = new DependenceList();
            foreach (string s in sceneslist)
            {
                foreach (string path in deplist.GetDepList(s).Dic)
                {
                    if (path.EndsWith(".prefab"))
                        files.Add(path.Substring(7));
                }
            }

            return files;
        }


        string[] fix_paths = new string[] 
        { 
            "Fbxs/roles/player/ResourcesExport",
            "Scene/Effects/Res/Skill_nodds/ResourcesExport",
            "Scene/Effects/Res/Skill_nodds2/ResourcesExport",
            "Prefabs/SimplePath/ResourcesExport"
        };

        bool IsFixResourcesGroup(string file)
        {
            foreach (string f in fix_paths)
            {
                if (file.StartsWith(f))
                    return true;
            }

            return false;
        }
    }
#endif
}