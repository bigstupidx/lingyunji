#if USE_ABL
//#define TESTRES
using System.IO;
using UnityEngine;
using UnityEditor;
using PackTool.New;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

namespace ABL
{
    // 打包
    public class BuildAllResources
    {
        static void SetAlawyIncludeShader(List<Shader> shaders)
        {
            Dictionary<Shader, int> currents = new Dictionary<Shader, int>(); // 当前设置的
            List<int> emptys = new List<int>();
            SerializedObject gs = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty alwaysIncludedShaders = gs.FindProperty("m_AlwaysIncludedShaders");

            int arraySize = alwaysIncludedShaders.arraySize;
            for (int i = 0; i < arraySize; ++i)
            {
                SerializedProperty sp = alwaysIncludedShaders.GetArrayElementAtIndex(i);
                if (sp.objectReferenceValue == null)
                {
                    emptys.Add(i);
                }
                else
                {
                    currents.Add(sp.objectReferenceValue as Shader, i);
                }
            }

            bool isDirty = false;
            for (int i = 0; i < shaders.Count; ++i)
            {
                if (currents.ContainsKey(shaders[i]))
                {
                    // 当前已经存在了
                }
                else
                {
                    isDirty = true;

                    if (emptys.Count != 0)
                    {
                        alwaysIncludedShaders.GetArrayElementAtIndex(emptys[0]).objectReferenceValue = shaders[i];
                        emptys.RemoveAt(0);
                    }
                    else
                    {
                        arraySize = alwaysIncludedShaders.arraySize;
                        alwaysIncludedShaders.InsertArrayElementAtIndex(arraySize);
                        alwaysIncludedShaders.GetArrayElementAtIndex(arraySize).objectReferenceValue = shaders[i];
                    }
                }
            }

            if (isDirty)
            {
                gs.ApplyModifiedProperties();
            }
        }

        public static bool SetAssetBundle(string assetPath)
        {
            AssetImporter ai = AssetImporter.GetAtPath(assetPath);
            string assetBundleName = GetAssetBundleName(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
            if (!string.Equals(assetBundleName, ai.assetBundleName, System.StringComparison.InvariantCultureIgnoreCase))
            {
                ai.assetBundleName = assetBundleName;
                EditorUtility.SetDirty(ai);
                return true;
            }

            return false;
        }
        
        // 一份需要打包的资源
        class PackAsset
        {
            public string assetPath;
            public int dependTotal { get { return depends.Count; } }
            public List<string> depends = new List<string>();
            public bool isExport = false; // 是否导出资源
            public bool isSprite = false; // 是否精灵
            public string spritePackingTag; // 精灵Tag
        }

        // 当前需要打包的所有资源
        Dictionary<string, PackAsset> Assets = new Dictionary<string, PackAsset>();

        public static string GetAssetBundleName(Object obj)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (obj is Sprite || obj is Texture2D)
            {
                TextureImporter ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (ti.textureType == TextureImporterType.Sprite)
                    return ti.spritePackingTag.ToLower();
            }

            if (assetPath.StartsWith("Assets/__copy__/"))
                return assetPath.Substring(16).ToLower();

            return assetPath.Substring(7).ToLower();
        }

        public static readonly string Folder = ResourcesPath.LocalBasePath;

        // 打包路径
        static public readonly string PackPath = Folder + "Data/";

        public IEnumerator BuildAll(System.Action onEnd)
        {
            IEnumerator itor = SetAssetBundles(null);
            while (itor.MoveNext())
                yield return 0;

            var Config = AutoExportAll.LoadConfig();
            BuildAssetBundles(Config, PackPath + "AB");
            XTools.Utility.CopyConfigToPath(PackPath);
            yield return 0;
            BuildPack(Config);
            yield return 0;

            if (onEnd != null)
                onEnd();
        }

