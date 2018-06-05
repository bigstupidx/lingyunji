using System.IO;

public class VoiceMisc
{
    static string filepath;
    static string mp3filepath;

    static public void Init()
    {
#if USER_IFLY
#if UNITY_ANDROID && !UNITY_EDITOR
        VoiceMgr.Init("5677916b");
#elif UNITY_IPHONE && !UNITY_EDITOR
        VoiceMgr.Init("57e32620");
#else
        VoiceMgr.Init("");
#endif
        VoiceMgr.SetCallback(OnResult);
        VoiceMgr.SetError(OnError);

        // 始化为网络
        VoiceMgr.SetParameter(VoiceMgr.ENGINE_TYPE, VoiceMgr.TYPE_CLOUD);
        VoiceMgr.SetParameter(VoiceMgr.AUDIO_FORMAT, "pcm");

        VoiceMgr.SetParameter(VoiceMgr.KEY_SPEECH_TIMEOUT, "60000");
        VoiceMgr.SetParameter(VoiceMgr.VAD_ENABLE, "0");
        //VoiceMgr.SetParameter(VoiceMgr.VAD_BOS, "60000");
        VoiceMgr.SetParameter(VoiceMgr.VAD_EOS, "60000");

        filepath = ResourcesPath.LocalTempPath + "record.pcm";
        mp3filepath = ResourcesPath.LocalTempPath + "record.mp3";
        VoiceMgr.SetParameter(VoiceMgr.ASR_AUDIO_PATH, filepath);

        lame.Lame_InitEncoder(1, 16000, 8000, 0, 0, 0);
#endif
    }

    static void OnResult(string text)
    {
#if USER_IFLY
        if (sResultFun != null)
        {
            lame.Lame_EncodeFile(filepath, mp3filepath);

            ResultData rd = new ResultData();
            rd.error = null;
            rd.lenght = VoiceMgr.current_lenght;
            rd.mp3file = mp3filepath;
            rd.text = text;

            Logger.LogDebug("VoiceMisc.OnResult:{0} l:{1}", text, rd.lenght);

            sResultFun(rd);
        }
#endif

    }

    static void OnError(string text)
    {
#if USER_IFLY
        JSONObject json = new JSONObject(text);
        if (sResultFun != null)
        {
            ResultData rd = new ResultData();

            int error = json.getInt("code");
            if (error == 10118)
            {
                lame.Lame_EncodeFile(filepath, mp3filepath);

                rd.error = null;
                rd.lenght = VoiceMgr.current_lenght;
                rd.mp3file = mp3filepath;
                rd.text = "";
            }
            else
            {
                rd.error = json.getString("text");
                rd.lenght = 0;
                rd.mp3file = null;
                rd.text = null;
            }

            sResultFun(rd);
        }
#endif
    }

    // 结果
    public struct ResultData
    {
        public string error;
        public string text;
        public string mp3file;
        public int lenght;
    }

    // 识别之后的回调
    // error 错误信息，要先检测是否为空，为空则正确
    // text 返回的识别的文本
    // filename 音频文件路径
    public delegate void OnResultCall(ResultData rd);

    static OnResultCall sResultFun;

    // 开始录音,录音完成之后的回调
    static public bool Start(OnResultCall onresult)
    {
#if USER_IFLY
        if (VoiceMgr.isListening)
            return false; // 当前正在录音当中

        sResultFun = onresult;
        VoiceMgr.Start();
        return true;
#else
        return false;
#endif
    }

    public static bool isListening
    {
        get
        {
#if USER_IFLY
            return VoiceMgr.isListening;
#else
            return false;
#endif
        }
    }

    static public bool Stop()
    {
#if USER_IFLY
        if (!VoiceMgr.isListening)
            return false;

        VoiceMgr.Stop();
        return true;
#else
        return false;
#endif
    }

    static public bool PlayMp3(string filepath)
    {
        if (!File.Exists(filepath))
            return false;
#if USER_IFLY
        return Mp3Play.Play(filepath);
#else
        return false;
#endif
    }

    static public void StopMp3()
    {
#if USER_IFLY
        Mp3Play.Stop();
#else
#endif
    }

    static public bool isPlaying
    {
        get
        {
#if USER_IFLY
            return Mp3Play.IsPlaying();
#else
            return false;
#endif  
        }
    }
}