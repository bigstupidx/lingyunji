#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using Object = UnityEngine.Object;

using UnityEditor;

namespace GUIEditor
{
    public partial class GuiTools
    {
        public static Enum EnumPopup(bool depth , Enum selected, params GUILayoutOption[] options)
        {
            return EnumPopup(depth, null, selected, options);
        }

        public static Enum EnumPopup(bool isIndentLevel, string label, Enum selected, params GUILayoutOption[] options)
        {
            Enum2Type.Etype etype = Enum2Type.To(selected.GetType());
            int value = etype.etype.FindIndex(selected);
            if (value == -1)
                value = 0;

            using (new GUIIndent(isIndentLevel))
            {
                if (string.IsNullOrEmpty(label))
                    value = EditorGUILayout.IntPopup(etype.etype.intValue[value], etype.etype.showTextList.ToArray(), etype.etype.intValue.ToArray(), options);
                else
                    value = EditorGUILayout.IntPopup(label, etype.etype.intValue[value], etype.etype.showTextList.ToArray(), etype.etype.intValue.ToArray(), options);

                int index = etype.etype.intValue.IndexOf(value);
                return etype.etype.valueList[index].value;
            }
        }

        public static string StringPopupT<T>(bool isIndentLevel, string selected, List<T> displayedOptions, Func<T, string> fun, params GUILayoutOption[] options)
        {
            List<string> ds = new List<string>();
            for (int i = 0; i < displayedOptions.Count; ++i)
                ds.Add(fun(displayedOptions[i]));

            return StringPopup(isIndentLevel, null, selected, ds, options);
        }

        public static string StringPopupT<T>(bool isIndentLevel, string label, string selected, List<T> displayedOptions, Func<T, string> fun, params GUILayoutOption[] options)
        {
            List<string> ds = new List<string>();
            for (int i = 0; i < displayedOptions.Count; ++i)
                ds.Add(fun(displayedOptions[i]));

            return StringPopup(isIndentLevel, label, selected, ds, options);
        }

        public static string StringPopup(bool isIndentLevel, string selected, List<string> displayedOptions, params GUILayoutOption[] options)
        {
            return StringPopup(isIndentLevel, null, selected, displayedOptions, options);
        }

        public static string StringPopup(bool isIndentLevel, string label, string selected, List<string> displayedOptions, params GUILayoutOption[] options)
        {
            int index = StringPopup(isIndentLevel, label, displayedOptions.IndexOf(selected), displayedOptions, options);
            return displayedOptions.Count == 0 ? selected : displayedOptions[index];
        }

        // selected当前选中的索引
        // displayedOptions当前显示的文本列表
        // 返回现在选中的索引
        public static int StringPopup(bool isIndentLevel, string label, int selected, List<string> displayedOptions, params GUILayoutOption[] options)
        {
            using (new GUIIndent(isIndentLevel))
            {
                return StringPopup(label, selected, displayedOptions, options);
            }
        }

        public static int StringPopup(string label, int selected, List<string> displayedOptions, params GUILayoutOption[] options)
        {
            if (selected < 0 || selected >= displayedOptions.Count)
                selected = 0;

            List<int> valueList = new List<int>();
            for (int i = 0; i < displayedOptions.Count; ++i)
                valueList.Add(i);

            if (!string.IsNullOrEmpty(label))
                selected = EditorGUILayout.IntPopup(label, selected, displayedOptions.ToArray(), valueList.ToArray(), options);
            else
                selected = EditorGUILayout.IntPopup(selected, displayedOptions.ToArray(), valueList.ToArray(), options);

            return selected;
        }

        public static T StringPopup<T>(string label, T selected, List<T> displayedOptions, System.Func<T, string> fun, params GUILayoutOption[] options)
        {
            if (!displayedOptions.Contains(selected) && displayedOptions.Count != 0)
                selected = displayedOptions[0];

            List<int> valueList = new List<int>();
            List<string> keyList = new List<string>();
            for (int i = 0; i < displayedOptions.Count; ++i)
            {
                valueList.Add(i);
                keyList.Add(fun(displayedOptions[i]));
            }

            int selectedindex = displayedOptions.IndexOf(selected);
            if (!string.IsNullOrEmpty(label))
                selectedindex = EditorGUILayout.IntPopup(label, selectedindex, keyList.ToArray(), valueList.ToArray(), options);
            else
                selectedindex = EditorGUILayout.IntPopup(selectedindex, keyList.ToArray(), valueList.ToArray(), options);

            if (selectedindex == -1)
                return selected;

            return displayedOptions[selectedindex];
        }

        public static string StringPopup(string label, string selected, List<string> displayedOptions, params GUILayoutOption[] options)
        {
            return displayedOptions[StringPopup(label, displayedOptions.IndexOf(selected), displayedOptions, options)];
        }

