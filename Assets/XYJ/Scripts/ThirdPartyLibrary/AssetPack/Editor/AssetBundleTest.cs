#if USE_ABL
using XTools;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Core;

public static class AssetBundleTest
{
    [MenuItem("PackTool/测试所有的AssetBundle")]
    [MenuItem("Assets/PackTool/测试所有的AssetBundle")]
    public static void TestAllAssetBundle()
    {
        GlobalCoroutine.StartCoroutine(TestAllAssetBundleItor());
    }

    static IEnumerator TestAllAssetBundleItor()
    {
        FileSystemScanner scanner = new FileSystemScanner("", "");
        int start = 0;

        // 动画对应动画控制器
        Dictionary<string, HashSet<string>> animToControllers = new Dictionary<string, HashSet<string>>();

        List<ScanEventArgs> scenes = new List<ScanEventArgs>();
        int lenght = ResourcesPath.streamingAssetsPath.Length;
        scanner.ProcessFile =
            (object sender, ScanEventArgs e) =>
            {
                string key = e.Name.Substring(lenght).Replace('\\', '/');
                if (!key.EndsWith(".ab"))
                    return;

                if (key.EndsWith(".unity.ab"))
                {
                    scenes.Add(e);
                    return;
                }

                AssetBundle assetBundle = AssetBundle.LoadFromFile(e.Name);
                if (assetBundle == null)
                {
                    Debug.LogError(string.Format("资源:{0}加载失败!", e.Name));
                    return;
                }

                if (key.EndsWith(".controller"))
                {
                    foreach (string name in assetBundle.GetAllAssetNames())
                    {
                        if (name.EndsWith(".controller") || name.EndsWith(".cs") || name.EndsWith(".dll"))
                            continue;

                        HashSet<string> list = null;
                        if (!animToControllers.TryGetValue(name, out list))
                        {
                            list = new HashSet<string>();
                            animToControllers.Add(name, list);
                        }

                        list.Add(key);
                    }
                }
                else
                {
                    Check(e.Name, assetBundle);
                }

                assetBundle.Unload(true);
                start++;
            };

        scanner.Scan(ResourcesPath.streamingAssetsPath, true);

        int animTotal = 0;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var itor in animToControllers)
        {
            if (itor.Value.Count >= 2)
            {
                ++animTotal;
                sb.AppendLine(string.Format("index:{2}) anim:{0} controller:{1}", itor.Key, itor.Value.Count, animTotal));
                foreach (string c in itor.Value)
                    sb.AppendLine(string.Format("   {0}", c));
            }
        }

        Debug.Log(string.Format("测试预置体总数:{0}! animToControllers:{1}/{2}", start, animTotal, animToControllers.Count));
        Debug.LogFormat("animController:\r\n{0}", sb.ToString());
        yield break;
    }

    public static void Check(string url, AssetBundle assetBundle)
    {
        if (assetBundle == null)
        {
            Debug.LogErrorFormat("url:{0} assetBundle == null", url);
            return;
        }

        string[] names = assetBundle.GetAllAssetNames();
        Object[] objs = assetBundle.LoadAllAssets();
        int index = 0;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine(string.Format("CheckAssetBundle url:{0}", url));
        for (int i = 1; i < names.Length; ++i)
        {
            string n = names[i];
            if (n.EndsWith(".js") || n.EndsWith(".cs"))
                continue;

            sb.AppendLine(string.Format("   {0}", n));
            ++index;
        }

        if (index >= 2)
        {
            Debug.Log(sb.ToString());
        }
    }
}
#endif