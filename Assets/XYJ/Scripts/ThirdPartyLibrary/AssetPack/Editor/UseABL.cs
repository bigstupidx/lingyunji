using XTools;
using PackTool;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace ABL
{
    public class UseABL : TemplatesMarco
    {
        const string marco = "USE_ABL";
        const string path = "";

        [MenuItem("PackTool/程序专属/ABL/取消", true)]
        public static bool BufCannelToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return macroDefine.has(marco);
        }

        [MenuItem("PackTool/程序专属/ABL/开启", true)]
        public static bool BufOpenToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return !macroDefine.has(marco);
        }

        [MenuItem("PackTool/程序专属/ABL/取消")]
        public static void BufCannel()
        {
            SetEnable(false);
        }

        [MenuItem("PackTool/程序专属/ABL/开启")]
        public static void BufOpen()
        {
            BuildPlatform.SetBuildSettingsScene();
            SetEnable(true);
        }

#if USE_ABL
        public static System.Action __update_ui_atlas__ = null;

        [MenuItem("PackTool/程序专属/ABL/Export")]
        static void Export()
        {
            XTools.TimeCheck tc = new XTools.TimeCheck(true);
            BuildAllResources br = new BuildAllResources();
            XTools.GlobalCoroutine.StartCoroutine(br.SetAssetBundles(()=> 
            {
                UnityEngine.Debug.LogFormat("Export:{0}", tc.delay);
            }));
        }

        [MenuItem("PackTool/程序专属/ABL/BuildPlayer")]
        static void BuildPlayer()
        {
            BuildAllResources.BuildPack(PackTool.New.AutoExportAll.LoadConfig());
        }

        [MenuItem("PackTool/程序专属/ABL/BuildAssetBundles")]
        static void BuildAssetBundles()
        {
            BuildAllResources.BuildAssetBundles(PackTool.New.AutoExportAll.LoadConfig(), BuildAllResources.PackPath + "AB");
        }

        [MenuItem("PackTool/程序专属/ABL/重新生成资源包")]
        static void GenResourceConfig()
        {
            BuildAllResources.GenResourceConfig(PackTool.New.AutoExportAll.LoadConfig());
            AssetsExport.UpdatePack();
        }

        [MenuItem("Assets/ABL/BuildSelect")]
        static void BuildSelect()
        {
            UnityEditor.Sprites.Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget);

            List<AssetBundleBuild> abbs = new List<AssetBundleBuild>();
            XTools.Utility.ForEachSelect((AssetImporter ai) =>
            {
                if (!string.IsNullOrEmpty(ai.assetBundleName))
                {
                    if (abbs.FindIndex((AssetBundleBuild abb)=> { return abb.assetBundleName == ai.assetBundleName; }) == -1)
                    {
                        abbs.Add(new AssetBundleBuild()
                        {
                            assetBundleName = ai.assetBundleName,
                            assetBundleVariant = ai.assetBundleVariant,
                            assetNames = new string[1] { ai.assetPath.Substring(7).ToLower() }
                        });
                    }
                }
            });

            if (abbs.Count != 0)
            {
                string path = AssetsExport.PackPath + "AB";
                System.IO.Directory.CreateDirectory(path);
                var abm = BuildPipeline.BuildAssetBundles(path, abbs.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
                UnityEngine.Debug.LogFormat("abm:{0}", abm.name);
            }
        }

        [MenuItem("PackTool/程序专属/ABL/ExportAndBuild")]
        static void BuildAll()
        {
            Logger.CreateInstance();
            BuildAllResources br = new BuildAllResources();

            Logger.LogDebug("isBatchmode:{0}", isBatchmode);
            GlobalCoroutine.StartCoroutine(br.BuildAll(() =>
            {
                if (isBatchmode)
                {
                    Logger.LogDebug("EditorApplication.Exit");
                    EditorApplication.Exit(0);
                }
            }));
        }

        [MenuItem("Assets/ABL/SetAssetBundleName")]
        static void SetAssetBundleName()
        {
            Logger.CreateInstance();
            Utility.ForEachSelect((AssetImporter ai) => { }, 
                (assetPath, root) => 
                {
                    if (!BuildAllResources.IsExportResources(assetPath))
                        return false;

                    BuildAllResources.SetAssetBundle(assetPath);
                    foreach (var dep in AssetDatabase.GetDependencies(assetPath))
                    {
                        if (!BuildAllResources.IsExportResources(dep))
                            continue;

                        BuildAllResources.SetAssetBundle(dep);
                    }

                    return false;
                });
        }

        static bool isBatchmode
        {
            get
            {
                var appType = typeof(UnityEngine.Application);
                bool value = (bool)appType.GetProperty("isBatchmode", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, new object[] { });
                return value;
            }
        }
#endif
        //[MenuItem("Assets/ABL/CheckTexture")]
        //static void CheckTexture()
        //{
        //    Logger.CreateInstance();
        //    XTools.GlobalCoroutine.StartCoroutine(XTools.Utility.ForEachSelectASync((TextureImporter ai) =>
        //    {
        //        UnityEngine.Debug.LogFormat("path:{0}", ai.assetPath);
        //        AssetDatabase.ImportAsset(ai.assetPath);
        //    }));
        //}

        static void SetEnable(bool enable)
        {
            SetEnable(enable, marco, path);

            if (enable)
            {
                ResetAllResources.BufCannel();
                MacroDefine macroDefine = new MacroDefine();
                macroDefine.Remove("USE_RESOURCESEXPORT");
                macroDefine.Remove("USER_ALLRESOURCES");

                if (macroDefine.has("USE_RESOURCES"))
                    ResetResources.BufCannel();

                macroDefine.Add("BEHAVIAC_RELEASE");
                macroDefine.Save();
            }
        }
    }
}