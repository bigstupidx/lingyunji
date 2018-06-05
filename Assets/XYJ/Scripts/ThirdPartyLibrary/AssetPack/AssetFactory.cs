#if USE_ABL
using UnityEngine;

namespace ABL
{
    class AssetFactory
    {
        public static IAsset Create(string url)
        {
            string type = url.Substring(url.LastIndexOf('.') + 1);
            switch (type)
            {
            case "prefab": return new AssetPrefab(url);
            case "mat": return new AssetMaterial(url);
            case "atlas": return new AssetAtlas(url);
            case "unity":return new MultiAsset(url);
            }

            return new UnityAsset(url);
        }
    }
}
#endif
