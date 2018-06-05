//#define USE_compressedAssetBundle
#if UNITY_EDITOR && USE_RESOURCESEXPORT

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
#pragma warning disable 0618

namespace PackTool
{
    public partial class AssetsExport
    {
        public static bool IsShowLog = true;

        public static string GetObjectCopyPath(Object obj, string suffix)
        {
            string assetspath = AssetDatabase.GetAssetPath(obj);
            string filename = System.IO.Path.GetFileNameWithoutExtension(assetspath).Replace('@', '/');
            string path = assetspath.Substring(0, assetspath.LastIndexOf('/')).Substring(7);

            return string.Format("Assets/__copy__/{0}/{1}/{2}{3}{4}", path, filename, obj.name.Replace(':', '_'), GetObjArraySuffix(assetspath, obj), suffix);
        }

        // 数组后缀
        static string GetObjArraySuffix(string path, Object obj)
        {
            Object[] objs = LoadAllAssetsAtPath(path, obj.GetType());
            if (objs.Length == 1)
                return "";

            int index = 0;
            for (int i = 0; i < objs.Length; ++i)
            {
                if (objs[i].name == obj.name)
                {
                    ++index;
                }
                else
                {
                    objs[i] = null;
                }
            }

            if (index <= 1)
                return "";

            index = 0;
            for (int i = 0; i < objs.Length; ++i)
            {
                if (objs[i] == null)
                    continue;

                if (objs[i] == obj)
                    return index.ToString();

                ++index;
            }

            return "";
        }

        static Object[] LoadAllAssetsAtPath(string assetPath, System.Type type)
        {
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            List<Object> ts = new List<Object>();
            for (int i = 0; i < objs.Length; ++i)
            {
                if (type == objs[i].GetType())
                {
                    ts.Add(objs[i]);
                }
            }

            return ts.ToArray();
        }

        public static string ExportAnimPath(AnimationClip anim)
        {
            return ExportObjectPath(anim, ".anim");
        }

        public static string ExportAvatarPath(Avatar avatar)
        {
            return ExportObjectPath(avatar, ".asset");
        }

        public static string ExportObjectPath(Object obj, string suffix)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.ToLower().EndsWith(suffix))
                return path.Substring(7);

            path = GetObjectCopyPath(obj, suffix);
            if (path.StartsWith("Assets/__copy__/"))
                path = path.Substring(16);
            else
                path = path.Substring(7);

