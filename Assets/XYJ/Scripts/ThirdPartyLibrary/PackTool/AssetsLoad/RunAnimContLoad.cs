#if USE_RESOURCESEXPORT
using XTools;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 加载基础资源
namespace PackTool
{
    public class RunAnimContLoad : AssetLoad<RuntimeAnimatorController>
    {
        static public void Load(string name, ResourcesEnd<RuntimeAnimatorController> fun, object funp)
        {
            RunAnimContLoad.LoadImp(name, fun, funp, Create);
        }

        static RunAnimContLoad Create(string name)
        {
            return CreateAsset<RunAnimContLoad>(name);
        }

        protected override void LoadAsset(string name)
        {
            AssetBundleLoad.Load(name, LoadAssetEnd);
        }

        void LoadAssetEnd(AssetBundle assetBundle)
        {
            isDone = true;
            asset = null;
            if (assetBundle == null)
            {
                Debuger.LogError(string.Format("LoadAssetEnd: {0} type:RunAnimContLoad assetBundle == null!", url));
            }
            else
            {
                asset = assetBundle.mainAsset as RuntimeAnimatorController;
                if (asset == null)
                {
                    var racs = assetBundle.LoadAllAssets<RuntimeAnimatorController>();
                    if (racs != null && racs.Length != 0)
                        asset = racs[0];
                }

                if (asset == null)
                {
                    Debuger.ErrorLog("asset == null url:{0}", url);
                }

                assetBundle.Unload(false);
            }

            OnEnd();
        }

        // 回收自身
        protected override void FreeSelf()
        {
            FreeAsset<RunAnimContLoad>(this);
        }
    }
}
#endif