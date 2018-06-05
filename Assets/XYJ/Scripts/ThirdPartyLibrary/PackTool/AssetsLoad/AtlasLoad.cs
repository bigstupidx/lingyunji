#if USE_RESOURCESEXPORT
using XTools;
using UnityEngine;
using UI;
using System.Collections.Generic;

// 加载基础资源
namespace PackTool
{
    public class AtlasLoad : AssetLoad<xys.UI.Atlas>
    {
        static public Data Load(string name, ResourcesEnd<xys.UI.Atlas> fun, object funp)
        {
            return LoadImp(name, fun, funp, Create);
        }

        static public Data Load(int nameHashcode, ResourcesEnd<xys.UI.Atlas> fun, object funp)
        {
            string name = StringHashCode.Get(nameHashcode);
            if (name == null)
                return null;

            return LoadImp(name, fun, funp, Create);
        }

        static AtlasLoad Create(string name)
        {
            return CreateAsset<AtlasLoad>(name);
        }

        protected override void LoadAsset(string name)
        {
            AssetBundleLoad.Load(name, LoadAssetEnd);
        }

        List<Texture> mTextures = new List<Texture>();

        void LoadAssetEnd(AssetBundle assetBundle)
        {
            isDone = true;
            if (assetBundle == null)
            {
                Debug.LogError(string.Format("LoadAssetEnd: {0} assetBundle == null!", url));
            }
            else
            {
                asset = assetBundle.mainAsset as xys.UI.Atlas;
                mTextures.AddRange(assetBundle.LoadAllAssets<Texture>());
                assetBundle.Unload(false);
            }

            AddRef();
            OnEnd();
        }

        public List<Texture> Textures
        {
            get
            {
                return mTextures;
            }
        }

        public int GetAllTextures(List<Texture> textures)
        {
            int total = 0;
            textures.AddRange(mTextures);
            Sprite[] sprites = asset.Sprites;
            for (int i = 0; i < sprites.Length; ++i)
            {
                if (!textures.Contains(sprites[i].texture))
                    textures.Add(sprites[i].texture);
            }

            for (int i = 0; i < textures.Count; ++i)
            {
                if (textures[i] == null)
                    continue;

                total += (textures[i].width * textures[i].height);
            }

            return (int)Mathf.Sqrt(total);
        }

        protected override bool DestroyImp()
        {
            if (asset != null)
            {
                HashSet<Texture> texts = new HashSet<Texture>();
                Sprite[] sprites = asset.Sprites;
                Sprite s = null;
                for (int i = 0; i < sprites.Length; ++i)
                {
                    s = sprites[i];
                    texts.Add(s.texture);
                    Object.DestroyImmediate(s, true);
                }

                foreach (Texture text in texts)
                {
                    Object.DestroyImmediate(text, true);
                }

                texts.Clear();
                Object.DestroyImmediate(asset.gameObject, true);
                asset = null;
            }

            RemoveKey(url);
            return true;
        }

        protected override void FreeSelf()
        {
            mTextures.Clear();
            FreeAsset(this);
        }
    }
}
#endif