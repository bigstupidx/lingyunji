#if USE_RESOURCESEXPORT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PackTool
{
    public class PluginsCopy
    {
        [MenuItem("Assets/模块/目录替换", false, 0)]
        static void ModuleReplace()
        {
            Dictionary<string, List<string>> assetsPaths = new Dictionary<string, List<string>>();
            XTools.Utility.ForEachSelect<AssetImporter>(null,
                (string assetPath, string root) =>
                {
                    if (assetPath.EndsWith(".cs", true, null) || assetPath.EndsWith(".dll", true, null) ||
                        assetPath.EndsWith(".cs.meta", true, null) || assetPath.EndsWith(".dll.meta", true, null))
                    {
                        List<string> rs = null;
                        if (!assetsPaths.TryGetValue(root, out rs))
                        {
                            rs = new List<string>();
                            assetsPaths.Add(root, rs);
                        }

                        rs.Add(assetPath);
                    }

                    return false;
                });

            string dst_path = "Assets/Plugins/Art/";
            foreach (var itor in assetsPaths)
            {
                string dp = dst_path + System.IO.Path.GetFileNameWithoutExtension(itor.Key) + "/";
                XTools.Utility.CopyFolder(itor.Key, dp, (string assetPath) =>
                {
                    string src_path = string.Format("{0}/{1}", itor.Key, assetPath.Substring(dp.Length));

                    return itor.Value.Contains(src_path);
                });
            }

            foreach (var itor in assetsPaths)
            {
                foreach (var file in itor.Value)
                    System.IO.File.Delete(file);
            }
        }

        [MenuItem("Assets/模块/导出所有Shader", false, 0)]
        static void ExportShader()
        {
            HashSet<string> exportList = new HashSet<string>();
            XTools.Utility.ForEach("Assets", (AssetImporter ai) =>
            {

            },
            (string assetPath, string root) =>
            {
                if (!assetPath.EndsWith(".shader"))
                    return false;

                if (assetPath.Contains("/Resources/"))
                {
                    string[] deps = AssetDatabase.GetDependencies(assetPath);
                    exportList.Add(assetPath);
                    exportList.UnionWith(deps);
                }

                return false;
            });

            if (exportList.Count != 0)
            {
                string path = Application.dataPath;
                string title = Path.GetFileNameWithoutExtension(path.Substring(0, path.LastIndexOf('/')));
                string file = EditorUtility.SaveFilePanel(title, Application.dataPath, title, "unitypackage");
                if (!string.IsNullOrEmpty(file))
                    AssetDatabase.ExportPackage((new List<string>(exportList)).ToArray(), file, ExportPackageOptions.Default);
            }
        }
    }
}
#endif
