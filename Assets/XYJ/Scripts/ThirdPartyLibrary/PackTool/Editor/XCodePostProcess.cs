#if UNITY_EDITOR && UNITY_IPHONE
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class XCodePostProcess
{
    public static T GetOrCreate<T>(this PlistElementDict dic, string key) where T : PlistElement
    {
        PlistElement pe = null;
        if (dic.values.TryGetValue(key, out pe))
        {
            if (pe is T)
                return (T)pe;
        }

        if (typeof(T) == typeof(PlistElementString))
        {
            dic.values[key] = new PlistElementString("");
        }
        else if (typeof(T) == typeof(PlistElementInteger))
        {
            dic.values[key] = new PlistElementInteger(0);
        }
        else if (typeof(T) == typeof(PlistElementBoolean))
        {
            dic.values[key] = new PlistElementBoolean(false);
        }
        else if (typeof(T) == typeof(PlistElementArray))
        {
            dic.values[key] = new PlistElementArray();
        }
        else if (typeof(T) == typeof(PlistElementDict))
        {
            dic.values[key] = new PlistElementDict();
        }
        else
        {
            Debug.LogErrorFormat("type:{0} not !", typeof(T).Name);
        }

        return dic.values[key] as T;
    }

    static List<System.Action<PostProcessBase>> Lists = new List<System.Action<PostProcessBase>>();

    public static void Reg(System.Action<PostProcessBase> fun)
    {
        Lists.Add(fun);
    }

//     [MenuItem("Assets/OnPostProcessBuild")]
//     static void OnPostProcessBuild()
//     {
//         OnPostProcessBuild(EditorUserBuildSettings.activeBuildTarget, "D:/Work/xys_ios/Game/AllResources/iPhone/2016-2-15-18-20-52");
//     }

    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        PostProcessBase ppb = new PostProcessBase(pathToBuiltProject);
        PlistDocument plist = ppb.plistDoc;
        plist.root.GetOrCreate<PlistElementBoolean>("Application supports iTunes file sharing").value = true;
        ppb.project.SetBuildProperty(ppb.targetGuid, "ENABLE_BITCODE", "NO");

        foreach (System.Action<PostProcessBase> itor in Lists)
        {
            itor(ppb);
        }

        ppb.SaveFile();

#if !(USE_ABL || USE_RESOURCESEXPORT)
        ppb.BuildIpa();
#endif
    }
}
#endif
