#if USE_ABL
//#define TESTRES
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class FileAtlerCheck
{
    static JSONObject json;

    static string FilePath
    {
        get { return PackTool.AssetsExport.TempPath + "unity.json"; }
    }

    public static void Begin()
    {
        string filepath = FilePath;
        if (File.Exists(filepath))
        {
            json = new JSONObject(File.ReadAllText(filepath));
        }
        else
        {
            json = new JSONObject();
        }
    }

    public static void End()
    {
        string filepath = FilePath;
        Directory.CreateDirectory(filepath.Substring(0, filepath.LastIndexOf('/')));
        File.WriteAllText(filepath, json.ToString());

        json = null;
    }

    public static bool isNeedUpdate(string assetPath)
    {
        string md5 = Md5.GetFileMd5(assetPath);
        if (json.optString(assetPath) == md5)
            return false;

        json.put(assetPath, md5);
        return true;
    }
}
#endif