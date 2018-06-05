using UnityEngine;
using System.Collections.Generic;

#if USE_UATLAS
using ATLAS = UI.uAtlas;
#else
using ATLAS = xys.UI.Atlas;
#endif

namespace UI
{
    public class AtlasData
    {
        public ATLAS atlas;
        public Vector2 atlasSize;
        public int totalArea;

#if UNITY_EDITOR
        public List<string> spriteTags = new List<string>();
#endif

        public AtlasData(ATLAS a)
        {
            Reset(a);
        }

#if USE_UATLAS
        public void Reset(ATLAS a)
        {
            atlas = a;
            totalArea = 0;
            atlas.Init();
            foreach (var itor in a.Atlas)
            {
                foreach (var d in itor.sprites)
                {
                    totalArea += (int)(d.width * d.height);
                }
            }
        }
#else
        public void Reset(ATLAS a)
        {
            atlas = a;

            totalArea = 0;
            for (int i = 0; i < atlas.Sprites.Length; ++i)
            {
                if (atlas.Sprites[i] == null)
                    continue;

                Vector2 size = atlas.Sprites[i].rect.size;
                totalArea += (int)(size.x * size.y);

#if UNITY_EDITOR
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(atlas.Sprites[i]);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    UnityEditor.TextureImporter ti = UnityEditor.AssetImporter.GetAtPath(assetPath) as UnityEditor.TextureImporter;
                    if (ti != null && !spriteTags.Contains(ti.spritePackingTag))
                    {
                        spriteTags.Add(ti.spritePackingTag);
                    }
                }
#endif
            }

            //UITexturePacker.PackTextures(out atlasSize, atlas.Sprites, (Sprite s) => { return s == null ? Vector2.zero : s.rect.size; }, 512, 512, 1, 8192);
        }
#endif
        public int atlasArea
        {
            get { return (int)(atlasSize.x * atlasSize.y); }
        }

        static string V2ToString(Vector2 x)
        {
            return string.Format("({0},{1})", (int)x.x, (int)x.y);
        }

        public float usedPrec
        {
            get { return 1f * totalArea / atlasArea; }
        }

#if UNITY_EDITOR
        public override string ToString()
        {
            return string.Format("大小:({0}*{0})/{1} 利用率:{2}% total:{3} tag:{4}", (int)Mathf.Sqrt(totalArea), V2ToString(atlasSize), (100f * usedPrec).ToString("0.00"), atlas.Sprites.Length, spriteTags.Count);
        }

        public void BuildTexture(string assetPath = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                assetPath = UnityEditor.AssetDatabase.GetAssetPath(atlas);
                assetPath = assetPath.Substring(0, assetPath.LastIndexOf('.'));
            }
            else
                assetPath += atlas.name;

            CreateAtlas(atlas.Sprites, assetPath);
        }

        public static void CreateAtlas(Sprite[] sds, string assetPath)
        {
            System.Func<Sprite, Sprite> fun = (Sprite s) => { return s; };
            var atlas = BuildAtlas.BuildSprites(new List<Sprite>(sds), fun);
            for (int i = 0; i < atlas.Count; ++i)
            {
                Texture2D t2d = atlas[i].build(fun);
                string filename = assetPath + (i == 0 ? ".png" : (i + ".png"));
                System.IO.Directory.CreateDirectory(filename.Substring(0, filename.LastIndexOf('/')));
                System.IO.File.WriteAllBytes(filename, t2d.EncodeToPNG());
            }
        }
#endif
    }
}
