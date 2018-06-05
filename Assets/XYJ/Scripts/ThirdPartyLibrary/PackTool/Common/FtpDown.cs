#if COM_DEBUG || UNITY_EDITOR
using System;
using System.Collections;
using System.Net;
using System.IO;
using UnityEngine;
using XTools;

namespace PackTool
{
    public class FtpDown
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && (!UNITY_EDITOR)
        static WWW DefaultWWW;
#endif
        public FtpDown(string u)
        {
            downing = false;
            url = u;
            isDone = false;
            size = 0;
            bytesDownloaded = 0;
            mTimeCheck = new TimeCheck();
            mTimeCheck.begin();
            error = null;
        }

        public string url { get; protected set; } // 路径

        // 当前是否已经在下载当中了
        public bool downing { get; protected set; }

        public bool isDone { get; protected set; } // 是否完成

        public string error { get; protected set; } // 是否有错误发生

        public long size { get; protected set; } // 文件大小

        public int bytesDownloaded { get; protected set; } // 已经下载的字节

        public float progress { get { return 1.0f * bytesDownloaded / size; } } // 进度

        TimeCheck mTimeCheck;

        public float time { get { return mTimeCheck.delay; } }

        public static FtpDown Down(string url, string downfile)
        {
            FtpDown ftp = new FtpDown(url);
            MagicThread.StartBackground(ftp.DownImp(downfile));

            return ftp;
        }

        // 得到某个ftp服务器上的文件大小
        long GetFtpFileSize()
        {
            Uri u = new Uri(url);

            FtpWebRequest ftpReq;
            try
            {
                ftpReq = (FtpWebRequest)FtpWebRequest.Create(u);
                ftpReq.Credentials = new NetworkCredential("Anonymous", "");
                ftpReq.Method = WebRequestMethods.Ftp.GetFileSize;
                ftpReq.UseBinary = true;
                ftpReq.UsePassive = true;
            }
            catch (System.Exception e)
            {
                XYJLogger.LogError("Create ftp:{0} DownImp error:{1}! StackTrace:{2}", url, e.Message, e.StackTrace);
                return 0;
            }

            FtpWebResponse ftpRes = null;
            try
            {
                ftpRes = (FtpWebResponse)ftpReq.GetResponse();
            }
            catch (System.Exception e)
            {
                error = e.Message;
                isDone = true;

                XYJLogger.LogError("GetResponse ftp:{0} DownImp error:{1}! StackTrace:{2}", url, e.Message, e.StackTrace);
                return 0;
            }

            return ftpRes.ContentLength;
        }

        public IEnumerator DownImp(string downfile)
        {
            yield return 0;

            size = GetFtpFileSize();
            if (size == 0)
            {
                error = downfile + " file size zero!";
                isDone = true;

                XYJLogger.LogError("error:" + error);
                yield break;
            }

            downfile = downfile.Replace('\\', '/');
            Uri u = new Uri(url);

            FtpWebRequest ftpReq;
            try
            {
                ftpReq = (FtpWebRequest)FtpWebRequest.Create(u);
                ftpReq.Credentials = new NetworkCredential("Anonymous", "");
                ftpReq.Method = WebRequestMethods.Ftp.DownloadFile;
                //ftpReq.KeepAlive = false;
                ftpReq.UseBinary = true;
                ftpReq.UsePassive = true;
            }
            catch (System.Exception e)
            {
                error = e.Message;
                isDone = true;

                XYJLogger.LogError("ftp:{0} DownImp error:{1}! StackTrace:{2}", url, e.Message, e.StackTrace);
                yield break;
            }

            FileStream fs;
            if (File.Exists(downfile))
            {
                bytesDownloaded = (int)(new FileInfo(downfile)).Length;
                ftpReq.ContentOffset = bytesDownloaded;
                fs = new FileStream(downfile, FileMode.Append, FileAccess.Write);
            }
            else
            {
                Directory.CreateDirectory(downfile.Substring(0, downfile.LastIndexOf('/')));
                fs = new FileStream(downfile, FileMode.Create, FileAccess.Write);
            }

            FtpWebResponse ftpRes = null;
            try
            {
                ftpRes = (FtpWebResponse)ftpReq.GetResponse();
            }
            catch (System.Exception e)
            {
                error = e.Message;
                isDone = true;

                XYJLogger.LogError("ftp:{0} DownImp error:{1}! StackTrace:{2}", url, e.Message, e.StackTrace);
                yield break;
            }

            Stream resStrm = ftpRes.GetResponseStream();
            int bufsize = (int)(size * 0.1);
            bufsize = UnityEngine.Mathf.Clamp(bufsize, 1024, 1024 * 1024);
            XYJLogger.LogDebug("bufsize:" + bufsize);
            byte[] buffer = new byte[bufsize];
            while (true)
            {
                IAsyncResult result = resStrm.BeginRead(buffer, 0, buffer.Length, null, null);
                while (!result.IsCompleted)
                    yield return 0;

                int readsize = resStrm.EndRead(result);
                if (readsize == 0)
                    break;

                bytesDownloaded += readsize;
                fs.Write(buffer, 0, readsize);
            }

            fs.Close();
            resStrm.Close();
            ftpRes.Close();

            isDone = true;
        }
    }
}
#endif