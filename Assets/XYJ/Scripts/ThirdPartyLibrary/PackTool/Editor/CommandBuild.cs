using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using PackTool;
using XTools;

#if USE_RESOURCESEXPORT
public class CommandBuild
{
    static void CreateLogger()
    {
        Logger.CreateInstance();
#if UNITY_EDITOR && !ASSET_DEBUG
        Logger.isSyncEditor = false;
#endif
        Debuger.SetLog(Logger.CreateLog());
    }

    [MenuItem("PackTool/BuildAutoExportTest")]
    public static void BuildAutoExportTestUI()
    {
        BuildAutoExportTest();
    }

    public static void BuildAutoExportTest()
    {
        GlobalCoroutine.StartCoroutine(ExportTest());
    }


    static IEnumerator ExportTest()
    {
        yield return 0;
        CreateLogger();
        yield return 0;
        PackTool.New.AutoExportAll aea = PackTool.New.AutoExportAll.GenTest();
        while (!aea.isDone)
            yield return 0;

        Logger.LogDebug("EditorApplication.Exit");
        EditorApplication.Exit(0);
    }
}
#endif