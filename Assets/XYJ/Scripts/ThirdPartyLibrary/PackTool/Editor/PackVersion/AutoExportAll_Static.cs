using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using XTools;

namespace PackTool.New
{
    public partial class AutoExportAll
    {
        [MenuItem("PackTool/VersionUpdate/宏/正式")]
        public static void Macro()
        {
            MacroDefine macroDefine = new MacroDefine();
            if (macroDefine.has("USE_RESOURCES"))
                ResetResources.BufCannel();
            macroDefine.Remove("USE_ABL");

            macroDefine.Remove("USER_ALLRESOURCES");
            macroDefine.Add("USE_RESOURCESEXPORT");
            macroDefine.Add("NO_JSON");
            macroDefine.Add("BEHAVIAC_RELEASE");
            macroDefine.Save();
        }

        [MenuItem("PackTool/VersionUpdate/宏/编辑器")]
        public static void MacroEditor()
        {
            MacroDefine macroDefine = new MacroDefine();
            macroDefine.RemoveAll();
            macroDefine.Add("COM_DEBUG");
            macroDefine.Save();
        }

#if USE_RESOURCESEXPORT || USE_ABL
        [MenuItem("PackTool/VersionUpdate/清除数据/所有")]
        public static void ClearAllTemp()
        {
            XTools.Utility.DeleteFolder(Application.dataPath + "/__copy__");
            XTools.Utility.DeleteFolder(AssetsExport.PackPath);
            XTools.Utility.DeleteFolder(AssetsExport.TempPath);

            AssetDatabase.Refresh();
        }

        [MenuItem("PackTool/VersionUpdate/清除数据/当前")]
        public static void ClearCurrentTemp()
        {
            XTools.Utility.DeleteFolder(Application.dataPath + "/__copy__");
            XTools.Utility.DeleteFolder(AssetsExport.PackPath);

            AssetDatabase.Refresh();
        }
#endif

#if USE_RESOURCESEXPORT
        // 自动更新方式打包
        [MenuItem("PackTool/VersionUpdate/Build")]
        public static void Build()
        {
            AutoExportAll.Gen();
        }


        [MenuItem("PackTool/资源更新/生成资源版本文件")]
        public static void GenResourcesConfig()
        {
            Versions config = LoadConfig();
            ResourceConfig resconfig = new ResourceConfig();

            resconfig.version = (config.resVersion == null ? config.startVersion : config.resVersion);
            resconfig.svn = config.svn;
            resconfig.SaveToFile();

            Archive.AssetPackArchive(ResourcesPath.LocalDataPath, ResourcesPath.streamingAssetsPath + AssetZip.DefaultName, "");
        }

        static void CheckResourcesVersion(Versions versions)
        {
            if (versions.resVersion == null)
                return;

            // 检测下资源包
            string filepath = ResourcesPath.streamingAssetsPath + AssetZip.DefaultName;
            if (!File.Exists(filepath))
                return;

            ResourceConfig rc = new ResourceConfig();
            rc.version = versions.resVersion;
            rc.svn = versions.svn;
            Debug.Log("version:" + rc.version);
            Debug.Log("svn:" + rc.svn);

            MemoryStream stream = new MemoryStream();
            {
                rc.SaveToStream(new StreamWriter(stream));
            }
            Debug.Log("stream.Position:" + stream.Position);
            stream.Position = 0;

            Archive archive = new Archive(filepath, 0);
            Dictionary<string, Stream> items = new Dictionary<string, Stream>();
            archive.ReadInfo((Archive.FileItem afi) => { items.Add(afi.name, afi.GetStream(archive)); });

            items[ResourceConfig.filename] = stream;

            Archive.Generate(filepath + ".tmp", items, new byte[1024 * 1024]);
            archive.Close();
            stream.Close();

            File.Delete(filepath);
            File.Move(filepath + ".tmp", filepath);
        }

        // 只打程序包,在有资源的情况下
        [MenuItem("PackTool/VersionUpdate/BuildProgram")]
        public static void BuildProgram()
        {
            Versions versions = LoadConfig();
            CheckResourcesVersion(versions);

            foreach (VersionConfig config in versions.configs)
            {
                // 检查下编辑器设置
                config.editor.Check(versions, config);

                PackBuildProgram program = new PackBuildProgram();
                string filepath = program.BuildPack(versions, config);
                Application.OpenURL(filepath);
            }
        }

        // 只打程序包,
        [MenuItem("PackTool/VersionUpdate/取配置打包")]
        public static void UpdateConfigAndBuildProgram()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("set svn_home=C:/Program Files/TortoiseSVN/bin");
            builder.AppendLine("set UnityProject=%~dp0");
            builder.AppendLine("\"%svn_home%\"\\TortoiseProc.exe/command:update /path:%UnityProject%/Assets/Config /notempfile /closeonend:2");

            string filepath = string.Format("{0}/../{1}.bat", Application.dataPath, Guid.NewGuid().ToString());
            if (File.Exists(filepath))
                File.Delete(filepath);
            System.IO.File.WriteAllText(filepath, builder.ToString());

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = filepath;
            p.StartInfo.Arguments = "1234";
            p.Start();
            p.WaitForExit();
            p.Close();

            System.IO.File.Delete(filepath);

            // 生成打包目录下的md5文件列表
            AssetsExport.CopyConfigFile();
            AssetsExport.UpdatePack(() => { BuildProgram(); });
        }

        // 开始生成
        static public AutoExportAll Gen()
        {
            Versions config = LoadConfig();
            AutoExportAll aea = new AutoExportAll(config);
            GlobalCoroutine.StartCoroutine(aea.Start());

            return aea;
        }
#endif
    }
}
