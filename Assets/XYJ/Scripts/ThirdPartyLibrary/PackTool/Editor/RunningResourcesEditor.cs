#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using PackTool;

partial class RunningResourcesEditor : BaseEditorWindow
{
    [MenuItem("PackTool/运行时资源查看", false, 9)]
    [MenuItem("Assets/PackTool/运行时资源查看", false, 0)]
    static public void OpenRunningResourcesEditor()
    {
        GetWindow<RunningResourcesEditor>(false, "RunningResourcesEditor", true);
    }

    ParamList mParamList = new ParamList();

    protected override void OnDisable()
    {
        base.OnDisable();
        mParamList.ReleaseAll();
    }

    static T[] FindObjectsOfType<T>(bool findunactive) where T : Component
    {
        if (findunactive)
        {
            List<GameObject> objs = new List<GameObject>();
            foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
            {
                if (obj.transform.parent == null)
                    objs.Add(obj);
            }

            List<T> ts = new List<T>();
            foreach (GameObject o in objs)
                ts.AddRange(o.GetComponentsInChildren<T>(true));

            return ts.ToArray();
        }
        else
        {
            return Object.FindObjectsOfType<T>();
        }
    }

    string showType = "";

    AtlasShow mAtlasShow = new AtlasShow();

    void OnGUI()
    {
        string[] keys = new string[] { "着色器", "贴图", "材质", "图集", "字体", "动画控制器", "网格", "音频", "字库", "预置体" };//不导出中文
        showType = GUIEditor.GuiTools.StringPopup(false, "资源类型", showType, new List<string>(keys));//不导出中文
        if (GUILayout.Button("资源卸载"))
        {
            AssetsUnLoad.UnloadUnusedAssets(false);
        }

        switch (showType)
        {
        case "贴图"://不导出中文
            ShowTexture();
            break;
        case "预置体"://不导出中文
            ShowPrefabList();
            break;
        case "材质"://不导出中文
            ShowMaterialList();
            break;
        case "图集":
            mAtlasShow.ShowAtlas(mParamList.Get("图集", () => { return new ParamList(); }));
            break;
        case "网格"://不导出中文
            ShowMeshList();
            break;
        case "音频": // 音频
            ShowAudioClip();
            break;
        case "动画控制器":
            ShowRACShow();
            break;
        }
    }
}
#endif