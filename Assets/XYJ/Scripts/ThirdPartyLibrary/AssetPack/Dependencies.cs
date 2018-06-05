#if USE_ABL
using UnityEngine;
using System.Collections.Generic;

namespace ABL
{
    public static class Dependencies
    {
        static Dictionary<string, string[]> depends = new Dictionary<string, string[]>();
        static AssetBundleManifest mainifest;

        public static System.Collections.IEnumerator Init()
        {
            bool isLoad = false;
            AssetBundleLoad.LoadAsync("AB", (AssetBundle ab)=> 
            {
                string[] assets = ab.GetAllAssetNames();
                mainifest = ab.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
                ab.Unload(false);

                isLoad = true;
            });

            while (!isLoad)
                yield return 0;
        }

        public static string[] Get(string url)
        {
            if (url.StartsWith("AB/"))
                url = url.Substring(3);

            string[] deps = null;
            if (depends.TryGetValue(url, out deps))
                return deps;

            deps = mainifest.GetAllDependencies(url);
            depends.Add(url, deps);
            return deps;
        }
    }
}
#endif