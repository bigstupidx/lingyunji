using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UI;

namespace PackTool
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    class WXBInit
    {
        static WXBInit()
        {
            WXB.Tools.s_get_sprite = SpritesLoad.Get;
            WXB.Tools.s_get_cartoon = CartoonLoad.Get;
        }
    }

#endif

    public class SpritesLoad
    {
#if USE_RESOURCESEXPORT || USE_ABL
        static SpritesLoad()
        {
            isInit = false;
        }

        class SD : xys.UI.SpriteData
        {
            public AtlasSprite parent;
            public Sprite sprite; // 对应的精灵
        }

        class AtlasSprite
        {
            public string name;
            public string path;

            public Dictionary<string, SD> sprites = new Dictionary<string, SD>();
            public bool isInit = false;
#if USE_ABL
            public void Set(Dictionary<string, Sprite> dics)
            {
                isInit = true;
                foreach (var itor in sprites)
                {
                    var a = itor.Value;
                    if (a.sprite == null)
                    {
                        if (!dics.TryGetValue(a.name, out a.sprite))
                        {
                            Debug.LogErrorFormat("atlas:{0} name:{1} not find!", path, a.name);
                        }
                    }
                }
            }
#endif

#if USE_RESOURCESEXPORT
            public xys.UI.Atlas atlas;
            public void Set(xys.UI.Atlas a)
            {
                atlas = a;
                Sprite[] ss = atlas.Sprites;
                for (int i = 0; i < ss.Length; ++i)
                {
                    SD sd = null;
                    if (sprites.TryGetValue(ss[i].name, out sd))
                    {
                        sd.sprite = ss[i];
                    }
                    else
                    {
                        Debug.LogErrorFormat("atlas:{0} name:{1} not find!", a.name, ss[i].name);
                    }
                }
            }
#endif
        }

        static Dictionary<string, AtlasSprite> Atlases = new Dictionary<string, AtlasSprite>();

        static Dictionary<string, SD> Sprites = new Dictionary<string, SD>();

        public static bool isInit { get; protected set; }

        public static bool isAstc = false; // 是否使用astc贴图，如果有的话

        public static void Init()
        {
            if (isInit)
                return;

            Stream stream = ResourcesPack.FindBaseStream("sprites_atlas.b");
            if (stream == null)
            {
                isInit = true;
                return;
            }

            Debuger.DebugLog("atlas load astc:{0}!", isAstc);
            BinaryReader reader = new BinaryReader(stream);
            int lenght = reader.ReadInt32();
            string root = "Art/UIData/UData/Atlas/";
            for (int i = 0; i < lenght; ++i)
            {
                AtlasSprite sa = new AtlasSprite();
#if USE_ABL
                sa.path = reader.ReadString();
#else
                string text = reader.ReadString();
                string astcpath;
                if (isAstc && (ResourcesPack.IsExistFile(astcpath = string.Format("{0}{1}_astc.prefab", root, text.Substring(0, text.LastIndexOf('.'))))))
                {
                    sa.path = astcpath;
                }
                else
                {
                    sa.path = root + text;
                }
#endif
                sa.name = reader.ReadString();
                int sl = reader.ReadInt32();
                for (int j = 0; j < sl; ++j)
                {
                    SD sd = new SD();
                    sd.parent = sa;
                    sd.Read(reader);
                    if (!sa.sprites.ContainsKey(sd.name))
                        sa.sprites.Add(sd.name, sd);
                    else
                    {
                        Debug.LogErrorFormat("Atlas:{0} sprite:{1} repate!", sa.name, sd.name);
                    }

                    if (!Sprites.ContainsKey(sd.name))
                        Sprites.Add(sd.name, sd);
                    else
                    {
                        Debug.LogErrorFormat("atlas:{0} name:{1} {2} repate!", sa.name, sd.name, Sprites[sd.name].parent.name);
                    }
                }

                Atlases.Add(sa.name, sa);
            }

            isInit = true;
        }

        public static Sprite Get(string name)
        {
            SD sd = null;
            if (Sprites.TryGetValue(name, out sd))
                return sd.sprite;

            return null;
        }

        public static Sprite has(string name)
        {
            return Get(name);
        }

        public static bool Get(string name, out Sprite s)
        {
            SD sd = null;
            if (Sprites.TryGetValue(name, out sd))
            {
                s = sd.sprite;
                return true;
            }

            s = null;
            return false;
        }

        public static void Load(string name, ResourcesEnd<Sprite> fun, object p)
        {
            XTools.TimerFrame.UPDATE callfun = (object o)=> 
            {
                if (fun != null)
                    fun((Sprite)o, p);

                return false;
            };

            if (string.IsNullOrEmpty(name))
            {
                XTools.TimerMgrObj.Instance.addFrameLateUpdate(callfun, null);
                return;
            }

#if USE_RESOURCESEXPORT
            if (name[0] == ':')
            {
                XTools.TimerMgrObj.Instance.addFrameLateUpdate(callfun, BuiltinResource.Instance.GetSprite(name.Substring(1)));
                return;
            }
#endif
            SD sd = null;
            if (!Sprites.TryGetValue(name, out sd))
            {
                Debug.LogErrorFormat("Sprite:{0} not find!", name);
                XTools.TimerMgrObj.Instance.addFrameLateUpdate(callfun, null);

                return;
            }

            if (sd.sprite != null)
            {
                XTools.TimerMgrObj.Instance.addFrameLateUpdate(callfun, sd.sprite);
            }
            else
            {
                // 需要动态去加载
#if USE_ABL
                ABL.AssetsMgr.LoadAtlas(sd.parent.path.ToLower(), (Dictionary<string, Sprite> dic) => 
                {
                    if (dic == null)
                    {
                        XTools.TimerMgrObj.Instance.addFrameLateUpdate(callfun, null);
                        return;
                    }

                    if (!sd.parent.isInit)
                    {
                        sd.parent.Set(dic);
                    }

                    dic.TryGetValue(name, out sd.sprite);
                    callfun(sd.sprite);
                });
#elif USE_RESOURCESEXPORT
                AtlasLoad.Load(sd.parent.path, (xys.UI.Atlas atlas, object o)=> 
                {
                    if (atlas == null)
                    {
                        XTools.TimerMgrObj.Instance.addFrameLateUpdate(callfun, null);
                        return;
                    }

                    if (sd.parent.atlas == null)
                        sd.parent.Set(atlas);

                    callfun(sd.sprite);
                }, null);
#endif
            }
        }
#else
        static Dictionary<string, Sprite> Sprites = null;
        public static bool isInit { get { return Sprites == null ? false : true; } }

        public static void Init()
        {
            if (isInit)
                return;

            Sprites = new Dictionary<string, Sprite>();

#if USE_RESOURCES || USER_ALLRESOURCES
            InitAllAtlas();
#elif UNITY_EDITOR
            List<Sprite> sprites = XTools.Utility.FindSprites("Assets/Art/UIData/UData/Sprites/Icon");
            foreach (var o in sprites)
            {
                if (!Sprites.ContainsKey(o.name))
                    Sprites.Add(o.name, o);
            }
#endif
        }

        public static Sprite Get(string name)
        {
            Sprite s = has(name);
            if (s == null)
                Debug.LogErrorFormat("Sprite:{0} not find!", name);
            return s;
        }

        public static Sprite has(string name)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(name))
                return null;

            Init();
