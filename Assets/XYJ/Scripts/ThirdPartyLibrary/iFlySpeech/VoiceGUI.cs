#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
#if USER_IFLY
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#pragma warning disable 414

public class VoiceGUI : MonoBehaviour
{
    List<string> devices = new List<string>();
    string current;

    string filepath = "";

    [SerializeField]
    string android_appid = "";
    [SerializeField]
    string ios_appid = "";

    [SerializeField]
    AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        GUITextShow.CreateInstance();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {

        }

        Logger.CreateInstance();
        PackTool.MagicThread.CreateInstance();
        Logger.LogDebug(Application.internetReachability.ToString());
#if UNITY_ANDROID && !UNITY_EDITOR
        VoiceMgr.Init(android_appid);
#elif UNITY_IPHONE && !UNITY_EDITOR
        VoiceMgr.Init(ios_appid);
#else
        VoiceMgr.Init("");
#endif
        VoiceMgr.SetCallback(OnResult);

        // 先初始化为网络
        VoiceMgr.SetParameter(VoiceMgr.ENGINE_TYPE, VoiceMgr.TYPE_CLOUD);
        VoiceMgr.SetParameter(VoiceMgr.AUDIO_FORMAT, "pcm");

        filepath = ResourcesPath.LocalTempPath + "record.pcm";
        VoiceMgr.SetParameter(VoiceMgr.ASR_AUDIO_PATH, filepath);

        lame.Lame_InitEncoder(1, 16000, 8000, 0, 0, 3);
	}

    static bool Button(string text)
    {
        return GUILayout.Button(text, GUILayout.Height(100), GUILayout.Width(300));
    }

    void OnResult(string text)
    {
        if (current == null)
            current = text; // SwitchText(text);
        else
        {
            current += text + string.Format(" l:" + VoiceMgr.current_lenght);
        }
    }

    void OnGUI()
    {
        GUILayout.Label(current);
        if (!VoiceMgr.CheckServiceInstalled())
        {
            if (Button("使用本地服务"))
            {
                Application.OpenURL(VoiceMgr.GetServiceUrl());
                return;
            }
        }
        else
        {
            if (Button("使用本地服务"))
            {
                VoiceMgr.SetParameter(VoiceMgr.PARAMS, null);
                VoiceMgr.SetParameter(VoiceMgr.ENGINE_TYPE, VoiceMgr.TYPE_LOCAL);
            }

            if (Button("使用网络服务"))
            {
                VoiceMgr.SetParameter(VoiceMgr.PARAMS, null);
                VoiceMgr.SetParameter(VoiceMgr.ENGINE_TYPE, VoiceMgr.TYPE_CLOUD);
            }
        }

        if (VoiceMgr.isListening)
        {
            GUILayout.Label("正在录音当中!");

            if (Button("停止录音"))
            {
                VoiceMgr.Stop();
            }

            if (Button("取消录音"))
            {
                VoiceMgr.Cancel();
            }

            return;
        }

        if (Button("开始识别"))
        {
            VoiceMgr.Start();
            current = "";
        }

        if (Button("压缩音频"))
        {
            lame.Lame_EncodeFile(filepath, filepath.Substring(0, filepath.LastIndexOf('.')) + ".mp3");
        }

        if (Button("播放mp3"))
        {
            Mp3Play.Play(filepath.Substring(0, filepath.LastIndexOf('.')) + ".mp3");
        }

        if (Button(audioSource.isPlaying ? "暂停" : "播放"))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            else
            {
                Logger.LogDebug("audioSource.volume:{0}", audioSource.volume);
                audioSource.volume = 0.2f;
                audioSource.Play();
            }
        }
    }

    protected void OnApplicationQuit()
    {
        Mp3Play.Stop();
    }
}
#endif
