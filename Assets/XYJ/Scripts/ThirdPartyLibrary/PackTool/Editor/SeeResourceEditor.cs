using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GUIEditor;
#pragma warning disable 414

namespace PackTool
{
    public partial class SeeResourceEditor : EditorWindow
    {
        [MenuItem("PackTool/资源工具/Resources目录资源查看", false, 9)]
        static public void OpenPackEditorWindow()
        {
            EditorWindow.GetWindow<SeeResourceEditor>(false, "SeeResourceEditor", true);
        }

        List<Object> Resources = new List<Object>();

        ResourcesUsedInfo usedInfo = null;

        private void OnEnable()
        {
            Check();
        }

        void Check()
        {
            paramList.ReleaseAll();
            Resources.Clear();
            XTools.Utility.ForEach("Assets", (AssetImporter ai) => { }, (string assetPath, string root) =>
            {
                if (assetPath.Contains("/Resources/"))
                {
                    Resources.Add(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
                }

                return false;
            });
        }

        ParamList paramList = new ParamList();
        void OnGUI()
        {
            GuiTools.HorizontalField(false, ()=> 
            {
                if (GUILayout.Button("重新检索", GUILayout.Width(200f), GUILayout.Height(50f)))
                {
                    Check();
                }

                if (GUILayout.Button("查看引用", GUILayout.Width(200f), GUILayout.Height(50f)))
                {
                    if (usedInfo == null)
                        usedInfo = new ResourcesUsedInfo();
                    usedInfo.Init();
                }
            });

            GuiTools.ObjectFieldList(paramList, Resources, false, (Object o)=> 
            {
                if (usedInfo == null)
                    return;

                List<string> infos = usedInfo.GetUsedInfo(AssetDatabase.GetAssetPath(o));
                string key = "showdep-" + o.GetInstanceID();
                bool v = paramList.Get<bool>(key, false);
                v = EditorGUILayout.Toggle(string.Format("显示使用情况(数量:{0})", infos.Count), v, GUILayout.ExpandWidth(false));
                paramList.Set(key, v);
                if (v == true)
                {
                    var p = paramList.Get<ParamList>(key + "show");
                    System.Func<string, Object> fun = (string path) => { return AssetDatabase.LoadAssetAtPath<Object>(path); };
                    EditorGUILayout.EndHorizontal();
                    GuiTools.ObjectFieldList<string>(p, infos, fun, true, null, null, null);
                    EditorGUILayout.BeginHorizontal();
                }
            }, 
            GUILayout.Width(300f));
        }
    }
}