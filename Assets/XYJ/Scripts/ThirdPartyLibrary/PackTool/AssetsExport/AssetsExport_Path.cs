#if UNITY_EDITOR && (USE_RESOURCESEXPORT || USE_ABL)
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public partial class AssetsExport
    {
        public static readonly string Folder = ResourcesPath.LocalBasePath;

        // 打包路径
        static public readonly string PackPath = Folder + "Data/";

        // 快速更新
        public static readonly string TempPath = Application.dataPath + "/../PackTmp/" + ResourcesPath.PlatformKey + "/";

        static string GetSavePath(Object o)
        {
            return PackPath + ConvPath(o);
        }

        public static string ConvPath(Object o)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (string.IsNullOrEmpty(path))
                return o.name.Replace('/', '_');

            path = path.Substring(7);
            return path;
        }

        //         static GenerateMd5FileList s_g = null;

        //          [MenuItem("PackTool/资源更新/生成MD5文件")]
        //          public static void GenerateMd5()
        //          {
        //              // 生成打包目录下的md5文件列表
        //              if (s_g == null)
        //                  s_g = new GenerateMd5FileList();
        // 
        //              if (s_g.isDone)
        //              {
        //                  GenerateMd5FileList g = new GenerateMd5FileList();
        //                  g.Generate(Md5File, PackPath, true);
        //              }
        //          }

        [MenuItem("PackTool/资源更新/更新配置至资源包")]
        public static void UpdateConfigPack()
        {
            // 生成打包目录下的md5文件列表
            CopyConfigFile();
            UpdatePack();
        }

        // 复制配制文件到目录
        public static void CopyConfigFile()
        {
            // 先拷贝文件到导出目录
            Utility.CopyConfigToPath(PackPath);
        }

        public static void UpdatePack(System.Action end = null)
        {
            // 把资源打成zip包
            Archive.AssetPackArchive(ResourcesPath.LocalDataPath, ResourcesPath.streamingAssetsPath + AssetZip.DefaultName, "");
            Debuger.DebugLog("生成资源包:" + AssetZip.DefaultName);

            if (end != null)
                end();
        }

        [MenuItem("PackTool/资源更新/重新生成资源包")]
        public static void ResetPackZip()
        {
            Archive.AssetPackArchive(ResourcesPath.LocalDataPath, ResourcesPath.streamingAssetsPath + AssetZip.DefaultName, "");
        }
    }
}

#endif
