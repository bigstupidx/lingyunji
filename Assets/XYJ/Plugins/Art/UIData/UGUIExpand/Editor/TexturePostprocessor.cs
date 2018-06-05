using UnityEngine;
using UnityEditor;
using System.IO;

public class TexturePostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.StartsWith("Assets/UIData/Data/GameIcon/"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            if (textureImporter.textureType != TextureImporterType.GUI)
            {
                textureImporter.textureType = TextureImporterType.GUI;
            }

            if (textureImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor)
            {
                textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            }
        }
    }
}