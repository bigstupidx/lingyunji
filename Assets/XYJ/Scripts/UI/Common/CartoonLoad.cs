using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WXB;

public class CartoonLoad
{
    static Dictionary<string, Cartoon> Cartoons = new Dictionary<string, Cartoon>();
    static List<Sprite> TempCartoons = new List<Sprite>();

    public static Cartoon Get(string name)
    {
        Cartoon t = null;
        if (Cartoons.TryGetValue(name, out t))
            return t;

        string prefix = string.Format("fb_{0}_", name);
        string fn = prefix + "1";
        Sprite sp = PackTool.SpritesLoad.has(fn);
        if (sp != null)
        {
            t = new Cartoon();
            Rect rect = sp.rect;
            t.width = (int)rect.width;
            t.height = (int)rect.height;

            TempCartoons.Add(sp);
            for (int i = 2; true; ++i)
            {
                sp = PackTool.SpritesLoad.has(prefix + i.ToString());
                if (sp != null)
                    TempCartoons.Add(sp);
                else
                    break;
            }

            t.name = name;
            t.sprites = TempCartoons.ToArray();
            TempCartoons.Clear();
            t.fps = 6f;

            Cartoons.Add(name, t);
        }

        return t;
    }
}
