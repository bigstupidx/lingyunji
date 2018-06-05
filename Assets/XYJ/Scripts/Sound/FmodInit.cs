#if USE_RESOURCESEXPORT || USE_ABL
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using PackTool;

public class FmodInit
{
    static FmodInit()
    {
        isInit = false;
    }

    public static bool isInit { get; private set; }

    const string format = "{0}Banks/{1}";

    static public string Root
    {
        get { return ResourcesPath.LocalTempPath + "Fmods/"; }
    }

    static public void Init()
    {
        if (isInit)
            return;

        isInit = true;
        string fileroot = Root; // 存放路径
        string versionfile = fileroot + "version";
        if (File.Exists(versionfile))
        {
            Version version = new Version(File.ReadAllText(versionfile));
            if (version == ResourcesPack.Current.config.version)
                return;
        }

        // 需要重新复制文件到文件，先删除旧目录
        XTools.Utility.DeleteFolder(fileroot);
        byte[] bytes = new byte[1024 * 1024]; // 创建1M的临时空间
        ResourcesPack.EachAllFile((string file) =>
        {
            if (file.StartsWith("Data/Sounds"))
            {
                // 拷贝到指定目录下
                Stream stream = ResourcesPack.FindBaseStream(file.ToLower());

                file = fileroot + file.Substring(12); // 存放路径
                Directory.CreateDirectory(file.Substring(0, file.LastIndexOf('/')));

                // 复制文件
                FileStream dst = new FileStream(file, FileMode.Create);
                XTools.Utility.CopyStream(stream, dst, bytes);
                dst.Close();
                stream.Close();
            }
        });

        WriteVersion(versionfile);
        bytes = null;
        System.GC.Collect();
    }

    // 写入版本文件
    static void WriteVersion(string file)
    {
        Directory.CreateDirectory(file.Substring(0, file.LastIndexOf('/')));
        string version = ResourcesPack.Current.config.version.ToString();
        File.WriteAllText(file, version);
    }
}
#endif