        public static K MapStringPopup<K,V>(
            string label, 
            K selectedValue, 
            IDictionary<K, V> maps, 
            Action<List<KeyValuePair<K, V>>> begin,
            Func<KeyValuePair<K, V>, string> keyfun,
            Action onintpopbegin,
            Action<K> onintpopend,
            Action<KeyValuePair<K, V>> onselect,
            params GUILayoutOption[] options)
        {
            List<KeyValuePair<K, V>> ls = new List<KeyValuePair<K, V>>();
            foreach (KeyValuePair<K, V> itor in maps)
                ls.Add(itor);

            if (begin != null)
                begin(ls);

            List<string> values = new List<string>();
            List<int> intV = new List<int>();
            int current = -1;

            foreach (KeyValuePair<K, V> itor in ls)
            {
                string key = keyfun(itor);
                values.Add(key);
                intV.Add(intV.Count);
                if (itor.Key.Equals(selectedValue))
                    current = values.Count - 1;
            }

            if (current == -1)
                current = 0;

            if (onintpopbegin != null)
                onintpopbegin();

            if (string.IsNullOrEmpty(label))
                current = EditorGUILayout.IntPopup(current, values.ToArray(), intV.ToArray(), options);
            else
                current = EditorGUILayout.IntPopup(label, current, values.ToArray(), intV.ToArray(), options);

            if (values.Count > current)
            {
                if (onintpopend != null)
                    onintpopend(ls[current].Key);

                if (onselect != null)
                {
                    onselect(ls[current]);
                }

                return ls[current].Key;
            }

            if (onintpopend != null)
                onintpopend(default(K));

            return default(K);
        }

        public static string MapStringPopup<T>(
            string label, 
            string selectedValue, 
            IDictionary<string, T> maps, 
            System.Action<List<KeyValuePair<string, T>>> begin, 
            Func<KeyValuePair<string, T>, string> fun, 
            Action<KeyValuePair<string, T>> onselect, 
            params GUILayoutOption[] options)
        {
            List<string> values = new List<string>();
            List<KeyValuePair<string, T>> vvs = new List<KeyValuePair<string, T>>();
            List<int> intV = new List<int>();
            Dictionary<string, string> showToKeys = new Dictionary<string, string>();
            string show = "";
            int current = -1;

            List<KeyValuePair<string, T>> ls = new List<KeyValuePair<string, T>>();
            foreach (KeyValuePair<string, T> itor in maps)
                ls.Add(itor);
            
            if (begin != null)
                begin(ls);

            foreach (KeyValuePair<string, T> itor in ls)
            {
                if (fun == null)
                    show = itor.Key;
                else
                    show = fun(itor);

                showToKeys[show] = itor.Key;

                if (itor.Key == selectedValue)
                    current = values.Count;

                values.Add(show);
                intV.Add(intV.Count);

                vvs.Add(itor);
            }

            if (current == -1)
                current = 0;

            if (string.IsNullOrEmpty(label))
                current = EditorGUILayout.IntPopup(current, values.ToArray(), intV.ToArray(), options);
            else
                current = EditorGUILayout.IntPopup(label, current, values.ToArray(), intV.ToArray(), options);

            if (values.Count > current)
            {
                if (onselect != null)
                {
                    onselect(vvs[current]);
                }

                return showToKeys[values[current]];
            }

            return null;
        }

        // selected当前选中的索引
        // displayedOptions当前显示的文本列表
        // 返回现在选中的索引
        public static int StringPopup(string label, bool isIndentLevel, int selected, List<string> displayedOptions, params GUILayoutOption[] options)
        {
            return StringPopup(isIndentLevel, label, selected, displayedOptions, options);
        }

        public static void TextureField(bool isIndentLevel, string text, Texture o)
        {
            TextureField<Texture>(isIndentLevel, text, o, (Texture t) => { return t; });
        }

        public static void TextureField<T>(bool isIndentLevel, string text, T o, Func<T, Texture> fun, Action<T> begin = null, Action<T> end = null, params GUILayoutOption[] options)
        {
            TextureField(fun == null ? (o as Texture) : fun(o), isIndentLevel,
                (Texture t) =>
                {
                    if (!string.IsNullOrEmpty(text))
                        EditorGUILayout.LabelField(text, GUILayout.Width(80f));

                    if (begin != null)
                        begin(o);
                },
                (Texture t) =>
                {
                    if (end != null)
                        end(o);
                }, options);
        }

        static string GetPlatformKey()
        {
#if UNITY_IPHONE
            return "iPhone";
#elif UNITY_ANDROID
            return "Android";
#else
            return "";
#endif
        }

