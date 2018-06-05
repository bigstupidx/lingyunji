using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace PackTool
{
    public class ResetAllResources : TemplatesMarco
    {
        const string marco = "USER_ALLRESOURCES";
        const string path = "";

        [MenuItem("PackTool/打包/所有资源/取消", true)]
        public static bool BufCannelToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return macroDefine.has(marco);
        }

        [MenuItem("PackTool/打包/所有资源/开启", true)]
        public static bool BufOpenToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return !macroDefine.has(marco);
        }

        [MenuItem("PackTool/打包/所有资源/取消")]
        public static void BufCannel()
        {
            SetEnable(false, marco, path);
        }

        [MenuItem("PackTool/打包/所有资源/开启")]
        public static void BufOpen()
        {
            SetEnable(true, marco, path);
            ResetResources.BufCannel();
        }

        public static string[] GetPara()
        {
            string[] para = Environment.GetCommandLineArgs();
            LogPrint("-Environment", para);
            int args_start = System.Array.FindLastIndex<string>(para, (string f) => { return f == "-args" ? true : false; });

            if (args_start == -1)
                return new string[0];

            List<string> args = new List<string>();
            for (int i = args_start + 1; i < para.Length; ++i)
            {
                if (para[i].StartsWith("-"))
                    break;

                args.Add(para[i]);
            }

            return args.ToArray();
        }

        static void LogPrint(string key, string[] ps)
        {
            XYJLogger.LogDebug("{0} lenght:{1}", key, ps.Length);
            for (int i = 0; i < ps.Length; ++i)
                XYJLogger.LogDebug(ps[i]);
        }

        public static void BuildByCommonline()
        {
            XYJLogger.CreateInstance();

            string[] ps = GetPara();
            LogPrint("-Args", ps);
            
            BuildFile(ps.Length == 0 ? null : ps[0], true);
        }

        public static void BuildFile(string file, bool iscommonline)
        {
            if (string.IsNullOrEmpty(file))
                file = XYJLogger.GetNowTime();

            XYJLogger.LogDebug("BuildFile:{0}", file);
            SceneVersion.SaveToFile();
#if USER_ALLRESOURCES
            UpdateAllResource();
#endif
            string filename = "";
            if (file.Contains(":"))
            {
                // 绝对路径
                filename = file;
            }
            else
            {
                filename = string.Format("{0}Game/AllResources/{1}/{2}", ResourcesPath.LocalBasePath, ResourcesPath.PlatformKey, file);
            }

            ProjectSetting.Check();

            filename = BuildPlayer.Build(BuildSceneList.GetAllSceneList().ToArray(), filename);
            if (!iscommonline)
                Application.OpenURL(filename);
            else
            {
                EditorApplication.Exit(0);
            }
        }

        [MenuItem("PackTool/打包/所有资源/打包")]
        public static void Build()
        {
            BuildFile(XYJLogger.GetNowTime(), false);
        }

#if USER_ALLRESOURCES
        [MenuItem("PackTool/打包/所有资源/更新资源")]
        public static void UpdateResources()
        {
            UpdateAllResource();
        }

        public static void UpdateAllResource()
        {
            BuildPlatform.SetBuildSettingsScene();
            string path = "Assets/Resources/AllResources.prefab";
            GameObject go = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            if (go == null)
            {
                GameObject cgo = new GameObject("AllResources");
                System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/");
                go = UnityEditor.PrefabUtility.CreatePrefab(path, cgo);
                GameObject.DestroyImmediate(cgo);
            }

            PackTool.AllResources all = go.GetComponent<PackTool.AllResources>();
            if (all == null)
                all = go.AddComponent<PackTool.AllResources>();

            all.UpdateAllResource();

            XTools.Utility.CopyConfigToPath(ResourcesPath.streamingAssetsPath);
        }
#endif
    }
}