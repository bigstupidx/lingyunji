// #if UNITY_EDITOR
// 
// using UnityEngine;
// using UnityEditor;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// 
// namespace PackTool
// {
//     public class AlreadPackFileList
//     {
//         // 当前已经加载的md5文件列表
//         static Dictionary<string, string> FileList;
//         static bool IsInit = false; // 当前是否已经初始化
//         static string Md5File = AssetExportMgr.TempPath + "AlreadPackList.csv"; // 配置文件
//         static void Init()
//         {
//             if (IsInit == true)
//                 return;
// 
//             if (FileList == null)
//                 FileList = new Dictionary<string, string>();
// 
//             IsInit = true;
//             string filePath = Md5File;
//             if (!File.Exists(filePath))
//                 return; // 没有此配置文件
// 
//             StreamReader reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
//             Csv.CsvReader csv = new Csv.CsvReader();
//             csv.LoadByText(reader.ReadToEnd(), ',');
//             reader.Close();
// 
//             string file;
//             string md5;
//             for (int i = 0; i < csv.getYCount(); ++i)
//             {
//                 file = csv.getStr(i, 0).Replace('\\', '/');
//                 md5 = csv.getStr(i, 1);
//                 FileList.Add(file, md5);
//             }
//         }
// 
//         public static void RemoveFile(string file)
//         {
//             Init();
// 
//             file = file.Substring(7);
//             FileList.Remove(file);
//             FileList.Remove(file + ".meta");
//         }
// 
//         // 文件是否一至
//         public static bool isSame(string filename, bool bToMd5)
//         {
//             Init();
// 
//             string fullPath = filename;
//             if (!fullPath.Contains(":"))
//                 fullPath = Application.dataPath + "/" + fullPath;
// 
//             string md5 = Md5.GetFileMd5(fullPath);
//             if (string.IsNullOrEmpty(md5))
//             {
//                 Debuger.LogError("file:" + filename + " not find!");
//                 return false;
//             }
// 
//             string old = "";
//             if (FileList.TryGetValue(filename, out old))
//             {
//                 if (md5 == old)
//                     return true;
//             }
// 
//             if (bToMd5 == true)
//             {
//                 FileList[filename] = md5;
//             }
// 
//             return false;
//         }
// 
//         // 写入MD5文件
//         public static void SaveToFile()
//         {
//             if (!IsInit)
//                 return;
// 
//             ThreadFileSave FileSave = new ThreadFileSave();
//             FileSave.init(FileList, Md5File);
//             FileSave.Release();
// 
//             IsInit = false;
//             FileList.Clear();
//         }
//     }
// }
// 
// #endif