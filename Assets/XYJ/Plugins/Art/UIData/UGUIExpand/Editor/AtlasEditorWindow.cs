using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
#if USE_UATLAS
using ATLAS = UI.uAtlas;
#else
using ATLAS = xys.UI.Atlas;
#endif

namespace UI
{
    public class AtlasEditorWindow : EditorWindow
    {
        [MenuItem("uGUI/AtlasEditorWindow")]
        static void CreateWizard()
        {
            GetWindow<AtlasEditorWindow>(false, "uGUI Atlas", true).Show();
        }

        // 当前图集
        List<AtlasData> mAtlas = new List<AtlasData>();

        void FindAllAtlas()
        {
            HashSet<string> guids = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { "Assets/__copy__/Art/UIData/UData/Atlas" }));
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!assetPath.EndsWith(".prefab", true, null))
                    continue;

                ATLAS atlas = AssetDatabase.LoadAssetAtPath<ATLAS>(assetPath);
                if (atlas != null)
                {
                    AtlasData ad = new AtlasData(atlas);
                    mAtlas.Add(ad);
                }
            }
        }

        int texturesize = 0;
        int usedTextures = 0;

        void OnEnable()
        {
            FindAllAtlas();
            texturesize = 0;
            usedTextures = 0;
            for (int i = 0; i < mAtlas.Count; ++i)
            {
                AtlasData ad = mAtlas[i];
                texturesize += (int)(ad.atlasSize.x * ad.atlasSize.y);
                usedTextures += ad.totalArea;
            }

            mPageBtn = new EditorPageBtn();
            mParamList.ReleaseAll();
        }

        static string ToSize(int area)
        {
            return string.Format("{0}*{0}({1}*(2048*2048) {2}mb",
                (int)Mathf.Sqrt(area),
                (1f * area / (2048 * 2048)).ToString("0.00"),
                (area * 4 / (1024f * 1024)).ToString("0.00"));
        }

        EditorPageBtn mPageBtn;

        // 精灵的排序类型
        enum SpriteSortType
        {
            Null, // 不排序
            Name, // 按名字
            Size, // 按大小
        }

        static int GetArea(Sprite s)
        {
            if (s == null)
                return 0;

            Vector2 size = s.rect.size;
            return (int)(size.x * size.y);
        }

        static void SortSpriteData(List<Sprite> sds, SpriteSortType sst, bool isr)
        {
            switch (sst)
            {
            case SpriteSortType.Null:
                break;
            case SpriteSortType.Name:
                sds.Sort((Sprite x, Sprite y) => { return isr ? x.name.CompareTo(y.name) : y.name.CompareTo(x.name); });
                break;
            case SpriteSortType.Size:
                sds.Sort((Sprite x, Sprite y) => { return isr ? GetArea(x).CompareTo(GetArea(y)) : GetArea(y).CompareTo(GetArea(x)); });
                break;
            }
        }

        enum Filter
        {
            All,
            RGB,
            ARGB,
        }

        static void FilterSprite(List<Sprite> sds, Filter filter)
        {
            switch (filter)
            {
            case Filter.All:
                break;

            case Filter.RGB:
                {
                    sds.RemoveAll((Sprite s) =>
                    {
                        if (s == null)
                            return true;

                        if (PackTool.TextureExport.isARGB(s.texture.format))
                            return true;

                        return false;
                    });
                }
                break;
            case Filter.ARGB:
                {
                    sds.RemoveAll((Sprite s) =>
                    {
                        if (s == null)
                            return true;

                        if (!PackTool.TextureExport.isARGB(s.texture.format))
                            return true;

                        return false;
                    });
                }
                break;
            }
        }

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
                if (File.Exists(assetPath))
                {
                    File.Delete(assetPath);
                }

                uAtlas go = PrefabUtility.CreatePrefab(assetPath, atlas.gameObject).GetComponent<uAtlas>();
                go.Atlas = atlas.Atlas;

                string path = assetPath.Substring(0, assetPath.LastIndexOf('.'));
                for (int i = 0; i < go.Atlas.Length; ++i)
                {
                    string tp = string.Format("{0}{1}.png", path, i + 1);
                    if (File.Exists(tp))
                        File.Delete(tp);

                    File.WriteAllBytes(tp, go.Atlas[i].texture.EncodeToPNG());
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                    DestroyImmediate(go.Atlas[i].texture);

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
            }

            return atlas;
        }

        public static void CheckAlluAtlas()
        {
            //CheckAllPanel cap = CheckAllPanel.StartCheckAllPanel();
            //for (int i = 0; i )
        }
#else