#endif
            Sprite s = null;
            if (Sprites.TryGetValue(name, out s))
                return s;

            return s;
        }
#endif

#if !USE_RESOURCESEXPORT && !USE_ABL
        public static void Load(string name, ResourcesEnd<Sprite> fun, object p)
        {

        }
#endif

#if USE_RESOURCES || USER_ALLRESOURCES
        static void InitAllAtlas()
        {
            xys.UI.Atlas atlas = Resources.Load<xys.UI.Atlas>("AllAtlas");
            if (atlas != null)
            {
                atlas.Init();
                Sprites = atlas.SpriteMap;
            }
            else
            {
                Sprites = new Dictionary<string, Sprite>();
            }
        }
#endif

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/PackTool/CreateIconAtlas")]
        public static void CreateIconAtlas()
        {
            CreateIconAtlas(true);
        }

        public static void CreateIconAtlas(bool isupdate)
        {
            xys.UI.Atlas atlas = Resources.Load<xys.UI.Atlas>("AllAtlas");
            if (atlas == null)
            {
                GameObject go = new GameObject("AllAtlas");
                go.AddComponent<xys.UI.Atlas>();
                System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources");
                UnityEditor.AssetDatabase.Refresh();
                GameObject copygo = UnityEditor.PrefabUtility.CreatePrefab("Assets/Resources/AllAtlas.prefab", go);
                Object.DestroyImmediate(go);

                atlas = copygo.GetOrAddComponent<xys.UI.Atlas>();
            }

            List<Sprite> ss = new List<Sprite>();
            foreach (Sprite s in XTools.Utility.FindSprites("Assets/Art/UIData/UData/Sprites/Icon"))
            {
                ss.Add(s);
            }

            atlas.Sprites = ss.ToArray();

            UnityEditor.EditorUtility.SetDirty(atlas.gameObject);

            if (isupdate)
            {
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        public static void DeleteAllAtlas()
        {
            xys.UI.Atlas atlas = Resources.Load<xys.UI.Atlas>("AllAtlas");
            if (atlas != null)
            {
                UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(atlas.gameObject));
            }
        }
#endif
    }
}