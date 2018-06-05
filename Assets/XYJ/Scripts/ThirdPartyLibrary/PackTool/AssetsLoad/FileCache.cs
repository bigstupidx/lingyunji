//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;
//using XTools;

//namespace PackTool
//{
//    // 多线程文件保存
//    internal class FileCache
//    {
//        const string crtfile = "crt.fc";
//        StreamWriter mWriter;
//        string mPath; // 路径

//        // 最多缓存1G的数据
//        const int totalNum = 1024 * 1024 * 1025;

//        int current_total = 0; // 当前缓存的文件总大小

//#if ASSET_DEBUG
//        static TimeTrack filecache_GetFile = TimeTrackMgr.Instance.Get("FileCache.GetFile");
//#endif

//        class Data
//        {
//            public Data(string f, int crc, int s)
//            {
//                file = f;
//                crc32 = crc;
//                size = s;
//            }

//            public string file; // 文件
//            public int crc32; // 对应的crc效验值
//            public int size; // 文件大小
//        }

//        // key值对应的文件列表
//        Dictionary<string, Data> keyToFiles = new Dictionary<string, Data>();

//        bool isRuning = true;

//        public void Release()
//        {
//            isRuning = false;
//            if (mThread != null)
//            {
//                System.Threading.Thread.Sleep(100);

//                Utility.AbortThread(mThread);
//                mThread = null;
//            }
            
//            try
//            {
//                if (mWriter != null)
//                {
//                    mWriter.Close();
//                    mWriter = null;
//                }
//            }
//            catch(System.Exception ex)
//            {
//                Logger.LogException(ex);
//            }

//            keyToFiles.Clear();
//        }

//        public void Init(string path, ResUnite ru)
//        {
//            lzma.lzma_init_alloc_root();

//            mPath = path;
//            string crtfile_path = mPath + crtfile;
//            if (!Directory.Exists(mPath))
//            {
//                Directory.CreateDirectory(mPath);

//                mWriter = new StreamWriter(crtfile_path);
//            }
//            else
//            {
//                if (File.Exists(crtfile_path))
//                {
//                    Csv.CsvReader csvReader = new Csv.CsvReader();
//                    Stream fileStream = File.Open(crtfile_path, FileMode.Open, FileAccess.Read, FileShare.Read);
//                    csvReader.LoadStream(fileStream, ',');
//                    fileStream.Close();
//                    string key;
//                    string dstfile;
//                    FileInfo fileinfo;
//                    int filesize = 0;
//                    bool isalter = false; // 是否有变化
//                    for (int i = 0; i < csvReader.getYCount(); ++i)
//                    {
//                        key = csvReader.getStr(i, 0);
//                        dstfile = mPath + KeyCheck(key);
//                        if (File.Exists(dstfile))
//                        {
//                            fileinfo = new FileInfo(dstfile);
//                            filesize = csvReader.getInt(i, 2, -1);
//                            if (filesize != fileinfo.Length || (!ru.IsExistFile(key)))
//                            {
//                                isalter = true;

//                                // 文件和保存的大小不一致,或当前不需要此文件了
//                                fileinfo.Delete(); // 删除掉
//                            }
//                            else
//                            {
//                                try
//                                {
//                                    int crc32 = int.Parse(csvReader.getStr(i, 1), System.Globalization.NumberStyles.HexNumber);
//                                    Data d = null;
//                                    if (keyToFiles.TryGetValue(key, out d))
//                                    {
//                                        // 有重复的，说明这个文件有更新了，以最后保存的数据为主
//                                        d.crc32 = crc32;
//                                        d.size = filesize;
//                                        isalter = true;
//                                    }
//                                    else
//                                    {
//                                        current_total += filesize;
//                                        keyToFiles.Add(key, new Data(dstfile, crc32, filesize));
//                                    }
//                                }
//                                catch(System.Exception ex)
//                                {
//                                    fileinfo.Delete(); // 当前保存的数值有问题
//                                    Debuger.ErrorLog("file:{0} message:{1}", key, ex.Message);
//                                    continue;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            Logger.LogError("file:{0} not find!", dstfile);
//                        }
//                    }

//                    fileinfo = null;

//                    if (isalter)
//                    {
//                        mWriter = new StreamWriter(crtfile_path);
//                        foreach (KeyValuePair<string, Data> itor in keyToFiles)
//                            mWriter.WriteLine(string.Format("\"{0}\",{1},{2}", itor.Key, itor.Value.crc32.ToString("X"), itor.Value.size));
//                        mWriter.Flush();
//                    }
//                    else
//                    {
//                        mWriter = new StreamWriter(crtfile_path, true);
//                    }
//                }
//                else
//                {
//                    mWriter = new StreamWriter(crtfile_path);
//                }
//            }

//            mThread = new Thread(new NetCore.SafeThread(ThreadUpdate).ThreadUpdate);
//            mThread.Priority = System.Threading.ThreadPriority.Highest;
//            mThread.Start();
//        }

//        Thread mThread = null;

//        public class RequestData : AsyncFile
//        {
//            public ResUnite.Res res;
//            public string key;
//            public bool isPreLoad = false; // 是否预加载资源

//            // 已经完成了
//            public void SetDone(string df)
//            {
//                dstfile = df;
//                isDone = true;
//            }
//        }

