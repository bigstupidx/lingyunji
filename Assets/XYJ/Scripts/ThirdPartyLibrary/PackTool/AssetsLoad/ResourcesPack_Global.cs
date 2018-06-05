#if USE_RESOURCESEXPORT || USE_ABL
using XTools;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    // 资源包
    public partial class ResourcesPack
    {
        // 资源包
        static ResourcesPack Instance = null;

        public static ResourcesPack Current { get { return Instance; } }

        //public static PackRead CurrentRead { get { return Current.packRead; } }

        public static bool IsVaild { get { return Instance == null ? false : true; } }

#if ASSET_DEBUG
        static TimeTrack TimeTrackFindAsyncFile = TimeTrackMgr.Instance.Get("ResourcesPack.FindAsynFile");
        static TimeTrack TimeTrackFindBaseStream = TimeTrackMgr.Instance.Get("ResourcesPack.FindBaseStream");
        public static int FindFileSize(string file)
        {
            int size;
            int offset;
            Current.packRead.Find(file, out offset, out size);
            return size;
        }
#endif

        public static Stream FindBaseStream(string file)
        {
            int lenght = 0;
            return FindBaseStream(file, out lenght);
        }

        public static void EachAllFile(System.Action<string> files)
        {
            Current.packRead.EachAllFile(files);
        }


        public static Stream FindBaseStream(string file, out int lenght)
        {
#if ASSET_DEBUG
            object key = TimeTrackFindBaseStream.Begin(file);
#endif
            Stream stream = Current.packRead.FindBaseStream(file, out lenght);

#if ASSET_DEBUG
            TimeTrackFindAsyncFile.End(key);
#endif
            return stream;
        }

        public static string Find(string file, out int offset, out int lenght)
        {
#if ASSET_DEBUG
            object key = TimeTrackFindBaseStream.Begin(file);
#endif
            string dst = Current.packRead.Find(file, out offset, out lenght);

#if ASSET_DEBUG
            TimeTrackFindAsyncFile.End(key);
#endif
            return dst;
        }

        public static bool IsExistFile(string file)
        {
            return Current.packRead.IsExistFile(file);
        }

        //        public static AsyncFile PreLoadFile(string file)
        //        {
        //            return Current.packRead.PreLoadFile(file);
        //        }

        //        public static AsyncFile FindAsynFile(string file)
        //        {
        //#if ASSET_DEBUG
        //            return TimeTrackFindAsyncFile.Execution(
        //                    file,
        //                    () =>
        //                    {
        //#endif
        //                        return Current.packRead.FindAsyncFile(file);
        //#if ASSET_DEBUG
        //                    }) as AsyncFile;
        //#endif
        //        }

        static public void InitEmpty()
        {
            Release();
            if (Instance == null)
                Instance = new ResourcesPack();

            Instance.InitEmptyFile();
            isInitEnd = true;
        }

        // 外部目录
        static public void InitExterPath(string rootpath)
        {
            Release();

            if (Instance == null)
                Instance = new ResourcesPack();

            Instance.InitExterDirectory(rootpath);
            RegQuit();
            isInitEnd = true;
        }

        static public List<Archive> GetUniteZips()
        {
            List<Archive> zips = new List<Archive>(2);
            string dst = string.Empty;
            int offset = 0;
            if (StreamingAssetLoad.GetFile(AssetZip.DefaultName, out dst, out offset))
            {
                Archive archive = null;
                archive = new Archive(dst, offset);
                archive.InitDefault();
                zips.Add(archive);
            }
            else
            {
                Debuger.LogError("查找内部资源包失败!");
            }

            dst = ResourcesPath.LocalTempPath + AssetZip.patchname;
            if (File.Exists(dst))
            {
                Archive archive = new Archive(dst, 0);
                archive.InitDefault();

                if (archive.current_version == null)
                {
                    Debuger.ErrorLog("外部包没有版本文件!删除掉!");
                    archive.Close();
                    File.Delete(dst);
                }
                else
                {
                    if (zips.Count >= 1 && zips[zips.Count - 1].current_version >= archive.current_version)
                    {
                        // 内部包版本高于外部包，删除外部包
                        archive.Close();
                        File.Delete(dst);
                    }
                    else
                    {
                        zips.Add(archive);
                    }
                }
            }

#if COM_DEBUG
            // 扩展包一般用来做测试用，默认扩展包是版本最高的
            dst = ResourcesPath.LocalTempPath + AssetZip.extendedname;
            if (File.Exists(dst))
            {
                Archive archive = new Archive(dst, 0);
                archive.InitDefault();

                zips.Add(archive);

                Debuger.LogWarning("查找扩展包成功！");
            }
#endif

            if (zips.Count == 0)
            {
                Debuger.LogError("查找内外部资源包失败!");
            }

            return zips;
        }

        // 是否初始化结束
        public static bool isInitEnd { get; protected set; }

        // 内部资源包与外部补丁包的结合
		static public IEnumerator InitUnite()
        {
            Release();

#if RESOURCES_DEBUG
			OnceStep step = new OnceStep();
			yield return step.Next("begin get pack!");
#endif
            List<Archive> zips = GetUniteZips();
            if (zips.Count == 0)
            {
                Debuger.LogError("查找内外部资源包失败!");
                isInitEnd = true;
				yield break;
            }

#if RESOURCES_DEBUG
			yield return step.Next("开始初始化资源包!");
#endif

            // 设置当前的资源包
            Instance = new ResourcesPack();
            Instance.InitPatchUnite(zips);

#if RESOURCES_DEBUG
            yield return step.Next("资源包初始化结束!");
#endif
            RegQuit();

            isInitEnd = true;
        }

        static void RegQuit()
        {

        }


        static public void Release()
        {
            Debuger.Log("ResourcesPack.Release");
            isInitEnd = false;

            if (Instance != null)
            {
                Instance.Clear();
                Instance = null;
            }
        }

        static ResourceConfig LoadResourceConfig(IPackRead packRead)
        {
            if (packRead == null)
                return null;

            int lenght = 0;
            Stream stream = packRead.FindBaseStream(ResourceConfig.filename, out lenght);
            if (stream == null)
            {
                Logger.LogError("资源包当中没有包含版本文件!");
                ResourceConfig rc = new ResourceConfig();
#if UNITY_EDITOR
                rc.svn = 0;
                if (rc.version == null)
                    rc.version = new Version(0,0,0);
#endif
                return rc;
            }

            ResourceConfig config = new ResourceConfig();
            StreamReader reader = new StreamReader(stream);
            config.Load(reader);
            reader.Close();
            stream.Close();

            return config;
        }
    }
}
#endif