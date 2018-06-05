using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PackTool;

#if UNITY_EDITOR
using UnityEditor;
public class EditorField : System.Attribute
{

}
#endif

namespace XTools
{
    public static partial class Utility
    {
#if UNITY_EDITOR
        public static bool IsExportResources(string assetPath)
        {
            if (assetPath.EndsWith(".cs", true, null) ||
                assetPath.EndsWith(".js", true, null) ||
                assetPath.EndsWith(".mask", true, null) ||
                assetPath.EndsWith(".dll", true, null))
                return false;

            return true;
        }

        public static string GetAssetBundleName(string assetPath)
        {
            if (assetPath.StartsWith("Assets/__copy__/"))
                return assetPath.Substring(16);

            return assetPath.Substring(7);
        }

        public static bool SetAssetBundle(string assetPath, out string assetBundleName)
        {
            AssetImporter ai = AssetImporter.GetAtPath(assetPath);
            assetBundleName = GetAssetBundleName(assetPath);
            if (!string.Equals(assetBundleName, ai.assetBundleName, System.StringComparison.InvariantCultureIgnoreCase))
            {
                ai.assetBundleName = assetBundleName;
                EditorUtility.SetDirty(ai);
                return true;
            }

            return false;
        }

        public static bool isAB(string file)
        {
            if (file.StartsWith("Data/") ||
                file.IndexOf('.') == -1 ||
                file == "sprites_atlas.b" ||
                file.EndsWith(".mat") ||
                file.EndsWith(".prefab.pd") ||
                file.EndsWith(".unity.sbd") ||
                (file.Contains("/t/") && file.EndsWith(".asset")) ||
                file.EndsWith(".unity.sbp") ||
                file.EndsWith(".manifest") ||
                file.EndsWith(".pdb") ||
                file.EndsWith(".dll"))
                return false;

            return true;
        }

        static public GameObject ResetSceneRoot(UnityEngine.SceneManagement.Scene currentScene)
        {
            GameObject[] roots = currentScene.GetRootGameObjects();
            var sceneRoot = new GameObject();

            sceneRoot.name = currentScene.name;
            sceneRoot.transform.position = Vector3.zero;
            sceneRoot.transform.localScale = Vector3.one;
            sceneRoot.transform.rotation = Quaternion.identity;

            if (roots.Length > 1)
            {
                foreach (var r in roots)
                {
                    if (!r.name.Contains("MapScene"))
                    {
                        Object.DestroyImmediate(r);
                    }
                }
            }

            roots = currentScene.GetRootGameObjects();
            Transform root = sceneRoot.transform;
            for (int i = 0; i < roots.Length; ++i)
            {
                switch (PrefabUtility.GetPrefabType(roots[i]))
                {
                case PrefabType.PrefabInstance:
                case PrefabType.ModelPrefabInstance:
                    PrefabUtility.DisconnectPrefabInstance(roots[i]);
                    break;
                }

                roots[i].transform.parent = root;
            }

            CheckSceneRoot(sceneRoot);
            return sceneRoot;
        }

        static public void CheckSceneRoot(UnityEngine.GameObject sceneRoot)
        {
            sceneRoot.SetActive(false);
            List<UnityEngine.Component> Components = new List<UnityEngine.Component>();
            sceneRoot.GetComponentsInChildren(true, Components);
            for (int i = 0; i < Components.Count; ++i)
                XTools.Utility.CheckScene(Components[i]);

            int childCount = sceneRoot.transform.childCount;
            List<UnityEngine.GameObject> childs = new List<UnityEngine.GameObject>();
            for (int i = 0; i < childCount; ++i)
                childs.Add(sceneRoot.transform.GetChild(i).gameObject);

            for (int i = 0; i < childs.Count; ++i)
                DestoryDisable(childs[i]);
        }

        public static void DestoryDisable(UnityEngine.GameObject go)
        {
            var tsfs = go.GetComponentsInChildren<UnityEngine.Transform>(true);
            for (int i = 0; i < tsfs.Length; ++i)
            {
                if (tsfs[i] == null)
                    continue;

                if (!tsfs[i].gameObject.activeSelf)
                {
                    Destroy(tsfs[i].gameObject);
                }
            }
        }