        public static TextureFormat GetTextureImporterFormat(Texture t)
        {
            if (t == null)
            {
                throw new System.Exception("Texture2D t2d== null");
            }

            string path = AssetDatabase.GetAssetPath(t);
            if (string.IsNullOrEmpty(path))
            {
                Texture2D t2d = t as Texture2D;
                if (t2d != null)
                    return t2d.format;

                return TextureFormat.ARGB32;
            }

            TextureImporter ti = TextureImporter.GetAtPath(path) as TextureImporter;
            int maxsize = 0;
            TextureFormat desiredFormat;
            ColorSpace colorSpace;
            ti.ReadTextureImportInstructions(EditorUserBuildSettings.activeBuildTarget, out desiredFormat, out colorSpace, out maxsize);
            return desiredFormat;
        }

        class TData<T>
        {
            public List<T> list = new List<T>();
            public long totalsize = 0;
            public long totalmemory = 0;

            public int Count { get { return list.Count; } }

            public void Add(T t, Texture texture)
            {
                list.Add(t);
                totalsize += texture.width * texture.height;
                totalmemory += TextureMemorySize(texture);
            }
        }

        public static List<T> GetSelectTextureByTextureImporterFormat<T>(ParamList paramlist, bool isIndentLevel, List<T> objs, Func<T, Texture> fun, out bool ischange)
        {
            //using (new GUIIndent(isIndentLevel))
            {
                int num = 0;
                Dictionary<string, TData<T>> formatList = paramlist.Get<Dictionary<string, TData<T>>>("formatList");
                foreach (KeyValuePair<string, TData<T>> itor in formatList)
                    num += itor.Value.Count;

                if (formatList.Count == 0 || num != objs.Count)
                {
                    formatList.Clear();
                    foreach (T item in objs)
                    {
                        Texture t = (fun == null ? (item as Texture) : fun(item));
                        if (t == null)
                            continue;

                        TData<T> td = null;
                        string key;

                        if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(t)))
                        {
                            key = GetTextureImporterFormat(t).ToString();
                            //formatList[].Add(item);
                        }
                        else if ((t is Texture2D))
                        {
                            key = ((Texture2D)t).format.ToString();
                        }
                        else
                        {
                            key = "运行时资源";
                        }

                        if (!formatList.TryGetValue(key, out td))
                        {
                            td = new TData<T>();
                            formatList.Add(key, td);
                        }

                        td.Add(item, t);
                    }
                }

                ischange = false;
                if (formatList.Count <= 1)
                    return objs;

                Dictionary<string, string> keylist = new Dictionary<string, string>();
                List<string> itemList = new List<string>();
                itemList.Add("all");
                int nums = 0;
                foreach (KeyValuePair<string, TData<T>> itor in formatList)
                {
                    string tex = string.Format("{0} total:{1} size:{2} memory:{3}", itor.Key, itor.Value.Count, TextureSize(itor.Value.totalsize), XTools.Utility.ToMb(itor.Value.totalmemory));
                    itemList.Add(tex);
                    keylist.Add(tex, itor.Key);
                    nums += itor.Value.Count;
                }

                string texSize = paramlist.GetString("select");
                //Color color = UnityEngine.GUI.color;
                if (itemList.Count > 2)
                    texSize = StringPopup(isIndentLevel, "纹理格式", texSize, itemList);

                if (texSize != paramlist.GetString("select"))
                    ischange = true;
                else
                    ischange = false;

                paramlist.Set("select", texSize);

                List<T> textlists = new List<T>();
                if (texSize == "all" || itemList.Count <= 2)
                {
                    foreach (var itor in formatList)
                        textlists.AddRange(itor.Value.list);
                }
                else
                {
                    TData<T> list = null;
                    if (formatList.TryGetValue(keylist[texSize], out list))
                        textlists.AddRange(list.list);
                }

