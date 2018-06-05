#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace UI
{
    public class BuildAtlas
    {
        static List<T> SortSprites<T>(List<T> sprites, System.Func<T, Sprite> fun)
        {
            List<T> copys = new List<T>(sprites);

            copys.Sort((T x, T y) => { return Area(fun(x)).CompareTo(Area(fun(y))); });

            return copys;
        }

        static int GetTotalSize<T>(IList<T> sprites, System.Func<T, Sprite> fun)
        {
            int total = 0;
            for (int i = 0; i < sprites.Count; ++i)
            {
                Vector2 size = fun(sprites[i]).rect.size;
                total += (int)((size.x + 1) * (size.y + 1));
            }

            return total;
        }

        static int GetMinSize<T>(IList<T> sprites, System.Func<T, Sprite> fun)
        {
            float h = 0;
            for (int i = 0; i < sprites.Count; ++i)
            {
                Vector2 size = fun(sprites[i]).rect.size;
                h = Mathf.Max(h, size.x);
                h = Mathf.Max(h, size.y);
            }

            return (int)h;
        }

        static int T2(int size)
        {
            int v = 1;
            while (v < size)
                v *= 2;

            return v;
        }

        public struct BuildData<T>
        {
            public Vector2 size;

            public T[] sprites;
            public Rect[] rects;

            public TypeSprite type_sprite;

            public float used
            {
                get
                {
                    float total = 0;
                    for (int i = 0; i < rects.Length; ++i)
                    {
                        total += (rects[i].width + 1) * (rects[i].height + 1);
                    }

                    return total / (size.x * size.y);
                }
            }

            public Texture2D build(System.Func<T, Sprite> fun)
            {
                TextureFormat format = TextureFormat.RGB24;
                for (int i = 0; i < sprites.Length; ++i)
                {
                    if (PackTool.TextureExport.isARGB(fun(sprites[i]).texture.format))
                    {
                        format = TextureFormat.ARGB32;
                        break;
                    }
                }

                Texture2D t2d = new Texture2D((int)size.x, (int)size.y, format, false);
                Color c = new Color(0f, 0f, 0f, 0f);
                for (int i = 0; i < t2d.height; ++i)
                {
                    for (int j = 0; j < t2d.width; ++j)
                        t2d.SetPixel(j, i, c);
                }

                for (int i = 0; i < sprites.Length; ++i)
                {
                    Rect rect = rects[i];
                    //rect = new Rect(rect.x * size.x, rect.y * size.y, rect.width * size.x, rect.height * size.y);
                    Sprite sprite = fun(sprites[i]);
                    try
                    {
                        t2d.SetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, GetSpriteColor(sprite));
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                        Debug.LogFormat("rect:{0} size:{1}", rect, size);
                        break;
                    }
                }

                t2d.Apply();
                return t2d;
            }

#if USE_UATLAS
            public uAtlas.AS Build(System.Func<T, Sprite> fun)
            {
                uAtlas.AS a = new uAtlas.AS();
                a.texture = build(fun);
                a.sprites = new uAtlas.Data[rects.Length];
                for (int i = 0; i < a.sprites.Length; ++i)
                {
                    a.sprites[i].name = fun(sprites[i]).name;
                    a.sprites[i].x = (short)rects[i].x;
                    a.sprites[i].y = (short)rects[i].y;
                    a.sprites[i].width = (short)rects[i].width;
                    a.sprites[i].height = (short)rects[i].height;
                }

                return a;
            }
#endif
        }

#if USE_UATLAS
        static public uAtlas BuilduAtlas(Sprite[] sprites)
        {
            // 精灵的分类
            Dictionary<TypeSprite, List<Sprite>> SpriteFlags = new Dictionary<TypeSprite, List<Sprite>>();
            for (int i = 0; i < sprites.Length; ++i)
            {
                TypeSprite ts = sprites[i].GetTypeSprite();
                List<Sprite> ss = null;
                if (!SpriteFlags.TryGetValue(ts, out ss))
                {
                    ss = new List<Sprite>();
                    SpriteFlags.Add(ts, ss);
                }

                ss.Add(sprites[i]);
            }

            var results = new List<BuildData<Sprite>>();
            foreach (var itor in SpriteFlags)
            {
                var result = BuildSprites(itor.Value, (Sprite s) => { return s; });
                if (result == null)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.AppendLine(string.Format("BuildSprites null! {0}", itor.Value.Count));
                    for (int i = 0; i < itor.Value.Count; ++i)
                    {
                        sb.AppendLine(string.Format("({0}):{1}", i, itor.Value[i].name));
                    }

                    Debug.LogError(sb.ToString());
                    continue;
                }
                for(int i = 0; i < result.Count; ++i)
                {
                    BuildData<Sprite> bd = result[i];
                    bd.type_sprite = itor.Key;
                    result[i] = bd;
                }

                results.AddRange(result);
            }

            GameObject go = new GameObject();
            uAtlas atlas = go.AddComponent<uAtlas>();
            for (int i = 0; i < results.Count; ++i)
            {
                var aas = results[i].Build((Sprite s) => { return s; });
                aas.type_sprite = (int)results[i].type_sprite;
                atlas.Add(aas);
            }

            return atlas;
        }
