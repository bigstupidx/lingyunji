using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GUIEditor;

namespace PackTool
{
    public partial class BuiltinResourceEditor : EditorWindow
    {
        class ShowShader
        {
            public ShowShader(BuiltinResourceEditor parent)
            {
                this.parent = parent;
                foreach (var mat in builtin.GetMaterials())
                {
                    List<Material> mats = null;
                    if (!shaderToMats.TryGetValue(mat.shader, out mats))
                    {
                        mats = new List<Material>();
                        shaderToMats.Add(mat.shader, mats);
                    }

                    mats.Add(mat);
                }
            }

            BuiltinResourceEditor parent;
            Dictionary<Shader, List<Material>> shaderToMats = new Dictionary<Shader, List<Material>>();
            public BuiltinResource builtin { get { return parent.builtin; } }

            int GetMatCount(Shader s)
            {
                List<Material> mats = GetMaterials(s);
                if (mats == null)
                    return 0;

                return mats.Count;
            }

            List<Material> GetMaterials(Shader s)
            {
                List<Material> mats = null;
                if (shaderToMats.TryGetValue(s, out mats))
                    return mats;
                return mats;
            }

            enum ShowType
            {
                All, // 所有
                In, // 内置Shader
                Resources,// Resources目录下的
                Custom, // 用户自定义
            }

            ShowType showType;

            // 当前显示的类型
            List<Shader> showShaders = new List<Shader>();

            //public ResourcesUsedInfo usedInfo { get { return parent.usedInfo; } }

            // 排序类型
            enum SortType
            {
                None,
                MatCount, // 材质数量
            }

            SortType sortType = SortType.None;

            string search_key = "";
            
            Shader[] GetShaders(string sk)
            {
                if (string.IsNullOrEmpty(sk))
                    return builtin.GetShaders();

                sk = sk.ToLower();
                List<Shader> ss = new List<Shader>();
                foreach (var s in builtin.GetShaders())
                {
                    if (s == null)
                        continue;
                    if (s.name.ToLower().Contains(sk))
                        ss.Add(s);
                }

                return ss.ToArray();
            }

            public void OnGUI(ParamList pl)
            {
                var v = (ShowType)GuiTools.EnumPopup(true, "着色器过滤", showType);
                var sv = (SortType)GuiTools.EnumPopup(true, "排序类型", sortType);
                var sk = EditorGUILayout.TextField("搜索", search_key);
                if (v != showType || showShaders.Count == 0 || sv != sortType || sk != search_key)
                {
                    search_key = sk;
                    showType = v;
                    sortType = sv;
                    showShaders.Clear();

                    var ss = GetShaders(search_key);
                    switch (showType)
                    {
                    case ShowType.All:
                        showShaders.AddRange(ss);
                        break;
                    case ShowType.Custom:
                        foreach (var s in ss)
                        {
                            var assetPath = AssetDatabase.GetAssetPath(s);
                            if(assetPath.StartsWith("Assets/") && !assetPath.Contains("/Resources/"))
                                showShaders.Add(s);
                        }
                        break;
                    case ShowType.In:
                        {
                            foreach (var s in ss)
                            {
                                var assetPath = AssetDatabase.GetAssetPath(s);
                                if (!assetPath.StartsWith("Assets/"))
                                    showShaders.Add(s);
                            }
                        }
                        break;
                    case ShowType.Resources:
                        {
                            foreach (var s in ss)
                            {
                                var assetPath = AssetDatabase.GetAssetPath(s);
                                if (assetPath.StartsWith("Assets/") && assetPath.Contains("/Resources/"))
                                    showShaders.Add(s);
                            }
                        }
                        break;
                    }

                    showShaders.RemoveAll((Shader s)=> { return s == null; });
                    switch (sortType)
                    {
                    case SortType.None:
                        break;
                    case SortType.MatCount:
                        showShaders.Sort((Shader x, Shader y) => { return GetMatCount(y).CompareTo(GetMatCount(x)); });
                        break;
                    }
                }

                GuiTools.ObjectFieldList(pl, showShaders, true, (Shader s)=> 
                {
                    List<Material> mats = GetMaterials(s);
                    GUILayout.Label("材质数量:" + (mats == null ? 0 : mats.Count), GUILayout.ExpandWidth(false));
                    string key = "showMat-" + s.GetInstanceID();
                    bool r = pl.Get<bool>(key, false);
                    r = GUILayout.Toggle(r, "", GUILayout.ExpandWidth(false));
                    pl.Set(key, r);
                    if (r)
                    {
                        key += " Show";
                        EditorGUILayout.EndHorizontal();
                        GuiTools.MaterialListField(pl.Get<ParamList>(key), true, "材质列表", mats, (Material m)=> { return m; });
                        EditorGUILayout.BeginHorizontal();
                    }
                },
                GUILayout.Width(400));
            }
        }

        void ShowShaderGUI(ParamList paramList)
        {
            var pl = paramList.Get<ParamList>("shader");
            showShader.OnGUI(pl);
        }
    }
}