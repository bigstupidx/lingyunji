#if UNITY_EDITOR && (USE_RESOURCESEXPORT || USE_ABL)
using xys.UI.State;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;

namespace xys.UI
{
    // 检测所有的面板，并且规划好精灵的分类
    public class CheckAllPanel
    {
        public class Sprites
        {
            private Sprite sprite_; // 精灵

            public Sprite sprite
            {
                get { return sprite_; }
                set
                {
                    sprite_ = value;
                    if (sprite_ != null)
                    {
                        assetPath = AssetDatabase.GetAssetPath(sprite_);
//                         if (!string.IsNullOrEmpty(assetPath))
//                             assetPath = assetPath.Substring("Assets/Art/UIData/UData/Sprites/".Length);
                    }
                    else
                        assetPath = "";
                }
            }

            public string assetPath { get; private set; }

            // 精灵的类型与参数
            public SpriteType spriteType;
            public string modleType; // 如果类型为模型，那么这里为属于哪个模块的

            public bool iscommon = false; // 是否公共图集

            public Dictionary<string, List<Behaviour>> behaviours = new Dictionary<string, List<Behaviour>>();

            public bool isDirty = false;

            static void SetBehaviourSprite(Behaviour b, Sprite src, Sprite dst)
            {
                bool isdirty = false;
                if (b is Image)
                {
                    Image image = b as Image;
                    if (image.sprite == src)
                    {
                        image.sprite = dst;
                        isdirty = true;
                    }
                }
                else if (b is StateRoot)
                {
                    StateRoot sr = b as StateRoot;
                    List<Element> elements = sr.elements;
                    for (int j = 0; j < elements.Count; ++j)
                    {
                        ElementStateData[] eds = elements[j].stateData;
                        for (int m = 0; m < eds.Length; ++m)
                        {
                            ElementStateData esd = eds[m];
                            if (esd.obj is Sprite && ((Sprite)esd.obj == src))
                            {
                                esd.obj = dst;
                                isdirty = true;
                            }
                        }
                    }
                }

                if (isdirty)
                {
                    EditorUtility.SetDirty(b);
                }
            }

            public void SetSpriteNew(Sprite s)
            {
                foreach (var itor in behaviours)
                {
                    foreach (var b in itor.Value)
                        SetBehaviourSprite(b, sprite, s);
                }

                AssetDatabase.SaveAssets();
            }

            public List<string> use_panels
            {
                get
                {
                    var p = new List<string>(behaviours.Keys);
                    p.Remove(string.Empty);
                    return p;
                }
            }
            
            public int totalBehaviour
            {
                get
                {
                    int total = 0;
                    foreach (var itor in behaviours)
                    {
                        total += itor.Value.Count;
                    }

                    return total;
                }
            }

            public void AddBehaviour(string type, Behaviour b)
            {
                List<Behaviour> ims = null;
                if (!behaviours.TryGetValue(type, out ims))
                {
                    ims = new List<Behaviour>();
                    behaviours.Add(type, ims);
                }

                ims.Add(b);
            }

            public override string ToString()
            {
                return string.Format("size:{0} Panel Total:{1} behaviour Total:{2} memory:{3}", sprite.rect.size, use_panels.Count, totalBehaviour, XTools.Utility.ToMb(GUIEditor.GuiTools.TextureMemorySize(sprite.texture)));
            }

            public string spritePackingTag;
        }

        public List<Sprites> spritesList = new List<Sprites>();

        // 精灵的使用情况
        public Dictionary<Sprite, Sprites> SpriteList = new Dictionary<Sprite, Sprites>();

        public class Panel
        {
            public List<Sprites> sprites = new List<Sprites>();
        }

        SortedDictionary<string, Panel> Panels = new SortedDictionary<string, Panel>();

        public void ForEach(System.Action<string, Panel> fun)
        {
            foreach (var itor in Panels)
            {
                fun(itor.Key, itor.Value);
            }
        }

        public Panel Get(string type)
        {
            Panel p = null;
            if (Panels.TryGetValue(type, out p))
                return p;

            return null;
        }

        public int total
        {
            get
            {
                return Panels.Count;
            }
        }

        static MethodInfo GetMethod(System.Type type, string name, BindingFlags flags)
        {
            var method = type.GetMethod(name, flags);
            if (method != null)
                return method;

            if (type.BaseType != null)
                return GetMethod(type.BaseType, name, flags);

            return null;
        }