        public IEnumerator SetAssetBundles(System.Action onEnd)
        {
            var spritePackerMode = EditorSettings.spritePackerMode;
            if (spritePackerMode != SpritePackerMode.Disabled)
                EditorSettings.spritePackerMode = SpritePackerMode.Disabled;

            FileAtlerCheck.Begin();

            HashSet<string> dirtyList = new HashSet<string>(); // 需要重新导的资源

            List<PackAsset> allAssets = new List<PackAsset>();
            int index = 0;
            var current = EditorSceneManager.GetActiveScene();
            IEnumerator ator = XTools.Utility.ForEachAsync("Assets", (AssetImporter ai) =>
            {
                if (ai.assetPath.EndsWith(".unity"))
                {
                    string assetPath = "Assets/__copy__/" + ai.assetPath.Substring(7);
                    if (FileAtlerCheck.isNeedUpdate(ai.assetPath) || !File.Exists(assetPath))
                    {
                        Directory.CreateDirectory(assetPath.Substring(0, assetPath.LastIndexOf('/')));
                        AssetDatabase.CopyAsset(ai.assetPath, assetPath);

                        var scene = EditorSceneManager.OpenScene(assetPath);
                        GameObject newRoot = XTools.Utility.ResetSceneRoot(scene);

                        newRoot.AddComponent<PackTool.SceneRoot>().SaveScene();
                        string newprefabPath = assetPath.Substring(0, assetPath.LastIndexOf('.')) + ".prefab";
                        Debug.LogFormat("newprefabPath:{0}", newprefabPath);
                        GameObject newPrefab = PrefabUtility.CreatePrefab(newprefabPath, newRoot);
                        newPrefab.SetActive(false);
                        Object.DestroyImmediate(newRoot);
                        newPrefab = Object.Instantiate(newPrefab);
                        newPrefab.name = Path.GetFileNameWithoutExtension(newprefabPath);
                        AssetDatabase.DeleteAsset(newprefabPath);
                        EditorSceneManager.SaveScene(scene);

//                         scene = EditorSceneManager.OpenScene(assetPath);
//                         PackTool.SceneRoot.SetCurrentBatching();
//                         EditorSceneManager.SaveScene(scene);

                        ai = AssetImporter.GetAtPath(assetPath);
                    }
                }

                PackAsset asset = new PackAsset() { assetPath = ai.assetPath, isExport = true };
                Assets.Add(ai.assetPath, asset);
                allAssets.Add(asset);

                Debug.LogFormat("export {0} url:{1}", index++, asset.assetPath);
            },
            (string assetPath)=> 
            {
                if (assetPath.StartsWith("Assets/__copy__/"))
                    return false;

                // 特定目录下的特定资源才允许导出
                if (assetPath.Contains("/ResourcesExport/") && assetPath.EndsWith(".prefab", true, null))
                    return true;

                if (assetPath.Contains("/MaterialExport/") && assetPath.EndsWith(".mat", true, null))
                    return true;

                if (XTools.Utility.HasExportScene(assetPath))
                    return true;

                if (assetPath.Contains("/ShaderExport/") && assetPath.EndsWith(".shader", true, null))
                    return true;

                return false;
            });

            while (ator.MoveNext())
                yield return 0;

            FileAtlerCheck.End();

            if (current.IsValid() && EditorSceneManager.GetActiveScene().path != current.path)
                EditorSceneManager.OpenScene(current.path);

            // 需要打包的资源查找完毕，现在开始计算查找下对应的依赖的资源
            for (int i = 0; i < allAssets.Count; ++i)
            {
                var asset = allAssets[i];
                string[] deps = AssetDatabase.GetDependencies(asset.assetPath, false);
                Debug.LogFormat("Check Dep:{0} {1}/{2}", asset.assetPath, i, allAssets.Count);
                for (int j = 0; j < deps.Length; ++j)
                {
                    AddAsset(deps[j]);
                    yield return 0;
                }
            }

            Dictionary<string, List<Sprite>> allSprites = xys.UI.CheckAllPanel.ExportSpriteInfo(PackPath + "sprites_atlas.b");
            foreach (var itor in allSprites)
            {
                foreach (var sprite in itor.Value)
                {
                    string assetPath = AssetDatabase.GetAssetPath(sprite);
                    if (!string.IsNullOrEmpty(assetPath) && !Assets.ContainsKey(assetPath))
                    {
                        PackAsset asset = new PackAsset() { assetPath = assetPath, isExport = true };
                        Assets.Add(assetPath, asset);
                        allAssets.Add(asset);
                    }
                }
            }

            // 开始设置下AssetBundleName
            HashSet<string> currentABNames = new HashSet<string>(AssetDatabase.GetAllAssetPaths());
            foreach (string name in currentABNames)
            {
                if (Assets.ContainsKey(name))
                    continue;

                AssetImporter ai = AssetImporter.GetAtPath(name);
                if (string.IsNullOrEmpty(ai.assetBundleName))
                    continue;

                ai.assetBundleName = string.Empty;
                UnityEditor.EditorUtility.SetDirty(ai);
                dirtyList.Add(name);
                yield return 0;
            }

            foreach (var itor in Assets)
            {
                PackAsset asset = itor.Value;
                if (asset.isExport || asset.dependTotal >= 2 || asset.isSprite || asset.assetPath.EndsWith(".mat"))
                {
                    if (SetAssetBundle(asset.assetPath))
                    {
                        dirtyList.Add(itor.Key);

                        Debug.LogFormat("SetAssetBundle:{0}", asset.assetPath);
                        yield return 0;
                    }
                }
            }

            AssetDatabase.Refresh();
            AssetDatabase.RemoveUnusedAssetBundleNames();

            if (spritePackerMode != SpritePackerMode.Disabled)
            {
                EditorSettings.spritePackerMode = spritePackerMode;
            }
            Debug.LogFormat("设置ABName完成!");
            if (onEnd != null)
                onEnd();
        }

