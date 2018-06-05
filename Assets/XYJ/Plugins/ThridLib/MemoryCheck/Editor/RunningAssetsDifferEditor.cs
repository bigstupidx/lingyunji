#if MEMORY_CHECK
using PackTool;
using GUIEditor;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

class RunningAssetsDifferEditor : BaseEditorWindow
{
    public static RunningAssetsDifferEditor Open()
    {
        return GetWindow<RunningAssetsDifferEditor>(false, "RunningAssetsDifferEditor", true) as RunningAssetsDifferEditor;
    }

    RunningAssetsDifferShow mRunningAssetsDifferShow = new RunningAssetsDifferShow();
    RunningAssetsDiffer mRunningAssetsDiffer = new RunningAssetsDiffer();

    //bool isfore = false;
    public void Show(AssetSyncInfo before, AssetSyncInfo current)
    {
        //isfore = true;
        mRunningAssetsDiffer.Differ(before, current);
        mRunningAssetsDifferShow.mRunningAssetsDiffer = mRunningAssetsDiffer;
    }

    void OnGUI()
    {
        mRunningAssetsDifferShow.OnGUI();
    }
}
#endif