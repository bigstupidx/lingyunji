#if UNITY_EDITOR
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public class TextureExport
    {
        static public void ETCAlphaTexture(TextureImporter textureImporter, int maxsize, TextureImporterFormat format = TextureImporterFormat.RGB24)
        {
            bool isdirty = false;
#if UNITY_5_5
            if (textureImporter.textureType != TextureImporterType.Default)
            {
                textureImporter.textureType = TextureImporterType.Default;
                isdirty = true;
            }
#else
            if (textureImporter.textureType != TextureImporterType.Default)
            {
                textureImporter.textureType = TextureImporterType.Default;
                isdirty = true;
            }
#endif

            if (textureImporter.maxTextureSize != maxsize)
            {
                textureImporter.maxTextureSize = maxsize;
                isdirty = true;
            }

            // RGB格式
            if (Set(textureImporter, AndroidPlatform(), format, maxsize))
                isdirty = true;

            // RGB格式
            if (Set(textureImporter, iPhonePlatform(), format, maxsize))
                isdirty = true;

            if (textureImporter.mipmapEnabled != false)
            {
                textureImporter.mipmapEnabled = false;
                isdirty = true;
            }

            if (textureImporter.borderMipmap != false)
            {
                textureImporter.borderMipmap = false;
                isdirty = true;
            }

            if (isdirty)
            {
                AssetDatabase.ImportAsset(textureImporter.assetPath);
            }
        }
        static string AndroidPlatform()
        {
            return "Android";
        }

        public static string iPhonePlatform()
        {
            return "iPhone";
        }

        static void GetCurrentPlatformTextureSettings(TextureImporter importer, out int maxTextureSize, out TextureImporterFormat textureFormat)
        {
            string platform = "Standalone";
#if UNITY_ANDROID
            platform = "Android";
#elif UNITY_IPHONE
            platform = "iPhone";
#endif
            importer.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat);
        }

        static void SetCurrentPlatformTextureSettings(TextureImporter importer, int maxTextureSize, TextureImporterFormat textureFormat)
        {
            string platform = "Standalone";
#if UNITY_ANDROID
            platform = "Android";
#elif UNITY_IPHONE
            platform = "iPhone";
#endif
            importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings() { maxTextureSize = maxTextureSize, format = textureFormat, name = platform });
        }

        public static bool isARGB(TextureFormat format)
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
            case TextureFormat.EAC_R :
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

        public static bool Set(TextureImporter textureImporter, string platform, TextureImporterFormat format, int maxSize = -1)
        {
            return GUIEditor.GuiTools.Set(textureImporter, platform, format, maxSize);
        }

        //[MenuItem("Assets/纹理/统一源大小一半")]
        //static void Scale()
        //{
        //    GlobalCoroutine.StartCoroutine(ScaleAsync());
        //}

        static System.Collections.IEnumerator ScaleAsync()
        {
            List<string> dirtys = new List<string>();
            var itor = Utility.ForEachSelectASync((TextureImporter ti) =>
            {
                Texture2D t2d = AssetDatabase.LoadAssetAtPath<Texture2D>(ti.assetPath);
                if (t2d != null)
                {
                    int s = 0;
                    if (!ti.assetPath.EndsWith(".exr"))
                    {
                        s = GetSourceTextureSize(ti) / 2;
                    }
                    else
                    {
                        s = 2048 / 2;
                    }

                    int size = Mathf.Max(t2d.width, t2d.height);
                    if (size > s)
                    {
                        Debug.LogFormat("s:{0} {1}->{2}", ti.assetPath, size, s);

                        ti.maxTextureSize = s;
                        dirtys.Add(ti.assetPath);
                    }
                }
            });

            while (itor.MoveNext())
                yield return 0;

            Debug.LogFormat("total:{0}", dirtys.Count);

            for (int i = 0; i < dirtys.Count; ++i)
            {
                AssetDatabase.ImportAsset(dirtys[i]);
            }
        }

        static int GetSourceTextureSize(TextureImporter ti)
        {
            return GUIEditor.GuiTools.GetSourceMaxSize(ti);
        }
    }
}
#endif