using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace PackTool.New
{
    public partial class AutoExportAll
    {
        [MenuItem("PackTool/VersionUpdate/宏/测试/内部")]
        public static void TestMacro()
        {
            MacroDefine macroDefine = new MacroDefine();
            if (macroDefine.has("USE_RESOURCES"))
                ResetResources.BufCannel();
            macroDefine.Remove("USE_ABL");
            macroDefine.Remove("USE_RESOURCES");
            macroDefine.Remove("USER_ALLRESOURCES");
            macroDefine.Add("USE_RESOURCESEXPORT");
            macroDefine.Add("NO_JSON");
            macroDefine.Add("BEHAVIAC_RELEASE");
            macroDefine.Save();
        }

#if USE_RESOURCESEXPORT
        [MenuItem("PackTool/VersionUpdate/BuildTest")]
        public static void BuildTest()
        {
            GenTest();
        }

        static public AutoExportAll GenTest()
        {
            Logger.CreateInstance();
            Versions config = LoadConfig();
            AutoExportAll aea = new AutoExportAll(config);
            XTools.GlobalCoroutine.StartCoroutine(aea.StartTest());

            return aea;
        }

        public static void BuildAutoExportTest()
        {
            XTools.GlobalCoroutine.StartCoroutine(ExportTest());
        }

        static IEnumerator ExportTest()
        {
            yield return 0;
            AutoExportAll aea = GenTest();
            while (!aea.isDone)
                yield return 0;

            EditorApplication.Exit(0);
        }

        // 开始打包与生成补丁包
        public IEnumerator StartTest()
        {
            // 再开始打包
            int index = 1;
            while (true)
            {
                ExportAllResources r = new ExportAllResources(Config.isSimplify, true);
                r.Export();
                while (!r.isDone)
                    yield return 0;

                if (!r.isError)
                    break;

                ++index;
                Config.isSimplify = true;
                Logger.LogError("资源打包出错，再来一次！" + index);
            }

            ResourceConfig resconfig = new ResourceConfig();
            resconfig.version = (Config.resVersion == null ? Config.startVersion : Config.resVersion);
            resconfig.svn = Config.svn;
            resconfig.SaveToFile();

            // 把资源打成zip包
            Archive.AssetPackArchive(ResourcesPath.LocalDataPath, ResourcesPath.streamingAssetsPath + XTools.AssetZip.DefaultName, "");

            try
            {
                string filepath = ResourcesPath.LocalBasePath + XTools.AssetZip.DefaultName;
                if (File.Exists(filepath))
                    File.Delete(filepath);

                File.Copy(ResourcesPath.streamingAssetsPath + XTools.AssetZip.DefaultName, filepath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            foreach (VersionConfig vc in Config.configs)
            {
                // 检查下编辑器设置
                vc.editor.Check(Config, vc);

                PackBuildProgram program = new PackBuildProgram();
                program.BuildPack(Config, vc);
            }

            isDone = true;
        }
#endif      
    }
}