#endif

        static void OnAtlasGUI(int index, AtlasData ad, ParamList paramList)
        {
            EditorGUILayout.BeginHorizontal();

            bool isShow = paramList.Get("isShow", false);
            isShow = EditorGUILayout.Foldout(isShow, string.Format("{0}){1}", index + 1, ad.ToString()));
            EditorGUILayout.ObjectField(ad.atlas, typeof(xys.UI.Atlas), false);
            EditorGUILayout.EndHorizontal();
            paramList.Set("isShow", isShow);
            if (isShow)
            {
                Vector2 pos = paramList.Get("ScrollPos", Vector2.zero);
                pos = EditorGUILayout.BeginScrollView(pos);
                paramList.Set("ScrollPos", pos);

                SpriteSortType sst = paramList.Get("sst", SpriteSortType.Size);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("排序", GUILayout.Width(30));
                sst = (SpriteSortType)EditorGUILayout.EnumPopup(sst);
                Filter filter = paramList.Get("filter", Filter.All);
                EditorGUILayout.LabelField("过滤", GUILayout.Width(30));
                filter = (Filter)EditorGUILayout.EnumPopup(filter);
                paramList.Set("filter", filter);
                bool isr = paramList.Get("isr", false);
                EditorGUILayout.LabelField("倒序", GUILayout.Width(30));
                isr = EditorGUILayout.Toggle(isr);

                List<Sprite> sprites = new List<Sprite>(ad.atlas.Sprites);
                FilterSprite(sprites, filter);

                EditorPageBtn epb = paramList.Get("epb", () => { return new EditorPageBtn(); });
                epb.total = sprites.Count;
                epb.pageNum = 30;
                epb.OnRender();

                if (GUILayout.Button("制作图集"))
                {
#if USE_UATLAS
                    string assetPath = AssetDatabase.GetAssetPath(ad.atlas);
                    string path = assetPath.Substring(0, assetPath.LastIndexOf('.'));

                    assetPath = path + "s.prefab";
                    CreateuAtlas(ad.atlas.Sprites, assetPath);
#else
                    ad.BuildTexture();
#endif
                }

                EditorGUILayout.EndHorizontal();

                paramList.Set("isr", isr);
                paramList.Set("sst", sst);
                List<Sprite> sds = new List<Sprite>(sprites);
                SortSpriteData(sds, sst, isr);

                epb.ForEach((int i)=> 
                {
                    Sprite s = sds[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField((i+1).ToString(), GUILayout.Width(20));
                    EditorGUILayout.LabelField("(", GUILayout.Width(7));
                    EditorGUILayout.LabelField((((int)s.rect.size.x).ToString()), GUILayout.Width(25));
                    EditorGUILayout.LabelField(",", GUILayout.Width(7));
                    EditorGUILayout.LabelField((((int)s.rect.size.y).ToString()), GUILayout.Width(25));
                    EditorGUILayout.LabelField(")", GUILayout.Width(8));
                    EditorGUILayout.ObjectField(s, typeof(Sprite), false);
                    if (GUILayout.Button("Select"))
                    {
                        Selection.activeObject = s;
                    }
                    EditorGUILayout.EndHorizontal();
                });

                EditorGUILayout.EndScrollView();
            }

        }

        ParamList mParamList = new ParamList();

        enum AtlasSortType
        {
            Null,
            Name,
            AtlasSize,
            SpriteSize,
            利用率,
        }

        static void SortAtlas(List<AtlasData> ads, AtlasSortType ast, bool isr)
        {
            switch (ast)
            {
            case AtlasSortType.Null:
                break;
            case AtlasSortType.Name:
                ads.Sort((AtlasData x, AtlasData y) => { return isr ? x.atlas.name.CompareTo(y.atlas.name) : y.atlas.name.CompareTo(x.atlas.name); });
                break;
            case AtlasSortType.AtlasSize:
                ads.Sort((AtlasData x, AtlasData y) => { return isr ? x.atlasArea.CompareTo(y.atlasArea) : y.atlasArea.CompareTo(x.atlasArea); });
                break;
            case AtlasSortType.SpriteSize:
                ads.Sort((AtlasData x, AtlasData y) => { return isr ? x.totalArea.CompareTo(y.totalArea) : y.totalArea.CompareTo(x.totalArea); });
                break;
            case AtlasSortType.利用率:
                ads.Sort((AtlasData x, AtlasData y) => { return isr ? x.usedPrec.CompareTo(y.usedPrec) : y.usedPrec.CompareTo(x.usedPrec); });
                break;
            }
        }

        void OnGUI()
        {
            GUILayout.Label(string.Format("图集总大小:{0} 个数:{1} 精灵总大小:{2} 图集利用率:{3}%", ToSize(texturesize), mAtlas.Count, ToSize(usedTextures), (100.0f * usedTextures / texturesize).ToString("0.00")));
            bool isAll = GUILayout.Button("制作所有图集");
            if (mPageBtn == null)
            {
                OnEnable();
            }

            mPageBtn.total = mAtlas.Count;
            mPageBtn.pageNum = 10;
            mPageBtn.OnRender();

            AtlasSortType ast = mParamList.Get("ast", AtlasSortType.AtlasSize);
            bool isr = mParamList.Get("isr", false);
            EditorGUILayout.BeginHorizontal();
            ast = (AtlasSortType)EditorGUILayout.EnumPopup("排序", ast);
            isr = EditorGUILayout.Toggle("反序", isr);
            EditorGUILayout.EndHorizontal();

            mParamList.Set("ast", ast);
            mParamList.Set("isr", isr);

            List<AtlasData> copys = new List<AtlasData>(mAtlas);
            SortAtlas(copys, ast, isr);
            mPageBtn.ForEach((int i) => 
            {
                AtlasData ad = copys[i];
                if (ad.atlas == null)
                    return;

                OnAtlasGUI(i, ad, mParamList.Get<ParamList>(ad.atlas.name));
            });

            if (isAll)
            {
                string assetPath = Application.dataPath + "/../png/";
                for (int i = 0; i < mAtlas.Count; ++i)
                {
                    mAtlas[i].BuildTexture(assetPath);
                }
            }
        }
    }
}