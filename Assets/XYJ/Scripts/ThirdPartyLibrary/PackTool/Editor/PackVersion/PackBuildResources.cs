#if USE_RESOURCESEXPORT
using System;
using XTools;
using System.IO;
using UnityEngine;
using PackTool.Patch;
using System.Collections;
using System.Collections.Generic;

namespace PackTool.New
{
    // 资源包
    public class PackBuildResources
    {
        public PackBuildResources()
        {
            isDone = false;
            isAlter = true;
            patchname = "";
        }

        public bool isDone { get; protected set; }

        // 与上个版本相比是否变动了
        public bool isAlter { get; protected set; }

        // 当前资源的版本号，注意，只记录到x.y.z,xy为程序的版本号，z为资源的版本号
        public Version current { get; protected set; }

        // 得到当前的版本号
        Version GetCurrentVersion()
        {
            string packFilePath = AssetsExport.PackPath + ResourceConfig.filename;
            if (File.Exists(packFilePath))
            {
                ResourceConfig config = new ResourceConfig();
                StreamReader reader = new StreamReader(packFilePath);
                config.Load(reader);
                reader.Close();

                return config.version;
            }

            return null;
        }

        public string patchname { get; protected set; } // 本地的补丁文件

        // 是否精简打包
        public bool isSimplify { get; protected set; }

        // 是否精简打包
        public IEnumerator Start(New.Versions config)
        {
            isSimplify = config.isSimplify;
            Version oldVersion = GetCurrentVersion();
            if (oldVersion == null)
            {
                Debuger.DebugLog("没有版本信息!");
                // 没有原先的版本信息
                if (isSimplify)
                {
                    Debuger.DebugLog("没有缓冲的数据，不能精简打包!");
                    isSimplify = false;
                }
            }
            else
            {
                if (oldVersion != config.startVersion)
                {
                    // 把原先的文件目录修改下
                    string dstPath = ResourcesPath.LocalBasePath + oldVersion.ToString() + "/";
                    Debuger.DebugLog("{0}->{1}", ResourcesPath.LocalDataPath, dstPath);
                    if (isSimplify)
                    {
                        Utility.CopyFolder(ResourcesPath.LocalDataPath, dstPath);
                    }
                    else
                    {
                        Directory.Move(ResourcesPath.LocalDataPath, dstPath);
                    }
                }
                else
                {
                    Debuger.DebugLog("新老版本号一至!version:{0}!", config.startVersion);
                }
            }

            if (isSimplify)
            {
                if (!Directory.Exists(AssetsExport.PackPath))
                {
                    Debuger.DebugLog("没有缓冲的数据，不能精简打包!");
                    isSimplify = false;
                }
            }
            else
            {
                if (Directory.Exists(AssetsExport.PackPath))
                {
                    Debuger.DebugLog("不是精简打包，删除已有资源!");
                    Directory.Delete(AssetsExport.PackPath, true);
                }
            }

            // 再开始打包
            ExportAllResources r = new ExportAllResources(isSimplify, true);
            r.Export();
            while (!r.isDone)
                yield return 0;

            // 
            current = config.startVersion;
            ResourceConfig resconfig = new ResourceConfig();
            resconfig.version = current;
            resconfig.svn = config.svn;
            resconfig.SaveToFile();

            // 要比较的版本号
            if (current.Build > 0)
            {
                //oldVersion = (new System.Version(current.Major, current.Minor, current.Build - 1));
                string oldResPath = ResourcesPath.LocalBasePath + oldVersion.ToString() + "/";
                if (Directory.Exists(oldResPath))
                {
                    PatchGenerate.Data data = new PatchGenerate.Data();
                    data.from = oldResPath;
                    data.to = ResourcesPath.LocalDataPath;
                    data.patchprefix = "Data";
                    data.password = "";
                    // 补丁的名字
                    data.patchname = ResourcesPath.LocalBasePath + string.Format("{0}-{1}.{2}", oldVersion.ToString(), current.ToString(), AssetZip.patchsuffix);
                    patchname = data.patchname;
                    isAlter = PatchGenerate.Generate(data,
                        (List<string> cf) =>
                        {
                            if (cf.Count == 1 && cf[0].EndsWith(ResourceConfig.filename))
                                return true;
                            return false;
                        });

                    if (!isAlter) // 没变化
                    {
                        Debug.Log("资源版本一至不需要更新!");
                        if (Directory.Exists(ResourcesPath.LocalDataPath))
                            Directory.Delete(ResourcesPath.LocalDataPath, true);
                        Directory.Move(oldResPath, ResourcesPath.LocalDataPath);
                        if (Directory.Exists(oldResPath))
                            Directory.Delete(oldResPath, true);

                        current = oldVersion;
                    }
                    else
                    {
                        // 有变化,生成补丁文件
                        string filepath;
                        foreach (VersionConfig vc in config.configs)
                        {
                            filepath = ResourcesPath.LocalBasePath + vc.GetPatchName(current, config.svn);
                            Directory.CreateDirectory(filepath.Substring(0, filepath.LastIndexOf('/')));
                            string dst = ResourcesPath.LocalBasePath + vc.GetPatchName(current, config.svn);
                            if (File.Exists(dst))
                                File.Delete(dst);
                            File.Copy(data.patchname, dst);
                        }
                    }
                }
            }

            // 把资源打成zip包
            Archive.AssetPackArchive(ResourcesPath.LocalDataPath, ResourcesPath.streamingAssetsPath + AssetZip.DefaultName, "");

            try
            {
                string filepath = ResourcesPath.LocalBasePath + AssetZip.DefaultName;
                if (File.Exists(filepath))
                    File.Delete(filepath);

                File.Copy(ResourcesPath.streamingAssetsPath + AssetZip.DefaultName, filepath);
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }

            isDone = true;
        }
    }
}
#endif