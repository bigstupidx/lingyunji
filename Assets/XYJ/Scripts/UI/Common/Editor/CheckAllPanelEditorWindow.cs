#if USE_RESOURCESEXPORT
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace xys.UI
{
    // 检测所有的面板，并且规划好精灵的分类
    public class CheckAllPanelEditorWindow : EditorWindow
    {
        [MenuItem("uGUI/Panel Check")]
        static void CreateWizard()
        {
            GetWindow<CheckAllPanelEditorWindow>(false, "UGUI Panel Check", true).Show();
        }

        CheckAllPanel m_CheckAllPanel;

        void OnEnable()
        {
            if (m_CheckAllPanel != null)
                m_CheckAllPanel = CheckAllPanel.StartCheckAllPanel();
        }

        int m_PanelType = -1;
        bool isShowRepateName = false; // 显示重名的

        void ShowPanelType()
        {
            List<string> keys = new List<string>();
            List<int> values = new List<int>();
            keys.Add("所有");
            values.Add(0);

            m_CheckAllPanel.ForEach((string type, CheckAllPanel.Panel panel) =>
            {
                values.Add(values.Count);
                keys.Add(type);
            });

            if (m_PanelType < 0)
                m_PanelType = 0;

            int newpt = EditorGUILayout.IntPopup("PanelType", m_PanelType, keys.ToArray(), values.ToArray());
            if (newpt != m_PanelType || AllShowSprites.Count == 0)
            {
                m_PanelType = newpt;
                if (m_PanelType != keys.Count)
                {
                    AllShowSprites.Clear();
                    if (m_PanelType <= 0 || m_PanelType >= keys.Count)
                    {
                        AllShowSprites.AddRange(m_CheckAllPanel.spritesList);
                    }
                    else
                    {
                        if (m_CheckAllPanel.Get(keys[newpt]) != null)
                            AllShowSprites.AddRange(m_CheckAllPanel.Get(keys[newpt]).sprites);
                    }

                    spritesbyName.Clear();
                }
            }

            AllShowSprites.RemoveAll((CheckAllPanel.Sprites s) => { return s.sprite == null ? true : false; });

            ShowSprites.Clear();
            ShowSprites.AddRange(AllShowSprites);
            bool isShowused = m_ParamList.Get("isShowused", false);
            isShowused = EditorGUILayout.Toggle("只显示有用的", isShowused);
            m_ParamList.Set("isShowused", isShowused);
            if (isShowused)
                ShowSprites.RemoveAll((CheckAllPanel.Sprites s) => { return s.spriteType == SpriteType.Null ? true : false; });

            bool ischange = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("格式过滤", GUILayout.Width(148));
            ShowSprites = GUIEditor.GuiTools.GetSelectTextureByTextureImporterFormat(m_ParamList.Get<ParamList>("TextureFormat"), false, ShowSprites, (CheckAllPanel.Sprites s)=> { return s.sprite.texture; }, out ischange);
            EditorGUILayout.EndHorizontal();
            isShowRepateName = EditorGUILayout.Toggle("显示重名", isShowRepateName);
            if (isShowRepateName)
            {
                if (spritesbyName.Count == 0)
                {
                    for (int i = 0; i < ShowSprites.Count; ++i)
                    {
                        List<CheckAllPanel.Sprites> caps = null;
                        if (!spritesbyName.TryGetValue(ShowSprites[i].sprite.name, out caps))
                        {
                            caps = new List<CheckAllPanel.Sprites>();
                            spritesbyName.Add(ShowSprites[i].sprite.name, caps);
                        }

                        caps.Add(ShowSprites[i]);
                    }
                }
            }
        }

        EditorPageBtn m_EditorPageBtn = new EditorPageBtn();
        Vector2 StartPosition = Vector2.zero;
        ParamList m_ParamList = new ParamList();

        List<CheckAllPanel.Sprites> AllShowSprites = new List<CheckAllPanel.Sprites>();

        // 当前显示的列表
        List<CheckAllPanel.Sprites> ShowSprites = new List<CheckAllPanel.Sprites>();

        // 重名列表
        Dictionary<string, List<CheckAllPanel.Sprites>> spritesbyName = new Dictionary<string, List<CheckAllPanel.Sprites>>();

        enum SpriteSortType
        {
            Null, // 不排序
            Name, // 名字
            Size, // 大小
        }

        SpriteSortType spriteSortType = SpriteSortType.Null;
        string search_sprite_key;
        bool isReverse = false;

        static CheckAllPanel.Sprites select_relpace = null;

        void OnGUI()
        {
            if (m_CheckAllPanel == null)
                m_CheckAllPanel = CheckAllPanel.StartCheckAllPanel();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("PanelTotal:{0}", m_CheckAllPanel.total));
            if (GUILayout.Button("刷新"))
            {
                m_CheckAllPanel = CheckAllPanel.StartCheckAllPanel();
            }
            if (GUILayout.Button("创建图集"))
            {
                m_CheckAllPanel.CreateAtlas();
            }
            EditorGUILayout.EndHorizontal();

            ShowPanelType();

            if (isShowRepateName)
            {
                List<List<CheckAllPanel.Sprites>> namesprites = new List<List<CheckAllPanel.Sprites>>();
                foreach (var itor in spritesbyName)
                {
                    if (itor.Value.Count >= 2)
                    {
                        namesprites.Add(itor.Value);
                    }
                }

                m_EditorPageBtn.total = namesprites.Count;
                m_EditorPageBtn.pageNum = 1;
                m_EditorPageBtn.OnRender();

                StartPosition = EditorGUILayout.BeginScrollView(StartPosition);
                List<CheckAllPanel.Sprites> deletes = null;
                int pos = -1;
                for (int i = m_EditorPageBtn.beginIndex; i < m_EditorPageBtn.endIndex; ++i)
                {
                    foreach (CheckAllPanel.Sprites s in namesprites[i])
                    {
                        ++pos;

                        if (s.sprite == null)
                            continue;

                        // 要删除掉
                        if (OnGUISprites(s, m_ParamList, true, false))
                        {
                            deletes = namesprites[i];
                            break;
                        }
                    }
                }

                EditorGUILayout.EndScrollView();

                if (deletes != null)
                {
                    CheckAllPanel.Sprites rep = null;
                    for (int i = 0; i < deletes.Count; ++i)
                    {
                        if (i != pos)
                        {
                            rep = deletes[i];
                            break;
                        }
                    }

                    if (rep != null)
                    {
                        deletes[pos].SetSpriteNew(rep.sprite);

                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(deletes[pos].sprite));
                    }

                    deletes = null;
                }
            }
            else
            {
                if (select_relpace != null)
                {
                    Color color = GUI.color;
                    GUI.color = Color.yellow;
                    EditorGUILayout.LabelField("这个要被删掉的");
                    OnGUISprites(select_relpace, m_ParamList, false, false);
                    GUI.color = color;
                }

                search_sprite_key = EditorGUILayout.TextField("搜索", search_sprite_key);
                spriteSortType = (SpriteSortType)EditorGUILayout.EnumPopup("排序类型", spriteSortType);
                isReverse = EditorGUILayout.Toggle("倒序", isReverse);
                switch (spriteSortType)
                {
                case SpriteSortType.Null:
                    break;
                case SpriteSortType.Name:
                    {
                        ShowSprites.Sort((CheckAllPanel.Sprites x, CheckAllPanel.Sprites y) => { return x.sprite.name.CompareTo(y.sprite.name); });
                    }
                    break;

                case SpriteSortType.Size:
                    {
                        ShowSprites.Sort((CheckAllPanel.Sprites x, CheckAllPanel.Sprites y) => 
                        {
                            Vector2 xs = x.sprite.rect.size;
                            Vector2 ys = y.sprite.rect.size;

                            return (xs.x * xs.y).CompareTo(ys.x * ys.y);
                        });
                    }
                    break;
                }

                if (isReverse)
                {
                    ShowSprites.Reverse();
                }

                if (!string.IsNullOrEmpty(search_sprite_key))
                {
                    string t = search_sprite_key.ToLower();
                    List<CheckAllPanel.Sprites> temp = new List<CheckAllPanel.Sprites>();
                    for (int i = 0; i < ShowSprites.Count; ++i)
                    {
                        if (ShowSprites[i].sprite.name.ToLower().Contains(t))
                        {
                            temp.Add(ShowSprites[i]);
                        }
                    }

                    ShowSprites.Clear();
                    ShowSprites.AddRange(temp);
                }

                m_EditorPageBtn.total = ShowSprites.Count;
                m_EditorPageBtn.pageNum = 25;
                m_EditorPageBtn.OnRender();

                StartPosition = EditorGUILayout.BeginScrollView(StartPosition);
                for (int i = m_EditorPageBtn.beginIndex; i < m_EditorPageBtn.endIndex; ++i)
                {
                    OnGUISprites(ShowSprites[i], m_ParamList, false, true);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        static bool OnGUISprites(CheckAllPanel.Sprites sprites, ParamList paramList, bool isshowdelete, bool isRelpace)
        {
            ParamList param = paramList.Get<ParamList>(sprites.GetHashCode().ToString());
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(sprites.sprite, typeof(Object), false, GUILayout.ExpandWidth(false));
            if (sprites.sprite != null)
                EditorGUILayout.TextField(sprites.ToString());

            //GUILayout.Label("详情");
            bool value = EditorGUILayout.Toggle(param.Get<bool>("info"), GUILayout.Width(60f));
            param.Set("info", value);

            if (GUILayout.Button("Select"))
            {
                Selection.activeObject = sprites.sprite.texture;
            }
             
            bool isdelete = false;
            if (isshowdelete)
            {
                isdelete = GUILayout.Button("删除");
            }

            if (isRelpace)
            {
                bool replacevalue = select_relpace == sprites;
                bool replacevaluenew = GUILayout.Toggle(replacevalue, "替换");
                if (replacevaluenew)
                {
                    select_relpace = sprites;
                }
                else if (replacevalue && !replacevaluenew)
                {
                    select_relpace = null;
                }
            }

            if (select_relpace != null && select_relpace != sprites)
            {
                if (GUILayout.Button("替换"))
                {
                    sprites.SetSpriteNew(select_relpace.sprite);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (value)
            {
                EditorGUI.indentLevel++;
                foreach (var itor in sprites.behaviours)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(string.Format("Panel:{0} behaviour Total:{1}", itor.Key, itor.Value.Count));
                    GameObject root = null;
                    if (itor.Value.Count > 0 && itor.Value[0] != null)
                        root = itor.Value[0].transform.root.gameObject;
                    EditorGUILayout.ObjectField(root, typeof(GameObject), true);
                    if (GUILayout.Button("Select"))
                    {
                        Selection.activeObject = root;
                    }
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < itor.Value.Count; ++i)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(itor.Value[i], typeof(Object), true);
                        if (GUILayout.Button("Select"))
                        {
                            Selection.activeObject = itor.Value[i];
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }

            return isdelete;
        }
    }
}
#endif