#if USER_IFLY
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public partial class Voice
{
#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    extern static bool iflyInterface_isListening();

    static public bool isListening()
    {
        return iflyInterface_isListening();
    }

    [DllImport("__Internal")]
    extern static void iflyInterface_Init(string appid);
    static public void Init(string appid)
    {
         iflyInterface_Init(appid);
    }

    [DllImport("__Internal")]
    extern static void iflyInterface_SetParameter(string var1, string var2);
    static public void SetParameter(string var1, string var2)
    {
        iflyInterface_SetParameter(var1, var2);
    }

    [DllImport("__Internal")]
    extern static bool iflyInterface_CheckServiceInstalled();
    public static bool CheckServiceInstalled()
    {
        return iflyInterface_CheckServiceInstalled();
    }

    [DllImport("__Internal")]
    extern static string iflyInterface_GetServiceUrl();
    public static string GetServiceUrl()
    {
        return iflyInterface_GetServiceUrl();
    }

    [DllImport("__Internal")]
    extern static int iflyInterface_Start();
    public static int Start()
    {
        return iflyInterface_Start();
    }

    [DllImport("__Internal")]
    extern static void iflyInterface_Stop();
    public static void Stop()
    {
        iflyInterface_Stop();
    }

    [DllImport("__Internal")]
    extern static void iflyInterface_Cancel();
    public static void Cancel()
    {
        iflyInterface_Cancel();
    }

#elif UNITY_ANDROID && !UNITY_EDITOR
    static AndroidJavaClass SpeechRecognizer = null;
    static AndroidJavaClass PCMPlay = null;

    public static bool isListening()
    {
        return SpeechRecognizer == null ? false : SpeechRecognizer.CallStatic<bool>("isListening");
    }

    public static void Init(string appid)
    {
        SpeechRecognizer = new AndroidJavaClass("com.wxb.speech.SpeechUtitly");
        SpeechRecognizer.CallStatic("Init", appid);

        PCMPlay = new AndroidJavaClass("com.wxb.speech.PlayPCM");
    }

    public static bool IsPlaying()
    {
        return PCMPlay.CallStatic<bool>("IsPlaying");
    }

    public static bool Play(string filepath)
    {
        return PCMPlay.CallStatic<bool>("Play", filepath);
    }

    public static bool StopPlay()
    {
        return PCMPlay.CallStatic<bool>("Stop");
    }

    public static int Start()
    {
        SetParameter("", "");
        return SpeechRecognizer.CallStatic<int>("Start");
    }

    public static void Stop()
    {
        SpeechRecognizer.CallStatic("Stop");
    }

    public static void Cancel()
    {
        SpeechRecognizer.CallStatic("Cancel");
    }

    public static void SetParameter(string var1, string var2)
    {
        SpeechRecognizer.CallStatic("SetParameter", var1, var2);
    }

    public static bool CheckServiceInstalled()
    {
        return SpeechRecognizer.CallStatic<bool>("CheckServiceInstalled");
    }

    public static string GetServiceUrl()
    {
        return SpeechRecognizer.CallStatic<string>("GetServiceUrl");
    }
#endif
}
#endif