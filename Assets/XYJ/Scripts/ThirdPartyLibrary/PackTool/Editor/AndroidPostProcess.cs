#if UNITY_EDITOR && UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Xml;
using System.IO;

public static class AndroidPostProcess
{
	[PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        pathToBuiltProject = pathToBuiltProject.Replace('\\', '/');

        if (pathToBuiltProject.EndsWith(".apk"))
            return;

#if USE_BUGLY
        // 拷贝文件到工程目录
        string srcpath = string.Format("{0}/{1}/src/{2}", pathToBuiltProject, PlayerSettings.productName, PlayerSettings.bundleIdentifier.Replace('.', '/'));
        string pluginspath = string.Format("{0}/{1}/libs/", pathToBuiltProject, PlayerSettings.productName);
        PackTool.Util.CopyFolder(Application.dataPath + "/../othersdk/Bugly/src/", srcpath);
        PackTool.Util.CopyFolder(Application.dataPath + "/../othersdk/Bugly/Plugins/Android/", pluginspath);
#endif
    }
}
#endif
