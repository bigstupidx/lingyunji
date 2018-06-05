using System.IO;
using CommonBase;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace PackTool
{
    public class ProjectSetting
    {
        static XMLAttributes Empty = new XMLAttributes("");
        static XMLAttributes current = null;
        static TinyXML.XMLNode CurrentNode = null;

        [MenuItem("PackTool/同步工程设置")]
        public static void SyncProjectSet()
        {
            Check();
        }

#if UNITY_STANDALONE_WIN
        static void SetPCSetting()
        {
            PlayerSettings.defaultIsFullScreen = false;
            PlayerSettings.defaultIsNativeResolution = false;
            PlayerSettings.defaultScreenWidth = 1334;
            PlayerSettings.defaultScreenHeight = 750;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        }
#endif
        static void SetAutoGraphicsApi(TinyXML.XMLNode node, BuildTarget target)
        {
#if UNITY_STANDALONE_WIN
            SetPCSetting();
#endif
            SetValue(node, "AutoGraphicsApi", () =>
            {
                bool isauto = current.getValueAsBool("value", PlayerSettings.GetUseDefaultGraphicsAPIs(target));
                PlayerSettings.SetUseDefaultGraphicsAPIs(target, isauto);
                if (!isauto)
                {
                    node = node.Get("AutoGraphicsApi");
                    if (node != null)
                    {
                        List<GraphicsDeviceType> gdts = new List<GraphicsDeviceType>();
                        for (int i = 0; i < node.childList.Count; ++i)
                        {
                            XMLAttributes xmlAtt = new XMLAttributes(node.childList[i].text, node.childList[i].attributes);
                            var value = xmlAtt.getValueAsEnum("value", GraphicsDeviceType.Null);
                            if (value != GraphicsDeviceType.Null && !gdts.Contains(value))
                                gdts.Add(value);
                        }

                        List<GraphicsDeviceType> srcs = new List<GraphicsDeviceType>(PlayerSettings.GetGraphicsAPIs(target));
                        if (!isSame(srcs, gdts))
                        {
                            PlayerSettings.SetGraphicsAPIs(target, gdts.ToArray());
                        }
                    }
                }
            });
        }


        static bool isSame<T>(List<T> x, List<T> y)
        {
            if (x.Count != y.Count)
                return false;

            for (int i = 0; i < x.Count; ++i)
            {
                if (!object.Equals(x[i], y[i]))
                    return false;
            }
            return true;
        }

        static public void Check()
        {
            GraphicsSettingsRef.Default();
            string filename = Application.dataPath + "/../../ProjectSetting.xml";
            if (!File.Exists(filename))
                filename = Application.dataPath + "/../ProjectSetting.xml";

            if (!File.Exists(filename))
                return;

            string file_text = File.ReadAllText(filename);

            TinyXML.XMLNode nodes = TinyXML.XMLParser.Parse((int p) => { return file_text[p]; }, file_text.Length);
            nodes = nodes.childList[0];

            SetValue(nodes, "colorSpace", () => { PlayerSettings.colorSpace = current.getValueAsEnum<ColorSpace>("value", PlayerSettings.colorSpace); });
            SetValue(nodes, "mobileMTRendering", () => { PlayerSettings.mobileMTRendering = current.getValueAsBool("value", PlayerSettings.mobileMTRendering); });

            SetValue(nodes, "aotOptions", () => { PlayerSettings.aotOptions = current.getValueAsString("value", PlayerSettings.aotOptions); });
            SetValue(nodes, "apiCompatibilityLevel", () => { PlayerSettings.apiCompatibilityLevel = current.getValueAsEnum<ApiCompatibilityLevel>("value", PlayerSettings.apiCompatibilityLevel); });
            SetValue(nodes, "bundleVersion", () => { PlayerSettings.bundleVersion = current.getValueAsString("value", PlayerSettings.bundleVersion); });
            SetValue(nodes, "accelerometerFrequency", () => { PlayerSettings.accelerometerFrequency = current.getValueAsInteger("value", PlayerSettings.accelerometerFrequency); });
            SetValue(nodes, "companyName", () => { PlayerSettings.companyName = current.getValueAsString("value", PlayerSettings.companyName); });
            SetValue(nodes, "productName", () => { PlayerSettings.productName = current.getValueAsString("value", PlayerSettings.productName); });

            SetValue(nodes, "bundleIdentifier", () => { PlayerSettings.applicationIdentifier = current.getValueAsString("value", PlayerSettings.applicationIdentifier); });
            SetValue(nodes, "defaultInterfaceOrientation", 
                () => 
                {
                    PlayerSettings.defaultInterfaceOrientation = current.getValueAsEnum<UIOrientation>("value", PlayerSettings.defaultInterfaceOrientation); 
                    if (CurrentNode == null)
                        return;

                    if (PlayerSettings.defaultInterfaceOrientation == UIOrientation.AutoRotation)
                    {
                        TinyXML.XMLNode node = CurrentNode;
                        SetValue(node, "allowedAutorotateToLandscapeLeft", () => { PlayerSettings.allowedAutorotateToLandscapeLeft = current.getValueAsBool("value", PlayerSettings.allowedAutorotateToLandscapeLeft); });
                        SetValue(node, "allowedAutorotateToLandscapeRight", () => { PlayerSettings.allowedAutorotateToLandscapeRight = current.getValueAsBool("value", PlayerSettings.allowedAutorotateToLandscapeRight); });
                        SetValue(node, "allowedAutorotateToPortrait", () => { PlayerSettings.allowedAutorotateToPortrait = current.getValueAsBool("value", PlayerSettings.allowedAutorotateToPortrait); });
                        SetValue(node, "allowedAutorotateToPortraitUpsideDown", () => { PlayerSettings.allowedAutorotateToPortraitUpsideDown = current.getValueAsBool("value", PlayerSettings.allowedAutorotateToPortraitUpsideDown); });
                    }
                });

            SetValue(nodes, "strippingLevel", () => { PlayerSettings.strippingLevel = current.getValueAsEnum<StrippingLevel>("value", PlayerSettings.strippingLevel); });
            SetValue(nodes, "use32BitDisplayBuffer", () => { PlayerSettings.use32BitDisplayBuffer = current.getValueAsBool("value", PlayerSettings.use32BitDisplayBuffer); });
            SetValue(nodes, "useAnimatedAutorotation", () => { PlayerSettings.useAnimatedAutorotation = current.getValueAsBool("value", PlayerSettings.useAnimatedAutorotation); });

            SetValue(nodes, "Android", 
                () => 
                {
                    if (CurrentNode == null)
                        return;

                    TinyXML.XMLNode node = CurrentNode;
                    SetValue(node, "bundleVersionCode", () => { PlayerSettings.Android.bundleVersionCode = current.getValueAsInteger("value", PlayerSettings.Android.bundleVersionCode); });
                    SetValue(node, "forceInternetPermission", () => { PlayerSettings.Android.forceInternetPermission = current.getValueAsBool("value", PlayerSettings.Android.forceInternetPermission); });
                    SetValue(node, "forceSDCardPermission", () => { PlayerSettings.Android.forceSDCardPermission = current.getValueAsBool("value", PlayerSettings.Android.forceSDCardPermission); });
                    SetValue(node, "minSdkVersion", () => { PlayerSettings.Android.minSdkVersion = current.getValueAsEnum<AndroidSdkVersions>("value", PlayerSettings.Android.minSdkVersion); });
                    SetValue(node, "preferredInstallLocation", () => { PlayerSettings.Android.preferredInstallLocation = current.getValueAsEnum<AndroidPreferredInstallLocation>("value", PlayerSettings.Android.preferredInstallLocation); });
                    SetValue(node, "showActivityIndicatorOnLoading", () => { PlayerSettings.Android.showActivityIndicatorOnLoading = current.getValueAsEnum<AndroidShowActivityIndicatorOnLoading>("value", PlayerSettings.Android.showActivityIndicatorOnLoading); });
                    SetValue(node, "splashScreenScale", () => { PlayerSettings.Android.splashScreenScale = current.getValueAsEnum<AndroidSplashScreenScale>("value", PlayerSettings.Android.splashScreenScale); });
                    SetValue(node, "targetDevice", () => { PlayerSettings.Android.targetDevice = current.getValueAsEnum<AndroidTargetDevice>("value", PlayerSettings.Android.targetDevice); });
                    SetValue(node, "disableDepthAndStencilBuffers", () => { PlayerSettings.Android.disableDepthAndStencilBuffers = current.getValueAsBool("value", PlayerSettings.Android.disableDepthAndStencilBuffers); });
                    SetValue(node, "useAPKExpansionFiles", () => { PlayerSettings.Android.useAPKExpansionFiles = current.getValueAsBool("value", PlayerSettings.Android.useAPKExpansionFiles); });
                    SetValue(node, "AndroidSdkVersions", () => { PlayerSettings.Android.minSdkVersion = current.getValueAsEnum("value", PlayerSettings.Android.minSdkVersion); });

                    SetAutoGraphicsApi(node, BuildTarget.Android);
                });

            SetValue(nodes, "iOS",
                () =>
                {
                    if (CurrentNode == null)
                        return;
                    
                    TinyXML.XMLNode node = CurrentNode;
                    SetValue(node, "appInBackgroundBehavior", () => { PlayerSettings.iOS.appInBackgroundBehavior = current.getValueAsEnum("value", PlayerSettings.iOS.appInBackgroundBehavior); });
                    SetValue(node, "allowHTTPDownload", () => { PlayerSettings.iOS.allowHTTPDownload = current.getValueAsBool("value", PlayerSettings.iOS.allowHTTPDownload); });
                    SetValue(node, "prerenderedIcon", () => { PlayerSettings.iOS.prerenderedIcon = current.getValueAsBool("value", PlayerSettings.iOS.prerenderedIcon); });
                    SetValue(node, "requiresPersistentWiFi", () => { PlayerSettings.iOS.requiresPersistentWiFi = current.getValueAsBool("value", PlayerSettings.iOS.requiresPersistentWiFi); });
                    SetValue(node, "scriptCallOptimization", () => { PlayerSettings.iOS.scriptCallOptimization = current.getValueAsEnum("value", PlayerSettings.iOS.scriptCallOptimization); });
                    SetValue(node, "sdkVersion", () => { PlayerSettings.iOS.sdkVersion = current.getValueAsEnum("value", PlayerSettings.iOS.sdkVersion); });
                    SetValue(node, "showActivityIndicatorOnLoading", () => { PlayerSettings.iOS.showActivityIndicatorOnLoading = current.getValueAsEnum("value", PlayerSettings.iOS.showActivityIndicatorOnLoading); });
                    SetValue(node, "statusBarStyle", () => { PlayerSettings.iOS.statusBarStyle = current.getValueAsEnum("value", PlayerSettings.iOS.statusBarStyle); });
                    SetValue(node, "targetDevice", () => { PlayerSettings.iOS.targetDevice = current.getValueAsEnum("value", PlayerSettings.iOS.targetDevice); });
                    SetValue(node, "targetOSVersion", () => { PlayerSettings.iOS.targetOSVersion = current.getValueAsEnum("value", PlayerSettings.iOS.targetOSVersion); });

                    SetValue(node, "Architecture", () => { PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, current.getValueAsInteger("value", PlayerSettings.GetArchitecture(BuildTargetGroup.iOS))); });

                    SetAutoGraphicsApi(node, BuildTarget.iOS);
                });
        }

        static void SetValue(TinyXML.XMLNode node, string path, System.Action fun)
        {
            if (!string.IsNullOrEmpty(path))
                node = node.Get(path);

            CurrentNode = node;
            if (node == null)
            {
                current = Empty;
            }
            else
            {
                current = new XMLAttributes(node.text, node.attributes);
            }
            
            fun();
        }
    }
}