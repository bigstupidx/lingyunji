using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    // 程序的配置信息
    public class ProgramConfig
    {
        static ProgramConfig instance = null;

        public const string filename = "program_config";

        public static ProgramConfig Default
        {
            get
            {
                if (instance == null)
                {
                    Stream stream = StreamingAssetLoad.GetFile(filename);
                    if (stream == null)
                    {
                        instance = new ProgramConfig();
                        return instance;
                    }

                    instance = new ProgramConfig();
                    StreamReader reader = new StreamReader(stream);
                    instance.Load(reader);
                    stream.Close();
                    reader.Close();
                }

                return instance;
            }
        }

        public ProgramConfig()
        {

        }

        // 程序的版本号
        public Version version = new Version(0, 0, 0);

        public string client_channel = "netease"; // 客户端类型渠道
        public string server_channel = "netease"; // 服务器过滤

        public string bundleIdentifier = ""; // 包id

        // 外部运营环境的patchlist下载地址,分程序文件与补丁文件
        public string outside_patchList_program;
        public string outside_patchList_patch;

        // 内部测试环境的patchlist下载地址,分程序文件与补丁文件
        public string inside_patchList_program;
        public string inside_patchList_patch;

        public bool isCheckUpdate = false; // 是否检测更新

//         public string toString()
//         {
//             return string.Format(
//                 "bundleIdentifier:{0} version:{1} svn1:{2} svn2:{3} ol:{4} il:{5} iscu:{6}", 
//                 bundleIdentifier,
//                 version, 
//                 svn1, 
//                 svn2,
//                 Util.UnionURL(outside_patchList_program) + "," + Util.UnionURL(outside_patchList_patch),
//                 Util.UnionURL(inside_patchList_program) + "," + Util.UnionURL(inside_patchList_patch),
//                 isCheckUpdate);
//         }


        // svn版本号
        public long svn
        {
            get;

#if UNITY_EDITOR
            set;
#else
            protected set;
#endif
        }

#if UNITY_EDITOR
        // 保存到文件
        public void SaveToFile()
        {
            string file = ResourcesPath.streamingAssetsPath + "/" + filename;
            if (File.Exists(file))
                File.Delete(file);
            else
                Directory.CreateDirectory(ResourcesPath.streamingAssetsPath);

            StreamWriter writer = new StreamWriter(file, false, System.Text.Encoding.UTF8);
            writer.WriteLine(bundleIdentifier);
            writer.WriteLine(version.ToString());
            writer.WriteLine(client_channel); // 客户端类型渠道
            writer.WriteLine(server_channel); // 服务器过滤渠道

            writer.WriteLine(outside_patchList_program);
            writer.WriteLine(outside_patchList_patch);
            writer.WriteLine(inside_patchList_program);
            writer.WriteLine(inside_patchList_patch);

            writer.WriteLine(svn);
            writer.WriteLine(isCheckUpdate);
            writer.Close();
        }
#endif
        public void Load(StreamReader reader)
        {
            bundleIdentifier = reader.ReadLine();
            version = new Version(reader.ReadLine());
            client_channel = reader.ReadLine(); // 客户端类型渠道
            server_channel = reader.ReadLine(); // 服务器过滤渠道

            outside_patchList_program = reader.ReadLine();
            outside_patchList_patch = reader.ReadLine();
            inside_patchList_program = reader.ReadLine();
            inside_patchList_patch = reader.ReadLine();

            string line = reader.ReadLine();
            svn = long.Parse(line);
            isCheckUpdate = bool.Parse(reader.ReadLine());
        }
    }
}