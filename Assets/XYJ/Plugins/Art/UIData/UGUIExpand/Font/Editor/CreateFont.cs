using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
#pragma warning disable 618

public class CreateFont
{
    [MenuItem("Assets/uGUI/创建艺术字")]
    static void Create()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        string[] guids = AssetDatabase.FindAssets("", new string[] { assetPath });

        List<CharacterInfo> infos = new List<CharacterInfo>();
        List<Texture2D> textures = new List<Texture2D>();
        int maxHeight = 0;
        for (int i = 0; i < guids.Length; ++i)
        {
            string ap = AssetDatabase.GUIDToAssetPath(guids[i]);
            Texture2D t2d = AssetDatabase.LoadAssetAtPath<Texture2D>(ap);
            if (t2d != null && !textures.Contains(t2d))
            {
                textures.Add(t2d);

                CharacterInfo info = new CharacterInfo();
                info.advance = t2d.width;
                info.glyphWidth = t2d.width;
                info.glyphHeight = t2d.height;
                info.vert = new Rect(0, 0, t2d.width, -t2d.height);

                info.index = t2d.name[0];
                infos.Add(info);

                maxHeight = Mathf.Max(maxHeight, t2d.height);
            }
        }

        if (infos.Count == 0)
            return;

        string fontassetpath = assetPath;
        BitmapFont font = AssetDatabase.LoadAssetAtPath<BitmapFont>(fontassetpath + ".prefab");
        if (font == null)
        {
            GameObject go = new GameObject(System.IO.Path.GetFileNameWithoutExtension(assetPath));
            go.AddComponent<BitmapFont>();

            Font rf = new Font();
            AssetDatabase.CreateAsset(rf, fontassetpath + ".fontsettings");
            AssetDatabase.Refresh();

            go.GetComponent<BitmapFont>().font = AssetDatabase.LoadAssetAtPath<Font>(fontassetpath + ".fontsettings");

            PrefabUtility.CreatePrefab(fontassetpath + ".prefab", go);
            Object.DestroyImmediate(go);

            Material material = new Material(Shader.Find("GUI/Text Shader"));
            AssetDatabase.CreateAsset(material, fontassetpath + ".mat");
            AssetDatabase.Refresh();

            font = AssetDatabase.LoadAssetAtPath<BitmapFont>(fontassetpath + ".prefab");
            font.font.material = material;

            Texture2D t2d = new Texture2D(128,128);
            AssetDatabase.CreateAsset(t2d, fontassetpath + ".png");
            AssetDatabase.Refresh();

            font.font.material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>(fontassetpath + ".png");

            AssetDatabase.Refresh();
        }

        Dictionary<int, CharacterInfo> olds = new Dictionary<int, CharacterInfo>();
        CharacterInfo[] o = font.font.characterInfo;
        foreach (var ci in o)
        {
            olds.Add(ci.index, ci);
        }

        Texture2D dst = new Texture2D(256, 256);
        Rect[] rects = dst.PackTextures(textures.ToArray(), 1);
        CharacterInfo[] infoss = infos.ToArray();
        for (int i = 0; i < rects.Length; ++i)
        {
            CharacterInfo c;
            if (olds.TryGetValue(infoss[i].index, out c))
            {
                infoss[i].vert = c.vert;
            }

            infoss[i].uv = rects[i];
            maxHeight = Mathf.Max((int)rects[i].height, maxHeight);
        }

        font.font.characterInfo = infoss;
        if (System.IO.File.Exists(fontassetpath + ".png"))
            System.IO.File.Delete(fontassetpath + ".png");
        System.IO.File.WriteAllBytes(fontassetpath + ".png", dst.EncodeToPNG());
        AssetDatabase.Refresh();

        font.font.material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture>(fontassetpath + ".png");

        EditorUtility.SetDirty(font.font);
        EditorUtility.SetDirty(font);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        string key = "m_LineSpacing: ";
        string fonttext = System.IO.File.ReadAllText(fontassetpath + ".fontsettings");
        int pos = fonttext.IndexOf(key) + key.Length;
        int endpos = fonttext.IndexOfAny(new char[] { '\n', '\r', }, pos);
        string lv = fonttext.Substring(pos, endpos - pos);
        int lineheight = (int)float.Parse(lv);
        
        if (lineheight != maxHeight)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(fonttext.Substring(0, pos));
            sb.Append(maxHeight.ToString());
            sb.Append(fonttext.Substring(endpos));

            System.IO.File.WriteAllText(fontassetpath + ".fontsettings", sb.ToString());
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
}
