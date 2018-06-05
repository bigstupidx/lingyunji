#if USE_ABL
using UnityEngine;

namespace ABL
{
    public interface IAsset
    {
        string url { get; }
        AssetBundle ab { get; }
        IAsset[] depends { get; }
        Object obj { get; }
        int RefCount { get; }
        void AddRef();
        void SubRef();
        void Unload();

        // AB加载完成
        void OnAssetLoadEnd(AssetBundle ab, IAsset[] deps);
    }
}
#endif