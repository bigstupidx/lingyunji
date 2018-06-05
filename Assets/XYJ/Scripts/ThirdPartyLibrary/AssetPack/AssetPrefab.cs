#if USE_ABL
using UnityEngine;

namespace ABL
{
    class AssetPrefab : TAsset<GameObject>
    {
        public AssetPrefab(string url) : base(url)
        {

        }

        PrefabAsset prefabAsset;

        protected override void OnLoadEnd()
        {
            PrefabAsset.isAutoAdd = true;
            (prefabAsset = asset.AddComponent<PrefabAsset>()).url = url;
            PrefabAsset.isAutoAdd = false;
        }

        protected override void OnUnload()
        {
            if (prefabAsset != null)
            {
                Object.DestroyImmediate(prefabAsset);
                prefabAsset = null;
            }
        }
    }
}
#endif