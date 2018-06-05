using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace PackTool
{
    public class AssetDebugMacro : TemplatesMarco
    {
        const string marco = "ASSET_DEBUG";
        const string path = "";

        const string MenuItemOpen = "PackTool/调试宏/资源调试/开启";
        const string MenuItemClose = "PackTool/调试宏/资源调试/取消";

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