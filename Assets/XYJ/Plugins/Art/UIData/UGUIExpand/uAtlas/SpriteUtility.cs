#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public enum TypeSprite
{
    Compression = 1, // 压缩类型
    Truecolors = 2, // 真彩
    RGB = 4, // RGB类型
    ARGB = 8, // ARGB类型
}

public static class SpriteUtility
{
    static bool isARGB(TextureFormat format)
    {
        switch (format)
        {
        //case TextureFormat.Alpha8:
        //case TextureFormat.ARGB4444:
        case TextureFormat.RGB24:
        //case TextureFormat.RGBA32:
        //case TextureFormat.ARGB32:
        case TextureFormat.RGB565:
        case TextureFormat.DXT1:
        //case TextureFormat.DXT5:
        //case TextureFormat.RGBA4444:
        //case TextureFormat.BGRA32:
        case TextureFormat.PVRTC_RGB2:
        //case TextureFormat.PVRTC_RGBA2:
        //case TextureFormat.PVRTC_2BPP_RGBA:
        case TextureFormat.PVRTC_RGB4:
        //case TextureFormat.PVRTC_4BPP_RGBA:
        //case TextureFormat.PVRTC_RGBA4:
        case TextureFormat.ETC_RGB4:
        case TextureFormat.ATC_RGB4:
        //case TextureFormat.ATC_RGBA8:
        //case TextureFormat.ATF_RGB_DXT1:
        //case TextureFormat.ATF_RGBA_JPG:
        //case TextureFormat.ATF_RGB_JPG:
        case TextureFormat.EAC_R:
        case TextureFormat.EAC_R_SIGNED:
        case TextureFormat.EAC_RG:
        case TextureFormat.EAC_RG_SIGNED:
        case TextureFormat.ETC2_RGB:
        //case TextureFormat.ETC2_RGBA1:
        //case TextureFormat.ETC2_RGBA8:
        case TextureFormat.ASTC_RGB_4x4:
        case TextureFormat.ASTC_RGB_5x5:
        case TextureFormat.ASTC_RGB_6x6:
        case TextureFormat.ASTC_RGB_8x8:
        case TextureFormat.ASTC_RGB_10x10:
        case TextureFormat.ASTC_RGB_12x12:
            //case TextureFormat.ASTC_RGBA_4x4:
            //case TextureFormat.ASTC_RGBA_5x5:
            //case TextureFormat.ASTC_RGBA_6x6:
            //case TextureFormat.ASTC_RGBA_8x8:
            //case TextureFormat.ASTC_RGBA_10x10:
            //case TextureFormat.ASTC_RGBA_12x12:
            return false;
        }

        return true;
    }

    static bool isCompression(TextureFormat format)
    {
        switch (format)
        {
        case TextureFormat.ARGB32:
        case TextureFormat.RGBA32:
        case TextureFormat.RGB24:
            return false;
        default:
            return true;
        }
    }

    public static TextureImporterCompression GetTextureFormat(TypeSprite ts)
    {
        switch (ts)
        {
        case TypeSprite.Compression | TypeSprite.RGB: return TextureImporterCompression.CompressedHQ;
        case TypeSprite.Compression | TypeSprite.ARGB: return TextureImporterCompression.CompressedHQ;
        case TypeSprite.Truecolors | TypeSprite.RGB: return TextureImporterCompression.Uncompressed;
        case TypeSprite.Truecolors | TypeSprite.ARGB: return TextureImporterCompression.Uncompressed;
        }

        return TextureImporterCompression.Uncompressed;
    }

    public static TypeSprite GetTypeSprite(this Sprite sprite)
    {
        TextureFormat format;
        ColorSpace space;
        int num3;
        TextureImporter assetToUnload = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite)) as TextureImporter;
        assetToUnload.ReadTextureImportInstructions(EditorUserBuildSettings.activeBuildTarget, out format, out space, out num3);
        TextureImporterSettings dest = new TextureImporterSettings();
        assetToUnload.ReadTextureSettings(dest);

        return (isARGB(format) ? TypeSprite.ARGB : TypeSprite.RGB) | (isCompression(format) ? TypeSprite.Compression : TypeSprite.Truecolors);
    }
}
#endif