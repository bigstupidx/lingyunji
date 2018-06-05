#if USE_RESOURCESEXPORT
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XTools;
#pragma warning disable 618
namespace PackTool
{
    public class PackAllShader
    {
        [MenuItem("Assets/PackTool/测试Shader/打包所有Shader")]
        [MenuItem("PackTool/测试Shader/打包所有Shader")]
        public static void ExportAllShader()
        {
            BuiltinResource br = AssetDatabase.LoadAssetAtPath<BuiltinResource>("Assets/__copy__/BuiltinResource.prefab");

            // 查找目录下的所有预置体
            HashSet<Shader> shaders = new HashSet<Shader>();
            if (br != null)
                shaders.UnionWith(br.GetShaders());

            FileList fileList = new FileList();
            GlobalCoroutine.StartCoroutine(Utility.ForEachAsync("Assets", (ShaderImporter ai) =>
            {

            },
            (string assetPath) =>
            {
                if (!assetPath.EndsWith(".shader"))
                    return false;

                if (br == null || (assetPath.Contains("/Resources/")))
                    shaders.Add(AssetDatabase.LoadAssetAtPath<Shader>(assetPath));

                return false;
            }, 
            ExportShaders(shaders)));
        }

        static IEnumerator ExportShaders(HashSet<Shader> shaders)
        {
            Debug.LogFormat("total shader:{0}", shaders.Count);
            yield return 0;
            int i = 0;
            foreach (var s in shaders)
            {
                ExportShader(s);
                Debug.LogFormat("ExportShader shader:{0} {1}/{2}", s.name, i + 1, shaders.Count);
                ++i;
                yield return 0;
            }
        }

        [MenuItem("Assets/PackTool/测试Shader/单独Shader")]
        public static void ExportSelectShader()
        {
            // 查找目录下的所有预置体
            foreach (Object obj in Selection.objects)
            {
                if (obj is Shader)
                {
                    ExportShader(obj as Shader);
                }
            }
        }

        static void ExportShader(Shader shader)
        {
            string prefab_path = AssetDatabase.GetAssetPath(shader);
            if (string.IsNullOrEmpty(prefab_path) || !prefab_path.StartsWith("Assets/"))
            {
                prefab_path = "__copy__/BuiltinShader/" + shader.name.Replace('/', '_').Replace('\\', '_') + ".prefab";
            }
            else
            {
                string path = prefab_path.Substring(7);
                int lastpos = path.LastIndexOf('.');
                prefab_path = "__copy__/" + path.Substring(0, lastpos) + ".prefab";
            }

            GameObject prefab = null;
            if (File.Exists(Application.dataPath + "/" + prefab_path))
            {
                prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/" + prefab_path, typeof(GameObject));
            }
            else
            {
                string dstPath = (Application.dataPath + "/" + prefab_path).Replace('\\', '/');
                Directory.CreateDirectory(dstPath.Substring(0, dstPath.LastIndexOf('/')));
                GameObject tmp = new GameObject("");
                prefab = (GameObject)PrefabUtility.CreatePrefab("Assets/" + prefab_path, tmp);
                Object.DestroyImmediate(tmp);
            }

            ShaderExport shaderExport = prefab.GetComponent<ShaderExport>();
            if (shaderExport == null)
                shaderExport = prefab.AddComponent<ShaderExport>();

            shaderExport.mShader = shader;
            UnityEditor.EditorUtility.SetDirty(prefab);
            AssetDatabase.ImportAsset("Assets/" + prefab_path);
            string pack_path = ResourcesPath.streamingAssetsPath + "PackShader/" + prefab_path.Substring(9);
            pack_path = pack_path.Substring(0, pack_path.LastIndexOf('.')) + ".us";
            Directory.CreateDirectory(pack_path.Substring(0, pack_path.LastIndexOf('/')));

            BuildAssetBundleOptions options = BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
            BuildPipeline.BuildAssetBundle(
                prefab,
                null,
                pack_path,
                options,
                UnityEditor.EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
#endif