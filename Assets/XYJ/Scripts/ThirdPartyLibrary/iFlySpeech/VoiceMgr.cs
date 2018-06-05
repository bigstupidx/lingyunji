#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
#if USER_IFLY
using UnityEngine;
using System.Collections;

#pragma warning disable 414

public class VoiceMgr : MonoBehaviour
{
    static public VoiceMgr instance { get; private set; }

    public static string PARAMS = "params";

    public const string LANGUAGE = "language"; // 语言
    public const string en_us = "en_us"; // 英文
    public const string zh_cn = "zh_cn"; // 中文
    public const string mandarin = "mandarin "; // 普通话

    public const string ENGINE_TYPE = "engine_type"; // 引擎类型
    public const string TYPE_LOCAL = "local"; // 本地方式
    public const string TYPE_CLOUD = "cloud"; // 服务器方式
    public const string ACCENT = "accent"; // 方言

    public const string AUDIO_FORMAT = "audio_format"; // 流保存的格式，有pcm,wav两种
    public const string ASR_AUDIO_PATH = "asr_audio_path"; // 保存的路径

    public const string VAD_BOS = "vad_bos"; // 前端点超时 开始录入音频后，音频前面部分最长静音时长。
    public const string VAD_EOS = "vad_eos"; // 后端点超时 是否必须设置：否 开始录入音频后，音频后面部分最长静音时长。

    public const string VAD_ENABLE = "vad_enable"; // 是否允许VAD VAD（Voice Activity Detection,静音抑制）是用于在音频传输时，通过控制音频 的静音时长，减少在网络传输没有意义的数据，以减少网络带宽使用等。

    public const string KEY_SPEECH_TIMEOUT = "speech_timeout"; // 设置录取音频的最长时间。在听写、识别、语音语义和声纹等需要录入音频的业务 下，在录音模式时，录取音频的最长时间。当录音超过这个时间时，SDK会自动结束 录音，并等待结果返回。当此参数值设为-1时，表示超时时间为无限，仅在评测和转 写时生效，在其他业务中，服务器最长仅支持60秒的音频，超过的音频将被忽略。 关于转写支持的最长时长，请参考转写类SpeechTranscripter说明。

    // 当前录音的长度
    public static int current_lenght
    {
        get
        {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR)
            return Voice.current_lenght;
#else
            return EndTime - StartTime;
#endif
        }
    }

    void Awake()
    {
        instance = this;
    }

    // 初始化
    static public void Init(string appid)
    {
        GameObject obj = new GameObject("SpeechRecognizer");
        DontDestroyOnLoad(obj);
        Microphone.IsRecording(null);
        obj.AddComponent<VoiceMgr>();

        Voice.Init(appid);
    }

    // 
    static public bool isListening
    {
        get
        {
            return Voice.isListening();
        }
    }

    public static void SetParameter(string var1, string var2)
    {
        SalfCall(() => 
        {
            Voice.SetParameter(var1, var2);
        });
    }

    public static bool CheckServiceInstalled()
    {
        bool value = false;
        SalfCall(() => 
        {
            value = Voice.CheckServiceInstalled();
        });

        return value;
    }

    static public string GetServiceUrl()
    {
        string url = string.Empty;
        SalfCall(() =>
        {
            url = Voice.GetServiceUrl();
        });

        return url;
    }

    static void SalfCall(System.Action action)
    {
        try
        {
            action();
        }
        catch(System.Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    static public int Start()
    {
        result_text = string.Empty;
        Logger.LogDebug("SpeechRecognizer:Start");
        int code = 0;
        SalfCall(() => 
        {
            code = Voice.Start();
        });

        Logger.LogDebug("SpeechRecognizer:Start({0})", code);
        return code;
    }

    static public void Stop()
    {
        Logger.LogDebug("SpeechRecognizer:Stop");
        SalfCall(() =>
        {
            Voice.Stop();
        });
    }

    static public void Cancel()
    {
        Logger.LogDebug("SpeechRecognizer:Cancel");
        SalfCall(() =>
        {
            Voice.Cancel();
        });
    }

    void InitEnd(string code)
    {
        Logger.LogDebug("SpeechRecognizer:InitEnd:" + code);
    }

    static int StartTime = 0;
    static int EndTime = 0;

    void onBeginOfSpeech(string text)
    {
        result_text = string.Empty;
        StartTime = (int)(Time.realtimeSinceStartup * 1000);
        Logger.LogDebug("onBeginOfSpeech");
    }

    void onError(string text)
    {
        Logger.LogDebug("onError:{0}", text);
        if (OnError != null)
            OnError(text);
    }

    void onEndOfSpeech(string text)
    {
        EndTime = (int)(Time.realtimeSinceStartup * 1000);
        Logger.LogDebug("onEndOfSpeech");
    }

    void onVolumeChanged(string volume)
    {
        Logger.LogDebug("onVolumeChanged:" + volume);
    }

    void OnInit(string code)
    {
        Logger.LogDebug("OnInit:" + code);
    }

    void LateUpdate()
    {
        bool isPause = false;
        if (Mp3Play.IsPlaying() || Voice.isListening())
            isPause = true;

        if (AudioListener.pause != isPause)
        {
            AudioListener.pause = isPause;
        }
    }

    static System.Action<string> callback = null;
    static System.Action<string> OnError = null;

    static public void SetCallback(System.Action<string> action)
    {
        callback = action;
    }

    static public void SetError(System.Action<string> error)
    {
        OnError = error;
    }

    static string result_text;

    void onResult(string text)
    {
        result_text += text;
    }

    void onResultEnd(string text)
    {
        result_text += text;
        Debuger.DebugLog("识别结果:{0} lenght:{1}", result_text, current_lenght);
        if (callback != null)
        {
            callback(result_text);
        }

        result_text = string.Empty;
    }
}
#endif
