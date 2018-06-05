#if USE_UATLAS
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace UI
{
    public class uAtlas : MonoBehaviour
    {
        [System.Serializable]
        public struct Data
        {
            public string name;

            public short x;
            public short y;
            public short width;
            public short height;

            public Rect rect
            {
                get { return new Rect(x, y, width, height); }
            }
        }

        [System.Serializable]
        public struct AS
        {
            public Texture2D texture;
            public Data[] sprites;

#if UNITY_EDITOR
            [System.NonSerialized]
            public int type_sprite;

            int totalMemory;
            public int TotalMemory
            {
                get
                {
                    if (totalMemory == 0)
                    {
                        totalMemory = GUIEditor.GuiTools.TextureMemorySize(texture);
                    }

                    return totalMemory;
                }
            }
            public override string ToString()
            {
                int totalsize = 0;
                for (int i = 0; i < sprites.Length; ++i)
                {
                    Vector2 size = sprites[i].rect.size;
                    totalsize += (int)(size.x * size.y);
                }

                return string.Format("size:({0}*{1}) memory:{2} used:{3}%", texture.width, texture.height, XTools.Utility.ToMb(TotalMemory), (100.0f * totalsize / (texture.width * texture.height)).ToString("0.00"));
            }
#endif
        }

        [SerializeField]
        AS[] atlas;

        Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();

        public AS[] Atlas
        {
            get { return atlas; }
            set { atlas = value; }
        }

        public void Init()
        {
            if (atlas == null)
                return;

            if (mSprites.Count != 0)
                return;

            Vector2 pivot = Vector2.one * 0.5f;
            for (int i = 0; i < atlas.Length; ++i)
            {
                AS a = atlas[i];
                for (int j = 0; j < a.sprites.Length; ++j)
                {
                    Data d = a.sprites[j];
                    if (!mSprites.ContainsKey(d.name))
                    {
                        Sprite s = Sprite.Create(a.texture, d.rect, pivot);
                        s.name = d.name;
                        mSprites.Add(d.name, s);
                    }
                    else
                        Debug.LogErrorFormat("Sprite:{0} 重复!", d.name);
                }
            }
        }

        public void Add(AS a)
        {
            if (atlas == null)
            {
                atlas = new AS[] { a };
            }
            else
            {
                System.Array.Resize(ref atlas, atlas.Length + 1);
                atlas[atlas.Length - 1] = a;
            }
        }

        public Texture2D[] textures
        {
            get
            {
                Texture2D[] t2ds = new Texture2D[atlas.Length];
                for (int i = 0; i < atlas.Length; ++i)
                    t2ds[i] = atlas[i].texture;

                return t2ds;
            }
        }

#if UNITY_EDITOR
        public Sprite[] Sprites
        {
            get
            {
                Init();
                Sprite[] ss = new Sprite[mSprites.Count];
                int i = 0;
                foreach (var itor in mSprites)
                {
                    ss[i++] = itor.Value;
                }

                return ss;
            }
        }

        public void Write(BinaryWriter write)
        {
            write.Write(name);
            int total = 0;
            foreach (AS ass in Atlas)
                total += ass.sprites.Length;

            write.Write(total);
            SpriteData sd = new SpriteData();
            foreach (AS ass in Atlas)
            {
                foreach (Data n in ass.sprites)
                {
                    sd.Init(n.name, n.width, n.height);
                    sd.Write(write);
                }
            }
        }
#endif
    }
}
#endif