        public static void CheckScene(UnityEngine.Component c)
        {
            if (c == null || c is UnityEngine.Transform)
                return;

            if (c is UnityEngine.Terrain)
            {
                UnityEngine.Terrain t = (UnityEngine.Terrain)c;
                if (!t.enabled || !t.gameObject.activeSelf)
                    Destroy(c.gameObject);
            }
            else if (c is MeshSimplify)
            {
                Destroy(c.gameObject);
            }
            else if (c is UnityEngine.Animator)
            {
                UnityEngine.Animator anim = c as UnityEngine.Animator;
                if (anim.runtimeAnimatorController == null)
                {
                    Destroy(anim);
                }
            }
            else if (c is UnityEngine.MeshRenderer)
            {
                UnityEngine.MeshRenderer mr = c as UnityEngine.MeshRenderer;
                UnityEngine.MeshFilter mf = mr.GetComponent<UnityEngine.MeshFilter>();
                if (mf == null || !mr.enabled || (mr.sharedMaterials.Length == 0) || mf.sharedMesh == null)
                {
                    Destroy(mr);
                    if (mf != null)
                        Destroy(mf);
                }
            }
            else if (c is UnityEngine.Animation)
            {
                UnityEngine.Animation anim = c as UnityEngine.Animation;
                if (!anim.enabled || (anim.GetClipCount() == 0 && anim.clip == null))
                {
                    Destroy(anim);
                }
            }
            else if (c is UnityEngine.Collider)
            {
                UnityEngine.Collider collider = c as UnityEngine.Collider;
                if (!collider.enabled)
                {
                    Destroy(collider);
                }
                else if (collider is UnityEngine.MeshCollider)
                {
                    if (((UnityEngine.MeshCollider)collider).sharedMesh == null)
                    {
                        Destroy(collider);
                    }
                }
            }
            else if (c is UnityEngine.Light)
            {
                UnityEngine.Light light = c as UnityEngine.Light;
                if (light.isBaked)
                {
                    Destroy(c.gameObject);
                }
            }
        }
#endif
        public static void SetAddOrSub<T1, T2>(HashSet<T1> olds, Dictionary<T1, T2> news, System.Action<T1, T2> onadd, System.Action<T1> onsub)
        {
            foreach (KeyValuePair<T1, T2> itor in news)
            {
                if (!olds.Contains(itor.Key))
                {
                    // 新的有，旧的没有，增加的
                    onadd(itor.Key, itor.Value);
                }
            }

            foreach (T1 t in olds)
            {
                if (!news.ContainsKey(t))
                {
                    // 旧的有，新的没有，移除的
                    onsub(t);
                }
            }
        }

        static public UnityEngine.Color ParseColor(string text, int offset)
        {
            int l = text.Length - offset;
            if (l >= 8)
            {
                return ParseColor32(text, offset);
            }
            else if (l >= 6)
            {
                return ParseColor24(text, offset);
            }

            return UnityEngine.Color.white;
        }

        static public UnityEngine.Color ParseColor24(string text, int offset)
        {
            int r = (HexToDecimal(text[offset]) << 4) | HexToDecimal(text[offset + 1]);
            int g = (HexToDecimal(text[offset + 2]) << 4) | HexToDecimal(text[offset + 3]);
            int b = (HexToDecimal(text[offset + 4]) << 4) | HexToDecimal(text[offset + 5]);
            float f = 1f / 255f;
            return new UnityEngine.Color(f * r, f * g, f * b);
        }

        static public UnityEngine.Color ParseColor32(string text, int offset)
        {
            int r = (HexToDecimal(text[offset]) << 4) | HexToDecimal(text[offset + 1]);
            int g = (HexToDecimal(text[offset + 2]) << 4) | HexToDecimal(text[offset + 3]);
            int b = (HexToDecimal(text[offset + 4]) << 4) | HexToDecimal(text[offset + 5]);
            int a = (HexToDecimal(text[offset + 6]) << 4) | HexToDecimal(text[offset + 7]);
            float f = 1f / 255f;
            return new UnityEngine.Color(f * r, f * g, f * b, f * a);
        }

        static public int HexToDecimal(char ch)
        {
            switch (ch)
            {
            case '0': return 0x0;
            case '1': return 0x1;
            case '2': return 0x2;
            case '3': return 0x3;
            case '4': return 0x4;
            case '5': return 0x5;
            case '6': return 0x6;
            case '7': return 0x7;
            case '8': return 0x8;
            case '9': return 0x9;
            case 'a':
            case 'A': return 0xA;
            case 'b':
            case 'B': return 0xB;
            case 'c':
            case 'C': return 0xC;
            case 'd':
            case 'D': return 0xD;
            case 'e':
            case 'E': return 0xE;
            case 'f':
            case 'F': return 0xF;
            }
            return 0xF;
        }

