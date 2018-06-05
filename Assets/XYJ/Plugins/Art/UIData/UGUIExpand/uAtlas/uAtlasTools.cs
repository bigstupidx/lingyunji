#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace xys.UI
{
    public static class uAtlasTools
    {
#if USE_UATLAS
        public static uAtlas CreateuAtlas(Sprite[] sds, string assetPath)
        {
            uAtlas atlas = BuildAtlas.BuilduAtlas(sds);
            if (atlas == null)
            {
                Debug.LogFormat("图集过大，打包失败!");
            }
            else
            {
                Debug.LogFormat("CreateuAtlas:{0}", assetPath);
                uAtlas go = GetOrCreate<uAtlas>(assetPath);
                go.Atlas = atlas.Atlas;
                Object.DestroyImmediate(atlas.gameObject);

                string path = assetPath.Substring(0, assetPath.LastIndexOf('.'));
                for (int i = 0; i < go.Atlas.Length; ++i)
                {
                    string tp = string.Format("{0}{1}.png", path, i + 1);
                    if (File.Exists(tp))
                        File.Delete(tp);

                    File.WriteAllBytes(tp, go.Atlas[i].texture.EncodeToPNG());
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                    Object.DestroyImmediate(go.Atlas[i].texture);

                    go.Atlas[i].texture = AssetDatabase.LoadAssetAtPath<Texture2D>(tp); ;

                    TextureImporter ti = AssetImporter.GetAtPath(tp) as TextureImporter;
                    ti.alphaIsTransparency = true;
                    ti.textureCompression = SpriteUtility.GetTextureFormat((TypeSprite)go.Atlas[i].type_sprite);
                    ti.mipmapEnabled = false;
                    EditorUtility.SetDirty(ti);
                    AssetDatabase.ImportAsset(tp);
                }

                EditorUtility.SetDirty(go);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                atlas = go.GetComponent<uAtlas>();
            }

            return atlas;
        }
#endif
        static public T GetOrCreate<T>(string assetPath) where T : Component
        {
            T atlas = null;
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (go == null)
            {
                Directory.CreateDirectory(assetPath.Substring(0, assetPath.LastIndexOf('/')));
                go = new GameObject(Path.GetFileNameWithoutExtension(assetPath));
                PrefabUtility.CreatePrefab(assetPath, go);
                Object.DestroyImmediate(go);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                atlas = go.AddComponent<T>();
                EditorUtility.SetDirty(go);
                AssetDatabase.SaveAssets();
            }

            if (atlas == null)
                atlas = go.GetOrAddComponent<T>();

            return atlas;
        }
    }
}
#endif