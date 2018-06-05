using System;
using CommonBase;
using System.Collections.Generic;

#pragma warning disable 414

namespace PackTool.New
{
    public class VersionConfigHandler : XMLHandlerReg
    {
        Dictionary<string, VersionConfig> VersoinConfigList = new Dictionary<string, VersionConfig>();

        VersionConfig CurrentConfig;

        List<string> android_name; // 安卓平台配置名
        List<string> ios_name;     // ios平台配置名

        Version startVersion = null; // 当前要打包的版本号
        Version resVersion = null;  // 资源版本，只用在打单独的程序包时

        bool isSimplify = true; // 是否精简打包

        long svn_offset = 0;

        string outside_patchList_program ;
        string outside_patchList_patch;

        // 内部测试环境的patchlist下载地址
        public string inside_patchList_program;
        public string inside_patchList_patch;

        // 当前要打包的配置
        public Versions Current
        {
            get 
            {
                Versions versions = new Versions(svn_offset);
                versions.startVersion = startVersion;
                versions.resVersion = resVersion;
                List<string> confignames = null;
                confignames = android_name;

#if UNITY_IPHONE
                confignames = ios_name;
#endif
                versions.isSimplify = isSimplify;
                foreach (string name in confignames)
                {
                    if (!string.IsNullOrEmpty(name))
                        versions.configs.Add(VersoinConfigList[name]);
                }
                return versions;
            }
        }

        static void EmptyCopy(List<string> empty, List<string> copy)
        {
            if (empty.Count == 0)
            {
                empty.AddRange(copy);
            }
        }

        public VersionConfigHandler()
        {
            RegElementStart("Configuration", elementConfiguration);
            RegElementStart("Channel", elementChannel);
            RegElementEnd("Channel", 
                (string element) => 
                {
                    CurrentConfig.inside_patchList_program = inside_patchList_program;
                    CurrentConfig.inside_patchList_patch = inside_patchList_patch;

                    CurrentConfig.outside_patchList_program = outside_patchList_program;
                    CurrentConfig.outside_patchList_patch = outside_patchList_patch;
                });

            RegElementStart("PlayerSettings", elementPlayerSettings);
            RegElementStart("GameIcon", elementGameIcon);
            RegElementStart("InstallConfig", elementInstallConfig);
            //RegElementStart("OutsidePathList", elementOutsidePathList);
            //RegElementStart("OutsideProgramList", elementOutsideProgramList);
//             RegElementStart("InsidePathList", elementInsidePathList);
//             RegElementStart("InsideProgramList", elementInsideProgramList);
            RegElementStart("ProgramName", elementProgramName);
            RegElementStart("PatchPath", elementPatchPath);
            //RegElementStart("PatchListDown", elementPatchListDown);
            RegElementStart("FtpUpdate", elementFtpUpdate);
            RegElementStart("SvnOffset",
                (string element, XMLAttributes attributes) =>
                {
                    svn_offset = attributes.getValueAsInteger("value", 0);
                });

            RegElementStart("DefaultOutsideProgramList", 
                (string element, XMLAttributes attributes) => 
                {
                    outside_patchList_program = attributes.getValue("value");
                });

            RegElementStart("DefaultOutsidePathList", 
                (string element, XMLAttributes attributes) => 
                {
                    outside_patchList_patch = attributes.getValue("value");
                });

            RegElementStart("DefaultInsideProgramList", 
                (string element, XMLAttributes attributes) =>
                {
                    inside_patchList_program = attributes.getValue("value");
                });

            RegElementStart("DefaultInsidePathList",
                (string element, XMLAttributes attributes) =>
                {
                    inside_patchList_patch = attributes.getValue("value");
                });
        }

        void elementConfiguration(string element, XMLAttributes attributes)
        {
            android_name = new List<string>(attributes.getValueAsString("Android").Split(','));
            ios_name = new List<string>(attributes.getValueAsString("ios").Split(','));
        }

        void elementChannel(string element, XMLAttributes attributes)
        {
            CurrentConfig = new VersionConfig();
            CurrentConfig.server_channel = attributes.getValueAsString("server_channel");
            CurrentConfig.client_channel = attributes.getValueAsString("client_channel");
            CurrentConfig.sdkpaths.AddRange(attributes.getValueAsString("sdk").Split('|'));
            VersoinConfigList.Add(CurrentConfig.client_channel, CurrentConfig);
        }

        void elementPlayerSettings(string element, XMLAttributes attributes)
        {
            CurrentConfig.editor.companyName = attributes.getValue("companyName");
            CurrentConfig.editor.productName = attributes.getValue("productName");
            CurrentConfig.editor.bundleIdentifier = attributes.getValue("bundleIdentifier");
        }

        void elementGameIcon(string element, XMLAttributes attributes)
        {
            CurrentConfig.editor.icons.Add(attributes.getValue("resPath"));
        }

        void elementInstallConfig(string element, XMLAttributes attributes)
        {
            startVersion = new Version(attributes.getValue("startVersion"));

            if (!string.IsNullOrEmpty(attributes.getValue("resVersion")))
                resVersion = new Version(attributes.getValue("resVersion"));

            isSimplify = attributes.getValueAsBool("isSimplify", isSimplify);
        }

//         void elementOutsidePathList(string element, XMLAttributes attributes)
//         {
//             CurrentConfig.outside_patchList_patch.Add(attributes.getValue("value"));
//         }
// 
//         void elementOutsideProgramList(string element, XMLAttributes attributes)
//         {
//             CurrentConfig.outside_patchList_program.Add(attributes.getValue("value"));
//         }

//         void elementInsidePathList(string element, XMLAttributes attributes)
//         {
//             CurrentConfig.inside_patchList_patch.Add(attributes.getValue("value"));
//         }
// 
//         void elementInsideProgramList(string element, XMLAttributes attributes)
//         {
//             CurrentConfig.inside_patchList_program.Add(attributes.getValue("value"));
//         }
     
        void elementProgramName(string element, XMLAttributes attributes)
        {
            CurrentConfig.install_programname = attributes.getValue("name");

            if (CurrentConfig.install_programname.Contains(":"))
                return; // 

            // 带没带sdk区别对待
            string suffix = "";

#if USE_HOT
            suffix += "hot_";
#endif

#if USE_NESDK
            suffix += "nesdk_";
#elif USE_T1SDK
            suffix += "t1sdk_";
#else
            suffix += "nosdk_";
#endif

#if COPYRIGHT_EDITION
            suffix += "out_";
#else
            suffix += "in_";
#endif

#if COM_DEBUG
            suffix += "gm";
            CurrentConfig.install_programname += "-gm";
#else
            suffix += "nogm";
#endif
            CurrentConfig.install_programname = string.Format("{0}/{1}", suffix, CurrentConfig.install_programname);
        }

        void elementPatchPath(string element, XMLAttributes attributes)
        {
            CurrentConfig.patchname = attributes.getValue("name");
        }

//         void elementPatchListDown(string element, XMLAttributes attributes)
//         {
//             CurrentConfig.httpsdown.Add(attributes.getValue("url"));
//         }

        void elementFtpUpdate(string element, XMLAttributes attributes)
        {
            CurrentConfig.ftpupdate = attributes.getValue("url");
            CurrentConfig.ftpusername = attributes.getValue("username");
            CurrentConfig.ftppassword = attributes.getValue("password");
        }
    }
}
