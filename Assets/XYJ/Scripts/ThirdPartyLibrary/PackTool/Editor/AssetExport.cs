#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using XTools;
#if USE_UATLAS
using ATLAS = UI.uAtlas;
#else
using ATLAS = xys.UI.Atlas;
#endif

namespace PackTool
{
    public class AssetExportEx
    {
        [MenuItem("Assets/动画/循环")]
        public static void SetAnimationLoop()
        {
            foreach (Object o in Selection.objects)
            {
                if (o as AnimationClip)
                    ((AnimationClip)o).wrapMode = WrapMode.Loop;
            }
        }

        [MenuItem("Assets/动画/一次")]
        public static void SetAnimationOnce()
        {
            foreach (Object o in Selection.objects)
            {
                if (o as AnimationClip)
                    ((AnimationClip)o).wrapMode = WrapMode.Once;
            }
        }


        [MenuItem("PackTool/动画/查找不规范动画")]
        public static void FindAnimation()
        {
            int total = 0;
            List<AnimationClip> clips = new List<AnimationClip>();
            {
                FileList mFileList = new FileList();
                PackResources mMatRes = new PackResources();
                PackResources.GetResources<AnimationClip>(mMatRes, mFileList.GetFiles(Application.dataPath), null, null, new string[] { ".anim" });
                foreach (AnimationClip mat in mMatRes.GetList<AnimationClip>())
                    clips.Add(mat);
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string assetPath;
            foreach (AnimationClip clip in clips)
            {
                assetPath= AssetDatabase.GetAssetPath(clip);
                string[] deps = AssetDatabase.GetDependencies(new string[] { assetPath });
                if (deps.Length != 0)
                {
                    bool add = false;
                    foreach (string dep in deps)
                    {
                        if (dep == assetPath)
                            continue;
                        if (dep.EndsWith(".cs") || dep.EndsWith(".js"))
                            continue;

                        if (add == false)
                        {
                            sb.AppendLine(string.Format("assetPath:{0}", AssetDatabase.GetAssetPath(clip)));
                            ++total;
                            add = true;
                        }

                        sb.AppendLine(string.Format("   dep:{0}", dep));
                    }
                }
            }

            string text = string.Format("total({0}):{1}\r\n{2}", clips.Count, total, sb.ToString());
            Debug.Log(text);
        }

        [MenuItem("Assets/PackTool/Prefab")]
        public static void PrefabExport()
        {
            CodeCheckAtler.Release();
            AssetsExport mgr = new AssetsExport();
            //List<GameObject> prefabs = new List<GameObject>();
            Utility.ForEachSelect((AssetImporter assetImporter) => 
            {
                mgr.CollectPrefab(AssetDatabase.LoadAssetAtPath<GameObject>(assetImporter.assetPath));
            },
            (string assetPath, string root) => 
            {
                return assetPath.EndsWith(".prefab");
            });

            if (!mgr.isEmpty)
            {
                GlobalCoroutine.StartCoroutine(mgr.BeginPack());
            }
        }

        [MenuItem("Assets/PackTool/ExportPrefab")]
        static void ExportPrefabImp()
        {
            AssetsExport.ExportPrefab(Selection.activeGameObject);
        }

        [MenuItem("Assets/PackTool/Prefab(Colllect)")]
        static void ExportPrefab()
        {
            CodeCheckAtler.Release();
            PackCollectList pcl = new PackCollectList();
            foreach (GameObject go in Selection.gameObjects)
                pcl.CollectPrefab(go);

            if (!pcl.isEmpty)
            {
                AssetsExport mgr = new AssetsExport(true);
                GlobalCoroutine.StartCoroutine(pcl.BeginCollect(mgr));
            }
        }

        //[MenuItem("Assets/PackTool/Atlas")]
        //public static void ExportAtlas()
        //{
        //    UnityEditor.Sprites.Packer.RebuildAtlasCacheIfNeeded(AssetsExport.GetBuildTarget());
        //    AssetsExport.ExportAtlas(Selection.activeGameObject.GetComponent<UI.Atlas>());
        //}

        [MenuItem("Assets/PackTool/AlluAtlas")]
        public static void ExportAlluAtlas()
        {
            AssetsExport ae = new AssetsExport(true);
            GlobalCoroutine.StartCoroutine(ae.BeginPack());
        }

        [MenuItem("Assets/PackTool/AudioClip")]
        public static void ExportAudioClip()
        {
            foreach (Object obj in Selection.objects)
            {
                if (obj is AudioClip)
                    AssetsExport.ExportSound(obj as AudioClip);
            }
        }

        [MenuItem("Assets/PackTool/Font")]
        public static void ExportFont()
        {
            AssetsExport.ExportFontLib(Selection.activeObject as Font);
        }

        [MenuItem("Assets/PackTool/Texture")]
        public static void TextureExport()
        {
            Export((AssetsExport mgr)=> 
            {
                foreach (Object o in Selection.objects)
                    if (o is Texture)
                    {
                        mgr.CollectTexture(o as Texture);
                    }
            });
        }

        static void Export(System.Action<AssetsExport> fun)
        {
            CodeCheckAtler.Release();
            AssetsExport mgr = new AssetsExport();
            fun(mgr);
            if (!mgr.isEmpty)
            {
                GlobalCoroutine.StartCoroutine(mgr.BeginPack());
            }
        }

        [MenuItem("Assets/PackTool/编译运行时脚本")]
        [MenuItem("PackTool/编译运行时脚本")]
        public static void TestRuntimeScript()
        {
            using (new SetSpritePackerMode(SpritePackerMode.Disabled))
            {
                string filepath = AssetsExport.Folder + "ScriptOnly";
                BuildPipeline.BuildStreamedSceneAssetBundle(new string[] { }, filepath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.BuildScriptsOnly);
                if (File.Exists(filepath))
                    File.Delete(filepath);
            }
        }


        [MenuItem("Assets/PackTool/Materail")]
        public static void MaterailExport()
        {
            CodeCheckAtler.Release();
            AssetsExport mgr = new AssetsExport();
            foreach (Object obj in Selection.objects)
            {
                if (obj as Material)
                    mgr.CollectMaterial(obj as Material);
            }

            if (!mgr.isEmpty)
            {
                GlobalCoroutine.StartCoroutine(mgr.BeginPack());
            }
        }

        [MenuItem("Assets/PackTool/Scene")]
        public static void PrefabScene()
        {
            //Selection.activeObject = LightmapSettings.lightProbes;
            //Debug.Log(AssetDatabase.GetAssetPath(LightmapSettings.lightProbes));

            //BuildPipeline.BuildAssetBundle(LightmapSettings.lightProbes,
            //    null, 
            //    ResourcesPath.streamingAssetsPath + "test.tb", 
            //    BuildAssetBundleOptions.ChunkBasedCompression,
            //    EditorUserBuildSettings.activeBuildTarget);

            AssetsExport.ExportScene(Selection.activeObject, true);
        }

        [MenuItem("Assets/PackTool/AtlasExport")]
        public static void AtlasExport()
        {
            CodeCheckAtler.Release();
            PackCollectList pcl = new PackCollectList();
            foreach (GameObject go in Selection.gameObjects)
            {
                ATLAS atlas = null;
                if ((atlas = go.GetComponent<ATLAS>()) != null)
                {
                    pcl.CollectAtlas(atlas);
                }
            }

            if (!pcl.isEmpty)
            {
                AssetsExport mgr = new AssetsExport(true);
                GlobalCoroutine.StartCoroutine(pcl.BeginCollect(mgr));
            }
        }

        [MenuItem("Assets/PackTool/Shader")]
        public static void ShaderExport()
        {
            CodeCheckAtler.Release();
            AssetsExport mgr = new AssetsExport();
            foreach (Object obj in Selection.objects)
            {
                if (obj is Shader)
                    mgr.CollectBuiltinResource(obj as Shader);
            }

            if (!mgr.isEmpty)
            {
                GlobalCoroutine.StartCoroutine(mgr.BeginPack());
            }
        }

        [MenuItem("PackTool/测试所有的AssetBundle")]
        [MenuItem("Assets/PackTool/测试所有的AssetBundle")]
        public static void TestAllAssetBundle()
        {
            GlobalCoroutine.StartCoroutine(TestAllAssetBundleItor());
        }

        static IEnumerator TestAllAssetBundleItor()
        {
            FileSystemScanner scanner = new FileSystemScanner("", "");
            int start = 0;
            if (Logger.IsHas)
            {
                Logger.Instance.Release();
                Logger.ReleaseInstance();
            }

            // 动画对应动画控制器
            Dictionary<string, HashSet<string>> animToControllers = new Dictionary<string, HashSet<string>>();

            List<ScanEventArgs> scenes = new List<ScanEventArgs>();
            Logger.CreateInstance();
            int lenght = ResourcesPath.LocalDataPath.Length;
            scanner.ProcessFile = 
                (object sender, ScanEventArgs e) =>
                {
                    string key = e.Name.Substring(lenght).Replace('\\', '/');
                    if (!Utility.isAB(key))
                        return;

                    if (key.EndsWith(".unity"))
                    {
                        scenes.Add(e);
                        return;
                    }

                    if (key.StartsWith("Art/UIData/UData/Atlas/"))
                        return;

                    AssetBundle assetBundle = AssetBundle.LoadFromFile(e.Name);
                    if (assetBundle == null)
                    {
                        Debug.LogError(string.Format("资源:{0}加载失败!", e.Name));
                        return;
                    }

                    if (key.EndsWith(".controller"))
                    {
                        foreach (string name in assetBundle.GetAllAssetNames())
                        {
                            if (name.EndsWith(".controller") || name.EndsWith(".cs") || name.EndsWith(".dll"))
                                continue;

                            HashSet<string> list = null;
                            if (!animToControllers.TryGetValue(name, out list))
                            {
                                list = new HashSet<string>();
                                animToControllers.Add(name, list);
                            }

                            list.Add(key);
                        }
                    }
                    else
                    {
                        AssetBundleLoad.Check(e.Name, assetBundle);
                    }
                    assetBundle.Unload(true);
                    start++;
                };

            scanner.Scan(ResourcesPath.LocalDataPath, true);

            //foreach (ScanEventArgs scene in scenes)
            //{
            //    AssetBundle assetBundle = AssetBundle.LoadFromFile(scene.Name);
            //    try
            //    {
            //        UnityEngine.SceneManagement.SceneManager.LoadScene(Path.GetFileNameWithoutExtension(scene.Name), UnityEngine.SceneManagement.LoadSceneMode.Single);
            //        yield return 0;

            //        string[] GetAllAssetNames = assetBundle.GetAllAssetNames();
            //        string[] GetAllScenePaths = assetBundle.GetAllScenePaths();
            //        bool isStreamedSceneAssetBundle = assetBundle.isStreamedSceneAssetBundle;

            //        Object mainAsset = assetBundle.mainAsset;

            //        Debug.LogFormat("{0} name:{1}", scene.Name, assetBundle.GetAllScenePaths().Length);
            //    }
            //    finally
            //    {
            //        assetBundle.Unload(true);
            //    }
            //}

            int animTotal = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var itor in animToControllers)
            {
                if (itor.Value.Count >= 2)
                {
                    ++animTotal;
                    sb.AppendLine(string.Format("index:{2}) anim:{0} controller:{1}", itor.Key, itor.Value.Count, animTotal));
                    foreach (string c in itor.Value)
                        sb.AppendLine(string.Format("   {0}", c));
                }
            }

            Debug.Log(string.Format("测试预置体总数:{0}! animToControllers:{1}/{2}", start, animTotal, animToControllers.Count));
            Debug.LogFormat("animController:\r\n{0}", sb.ToString());
            yield break;
        }

