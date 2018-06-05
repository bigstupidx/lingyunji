#define ETC_SINGLE

#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public partial class AssetsExport    
    {
        const string etc_alpha = "__copy__/etc/a/";
        const string etc_rgb = "__copy__/etc/d/";

        [MenuItem("Assets/PackTool/ETC/透明分离", false, 0)]
        static void ExportETC_Alpha()
        {
            ETCType t;
            Texture2D etc;
            Texture2D alpha;
            foreach (Object o in Selection.objects)
            {
                if (o is Texture2D)
                {
                    GenETC(o as Texture2D, out etc, out alpha, out t);
                }
            }
        }

        static bool is2Value(int v)
        {
            while (true)
            {
                if (v % 2 != 0)
                    return false;

                v = v / 2;
                if (v == 1)
                    return true;
            }
        }

        static IEnumerator CheckTexture()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int total = 0;
            {
                FileList mFileList = new FileList();
                PackResources mMatRes = new PackResources();
                IEnumerator itor = PackResources.GetResourcesImpItor<Texture2D>(
                    (Texture2D t2d, string file)=>
                    {
                        if (file.StartsWith("Assets/UIData/"))
                            return;

                        if (t2d.width == t2d.height && is2Value(t2d.width))
                            return;

                        ++total;
                        sb.AppendLine(string.Format("{0} width:{1} height:{2}", file, t2d.width, t2d.height));
                    },
                    ()=>
                    {

                    },
                    mFileList.GetFiles(Application.dataPath), 
                    null, 
                    null,
                    new string[] { ".png", ".tga", ".jpg" });

                while (itor.MoveNext())
                    yield return 0;

                Debug.Log(string.Format("total:{0}\r\n {1}", total, sb.ToString()));
            }
        }

        [MenuItem("PackTool/纹理/非2倍数", false, 0)]
        static void CheckTextureMenu()
        {
            GlobalCoroutine.StartCoroutine(CheckTexture());
        }

        [MenuItem("PackTool/ETC/检测材质", false, 0)]
        static void ExportETC_Material()
        {
            HashSet<Shader> shader = new HashSet<Shader>();
            {
                FileList mFileList = new FileList();
                PackResources mMatRes = new PackResources();
                PackResources.GetResources<Material>(mMatRes, mFileList.GetFiles(Application.dataPath), null, null, new string[] { ".mat" });
                foreach (Material mat in mMatRes.GetList<Material>())
                {
                    if (ETCShader.CheckMaterialETC(mat))
                    {
                        // 需要ETC shader
                        shader.Add(mat.shader);
                    }
                }
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(string.Format("total:{0}", shader.Count));
            foreach (Shader s in shader)
            {
                sb.AppendLine(string.Format("{0} path:{1}", s.name, AssetDatabase.GetAssetPath(s)));
            }

            Debug.Log(sb.ToString());
        }

        public enum ETCType
        {
            Null,
            R,
            G,
            B,
        }

        static string FindTexture(Texture2D t2d, out ETCType t, out bool has)
        {
            string path = AssetDatabase.GetAssetPath(t2d);
            string filepath = null;
            CacheData cd = null;
            if (CacheList.TryGetValue(path, out cd))
            {
                // 当前有缓存的数据
                filepath = cd.alpha_path;
                t = cd.type;
                has = true;
                return cd.alpha_path;
            }
            else
            {
                filepath = FindTexture(path, out t);
            }

            cd = new CacheData();
            if (string.IsNullOrEmpty(filepath))
            {
                // 当前没有，那么查找个空的分配过去
                has = false;
                cd.alpha_path = FindEmptyTexture(t2d.width, t2d.height, out t);
            }
            else
            {
                // 当前已经有了，那么要判断下，当前的大小是否与之前的一至
                string name = System.IO.Path.GetFileNameWithoutExtension(filepath);
                if (name.StartsWith(string.Format("{0}x{1}", t2d.width, t2d.height)))
                {
                    // 大小一至，直接返回
                    has = true;
                    return filepath;
                }
                else
                {
                    has = false;
                    RemoveTexture(path);
                    cd.alpha_path = FindEmptyTexture(t2d.width, t2d.height, out t); 
                }
            }

            cd.type = t;
            CacheList.Add(path, cd);
            return cd.alpha_path;
        }

        static string FindTexture(string filepath, out ETCType t)
        {
            t = ETCType.Null;
            HashSet<string> files = null;
            bool hasremove = true;
            while (hasremove)
            {
                string alphaPath = string.Format("{0}/{1}", Application.dataPath, etc_alpha);
                if (!System.IO.Directory.Exists(alphaPath))
                    return "";

                files = new HashSet<string>(System.IO.Directory.GetFiles(alphaPath));
                if (files.Count == 0)
                    return "";

                hasremove = false;
                foreach (string file in files)
                {
                    if (file.EndsWith(".txt"))
                    {
                        if (!files.Contains(file.Substring(0, file.Length - 4)))
                        {
                            File.Delete(file);
                            hasremove = true;
                        }
                    }
                    else if (file.EndsWith(".png"))
                    {
                        if (!files.Contains(file + ".txt"))
                        {
                            File.Delete(file + ".txt");
                            hasremove = true;
                        }
                    }
                }
            }

            ETCFile etcfile = new ETCFile();
            foreach (string file in files)
            {
                if (!file.EndsWith(".txt"))
                    continue;

                etcfile.Init(file);
                t = etcfile.has(filepath);
                if (t != ETCType.Null)
                    return file.Substring(0, file.Length - 4);
            }

            return "";
        }

        static bool RemoveTexture(string file)
        {
            string[] files = System.IO.Directory.GetFiles(string.Format("{0}{1}", Application.dataPath, etc_alpha));
            if (files.Length == 0)
                return false;

            ETCType t = ETCType.Null;
            ETCFile etcfile = new ETCFile();
            for (int i = 0; i < files.Length; ++i)
            {
                etcfile.Init(files[i]);
                t = etcfile.has(file);
                if (t != ETCType.Null)
                {
                    etcfile.Set(t, "");
                    etcfile.SaveFile(files[i]);
                    return true;
                }
            }

            return false;
        }

        static string FindEmptyTexture(int width, int height, out ETCType t)
        {
            t = ETCType.Null;
            int index = 0;
            string filename;
            while (true)
            {
                filename = string.Format("{0}/{1}{2}x{3}_{4}.png", Application.dataPath, etc_alpha, width, height, index);
                if (File.Exists(filename))
                {
                    // 存在图集，查找下当前写到第几张了
                    ETCFile etcfile = new ETCFile();
                    etcfile.Init(filename + ".txt");
                    t = etcfile.GetEmpty();
                    if (t != ETCType.Null)
                        return filename;
                    else
                        ++index;
                }
                else
                {
                    t = ETCType.R;
                    break;
                }
            }

            return filename;
        }

        static void CopyImportTexture(string src, string dst, bool iscopy = true)
        {
            TextureImporter src_ti = AssetImporter.GetAtPath(src) as TextureImporter;
            if (src_ti == null)
            {
                Debug.LogError("null! src:" + src);
                return;
            }

            TextureImporterSettings settings = new TextureImporterSettings();
            src_ti.ReadTextureSettings(settings);

            TextureImporter dst_ti = AssetImporter.GetAtPath(dst) as TextureImporter;
            if (dst_ti == null)
            {
                Debug.LogError("null! dst:" + dst);
                return;
            }

            dst_ti.SetTextureSettings(settings);

            int maxTextureSize;
            TextureImporterFormat textureFormat;
            src_ti.GetPlatformTextureSettings("Android", out maxTextureSize, out textureFormat);
            dst_ti.SetPlatformTextureSettings("Android", maxTextureSize, iscopy ? textureFormat : TextureImporterFormat.ETC_RGB4);

            src_ti.GetPlatformTextureSettings("iPhone", out maxTextureSize, out textureFormat);
            dst_ti.SetPlatformTextureSettings("iPhone", maxTextureSize, iscopy ? textureFormat : TextureImporterFormat.PVRTC_RGB4);

            AssetDatabase.ImportAsset(dst, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }

        static void CopyTextureAsset(string src, string dst)
        {
            string copy_file = Application.dataPath + dst.Substring(6);
            if (File.Exists(copy_file))
            {
                File.Delete(copy_file);
                string srcpath = Application.dataPath + src.Substring(6);
                File.Copy(src, copy_file);
            }
            else
            {
                Directory.CreateDirectory(copy_file.Substring(0, copy_file.LastIndexOf('/')));
                AssetDatabase.CopyAsset(src, dst);
            }

            AssetDatabase.ImportAsset(dst, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

            CopyImportTexture(src, dst, true);
        }

        class CacheData
        {
            public string alpha_path;
            public ETCType type;
        }

        // 当前缓存的数据
        static Dictionary<string, CacheData> CacheList = new Dictionary<string, CacheData>();

#if ETC_SINGLE
        static bool GenETC_Single(Texture2D t2d, out Texture2D etc_copy_t2d, out Texture2D alpha_t2d, out ETCType t)
        {
            t = ETCType.Null;
            etc_copy_t2d = null;
            alpha_t2d = null;
            if (!TextureExport.isARGB(t2d.format))
                return false;

            t = ETCType.R;
            string src_asset_path = AssetDatabase.GetAssetPath(t2d);
            string alpha_asset_file = src_asset_path.Insert(7, etc_alpha);
            string alpha_file = Application.dataPath + alpha_asset_file.Substring(6);
            alpha_t2d = AssetDatabase.LoadAssetAtPath(alpha_file, typeof(Texture2D)) as Texture2D;
            if (alpha_t2d != null)
            {
                // 当前已经拥有了,判断下是否需要更新
                if (IsNeedUpdate(src_asset_path.Substring(7)))
                {
                    // 需要更新
                }
                else
                {
                    // 不需要更新
                    etc_copy_t2d = AssetDatabase.LoadAssetAtPath(src_asset_path.Insert(7, etc_rgb), typeof(Texture2D)) as Texture2D;
                    return true;
                }
            }

            // 拷贝贴图资源到etc目录下
            string etc_copy_path = src_asset_path.Insert(7, etc_rgb);
            CopyTextureAsset(src_asset_path, etc_copy_path);
            etc_copy_t2d = AssetDatabase.LoadAssetAtPath(etc_copy_path, typeof(Texture2D)) as Texture2D;

            // 把透明信息写入到文件当中
            {
                Color32[] cols = null;
                if (File.Exists(alpha_file))
                {
                    File.Delete(alpha_file);
                }

                {
                    int cl = t2d.width * t2d.height;
                    cols = new Color32[cl];
                    for (int i = 0; i < cl; ++i)
                    {
                        cols[i].a = 0;
                        cols[i].r = 0;
                        cols[i].g = 0;
                        cols[i].b = 0;
                    }
                }

                WriteAlphaToColor(etc_copy_t2d, cols, t);
                Directory.CreateDirectory(alpha_file.Substring(0, alpha_file.LastIndexOf('/')));
                Texture2D temp_t2d = new Texture2D(t2d.width, t2d.height);
                temp_t2d.SetPixels32(cols);
                temp_t2d.Apply();
                cols = null;
                File.WriteAllBytes(alpha_file, temp_t2d.EncodeToPNG());
                Object.DestroyImmediate(temp_t2d);
                //System.GC.Collect();

                AssetDatabase.Refresh();
                CopyImportTexture(src_asset_path, etc_copy_path, false);
                CopyImportTexture(etc_copy_path, alpha_asset_file, true);
                TextureExport.ETCAlphaTexture(AssetImporter.GetAtPath(alpha_asset_file) as TextureImporter, Mathf.Max(t2d.width, t2d.height), TextureImporterFormat.ETC_RGB4);
                alpha_t2d = AssetDatabase.LoadAssetAtPath(alpha_asset_file, typeof(Texture2D)) as Texture2D;
            }

            //AssetDatabase.Refresh();
            //System.GC.Collect();

            return true;
        }
#endif

        // 收集所有ETC材质
        void CollectAllETCShader()
        {
            if (ETCShader.AllETCShaders.Count == 0)
                ETCShader.Init();

            foreach (KeyValuePair<string, Shader> itor in ETCShader.AllETCShaders)
            {
#if ETC_SINGLE
                if (itor.Value.name.Contains("_ETCR"))
#else

#endif
                {
                    Logger.LogDebug("etc:{0}", itor.Value.name);
                    CollectBuiltinResource(itor.Value);
                }
            }
        }

        static bool GenETC(Texture2D t2d, out Texture2D etc_copy_t2d, out Texture2D alpha_t2d, out ETCType t)
        {
#if ETC_SINGLE
            return GenETC_Single(t2d, out etc_copy_t2d, out alpha_t2d, out t);
#else
            return GenETC_RGB(t2d, out etc_copy_t2d, out alpha_t2d, out t);
#endif
        }


#if !ETC_SINGLE
        static bool GenETC_RGB(Texture2D t2d, out Texture2D etc_copy_t2d, out Texture2D alpha_t2d, out ETCType t)
        {
            t = ETCType.Null;
            etc_copy_t2d = null;
            alpha_t2d = null;
            if (!TextureExport.isARGB(t2d.format))
                return false;

            bool has = false;
            string src_asset_path = AssetDatabase.GetAssetPath(t2d);
            string alpha_file = FindTexture(t2d, out t, out has);
            if (has == true)
            {
                // 当前已经拥有了,判断下是否需要更新
                if (IsNeedUpdate(src_asset_path.Substring(7)))
                {
                    // 需要更新
                }
                else
                {
                    // 不需要更新
                    etc_copy_t2d = AssetDatabase.LoadAssetAtPath(src_asset_path.Insert(7, etc_rgb), typeof(Texture2D)) as Texture2D;
                    alpha_t2d = AssetDatabase.LoadAssetAtPath(alpha_file.Substring(Application.dataPath.Length - 6), typeof(Texture2D)) as Texture2D;
                    return true;
                }
            }

            // 拷贝贴图资源到etc目录下
            string etc_copy_path = src_asset_path.Insert(7, etc_rgb);
            {
                CopyTextureAsset(src_asset_path, etc_copy_path);

                etc_copy_t2d = AssetDatabase.LoadAssetAtPath(etc_copy_path, typeof(Texture2D)) as Texture2D;
            }

            // 把透明信息写入到文件当中
            {
                string alpha_asset_file = alpha_file.Substring(Application.dataPath.Length - 6);
                Color32[] cols = null;
                if (File.Exists(alpha_file))
                {
                    Texture2D etc = new Texture2D(1, 1);
                    etc.LoadImage(File.ReadAllBytes(alpha_file));
                    etc.Apply();
                    cols = etc.GetPixels32();
                    Object.DestroyImmediate(etc);
                }
                else
                {
                    int cl = t2d.width * t2d.height;
                    cols = new Color32[cl];
                    for (int i = 0; i < cl; ++i)
                    {
                        cols[i].a = 0;
                        cols[i].r = 0;
                        cols[i].g = 0;
                        cols[i].b = 0;
                    }
                }

                WriteAlphaToColor(etc_copy_t2d, cols, t);
                CopyImportTexture(src_asset_path, etc_copy_path, false);
                Directory.CreateDirectory(alpha_file.Substring(0, alpha_file.LastIndexOf('/')));
                Texture2D temp_t2d = new Texture2D(t2d.width, t2d.height);
                temp_t2d.SetPixels32(cols);
                temp_t2d.Apply();
                cols = null;
                File.WriteAllBytes(alpha_file, temp_t2d.EncodeToPNG());
                Object.DestroyImmediate(temp_t2d);
                //System.GC.Collect();

                AssetDatabase.Refresh();
                TextureExport.ETCAlphaTexture(AssetImporter.GetAtPath(alpha_asset_file) as TextureImporter, Mathf.Max(t2d.width, t2d.height) / 2);
                alpha_t2d = AssetDatabase.LoadAssetAtPath(alpha_asset_file, typeof(Texture2D)) as Texture2D;
            }

            ETCFile etcfile = new ETCFile();
            etcfile.Init(alpha_file + ".txt");
            etcfile.Set(t, src_asset_path);
            etcfile.SaveFile(alpha_file + ".txt");

            //AssetDatabase.Refresh();
            //System.GC.Collect();

            return true;
        }
#endif

        static void WriteAlphaToColor(Texture2D copy_t2d, Color32[] cols, ETCType type)
        {
            int width = copy_t2d.width;
            int height = copy_t2d.height;
            ImportTextureReader(copy_t2d,
                () =>
                {
                    int pos = 0;
                    Color32[] cs = copy_t2d.GetPixels32();
                    for (int i = 0; i < width; ++i)
                    {
                        for (int j = 0; j < height; ++j)
                        {
                            pos = j * width + i;
                            switch (type)
                            {
                            case ETCType.R:
                                cols[pos].r = cs[pos].a;
                                break;
                            case ETCType.G:
                                cols[pos].g = cs[pos].a;
                                break;
                            case ETCType.B:
                                cols[pos].b = cs[pos].a;
                                break;
                            }
                        }
                    }

                    cs = null;
                });
        }

        static void ImportTextureReader(Texture2D t2d, System.Action fun)
        {
            string path = AssetDatabase.GetAssetPath(t2d);
            if (!string.IsNullOrEmpty(path))
            {
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                TextureImporterSettings settings = new TextureImporterSettings();
                ti.ReadTextureSettings(settings);

                bool isset = false;
                if (!settings.readable)
                {
                    isset = true;
                    settings.readable = true;
                    ti.SetTextureSettings(settings);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                    //AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                }

                fun();

                if (isset)
                {
                    settings.readable = false;
                    ti.SetTextureSettings(settings);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                    //AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                }
            }
            else
            {
                fun();
            }
        }
    }
}
#endif