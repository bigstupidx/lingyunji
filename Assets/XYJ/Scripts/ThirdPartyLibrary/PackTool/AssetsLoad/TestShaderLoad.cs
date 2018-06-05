#if USE_RESOURCESEXPORT
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
#if COM_DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PackTool;

public class TestShaderLoad : MonoBehaviour
{
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        MagicThread.CreateInstance();
        GUITextShow.CreateInstance();
    }

    IEnumerator Start()
    {
        GUITextShow.AddText("开始检测shader兼容性!");
        GUITextShow.TextInfo current = GUITextShow.AddText("进度!");
        GUITextShow.TextInfo info = GUITextShow.AddText("进度!");
        ShaderCheckSupport scs = new ShaderCheckSupport();
        StartCoroutine(scs.BeginCheck());
        while (!scs.isDone)
        {
            current.text = scs.current;
            info.text = string.Format("进度:{1}/{2} {0}%", (100f * scs.progress).ToString("0.00"), scs.index + 1, scs.total);
            yield return 0;
        }

        info.text = string.Format("进度:{0}", scs.progress.ToString("0.00"));
        GUITextShow.AddText("完成!");
    }
}
#endif
#endif