#if USE_ABL
using System;
using UnityEngine;
using System.Collections.Generic;
using XTools;

namespace ABL
{
    public partial class AssetsMgr
    {
        // 已加载完的资源
        static Dictionary<string, IAsset> Assets = new Dictionary<string, IAsset>();

        // 正在加载当中的资源
        static Dictionary<string, Asseting> Assetsing = new Dictionary<string, Asseting>();

        internal static IAsset Get(string url)
        {
            IAsset asset;
            if (Assets.TryGetValue(url, out asset))
                return asset;

            return null;
        }

        public static void UnloadAll()
        {
            foreach (var itor in Assets)
            {
                itor.Value.ab.Unload(false);
            }

            Assets.Clear();
        }

        // 加载一个预置体
        public static void Load<T>(string url, Action<T> fun) where T : UnityEngine.Object
        {
            Load(url, (IAsset a) =>
            {
                fun(a == null ? null : a.obj as T);
            });
        }

        // 加载一个预置体
        public static void Load(string url, Action<GameObject> fun)
        {
            Load<GameObject>(url, fun);
        }

        public static void Load(string url, Action<Material> fun)
        {
            Load<Material>(url, fun);
        }

        public static void LoadScene(string url, Action<IAsset> fun)
        {
            Load(url, fun);
        }

        public static void LoadAtlas(string url, Action<Dictionary<string, Sprite>> fun)
        {
            Load(url, (IAsset asset) =>
            {
                AssetAtlas atlas = asset as AssetAtlas;
                if (atlas == null)
                {
                    fun(null);
                }
                else
                {
                    fun(atlas.Sprites);
                }
            });
        }

        static void Load(string url, Action<IAsset> fun)
        {
            if (url.StartsWith("AB/"))
                url = url.Substring(3);

            IAsset a = null;
            if (Assets.TryGetValue(url, out a))
            {
                TimerMgrObj.Instance.addFrameLateUpdate((object p) => { fun(a); return false; }, null);
            }
            else
            {
                Asseting asset = null;
                if (Assetsing.TryGetValue(url, out asset))
                {
                    asset.Add(fun);
                }
                else
                {
                    asset = new Asseting();
                    asset.url = url;
                    asset.Add(fun);

                    Assetsing.Add(url, asset);

                    asset.Load();
                }
            }
        }

        public static AsyncOperation UnloadUnusedAssets()
        {
            UnloadUnusedAssetsSelf();
            return Resources.UnloadUnusedAssets();
        }

        // 卸载资源的总个数
        static int UnloadUnusedAssetsSelf()
        {
            int total = 0;
            List<IAsset> unloads = new List<IAsset>();
            while (true)
            {
                foreach (var itor in Assets)
                {
                    if (itor.Value.RefCount == 0)
                    {
                        // 外部没有任何地方引用此资源了，可以卸载了
                        unloads.Add(itor.Value);
                    }
                }

                if (unloads.Count == 0)
                    break;

                total += unloads.Count;
                for (int i = 0; i < unloads.Count; ++i)
                {
                    IAsset a = unloads[i];
                    a.Unload();
                    Assets.Remove(a.url);
                }

                unloads.Clear();
            }

            return total;
        }
    }
}
#endif