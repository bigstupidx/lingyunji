#if USE_RESOURCESEXPORT && !UNITY_EDITOR
#define USE_AB
#endif

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class Texture2DAsset : ScriptableObject
{
#if !USE_AB
    public Texture text { get { return texture; } }

    [SerializeField]
    Texture2D texture;

    Color32[] color32s;

    public void Cache()
    {
        if (texture == null)
        {
            Width = 0;
            Height = 0;
            color32s = null;
        }
        else
        {
            if (Width == 0) Width = texture.width;
            if (Height == 0) Height = texture.height;
            if (color32s == null) color32s = texture.GetPixels32();
        }
    }
#endif

    int Width { get; set; }
    int Height { get; set; }

    public int width
    {
        get
        {
#if !USE_AB
            if (texture != null)
            {
                if (Width != 0)
                    return Width;
                else
                {
                    Width = texture.width;
                }

                return Width;
            }
#endif

#if USE_RESOURCESEXPORT
            return Width;
#else
            return 0;
#endif
        }
    }

    public int height
    {
        get
        {
#if !USE_AB
            if (texture != null)
            {
                if (Height != 0)
                    return Height;
                else
                {
                    Height = texture.height;
                }

                return Height;
            }
#endif
#if USE_RESOURCESEXPORT
            return Height;
#else
            return 0;
#endif
        }
    }

    public Color32[] GetPixels32()
    {
        Color32[] color32 = new Color32[width * height];
        GetPixels32(ref color32);
        return color32;
    }

    public void GetPixels32(ref Color32[] color32s)
    {
#if !USE_AB
        if (texture != null)
        {
            Color32[] src = this.color32s == null ? texture.GetPixels32() : this.color32s;
            if (color32s == null || color32s.Length < src.Length)
                color32s = new Color32[src.Length];

            for (int i = 0; i < src.Length; ++i)
                color32s[i] = src[i];
        }
#endif

#if USE_RESOURCESEXPORT
        ReadColor32(ref color32s);
#endif
    }

#if USE_RESOURCESEXPORT
    BinaryReader reader = null;

    public void Init(BinaryReader reader)
    {
        this.reader = reader;
        Width = reader.ReadInt16();
        Height = reader.ReadInt16();
    }

#if ASSET_DEBUG
        public string url;
        static PackTool.TimeTrack AD_ReadColor32 = PackTool.TimeTrackMgr.Instance.Get("Texture2DAsset.ReadColor32");
#endif

    static byte[] bytes = new byte[2048];

#if COM_DEBUG
    static long total_acc = 0;
#endif

    void ReadColor32(ref Color32[] color32s)
    {
        int count = Width * Height;
        if (color32s == null || color32s.Length < count)
        {
            color32s = new Color32[count];

#if COM_DEBUG
            total_acc += count;
            Debuger.DebugLog("ReadColor32 src:{0} w:{1} h:{2} total:{3}", 
                color32s == null ? 0 : color32s.Length, 
                width, 
                height, 
                XTools.Utility.ToMb(total_acc));
#endif
        }

#if ASSET_DEBUG
        AD_ReadColor32.Execution(url, () =>
        {
#endif
            Color32 c;
            long startpos = reader.BaseStream.Position;
            int total = count;
            int current = 0;
            int pos = 0;
            while (total > 0)
            {
                count = reader.Read(bytes, 0, 2048) / 4;
                total -= count;
                for (int i = 0; i < count; ++i)
                {
                    pos = i * 4;
                    c.r = bytes[pos];
                    c.g = bytes[pos + 1];
                    c.b = bytes[pos + 2];
                    c.a = bytes[pos + 3];

                    color32s[current++] = c;
                }
            }

            reader.BaseStream.Position = startpos;
#if ASSET_DEBUG
        });
#endif
    }
#endif

#if UNITY_EDITOR
    [MenuItem("Assets/Textures/创建可读纹理资源")]
    public static void Create()
    {
        XTools.Utility.ForEachSelect((TextureImporter ti) =>
        {
            if (!ti.isReadable)
                return;
            GetOrCreate(AssetDatabase.LoadAssetAtPath<Texture2D>(ti.assetPath));
        },
        (string assetPath, string root) => 
        {
            return assetPath.EndsWith("tga");
        });
    }

    static Texture2DAsset GetOrCreate(Texture2D t2d)
    {
        string t2dAssetPath = AssetDatabase.GetAssetPath(t2d);
        if (!t2dAssetPath.StartsWith("Assets/"))
            return null;

        string assetPath = t2dAssetPath;
        assetPath = assetPath.Substring(0, assetPath.LastIndexOf('/')) + "/t/";
        System.IO.Directory.CreateDirectory(assetPath);

        assetPath = assetPath + System.IO.Path.GetFileNameWithoutExtension(t2dAssetPath) + ".asset";
        var t2 = AssetDatabase.LoadAssetAtPath<Texture2DAsset>(assetPath);
        if (t2 != null)
        {
            if (t2.texture == t2d)
                return t2;

            AssetDatabase.DeleteAsset(assetPath);
        }

        t2 = CreateInstance<Texture2DAsset>();
        t2.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(t2dAssetPath);

        AssetDatabase.CreateAsset(t2, assetPath);
        AssetDatabase.Refresh();

        return AssetDatabase.LoadAssetAtPath<Texture2DAsset>(t2dAssetPath);
    }

    public static Texture2DAsset Get(Texture2D t2d)
    {
        return GetOrCreate(t2d);
    }

    public void Export(string file)
    {
        if (File.Exists(file))
            File.Delete(file);

        var fs = File.Open(file, FileMode.CreateNew);
        BinaryWriter writer = new BinaryWriter(fs);
        writer.Write((ushort)texture.width);
        writer.Write((ushort)texture.height);
        Color32[] colors = texture.GetPixels32();
        for (int i = 0; i < colors.Length; ++i)
        {
            Color32 c = colors[i];
            writer.Write(c.r);
            writer.Write(c.g);
            writer.Write(c.b);
            writer.Write(c.a);
        }

        writer.Close();
        fs.Close();
    }
#endif
}