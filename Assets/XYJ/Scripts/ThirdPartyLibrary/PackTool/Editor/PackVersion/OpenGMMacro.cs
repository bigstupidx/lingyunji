using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace PackTool
{
    public class OpenGMMacro : TemplatesMarco
    {
        const string marco = "COM_DEBUG";
        const string path = "";

        const string MenuItemOpen = "PackTool/调试宏/GM/开启";
        const string MenuItemClose = "PackTool/调试宏/GM/取消";

        [MenuItem(MenuItemClose, true)]
        public static bool BufCannelToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return macroDefine.has(marco);
        }

        [MenuItem(MenuItemOpen, true)]
        public static bool BufOpenToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return !macroDefine.has(marco);
        }

        [MenuItem(MenuItemClose)]
        public static void BufCannel()
        {
            SetEnable(false, marco, path);
        }

        [MenuItem(MenuItemOpen)]
        public static void BufOpen()
        {
            SetEnable(true, marco, path);
        }
    }
}