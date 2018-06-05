using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
#if USE_RESOURCESEXPORT

namespace PackTool
{
    [CustomEditor(typeof(PrefabCache), true)]
    public class PrefabCacheEditor : Editor
    {
        List<PrefabLoad> PrefabLoads = new List<PrefabLoad>();

        RunningResourcesEditor.PrefabLoadShow PrefabShow = new RunningResourcesEditor.PrefabLoadShow();

        ParamList mParamList = new ParamList();

        public override void OnInspectorGUI()
        {
            PrefabLoads.Clear();
            PrefabCacheLoad.GetAll(PrefabLoads);
            Dictionary<string, AssetLoadObject> objs = new Dictionary<string, AssetLoadObject>();
            foreach (PrefabLoad pl in PrefabLoads)
                objs.Add(pl.url, pl);

            if (GUILayout.Button("清除所有缓存"))
            {
                PrefabCacheLoad.ClearAllCache();
            }

            PrefabShow.ShowPrefabList(mParamList, objs);
        }
    }
}
#endif