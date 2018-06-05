#if UNITY_EDITOR && UNITY_IPHONE
using UnityEngine;
using UnityEditor.iOS.Xcode;
using System.IO;

[UnityEditor.InitializeOnLoad()]
public static class CameraPhotoPostProcess
{
    static CameraPhotoPostProcess()
    {
        XCodePostProcess.Reg(OnPostProcessBuild);
    }

    static void OnPostProcessBuild(PostProcessBase ppb)
    {
        PlistDocument plist = ppb.plistDoc;

        PBXProject project = ppb.project;
        string targetGuid = ppb.targetGuid;
        project.AddFrameworkToProject(targetGuid, "MobileCoreServices.framework", true);

        plist.root.GetOrCreate<PlistElementString>("NSAppleMusicUsageDescription").value = "for playing music";
        plist.root.GetOrCreate<PlistElementString>("NSCameraUsageDescription").value = "for making pictures";
        plist.root.GetOrCreate<PlistElementString>("NSPhotoLibraryUsageDescription").value = "for taking pictures";
    }
}
#endif
