using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PackTool
{
    public class TemplatesMarco
    {
        public static void SetEnable(bool isenable, string marco, string path)
        {
            MacroDefine macroDefine = new MacroDefine();
            if (isenable)
            {
                macroDefine.Add(marco);

                // 添加
                if (!string.IsNullOrEmpty(path))
                    XTools.Utility.CopyFolder(Application.dataPath + path, Application.dataPath);
            }
            else
            {
                macroDefine.Remove(marco);

                // 得到某个目录的文件
                if (!string.IsNullOrEmpty(path))
                    XTools.Utility.RemoveFile(Application.dataPath + path, Application.dataPath);
            }

            AssetDatabase.Refresh();
            macroDefine.Save();
        }

        static void ToggleBug(string marco, string path)
        {
            MacroDefine macroDefine = new MacroDefine();
            if (macroDefine.has(marco))
            {
                // 移除
                macroDefine.Remove(marco);

                // 得到某个目录的文件
                if (!string.IsNullOrEmpty(path))
                    XTools.Utility.RemoveFile(Application.dataPath + path, Application.dataPath);
            }
            else
            {
                macroDefine.Add(marco);

                // 添加
                if (!string.IsNullOrEmpty(path))
                    XTools.Utility.CopyFolder(Application.dataPath + path, Application.dataPath);
            }

            AssetDatabase.Refresh();
            macroDefine.Save();
        }
    }
}