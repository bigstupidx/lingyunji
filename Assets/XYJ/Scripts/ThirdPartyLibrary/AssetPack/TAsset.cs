#if USE_ABL
using UnityEngine;

namespace ABL
{
    class TAsset<T> : IAsset where T : Object
    {
        public TAsset(string url)
        {
            this.url = url;
        }

        public string url { get; private set; }
        public AssetBundle ab { get; protected set; }
        public T asset { get; protected set; }
        public Object obj { get { return asset; } }

        protected IAsset[] Depends = null;
        public IAsset[] depends { get { return Depends; } }

        protected int ref_count = 0; // 外部引用的个数
        public int RefCount { get { return ref_count; } }

        public void AddRef()
        {
            ++ref_count;
        }

        public void SubRef()
        {
            --ref_count;
        }

        protected virtual void OnUnload()
        {

        }

        public void Unload()
        {
            OnUnload();

            if (ab != null)
            {
                Debug.LogFormat("url:{0} Unload", url);
                ab.Unload(true);
                ab = null;
            }

            if (Depends != null)
            {
                foreach (var a in Depends)
                {
                    if (a != null)
                        a.SubRef();
                }
                Depends = null;
            }
        }

        // AB加载完成
        public void OnAssetLoadEnd(AssetBundle ab, IAsset[] deps)
        {
            this.ab = ab;
            Depends = deps;
            if (ab == null)
            {
                Debug.LogErrorFormat("url:{0} is null ab!", url);
            }
            else
            {
                OnAssetLoadEnd();
            }

            OnLoadEnd();
        }

        protected virtual void OnAssetLoadEnd()
        {
            asset = ab.LoadAsset<T>("assets/" + ab.name);
        }

        // 资源加载结束
        protected virtual void OnLoadEnd()
        {

        }

        public override string ToString()
        {
            return url;
        }
    }

    class MultiAsset : TAsset<Object>
    {
        public MultiAsset(string url) : base(url)
        {

        }

        protected override void OnAssetLoadEnd()
        {

        }
    }

    class UnityAsset : TAsset<Object>
    {
        public UnityAsset(string url) : base(url)
        {

        }
    }
}
#endif