        public static void AbortThread(Thread t)
        {
            if (t != null)
            {
                try
                {
                    t.Abort();
                }
                catch (System.Exception ex)
                {
                    XYJLogger.LogException(ex);
                }
            }
        }

        public static void EachGameObject(UnityEngine.GameObject root, System.Action<UnityEngine.GameObject> fun)
        {
            fun(root);
            UnityEngine.Transform tran = root.transform;
            for (int i = 0; i < tran.childCount; ++i)
            {
                EachGameObject(tran.GetChild(i).gameObject, fun);
            }
        }

        public static bool isLzma(string name)
        {
            //if (name.EndsWith(PackTool.Suffix.PrefabDataByte) ||
            //    name.EndsWith(PackTool.Suffix.SceneDataByte) ||
            //    name.EndsWith(PackTool.Suffix.ScenePosByte) ||
            //    name.EndsWith(".mat", true, null) ||
            //    name.EndsWith(".x"))
            //    return false; // 这种格式的不压缩

            //if (name.StartsWith("Data/"))
            //    return false; // 不压缩

            //if (name == "sprites_atlas.csv")
            //    return false; // 不压缩

            //if (name == "sprites_atlas.b")
            //    return false; // 不压缩

            //if (name == "resoruce_config")
            //    return false; // 不压缩

            return false;
        }


        public static bool isInherited(System.Type type, System.Type baseType)
        {
            if (type == null)
                return false;

            if (type == baseType)
                return true;

            if (type.BaseType == null)
                return false;

            if (type.BaseType == baseType)
                return true;

            return isInherited(type.BaseType, baseType);
        }

        public const string FmodPlatformKey =
#if UNITY_IPHONE || UNITY_ANDROID
            "Mobile";
#else
            "Desktop";
#endif

#if UNITY_EDITOR
        public static UnityEditor.BuildTarget GetIOSBuildTarget()
        {
            return UnityEditor.BuildTarget.iOS;
        }

