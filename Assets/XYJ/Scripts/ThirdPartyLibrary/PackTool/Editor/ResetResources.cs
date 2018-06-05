using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace PackTool
{
    public class ResetResources : TemplatesMarco
    {
        const string marco = "USE_RESOURCES";
        const string path = "";

        [MenuItem("PackTool/程序专属/Resources/取消", true)]
        public static bool BufCannelToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return macroDefine.has(marco);
        }

        [MenuItem("PackTool/程序专属/Resources/开启", true)]
        public static bool BufOpenToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return !macroDefine.has(marco);
        }

        [MenuItem("PackTool/程序专属/Resources/取消")]
        public static void BufCannel()
        {
            SetEnable(false);
        }

        [MenuItem("PackTool/程序专属/Resources/开启")]
        public static void BufOpen()
        {
            BuildPlatform.SetBuildSettingsScene();
            SetEnable(true);
        }

        [MenuItem("PackTool/程序专属/Resources/更新Atlas")]
        public static void UpdateAtlas()
        {
            SpritesLoad.CreateIconAtlas();
        }

        [MenuItem("PackTool/程序专属/Resources/更新")]
        public static void Update()
        {
            // svn更新
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = Application.dataPath + "/../svnupdate.bat";
                p.StartInfo.Arguments = "1234";
                p.Start();
                p.WaitForExit();
                p.Close();
            }

            FileIfSet(true);

            AssetDatabase.Refresh();
        }

        [MenuItem("PackTool/程序专属/Resources/更新", true)]
        public static bool BufUpdate()
        {
            return BufCannelToggle();
        }

        [MenuItem("PackTool/程序专属/Resources/打包")]
        public static void Build()
        {
            SceneVersion.SaveToFile();

            PlayerSettings.Android.bundleVersionCode = (int)SceneVersion.svn_version;

            XTools.Utility.CopyConfigToPath(ResourcesPath.streamingAssetsPath);

            string filepath = string.Format("{0}/../Game/Resources/{1}/{2}", ResourcesPath.dataPath, ResourcesPath.PlatformKey, XYJLogger.GetNowTime());
            ProjectSetting.Check();
            Application.OpenURL(BuildPlayer.Build(BuildSceneList.GetAllSceneList().ToArray(), filepath));
        }

        static string[][] Keys = new string[][]
        {
            new string[] { "/ResourcesExport/", "/Resources/", ".prefab" },
            new string[] {"/AudioClipExport/", "/Resources/", ""},
            new string[] {"/MaterialExport/", "/Resources/", ".mat"},
        };

        static bool GetCopyPath(string assetPath, out string newpath)
        {
            newpath = assetPath;
            for (int i = 0; i < Keys.Length; ++i)
            {
                if (newpath.Contains(Keys[i][0]))
                {
                    if (!string.IsNullOrEmpty(Keys[i][2]))
                    {
                        if (!newpath.EndsWith(Keys[i][2]))
                            continue;
                    }

                    newpath = newpath.Replace(Keys[i][0], Keys[i][1]);
                }
            }

            if (newpath == assetPath)
                return false;

            newpath = "Assets/__copy__/" + newpath.Substring(7);
            return true;
        }

        static string asset_path_contaion = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_PrefabParentObject: {{fileID: 0}}
  m_PrefabInternal: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: 68046c8282b8d5849a59689fc8879ab4, type: 3}}
  m_Name: {0}
  m_EditorClassIdentifier: 
  asset: {{fileID: {2}, guid: {1}, type: {3}}}
";

        static void GetFileID(string assetPath, out string fileid, out string type)
        {
            type = "3";
            fileid = "8300000";
            string suffix = assetPath.Substring(assetPath.LastIndexOf('.')+1);
            switch (suffix)
            {
            case "mat":
                fileid = "2100000";
                type = "2";
                break;
            case "prefab":
                {
                    string text = File.ReadAllText(assetPath);
                    string key = "m_RootGameObject: {fileID: ";
                    int pos = text.IndexOf(key) + key.Length;
                    int end = text.IndexOf("}", pos);

                    type = "2";
                    fileid = text.Substring(pos, end - pos);
                }
                break;
            }
        }

        static void Create(string srcPath, string dstPath)
        {
            string assetPath = dstPath;
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf('.'));
            assetPath += ".asset";
            string guid = AssetDatabase.AssetPathToGUID(srcPath);

            Directory.CreateDirectory(assetPath.Substring(0, assetPath.LastIndexOf('/')));

            //ar = new AssetResoruce();
            //ar.asset = AssetDatabase.LoadMainAssetAtPath(itor.first);
            //ar.guid = guid;
            //AssetDatabase.CreateAsset(ar, assetPath);

            string type;
            string fileid;
            GetFileID(srcPath, out fileid, out type);
            string result = string.Format(asset_path_contaion, Path.GetFileNameWithoutExtension(assetPath), guid, fileid, type);
            File.WriteAllText(assetPath, result);
        }

        static void FileIfSet(bool enable)
        {
            var tc = new XTools.TimeCheck(true);
            string root = Application.dataPath.Replace('\\', '/');
            int root_lenght = root.Length + 1;
            if (enable == true)
            {
                var guids = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { "Assets" }));
                HashSet<string> old_assets = new HashSet<string>();
                HashSet<string> new_assets = new HashSet<string>();
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (AssetDatabase.IsValidFolder(assetPath))
                        continue;

                    if (assetPath.StartsWith("Assets/__copy__/"))
                    {
                        old_assets.Add(assetPath);
                        continue;
                    }

                    string newpath;
                    if (GetCopyPath(assetPath, out newpath))
                    {
                        Create(assetPath, newpath);
                        new_assets.Add(newpath);
                    }
                }

                foreach (string asset in old_assets)
                {
                    if (!new_assets.Contains(asset))
                    {
                        AssetDatabase.DeleteAsset(asset);
                    }
                }

                SpritesLoad.CreateIconAtlas(false);
            }
            else
            {
                string copy_root = root + "/__copy__";
                XTools.Utility.DeleteFolder(copy_root);
            }

            Debug.LogFormat("FileIfSet({0}):{1}", enable, tc.renew);
        }

        static void SetEnable(bool enable)
        {
            FileIfSet(enable);
            SetEnable(enable, marco, path);

            if (enable)
            {
                ResetAllResources.BufCannel();
                MacroDefine macroDefine = new MacroDefine();
                macroDefine.Remove("USE_RESOURCESEXPORT");
                macroDefine.Remove("USER_ALLRESOURCES");
                macroDefine.Add("BEHAVIAC_RELEASE");
                macroDefine.Save();
            }
        }
    }
}