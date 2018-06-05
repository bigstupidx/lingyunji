using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool.New
{
    // 打程序包
    public class PackBuildProgram
    {
        // 当前程序的版本号，注意，只记录到x.x
        public Version current { get; protected set; }

        // 生成安装包的路径
        public string installpath { get; protected set; }

        // 生成程序包
        public string BuildPack(Versions versions, VersionConfig config)
        {
            System.Version version = versions.startVersion;
            PlayerSettings.bundleVersion = (new Version(version.Major, version.Minor, version.Build)).ToString();
            Debug.Log("打包程序版本号:" + PlayerSettings.bundleVersion);

            current = new Version(PlayerSettings.bundleVersion);

            // 程序包配置数据
            {
                ProgramConfig pc = new ProgramConfig();
                pc.bundleIdentifier = PlayerSettings.applicationIdentifier; // bundleIdentifier
                pc.version = new Version(PlayerSettings.bundleVersion); // 程序版本号
                pc.client_channel = config.client_channel;
                pc.server_channel = config.server_channel;
                pc.outside_patchList_program = config.outside_patchList_program; // 正式运营服的patchlist地址
                pc.outside_patchList_patch = config.outside_patchList_patch; // 正式运营服的patchlist地址
                pc.inside_patchList_program = config.inside_patchList_program; // 测试服的patchlist地址
                pc.inside_patchList_patch = config.inside_patchList_patch; // 测试服的patchlist地址
                pc.svn = versions.svn;

#if NetEase_Version
                pc.isCheckUpdate = true;
#else
                pc.isCheckUpdate = false;
#endif
                pc.SaveToFile();
            }

            // 开始打程序包
            //BuildTarget target = AssetsExport.GetBuildTarget();
            List<string> scenes = new List<string>();
            scenes.Add("Assets/Art/UIData/Levels/StartInit.unity");
            scenes.Add("Assets/Art/UIData/Levels/Empty.unity");

            Directory.CreateDirectory(ResourcesPath.LocalBasePath);

            installpath = config.GetProgramName(versions.startVersion, versions.svn);
            if (!installpath.Contains(":"))
                installpath = ResourcesPath.LocalBasePath + installpath;
            installpath = BuildPlayer.Build(scenes.ToArray(), installpath);

            Debug.Log("程序包打包成功!" + installpath);
            return installpath;
        }

        static string FileBackupName(string filename)
        {
            string name = filename;
            string f = "";
            int index = name.LastIndexOf('.');
            if (index != 0)
            {
                name = name.Substring(0, index);
                f = filename.Substring(index + 1);
            }

            index = 0;
            while (true)
            {
                filename = string.Format("{0}_{1}backup.{2}", name, (index+1), f);
                if (!File.Exists(filename))
                    break;

                ++index;
            }

            return filename;
        }
    }
}
