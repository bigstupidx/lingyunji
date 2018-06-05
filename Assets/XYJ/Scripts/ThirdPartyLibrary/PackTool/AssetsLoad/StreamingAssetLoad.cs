using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using XTools;

namespace PackTool
{
    public class StreamingAssetLoad
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        static AssetZip ApkFile = null;
#elif UNITY_IPHONE
        static Dictionary<string, string> FileLowerToName = new Dictionary<string, string>(); // 小写文件名对应真实的文件名

        public static string FindFile(string file)
        {
            Init();
            string rf = file.ToLower();
            if (!FileLowerToName.TryGetValue(rf, out rf))
                return "";
            return rf;
        }
#endif

        public static void Init()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ApkFile == null)
            {
                ApkFile = new AssetZip();
                ApkFile.Init(ResourcesPath.dataPath);
            }
#elif UNITY_IPHONE
            if (FileLowerToName.Count == 0)
            {
                EachAllFile((string file) =>
                {
                    FileLowerToName.Add(file.ToLower(), file);
                });
            }
#endif
        }

        public static void Release()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (ApkFile != null)
            {
                ApkFile.Release();
                ApkFile = null;

                GC.Collect();
            }
#endif
        }

        public static bool FileExist(string file)
        {
            string dstfile = null;
            int offset = 0;
            if (GetFile(file, out dstfile, out offset))
                return true;

            return false;
        }

        public static bool GetFile(string file, out string dstfile, out int offset)
        {
//#if UNITY_EDITOR || UNITY_IOS || UNITY_STANDALONE_WIN
            dstfile = ResourcesPath.streamingAssetsPath + file;
            if (!File.Exists(dstfile))
            {
                dstfile = string.Empty;
                offset = 0;
                XYJLogger.LogError("StreamingAssetLoad.GetFile({0}) not exist!", dstfile);
                return false;
            }

            offset = 0;
            return true;
//#elif UNITY_ANDROID // 安卓平台的，数据存储在apk包当中
//            Init();
//            ZipFile.PartialInputStream stream = ApkFile.FindFileStream("assets/" + file) as ZipFile.PartialInputStream;
//            if (stream == null)
//            {
//                dstfile = string.Empty;
//                offset = 0;
//                return false;
//            }
//            else
//            {
//                dstfile = ResourcesPath.dataPath;
//                offset = (int)stream.StartPos;
//                return true;
//            }
//#endif
        }

        public static Stream GetFile(string file)
        {
#if UNITY_EDITOR || UNITY_IOS || UNITY_STANDALONE_WIN
            string fullfile = ResourcesPath.streamingAssetsPath + file;
            if (!File.Exists(fullfile))
            {
                XYJLogger.LogError("StreamingAssetLoad.GetFile({0}) not exist!", fullfile);
                return null;
            }

            try
            {
                return File.Open(fullfile, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception ex)
            {
                XYJLogger.LogError("StreamingAssetLoad.GetFile({0}) error:{1}!", fullfile, ex.Message);
                XYJLogger.LogException(ex);
                return null;
            }
#elif UNITY_ANDROID // 安卓平台的，数据存储在apk包当中
            Init();
            return ApkFile.FindFileStream("assets/" + file);
#endif
        }

        public static void EachAllFile(System.Action<string> fun)
        {
#if UNITY_EDITOR || UNITY_IPHONE || UNITY_STANDALONE_WIN
            string fullfile = ResourcesPath.streamingAssetsPath;
            FileSystemScanner scanner = new FileSystemScanner("", "");
            scanner.ProcessFile =
                (object sender, ScanEventArgs e) =>
                {
                    fun(e.Name.Substring(fullfile.Length).Replace('\\', '/'));
                };

            scanner.Scan(fullfile, true);
            
#elif UNITY_ANDROID // 安卓平台的，数据存储在apk包当中
            Init();
            ApkFile.EachAllFile(
                (string file) => 
                {
                    if (file.StartsWith("assets/"))
                        fun(file.Substring(7));
                });
#endif
        }

        public static bool GetFileBytes(string file, byte[] bytes, out int lenght)
        {
            lenght = 0;
            Stream stream = GetFile(file);
            if (stream == null)
                return false;

            lenght = (int)stream.Length;
            if (lenght > bytes.Length)
                return false;

            stream.Read(bytes, 0, lenght);
            return true;
        }
    }
}