#endif
        static public List<BuildData<T>> BuildSprites<T>(List<T> sprites, System.Func<T, Sprite> fun)
        {
            int bestsize = (int)Mathf.Sqrt(GetTotalSize(sprites, fun)) + 1;

            // 分析下，尽可能达到图集利用率最优
            int t2v = T2(bestsize);
            if (t2v * t2v * s_used > bestsize)
            {
                // 利用率没有达到,要拆分下
                t2v = t2v / 2;
            }
            t2v = Mathf.Max(t2v, T2(GetMinSize(sprites, fun)));
            t2v = Mathf.Min(t2v, 2048);

            while (true)
            {
                List<T> copys = SortSprites(sprites, fun);
                List<T> sps = new List<T>(copys);
                List<T> nosps = new List<T>();
                List<BuildData<T>> results = new List<BuildData<T>>();
                while (sps.Count != 0)
                {
                    BuildData<T> bd;
                    if (Build(sps, nosps, t2v, out bd, fun))
                    {
                        results.Add(bd);
                        sps = nosps;
                        nosps = new List<T>();
                    }
                    else
                    {
                        break;
                    }
                }

                if (results.Count == 0)
                    t2v *= 2;
                else
                    return results;

                if (t2v > 2048)
                    break;
            }

            return null;
        }

        static float s_used = 0.8f;

        static bool Build<T>(List<T> sps, List<T> rnosps, int max, out BuildData<T> result, System.Func<T, Sprite> fun)
        {
            result = new BuildData<T>();
            List<T> nosps_temp = new List<T>(rnosps);
            bool ishas = false;

            Vector2 size = Vector2.zero;
            sps = new List<T>(sps);
            //int count = sps.Count;
            bool issmall = false;
            int totalmax = sps.Count * 100;
            while ((totalmax--) >= 0)
            {
                int minV = T2(GetMinSize(sps, fun));
                Rect[] rects = UITexturePacker.PackTextures(out size, sps, (T s) => { return fun(s).rect.size; }, 256, 256, 1, max);
                if (rects == null || rects.Length == 0)
                {
                    if (issmall)
                    {
                        if (nosps_temp.Count >= 2 && result.used < s_used)
                        {
                            // 利用率过小
                            nosps_temp.Add(sps[sps.Count - 1]); // 把最大的移除
                            sps.RemoveAt(sps.Count - 1);
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    if (sps.Count <= 1)
                        return false;

                    int totalsize = GetTotalSize(sps, fun);
                    do
                    {
                        T t = sps[0];
                        nosps_temp.Add(t);
                        sps.RemoveAt(0);

                        Vector2 spssize = fun(t).rect.size;

                        totalsize -= (int)(spssize.x * spssize.y);
                    }
                    while ((totalsize >= max * max * 0.9f && sps.Count > 0 && size.x > minV));
                }
                else
                {
                    if (!ishas)
                    {
                        result.sprites = sps.ToArray();
                        result.rects = rects;
                        result.size = size;
                        ishas = true;

                        rnosps.Clear();
                        rnosps.AddRange(nosps_temp);
                    }
                    else
                    {
                        var bdr = new BuildData<T>();
                        bdr.sprites = sps.ToArray();
                        bdr.rects = rects;
                        bdr.size = size;

                        if (bdr.used > result.used)
                        {
                            result = bdr;

                            rnosps.Clear();
                            rnosps.AddRange(nosps_temp);
                        }
                    }

                    if (nosps_temp.Count == 0)
                    {
                        if (max == minV)
                            return true;

                        if (result.used <= 0.5f && max >= 2048)
                        {
                            if (sps.Count == 1)
                            {
                                //result = new BuildData<T>();
                                //Sprite s = fun(sps[0]);
                                //result.size = s.rect.size;
                                //result.rects = new Rect[1] { new Rect(0, 0, result.size.x, result.size.y), };
                                //result.sprites = sps.ToArray();

                                return true;
                            }

                            max /= 2;
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    if (issmall && result.used >= s_used)
                        return true;

                    // 可以添加进去，再挤挤，从最小的再往里面放
                    issmall = true;

                    while (nosps_temp.Count != 0)
                    {
                        sps.Insert(0, nosps_temp[0]);
                        nosps_temp.RemoveAt(0);

                        float total = GetTotalSize(sps, fun);
                        float used = total / (max * max);
                        if (used >= s_used)
                            break;
                    }
                }
            }

            return ishas;
        }

        static int Area(Sprite s)
        {
            Vector2 size = s.rect.size;
            return (int)(size.x * size.y);
        }

        public static Texture2D Build(Sprite[] sprites)
        {
            sprites = SortSprites(new List<Sprite>(sprites), (Sprite s)=> { return s; }).ToArray();

            List<Vector2> sizes = new List<Vector2>();
            for (int i = 0; i < sprites.Length; ++i)
                sizes.Add(sprites[i].rect.size);

            Vector2 size = Vector2.zero;
            Rect[] rects = UITexturePacker.PackTextures(out size, sizes.ToArray(), 256, 256, 1, 2048);
            if (rects == null)
                return null;

            Texture2D t2d = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false);
            Color c = new Color(0, 0, 0, 0);
            for (int i = 0; i < t2d.height; ++i)
            {
                for (int j = 0; j < t2d.width; ++j)
                    t2d.SetPixel(j, i, c);
            }

            for (int i = 0; i < sprites.Length; ++i)
            {
                Rect rect = new Rect(rects[i].x * size.x, rects[i].y * size.y, rects[i].width * size.x, rects[i].height * size.y);
                Sprite sprite = sprites[i];
                t2d.SetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, GetSpriteColor(sprite));
            }

            t2d.Apply();
            return t2d;
        }

        static Texture2D Build(Vector2 size, Dictionary<Sprite, Rect> sprites)
        {
            Texture2D t2d = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false);
            Color c = new Color(0, 0, 0, 0);
            for (int i = 0; i < t2d.height; ++i)
            {
                for (int j = 0; j < t2d.width; ++j)
                    t2d.SetPixel(j, i, c);
            }

            foreach(var itor in sprites)
            {
                Rect rect = itor.Value;
                rect = new Rect(rect.x * size.x, rect.y * size.y, rect.width * size.x, rect.height * size.y);
                Sprite sprite = itor.Key;
                t2d.SetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, GetSpriteColor(sprite));
            }

            t2d.Apply();
            return t2d;
        }

        static Color[] GetSpriteColor(Sprite s)
        {
            Rect rect = s.rect;
            int x = (int)rect.x;
            int y = (int)rect.y;
            int width = (int)rect.width;
            int height = (int)rect.height;

            try
            {
                return s.texture.GetPixels(x, y, width, height);
            }
            catch (System.Exception)
            {
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(s.texture);
                Texture2D t2d = new Texture2D(1, 1);
                t2d.LoadImage(System.IO.File.ReadAllBytes(assetPath));

                Color[] cols;
                if (t2d.width != width || t2d.height != height)
                {
                    float u = 1f / t2d.width;
                    float v = 1f / t2d.height;

                    x = (int)((1f * x / s.texture.width) * t2d.width);
                    x = (int)((1f * y / s.texture.height)* t2d.height);

                    cols = new Color[(int)(width * height)];
                    for (int i = 0; i < height; ++i)
                    {
                        for (int j = 0; j < width; ++j)
                        {
                            int pos = (int)(i * width + j);
                            cols[pos] = t2d.GetPixelBilinear((x + j) * u, (y + i) * v);
                        }
                    }
                }
                else
                {
                    cols = t2d.GetPixels(x, y, width, height);
                }

                Object.DestroyImmediate(t2d);

                return cols;
            }

        }
    }
}
#endif