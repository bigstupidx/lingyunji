#if USE_ABL
using XTools;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace ABL
{
    // 加载中的资源
    public partial class AssetsMgr
    {
        static void OnLoadAssetBundleEnd(string url, Action<IAsset> fun)
        {
            IAsset asset;
            if (!Assets.TryGetValue(url, out asset))
            {
                Asseting asseting;
                if (Assetsing.TryGetValue(url, out asseting))
                {
                    if (asseting.asset != null)
                        asset = asseting.asset;
                    else
                    {
                        asseting.OnABEnd.Add(fun);
                        return;
                    }
                }
            }

            if (asset != null)
            {
                TimerMgrObj.Instance.addFrameLateUpdate((object p) => { fun(asset); return false; }, null);
            }
            else
            {
                Load(url, fun);
            }
        }

        class Asseting
        {
            public string url;
            public string[] depends;

            List<Action<IAsset>> OnEnd = new List<Action<IAsset>>(); // 结束之后的回调
            internal List<Action<IAsset>> OnABEnd = new List<Action<IAsset>>(); // 结束之后的回调

            internal IAsset asset = null;

            // 开始加载资源
            public void Load()
            {
                AssetBundleLoad.LoadAsync(url, OnRootLoadEnd);
            }

            int total = 0;
            IAsset[] depAssets = null;

            AssetBundle assetBundle;

            static IAsset[] Emptys = new IAsset[0];

            void OnRootLoadEnd(AssetBundle ab)
            {
                asset = AssetFactory.Create(url);
                assetBundle = ab;

                for (int i = 0; i < OnABEnd.Count; ++i)
                    OnABEnd[i](asset);
                OnABEnd = null;

                depends = Dependencies.Get(url);
                depAssets = depends.Length ==0  ? Emptys : new IAsset[depends.Length];
                for (int i = 0; i < depends.Length; ++i)
                {
                    ++total;
                    int index = i;
                    OnLoadAssetBundleEnd(depends[i], (IAsset a) => { OnDependAssetEnd(a, index); });
                }

                if (total == 0)
                {
                    OnDependAssetEnd();
                }
            }

            void OnDependAssetEnd()
            {
                // 资源为预置体
                asset.OnAssetLoadEnd(assetBundle, depAssets);
                Assetsing.Remove(url);
                Assets.Add(url, asset);

                for (int i = 0; i < OnEnd.Count; ++i)
                {
                    OnEnd[i](asset);
                }
                OnEnd = null;

                Debug.LogFormat("url OnDependAssetEnd:{0}", url);
            }

            void OnDependAssetEnd(IAsset obj, int i)
            {
                --total;
                obj.AddRef();
                depAssets[i] = obj;
                if (total == 0)
                {
                    OnDependAssetEnd();
                }
            }

            public void Add(Action<IAsset> fun)
            {
                OnEnd.Add(fun);
            }
        }
    }
}
#endif