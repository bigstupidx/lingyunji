using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

public partial class ConsoleSelf 
#if COM_DEBUG
    : SingletonMonoBehaviour<ConsoleSelf>
#endif
{
#if !COM_DEBUG
    [Conditional("COM_DEBUG")]
    public static void AddText(string text, params object[] args)
    {

    }

    [Conditional("COM_DEBUG")]
    public void ConsoleSelfGUI()
    {

    }

    [Conditional("COM_DEBUG")]
    public static void CreateInstance()
    {

    }

    [Conditional("COM_DEBUG")]
    public static void SetEnabled(bool enable)
    {

    }

    public static bool GetEnabled()
    {
        return false;
    }
#else
    public static bool GetEnabled()
    {
        if (sInstance == null)
            return false;
        return sInstance.enabled;
    }

    public static void SetEnabled(bool enable)
    {
        if (sInstance == null)
            return;
        sInstance.enabled = enable;
    }

    void addText(string text, params object[] args)
    {
        AddText(string.Format(text, args));
    }

    static List<string> CacheList = new List<string>();
    public static void AddText(string text, params object[] args)
    {
        if (IsCreate == false)
            return;

        lock (CacheList)
        {
            if (args == null || args.Length == 0)
                CacheList.Add(text);
            else
                CacheList.Add(string.Format(text, args));
        }
    }

    public static void GetText(ref List<string> ts)
    {
        if (CacheList.Count == 0)
            return;

        lock (CacheList)
        {
            ts.AddRange(CacheList);
            CacheList.Clear();
        }
    }

    void OnEnable()
    {
        StartCoroutine(UpdateList());
    }

    LinkedList<string> AllTextList = new LinkedList<string>();
    List<string> TempText = new List<string>();
    void ReadList()
    {
        TempText.Clear();
        GetText(ref TempText);
        for (int i = 0; i < TempText.Count; ++i)
            AllTextList.AddLast(TempText[i]);

        while (AllTextList.Count > 60)
            AllTextList.RemoveFirst();
    }

    Vector2 ScrollPosition;
    bool IsShow = false;

    void ShowTextList()
    {
        float w = Screen.width / 1280.0f;
        if (GUI.Button(new Rect(800 * w, 40 * w, 120 * w, 60 * w), IsShow ? "隐藏内容" : "显示内容"))
            IsShow = !IsShow;

        if (GUI.Button(new Rect(800 * w, 110 * w, 120 * w, 60 * w), "清除全部"))
        {
            AllTextList.Clear();
            AllKeyToValue.Clear();
        }

        if (IsShow)
        {
            GUI.color = Color.yellow;
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
            foreach (string t in AllTextList)
            {
                GUILayout.Label(t, GUILayout.Width(Screen.width - 20));
            }

            GUILayout.EndScrollView();
        }
    }

    void OnGUI()
    {
        ShowTextList();
        ShowKeyTextList();
    }

    IEnumerator UpdateList()
    {
        yield return 0;

        while (true)
        {
            ReadList();
            GetText(AllKeyToValue);
            yield return 0;
        }
    }
#endif
}