                return textlists;
            }
        }

        class Vector2Compare : IComparer<Vector2>
        {
            public int Compare(Vector2 x, Vector2 y)
            {
                float xs = x.x * x.y;
                float ys = y.x * y.y;
                return ys.CompareTo(xs);
            }
        }

        public static string TextureSize(long total)
        {
            long cs = 2048 * 2048;
            if (total > cs)
                return string.Format("{0}x(2048*2048)", (1.0f * total / cs).ToString("0.00"));

            return string.Format("{0}*{0}", Mathf.Sqrt(total).ToString());
        }

        static float PixelTextureSize(TextureFormat format)
        {
            switch (format)
            {
            case TextureFormat.ARGB32:
            case TextureFormat.RGBA32:
                return 4;
            case TextureFormat.RGB24:
                return 3;

            case TextureFormat.PVRTC_RGB2:
            case TextureFormat.PVRTC_RGB4:
            case TextureFormat.ETC2_RGB:
            case TextureFormat.ETC_RGB4:
            case TextureFormat.PVRTC_RGBA2:
            case TextureFormat.PVRTC_RGBA4:
            case TextureFormat.ETC2_RGBA1:
            case TextureFormat.ETC2_RGBA8:
            case TextureFormat.DXT1:
                return 0.5f;

            case TextureFormat.DXT5:
                return 1f;

            default:
                return 4;
            }
        }

        public static long TextureMemorySize(Texture texture)
        {
            return GetMemoryObject(texture);
        }

        public static long TextureMemorySizes(List<Texture> textures)
        {
            long total = 0;
            foreach (Texture t in textures)
                total += TextureMemorySize(t);

            return total;
        }

        public static List<T> GetSelectTextureByTextureSize<T>(ParamList paramlist, bool isIndentLevel, List<T> objs, Func<T, Texture> fun, out bool ischange)
        {
            long totalSize = paramlist.Get<long>("totalSize");
            long totalMemory = paramlist.Get<long>("totalMemory");

            // 序列好的列表
            int num = 0;
            if (!paramlist.Has("diclist"))
                paramlist.Set("diclist", new XTools.SortedMap<Vector2, List<T>>(new Vector2Compare()));
            XTools.SortedMap<Vector2, List<T>> diclist = paramlist.Get<XTools.SortedMap<Vector2, List<T>>>("diclist");
            foreach (KeyValuePair<Vector2, List<T>> itor in diclist)
                num += itor.Value.Count;
            if (diclist.Count == 0 || num != objs.Count)
            {
                diclist.Clear();
                foreach (T item in objs)
                {
                    Texture tex = (fun == null ? (item as Texture) : fun(item));
                    if (tex != null)
                    {
                        Vector2 size = new Vector2(tex.width, tex.height);
                        totalSize += (int)(size.x * size.y);
                        totalMemory += TextureMemorySize(tex);
                        if (!diclist.ContainsKey(size))
                            diclist.Add(size, new List<T>());

                        diclist[size].Add(item);
                    }
                }

                paramlist.Set("totalSize", totalSize);
                paramlist.Set("totalMemory", totalMemory);
            }

            if (totalSize > 0 && paramlist.Get<bool>("showTextureInfo", true))
                LabelField(isIndentLevel, string.Format("纹理大小:{0} 纹理个数:{1} 占用内存:{2}", TextureSize(totalSize), objs.Count, XTools.Utility.ToMb(totalMemory)));

            Dictionary<string, Vector2> keylist = new Dictionary<string, Vector2>();
            List<string> itemList = new List<string>();
            itemList.Add("all");
            int nums = 0;
            foreach (KeyValuePair<Vector2, List<T>> itor in diclist)
            {
                string tex = string.Format("{0}*{1} total:{2}", (int)itor.Key.x, (int)itor.Key.y, itor.Value.Count);
                itemList.Add(tex);
                keylist.Add(tex, itor.Key);
                nums += itor.Value.Count;
            }

            string texSize = paramlist.GetString("select");
            //Color color = UnityEngine.GUI.color;
            bool ismemory = paramlist.Get<bool>("ismemory");
            if (itemList.Count > 2)
            {
                GuiTools.HorizontalField(false, () => 
                {
                    texSize = StringPopup(isIndentLevel, "纹理大小", texSize, itemList);
                    if (GUILayout.Button(ismemory ? "内存排序" : "不排序", GUILayout.Width(60f)))
                    {
                        ismemory = !ismemory;
                    }
                    paramlist.Set("ismemory", ismemory);
                });
            }
            if (texSize != paramlist.GetString("select"))
            {
                ischange = true;
                ismemory = false;
            }
            else
            {
                ischange = false;
                ismemory = false;
            }

            paramlist.Set("select", texSize);

            List<T> textlists = new List<T>();
            if (texSize == "all" || itemList.Count <= 2)
            {
                foreach (KeyValuePair<Vector2, List<T>> itor in diclist)
                    textlists.AddRange(itor.Value);
            }
            else
            {
                List<T> list = null;
                if (diclist.TryGetValue(keylist[texSize], out list))
                    textlists.AddRange(list);
            }

            {
                if (ismemory)
                {
                    textlists.RemoveAll((T t) => { return t == null ? true : false; });
                    textlists.Sort((T y, T x) =>
                    {
                        return TextureMemorySize(fun(x)).CompareTo(TextureMemorySize(fun(y)));
                    });
                }
            }

            return textlists;
        }

        public static int GetSourceMaxSize(TextureImporter ti)
        {
            string assetPath = ti.assetPath;
            int type = FreeImage.GetFileType(assetPath);
            var ptr = FreeImage.Load(type, assetPath, 0);
            int w = FreeImage.GetWidth(ptr);
            int h = FreeImage.GetHeight(ptr);
            FreeImage.Unload(ptr);
            return Mathf.Max(w, h);
        }

        public static bool Set(TextureImporter textureImporter, string platform, TextureImporterFormat format, int maxSize = -1)
        {
            int maxTextureSize;
            TextureImporterFormat textureFormat;
            textureImporter.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat);
            if (maxSize == -1)
            {
                if (textureFormat != format)
                {
                    textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings() { maxTextureSize = maxTextureSize, format = textureFormat, name = platform });
                    return true;
                }
            }
            else
            {
                if (textureFormat != format || maxSize != maxTextureSize)
                {
                    textureImporter.SetPlatformTextureSettings(new TextureImporterPlatformSettings() { maxTextureSize = maxSize, format = textureFormat, name = platform });
                    return true;
                }
            }

            return false;
        }

        static bool SetScale(TextureImporter ti, float scale)
        {
            int maxSize = 0;
            if (scale <= 0)
            {
                maxSize = GetSourceMaxSize(ti);
            }
            else
            {
                maxSize = (int)(ti.maxTextureSize * scale);
            }

            ti.maxTextureSize = maxSize;
            return true;
        }

        static void ForEach<T>(List<T> objs, Func<T, Texture> fun, Func<TextureImporter, bool> onAction)
        {
            List<TextureImporter> tis = new List<TextureImporter>();
            objs.ForEach((T t) =>
            {
                string path = AssetDatabase.GetAssetPath(fun(t));
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti != null)
                {
                    if (onAction(ti))
                    {
                        tis.Add(ti);
                    }
                }
            });
            

            for (int i = 0; i < tis.Count; ++i)
            {
                TextureImporter ti = tis[i];
                AssetDatabase.ImportAsset(ti.assetPath);
            }

            if (tis.Count != 0)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }

        // 得到选中的纹理
        public static List<T> GetSelectTexture<T>(ParamList paramlist, bool isIndentLevel, List<T> objs, Func<T, Texture> fun)
        {
            //using (new GUIIndent(isIndentLevel))
            {
                bool ischange = false;
                objs = GetSelectTextureByTextureImporterFormat(paramlist.Get<ParamList>("GetSelectTextureByTextureImporterFormat"), isIndentLevel, objs, fun, out ischange);
                if (ischange)
                {
                    paramlist.Get<ParamList>("GetSelectTextureByTextureSize").ReleaseAll();

                    objs.Sort((T y, T x) =>
                    {
                        return TextureMemorySize(fun(x)).CompareTo(TextureMemorySize(fun(y)));
                    });
                }

                objs = GetSelectTextureByTextureSize(paramlist.Get<ParamList>("GetSelectTextureByTextureSize"), isIndentLevel, objs, fun, out ischange);

                GuiTools.HorizontalFieldAsTextArea(true, () => 
                {
                    if (GUILayout.Button("选中所有", GUILayout.Width(100f), GUILayout.Height(50f)))
                    {
                        List<Object> os = new List<Object>();
                        for (int i = 0; i < objs.Count; ++i)
                            os.Add(fun(objs[i]));

                        Selection.objects = os.ToArray();
                    }

#if UNITY_ANDROID
                    if (GUILayout.Button("纹理设置同步iOS", GUILayout.Width(100f), GUILayout.Height(50f)))
                    {
                        foreach(var obj in objs)
                        {
                            Texture t = fun(obj);
                            TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t));
                            TextureImporterPlatformSettings ios_tps = ti.GetPlatformTextureSettings("iPhone");
                            if (!ios_tps.overridden)
                                continue;

                            TextureImporterPlatformSettings ad_tps = ti.GetPlatformTextureSettings("Android");

                            TextureImporterFormat tif = ad_tps.format;
                            int size = ios_tps.maxTextureSize / 2;
                            switch (ios_tps.format)
                            {
                            case TextureImporterFormat.ASTC_RGBA_4x4:
                            case TextureImporterFormat.ASTC_RGBA_5x5:
                            case TextureImporterFormat.ASTC_RGBA_6x6:
                            case TextureImporterFormat.ASTC_RGBA_8x8:
                            case TextureImporterFormat.ASTC_RGBA_10x10:
                            case TextureImporterFormat.ASTC_RGBA_12x12:
                                tif = TextureImporterFormat.ETC2_RGBA8;
                                break;

                            case TextureImporterFormat.ASTC_RGB_4x4:
                            case TextureImporterFormat.ASTC_RGB_5x5:
                            case TextureImporterFormat.ASTC_RGB_6x6:
                            case TextureImporterFormat.ASTC_RGB_8x8:
                            case TextureImporterFormat.ASTC_RGB_10x10:
                            case TextureImporterFormat.ASTC_RGB_12x12:
                                tif = TextureImporterFormat.ETC_RGB4;
                                break;
                            }
                            if (tif != ad_tps.format || size != ad_tps.maxTextureSize || !ad_tps.overridden)
                            {
                                ad_tps.maxTextureSize = size;
                                ad_tps.format = tif;
                                ad_tps.overridden = true;

                                ti.SetPlatformTextureSettings(ad_tps);
                                EditorUtility.SetDirty(ti);
                                EditorUtility.SetDirty(t);
                                AssetDatabase.ImportAsset(ti.assetPath);
                                Debug.LogFormat("name:{0} format:{1} size:{2}", t.name, ad_tps.format, ad_tps.maxTextureSize);
                            }
                        }

                        AssetDatabase.Refresh();
                    }
#endif
                });

                return objs;
            }
        }

        public static void TextureListField(ParamList paramlist, bool isIndentLevel, string text, List<Texture> objs, Action<Texture> onGUITextureEnd = null, params GUILayoutOption[] options)
        {
            TextureListField<Texture>(paramlist, isIndentLevel, text, objs, (Texture t) => { return t; }, onGUITextureEnd);
        }

        // 
        public static void TextureListField<T>(ParamList paramlist, bool isIndentLevel, string text, List<T> objs, Func<T, Texture> fun, Action<T> onGUITextureEnd = null, Action<T> begin = null, Action<T> end = null, params GUILayoutOption[] options)
        {
            using (new GUIIndent(isIndentLevel))
            {
                List<T> textlists = GetSelectTexture<T>(paramlist, isIndentLevel, objs, fun);
                EditorPageBtn epb = paramlist.Get<EditorPageBtn>("EditorPageBtn");
                epb.pageNum = 20;
                epb.total = textlists.Count;
                if (epb.TotalPage >= 2)
                    epb.OnRender();

                int index = 0;

                ColorQueue cq = new ColorQueue();
                for (int i = epb.beginIndex; i < epb.endIndex; ++i, ++index)
                {
                    cq.Next();
                    TextureField<T>(isIndentLevel, (i + 1).ToString() + ") ", textlists[i], fun, begin, end, options);
                    if (onGUITextureEnd != null)
                    {
                        onGUITextureEnd(textlists[i]);
                    }
                }

                cq.Recover();
            }
        }

        static string GetTextureInfoByMaterials<T>(ParamList paramlist, List<T> objs, Func<T, Material> fun)
        {
            string textInfo = paramlist.GetString("allTextures:" + objs.GetHashCode());
            if (string.IsNullOrEmpty(textInfo))
            {
                HashSet<Texture> allTextures = paramlist.Get<HashSet<Texture>>("allTextures:" + objs.GetHashCode(), () => { return new HashSet<Texture>(); });
                for (int i = 0; i < objs.Count; ++i)
                {
                    allTextures.UnionWith(XTools.Utility.GetMaterialTexture(fun(objs[i])));
                }

                long totalSize = 0;
                long totalMemory = 0;
                foreach (Texture t in allTextures)
                {
                    totalMemory += TextureMemorySize(t);
                    totalSize += t.width * t.height;
                }

                textInfo = string.Format("TextureNum:{0} size:{1} memory:{2}", allTextures.Count, TextureSize(totalSize), XTools.Utility.ToMb(totalMemory));
                paramlist.Set("allTextures:" + objs.GetHashCode(), textInfo);
            }

            return textInfo;
        }

        // 得到选中的纹理
        public static List<T> GetSelectMaterial<T>(ParamList paramlist, bool isIndentLevel, List<T> objs, Func<T, Material> fun)
        {
            if (objs == null)
                return null;

            using (new GUIIndent(isIndentLevel))
            {
                XTools.SortedMap<string, List<T>> mats = paramlist.Get<XTools.SortedMap<string, List<T>>>("shaderTypeList");
                if (mats == null)
                {
                    mats = new XTools.SortedMap<string, List<T>>();
                    paramlist.Set("shaderTypeList", mats);
                }

                int total = 0;
                foreach (KeyValuePair<string, List<T>> itor in mats)
                    total += itor.Value.Count;

                if (total != objs.Count)
                {
                    mats.Clear();
                    foreach (T t in objs)
                    {
                        if (fun == null)
                            continue;

                        Material mat = fun(t);
                        if (mat.shader != null)
                        {
                            mats[mat.shader.name].Add(t);
                        }
                        else
                        {
                            mats["null"].Add(t);
                        }
                    }
                }

                string select = paramlist.GetString("select");
                EditorGUILayout.LabelField(string.Format("shader:{0} mat:{1} textInfo:{2}", mats.Count, objs.Count, GetTextureInfoByMaterials(paramlist, objs, fun)));
                List<KeyValuePair<string, List<T>>> keys = new List<KeyValuePair<string, List<T>>>();
                keys.Add(new KeyValuePair<string, List<T>>("all", null));
                KeyValuePair<string, List<T>> s = keys[0];
                foreach (KeyValuePair<string, List<T>> itor in mats)
                {
                    if (itor.Key == select)
                        s = itor;

                    keys.Add(itor);
                }

                s = GuiTools.StringPopup<KeyValuePair<string, List<T>>>(
                    "着色器",
                    s,
                    keys,
                    (KeyValuePair<string, List<T>> kv) =>
                    {
                        return kv.Key == "all" ? kv.Key : string.Format("{1} Num:{0}", kv.Value.Count, kv.Key);
                    });

                paramlist.Set("select", s.Key);
                select = s.Key;

                return select == "all" ? objs : mats[select];
            }
        }

        public static void MaterialListField<T>(ParamList paramlist, bool isIndentLevel, string text, List<T> objs, Func<T, Material> fun, Action<List<T>, ParamList> beginList = null, Action<T> onMaterialEnd = null, Action<T> begin = null, Action<T> end = null, params GUILayoutOption[] options)
        {
            using (new GUIIndent(isIndentLevel))
            {
                List<T> matlists = GetSelectMaterial<T>(paramlist, isIndentLevel, objs, fun);
                if (matlists == null)
                    return;

                EditorPageBtn epb = paramlist.Get<EditorPageBtn>("EditorPageBtn");
                epb.pageNum = 20;
                epb.total = matlists.Count;
                epb.OnRender();

                int index = 0;

                if (beginList != null)
                {
                    beginList(matlists, paramlist);
                }

                ColorQueue cq = new ColorQueue();
                Vector2 scrollposition = paramlist.Get<Vector2>("scrollposition");
                scrollposition = EditorGUILayout.BeginScrollView(scrollposition);
                paramlist.Set("scrollposition", scrollposition);
                for (int i = epb.beginIndex; i < epb.endIndex; ++i, ++index)
                {
                    cq.Next();
                    GuiTools.AnyObjectField<T>(isIndentLevel, matlists[i], (T t) => { return fun(t); }, begin, end, options);
                    if (onMaterialEnd != null)
                    {
                        onMaterialEnd(matlists[i]);
                    }
                }

                EditorGUILayout.EndScrollView();
                cq.Recover();
            }
        }

        // 得到选中的纹理
        public static List<T> GetSelectMesh<T>(ParamList paramlist, bool isIndentLevel, List<T> objs, Func<T, Mesh> fun,
            Action<List<KeyValuePair<string, Func<List<T>, List<T>>>>> onSelects = null,
            Action<List<KeyValuePair<string, Action<List<T>>>>> onSorts = null)
        {
            ParamList MeshParamList = paramlist.Get<ParamList>("GetSelectMesh");
            List<KeyValuePair<string, Func<List<T>, List<T>>>> keys = new List<KeyValuePair<string, Func<List<T>, List<T>>>>();
            keys.Add(new KeyValuePair<string, Func<List<T>, List<T>>>("all", null));
            keys.Add(new KeyValuePair<string, Func<List<T>, List<T>>>("含有颜色", 
                (List<T> os)=> 
                {
                    return os.FindAll(
                        (T k)=> 
                        {
                            if (fun(k).colors.Length == 0)
                                return false;
                            return true;
                        });
                }));

            if (onSelects != null)
                onSelects(keys);

            string selected = MeshParamList.GetString("selected");
            EditorGUILayout.BeginHorizontal();
            selected = GuiTools.StringPopupT<KeyValuePair<string, Func<List<T>, List<T>>>>(false, selected, keys, (KeyValuePair<string, Func<List<T>, List<T>>> v)=> { return v.Key; });
            MeshParamList.Set("selected", selected);
            List<T> tmplist = objs;
            for (int i = 0; i < keys.Count; ++i)
            {
                if (keys[i].Key == selected)
                {
                    if (keys[i].Value != null)
                        tmplist = keys[i].Value(objs);
                    break;
                }
            }

            if (tmplist != objs)
            {
                objs.Clear();
                objs.AddRange(tmplist);
            }

            // 排序类型
            string sorttype = MeshParamList.GetString("sorttype");
            List<KeyValuePair<string, Action<List<T>>>> sortkeys = new List<KeyValuePair<string, Action<List<T>>>>();
            //new List<string>(new string[] { "不排序", "顶点数", "三角形数", "内存" });//不导出中文
            sortkeys.Add(new KeyValuePair<string, Action<List<T>>>("不排序", null));
            sortkeys.Add(new KeyValuePair<string, Action<List<T>>>("顶点数", (List<T> tt)=> 
            {
                tt.Sort((T x, T y) =>
                {
                    Mesh xm = fun(x);
                    Mesh ym = fun(y);
                    if (xm == null)
                        return -1;
                    if (ym == null)
                        return 1;

                    return xm.vertexCount.CompareTo(ym.vertexCount);
                });
            }));
            sortkeys.Add(new KeyValuePair<string, Action<List<T>>>("三角形数", (List<T> tt)=> 
            {
                tt.Sort((T x, T y) =>
                {
                    Mesh xm = fun(x);
                    Mesh ym = fun(y);
                    if (xm == null)
                        return -1;
                    if (ym == null)
                        return 1;

                    MeshInfo xmi = GetMeshInfo(xm);
                    MeshInfo ymi = GetMeshInfo(ym);

                    return xmi.faceCount.CompareTo(ymi.faceCount);
                });
            }));
            sortkeys.Add(new KeyValuePair<string, Action<List<T>>>("内存", (List<T> tt)=> 
            {
                tt.Sort((T x, T y) =>
                {
                    Mesh xm = fun(x);
                    Mesh ym = fun(y);
                    if (xm == null)
                        return -1;
                    if (ym == null)
                        return 1;

                    MeshInfo xmi = GetMeshInfo(xm);
                    MeshInfo ymi = GetMeshInfo(ym);

                    return xmi.totalMemory.CompareTo(ymi.totalMemory);
                });
            }));

            if (onSorts != null)
                onSorts(sortkeys);

            sorttype = GuiTools.StringPopupT(false, sorttype, sortkeys, (KeyValuePair<string, Action<List<T>>> vv)=> { return vv.Key; });
            MeshParamList.Set("sorttype", sorttype);

            bool isdisc = MeshParamList.Get<bool>("sorttype_isdisc", true);
            EditorGUILayout.LabelField("升序", GUILayout.Width(30f));
            isdisc = EditorGUILayout.Toggle(isdisc, GUILayout.Width(20f));
            MeshParamList.Set("sorttype_isdisc", isdisc);

            for (int i = 0; i < sortkeys.Count; ++i)
            {
                if (sortkeys[i].Key == sorttype)
                {
                    if (sortkeys[i].Value != null)
                        sortkeys[i].Value(objs);
                    break;
                }
            }

            EditorGUILayout.EndHorizontal();

            if (isdisc)
            {
                objs.Reverse();
            }

            return objs;
        }

        private static string StringPopupT<T>(bool v, string selected, List<T> keys, Func<T, string> p)
        {
            List<string> strs = new List<string>();
            int select = -1;
            for (int i = 0; i < keys.Count; ++i)
            {
                string value = p(keys[i]);
                if (value == selected)
                    select = i;
                strs.Add(value);
            }

            using (new GUIIndent(v))
            {
                int s = EditorGUILayout.Popup(select, strs.ToArray());
                if (s < 0 || s >= strs.Count)
                    s = 0;

                return strs[s];
            }
        }

        public static void MeshListField<T>(ParamList paramlist, bool isIndentLevel, string text, List<T> objs, Func<T, Mesh> fun, Action<List<T>, ParamList> beginList = null, Action<T> onGUIEnd = null, Action<T> begin = null, Action<T> end = null, params GUILayoutOption[] options)
        {
            List<T> meshs = GetSelectMesh<T>(paramlist, isIndentLevel, objs, fun);
            EditorPageBtn epb = paramlist.Get<EditorPageBtn>("EditorPageBtn");
            epb.pageNum = 20;
            epb.total = meshs.Count;
            if (epb.TotalPage >= 2)
                epb.OnRender();

            int index = 0;

            if (beginList != null)
            {
                beginList(meshs, paramlist);
            }

            ColorQueue cq = new ColorQueue();
            Vector2 scrollposition = paramlist.Get<Vector2>("scrollposition");
            scrollposition = EditorGUILayout.BeginScrollView(scrollposition);
            paramlist.Set("scrollposition", scrollposition);
            for (int i = epb.beginIndex; i < epb.endIndex; ++i, ++index)
            {
                cq.Next();
                GuiTools.AnyObjectField<T>(isIndentLevel, meshs[i], (T t) => { return fun(t); }, begin, end, options);
                if (onGUIEnd != null)
                {
                    onGUIEnd(meshs[i]);
                }
            }

            EditorGUILayout.EndScrollView();
            cq.Recover();
        }
    }
}

#endif