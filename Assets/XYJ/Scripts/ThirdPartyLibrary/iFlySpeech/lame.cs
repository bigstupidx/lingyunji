#if USER_IFLY
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.IO;

public class lame
{
#if !UNITY_EDITOR
#if UNITY_IPHONE
        // 初始化lame解码器
    [DllImport("__Internal")]
    public static extern int Lame_InitEncoder(int in_num_channels, int in_samplerate, int out_samplerate, int in_brate, int in_mode, int in_quality);

     // 设置音频的采样率
    [DllImport("__Internal")]
    public static extern void Lame_EncodeFile(string source_path, string target_path);
#else
    // 初始化lame解码器
    [DllImport("map3lame")]
    public static extern int Lame_InitEncoder(int in_num_channels, int in_samplerate, int out_samplerate, int in_brate, int in_mode, int in_quality);

     // 设置音频的采样率
    [DllImport("map3lame")]
    public static extern void Lame_EncodeFile(string source_path, string target_path);
#endif
#else
    public static int Lame_InitEncoder(int in_num_channels, int in_samplerate, int out_samplerate, int in_brate, int in_mode, int in_quality)
    {
        return -1;
    }

    public static void Lame_EncodeFile(string source_path, string target_path)
    {

    }
#endif
}
#endif