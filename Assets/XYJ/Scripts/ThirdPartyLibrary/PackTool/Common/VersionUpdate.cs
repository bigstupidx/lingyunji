#if USE_RESOURCESEXPORT
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using XTools;

namespace PackTool
{
    public class VersionUpdate : ASyncOperation
    {
        // 更新的步骤
        public enum Step
        {
            Null, // 还未开始
            DownPathlisting, // 下载更新文件当中
            DownPrograming,  // 下载程序包当中
            ShowProgramUrl,  // 显示程序包的下载地址
            DownPatching,    // 下载补丁当中
            InstallPatching, // 安装补丁当中
        }

        // 正在下载的文件列表
        public string downingfile { get; protected set; }

        // 当前需要下载的数据大小
        public int downsize { get; protected set; }

        public float downspeed { get; protected set; }

        // 当前下载大小
        public int currentsize { get; protected set; }

        Step mStep = Step.Null;

        public PackListLoad.Data programData;

        // 当前下载的补丁列表
        public List<PatchListLoad.Data> down_patch_list;

        // 当前正在下载的补丁
        public PatchListLoad.Data current_down;

        // 补丁合并
        public PatchUnite patchunite { get; protected set; }

        public bool isContinue = false; // 是否

        public Step CurrentStep { get { return mStep; } }

        string mPatchlists_program; // 补丁列表下载地址
        string mPatchlists_patch; // 补丁列表下载地址
        string Channel; // 渠道标识

        // 打开外部网址更新程序包
        public string program_down_url { get; protected set; }

        // 资源是否有更新
        public bool isAlterResources { get; protected set; }

        public VersionUpdate(string pathlist_program, string pathlist_patch, string channel)
        {
            mPatchlists_program = pathlist_program;
            mPatchlists_patch = pathlist_patch;
            Channel = channel;
            isAlterResources = false;
            MagicThread.StartBackground(Begin);
        }

        protected override void error_message(string message)
        {
            error = message;
            Logger.LogError("版本更新失败:" + message);
            isDone = true;
        }

        bool DownAndInitPackList()
        {
            HttpDown http_program = HttpDown.Down(mPatchlists_program, 5f);
            while (!http_program.isDone)
            {
                progress = http_program.progress * 0.5f;
                Thread.Sleep(100);
            }

            if (!string.IsNullOrEmpty(http_program.error))
            {
                error_message(string.Format("下载程序列表失败! error:{0}", http_program.error));
                return false;
            }

            if (!PackListLoad.Load(http_program.bytes, Channel, out programData))
            {
                error_message("程序列表格式错误!");
                return false;
            }

            return true;
        }

        bool DownAndInitPatchList()
        {
            HttpDown http_pach = HttpDown.Down(mPatchlists_patch, 5f);
            while (!http_pach.isDone)
            {
                progress = http_pach.progress * 0.5f;
                Thread.Sleep(100);
            }

            if (!string.IsNullOrEmpty(http_pach.error))
            {
                error_message(string.Format("下载补丁列表失败! error:{0}", http_pach.error));
                return false;
            }

#if UNITY_ANDROID
            string platform = "ad";
#elif UNITY_IPHONE
            string platform = "ios";
#else
            string platform = "ad";
#endif

            down_patch_list = new List<PatchListLoad.Data>();
            if (!PatchListLoad.Load(http_pach.bytes, platform, ResourcesPack.Current.config.version, down_patch_list))
            {
                Logger.LogError("url:{0} error!", http_pach.url);
                error_message("更新失败,请重试!");
                return false;
            }

            return true;
        }

        static public string GetUpdateTempPath(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return ResourcesPath.LocalTempPath + "UpdateTemp";

            return ResourcesPath.LocalTempPath + "UpdateTemp/" + filename;
        }

