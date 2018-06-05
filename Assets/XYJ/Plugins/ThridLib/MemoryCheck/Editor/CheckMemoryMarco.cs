using System;
using UnityEditor;
using UnityEngine;

namespace PackTool
{
    public class CheckMemoryMarco : TemplatesMarco
    {
        const string marco = "MEMORY_CHECK";
        const string path = "/../OtherSDK/MemoryCheck/";

        [MenuItem("PackTool/调试宏/内存/关闭", true)]
        public static bool BufCannelToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return macroDefine.has(marco);
        }

        [MenuItem("PackTool/调试宏/内存/开启", true)]
        public static bool BufOpenToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return !macroDefine.has(marco);
        }

        [MenuItem("PackTool/调试宏/内存/关闭")]
        public static void Channel()
        {
            SetEnable(false, marco, path);
        }

        [MenuItem("PackTool/调试宏/内存/开启")]
        public static void Open()
        {
            SetEnable(true, marco, path);
        }
    }
}