        public delegate void OnExportAllResourceEnd();

        [MenuItem("Assets/PackTool/复制动画")]
        static void CopyAnim()
        {
            foreach (Object o in Selection.objects)
            {
                if (o is AnimationClip)
                {
                    string path = AssetDatabase.GetAssetPath(o);
                    if (path.ToLower().EndsWith(".anim"))
                        continue;

                    path = path.Substring(0, path.LastIndexOf('/'));
                    Object copy = Object.Instantiate(o);
                    path = string.Format("{0}/{1}{2}", path, o.name, ".anim");
                    Debug.Log("copy:" + path);
                    AssetDatabase.CreateAsset(copy, path);
                }
            }
        }


        [MenuItem("Assets/PackTool/重新打Material")]
        static void PackMaterial()
        {
            CodeCheckAtler.Release();
            AssetsExport mgr = new AssetsExport();
            Utility.ForEach("Assets", 
                (AssetImporter ai) =>
                {
                    string filedst = AssetsExport.PackPath + ai.assetPath.Substring(7);
                    if (File.Exists(filedst))
                    {
                        //Debug.Log("mat:" + filedst);
                        Material m = AssetDatabase.LoadAssetAtPath<Material>(ai.assetPath);
                        if (m != null)
                        {
                            mgr.CollectMaterial(m);
                        }
                    }
                }, 
                (string path, string root) => { return path.EndsWith(".mat", true, null); });

            if (!mgr.isEmpty)
            {
                GlobalCoroutine.StartCoroutine(mgr.BeginPack());
            }
        }
    }
}
#endif