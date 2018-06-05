#if COM_DEBUG || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine;

namespace PackTool
{
    public class FtpUpDown
    {
        static WWW www;

        string ftpServerIP;
        string ftpUserID;
        string ftpPassword;
        FtpWebRequest reqFTP;

        public static void Test()
        {
            Debuger.Log("Test 1");
            // 根据uri创建FtpWebRequest对象
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create("ftp://192.168.1.200/Netease");
            Debuger.Log("Test 2");
            
            // 指定数据传输类型
            reqFTP.UseBinary = true;

            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential("patchlist", "123456");

            reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
            Debuger.Log("Test 3");
            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
            Debuger.Log("Test 4");
            response.Close();
            reqFTP.Abort();
        }

        private void Connect(bool isdir)//连接ftp
        {
            ParserPath(isdir);

            // 根据uri创建FtpWebRequest对象
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(ftpServerIP);
            reqFTP.UseBinary = true;
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }

        public FtpUpDown(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIP = ftpServerIP;
            this.ftpUserID = ftpUserID;
            this.ftpPassword = ftpPassword;
        }

        static string GetCopyFile(string file)
        {
            int index = file.LastIndexOf('.');
            if (index <= -1)
                return file + "__copy__";

            return file.Insert(index, "__copy__");
        }

        static Stream GetFileStream(string filename)
        {
            try
            {
                return File.OpenRead(filename);
            }
            catch (Exception)
            {
                string file = GetCopyFile(filename);
                while (File.Exists(file))
                    file = GetCopyFile(file);

                File.Copy(filename, file);
                MemoryStream stream = new MemoryStream(File.ReadAllBytes(file));
                File.Delete(file);
                return stream;
            }
        }

        // 上传
        public void Upload(string filename)
        {
            FileInfo fileInf = new FileInfo(filename);
            if (!fileInf.Exists)
            {
                Debuger.LogError("Not Exists filename:" + filename);
                return;
            }

            Connect(false);//连接         

            reqFTP.KeepAlive = false;

            // 指定执行什么命令
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // 上传文件时通知服务器文件的大小
            reqFTP.ContentLength = fileInf.Length;

            // 缓冲大小设置为kb
            int buffLength = 4096;
            byte[] buff = new byte[buffLength];

            int contentLen;

            try
            {
                // 打开一个文件流(System.IO.FileStream) 去读上传的文件
                Stream fs = GetFileStream(filename);

                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的kb
                contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // 关闭两个流
                strm.Close();
                fs.Close();
                reqFTP.Abort();
            }
            catch (Exception ex)
            {
                Debuger.Log(ftpServerIP);
                Debuger.LogException(ex);
            }
        }

        // 创建目录
        public void MakeDir()
        {
            if (DirectoryIsExist())
                return;

            try
            {
                Connect(false);//连接
                reqFTP.UseBinary = false;
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                reqFTP.Abort();
            }
            catch (Exception ex)
            {
                Debuger.Log("url:" + ftpServerIP);
                Debuger.LogException(ex);
            }
        }

        //删除目录
        public void delDir()
        {
            try
            {
                Connect(true);//连接      
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                reqFTP.Abort();
            }
            catch (Exception ex)
            {
                Debuger.LogException(ex);
            }
        }

        // 设置为目录路径
        void ParserPath(bool isdir)
        {
            if (isdir)
            {
                if (!ftpServerIP.EndsWith("/"))
                    ftpServerIP += "/";
            }
            else
            {
                // 不是路径的
                if (ftpServerIP.EndsWith("/"))
                    ftpServerIP = ftpServerIP.Substring(0, ftpServerIP.Length - 1);
            }
        }

        public string[] GetFileList()
        {
            //StringBuilder result = new StringBuilder();
            try
            {
                Connect(true);//连接
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                List<string> files = new List<string>();
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    files.Add(line);
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                reqFTP.Abort();
                return files.ToArray();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        public bool DirectoryIsExist()
        {
            string[] value = GetFileList();
            if (value == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
#endif