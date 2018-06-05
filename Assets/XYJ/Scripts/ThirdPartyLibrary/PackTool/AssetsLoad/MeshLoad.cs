#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XTools;

// 加载基础资源
namespace PackTool
{
    public class MeshLoad : AssetLoad<Mesh>
    {
        static public void Load(string name, ResourcesEnd<Mesh> fun, object funp)
        {
            MeshLoad.LoadImp(name, fun, funp, Create);
        }

        static MeshLoad Create(string url)
        {
            return CreateAsset<MeshLoad>(url);
        }

        protected override void LoadAsset(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debuger.ErrorLog("MeshLoad is empty!");
                NextFrame();
            }
            else
            {
                if (name.LastIndexOf('.') == -1)
                {
                    asset = BuiltinResource.Instance.GetMesh(name);
                    AddRef(); // 内置资源不卸载

                    if (asset == null)
                    {
                        Debuger.ErrorLog("MeshLoad:{0} not find!", name);
                    }

                    NextFrame();
                }
                else
                {
                    AssetBundleLoad.Load(name, LoadAssetEnd);
                }
            }
        }

        void LoadAssetEnd(AssetBundle assetBundle)
        {
            isDone = true;
            asset = null;
            if (assetBundle == null)
            {
                Debuger.LogError(string.Format("MeshLoad: {0} assetBundle == null!", url));
            }
            else
            {
                asset = assetBundle.mainAsset as Mesh;
                assetBundle.Unload(false);
            }

            OnEnd();
        }

        protected override void FreeSelf()
        {
            FreeAsset<MeshLoad>(this);
        }
    }
}
#endif