#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    public class CodeCheckAtler
    { 
        class Alter
        {
            // 旧版本的资源md5值
            Dictionary<string, string> Old_CodeFileList = new Dictionary<string, string>();

            // 新版本的资源md5值
            Dictionary<string, string> New_CodeFileList = new Dictionary<string, string>();

            public int old_total { get { return Old_CodeFileList.Count; } }

            public string filepath;
 
            public void Init(string file)
            {
                filepath = file;

                LoadConfig(); // 读取配置文件
            }
            
            // 脚本是否有修改
            public bool isAlter(string file)
            {
                file = file.Substring(7);
                string file_meta = file + ".meta";
                string old_md5 = GetMd5(Old_CodeFileList, file);
                string old_md5_meta = GetMd5(Old_CodeFileList, file_meta);
                if (old_md5 == null || old_md5_meta == null)
                {
                    New_CodeFileList[file] = GetFileMd5(file);
                    New_CodeFileList[file_meta] = GetFileMd5(file_meta);
                    return true;
                }

                string new_md5 = GetMd5(New_CodeFileList, file);
                if (new_md5 == null)
                {
                    new_md5 = GetFileMd5(file);
                    New_CodeFileList[file] = new_md5;
                }

                string new_md5_meta = GetMd5(New_CodeFileList, file_meta);
                if (new_md5_meta == null)
                {
                    new_md5_meta = GetFileMd5(file_meta);
                    New_CodeFileList[file_meta] = new_md5_meta;
                }

                if (old_md5 == new_md5 && old_md5_meta == new_md5_meta)
                    return false; // 没有修改

                return true;
            }

            // 配置文件
            public string ConfigFile 
            {
                get
                {
                    return string.Format("{0}{1}__deplist__.csv", AssetsExport.TempPath, filepath);
                }
            }
            
            public void SaveToFile()
            {
                Save(ConfigFile);
            }
            
            void LoadConfig()
            {
                Old_CodeFileList.Clear();
                string filePath = ConfigFile;
                if (!File.Exists(filePath))
                    return; // 没有此配置文件

                CsvCommon.CsvReader csv = new CsvCommon.CsvReader();
                var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                csv.LoadStream(stream, ',');
                stream.Close();

                string file;
                string md5;
                for (int i = 0; i < csv.YCount; ++i)
                {
                    file = csv.getStr(i, 0).Replace('\\', '/');
                    md5 = csv.getStr(i, 1);
                    if (md5[0] == '"' && md5[md5.Length - 1] == '"')
                        md5 = md5.Substring(1, md5.Length - 2);

                    Old_CodeFileList.Add(file, md5);
                }
            }

            // 写入MD5文件
            void Save(string file)
            {
                StreamWriter writer = null;
                string fileName = file.Replace('\\', '/');
                if (!File.Exists(fileName))
                {
                    string path = fileName.Substring(0, fileName.LastIndexOf('/'));
                    Directory.CreateDirectory(path);
                    writer = File.CreateText(fileName);
                }
                else
                {
                    File.Delete(fileName);
                    FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                    writer = new StreamWriter(fs);
                }
                
                foreach (KeyValuePair<string, string> itor in New_CodeFileList)
                    writer.WriteLine(string.Format("\"{0}\",\"{1}\"", itor.Key, itor.Value));
                
                writer.Flush();
                writer.BaseStream.Close();
                writer.Close();

                writer.Dispose();
            }
        }

        // 一份资源的对应的依赖
        static Dictionary<string, Alter> AllList = new Dictionary<string, Alter>();

        public static void Release()
        {
            AllList.Clear();
            dependenceList.Release();
            FindListMd5.Clear();
        }

        static DependenceList dependenceList = new DependenceList();

        static string GetMd5(Dictionary<string, string> dic, string file)
        {
            string md5;
            if (dic.TryGetValue(file, out md5))
                return md5;

            return null;
        }

        static public string GetFileMd5(string file)
        {
            string md5 ;
            if (FindListMd5.TryGetValue(file, out md5))
                return md5;

            md5 = Md5.GetFileMd5(Application.dataPath + "/" + file);
            FindListMd5.Add(file, md5);
            return md5;
        }

        static Dictionary<string, string> FindListMd5 = new Dictionary<string, string>();

        // assets
        public static bool isAlter(string assetPath)
        {
            Alter alter = null;
            string key = assetPath.Substring(7);
            if (!AllList.TryGetValue(key, out alter))
            {
                alter = new Alter();
                alter.Init(key);
                AllList.Add(key, alter);
            }

            DependenceList.DepList deplist = dependenceList.GetDepList(assetPath);

            bool bhit = false;
            foreach (string src in deplist.Dic)
            {
                if (alter.isAlter(src))
                {
                    bhit = true;
                }
            }

            if (alter.old_total != (deplist.Dic.Count * 2))
            {
                bhit = true;
            }

            return bhit;
        }

        public static void SaveToFile()
        {
            foreach (KeyValuePair<string, Alter> itor in AllList)
            {
                itor.Value.SaveToFile();
            }

            Release();
        }
    }
}
#endif