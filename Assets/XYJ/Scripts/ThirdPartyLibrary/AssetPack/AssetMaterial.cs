#if USE_ABL
using UnityEngine;

namespace ABL
{
    class AssetMaterial : TAsset<Material>
    {
        public AssetMaterial(string url) : base(url)
        {

        }

        protected override void OnLoadEnd()
        {
#if UNITY_EDITOR
            if (asset != null)
                asset.shader = Shader.Find(asset.shader.name);
#endif
        }
    }
}
#endif