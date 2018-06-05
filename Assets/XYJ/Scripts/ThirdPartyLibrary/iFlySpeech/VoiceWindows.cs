#if USER_IFLY
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

public partial class Voice
{
    static System.Diagnostics.Process s_process = null;
    static Mutex s_stopMutex;

    static public void Init(string appid)
    {
#if UNITY_EDITOR
        Debug.LogFormat("data:{0}", ResourcesPath.dataPath);
#endif
    }

    public static bool isListening()
    {
        return s_process == null ? false : true;
    }

    public static bool IsPlaying()
    {
        Debug.LogError("Only support Anroid and ios Platform");
        return false;
    }

    public static bool Play(string filepath)
    {
        Debug.LogError("Only support Anroid and ios Platform");
        return false;
    }

    public static bool StopPlay()
    {
        Debug.LogError("Only support Anroid and ios Platform");
        return false;
    }

    public static int Start()
    {
        if (s_process != null || s_stop_thread_running)
            return -1;

        PackTool.MagicThread.StartBackground(BeginThread());

        return 0;
    }

    static string ReadFile(string file)
    {
        if (!System.IO.File.Exists(file))
            return null;

        string text = System.IO.File.ReadAllText(file);
        System.IO.File.Delete(file);
        return text;
    }

    // 识别结束之后，当前的语音长度
    public static int current_lenght { get; private set; }

    static bool s_isStop = false;
    static bool s_stop_thread_running = false;

    static void StopThread(object p)
    {
        string name = p as string;
        s_stopMutex = new Mutex(true, name);
        s_stopMutex.WaitOne();
        s_stop_thread_running = true;
        Debug.LogFormat("Mutex:{0}", name);

        while (true)
        {
            if (s_isStop || s_process == null)
                break;

            Thread.Sleep(100);
        }

        s_stopMutex.ReleaseMutex();
        s_stop_thread_running = false;
        s_isStop = false;
        Debug.LogFormat("s_stop_thread_running false!");
    }

    static string pc_exe_path
    {
        get
        {
#if UNITY_EDITOR
            return string.Format("{0}/../OtherSDK/ifly/pcexe/", ResourcesPath.dataPath); ;
#else
            return ResourcesPath.streamingAssetsPath;
#endif
        }
    }

    static IEnumerator BeginThread()
    {
        s_process = new System.Diagnostics.Process();
        string name = System.Guid.NewGuid().ToString();
        PackTool.MagicThread.StartBackground(StopThread, name);

        while(!s_stop_thread_running)
            Thread.Sleep(10);

//#if !UNITY_EDITOR
        s_process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
//#endif
        s_process.StartInfo.FileName = pc_exe_path + "/iat_record.exe";

        System.IO.Directory.CreateDirectory(ResourcesPath.LocalTempPath);
        string pcm_file = ResourcesPath.LocalTempPath + "record.pcm";
        //string mp3_file = ResourcesPath.LocalTempPath + "record.mp3";
        string result_file = ResourcesPath.LocalTempPath + "record_result.txt";

        string cmd = string.Format("{0} {1} {2}", pcm_file, result_file, name);
        Debug.Log(cmd);
        s_process.StartInfo.Arguments = cmd;
        try
        {
            s_process.Start();
        }
        catch(System.Exception ex)
        {
            Debug.LogException(ex);
        }

        System.Threading.Thread.Sleep(1000);
        s_process.WaitForExit();
        s_process = null;

        string result = ReadFile(result_file);
        string error = null;
        string timel = null;
        int lenght = 0;
        if (!string.IsNullOrEmpty(result))
        {
            JSONObject json = new JSONObject(result);

            result = json.getString("result");
            error = json.getString("error");
            timel = json.getString("time");

            if (string.IsNullOrEmpty(error))
                error = null;

            if (!string.IsNullOrEmpty(timel))
            {
                int.TryParse(timel, out lenght);
            }
        }

        yield return new PackTool.ForegroundTask(); // 到主线程去执行

        current_lenght = lenght;
        if (error == null)
        {
            VoiceMgr.instance.SendMessage("onResultEnd", result == null ? string.Empty : result);
            Debug.LogFormat("录音结果:{0}", result);
        }
        else
        {
            VoiceMgr.instance.SendMessage("onError", error == null ? string.Empty : error);
            Debug.LogErrorFormat("录音报错:{0}", error);
        }

        s_process = null;
    }

    public static void Stop()
    {
        if (s_stop_thread_running)
        {
            Debug.LogFormat("s_isStop = true");
            s_isStop = true;
        }
    }

    public static void Cancel()
    {
        Stop();
    }

    public static void SetParameter(string var1, string var2)
    {
        
    }

    public static bool CheckServiceInstalled()
    {
        return false;
    }

    public static string GetServiceUrl()
    {
        return "";
    }
}
#endif
#endif