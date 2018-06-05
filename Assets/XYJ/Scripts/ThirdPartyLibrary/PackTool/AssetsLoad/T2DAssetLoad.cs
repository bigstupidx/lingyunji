#if USE_RESOURCESEXPORT
using XTools;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 加载基础资源
namespace PackTool
{
    public class T2DAssetLoad : AssetLoad<Texture2DAsset>
    {
        static public T2DAssetLoad.Data Load(string name, ResourcesEnd<Texture2DAsset> fun, object funp)
        {
            return T2DAssetLoad.LoadImp(name, fun, funp, Create);
        }

        static T2DAssetLoad Create(string name)
        {
            return CreateAsset<T2DAssetLoad>(name);
        }

        protected override void LoadAsset(string name)
        {
            System.IO.Stream stream = ResourcesPack.FindBaseStream(name);
            if (stream == null || stream.Length == 0)
            {
                asset = null;
                NextFrame();
                return;
            }

            BinaryReader reader = new BinaryReader(stream);
            asset = ScriptableObject.CreateInstance<Texture2DAsset>();
#if ASSET_DEBUG
            asset.url = name;
#endif
            asset.name = Path.GetFileNameWithoutExtension(name);
            asset.Init(reader);
            NextFrame();
        }

        protected override void FreeSelf()
        {
            FreeAsset<T2DAssetLoad>(this);
        }
    }
}
#endif