#if USE_RESOURCESEXPORT
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using PackTool;
using GUIEditor;

partial class RunningResourcesEditor : BaseEditorWindow
{
    class AtlasShow
    {
        string search_key = "";

        Vector2 ScrollPosition = Vector2.zero;

        public void ShowAtlas(ParamList paramList)
        {
            search_key = EditorGUILayout.TextField("搜索key", search_key);

            // 查看下纹理的使用情况
            List<AtlasLoad> atlasList = new List<AtlasLoad>();
            {
                Dictionary<string, AssetLoadObject> objs = AllAsset.AlreadLoadList;
                foreach (KeyValuePair<string, AssetLoadObject> itor in objs)
                {
                    AtlasLoad al = itor.Value as AtlasLoad;
                    if (al == null || !al.isDone)
                        continue;

                    if (!string.IsNullOrEmpty(search_key) && !al.url.Contains(search_key))
                        continue;

                    atlasList.Add(itor.Value as AtlasLoad);
                }
            }

            EditorGUILayout.LabelField("图集");//不导出中文
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            GuiTools.ObjectFieldList<AtlasLoad>(
                paramList,
                atlasList,
                (AtlasLoad pl) => { return pl.asset; },
                false,
                null,
                (AtlasLoad pl) =>
                {

                },
                (AtlasLoad pl) =>
                {
                    ParamList ppp = paramList.Get<ParamList>(pl.url);
                    List<Texture> allTextures = new List<Texture>();
                    int size = pl.GetAllTextures(allTextures);
                    GUILayout.Label(string.Format("texture:{0} Sprite:{1} size:({2}*{2}) memory:{3}", allTextures.Count, pl.asset.Sprites.Length, size, XTools.Utility.ToMb(GuiTools.TextureMemorySizes(allTextures))));
                    bool isshow = ppp.Get("isTexture", false);
                    isshow = EditorGUILayout.Toggle("显示纹理", isshow);
                    ppp.Set("isTexture", isshow);
                    if (isshow)
                    {
                        EditorGUILayout.EndHorizontal();
                        GuiTools.ObjectFieldList(ppp.Get<ParamList>("show-texture", ()=> { return new ParamList(); }), allTextures, true);
                        GUILayout.BeginHorizontal();
                    }

                    isshow = ppp.Get("isSprites", false);
                    isshow = EditorGUILayout.Toggle("显示精灵", isshow);
                    ppp.Set("isSprites", isshow);
                    if (isshow)
                    {
                        EditorGUILayout.EndHorizontal();
                        string key = ppp.Get<string>("search_sprite", ()=> { return ""; });
                        key = EditorGUILayout.TextField("搜索精灵", key);
                        ppp.Set("search_sprite", key);
                        List<Sprite> sprites = null;
                        if (string.IsNullOrEmpty(key))
                        {
                            sprites = new List<Sprite>(pl.asset.Sprites);
                        }
                        else
                        {
                            sprites = new List<Sprite>();
                            foreach (Sprite s in pl.asset.Sprites)
                            {
                                if (s.name.Contains(key))
                                    sprites.Add(s);
                            }
                        }

                        GuiTools.ObjectFieldList(ppp.Get<ParamList>("show-sprites", () => { return new ParamList(); }), sprites, true);
                        GUILayout.BeginHorizontal();
                    }
                });
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif