#if USE_RESOURCESEXPORT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XTools;

namespace PackTool.Patch
{
    public partial class PatchGenerate
    {
        // 得到某个目录下的所有文件的md5值
        static Dictionary<string, string> GetDirMd5List(string path)
        {
            if (path[path.Length - 1] != '/')
                path = path + "/";

            Dictionary<string, string> md5list = new Dictionary<string, string>();
            string filePath = path;
            string[] allfiles = Directory.GetFiles(filePath, "*", SearchOption.AllDirectories);
            //int total = allfiles.Length;

            string md5, newfile;
            int current = 0;
            foreach (string file in allfiles)
            {
                current++;
                if (file.EndsWith(".meta"))
                    continue;

                md5 = Md5.GetFileMd5(file);
                newfile = file.Substring(filePath.Length).Replace('\\', '/');
                md5list.Add(newfile, md5);
            }

            return md5list;
        }

        static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
                output.Write(buffer, 0, len);
        }

        // islzma是否压缩
        //static FileStream GetFileStream(string filename, bool islzma)
        //{
        //    if (islzma)
        //    {
        //        // 在压缩
        //        lzma.FileEncode(filename, filename + "___lzma___");
        //        filename += "___lzma___";
        //    }

        //    return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        //}

        public delegate bool CheckFile(List<string> alterList);

        public struct Data
        {
            public string from; // 源目录
            public string to; // 目标目录
            public string patchprefix; // 打包出来的补丁路径前缀
            public string patchname; // 生成的补丁名
            public string password; // 补丁文件的密码
        }

        static public bool Generate(Data config, CheckFile cf)
        {
            string prefix = string.IsNullOrEmpty(config.patchprefix) ? "" : config.patchprefix + "/";

            UnityEngine.Debug.Log(string.Format("from:{0} to:{1}", config.from, config.to));
            Dictionary<string, string> from_md5list = GetDirMd5List(config.from);
            Dictionary<string, string> to_md5list = GetDirMd5List(config.to);

            StringBuilder deleteStr = new StringBuilder(1000); // 记录下要删除的文件
            List<string> atlerList = new List<string>(); // 改变了的文件

            foreach (KeyValuePair<string, string> itor in from_md5list)
            {
                if (!to_md5list.ContainsKey(itor.Key))
                    deleteStr.AppendLine(prefix + itor.Key);
            }

            // 查找不一至的或原先版本不存在的文件
            foreach (KeyValuePair<string, string> itor in to_md5list)
            {
                string md5 = null;
                if (from_md5list.TryGetValue(itor.Key, out md5))
                {
                    // 当前目录存在此文件并且，文件内容一至,则不需要更新
                    if (md5 == itor.Value)
                        continue;
                }

                atlerList.Add(itor.Key);
            }

            if (atlerList.Count == 0 && deleteStr.Length == 0)
            {
                UnityEngine.Debug.Log(string.Format("目录内容一至，不需要补丁包!from:{0} to:{1}", config.from, config.to));
                return false;
            }

            if (cf != null)
            {
                if (cf(atlerList))
                    return false;
            }

            // 开始生成zip文件
            string patchname = config.patchname;

            if (File.Exists(patchname))
                File.Delete(patchname);
            else
                Directory.CreateDirectory(patchname.Substring(0, patchname.LastIndexOf('/')));

            List<Pair<string, Stream>> fileitems = new List<Pair<string, Stream>>();

            // 添加要删除的文件到包里
            if (deleteStr.Length != 0)
            {
                byte[] data = Encoding.UTF8.GetBytes(deleteStr.ToString());
                MemoryStream stream = new MemoryStream(data);
                fileitems.Add(new Pair<string, Stream>("deletelist.txt", stream));
            }

            //lzma.lzma_init_alloc_root();

            // 添加改变的文件到包里
            string name;
            foreach (string file in atlerList)
            {
                name = prefix + file;                
                FileStream fs = new FileStream(config.to + "/" + file, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileitems.Add(new Pair<string, Stream>(name, fs));
            }

            try
            {
                Archive.Generate(patchname, fileitems, new byte[1024 * 1024]);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                UnityEngine.Debug.LogError("补丁生成出错!");
                return true;
            }

            UnityEngine.Debug.Log(string.Format("{0}补丁生成成功!", config.patchname));
            return true;
        }
    }
}
#endif