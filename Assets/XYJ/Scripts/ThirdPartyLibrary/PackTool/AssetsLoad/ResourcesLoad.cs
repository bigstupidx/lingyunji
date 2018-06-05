#if USE_RESOURCESEXPORT
using XTools;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 加载基础资源
namespace PackTool
{
    public class ResourcesLoad<T> : AssetLoad<T> where T : Object
    {
        static public void Load(string name, ResourcesEnd<T> fun, object funp)
        {
            ResourcesLoad<T>.LoadImp(name, fun, funp, Create);
        }

        static ResourcesLoad<T> Create(string name)
        {
            return CreateAsset<ResourcesLoad<T>>(name);
        }

        protected override void LoadAsset(string name)
        {
            AssetBundleLoad.Load(name, LoadAssetEnd);
        }

        protected virtual bool OnLoadAssetEnd(AssetBundle assetBundle)
        {
            return false;
        }

        void LoadAssetEnd(AssetBundle assetBundle)
        {
            isDone = true;
            asset = null;
            if (assetBundle == null)
            {
                Debuger.LogError(string.Format("LoadAssetEnd: {0} type:ResourcesLoad<{1}> assetBundle == null!", url, typeof(T).Name));
            }
            else
            {
                if (!OnLoadAssetEnd(assetBundle))
                {
                    asset = assetBundle.mainAsset as T;
                    if (asset == null)
                    {
                        //string[] ns = assetBundle.GetAllAssetNames();
                        //Object[] objs = assetBundle.LoadAllAssets<Object>();
                        Debuger.DebugLog(url);
                    }
                }
                assetBundle.Unload(false);
            }

            OnEnd();
        }

        // 回收自身
        protected override void FreeSelf()
        {
            FreeAsset(this);
        }
    }
}
#endif