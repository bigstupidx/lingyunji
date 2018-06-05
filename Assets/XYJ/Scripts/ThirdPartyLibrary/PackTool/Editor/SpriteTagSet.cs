using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public class SpriteTagSet
    {
        //[MenuItem("Assets/SetSpriteTag")]
        static public void SetDefaultTag()
        {
            string atlaspath = "Assets/Arts/UIData/UIAtlas/";

            string abstlaspath = Application.dataPath + "/Arts/UIData/UIAtlas/";
            if (Directory.Exists(abstlaspath))
                Utility.DeleteFolder(atlaspath);

            Directory.CreateDirectory(atlaspath);
            AssetDatabase.Refresh();
            string dic_path = "Assets";
            Dictionary<string, List<Sprite>> Sprites = new Dictionary<string, List<Sprite>>();
            HashSet<string> guids = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { dic_path }));
            foreach (string guid in guids)
            {
                string file = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter textureImporter = TextureImporter.GetAtPath(file) as TextureImporter;
                if (textureImporter != null)
                {
                    if (textureImporter.spriteImportMode != SpriteImportMode.None)
                    {
                        //if (string.IsNullOrEmpty(textureImporter.spritePackingTag))
                        {
                            string name = file.Replace('\\', '/');
                            name = name.Substring(0, name.LastIndexOf('/'));
                            name = name.Substring(name.LastIndexOf('/') + 1);
                            textureImporter.spritePackingTag = name;

                            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<Object>(file));
                            AssetDatabase.ImportAsset(file);

                            List<Sprite> sprites = null;
                            if (!Sprites.TryGetValue(name, out sprites))
                            {
                                sprites = new List<Sprite>();
                                Sprites.Add(name, sprites);
                            }

                            Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(file);
                            foreach (Object o in objs)
                            {
                                if (o is Sprite)
                                {
                                    sprites.Add(o as Sprite);
                                }
                            }
                        }
                    }
                }
            }

            System.IO.Directory.CreateDirectory(Application.dataPath + "/UIAtlas/");
            foreach (var itor in Sprites)
            {
                xys.UI.Atlas atlas = new GameObject(itor.Key).AddComponent<xys.UI.Atlas>();
                atlas.Sprites = itor.Value.ToArray();

                string path = "Assets/Arts/UIData/UIAtlas/" + itor.Key + ".prefab";
                PrefabUtility.CreatePrefab(path, atlas.gameObject);
                Debug.LogFormat("PrefabUtility.CreatePrefab({0})", path);
                Object.DestroyImmediate(atlas.gameObject);
            }

            AssetDatabase.Refresh();
        }
    }

}