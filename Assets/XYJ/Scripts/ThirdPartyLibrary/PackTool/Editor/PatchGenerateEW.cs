#if USE_RESOURCESEXPORT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XTools;
using UnityEditor;
using UnityEngine;

namespace PackTool.Patch
{
    public partial class PatchGenerate : EditorWindow
    {
        [MenuItem("PackTool/目录对比", false, 9)]
        static public void OpenPackEditorWindow()
        {
            EditorWindow.GetWindow<PatchGenerate>(false, "PatchGenerate", true);
        }

        public string srcfilepath = "";
        public string dstfilepath = "";
        public string pathfilepath = "";

        static void ShowFilePath(ref string filepath, string key)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(key))
            {
                filepath = EditorUtility.OpenFolderPanel(key,
                    string.IsNullOrEmpty(filepath) ? filepath : ResourcesPath.LocalBasePath + "Data", "");
            }

            EditorGUILayout.LabelField(key, filepath);
            EditorGUILayout.EndHorizontal();
        }

        void OnGUI()
        {
            ShowFilePath(ref srcfilepath, "基础资源");//不导出中文
            ShowFilePath(ref dstfilepath, "目标资源");//不导出中文
            ShowFilePath(ref pathfilepath, "补丁目录");//不导出中文

            if (GUILayout.Button("生成补丁", GUILayout.Height(50f)))
            {
                PatchGenerate.Data data = new PatchGenerate.Data();
                data.from = srcfilepath;
                data.to = dstfilepath;
                data.patchprefix = "Data";
                data.password = "";

                // 补丁的名字
                data.patchname = (string.IsNullOrEmpty(pathfilepath) ? ResourcesPath.LocalBasePath : pathfilepath + "/");
                data.patchname += string.Format("{0}-{1}{2}", Path.GetFileName(srcfilepath), Path.GetFileName(dstfilepath), AssetZip.patchsuffix);
                bool isAlter = PatchGenerate.Generate(data,
                    (List<string> cf) =>
                    {
                        if (cf.Count == 1 && cf[0].EndsWith(ResourceConfig.filename))
                            return true;
                        return false;
                    });

                if (!isAlter)
                {
                    Debug.Log("两份目录完全一致!");
                }
                else
                {
                    Debug.Log("生成补丁文件成功!" + data.patchname);
                }
            }
        }
    }
}
#endif