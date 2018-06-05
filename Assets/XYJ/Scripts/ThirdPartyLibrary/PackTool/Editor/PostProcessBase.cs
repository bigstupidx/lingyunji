#if UNITY_EDITOR && UNITY_IPHONE
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.Collections.Generic;

public class PostProcessBase
{
    public string proj_root { get; private set; }
    public string project_path { get; private set; }
    public string plist_path { get; private set; }

    public PBXProject project { get; private set; }
    public PlistDocument plistDoc { get; private set; }

    public string targetGuid { get; private set; }

    public PostProcessBase(string filepath)
    {
        proj_root = filepath;

        plist_path = Path.Combine(proj_root, "Info.plist");
        project_path = PBXProject.GetPBXProjectPath(proj_root);

        plistDoc = new PlistDocument();
        plistDoc.ReadFromFile(plist_path);
        plistDoc.root.CreateDict("NSAllowsArbitraryLoads").SetBoolean("NSAppTransportSecurity", true);

        project = new PBXProject();
        project.ReadFromFile(project_path);

        targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
    }

    public void SaveFile()
    {
        project.WriteToFile(project_path);
        plistDoc.WriteToFile(plist_path);
    }

    static void FileCopy(string src, string dst)
    {
        if (File.Exists(dst))
            File.Delete(dst);

        File.Copy(src, dst);
    }

    public void BuildIpa()
    {
#if UNITY_EDITOR_WIN
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        FileCopy(Application.dataPath + "/../iOS/BuildIOS.bat", proj_root + "/BuildIOS.bat");
        FileCopy(Application.dataPath + "/../iOS/iOS.mobileprovision", proj_root + "/iOS.mobileprovision");
        FileCopy(Application.dataPath + "/../iOS/test.bat", proj_root + "/test.bat");
        if (!proj_root.Contains(":"))
            p.StartInfo.FileName = Application.dataPath + "/../" + proj_root + "/BuildIOS.bat";
        else
            p.StartInfo.FileName = proj_root + "/BuildIOS.bat";

        string ipaname = PlayerSettings.iPhoneBundleIdentifier.Substring(PlayerSettings.iPhoneBundleIdentifier.LastIndexOf('.') + 1);

        string copyname = proj_root.Substring(proj_root.LastIndexOf('/') + 1);
        if (copyname.ToLower().EndsWith(".ipa"))
            copyname = copyname.Substring(0, copyname.Length - 4);

        // 参数1 xcode路径
        // 参数2 打包出来的ipa名
        string cmd = string.Format("\"{0}\" \"{1}\"", proj_root, ipaname);
        p.StartInfo.Arguments = cmd;
        Debug.LogFormat("cmd:{0} path:{1}", cmd, p.StartInfo.FileName);
        p.Start();
        p.WaitForExit();
        p.Close();
        //Directory.SetCurrentDirectory(currentpath);

        if (File.Exists(proj_root + "/Packages/" + ipaname + ".ipa"))
        {
            File.Delete(proj_root + "/BuildIOS.bat");
            File.Delete(proj_root + "/iphone.mobileprovision");
            File.Delete(proj_root + "/test.bat");

            if (File.Exists(proj_root + "/../" + copyname + ".ipa"))
                File.Delete(proj_root + "/../" + copyname + ".ipa");

            File.Copy(proj_root + "/Packages/" + ipaname + ".ipa", proj_root + "/../" + copyname + ".ipa");
        }
#endif
    }
}
#endif