using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if MEMORY_CHECK

partial class MemoryEditor : BaseEditorWindow
{
    [MenuItem("PackTool/内存/运行时查看", false, 9)]
    [MenuItem("Assets/PackTool/内存/运行时查看", false, 0)]
    static public void OpenRunningResourcesEditor()
    {
        GetWindow<MemoryEditor>(false, "MemoryEditor", true);
    }

    MemoryGUIShow mMemoryGUIShow = new MemoryGUIShow();

    protected override void OnDisable()
    {
        base.OnDisable();
        mMemoryGUIShow.Release();
    }

    void OnGUI()
    {
        if (GUILayout.Button("更新数据"))
        {
            MemoryObjectMgr.GetAll(mMemoryGUIShow.mMemoryInfo);
        }

        mMemoryGUIShow.OnGUI();
    }
}
#endif