

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



/// <summary>
/// gm日志
/// </summary>
/// 
namespace xys.gm
{

    public class GM_Log
    {
        static private bool s_showLog = true;
        static private string s_log = string.Empty;
        static Vector2 scrollposition;
        static public void LogUI(Rect rect)
        {
            GUILayout.BeginArea(rect);

            if (GUILayout.Button("日志", GUILayout.Width(60)))
                s_showLog = !s_showLog;

            bool v = false;
			v = ConsoleSelf.GetEnabled();
            if (GUILayout.Button(v ? "隐藏日志" : "显示日志"))
            {
                ConsoleSelf.CreateInstance();
                ConsoleSelf.SetEnabled(!v);
            }

#if UNITY_EDITOR || COM_DEBUG
            if (GUILayout.Button("上传日志到服务器"))
            {
                UpLog();
            }
#endif

#if ASSET_DEBUG
            if (GUILayout.Button("记录加载文件"))
            {
                PackTool.TimeTrackMgr.Instance.Save();
            }
#endif

            if (s_showLog)
            {
                //日志
                if (GUILayout.Button("清理日志"))
                    ClearLog();
                //
                if (GUILayout.Button("打印GM命令"))
                {
                    GM_Cmd.instance.ParseCmd("help");
                }

                //太长了截取一般
                if (s_log.Length > 5000)
                    s_log = s_log.Substring(s_log.Length / 2);

                scrollposition = GUILayout.BeginScrollView(scrollposition);
                GUILayout.TextArea(s_log);
                GUILayout.EndScrollView();
            }

            GUILayout.EndArea();
        }

        /// <summary>
        /// Clear Log Message
        /// </summary>
        static public void ClearLog()
        {
            s_log = "";
        }

        /// <summary>
        /// Log Message
        /// </summary>
        static public void SetLog(string msg)
        {
            s_log = msg;
        }

        static public void AddLog(string msg)
        {
            s_log += msg+"\r\n";
        }

#if UNITY_EDITOR || COM_DEBUG
        // 上传文件到FTP上
        public static void UpdateToFtpServer(List<CommonBase.Pair<string, string>> upftplist, string ftp, string usename, string password)
        {
            if (ftp[ftp.Length - 1] == '/')
                ftp = ftp.Substring(0, ftp.Length - 1);
#if !SCENE_DEBUG
            PackTool.FtpUpDown ftpupdown = new PackTool.FtpUpDown(ftp, usename, password);
            ftpupdown.MakeDir();

            // 把文件拷贝到服务器上
            string ftppathurl;
            foreach (CommonBase.Pair<string, string> itor in upftplist)
            {
                string[] paths = itor.first.Split('/');
                ftppathurl = ftp + "/";
                for (int i = 0; i < paths.Length - 1; ++i)
                {
                    ftppathurl += paths[i];
                    ftpupdown = new PackTool.FtpUpDown(ftppathurl, usename, password);
                    ftpupdown.MakeDir();
                    ftppathurl += "/";
                }

                ftpupdown = new PackTool.FtpUpDown(ftp + "/" + itor.first, usename, password);
                ftpupdown.Upload(itor.second);

                Debuger.Log("上传文件:" + itor.second + "到" + ftp + "/" + itor.first);
            }
#endif
        }

        static public void UpLog()
        {
            string[] files = System.IO.Directory.GetFiles(XYJLogger.RootPath, "*.log");
            if (files.Length == 0)
            {
                Debuger.Log("查找日志失败!");
                return;
            }
            Debuger.Log("日志数量:" + files.Length);

            string name = "";
            if (App.my.appStateMgr.curState == AppStateType.GameIn)
            {
                if (App.my != null && App.my.localPlayer != null)
                {
                    name = NPinyin.Pinyin.GetPinyin(App.my.localPlayer.name) + "_";
                }
            }

            name += XYJLogger.GetNowTime();
            string ftp = "ftp://192.168.1.200/Common/";
            List<CommonBase.Pair<string, string>> upftplist = new List<CommonBase.Pair<string, string>>();
            foreach (string file in files)
                upftplist.Add(new CommonBase.Pair<string, string>("xysgamelog/" + name + "/" + System.IO.Path.GetFileName(file), file));

            UpdateToFtpServer(upftplist, ftp, "Anonymous", "");

            foreach (string file in files)
            {
                try
                {
                    if (file == XYJLogger.Instance.logfile)
                        continue;

                    System.IO.File.Delete(file);
                }
                catch (Exception)
                {

                }
            }
        }
#endif
    }
}