            return path;
        }

        public static string ExportMeshPath(Mesh mesh)
        {
            return ExportObjectPath(mesh, ".asset");
        }

        // 打包对象copy, 使用src的路径
        static public void ExportCopy(Object copy, Object src, BuildAssetBundleOptions options)
        {
            string path = ConvPath(src);
            ExportPath(copy, path, options);
        }

        static public void ExportPath(Object o, string path, BuildAssetBundleOptions options)
        {
            ExportPathEx(o, null, path, options);
        }

        static public bool IsNeedUpdate(string file)
        {
            if (file.StartsWith("__copy__/") && file.ToLower().EndsWith(".prefab"))
            {
                string temp = "Assets/" + file.Substring(9);
                if (File.Exists(Application.dataPath + "/" + temp.Substring(7)))
                    file = temp;
                else
                    file = "Assets/" + file;
            }
            else
            {
                file = "Assets/" + file;
            }

            return CodeCheckAtler.isAlter(file);
        }

        static public void ExportAB(Object mainAsset, BuildAssetBundleOptions options)
        {
            string assetPath = AssetDatabase.GetAssetPath(mainAsset);
            List<AssetBundleBuild> abbs = new List<AssetBundleBuild>();
            string abname = XTools.Utility.GetAssetBundleName(assetPath);
            abbs.Add(new AssetBundleBuild()
            {
                assetBundleName = abname,
                assetNames = new string[1] { assetPath }
            });

            options |= BuildAssetBundleOptions.ChunkBasedCompression;

            Directory.CreateDirectory(TempPath);
            var abm = BuildPipeline.BuildAssetBundles(TempPath, abbs.ToArray(), options, EditorUserBuildSettings.activeBuildTarget);

            string file = TempPath + abname;
            if (File.Exists(file))
            {
                string nf = PackPath + abname;
                if (File.Exists(nf))
                    File.Delete(nf);

                File.Copy(file, nf);
            }

            Debug.LogFormat("abm:{0} lenght:{1}", abm == null ? "null" : abm.name, abbs.Count);
        }

        static public void ExportPathEx(Object mainAsset, Object[] assets, string path, BuildAssetBundleOptions options)
        {
#if !USE_compressedAssetBundle
            //if ((options & BuildAssetBundleOptions.ChunkBasedCompression) == 0)
            //{
            //    options |= BuildAssetBundleOptions.ChunkBasedCompression;
            //}

            options |= BuildAssetBundleOptions.ChunkBasedCompression;
#endif
            string pathSrc = AssetDatabase.GetAssetPath(mainAsset).Substring(7);
            string savePath = PackPath + path;
            Directory.CreateDirectory(savePath.Substring(0, savePath.LastIndexOf('/')));
            string oldPath = TempPath + path;
            if ((!IsNeedUpdate(pathSrc)) && File.Exists(oldPath))
            {
                if (File.Exists(savePath))
                    File.Delete(savePath);

                if (IsShowLog)
                    Debuger.Log("复制:" + pathSrc);

                File.Copy(oldPath, savePath);
            }
            else
            {
                if (File.Exists(oldPath))
                    File.Delete(oldPath);

                string dspath = path;
                int pos = dspath.LastIndexOf('.');
                //int pos1 = -1;
                if ((dspath.IndexOf('.', 0, pos) != -1))
                {
                    dspath = dspath.Substring(0, pos).Replace('.', '_') + path.Substring(pos);
                    //Debug.Log("out:" + dspath);
                }
                string outpath = PackPath + dspath.Replace('/', '_');
                BuildPipeline.BuildAssetBundle(
                    assets == null ? mainAsset : null,
                    assets,
                    outpath,
                    options,
                    GetBuildTarget());

                if (outpath != savePath)
                {
                    if (File.Exists(savePath))
                        File.Delete(savePath);

                    if (File.Exists(outpath))
                        File.Move(outpath, savePath);
                }

                if (IsShowLog)
                    Debuger.Log("打包:" + pathSrc);

                Directory.CreateDirectory(oldPath.Substring(0, oldPath.LastIndexOf('/')));

                if (File.Exists(savePath))
                    File.Copy(savePath, oldPath);
            }
        }

        static public string Export(Object o, BuildAssetBundleOptions options)
        {
            string path = ConvPath(o);
            ExportPath(o, path, options);
            return path;
        }

#if USE_SELFATLAS
        public static IEnumerator ExportAtlas(UI.uAtlas atlas)
        {
            UnityEditor.Sprites.Packer.RebuildAtlasCacheIfNeeded(GetBuildTarget());
            ExportPath(atlas, GetSrcPath(atlas), BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CollectDependencies);
            yield break;
        }
