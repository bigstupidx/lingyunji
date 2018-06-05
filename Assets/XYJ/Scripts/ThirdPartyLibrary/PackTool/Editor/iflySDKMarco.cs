using System;
using UnityEditor;
using UnityEngine;

namespace PackTool
{
    // 讯飞语音识别SDK
    public class iflySDKMarco : TemplatesMarco
    {
        const string marco = "USER_IFLY";
        const string path = "/../OtherSDK/ifly/Project/";

        [MenuItem("PackTool/扩展SDK/讯飞语音/取消", true)]
        public static bool BufCannelToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return macroDefine.has(marco);
        }

        [MenuItem("PackTool/扩展SDK/讯飞语音/开启", true)]
        public static bool BufOpenToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return !macroDefine.has(marco);
        }

        [MenuItem("PackTool/扩展SDK/讯飞语音/取消")]
        public static void BufCannel()
        {
            SetEnable(false, marco, path);
        }

        [MenuItem("PackTool/扩展SDK/讯飞语音/开启")]
        public static void BufOpen()
        {
            SetEnable(true, marco, path);
        }
    }
}