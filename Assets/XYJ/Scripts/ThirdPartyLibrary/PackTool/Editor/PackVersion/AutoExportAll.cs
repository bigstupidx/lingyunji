#if USE_RESOURCESEXPORT
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
        public AutoExportAll(Versions config)
        {
            isDone = false;
            Config = config;
        }

        public bool isDone { get; protected set; }

        public Versions Config { get; protected set; }

        // 开始打包与生成补丁包
        public IEnumerator Start()
        {
            PackBuildResources resources = new PackBuildResources();

            // 资源包
            GlobalCoroutine.StartCoroutine(resources.Start(Config));
            while (!resources.isDone)
                yield return 0;

            foreach (VersionConfig vc in Config.configs)
            {
                // 检查下编辑器设置
                vc.editor.Check(Config, vc);

                PackBuildProgram program = new PackBuildProgram();
                program.BuildPack(Config, vc);

                // 包生成了，现在，就需要上传到FTP上，并生成patchlist文件
                // 需要上传3个东西
                // 1 patchlist
                // 2 补丁
                // 3 程序包
//                if (vc.httpsdown.Count != 0)
//                {
                    // 生成更新列表文件
//                     string[] patchlist = BuildPatchList(resources, program, Config, vc);
// 
//                     if (!string.IsNullOrEmpty(vc.ftpupdate))
//                     {
//                         List<Part<string, string>> upftpList = new List<Part<string, string>>();
//                         string patchpath = resources.patchname;
//                         string installpath = program.installpath;
// 
//                         if (File.Exists(patchlist[0]))
//                         {
//                             upftpList.Add(new Part<string, string>(vc.inside_filename_program, patchlist[0]));
//                             upftpList.Add(new Part<string, string>(vc.outside_filename_program, patchlist[0]));
//                         }
// 
//                         if (File.Exists(patchlist[1]))
//                         {
//                             upftpList.Add(new Part<string, string>(vc.inside_filename_patch, patchlist[1]));
//                             upftpList.Add(new Part<string, string>(vc.outside_filename_patch, patchlist[1]));
//                         }
// 
//                         if (File.Exists(patchpath))
//                             upftpList.Add(new Part<string, string>(ResourcesPath.PlatformKey + "/" + vc.GetPatchName(resources.current, Config.svn1), patchpath));
// 
//                         if (File.Exists(installpath))
//                             upftpList.Add(new Part<string, string>(ResourcesPath.PlatformKey + "/" + vc.GetProgramName(Config.startVersion, Config.svn1) + Util.programsuffix, installpath));
// 
//                         Util.UpdateToFtpServer(upftpList, vc.ftpupdate, vc.ftpusername, vc.ftppassword);
//                    }
//                }
            }

            isDone = true;
        }
    }
}
#endif