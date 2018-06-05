using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;

namespace PackTool
{
    public class HttpDown : ASyncOperation
    {
        public HttpDown(string u, float to)
        {
            downing = false;
            url = u;
            size = 0;
            bytesDownloaded = 0;
            timeout = to;
        }

        int downTotalbytes = 0;

        float timeout = 5.0f;

        public string url { get; protected set; } // 路径

        // 当前是否已经在下载当中了
        public bool downing { get; protected set; }

        public long size { get; protected set; } // 文件大小

        public int bytesDownloaded { get; protected set; } // 已经下载的字节

        public override float progress { get { return size == 0 ? 0 : (1.0f * bytesDownloaded / size); } } // 进度

        public float speed { get { float delay = timecheck.delay; return delay > 0 ? downTotalbytes / delay : 0; } }

        public byte[] bytes { get; protected set; }

        // 存储的数据流
        Stream stream = null;

        static int current_total = 0; // 当前加载的总数

        public static HttpDown Down(string url, string downfile, float timeout)
        {
            HttpDown ftp = new HttpDown(url, timeout);
            MagicThread.StartBackground(ftp.DownImp(downfile));

            return ftp;
        }

        public static HttpDown Down(string url, float timeout)
        {
            HttpDown ftp = new HttpDown(url, timeout);
            MagicThread.StartBackground(ftp.DownImp(null));

            return ftp;
        }

        void GetFileSize()
        {
            size = -1;
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                req = WebRequest.Create(url) as HttpWebRequest;
                req.Method = WebRequestMethods.Http.Head;
                req.Timeout = (int)(timeout * 1000);
                res = req.GetResponse() as HttpWebResponse;
                if (res.StatusCode == HttpStatusCode.OK)
                    size = res.ContentLength;

                res.Close();
                //req.Abort();

                if (size <= -1)
                    size = 0; // 说明是空的文件
            }
            catch (WebException wex)
            {
                error = wex.Message;
                if (res != null)
                    res.Close();

                if (req != null)
                    req.Abort();

                XYJLogger.LogError("GetHttpFileSize WebException url:{0} error:{1} StackTrace:{2}", url, wex.Message, wex.StackTrace);
                size = -1;
                error_message(wex.Message);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                if (res != null)
                    res.Close();

                if (req != null)
                    req.Abort();

                XYJLogger.LogError("GetHttpFileSize Exception url:{0} error:{1} StackTrace:{2}", url, ex.Message, ex.StackTrace);
                error_message(ex.Message);
                size = -1;
            }
        }

        public IEnumerator DownImp(string downfile)
        {
            while (current_total != 0)
                yield return 0;
            ++current_total;

            GetFileSize();
            while (size == -1)
            {
                System.Threading.Thread.Sleep(100);
                if (isDone)
                {
                    --current_total;
                    yield break;
                }
            }

            if (size == 0 || bytesDownloaded == size)
            {
                if (stream != null)
                    stream.Close();
                else
                    bytes = new byte[0];

                isDone = true;
                --current_total;
                yield break;
            }

            HttpWebRequest httpReq;
            try
            {
                httpReq = HttpWebRequest.Create(url) as HttpWebRequest;
                httpReq.Method = WebRequestMethods.Http.Get;
                httpReq.UseDefaultCredentials = true;
            }
            catch (System.Exception e)
            {
                error = e.Message;
                XYJLogger.LogError("http:{0} DownImp error:{1}! StackTrace:{2}", url, e.Message, e.StackTrace);
                isDone = true;
                --current_total;
                yield break;
            }

            FileStream fs = null;
            if (!string.IsNullOrEmpty(downfile))
            {
                try
                {
                    if (File.Exists(downfile))
                    {
                        bytesDownloaded = (int)(new FileInfo(downfile)).Length;
                        fs = new FileStream(downfile, FileMode.Append, FileAccess.Write);
                    }
                    else
                    {
                        Directory.CreateDirectory(downfile.Substring(0, downfile.LastIndexOf('/')));
                        fs = new FileStream(downfile, FileMode.Create, FileAccess.Write);
                        bytesDownloaded = 0;
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    isDone = true;
                    --current_total;

                    XYJLogger.LogError("打开文件失败!file:{0} error:{1}", downfile, ex.Message);
                    XYJLogger.LogException(ex);
                    yield break;
                }

                if (bytesDownloaded == size)
                {
                    fs.Close();
                    isDone = true;
                    --current_total;
                    yield break;
                }
                else if (bytesDownloaded != 0)
                {
                    httpReq.AddRange(bytesDownloaded);
                }
            }
            else
            {
                bytesDownloaded = 0;
            }

            try
            {
                HttpWebResponse httpRes = httpReq.GetResponse() as HttpWebResponse;
                XYJLogger.LogDebug("url:{0} size:{1} bytesDownloaded:{2} dif:{3}", url, size, bytesDownloaded, size - bytesDownloaded);

                Stream resStrm = httpRes.GetResponseStream();
                if (fs == null)
                {
                    Network.BitStream bitStream = new Network.BitStream((int)size);

                    int writeSize = bitStream.WriteRemain; // 可写入的空间
                    int len = 0;
                    while ((len = resStrm.Read(bitStream.Buffer, bitStream.WritePos, writeSize)) > 0)
                    {
                        bitStream.WritePos += len;
                        writeSize = bitStream.WriteRemain;
                        bytesDownloaded += len;
                        downTotalbytes += len;

                        if (writeSize == 0)
                            break;
                    }

                    bytes = bitStream.Buffer;
                }
                else
                {
                    int bufsize = (int)(size * 0.1);
                    bufsize = UnityEngine.Mathf.Clamp(bufsize, 1024, 1024 * 1024 * 5);

                    byte[] buffer = new byte[bufsize];
                    int len;
                    while ((len = resStrm.Read(buffer, 0, bufsize)) > 0)
                    {
                        fs.Write(buffer, 0, len);

                        bytesDownloaded += len;
                        downTotalbytes += len;
                    }

                    fs.Flush();
                    fs.Close();
                }

                if (size != bytesDownloaded)
                {
                    error = string.Format("downerror! size:{0}, downsize:{1}!", size, bytesDownloaded);
                }

                resStrm.Close();
                httpRes.Close();
                isDone = true;

                isDone = true;
                --current_total;
            }
            catch (System.Exception e)
            {
                XYJLogger.LogError("http:{0}流写入文件:{1}错误!error:{2}", url, downfile, e.Message);
                XYJLogger.LogException(e);
                error = url + e.Message;
                isDone = true;
                --current_total;
            }
        }
    }
}
 
