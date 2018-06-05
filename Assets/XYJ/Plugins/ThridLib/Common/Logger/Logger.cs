//#define SYNC_LOG // 同步日志，立即写下文件

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

public partial class XYJLogger : Singleton<XYJLogger>
{
    string Filename = "Logger{0}.log";
    public bool IsAppend = false;

    public static string GetNowTime()
    {
        System.DateTime time = System.DateTime.Now;
        return string.Format("{0}-{1}-{2}-{3}-{4}-{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
    }

    static public string RootPath
    {
        get
        {
            return
#if (UNITY_ANDROID || UNITY_IPHONE) && (!UNITY_EDITOR)
                Application.temporaryCachePath + "/";
#else
                Application.dataPath + "/../";
#endif
        }
    }

    string FullPath 
    {
        get
        {
            return RootPath + 
#if UNITY_EDITOR
        string.Format(Filename, "");
#else
        string.Format(Filename, GetNowTime());
#endif
        }
    }

    public string logfile { get; protected set; }

    public static void RemoveLogFile()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        try
        {
            string[] files = Directory.GetFiles(RootPath, "*.log");
            foreach (string file in files)
                File.Delete(file);
        }
        catch(System.Exception )
        {

        }
#endif
    }

    public XYJLogger()
    {
        logfile = FullPath;

        if (IsAppend == false && File.Exists(logfile))
            File.Delete(logfile);

        try
        {
            m_streamWriter = new StreamWriter(logfile, IsAppend, System.Text.Encoding.UTF8);
            m_streamWriter.WriteLine(System.DateTime.Now);
            m_streamWriter.WriteLine("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
            m_streamWriter.WriteLine("+                                                                             +");
            m_streamWriter.WriteLine("+                         Logger singleton created.                           +");
            m_streamWriter.WriteLine("+                                                     	                    +");
            m_streamWriter.WriteLine("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
            m_streamWriter.Flush();

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR && !USE_TESTIN
            Debuger.SetLog(XYJLogger.CreateLog());
#elif !ASSET_DEBUG &&  !UNITY_EDITOR
            Debuger.SetLog(new Logger.RunTime());
#endif

#if SYNC_LOG
            Application.logMessageReceived += LogCallback;
#else
            m_thread = new Thread(UpdateThread);
            m_thread.Start();
            Application.logMessageReceived += LogCallback;
#endif
        }
        catch (System.Exception /*ex*/)
        {
            IsEnd = true;
            Release();
        }
    }

#if UNITY_EDITOR
    static bool isSelf = false; // 是否自己调用系统的日志
#endif

    // 系统日志回调
    static void LogCallback(string condition, string stackTrace, LogType type)
    {
#if UNITY_EDITOR
        if (isSelf)
            return;
#endif
        //if (type == LogType.Log || type == LogType.Warning)
            //return;
        XYJLogger.Instance.Log(type, "condition:{0}, stackTrace:{1}", condition, stackTrace);
    }

    static void LogInfo(LogType level, string format, params object[] objs)
    {
        if (XYJLogger.IsCreate == false)
            return;

        XYJLogger.Instance.Log(level, format, objs);
    }

    [Conditional("COM_DEBUG")]
    public static void LogDebug(string format, params object[] objs)
    {
        LogInfo(LogType.Log, format, objs);
    }

    //[Conditional("COM_DEBUG")]
    //部分代码会使用这个来调试输出，但并不是报错
    public static void LogWarning(string format, params object[] objs)
    {
        LogInfo(LogType.Warning, format, objs);
    }

    public static void LogError(string format, params object[] objs)
    {
        LogInfo(LogType.Error, format, objs);
    }

    public static void LogException(Exception ex)
    {
        LogInfo(LogType.Exception, "Message:{0} StackTrace:{1} Source:{2}", ex.Message, ex.StackTrace, ex.Source);
    }

    public void Release()
    {
        Application.logMessageReceived -= LogCallback;

        if (m_thread != null)
        {
            while (IsRun)
            {
                if (IsEnd == true && m_writeList.Count == 0)
                {
                    IsRun = false;
                }
                else
                {
                    if (IsThreadEnd == true)
                    {
                        m_thread = null;
                        break;
                    }
                }

                Thread.Sleep(200);
            }
        }

        try
        {
            if (m_streamWriter != null)
            {
                m_streamWriter.Close();
                m_streamWriter = null;
            }
        }
        catch(System.Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }

        if (m_poolList != null)
            m_poolList.Clear();

        if (m_writeList != null)
            m_writeList.Clear();

        ReleaseInstance();
    }

    bool IsEnd = false;
    bool IsRun = true;
    bool IsThreadEnd = false;
    int LifeCount = 0;

    void UpdateThread()
    {
        List<LogData> ldList = new List<LogData>();

        while (IsRun)
        {
            LifeCount++;
            if (LifeCount >= 200)
            {
                Log(LogType.Log, "UpdateThread Life!");
                LifeCount = 0;
            }

            if (m_writeList.Count == 0)
            {
                IsEnd = true;
                Thread.Sleep(200);
                continue;
            }

            IsEnd = false;
            {
                lock (m_write_lock)
                {
                    List<LogData> tmp = ldList;
                    ldList = m_writeList;
                    m_writeList = tmp;
                }
            }

            LogData ld;
            for (int i = 0; i < ldList.Count; ++i)
            {
                ld = ldList[i];
                try
                {
                    m_streamWriter.Write("tid:{3} {0}:{1} {2}\r\n", ld.level, ld.Time, ld.text, ld.threadid);
                }
                catch(System.Exception /*ex*/)
                {
                    PackTool.MagicThread.StartForeground(() => { Release(); });
                    IsThreadEnd = true;
                    return;
                }

                ld.text = string.Empty;
            }

            lock (m_poolList)
            {
                m_poolList.AddRange(ldList);
            }

            ldList.Clear();
            m_streamWriter.Flush();
            Thread.Sleep(200);
        }
        IsThreadEnd = true;
    }

    StreamWriter m_streamWriter;

    class LogData
    {
        public LogType level;
        public string text;
        public DateTime time;
        public int threadid;

        public string Time
        {
            get
            {
                return string.Format("{0}-{1}-{2},{3}:{4}:{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
            }
        }
    }

#if UNITY_EDITOR && !ASSET_DEBUG
    public static bool isSyncEditor = true;
#endif

    string Log(LogType level, string format, params object[] objs)
    {
        string text = (objs == null || objs.Length == 0) ? format : string.Format(format, objs);

#if UNITY_EDITOR && !ASSET_DEBUG
        if (isSyncEditor)
        {
            isSelf = true;
            Debuger.UnityLog.Log(level, text);
            isSelf = false;
        }
#endif

#if SYNC_LOG
        ConsoleSelf.AddText(text);
        m_streamWriter.WriteLine("tid:{3} {0}:{1} {2}", level, System.DateTime.Now, text, Thread.CurrentThread.ManagedThreadId);
        m_streamWriter.Flush();
        return text;
#else
        LogData ld = GetEmpty();
        ld.level = level;
        ld.text = text;
        ld.time = System.DateTime.Now;
        ld.threadid = Thread.CurrentThread.ManagedThreadId;

        lock (m_write_lock)
        {
            m_writeList.Add(ld);
        }

        ConsoleSelf.AddText("time:{0} text:{1}", ld.time, ld.text);

        return ld.text;
#endif
    }

    List<LogData> m_poolList = new List<LogData>();
    List<LogData> m_writeList = new List<LogData>();
    object m_write_lock = new object(); // 锁

    LogData GetEmpty()
    {
        lock (m_poolList)
        {
            if (m_poolList.Count != 0)
            {
                LogData ld = m_poolList[m_poolList.Count - 1];
                m_poolList.RemoveAt(m_poolList.Count - 1);
                return ld;
            }
        }

        return new LogData();
    }

    Thread m_thread;
}