#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Reflection;
#pragma warning disable 0618

namespace PackTool
{
    public partial class AssetsExport
    {
        [MenuItem("Assets/PackTool/清除默认数据")]
        static void ClearDefaultResources()
        {
            int total = 0;
            XTools.Utility.ForEach<AssetImporter>("Assets",
                (assetImporter) => 
                {
                    var deps = new HashSet<string>(AssetDatabase.GetDependencies(assetImporter.assetPath));
                    deps.Remove(assetImporter.assetPath);
                    if (assetImporter.assetPath.EndsWith(".shader"))
                    {
                        foreach (var d in new List<string>(deps))
                        {
                            if (d.EndsWith(".shader"))
                                deps.Remove(d);
                        }
                    }

                    if (deps.Count == 0)
                        return;

                    ++total;
                    Debug.LogFormat("清除:{0}", assetImporter.assetPath);
                    if (assetImporter is MonoImporter)
                    {
                        var mono = assetImporter as MonoImporter;
                        mono.SetDefaultReferences(new string[0], new Object[0]);
                    }
                    else if (assetImporter is ShaderImporter)
                    {
                        var shader = assetImporter as ShaderImporter;
                        shader.SetDefaultTextures(new string[0], new Texture[0]);
                    }

                    EditorUtility.SetDirty(assetImporter);
                    AssetDatabase.ImportAsset(assetImporter.assetPath);
                },
                (string assetPath, string root) => 
                {
                    return assetPath.EndsWith(".cs") || assetPath.EndsWith(".shader");
                });

            Debug.LogFormat("总数:{0}", total);
        }

        // 导出使用到的内置的shader
        static void ExportBuiltinResources(List<Object> objs, List<Material> mats)
        {
            string path = "__copy__/" + BuiltinResource.filepath;
            GameObject targetPrefab = null;
            if (File.Exists(Application.dataPath + "/" + path))
            {
                targetPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/" + path, typeof(GameObject));
            }
            else
            {
                string dstPath = (Application.dataPath + "/" + path).Replace('\\', '/');
                Directory.CreateDirectory(dstPath.Substring(0, dstPath.LastIndexOf('/')));
                GameObject tmp = new GameObject("");
                targetPrefab = (GameObject)PrefabUtility.CreatePrefab("Assets/" + path, tmp);
                Object.DestroyImmediate(tmp);
            }

            BuiltinResource builtinExport = targetPrefab.GetComponent<BuiltinResource>();
            if (builtinExport == null)
            {
                builtinExport = targetPrefab.AddComponent<BuiltinResource>();
            }

            bool isdirty = false;
            if (objs != null)
            {
                for (int i = 0; i < objs.Count; ++i)
                {
                    if (builtinExport.Add(objs[i]))
                        isdirty = true;
                }
            }

            if (mats != null)
            {
                for (int i = 0; i < mats.Count; ++i)
                {
                    builtinExport.AddMaterial(mats[i]);
                    isdirty = true;
                }
            }

            if (isdirty)
            {
                UnityEditor.EditorUtility.SetDirty(targetPrefab);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                ImportAsset("Assets/" + path);
            }

            BuildAssetBundleOptions options = BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle;
            ExportPath(targetPrefab, BuiltinResource.filepath, options);
        }

        [MenuItem("Assets/PackTool/ExportBuiltinResources")]
        static void ExportBuiltinResources111()
        {
            ExportBuiltinResources(null, null);
            CodeCheckAtler.SaveToFile();
        }
    }
}
#endif