//        // 请求列表
//        List<RequestData> mRequestList = new List<RequestData>();

//        object mLock = new object(); // mRequestList列表

//        public RequestData AddRequest(ResUnite.Res res, string key, int crc32, bool isPreLoad)
//        {
//            RequestData rd = new RequestData();
//            rd.res = res;
//            rd.key = key;
//            rd.isPreLoad = isPreLoad;

//            lock (mLock)
//            {
//                mRequestList.Add(rd);
//            }

//            return rd;
//        }

//        byte[] properties = new byte[13];

//        // 得到解压之后的流大小
//        int GetLzmaSize(Stream inStream)
//        {
//            if (inStream.Read(properties, 0, 13) != 13)
//                throw (new System.Exception("input .lzma is too short"));

//            long outSize = 0;
//            for (int i = 0; i < 8; i++)
//            {
//                int v = properties[i + 5];
//                if (v < 0)
//                    throw (new System.Exception("Can't Read 1"));
//                outSize |= ((long)(byte)v) << (8 * i);
//            }

//            return (int)outSize;
//        }

//        static public string KeyCheck(string key)
//        {
//            return key;
//        }

//        public bool GetFile(ResUnite.Res res, string key, out string dstfile)
//        {
//#if ASSET_DEBUG
//            object fgp = filecache_GetFile.Begin(key);
//#endif
//            dstfile = mPath + KeyCheck(key);

//            Data d = null;
//            if (keyToFiles.TryGetValue(key, out d))
//            {
//                if (d.crc32 == res.crc32)
//                {
//                    return true; // 与之前保存的一至，不处理
//                }
//                else
//                {
//                    d.crc32 = -1;
//                    File.Delete(dstfile); // 删除之前的文件
//                }
//            }
//            else
//            {
//                d = new Data(dstfile, -1, -1);
//                keyToFiles.Add(key, d);
//            }

//            string itemfile = res.zip.data_file;
//            Archive.PartialInputStream stream = res.GetStream();
//            int offset = (int)stream.StartPosition;

//            int size = GetLzmaSize(stream);
//            stream.Close();
//            bool bhit = true;
//            if (current_total + size > totalNum)
//            {
//                dstfile = "";
//                bhit = false;
//            }
//            else
//            {
//                try
//                {
//                    mWriter.WriteLine("\"{0}\",{1},{2}", key, res.crc32.ToString("X"), size);
//                    mWriter.Flush();
//                    Directory.CreateDirectory(dstfile.Substring(0, dstfile.LastIndexOf('/')));
//                }
//                catch(System.Exception ex)
//                {
//                    Logger.LogException(ex);
//                    bhit = false;
//                }

//                if (bhit)
//                {
//                    // 再真实写入文件当中
//                    int result = lzma.DecodeFile(itemfile, offset, dstfile);
//                    if (result != 0)
//                    {
//                        bhit = false;
//                        Logger.LogError("写入文件:{0} 失败!result:{1}", dstfile, result);

//                        if (File.Exists(dstfile))
//                            File.Delete(dstfile);
//                    }
//                    else
//                    {
//                        current_total += size;
//                        stream.Close();
//                        d.crc32 = res.crc32;
//                    }
//                }
//            }

//#if ASSET_DEBUG
//            filecache_GetFile.End(fgp);
//#endif
//            if (!bhit)
//            {
//                MagicThread.StartForeground(ResourcesPack.OnNotEnoughDiskSpace);
//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }

//        public bool DeleteFile(string key)
//        {
//            lock (keyToFiles)
//            {
//                string dst = mPath + key;
//                if (File.Exists(dst))
//                {
//                    try
//                    {
//                        File.Delete(dst);
//                    }
//                    catch(System.Exception ex)
//                    {
//                        Logger.LogException(ex);
//                        return false;
//                    }

//                    Data d = null;
//                    if (keyToFiles.TryGetValue(key, out d))
//                    {
//                        keyToFiles.Remove(key);
//                        current_total -= d.size;
//                    }

//                    return true;
//                }
//            }

//            return false;
//        }

//        internal bool isDiskFull = false; // 硬盘空间是否已满

//        void ThreadUpdate()
//        {
//            List<RequestData> requestList = new List<RequestData>();
//            RequestData rd;
//            string dstfile;
//            while (true)
//            {
//                if (!isRuning)
//                {
//                    return;
//                }

//                if (mRequestList.Count == 0)
//                {
//                    Thread.Sleep(33);
//                    continue;
//                }

//                lock (mLock)
//                {
//                    List<RequestData> tmp = requestList;
//                    requestList = mRequestList;
//                    mRequestList = tmp;
//                }

//                for (int i = 0; i < requestList.Count; ++i)
//                {
//                    rd = requestList[i];
//                    while (!GetFile(rd.res, rd.key, out dstfile))
//                    {
//                        if (!rd.isPreLoad)
//                        {
//                            isDiskFull = true;
//                            while (isDiskFull)
//                                Thread.Sleep(100);// 加载失败了，再重新尝试上
//                        }
//                        else
//                        {
//                            dstfile = "";
//                            break;
//                        }
//                    }

//                    rd.SetDone(dstfile);
//                    //if (!rd.isPreLoad)
//                    //    Thread.Sleep(10);
//                }

//                requestList.Clear();
//            }
//        }
//    }
//}
