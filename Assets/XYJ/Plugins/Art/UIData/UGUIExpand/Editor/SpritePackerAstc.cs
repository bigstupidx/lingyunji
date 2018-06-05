#if USE_RESOURCESEXPORT && UNITY_IPHONE
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;
using System.Collections.Generic;
class SpritePackerAstc : SpritePacker
{
    protected override void OnAddAtlas(ref AtlasSettings settings)
    {
        switch (settings.format)
        {
        case TextureFormat.PVRTC_RGB2:
        case TextureFormat.PVRTC_RGB4:
            settings.format = TextureFormat.ASTC_RGB_4x4;
            break;
        case TextureFormat.PVRTC_RGBA2:
        case TextureFormat.PVRTC_RGBA4:
            settings.format = TextureFormat.ASTC_RGBA_4x4;
            break;
        case TextureFormat.RGBA32:
        case TextureFormat.RGB24:
            break;
        default:
            {
                if (settings.allowsAlphaSplitting)
                {
                    settings.format = TextureFormat.ASTC_RGBA_4x4;
                }
                else
                {
                    settings.format = TextureFormat.ASTC_RGB_4x4;
                }
            }
            break;
        }
    }
}

#endif