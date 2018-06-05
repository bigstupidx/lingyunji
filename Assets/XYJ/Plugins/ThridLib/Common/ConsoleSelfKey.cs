using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

public partial class ConsoleSelf
{
#if !COM_DEBUG
    [Conditional("COM_DEBUG")]
    public static void AddKeyText(string key, string text, params object[] args)
    {

    }
#else
    static Dictionary<string, string> KeyToValue = new Dictionary<string,string>();
    public static void AddKeyText(string key, string text, params object[] args)
    {
        if (IsCreate == false)
            return;

        string value = string.Format(text, args);
        lock (KeyToValue)
        {
            KeyToValue[key] = value;
        }
    }

    public static void RemoveKeyText(string key)
    {
        if (IsCreate == false)
            return;

        lock (KeyToValue)
        {
            KeyToValue[key] = null;
        }
    }

    Dictionary<string, string> AllKeyToValue = new Dictionary<string, string>();

    public static void GetText(Dictionary<string, string> ts)
    {
        if (KeyToValue.Count == 0)
            return;

        lock (KeyToValue)
        {
            foreach (KeyValuePair<string, string> itor in KeyToValue)
            {
                if (itor.Value == null)
                    ts.Remove(itor.Key);
                else
                    ts[itor.Key] = itor.Value;
            }
            KeyToValue.Clear();
        }
    }

    bool IsKeyTextShow = false;
    void ShowKeyTextList()
    {
        float w = Screen.width / 1280.0f;
        if (GUI.Button(new Rect(930 * w, 40 * w, 150 * w, 60 * w), IsKeyTextShow ? "隐藏Key内容" : "显示Key内容"))
            IsKeyTextShow = !IsKeyTextShow;

        if (IsKeyTextShow)
        {
            if (AllKeyToValue.Count == 0)
                return;

            GUI.color = Color.yellow;
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
            foreach (KeyValuePair<string, string> itor in AllKeyToValue)
            {
                GUILayout.Label(string.Format("{0}:{1}", itor.Key, itor.Value), GUILayout.Width(Screen.width - 20));
            }

            GUILayout.EndScrollView();
        }
    }
#endif
}