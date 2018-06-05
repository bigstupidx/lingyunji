#if USER_IFLY
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using NAudio.Wave;
using System.IO;
#endif

public class Mp3Play
{
#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
	public static extern bool Mp3Play_Pause();

    public static bool Pause()
    {
        return Mp3Play_Pause();
    }

    [DllImport("__Internal")]
	public static extern bool Mp3Play_PlayByPause();
    public static bool PlayByPause()
    {
		return Mp3Play_PlayByPause();
    }

    [DllImport("__Internal")]
    public static extern bool Mp3Play_IsPlaying();
    public static bool IsPlaying()
    {
        return Mp3Play_IsPlaying();
    }

    [DllImport("__Internal")]
    public static extern bool Mp3Play_Play(string filepath);

    public static bool Play(string filepath)
    {
        return Mp3Play_Play(filepath);
    }

    [DllImport("__Internal")]
    public static extern bool Mp3Play_Stop();
    public static bool Stop()
    {
        return Mp3Play_Stop();
    }
#elif UNITY_ANDROID && !UNITY_EDITOR
    static AndroidJavaClass mp3Play_ = null;

    static AndroidJavaClass mp3Play
    {
        get
        {
            if (mp3Play_ == null)
                mp3Play_ = new AndroidJavaClass("com.wxb.speech.PlayMp3");

            return mp3Play_;
        }
    }

    public static bool Pause()
    {
        return mp3Play.CallStatic<bool>("Pause");
    }

    public static bool PlayByPause()
    {
        return mp3Play.CallStatic<bool>("PlayByPause");
    }

    public static bool IsPlaying()
    {
        return mp3Play.CallStatic<bool>("IsPlaying");
    }

    public static bool setVolume(float leftVolume, float rightVolume)
    {
        return mp3Play.CallStatic<bool>("setVolume", leftVolume, rightVolume);
    }

    public static bool Play(string filepath)
    {
        if (!System.IO.File.Exists(filepath))
            return false;

        setVolume(3f, 3f);
        return mp3Play.CallStatic<bool>("Play", filepath);
    }

    public static bool Stop()
    {
        return mp3Play.CallStatic<bool>("Stop");
    }
#else
    static IWavePlayer iwavePlayer;
    static WaveChannel32 mp3fileStream;
    static FileStream fileStream;

    public static void Stop()
    {
        if (iwavePlayer != null)
        {
            iwavePlayer.Stop();
            iwavePlayer.Dispose();
        }
        iwavePlayer = null;
        if (mp3fileStream != null)
        {
            mp3fileStream.Close();
            mp3fileStream.Dispose();
        }
        mp3fileStream = null;

        if (fileStream != null)
        {
            fileStream.Close();
        }
        fileStream = null;
    }

    public static bool Play(string filepath)
    {
        Stop();

        if (!File.Exists(filepath))
            return false;

        fileStream = new FileStream(filepath, FileMode.Open);
        mp3fileStream = new WaveChannel32(new Mp3FileReader(fileStream));

        iwavePlayer = new WaveOut();
        iwavePlayer.Init(mp3fileStream);
        iwavePlayer.Play();
        return true;
    }

    static public bool Pause()
    {
        if (iwavePlayer == null || iwavePlayer.PlaybackState != PlaybackState.Playing)
            return false;

        iwavePlayer.Pause();
        return true;
    }

    public static bool IsPlaying()
    {
        if (iwavePlayer == null)
            return false;

        return iwavePlayer.PlaybackState == PlaybackState.Playing;
    }
#endif
}
#endif