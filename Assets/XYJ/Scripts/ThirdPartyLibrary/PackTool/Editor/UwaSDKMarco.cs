using System;
using UnityEditor;
using UnityEngine;

namespace PackTool
{
    public class UwaSDKMarco : TemplatesMarco
    {
        const string marco = "USER_UWASDK";
        const string path = "/../OtherSDK/uwaSDK/";

        [MenuItem("PackTool/扩展SDK/uwaSDK/取消", true)]
        public static bool BufCannelToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return macroDefine.has(marco);
        }

        [MenuItem("PackTool/扩展SDK/uwaSDK/开启", true)]
        public static bool BufOpenToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return !macroDefine.has(marco);
        }

        [MenuItem("PackTool/扩展SDK/uwaSDK/取消")]
        public static void BufCannel()
        {
            SetEnable(false, marco, path);
        }

        [MenuItem("PackTool/扩展SDK/uwaSDK/开启")]
        public static void BufOpen()
        {
            SetEnable(true, marco, path);
        }
    }
}