        static public string GetPath(UnityEngine.GameObject root, UnityEngine.GameObject child)
        {
            if (root == child)
                return "";

            List<string> childs = new List<string>();
            var current = child.transform;
            var rt = root == null ? null : root.transform;
            while (true)
            {
                childs.Add(current.name);
                current = current.parent;
                if (current == null || current == rt)
                    break;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(childs[childs.Count - 1]);
            for (int i = childs.Count - 2; i >= 0; --i)
                sb.AppendFormat("/{0}", childs[i]);

            return sb.ToString();
        }

        // 拷贝配置文件到指定的目录
        public static void CopyConfigToPath(string fileroot)
        {
            string audio_platform_suffix = string.Format("Sounds/{0}/", FmodPlatformKey);
            string filedata = fileroot + "Data";
            filedata = Path.GetFullPath(filedata).Replace('\\', '/');

            DeleteFolder(filedata);
            string root = ResourcesPath.dataPath + "/../" + "Data";
            root = Path.GetFullPath(root).Replace('\\', '/');
            CopyFolder(root, filedata,
                (string fn) =>
                {
                    fn = Path.GetFullPath(fn).Replace('\\', '/');
                    fn = fn.Substring(filedata.Length + 1);
                    if (fn.StartsWith("obj/"))
                        return false;
                    if (fn.EndsWith(".exe"))
                        return false;
                    if (fn.EndsWith(".bat"))
                        return false;
                    if (fn.Equals("Lua.luaprj"))
                        return false;

                    if (fn.StartsWith("LuaScripts."))
                        return false;

                    if (fn.StartsWith("Data/Config/Terrain/"))
                        return false;

                    if (fn.EndsWith(".bin"))
                        return false;

                    if (fn.StartsWith("Sounds/"))
                    {
                        if (!fn.StartsWith(audio_platform_suffix))
                            return false;
                    }

                    return !fn.EndsWith(".meta", true, null);
                });
        }

        public static UnityEngine.Object[] FindAssets(string file, System.Func<string, bool> fun)
        {
            string root = (UnityEngine.Application.dataPath + "/").Replace('\\', '/');
            string path = root + file.Substring(7);
            string fp;
            string[] ress = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
            foreach (string res in ress)
            {
                if (fun(res))
                {
                    fp = res.Replace('\\', '/');
                    fp = fp.Substring(fp.IndexOf("/Assets/") + 1);
                    UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(fp, typeof(UnityEngine.Object));
                    if (obj != null)
                        objs.Add(obj);
                }
            }

            return objs.ToArray();
        }

        // 是否需要加密
        public static bool isEncry(string name)
        {
            if (name.StartsWith("Data/StreamingAssets/Data/Config/"))
                return true; // 只加密配置表

            return false; // 不加密
        }

        static int GetFileSize(string file)
        {
            FileInfo fi = new FileInfo(file);
            if (fi.Exists)
                return (int)fi.Length;

            return 0;
        }
#endif
        public static bool CopyStream(Stream input, Stream output, byte[] bytes)
        {
            try
            {
                int len;
                while ((len = input.Read(bytes, 0, bytes.Length)) > 0)
                    output.Write(bytes, 0, len);

                return true;
            }
            catch (System.Exception ex)
            {
                XYJLogger.LogException(ex);
                return false;
            }
        }

        public static void CopyStream(Stream input, Network.BitStream output)
        {
            int writeSize = output.WriteRemain; // 可写入的空间
            int len = 0;
            while ((len = input.Read(output.Buffer, output.WritePos, writeSize)) > 0)
            {
                output.WritePos += len;
                writeSize = output.WriteRemain;
                if (writeSize == 0)
                {
                    output.ensureCapacity(1024);
                    writeSize = output.WriteRemain;
                }
            }
        }

        static public string ToString(UnityEngine.Color color)
        {
            return string.Format("{0},{1},{2},{3}", color.r, color.g, color.b, color.a);
        }

        static public UnityEngine.Color ToColor(string cols)
        {
            string[] vs = cols.Split(',');
            UnityEngine.Color c;
            c.r = float.Parse(vs[0]);
            c.g = float.Parse(vs[1]);
            c.b = float.Parse(vs[2]);
            c.a = float.Parse(vs[3]);

            return c;
        }

        static public string ToString(UnityEngine.Vector4 v)
        {
            return string.Format("{0},{1},{2},{3}", v.x, v.y, v.z, v.w);
        }

        static public string ToRect(UnityEngine.Rect v)
        {
            return string.Format("{0},{1},{2},{3}", v.x, v.y, v.width, v.height);
        }

        static public UnityEngine.Vector4 ToVector4(string cols)
        {
            string[] vs = cols.Split(',');
            UnityEngine.Vector4 v;
            v.x = float.Parse(vs[0]);
            v.y = float.Parse(vs[1]);
            v.z = float.Parse(vs[2]);
            v.w = float.Parse(vs[3]);

            return v;
        }

        static public string ToString(UnityEngine.Vector2 v)
        {
            return string.Format("{0},{1}", v.x, v.y);
        }

        static public UnityEngine.Vector2 ToVector2(string cols)
        {
            string[] vs = cols.Split(',');

            UnityEngine.Vector2 v;
            v.x = float.Parse(vs[0]);
            v.y = float.Parse(vs[1]);

            return v;
        }

        public const string programsuffix =
#if UNITY_IPHONE
            ".ipa";
#elif UNITY_ANDROID
            ".apk";
#elif UNITY_STANDALONE_WIN
            ".exe";
#else
            ".unknow";
#endif

        public static List<T> GetComponent<T>(List<UnityEngine.GameObject> objs) where T : UnityEngine.Component
        {
            List<T> l = new List<T>();
            T t = null;
            foreach (var obj in objs)
            {
                if ((t = obj.GetComponent<T>()) != null)
                {
                    l.Add(t);
                }
            }

            return l;
        }

        public static List<string> SplitURL(string text)
        {
            List<string> urls = new List<string>();
            string[] us = text.Split(',');
            foreach (string u in us)
            {
                if (!string.IsNullOrEmpty(u))
                    urls.Add(u);
            }

            return urls;
        }

        public static string UnionURL(List<string> urls)
        {
            if (urls.Count == 0)
                return "";
            else if (urls.Count == 1)
                return urls[0];

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(urls[0]);
            for (int i = 1; i < urls.Count; ++i)
            {
                sb.Append(",");
                sb.Append(urls[i]);
            }

            return sb.ToString();
        }

        // 两个map的比较，
        // common两个链接都有的key值
        // addnew t1有，t2没有的key值
        // remove t2有，t1没有的key值
        public static void Compare<T1, T2>(this Dictionary<T1, T2> t1, Dictionary<T1, T2> t2, System.Action<KeyValuePair<T1, T2>, KeyValuePair<T1, T2>> common, System.Action<KeyValuePair<T1, T2>> addnew, System.Action<KeyValuePair<T1, T2>> remove)
        {
            List<KeyValuePair<KeyValuePair<T1, T2>, KeyValuePair<T1, T2>>> commons = new List<KeyValuePair<KeyValuePair<T1, T2>, KeyValuePair<T1, T2>>>();
            List<KeyValuePair<T1, T2>> addnews = new List<KeyValuePair<T1, T2>>();
            List<KeyValuePair<T1, T2>> removes = new List<KeyValuePair<T1, T2>>();

            foreach (KeyValuePair<T1, T2> itor in t1)
            {
                T2 t2v;
                if (t2.TryGetValue(itor.Key, out t2v))
                {
                    // 都有的
                    commons.Add(new KeyValuePair<KeyValuePair<T1, T2>, KeyValuePair<T1, T2>>(itor, new KeyValuePair<T1, T2>(itor.Key, t2v)));
                }
                else
                {
                    // t1有，t2没有的，那么说明是
                    addnews.Add(itor);
                }
            }

            foreach (var itor in t2)
            {
                if (!t1.ContainsKey(itor.Key))
                {
                    // t1有,t2没有的，要移除的
                    removes.Add(itor);
                }
            }

            foreach (var itor in commons)
            {
                common(itor.Key, itor.Value);
            }

            foreach (var itor in addnews)
            {
                addnew(itor);
            }

            foreach (var itor in removes)
            {
                remove(itor);
            }
        }

#if UNITY_EDITOR
        static public bool WinZipPackZip(string srcpath, string dstfile = null)
        {
            if (srcpath[srcpath.Length - 1] == '/' || srcpath[srcpath.Length - 1] == '\\')
                srcpath = srcpath.Substring(0, srcpath.Length - 1);

            srcpath = srcpath.Replace('\\', '/');

            if (string.IsNullOrEmpty(dstfile))
                dstfile = srcpath + ".zip";

            dstfile = dstfile.Replace('\\', '/');
            if (File.Exists(dstfile))
                File.Delete(dstfile);
            else
            {
                Directory.CreateDirectory(dstfile.Substring(0, dstfile.LastIndexOfAny(new char[] { '\\', '/' })));
            }

    #if UNITY_EDITOR_WIN
            string winrar = "C:/Program Files/WinRAR/WinRAR.exe";
            if (System.IO.File.Exists(winrar))
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = winrar;
                string cmd = string.Format("a {0}.zip {0} -ep1 -m3", srcpath);
                p.StartInfo.Arguments = cmd;
                p.Start();
                p.WaitForExit();
                p.Close();

                if (dstfile != (srcpath + ".zip"))
                    File.Move(srcpath + ".zip", dstfile);
                return true;
            }
            else
            {
                return false;
            }
    #else
			ICSharpCode.SharpZipLib.Zip.ZipFile zips = new ICSharpCode.SharpZipLib.Zip.ZipFile(dstfile);
            string[] allfiles = System.IO.Directory.GetFiles(srcpath, "*", SearchOption.AllDirectories);
            string prefix = srcpath.Substring(srcpath.LastIndexOf('/') + 1);
            int srcindex = srcpath.Length;
            zips.BeginUpdate();
            string name;
            foreach (string file in allfiles)
            {
				PackTool.StreamDataSource sds = new PackTool.StreamDataSource(new FileStream(file, FileMode.Open));
                name = prefix + "/" + file.Substring(srcindex + 1);
				zips.Add(sds, name, ICSharpCode.SharpZipLib.Zip.CompressionMethod.Stored, true);
            }

            if (zips.IsUpdating)
                zips.CommitUpdate();

            zips.Close();

            Debuger.Log("生成文件包:" + System.IO.Path.GetFileName(dstfile));
            return true;
    #endif
        }
#endif
    }
}