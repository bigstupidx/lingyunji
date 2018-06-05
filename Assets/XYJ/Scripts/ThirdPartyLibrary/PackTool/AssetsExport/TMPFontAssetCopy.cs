//#if UNITY_EDITOR && USE_RESOURCESEXPORT
//using XTools;
//using System.IO;
//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//using System.Collections.Generic;

//namespace PackTool
//{
//    public partial class AssetsExport
//    {
//        static string GetCopyPath(string src, string name)
//        {
//            return src.Substring(0, src.LastIndexOf('/') + 1) + name;
//        }

//        [MenuItem("Assets/CreateTMPFontAsset")]
//        static void CreateTMPFontAsset()
//        {
//            AssetDatabase.CreateAsset(new TMPFontAsset(), "Assets/CreateTMPFontAsset.asset");
//        }

//        static TMPro.TMP_FontAsset CopyTMPFontAsset(TMPro.TMP_FontAsset font, IAssetsExport mgr)
//        {
//            string path = AssetDatabase.GetAssetPath(font);
//            string copy_path = "Assets/__copy__/" + path.Substring(7);
//            TMPFontAsset copy = AssetDatabase.LoadAssetAtPath<TMPFontAsset>(copy_path);
//            if (copy != null && !IsNeedUpdate(path.Substring(7)))
//                return copy;

//            Directory.CreateDirectory(copy_path.Substring(0, copy_path.LastIndexOf('/')));
//            string text = File.ReadAllText(path);
//            text = text.Replace("m_IsReadable: 0", "m_IsReadable: 1");
//            text = text.Replace("m_Script: {fileID: -667331979, guid: 89f0137620f6af44b9ba852b4190e64e, type: 3}", "m_Script: {fileID: 0}");
//            File.WriteAllText(copy_path, text);
//            AssetDatabase.Refresh();
//            AssetDatabase.ImportAsset(copy_path);

//            TMPFontAsset copy_font = (TMPFontAsset)AssetDatabase.LoadAssetAtPath(copy_path, typeof(UnityEngine.Object));
//            {
//                Material mat = copy_font.material;
//                string mat_path = GetCopyPath(copy_path, mat.name + ".mat");

//                Material copyMat = Object.Instantiate(mat);
//                AssetDatabase.CreateAsset(copyMat, mat_path);
//                AssetDatabase.SaveAssets();
//                Object.DestroyImmediate(mat, true);
//                copy_font.material = AssetDatabase.LoadAssetAtPath<Material>(mat_path);
//            }

//            {
//                Texture2D atlas = copy_font.atlas;
//                byte[] bytes = atlas.EncodeToPNG();
//                string atlas_path = GetCopyPath(copy_path, atlas.name + ".png");
//                File.WriteAllBytes(atlas_path, bytes);
//                AssetDatabase.Refresh();
//                TextureImporter ti = AssetImporter.GetAtPath(atlas_path) as TextureImporter;
//                ti.isReadable = false;
//                ti.textureCompression = TextureImporterCompression.CompressedHQ;
//                ti.wrapMode = atlas.wrapMode;
//                ti.textureType = TextureImporterType.SingleChannel;
//                ti.maxTextureSize = Mathf.Max(atlas.width, atlas.height);
//                ti.mipmapEnabled = false;
//                ti.alphaSource = TextureImporterAlphaSource.FromInput;
//                EditorUtility.SetDirty(ti);
//                AssetDatabase.SaveAssets();
//                AssetDatabase.ImportAsset(atlas_path);
//                copy_font.atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(atlas_path);
//                Object.DestroyImmediate(atlas, true);

//                TMPro.ShaderUtilities.GetShaderPropertyIDs();
//                copy_font.material.SetTexture(TMPro.ShaderUtilities.ID_MainTex, copy_font.atlas);
//            }

//            MemoryStream stream = new MemoryStream();
//            BinaryWriter writer = new BinaryWriter(stream);
//            ComponentData.__CollectTexture__(ref copy_font.atlas, writer, mgr);
//            ComponentData.__CollectMaterial__(ref copy_font.material, writer, mgr);

//            copy_font.bytes = stream.GetBuffer();
//            writer.Close();

//            EditorUtility.SetDirty(copy_font);
//            AssetDatabase.SaveAssets();
//            return copy_font;
//        }

//    }
//}

//#endif