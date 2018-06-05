#if USE_RESOURCESEXPORT
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.Sprites;
using System.Collections.Generic;

class SpritePacker : IPackerPolicy
{
    protected class Entry
    {
        public int anisoLevel;
        public string atlasName;
        public SpritePackingMode packingMode;
        public AtlasSettings settings;
        public Sprite sprite;
    }

    private const uint kDefaultPaddingPower = 3;

    private SpritePackingMode GetPackingMode(string packingTag, SpriteMeshType meshType)
    {
        if ((meshType == SpriteMeshType.Tight) && (this.IsTagPrefixed(packingTag) == this.AllowTightWhenTagged))
        {
            return SpritePackingMode.Tight;
        }
        return SpritePackingMode.Rectangle;
    }

    public virtual int GetVersion() { return 1; }

    protected bool HasPlatformEnabledAlphaSplittingForCompression(string targetName, TextureImporter ti)
    {
        TextureImporterPlatformSettings platformTextureSettings = ti.GetPlatformTextureSettings(targetName);
        return (platformTextureSettings.overridden && platformTextureSettings.allowsAlphaSplitting);
    }

    protected bool IsTagPrefixed(string packingTag)
    {
        packingTag = packingTag.Trim();
        if (packingTag.Length < this.TagPrefix.Length)
        {
            return false;
        }
        return (packingTag.Substring(0, this.TagPrefix.Length) == this.TagPrefix);
    }

    static MethodInfo GetBuildTargetName = typeof(BuildPipeline).GetMethod("GetBuildTargetName", BindingFlags.Static | BindingFlags.NonPublic);
    static MethodInfo IsCompressedTextureFormat = typeof(BuildPipeline).Assembly.GetType("UnityEditor.TextureUtil").GetMethod("IsCompressedTextureFormat", BindingFlags.Static | BindingFlags.Public);
    
    public void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs)
    {
        List<Entry> list = new List<Entry>();
        string targetName = "";
        if (target != BuildTarget.NoTarget)
        {
            targetName = (string)GetBuildTargetName.Invoke(null, new object[] { target });
        }
        foreach (int num in textureImporterInstanceIDs)
        {
            TextureFormat format;
            ColorSpace space;
            int num3;
            TextureImporter ti = EditorUtility.InstanceIDToObject(num) as TextureImporter;
            ti.ReadTextureImportInstructions(target, out format, out space, out num3);
            TextureImporterSettings dest = new TextureImporterSettings();
            ti.ReadTextureSettings(dest);
            bool flag = (targetName != "") && this.HasPlatformEnabledAlphaSplittingForCompression(targetName, ti);
            Sprite[] spriteArray = Enumerable.Where<Sprite>(Enumerable.Select<UnityEngine.Object, Sprite>(AssetDatabase.LoadAllAssetRepresentationsAtPath(ti.assetPath), (UnityEngine.Object x)=> { return x as Sprite; }), (UnityEngine.Sprite x)=> { return x != null; }).ToArray<Sprite>();
            foreach (Sprite sprite in spriteArray)
            {
                Entry item = new Entry
                {
                    sprite = sprite,
                    settings =
                    {
                        format = format,
                        colorSpace = space,
                        compressionQuality = !((bool)IsCompressedTextureFormat.Invoke(null, new object[] { format})) ? 0 : num3,
                        filterMode = !Enum.IsDefined(typeof(UnityEngine.FilterMode), ti.filterMode) ? UnityEngine.FilterMode.Bilinear : ti.filterMode,
                        maxWidth = 0x800,
                        maxHeight = 0x800,
                        generateMipMaps = ti.mipmapEnabled,
                        enableRotation = this.AllowRotationFlipping,
                        allowsAlphaSplitting = flag
                    }
                };
                if (ti.mipmapEnabled)
                {
                    item.settings.paddingPower = 3;
                }
                else
                {
                    item.settings.paddingPower = (uint)EditorSettings.spritePackerPaddingPower;
                }
                item.atlasName = this.ParseAtlasName(ti.spritePackingTag);
                item.packingMode = this.GetPackingMode(ti.spritePackingTag, dest.spriteMeshType);
                item.anisoLevel = ti.anisoLevel;
                list.Add(item);
            }
            UnityEngine.Resources.UnloadAsset(ti);
        }

        IEnumerable<IGrouping<string, Entry>> enumerable = Enumerable.GroupBy<Entry, string, Entry>(list, (Entry e)=> { return e.atlasName; }, (Entry e)=> { return e; });
        foreach (IGrouping<string, Entry> grouping in enumerable)
        {
            int num5 = 0;
            IEnumerable<IGrouping<AtlasSettings, Entry>> source = Enumerable.GroupBy<Entry, AtlasSettings, Entry>(grouping, (Entry t)=> { return t.settings; }, (Entry t)=> { return t; });
            foreach (IGrouping<AtlasSettings, Entry> grouping2 in source)
            {
                string key = grouping.Key;
                if (source.Count<IGrouping<AtlasSettings, Entry>>() > 1)
                {
                    key = key + string.Format(" (Group {0})", num5);
                }
                AtlasSettings settings = grouping2.Key;
                settings.anisoLevel = 1;
                if (settings.generateMipMaps)
                {
                    foreach (Entry entry2 in grouping2)
                    {
                        if (entry2.anisoLevel > settings.anisoLevel)
                        {
                            settings.anisoLevel = entry2.anisoLevel;
                        }
                    }
                }

                var format = settings.format;
                OnAddAtlas(ref settings);
                job.AddAtlas(key, settings);
                Debug.LogFormat("name:{0} format:{1} -> {2}", key, format, settings.format);
                foreach (Entry entry3 in grouping2)
                {
                    job.AssignToAtlas(key, entry3.sprite, entry3.packingMode, SpritePackingRotation.None);
                }
                num5++;
            }
        }
    }

    protected virtual void OnAddAtlas(ref AtlasSettings settings)
    {

    }

    private string ParseAtlasName(string packingTag)
    {
        string str = packingTag.Trim();
        if (this.IsTagPrefixed(str))
        {
            str = str.Substring(this.TagPrefix.Length).Trim();
        }
        return ((str.Length != 0) ? str : "(unnamed)");
    }

    protected virtual bool AllowRotationFlipping { get { return false; } }

    protected virtual bool AllowTightWhenTagged { get { return true; } }

    protected virtual string TagPrefix { get { return "[TIGHT]"; } }
}

#endif