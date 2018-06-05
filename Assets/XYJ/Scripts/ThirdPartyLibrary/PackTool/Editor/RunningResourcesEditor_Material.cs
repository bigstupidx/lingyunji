#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PackTool;
using GUIEditor;

partial class RunningResourcesEditor : BaseEditorWindow
{
    string materail_search_key;

    void ShowMaterialList()
    {
        materail_search_key = EditorGUILayout.TextField("搜索key", materail_search_key);

        // 查看下预置体的使用情况
        List<MaterialLoad> prefabs = new List<MaterialLoad>();
        {
            Dictionary<string, AssetLoadObject> objs = MaterialLoad.GetAllList();
            foreach (KeyValuePair<string, AssetLoadObject> itor in objs)
            {
                if (!string.IsNullOrEmpty(materail_search_key) && !itor.Key.Contains(materail_search_key))
                    continue;

                prefabs.Add(itor.Value as MaterialLoad);
            }
        }

        ParamList paramList = mParamList.Get<ParamList>("材质");//不导出中文

        GuiTools.ObjectFieldList<MaterialLoad>(
            paramList,
            prefabs,
            (MaterialLoad ml) => { return ml.asset; },
            false,
            null,
            (MaterialLoad ml) => 
            {
            },
            (MaterialLoad ml) => 
            {
                ParamList mmpl = paramList.Get<ParamList>("Material:" + ml.url);
                bool isshowtexture = mmpl.Get<bool>("show", false);
                isshowtexture  = EditorGUILayout.Toggle("显示", isshowtexture, GUILayout.Width(30f));
                EditorGUILayout.LabelField("计数:" + ml.Refcount, GUILayout.Width(60f));
                mmpl.Set("show", isshowtexture);
                if (isshowtexture)
                {
                    EditorGUILayout.EndHorizontal();
                    List<Texture> texList = XTools.Utility.GetMaterialTexture(ml.asset);
                    GuiTools.TextureListField(mmpl.Get<ParamList>("TextureList"), true, "", texList);
                    EditorGUILayout.BeginHorizontal();
                }
            });
    }
}
#endif