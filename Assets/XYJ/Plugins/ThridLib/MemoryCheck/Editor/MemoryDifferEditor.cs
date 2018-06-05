#if MEMORY_CHECK
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using XTools;


class MemoryDifferEditor : BaseEditorWindow
{
    public static MemoryDifferEditor Open()
    {
        return GetWindow<MemoryDifferEditor>(false, "MemoryDifferEditor", true) as MemoryDifferEditor;
    }

    MemoryDifferShow mMemoryDifferShow = new MemoryDifferShow();

    bool isfore = false;
    public void Show(MemoryInfo before, MemoryInfo current)
    {
        isfore = true;
        mMemoryDifferShow.mMemoryDiffer = MemoryDiffer.Differ(before, current);
    }

    void OnGUI()
    {
        mMemoryDifferShow.OnGUI(isfore);
    }
}
#endif