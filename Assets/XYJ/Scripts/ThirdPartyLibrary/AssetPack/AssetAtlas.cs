#if USE_ABL
using UnityEngine;
using System.Collections.Generic;

namespace ABL
{
    class AssetAtlas : MultiAsset
    {
        internal Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

        public AssetAtlas(string url) : base(url)
        {

        }

        protected override void OnAssetLoadEnd()
        {
            foreach (var sprite in ab.LoadAllAssets<Sprite>())
            {
                Sprites.Add(sprite.name, sprite);
            }
        }

        public Sprite Get(string name)
        {
            Sprite s = null;
            if (Sprites.TryGetValue(name, out s))
                return s;

            return null;
        }
    }
}
#endif