        // 开启版本的更新与检测
        void Begin()
        {
            mStep = Step.DownPathlisting;

            // 先下载程序包列表
            if (!DownAndInitPackList())
                return;

            // 下载完成,开始解析内容
            if (programData == null)
            {
                isDone = true;
                Logger.LogWarning("programData == null");
                return;
            }

            if (string.IsNullOrEmpty(programData.url))
            {
                error_message(string.Format("补丁列表当中查找程序包失败!channel:{0}", Channel));
                return;
            }

            Version current = ProgramConfig.Default.version;
            Logger.LogWarning("server version:{0} {1}", programData.version, current);

            // 获取当前游戏的版本号信息
            if (current > programData.version)
            {
                // 程序包大于服务器上的，那么不需要更新了
                Logger.LogWarning("localversion:{0} > serverversion:{1} 不需要更新!", current, programData.version);
                isDone = true;
                return;
            }
            else if (current < programData.version)
            {
                // 需要更新下程序包
#if UNITY_ANDROID && USE_APK
                downsize = programData.size;
                current_down = programData;
                progress = 0.0f;
                isContinue = false;
                mStep = Step.DownPrograming;
                while (!isContinue)
                    Thread.Sleep(100);

                currentsize = 0;
                downspeed = 0;
                downingfile = programData.downurls[0];
                program_down_url = Util.GetUpdateTempPath(Path.GetFileName(programData.downurls[0]));
                HttpsDown httpdowns = new HttpsDown(programData.downurls, program_down_url);
                while (!httpdowns.isDone)
                {
                    Thread.Sleep(100);
                    currentsize = (int)httpdowns.downsize;
                    downspeed = httpdowns.speed;
                    progress = httpdowns.progress; // 进度
                }

                if (!string.IsNullOrEmpty(httpdowns.error))
                {
                    error_message(StrUtil.GetStr("安装包:{0}下载失败!error:{1}", downingfile, httpdowns.error));
                    return;
                }
                else
                {
                    string filemd5 = "";
                    if (!programData.Md5Check(program_down_url, out filemd5))
                    {
                        error_message(StrUtil.GetStr("安装包:{0} md5效验失败! httpmd5:{1} realmd5:{2} ", downingfile, programData.md5, filemd5));
                        File.Delete(program_down_url);
                        return;
                    }
                }
#else
                program_down_url = programData.url;
#endif
                mStep = Step.ShowProgramUrl;
                //isDone = true; // 完成了
                return;
            }

            // 版本一至程序包不需要更新，检测下，是否需要更新下资源包
            if (!DownAndInitPatchList())
                return;

            progress = 0.4f;
            if (down_patch_list == null || down_patch_list.Count == 0)
            {
                Logger.LogDebug("本地版本与服务器版本一至或高于服务器版本，不需要更新！");
                isDone = true;
                return;
            }

            downsize = 0;
            for (int i = 0; i < down_patch_list.Count; ++i)
                downsize += down_patch_list[i].size;

            mStep = Step.DownPatching;
            isContinue = false;
            while (!isContinue)
                Thread.Sleep(100);

            // 开始下载补丁文件
            List<Pair<Version, string>> patchList = new List<Pair<Version, string>>();
            currentsize = 0;
            int totalsize = 0;
            downspeed = 0f;
            for (int i = 0; i < down_patch_list.Count; ++i)
            {
                current_down = down_patch_list[i];
                if (string.IsNullOrEmpty(current_down.url))
                {
                    error_message(string.Format("补丁:{0} 下载路径没有配置!", current_down.version.ToString()));
                    return;
                }

                Pair<Version, string> localpatch = new Pair<Version, string>();
                downingfile = current_down.url;

                string patchfile = Path.GetFileName(downingfile);
                localpatch.first = current_down.version;
                localpatch.second = GetUpdateTempPath(patchfile);
                patchList.Add(localpatch); // 保存的文件目录

                Logger.LogDebug("down patch! version:{0} url:{1}", current_down.version, downingfile);
                HttpDown http = HttpDown.Down(downingfile, localpatch.second, 5f);
                while (!http.isDone)
                {
                    currentsize = (int)(totalsize + http.bytesDownloaded);
                    downspeed = http.speed;
                    Thread.Sleep(100);
                }

                totalsize += current_down.size;
                currentsize = totalsize;

                if (http.error != null)
                {
                    error_message(string.Format("补丁:{0}:{1} 下载失败! error:{2}", current_down.version.ToString(), downingfile, http.error));
                    return;
                }

                string filemd5 = "";
                if (!current_down.Md5Check(localpatch.second, out filemd5))
                {
                    error_message(string.Format("补丁:{0} md5效验失败! httpmd5:{1} realmd5:{2} ", downingfile, current_down.md5, filemd5));
                    File.Delete(localpatch.second);
                    return;
                }
            }

            ResourcesPack.Release();

            // 安装补丁
            mStep = Step.InstallPatching;

            // 补丁合并包路径
            string datapackurl = ResourcesPath.LocalTempPath + AssetZip.patchname;
            string localTempPath = GetUpdateTempPath("Install" + AssetZip.patchname);
            try
            {
                if (File.Exists(localTempPath))
                    File.Delete(localTempPath);

                // 要开始安装补丁了，先把当前版本复制下
                if (File.Exists(datapackurl))
                    File.Copy(datapackurl, localTempPath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                error_message("硬盘空间不足，安装补丁失败!请清理部分空间后再尝试!");
                return;
            }

            // 所有的补丁都下载完成了，开始安装补丁
            patchunite = new PatchUnite();
            for (int i = 0; i < patchList.Count; ++i)
                patchunite.AddPatch(patchList[i].second, "");

            // 计算补丁包的安装时间
            patchunite.Begin(localTempPath);
            if (!string.IsNullOrEmpty(patchunite.error))
            {
                error_message("硬盘空间不足，安装补丁失败!请清理部分空间后再尝试!");
                return;
            }

            isAlterResources = true;

            // 所有的都处理完成了
            File.Delete(datapackurl);
            File.Move(localTempPath, datapackurl);

            // 删除目录下的所有临时文件
            Directory.Delete(GetUpdateTempPath(""), true);

            // 重新初始化资源包
            ResourcesGroup.Clear();
            IEnumerator  itor = ResourcesPack.InitUnite();
            while (itor.MoveNext())
            {
                Thread.Sleep(10);                
            }

            Logger.LogDebug("资源更新完成!");
            if (ResourcesPack.Current.config.version != patchList[patchList.Count - 1].first)
            {
                string text = string.Format("更新之后版本与配置版本不匹配!version:{0}->{1}", ResourcesPack.Current.config.version, patchList[patchList.Count - 1].first);//不导出中文
                GUITextShow.AddButton(text, (object p) => { }, null, 1);
                Logger.LogError(text);
            }

            // 下载完成了
            isDone = true;
        }
    }
}
#endif