using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class PackListLoad
    {
        const char compart = ' ';

        public class Data
        {
            public Version version;
            public string url;
        }

        static public bool Load(byte[] bytes, string channel, out Data data)
        {
            // 解析文件
            CsvCommon.CsvReader reader = new CsvCommon.CsvReader();
            reader.LoadByText(System.Text.Encoding.UTF8.GetString(bytes), compart);

            data = null;

            // 版本号(0)
            // patch下载地址
            // patch的md5(2)
            // patch大小(3)
            for (int i = 0; i < reader.YCount; ++i)
            {
                string httpurl = reader.getStr(i, 1);
                if (string.IsNullOrEmpty(httpurl))
                {
                    XYJLogger.LogError("下载地址格式错误!" + reader.getStr(i, 1));
                    return false;
                }

                if (httpurl.StartsWith("appstoreurl="))
                {
                    // 苹果ios的更新，如果当前不是ios平台那就取消掉
                    if (!channel.Equals("app_store"))
                        continue;
                    httpurl = httpurl.Substring(12);
                }
                else
                {
                    // 安装包格式(-分隔) 项目名-版本号-渠道.扩展名
                    int start = httpurl.LastIndexOf('-');
                    int end = httpurl.LastIndexOf('.');
                    if (end <= start || start < 0 || end < 0)
                    {
                        XYJLogger.LogError("更新文件http下载地址格式错误!http:" + httpurl);
                        return false;
                    }

                    if (!httpurl.Substring(start+1, end - start - 1).Equals(channel))
                        continue; // 渠道不同
                }

                Version current = new Version(reader.getStr(i, 0));
                XYJLogger.LogDebug("version:{0} url:{1}", current.ToString(), httpurl);
                if (data == null)
                {
                    data = new Data();
                    data.version = current;
                    data.url = httpurl;
                }
                else if (data.version < current)
                {
                    data.version = current;
                    data.url = httpurl;
                }
            }

            return true;
        }
    }
}