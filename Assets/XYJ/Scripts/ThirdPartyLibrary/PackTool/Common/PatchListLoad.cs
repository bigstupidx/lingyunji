using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class PatchListLoad
    {
        const char compart = ' ';

        public class Data
        {
            public Version version;
            public string url;
            public string md5;
            public int size;

            public string toString()
            {
                return string.Format("{1}{0}{2}{0}{3}{0}{4}", compart, version.ToString(), url, md5, size);
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

        static bool isCanUsed(string platform, string url)
        {
            if (url.EndsWith(".all.wp"))
            {
                // 全平台通用的补丁
                return true;
            }
            else if (url.EndsWith(".ios.wp") && platform.Equals("ios"))
            {
                // ios平台
                return true;
            }
            else if (url.EndsWith(".ad.wp") && platform.Equals("ad"))
            {
                // 安卓平台
                return true;
            }

            return false;
        }

        static bool CheckVersion(Version version, List<Data> patchs, string platform)
        {
            for (int m = 0; m < patchs.Count; ++m)
            {
                if (patchs[m].version == version)
                {
                    XYJLogger.LogDebug("same version:{0} platform:{1} {2}", version.ToString(), platform, patchs[m].url);
                    return false;
                }
            }

            return true;
        }

        static public bool Load(byte[] bytes, string platform, Version current, List<Data> patchs)
        {
            // 解析文件
            CsvCommon.CsvReader reader = new CsvCommon.CsvReader();
            reader.LoadByText(System.Text.Encoding.UTF8.GetString(bytes), compart);

            // 0 版本号
            // 1 patch下载地址
            // 2 patch的md5
            // 3 patch大小
            for (int i = 0; i < reader.YCount; ++i)
            {
                Version version = new Version(reader.getStr(i, 0));
                XYJLogger.LogWarning("platform:{0} current:{1} version:{2}", platform, current, version);
                if (version > current)
                {
                    string url = reader.getStr(i, 1);
                    if (!isCanUsed(platform, url))
                    {
                        XYJLogger.LogDebug("platform not match!{0} {1}", platform, url);
                        continue;
                    }

                    if (!CheckVersion(version, patchs, platform))
                    {
                        return false;
                    }

                    // 需要更新的补丁
                    Data patch = new Data();
                    patch.version = version;
                    patch.url = url;
                    patch.md5 = reader.getStr(i, 2);
                    patch.size = reader.getInt(i, 3, 0);

                    patchs.Add(patch);
                    XYJLogger.LogDebug("update patch!", patch.toString());
                }
            }

            patchs.Sort((Data x, Data y) => 
            {
                return x.version.CompareTo(y.version);
            });

            return true;
        }
    }
}