        public static void BuildAssetBundles(Versions Config, string path)
        {
            Directory.CreateDirectory(path);
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

            GenResourceConfig(Config);

            PackTool.AssetsExport.UpdatePack();
        }

        public static void GenResourceConfig(Versions Config)
        {
            PackTool.ResourceConfig resconfig = new PackTool.ResourceConfig();
            resconfig.version = (Config.resVersion == null ? Config.startVersion : Config.resVersion);
            resconfig.svn = Config.svn;
            resconfig.SaveToFile();
        }

        static public void BuildPack(Versions Config)
        {
            foreach (VersionConfig vc in Config.configs)
            {
                // 检查下编辑器设置
                vc.editor.Check(Config, vc);

                PackBuildProgram program = new PackBuildProgram();
                program.BuildPack(Config, vc);
            }
        }

        public static bool IsExportResources(string assetPath)
        {
            if (assetPath.EndsWith(".cs", true, null) ||
                assetPath.EndsWith(".js", true, null) ||
                assetPath.EndsWith(".mask", true, null) ||
                assetPath.EndsWith(".dll", true, null))
                return false;

            return true;
        }

        void AddAsset(string assetPath)
        {
            if (!IsExportResources(assetPath))
                return;

            PackAsset da = null;
            if (Assets.TryGetValue(assetPath, out da))
            {
                da.depends.Add(assetPath);
                return;
            }

            da = new PackAsset() { assetPath = assetPath, isExport = false };
            da.depends.Add(assetPath);
            TextureImporter ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (ti != null)
            {
                if (ti.textureType == TextureImporterType.Sprite)
                {
                    da.isSprite = true;
                    da.spritePackingTag = ti.spritePackingTag;
                }
            }
            Assets.Add(assetPath, da);
            //Debug.LogFormat("Depends:{0}", assetPath);

            string[] deps = AssetDatabase.GetDependencies(assetPath, false);
            for (int i = 0; i < deps.Length; ++i)
                AddAsset(deps[i]);
        }
    }
}
#endif