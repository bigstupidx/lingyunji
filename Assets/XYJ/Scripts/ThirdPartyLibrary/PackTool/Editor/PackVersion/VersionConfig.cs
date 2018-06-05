using System;
using System.Collections;
using System.Collections.Generic;

namespace PackTool.New
{
    public class VersionConfig
    {
        public VersionConfig()
        {

        }

        public ConfigEditorSet editor = new ConfigEditorSet(); // 编辑器设置

        public string client_channel = "netease"; // 客户端渠道
        public string server_channel = "netease"; // 用品过滤渠道
        public List<string> sdkpaths = new List<string>(); // SDK目录

        // 外部运营环境的patchlist下载地址
        public string outside_patchList_program;
        public string outside_patchList_patch;

//         public string outside_filename_program
//         {
//             get
//             {
//                 if (outside_patchList_program.Count == 0)
//                     return "outside_patchlist_program.txt";
// 
//                 return System.IO.Path.GetFileName(outside_patchList_program[0]);
//             }
//         }
// 
//         public string outside_filename_patch
//         {
//             get
//             {
//                 if (outside_patchList_patch.Count == 0)
//                     return "outside_patchlist_patch.txt";
// 
//                 return System.IO.Path.GetFileName(outside_patchList_patch[0]);
//             }
//         }

        // 内部测试环境的patchlist下载地址
        public string inside_patchList_program;
        public string inside_patchList_patch;

//         public string inside_filename_program
//         {
//             get
//             {
//                 if (inside_patchList_program.Count == 0)
//                     return "inside_patchlist_program.txt";
// 
//                 return System.IO.Path.GetFileName(inside_patchList_program[0]);
//             }
//         }
// 
//         public string inside_filename_patch
//         {
//             get
//             {
//                 if (inside_patchList_patch.Count == 0)
//                     return "inside_patchlist_patch.txt";
// 
//                 return System.IO.Path.GetFileName(inside_patchList_patch[0]);
//             }
//         }

        // 生成安装包的文件名
        public string install_programname;

        // 补丁包的文件名
        public string patchname = "";

        // 存储此更新patchlist的下载地址
        public string httpdown;

        // 上传patchlist的地址，用户名和密码
        public string ftpupdate = "";
        public string ftpusername = "";
        public string ftppassword = "";

        // 得到安装包文件名
        public string GetProgramName(Version verson, long svn)
        {
            return string.Format(install_programname, verson.ToString(), client_channel, svn);
        }

        public string GetPatchName(Version verson, long svn)
        {
            return string.Format(patchname, verson.ToString(), client_channel, svn);
        }
    }
}