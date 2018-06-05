#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public class BuildPlayer
    {
        public static string Build(string[] levels, string locationPathName)
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildOptions options = BuildOptions.None;

#if USE_RESOURCESEXPORT
            var spritePackerMode = EditorSettings.spritePackerMode;
            if (spritePackerMode != SpritePackerMode.Disabled)
            {
                EditorSettings.spritePackerMode = SpritePackerMode.Disabled;
            }
#endif
            locationPathName = locationPathName.Replace("\\", "/");
            try
            {
                switch (target)
                {
                case BuildTarget.Android:
                    {
                        PlayerSettings.Android.keystoreName = Application.dataPath + "/Scripts/ThirdPartyLibrary/PackTool/user.keystore";
                        PlayerSettings.Android.keystorePass = "eyesblack";
                        PlayerSettings.Android.keyaliasName = "xys";
                        PlayerSettings.Android.keyaliasPass = "eyesblack";

                        if (!locationPathName.EndsWith(".apk", true, null))
                        {
                            locationPathName += ".apk";
                        }

                        if (File.Exists(locationPathName))
                            File.Delete(locationPathName);
                        //options |= (BuildOptions.ConnectWithProfiler | BuildOptions.Development);
                    }
                    break;
                case BuildTarget.iOS:
                    {
                        if (locationPathName.EndsWith(".ipa", true, null))
                        {
                            if (File.Exists(locationPathName))
                                File.Delete(locationPathName);

                            locationPathName = locationPathName.Substring(0, locationPathName.LastIndexOf('.'));
                        }

                        if (Directory.Exists(locationPathName))
                            Directory.Delete(locationPathName, true);

                        //options |= (BuildOptions.ConnectWithProfiler | BuildOptions.Development);
                    }
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    {
                        if (!locationPathName.EndsWith(".exe", true, null))
                        {
                            if (Directory.Exists(locationPathName))
                                Directory.Delete(locationPathName, true);

                            locationPathName += "/Client.exe";
                        }
                        else
                        {
                            string path = locationPathName.Substring(0, locationPathName.LastIndexOf('/'));
                            if (Directory.Exists(path))
                                Directory.Delete(path, true);
                        }
                    }
                    break;
                }

                Directory.CreateDirectory(locationPathName.Substring(0, locationPathName.LastIndexOf('/')));
                Debug.LogFormat("BuildPipeline.BuildPlayer({0})", locationPathName);
                BuildPipeline.BuildPlayer(levels, locationPathName, target, options);

                switch (target)
                {
                case BuildTarget.iOS:
                    {
                        if (File.Exists(locationPathName + ".ipa"))
                        {
                            Directory.Delete(locationPathName, true);

                            locationPathName += ".ipa";
                        }
                        else
                        {
                            if (Utility.WinZipPackZip(locationPathName))
                            {
                                Directory.Delete(locationPathName, true);
                            }
                        }
                    }
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    {
                        string path = locationPathName.Substring(0, locationPathName.LastIndexOf('/'));
                        string[] files = Directory.GetFiles(path, "*.pdb");
                        for (int i = 0; i < files.Length; ++i)
                        {
                            File.Delete(files[i]);
                        }

#if USER_IFLY
                        string datapath = locationPathName.Substring(0, locationPathName.LastIndexOf('.')) + "_Data/StreamingAssets/";
                        Utility.CopyFolder(Application.dataPath + "/../OtherSDK/ifly/pcexe/", datapath);
#endif
                    }
                    break;
                case BuildTarget.Android:
                    {
//#if USER_ALLRESOURCES
                        if (File.Exists(locationPathName))
                        {
                            File.Copy(locationPathName, locationPathName + "_copy_");
                            File.Delete(locationPathName);
                            File.Copy(locationPathName + "_copy_", locationPathName);
                            File.Delete(locationPathName + "_copy_");
                        }
//#endif
                    }
                    break;
                }

                return locationPathName;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
#if USE_RESOURCESEXPORT
            finally
            {
                if (spritePackerMode != SpritePackerMode.Disabled)
                {
                    EditorSettings.spritePackerMode = spritePackerMode;
                }
            }
#endif
            return null;
        }
    }
}
#endif