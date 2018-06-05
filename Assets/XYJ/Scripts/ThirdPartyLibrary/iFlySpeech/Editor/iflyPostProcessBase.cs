#if UNITY_EDITOR && UNITY_IPHONE && USER_IFLY
using UnityEngine;
using UnityEditor.iOS.Xcode;
using System.IO;

[UnityEditor.InitializeOnLoad()]
public static class iflyPostProcessBase
{
    static iflyPostProcessBase()
    {
        XCodePostProcess.Reg(OnPostProcessBuild);
    }

    const string appid = "57e32620";

    public static void CopyFolder(string direcSource, string direcTarget, System.Func<string, bool> fun)
    {
        if (!Directory.Exists(direcSource))
            return;

        if (!Directory.Exists(direcTarget))
            Directory.CreateDirectory(direcTarget);

        DirectoryInfo direcInfo = new DirectoryInfo(direcSource);
        FileInfo[] files = direcInfo.GetFiles();
        foreach (FileInfo file in files)
        {
            if (fun != null && !fun(file.Name))
                continue;

            file.CopyTo(Path.Combine(direcTarget, file.Name), true);
        }

        DirectoryInfo[] direcInfoArr = direcInfo.GetDirectories();
        foreach (DirectoryInfo dir in direcInfoArr)
            CopyFolder(Path.Combine(direcSource, dir.Name), Path.Combine(direcTarget, dir.Name), fun);
    }


    static void OnPostProcessBuild(PostProcessBase ppb)
    {
        PlistDocument plist = ppb.plistDoc;

        PBXProject project = ppb.project;

        string targetGuid = ppb.targetGuid;

        // 需要的依赖库
        project.AddFrameworkToProject(targetGuid, "libz.tbd", true);
        project.AddFrameworkToProject(targetGuid, "CoreLocation.framework", true);
        project.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", true);
        project.AddFrameworkToProject(targetGuid, "AVFoundation.framework", true);
        project.AddFrameworkToProject(targetGuid, "AddressBook.framework", true);
        project.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", true);
        project.AddFrameworkToProject(targetGuid, "AudioToolbox.framework", true);
        project.AddFrameworkToProject(targetGuid, "QuartzCore.framework", true);
        project.AddFrameworkToProject(targetGuid, "UIKit.framework", true);
        project.AddFrameworkToProject(targetGuid, "Foundation.framework", true);
        project.AddFrameworkToProject(targetGuid, "CoreGraphics.framework", true);

        //File.Copy(Application.dataPath + "/../OtherSDK/ifly/iflyMSC.framework.zip", ppb.proj_root + "/iflyMSC.framework.zip");
        XTools.Utility.CopyFolder(Application.dataPath + "/../OtherSDK/ifly/iflyMSC.framework", ppb.proj_root + "/iflyMSC.framework");

        project.AddFileToBuild(targetGuid, project.AddFile("iflyMSC.framework", "iflyMSC.framework", PBXSourceTree.Source));
        project.SetBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
        project.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");
        project.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)");

        project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

        // 添加URL-Scheme
        {
            PlistElementArray urltypes = plist.root.GetOrCreate<PlistElementArray>("CFBundleURLTypes");
            urltypes.AddDict().GetOrCreate<PlistElementArray>("CFBundleURLSchemes").AddString("isp" + appid);
        }

        // 添加白名单
        {
            plist.root.GetOrCreate<PlistElementArray>("LSApplicationQueriesSchemes").AddString("IFlySpeechPlus");
        }
    }
}
#endif
