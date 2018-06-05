using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public class PatchListFile
    {
        const char compart = ' ';

        public PatchListFile(string cl)
        {
            patchs = new List<Data>();
            programs = new List<Data>();
            channel = cl;
        }

        // 程序包信息
        public Data program
        {
            get
            {
                if (programs == null || programs.Count == 0)
                    return null;

                if (string.IsNullOrEmpty(channel))
                    return programs[0];

                Data maxVersion = null; // 当前最高的版本号
                foreach (Data d in programs)
                {
                    if (d.channel == channel)
                    {
                        if (maxVersion == null)
                            maxVersion = d;
                        else if (d.version > maxVersion.version)
                            maxVersion = d;
                    }
                }

                return maxVersion;
            }
        }

        public List<Data> patchs { get; protected set; }

        public List<Data> programs { get; protected set; }

        public string channel { get; protected set; }

        public class Data
        {
            public Version version; // 版本号
            public List<string> downurls = new List<string>(); // 下载地址
            public string channel; // 服务器过滤渠道
            public string md5; // 文件的md5值
            public int size; // 文件的大小

            public string toString()
            {
                return string.Format("{1}{0}{2}{0}{3}{0}{4}", compart, version.ToString(), Utility.UnionURL(downurls), md5, size);
            }

            // md5检测是否合格
            public bool Md5Check(string filename, out string filemd5)
            {
                filemd5 = Md5.GetFileMd5(filename);
                if (filemd5 == md5)
                    return true;
                else
                    return false;
            }
        }

        public void SaveToFile(string filename)
        {
            List<Data> writeList = new List<Data>(programs);
            writeList.AddRange(patchs);
            SaveToFile(writeList, filename);
        }

        static public void SaveToFile(List<Data> writeList, string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
            else
                Directory.CreateDirectory(filename.Substring(0, filename.LastIndexOf('/')));

            StreamWriter writer = new StreamWriter(filename);
            if (writeList.Count >= 1)
                writer.Write(writeList[0].toString());

            for (int i = 1; i < writeList.Count; ++i)
            {
                writer.WriteLine();
                writer.Write(writeList[i].toString());
            }

            writer.Close();
        }

        static public PatchListFile Load(byte[] bytes, string channel)
        {
            // 解析文件
            PatchListFile pathlistfile = new PatchListFile(channel);
            pathlistfile.Load(bytes);
            return pathlistfile;
        }

        static Data HandleURL(string url, out List<string> httpsdown, out bool isprogram)
        {
            httpsdown = Utility.SplitURL(url);
            isprogram = true;
            if (httpsdown.Count <= 0)
            {
                XYJLogger.LogError("httpsdown.Count 更新文件http下载地址格式错误!http:" + url);
                return null;
            }

            // 是否程序包
            string http = httpsdown[0];
            if (http.StartsWith("appstoreurl="))
            {
                // ios的更新渠道
                for (int i = httpsdown.Count - 1; i >= 0; --i)
                {
                    if (httpsdown[i].Length > 12)
                    {
                        httpsdown[i] = httpsdown[i].Substring(12);
                    }
                    else
                    {
                        XYJLogger.LogError("地址:{0} 配置不规范!", httpsdown[i]);
                        httpsdown.RemoveAt(i);
                    }
                }

                if (httpsdown.Count <= 0)
                {
                    XYJLogger.LogError("httpsdown.Count 更新文件http下载地址格式错误!http:" + url);
                    return null;
                }

                isprogram = true;
                Data d = new Data();
                d.channel = "app_store";
                return d;
            }
            else
            {
                if (http.ToLower().EndsWith(AssetZip.patchsuffix))
                    isprogram = false; // 补丁包

                // 安装包格式(-分隔) 项目名-版本号-渠道.扩展名
                // 补丁包(.分隔)     patch版本号.项目名.渠道.扩展名
                string[] httpslips = http.Split(isprogram ? '-' : '.');
                if (httpslips.Length < 3)
                {
                    XYJLogger.LogError("更新文件http下载地址格式错误!http:" + http);
                    return null;
                }

                Data data = new Data();
                if (isprogram)
                {
                    string s = httpslips[httpslips.Length - 1];
                    int pos = s.LastIndexOf('.');
                    if (pos == -1)
                    {
                        XYJLogger.LogError("channels.Length 更新文件http下载地址格式错误!http:" + url);
                        return null;
                    }

                    data.channel = s.Substring(0, pos);
                }
                else
                {
                    data.channel = httpslips[httpslips.Length - 2];
                }

                return data;
            }
        }

        public bool Load(byte[] bytes)
        {
            // 解析文件
            CsvCommon.CsvReader reader = new CsvCommon.CsvReader();
            reader.LoadByText(System.Text.Encoding.UTF8.GetString(bytes), compart);

            // 版本号(0)
            // patch下载地址
            // patch的md5(2)
            // patch大小(3)
            for (int i = 0; i < reader.YCount; ++i)
            {
                List<string> httpsdown = null;
                bool isprogram;
                Data data = HandleURL(reader.getStr(i, 1), out httpsdown, out isprogram);
                if (data == null)
                {
                    XYJLogger.LogError("下载地址格式错误!" + reader.getStr(i, 1));
                    return false;
                }

                data.version = new Version(reader.getStr(i, 0));
                data.downurls = httpsdown;
                if (data.downurls.Count == 0)
                {
                    XYJLogger.LogError("补丁列表下载地址为空!");
                    return false;
                }

                data.md5 = reader.getStr(i, 2);
                data.size = reader.getInt(i, 3, 0);

                if (isprogram)
                    programs.Add(data);
                else
                    patchs.Add(data);
            }

            patchs.Sort(ComparisonPatch);
            return true;
        }

        static int ComparisonPatch(Data x, Data y)
        {
            if (x.version == y.version)
                return 0;
            else if (x.version > y.version)
                return -1;
            return 1;
        }

        // 资源包最新版本
        public Version GetMaxPatchVersion()
        {
            if (patchs == null || patchs.Count == 0)
                return null;

            return patchs[0].version;
        }

        public int GetVersionIndex(Version version)
        {
            Version maxVersion = GetMaxPatchVersion();
            if (maxVersion == null || version > maxVersion)
            {
                XYJLogger.LogDebug("GetVersionIndex:maxVersion:{0} version:{1}", maxVersion, version);
                return -2; // 已经是最高版本了
            }

            for (int i = 0; i < patchs.Count; ++i)
            {
                if (patchs[i].version == version)
                    return i;
            }

            return -1;
        }
    }
}