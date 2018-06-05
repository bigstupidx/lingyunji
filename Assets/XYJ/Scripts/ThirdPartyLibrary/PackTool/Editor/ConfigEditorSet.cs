using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PackTool.New;

namespace PackTool
{ 
    public class ConfigEditorSet
    {
        public string companyName = ""; // 公司名
        public string productName = ""; // 产品名
        public string bundleIdentifier = ""; // id名

        // 游戏的icon
        public List<string> icons = new List<string>();

        public void Check(Versions versions, VersionConfig config)
        {
            ProjectSetting.Check();

            PlayerSettings.companyName = companyName;
            PlayerSettings.productName = productName;

            List<Texture2D> t2ds = new List<Texture2D>(icons.Count);
            foreach (string s in icons)
                t2ds.Add(AssetDatabase.LoadAssetAtPath("Assets/" + s, typeof(Texture2D)) as Texture2D);

            PlayerSettings.SetIconsForTargetGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup, t2ds.ToArray());

#if UNITY_IPHONE
			PlayerSettings.iOS.buildNumber = versions.svn.ToString();
#endif

#if UNITY_ANDROID // 需要打SDK
            PlayerSettings.Android.bundleVersionCode = (int)versions.svn;
#endif

            PlayerSettings.applicationIdentifier = bundleIdentifier;
            AssetDatabase.SaveAssets();
        }
    }
}