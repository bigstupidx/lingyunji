using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    // 资源使用情况查看
    public class ResourcesUsedInfo
    {
        static ResourcesUsedInfo instance = null;

        public static ResourcesUsedInfo my
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResourcesUsedInfo();
                    instance.Init();
                }

                return instance;
            }
        }

        // 使用了key资源的列表
        Dictionary<string, List<string>> UsedList = new Dictionary<string, List<string>>();

        public int Count { get { return UsedList.Count; } }

        bool isIniting = false;

        public void Init()
        {
            if (isIniting)
                return;

            isIniting = true;
            UsedList.Clear();
            XTools.Utility.ForEachInfo info = new XTools.Utility.ForEachInfo();
            XTools.GlobalCoroutine.StartCoroutine(XTools.Utility.ForEachAsync("Assets", (AssetImporter ai) => { }, 
                (string assetPath) => 
                {
                    if ((info.current % 100) == 0)
                    {
                        Debug.LogFormat("total:{0} current:{1}", info.total, info.current);
                    }

                    if (info.current == info.total)
                        isIniting = true;

                    string[] assets = AssetDatabase.GetDependencies(assetPath, true);
                    foreach (var asset in assets)
                    {
                        if (asset == assetPath)
                            continue;

                        List<string> l = null;
                        if (!UsedList.TryGetValue(asset, out l))
                        {
                            l = new List<string>();
                            UsedList.Add(asset, l);
                        }

                        if (l.Contains(assetPath))
                            continue;
                        l.Add(assetPath);
                    }

                    return false;
                }, null, info));
        }

        public List<string> GetUsedInfo(string assetPath)
        {
            List<string> v = null;
            if (UsedList.TryGetValue(assetPath, out v))
                return v;

            return new List<string>();
        }
    }
}