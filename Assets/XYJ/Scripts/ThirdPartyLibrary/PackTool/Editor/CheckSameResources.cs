using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    public partial class CheckSameResources : EditorWindow
    {
        [MenuItem("PackTool/资源工具/查找相同资源", false, 9)]
        static public void OpenPackEditorWindow()
        {
            EditorWindow.GetWindow<CheckSameResources>(false, "CheckSameResources", true);
        }

        // 生成所有文件的md5索引,md5值对应文件列表
        XTools.Multimap<string, string> GenFileMd5()
        {
            List<string> fileList = GlobalFileList.instance.GetFiles(Application.dataPath);
            XTools.Multimap<string, string> md5list = new XTools.Multimap<string, string>();
            foreach (string file in fileList)
            {
                if (file.StartsWith("Plugins/") || file.StartsWith("Data/Config/"))
                    continue;

                string md5 = Md5.GetFileMd5(Application.dataPath + "/" + file);
                if (md5list.Add(md5, file) != 1)
                    Debug.Log("资源重复了!" + file);
            }

            return md5list;
        }

        // 重复的资源
        XTools.SortedMap<string, List<List<Object>>> CheckSameList = new XTools.SortedMap<string, List<List<Object>>>();
        List<string> types = new List<string>();

        EditorPageBtn CheckSamePageBtn = new EditorPageBtn();

        Vector2 CheckSameScrollPosition = Vector2.zero;

        string selectType = ""; // 选中的类型

        string search_key;

        void OnGUI()
        {
            OnFindRepeatResources();
        }

        // 查找重复的资源文件
        void OnFindRepeatResources()
        {
            if (GUILayout.Button("检测重复文件"))
            {
                XTools.Multimap<string, string> md5list = GenFileMd5();
                foreach (KeyValuePair<string, List<string>> itor in md5list.dictionary)
                {
                    if (itor.Value.Count > 1)
                    {
                        string type = "";
                        List<Object> objs = new List<Object>();
                        foreach (string o in itor.Value)
                        {
                            string assetPath = "Assets/" + o;
                            Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
                            if (obj != null)
                            {
                                if (obj is Texture2D)
                                {
                                    Object[] sps = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                                    if (sps.Length != 0)
                                    {
                                        objs.AddRange(sps);
                                        type = "Sprite";
                                        continue;
                                    }
                                }

                                objs.Add(obj);
                                type = obj.GetType().Name;
                            }
                        }

                        CheckSameList[type].Add(objs);
                    }
                }
            }

            types.Clear();
            types.Add("All");
            foreach (KeyValuePair<string, List<List<Object>>> itor in CheckSameList)
                types.Add(itor.Key);

            List<List<Object>> showlist = new List<List<Object>>();
            selectType = GUIEditor.GuiTools.StringPopup(false, selectType, types);
            if (selectType == "All")
            {
                foreach (KeyValuePair<string, List<List<Object>>> itor in CheckSameList)
                    showlist.AddRange(itor.Value);
            }
            else
            {
                showlist.AddRange(CheckSameList[selectType]);
            }

            search_key = EditorGUILayout.TextField("搜索路径", search_key);
            if (!string.IsNullOrEmpty(search_key))
            {
                List<List<Object>> templist = new List<List<Object>>();
                string k = search_key.ToLower();
                for (int i = 0; i < showlist.Count; ++i)
                {
                    foreach (Object o in showlist[i])
                    {
                        string assetPath = AssetDatabase.GetAssetPath(o);
                        if (assetPath.ToLower().Contains(k))
                        {
                            templist.Add(showlist[i]);
                        }
                    }
                }

                showlist = templist;
            }

            CheckSamePageBtn.total = showlist.Count;
            CheckSamePageBtn.pageNum = 5;
            CheckSamePageBtn.OnRender();
            CheckSameScrollPosition = EditorGUILayout.BeginScrollView(CheckSameScrollPosition);
            for (int i = CheckSamePageBtn.beginIndex; i < CheckSamePageBtn.endIndex; ++i)
            {
                List<Object> os = showlist[i];
                os.RemoveAll((Object o) => { return o == null ? true : false; });
                EditorGUILayout.LabelField("一至的资源名:" + (os.Count == 0 ? "" : os[0].name));
                foreach (Object o in os)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(o, o.GetType(), true);
                    if (o != null)
                    {
                        if (GUILayout.Button("Select"))
                        {
                            Selection.activeObject = o;
                        }

                        string assetPath = AssetDatabase.GetAssetPath(o);
                        if (!string.IsNullOrEmpty(assetPath))
                            EditorGUILayout.LabelField(assetPath.Substring(7));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}