#endif
        public static IEnumerator ExportAtlas(xys.UI.Atlas atlas)
        {
            UnityEditor.Sprites.Packer.RebuildAtlasCacheIfNeeded(GetBuildTarget());
            ExportPath(atlas, GetSrcPath(atlas), BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CollectDependencies);
            yield break;
        }

        public static IEnumerator ExportFontLib(Font font)
        {
            Export(font, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CollectDependencies);
            yield break;
        }

        static string GetExportPath(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.StartsWith("Assets/__copy__/"))
                path = path.Substring(16);

            return path;
        }

        public static void ExportMesh(Mesh mesh)
        {
            // 单独的网格资源
            ExportPath(mesh, GetSrcPath(mesh), BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CompleteAssets);
        }

        static void ExportAnim(AnimationClip anim)
        {
            ExportPath(anim, GetSrcPath(anim), BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle);
        }

        static bool IsAstc(TextureImporterFormat textureFormat, out TextureImporterFormat format)
        {
            switch (textureFormat)
            {
            case TextureImporterFormat.ASTC_RGB_4x4:
            case TextureImporterFormat.ASTC_RGB_5x5:
            case TextureImporterFormat.ASTC_RGB_6x6:
            case TextureImporterFormat.ASTC_RGB_8x8:
            case TextureImporterFormat.ASTC_RGB_10x10:
            case TextureImporterFormat.ASTC_RGB_12x12:
                format = TextureImporterFormat.PVRTC_RGB4;
                return true;

            case TextureImporterFormat.ASTC_RGBA_4x4:
            case TextureImporterFormat.ASTC_RGBA_5x5:
            case TextureImporterFormat.ASTC_RGBA_6x6:
            case TextureImporterFormat.ASTC_RGBA_8x8:
            case TextureImporterFormat.ASTC_RGBA_10x10:
            case TextureImporterFormat.ASTC_RGBA_12x12:
                format = TextureImporterFormat.PVRTC_RGBA4;
                return true;
            }

            format = 0;
            return false;
        }

        public static IEnumerator ExportTexture(Texture texture)
        {
            ExportPath(texture, GetSrcPath(texture), BuildAssetBundleOptions.DeterministicAssetBundle);

#if UNITY_IPHONE
            //if (texture is Texture2D)
            //{
            //    TextureImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
            //    TextureImporterPlatformSettings settings = ai.GetPlatformTextureSettings("iPhone");
            //    TextureImporterFormat textureFormat;
            //    if (IsAstc(settings.format, out textureFormat))
            //    {
            //        string source_path = ai.assetPath.Substring(7);
            //        string copy_path = "Assets/__copy__/PVRTC/" + source_path;
            //        if (IsNeedUpdate(source_path))
            //        {
            //            Directory.CreateDirectory(copy_path.Substring(0, copy_path.LastIndexOf('/')));
            //            AssetDatabase.CopyAsset(ai.assetPath, copy_path);
            //            ai = AssetImporter.GetAtPath(copy_path) as TextureImporter;
            //            settings.format = textureFormat;
            //            ai.SetPlatformTextureSettings(settings);
            //            EditorUtility.SetDirty(ai);
            //            AssetDatabase.ImportAsset(copy_path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            //            Texture pvrtc_text = AssetDatabase.LoadAssetAtPath<Texture>(copy_path);
            //            ExportPath(pvrtc_text, GetSrcPath(pvrtc_text), BuildAssetBundleOptions.DeterministicAssetBundle);
            //        }
            //    }
            //}
#endif
            yield break;
        }

        IEnumerator ExportAnimator(RuntimeAnimatorController obj)
        {
            ExportAB(obj, BuildAssetBundleOptions.DeterministicAssetBundle);
            yield break;

            //{
            //    string srcpath = AssetDatabase.GetAssetPath(obj);
            //    if (!srcpath.StartsWith("Assets/__copy__/"))
            //        return;

            //    string path_data = srcpath.Substring(16) + Suffix.AnimConByte;
            //    string fs = PackPath + path_data;
            //    if (File.Exists(fs))
            //        File.Delete(fs);

            //    File.Copy(Application.dataPath + "/__copy__/" + path_data, PackPath + path_data);
            //}
        }

        IEnumerator ExportAvatar(Avatar avatar)
        {
            ExportPath(avatar, GetSrcPath(avatar), BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle);
            yield break;
        }

        public static IEnumerator ExportTMPFont(TMPro.TMP_FontAsset font)
        {
            ExportPath(font, GetSrcPath(font), BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle);
            yield break;
        }

        public static IEnumerator ExportTexture2DAsset(Texture2DAsset asset)
        {
            string assetPath = GetSrcPath(asset);
            string savePath = PackPath + assetPath;
            Directory.CreateDirectory(savePath.Substring(0, savePath.LastIndexOf('/')));
            if (File.Exists(savePath) && !IsNeedUpdate(assetPath))
            {
                // 文件存在，并且文件不需要更新
                yield break;
            }

            asset.Export(savePath);
            yield break;
        }

        public static IEnumerator ExportSound(AudioClip clip)
        {
            Export(clip, BuildAssetBundleOptions.DeterministicAssetBundle);
            yield break;
        }

        static string GetSrcPath(Object obj)
        {
            return AssetDatabase.GetAssetPath(obj).Substring(7).Replace("__copy__/", "");
        }

        public static void ExportPrefab(GameObject prefab)
        {
            ExportPath(prefab, GetSrcPath(prefab), BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
        }
        public static void ExportLightProbes(LightProbes lightProbes)
        {
            Export(lightProbes, BuildAssetBundleOptions.ChunkBasedCompression);
        }

        public static IEnumerator ExportScene(Object scene)
        {
            ExportScene(scene, false);
            yield break;
        }

        public static void ExportScene(Object scene, bool isFouce)
        {
            string pathSrc = AssetDatabase.GetAssetPath(scene);
            string path = GetSrcPath(scene);
            string savePath = PackPath + path;
            Directory.CreateDirectory(savePath.Substring(0, savePath.LastIndexOf('/')));
            string oldPath = TempPath + path;

            {
                string path_data = path + Suffix.SceneDataByte;
                string path_pos = path + Suffix.ScenePosByte;

                string fs = PackPath + path_data;
                if (File.Exists(fs))
                    File.Delete(fs);

                if (File.Exists(Application.dataPath + "/__copy__/" + path_data))
                    File.Copy(Application.dataPath + "/__copy__/" + path_data, PackPath + path_data);

                fs = PackPath + path_pos;
                if (File.Exists(fs))
                    File.Delete(fs);

                if (File.Exists(Application.dataPath + "/__copy__/" + path_pos))
                    File.Copy(Application.dataPath + "/__copy__/" + path_pos, PackPath + path_pos);
            }

            if ((!IsNeedUpdate(pathSrc.Substring(7))) && File.Exists(oldPath) && (!isFouce))
            {
                if (File.Exists(savePath))
                    File.Delete(savePath);

                if (IsShowLog)
                    Debuger.Log("复制:" + pathSrc.Substring(7));

                File.Copy(oldPath, savePath);
            }
            else
            {
                if (File.Exists(oldPath))
                    File.Delete(oldPath);

                string outpath = PackPath + path.Replace('/', '_');

#if !PREFAB_SCENE
                BuildPipeline.BuildStreamedSceneAssetBundle(
                    new string[] { pathSrc }, 
                    outpath, 
                    GetBuildTarget(),
                    BuildOptions.UncompressedAssetBundle);
#else
                GameObject scene_prefab = AssetDatabase.LoadAssetAtPath(pathSrc.Substring(0, pathSrc.LastIndexOf('.')) + ".scene.prefab", typeof(GameObject)) as GameObject;
                BuildPipeline.BuildAssetBundle(
                    scene_prefab,
                    null,
                    outpath,
                    BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle,
                    GetBuildTarget());
#endif

                if (outpath != savePath)
                {
                    if (File.Exists(savePath))
                        File.Delete(savePath);

                    if (File.Exists(outpath))
                        File.Move(outpath, savePath);
                }

                if (IsShowLog)
                    Debuger.Log("打包:" + pathSrc);

                Directory.CreateDirectory(oldPath.Substring(0, oldPath.LastIndexOf('/')));

                if (File.Exists(savePath))
                    File.Copy(savePath, oldPath);
            }
        }

        public static BuildTarget GetBuildTarget()
        {
            return EditorUserBuildSettings.activeBuildTarget;
        }

        public static BuildTargetGroup GetBuildTargetGroup()
        {
            return EditorUserBuildSettings.selectedBuildTargetGroup;
        }
    }
}
#endif