        public void Check(UIPanelBase pb)
        {
            var method = GetMethod(pb.GetType(), "GetPanelType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SuppressChangeType);
            if (method == null)
            {
                Debug.LogErrorFormat("{0} not find Method:GetPanelType", pb.GetType());
                return;
            }

            string type = (string)(method.Invoke(pb, new object[] { }));
            if (string.IsNullOrEmpty(type))
            {
                Debug.LogErrorFormat("PanelType:{0} not find type path:{1}!", pb.GetType().Name, AssetDatabase.GetAssetPath(pb));
                return;
            }

            Panel p = new Panel();
            Panels[type] = p;

            Check(pb.gameObject, (Sprites ss, Behaviour b) => 
            {
                ss.AddBehaviour(type, b);
                if (!p.sprites.Contains(ss))
                    p.sprites.Add(ss);
            });
        }

        public Sprites GetOrCreate(Sprite s)
        {
            Sprites ss = null;
            if (!SpriteList.TryGetValue(s, out ss))
            {
                ss = new Sprites();
                SpriteList.Add(s, ss);
                ss.sprite = s;

                spritesList.Add(ss);
            }

            return ss;
        }

#if USE_RESOURCESEXPORT
        public void Check(GameObject go, System.Action<Sprites, Behaviour> fun)
        {
            bool isActive = go.activeSelf;
            if (isActive)
                go.SetActive(false);

            PackTool.AEDelegation add = null;
            add = new PackTool.AEDelegation()
            {
                _CollectSprite = (Sprite s) => 
                {
                    if (s == null)
                        return;

                    string assetPath = AssetDatabase.GetAssetPath(s);
                    if (string.IsNullOrEmpty(assetPath) || !assetPath.StartsWith("Assets/"))
                        return;

                    Sprites ss = GetOrCreate(s);
                    if (fun != null)
                    {
                        fun(ss, PrefabUtility.GetPrefabParent(add.current) as Behaviour);
                    }
                }
            };

            GameObject instance = PrefabUtility.InstantiatePrefab(go) as GameObject;
            instance.name = go.name;
            PackTool.ComponentSave cs = new PackTool.ComponentSave();
            cs.Save(instance, add);
            Object.DestroyImmediate(instance);

            if (isActive)
                go.SetActive(true);
        }
#endif
#if USE_ABL
        public void Check(GameObject go, System.Action<Sprites, Behaviour> fun)
        {
            System.Action<Sprite, Behaviour> exefun = (Sprite s, Behaviour b) =>
            {
                if (s == null)
                    return;

                string assetPath = AssetDatabase.GetAssetPath(s);
                if (string.IsNullOrEmpty(assetPath) || !assetPath.StartsWith("Assets/"))
                    return;

                //if (!assetPath.StartsWith("Assets/Art/UIData/UData/Sprites"))
                //{
                //    Debug.LogErrorFormat("name:{0} 引用错误的精灵:{1}资源!", AssetDatabase.GetAssetPath(go).Substring(7), s.name);
                //}

                Sprites ss = GetOrCreate(s);
                if (fun != null)
                {
                    fun(ss, b);
                }
            };

            List<Image> images = new List<Image>();
            go.GetComponentsInChildren(true, images);
            for (int i = 0; i < images.Count; ++i)
            {
                exefun(images[i].sprite, images[i]);
            }

            List<StateRoot> stateRoots = new List<State.StateRoot>();
            go.GetComponentsInChildren(true, stateRoots);
            for (int i = 0; i < stateRoots.Count; ++i)
            {
                StateRoot sr = stateRoots[i];
                List<Element> elements = sr.elements;
                for (int j = 0; j < elements.Count; ++j)
                {
                    ElementStateData[] eds = elements[j].stateData;
                    for (int m = 0; m < eds.Length; ++m)
                    {
                        ElementStateData esd = eds[m];
                        if (esd.obj is Sprite)
                        {
                            exefun((Sprite)esd.obj, sr);
                        }
                    }
                }
            }

            List <global::UI.LineImage> lineImages = new List<global::UI.LineImage>();
            go.GetComponentsInChildren(true, lineImages);
            for (int i = 0; i < lineImages.Count; ++i)
            {
                exefun(lineImages[i].m_Sprite, images[i]);
                exefun(lineImages[i].m_XSprite, images[i]);
                exefun(lineImages[i].m_LSprite, images[i]);
                exefun(lineImages[i].m_TSprite, images[i]);
            }
        }
#endif
        static void SetSpriteTag(Sprites sprite, string spritePackingTag, bool setformat, TextureImporterCompression format)
        {
#if USE_ABL
            if (!string.IsNullOrEmpty(spritePackingTag))
                spritePackingTag = string.Format("{0}{1}.atlas", atlas_root.Substring(7), spritePackingTag);
#endif
            TextureImporter ti = AssetImporter.GetAtPath(sprite.assetPath) as TextureImporter;
            bool isdirty = false;
            if (!string.Equals(spritePackingTag, ti.spritePackingTag))
            {
                ti.spritePackingTag = spritePackingTag;
                isdirty = true;
            }

            sprite.spritePackingTag = spritePackingTag;

            if (setformat)
            {
                Vector2 size = sprite.sprite.rect.size;
                if (size.x * size.y >= 512 * 512)
                {
                    // 这么大，还是压缩下吧
                    format = TextureImporterCompression.Compressed;
                }

                if (ti.textureCompression != format)
                {
                    ti.textureCompression = format;
                    ti.ClearPlatformTextureSettings("Android");
                    ti.ClearPlatformTextureSettings("iPhone");
                    isdirty = true;
                }
            }

            if (ti.mipmapEnabled && !string.IsNullOrEmpty(spritePackingTag))
            {
                ti.mipmapEnabled = false;
                isdirty = true;
            }

            if (!string.IsNullOrEmpty(ti.userData))
            {
                ti.userData = null;
                isdirty = true;
            }

            sprite.isDirty = isdirty;
            if (isdirty)
            {
                EditorUtility.SetDirty(ti);
                AssetDatabase.ImportAsset(sprite.assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        // 有些特殊的面板会一直存在，比如说主界面，也就是战斗界面
        static HashSet<string> CommonPanels = new HashSet<string>(new string[] 
        {
            "UIBattlePanel",
        });

        // 分析精灵的类型
        public void AnalysisType(bool setformat = false)
        {
            for (int i = 0; i < spritesList.Count; ++i)
            {
                Sprites sprites = spritesList[i];
                //if (sprites.spriteType == SpriteType.Icon)
                //{
                //    SetSpriteTag(sprites.sprite, "Common", setformat, TextureImporterFormat.AutomaticCompressed);
                //    continue;
                //}

                //Rect rect = sprites.sprite.rect;
                int panelTotal = sprites.use_panels.Count;
                if (sprites.iscommon)
                {
                    sprites.spriteType = SpriteType.Common;
                    SetSpriteTag(sprites, "Common", setformat, TextureImporterCompression.Uncompressed);
                }
                else if (panelTotal == 1)
                {
                    // 只有一个面板用到，那么是属于模块类型的资源
                    sprites.spriteType = SpriteType.Module;
                    sprites.modleType = sprites.use_panels[0];

                    SetSpriteTag(sprites, "Module-" + sprites.modleType.ToString(), setformat, TextureImporterCompression.Compressed);
                }
                else if (panelTotal >= 2)
                {
                    // 大于2个面板使用到，定义为公用资源
                    sprites.spriteType = SpriteType.Common;

                    SetSpriteTag(sprites, "Common", setformat, TextureImporterCompression.Uncompressed);
                }
                else
                {
                    sprites.spriteType = SpriteType.Null;

                    // 没有任何面板使用到的资源，这种资源可有可无
                    SetSpriteTag(sprites, string.Empty, setformat, TextureImporterCompression.Uncompressed);
                }
            }
        }

        static public void FindAllPrefab(List<string> common, List<string> panels)
        {
            string dic_path = "Assets/Art/UIData";
            XTools.Utility.ForEach(dic_path, (AssetImporter ai)=> { }, (string assetPath, string root)=> 
            {
                if (assetPath.Contains("/ResourcesExport/") && assetPath.EndsWith(".prefab", true, null))
                {
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (go != null)
                    {
                        UIPanelBase pb = go.GetComponent<UIPanelBase>();
                        if (pb != null && !CommonPanels.Contains(pb.panelType))
                        {
                            Debug.LogFormat("panel:{0}", assetPath);
                            panels.Add(assetPath);
                        }
                        else
                            common.Add(assetPath);
                    }
                }

                return false;
            });
        }

        static public List<UIPanelBase> FindAllPanel()
        {
            string dic_path = "Assets/Art/UIData/Data/Prefabs/UIPanel/ResourcesExport";
            List<UIPanelBase> panels = new List<UIPanelBase>();
            XTools.Utility.ForEach(dic_path, (AssetImporter ai) => { }, (string assetPath, string root) => 
            {
                UIPanelBase panel = AssetDatabase.LoadAssetAtPath<UIPanelBase>(assetPath);
                if (panel != null)
                    panels.Add(panel);

                return false;
            });

            return panels;
        }

        [MenuItem("Assets/PackTool/CheckAllPanel")]
        public static void CheckAllPanelMenu()
        {
            StartCheckAllPanel();
        }

        public static CheckAllPanel StartCheckAllPanel()
        {
            CheckAllPanel cap = new CheckAllPanel();
            List<Sprite> allSprites = XTools.Utility.FindAllSprites();
            for (int i = 0; i < allSprites.Count; ++i)
            {
                Sprites ss = new Sprites();
                ss.sprite = allSprites[i];
                if (isIconType(ss.sprite))
                {
                    ss.spriteType = SpriteType.Common;
                    ss.iscommon = true;
                }

                cap.SpriteList.Add(allSprites[i], ss);
                cap.spritesList.Add(ss);
            }

            List<string> common = new List<string>();
            List<string> panels = new List<string>();
            FindAllPrefab(common, panels);
            for (int i = 0; i < panels.Count; ++i)
            {
                cap.Check(AssetDatabase.LoadAssetAtPath<UIPanelBase>(panels[i]));
            }
            for (int i = 0; i < common.Count; ++i)
            {
                cap.Check(AssetDatabase.LoadAssetAtPath<GameObject>(common[i]), (Sprites ss, Behaviour behaviour) => 
                {
                    ss.AddBehaviour(string.Empty, behaviour);
                    ss.iscommon = true;
//                     if (ss.use_panels.Count == 0)
//                         ss.iscommon = true;
                });
            }

            // 查找下Main场景，此场景内的引用的精灵也属于公用
            string[] deps = AssetDatabase.GetDependencies("Assets/Art/UIData/Levels/SceneExport/main.unity");
            for (int i = 0; i < deps.Length; ++i)
            {
                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(deps[i]);
                if (s != null)
                {
                    Sprites ss = cap.GetOrCreate(s);
                    ss.iscommon = true;
                }
            }

            // 分析精灵的类型
            cap.AnalysisType(false);

            return cap;
        }

        // 是否为通用ICON类型资源
        public static bool isIconType(Sprite sprite)
        {
            string assetPath = AssetDatabase.GetAssetPath(sprite);
            if (assetPath.StartsWith("Assets/Art/UIData/UData/Sprites/Icon/"))
                return true;

            return false;
        }

        public const string atlas_root = "Assets/__copy__/Art/UIData/UData/Atlas/";

        public void UpdateAtlas()
        {
            CreateAtlas();
        }

        class AtlasData
        {
            //public Atlas atlas;
            public HashSet<Sprite> waitAddList = new HashSet<Sprite>();
            public string assetPath { get; set; }

            public void AddSprite(Sprite s)
            {
                waitAddList.Add(s);
            }

#if USE_UATLAS
            public void uUpdateToFile()
            {
                List<Sprite> sprites = new List<Sprite>(waitAddList);
                sprites.RemoveAll((Sprite x) => { return x == null; });
                uAtlasTools.CreateuAtlas(sprites.ToArray(), assetPath);
            }
#endif

            static void SaveToFile(string assetPath, List<Sprite> sprites)
            {
                bool isDirty = false;
                Atlas atlas = uAtlasTools.GetOrCreate<Atlas>(assetPath);

                if (atlas.Sprites.Length != sprites.Count)
                {
                    isDirty = true;
                }
                else
                {
                    for (int i = 0; i < sprites.Count; ++i)
                    {
                        if (atlas.Sprites[i] == sprites[i])
                            continue;

                        isDirty = true;
                        break;
                    }
                }

                if (isDirty)
                {
                    if (atlas == null)
                    {
                        atlas = AssetDatabase.LoadAssetAtPath<Atlas>(assetPath);
                    }

                    GameObject go = atlas.gameObject;
                    atlas.Sprites = sprites.ToArray();

                    EditorUtility.SetDirty(go);
                    AssetDatabase.SaveAssets();
                }
            }

            public void UpdateToFile()
            {
                List<Sprite> sprites = new List<Sprite>(waitAddList);
                sprites.RemoveAll((Sprite x) => { return x == null; });
                sprites.Sort((Sprite x, Sprite y) => { return x.name.CompareTo(y.name); });

                SaveToFile(assetPath, sprites);

#if UNITY_IOS
                SaveToFile(assetPath.Substring(0, assetPath.LastIndexOf('.')) + "_astc.prefab", sprites);
#endif
            }
        }

        // 创建图集
        public void CreateAtlas()
        {
            Dictionary<string, AtlasData> atlas = new Dictionary<string, AtlasData>();
            //AtlasData allAtlas = GetOrCreate(new Dictionary<string, AtlasData>(), string.Format("{0}All.prefab", atlas_root));
            foreach (Sprites sprites in spritesList)
            {
                List<AtlasData> atlaslist = GetAtlas(atlas, sprites);
                for (int i = 0; i < atlaslist.Count; ++i)
                {
                    atlaslist[i].AddSprite(sprites.sprite);
                    //allAtlas.AddSprite(sprites.sprite);
                }
            }

            foreach (var itor in atlas)
            {
                itor.Value.UpdateToFile();
            }

            //allAtlas.UpdateToFile();
        }

#if USE_UATLAS
        [MenuItem("Assets/CreateUAtlas")]
        static void CreateUAtlas()
        {
            XTools.TimeCheck tc = new XTools.TimeCheck(true);
            StartCheckAllPanel().CreateuAtlas();
            Debug.LogFormat("CreateUAtlas time:{0}", tc.delay);
        }

        public void CreateuAtlas()
        {
            Dictionary<string, AtlasData> atlas = new Dictionary<string, AtlasData>();
            foreach (Sprites sprites in spritesList)
            {
                List<AtlasData> atlaslist = GetAtlas(atlas, sprites);
                for (int i = 0; i < atlaslist.Count; ++i)
                {
                    atlaslist[i].AddSprite(sprites.sprite);
                }
            }

            foreach (var itor in atlas)
            {
                //if (itor.Key.Contains("UIActivityPanel"))
                    itor.Value.uUpdateToFile();
            }
        }
#endif

        static List<AtlasData> GetAtlas(Dictionary<string, AtlasData> atlass, Sprites sprites)
        {
            List<AtlasData> atlasList = new List<AtlasData>();
            switch (sprites.spriteType)
            {
            case SpriteType.Common:
                {
                    atlasList.Add(GetOrCreate(atlass, string.Format("{0}Common.prefab", atlas_root)));
                }
                break;
            case SpriteType.Module:
                {
                    atlasList.Add(GetOrCreate(atlass, string.Format("{0}Module/{1}.prefab", atlas_root, sprites.modleType)));
                }
                break;
            //case SpriteType.Icon:
            //    {
            //        atlasList.Add(GetOrCreate(atlass, string.Format("{0}Icon.prefab", atlas_root)));
            //    }
            //    break;
            }

            return atlasList;
        }

        static AtlasData GetOrCreate(Dictionary<string, AtlasData> atlass, string path)
        {
            AtlasData ad = null;
            if (atlass.TryGetValue(path, out ad))
                return ad;

            ad = new AtlasData();
            ad.assetPath = path;

            //GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            //if (go == null)
            //{
            //    Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/')));
            //    go = new GameObject(Path.GetFileNameWithoutExtension(path));
            //    PrefabUtility.CreatePrefab(path, go);
            //    Object.DestroyImmediate(go);
            //    AssetDatabase.SaveAssets();
            //    AssetDatabase.Refresh();

            //    go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            //    ad.atlas = go.AddComponent<Atlas>();
            //    EditorUtility.SetDirty(go);
            //    AssetDatabase.SaveAssets();
            //}

            //ad.atlas = go.GetOrAddComponent<Atlas>();

            atlass.Add(path, ad);
            return ad;
        }

        public static Dictionary<string, List<Sprite>> ExportSpriteInfo(string filepath)
        {
            CheckAllPanel cap = StartCheckAllPanel();
            UnityEditor.Sprites.Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget);

            Dictionary<string, List<Sprite>> spritePackToSprite = new Dictionary<string, List<Sprite>>();
            foreach (Sprites sprite in cap.spritesList)
            {
                if (!string.IsNullOrEmpty(sprite.spritePackingTag))
                {
                    List<Sprite> items = null;
                    if (!spritePackToSprite.TryGetValue(sprite.spritePackingTag, out items))
                    {
                        items = new List<Sprite>();
                        spritePackToSprite.Add(sprite.spritePackingTag, items);
                    }

                    items.Add(sprite.sprite);
                }
            }

            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(spritePackToSprite.Count);
            foreach (var itor in spritePackToSprite)
            {
                writer.Write(itor.Key);
                writer.Write(Path.GetFileNameWithoutExtension(itor.Key));
                writer.Write(itor.Value.Count);
                foreach (var sprite in itor.Value)
                {
                    writer.Write(sprite.name);
                    writer.Write((short)sprite.rect.size.x);
                    writer.Write((short)sprite.rect.size.y);
                }
            }

            Directory.CreateDirectory(filepath.Substring(0, filepath.LastIndexOf('/')));
            File.WriteAllBytes(filepath, ms.ToArray());
            ms.Close();
            writer.Close();

            return spritePackToSprite;
        }
    }
}
#endif