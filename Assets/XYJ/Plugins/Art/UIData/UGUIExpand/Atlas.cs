using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace xys.UI
{
    public class Atlas : MonoBehaviour
    {
        [SerializeField]
        public Sprite[] Sprites = new Sprite[0];

#if !USE_RESOURCESEXPORT
        public Dictionary<string, Sprite> SpriteMap = null;

        public void Init()
        {
            SpriteMap = new Dictionary<string, Sprite>();
            Sprite sprite = null;
            for (int i = 0; i < Sprites.Length; ++i)
            {
                sprite = Sprites[i];
                SpriteMap[sprite.name] = sprite;
            }
        }

        public Sprite Get(string name)
        {
            Sprite sprite = null;
            if (SpriteMap.TryGetValue(name, out sprite))
                return sprite;

            return null;
        }
#endif
#if UNITY_EDITOR
        public void Write(BinaryWriter write)
        {
            write.Write(name);
            write.Write(Sprites.Length);
            SpriteData sd = new SpriteData();
            for (int i = 0; i < Sprites.Length; ++i)
            {
                sd.Init(Sprites[i]);
                sd.Write(write);
            }
        }
#endif
    }
}
