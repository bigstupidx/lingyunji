using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    // 程序的配置信息
    public class ResourceConfig
    {
        public const string filename = "resoruce_config";

        // 程序的版本号
        public Version version = new Version(0, 0, 0);

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
            string file = ResourcesPath.LocalDataPath  + "/" + filename;
            if (File.Exists(file))
                File.Delete(file);
            else
                Directory.CreateDirectory(ResourcesPath.LocalDataPath);

            StreamWriter writer = new StreamWriter(file, false, System.Text.Encoding.UTF8);
            SaveToStream(writer);
            writer.Close();
        }

        public void SaveToStream(StreamWriter writer)
        {
            writer.WriteLine(version.ToString());
            writer.Write(svn);
            writer.Flush();
        }
#endif
        public void Load(StreamReader reader)
        {
            string line = reader.ReadLine();
            version = new Version(line);

            line = reader.ReadLine();
            svn = long.Parse(line);
        }
    }
}