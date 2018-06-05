using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PackTool;

public class GUITextShow : SingletonMonoBehaviour<GUITextShow>
{
    public void Release()
    {
        textList.Clear();
    }

    public delegate void OnButtonClick(object p);

    public static void CannelList(List<TextInfo> list)
    {
        foreach (TextInfo info in list)
            info.isCannel = true;

        list.Clear();
    }

    public class TextInfo
    {
        public string text;
        public bool isbutton = false;
        public OnButtonClick click = null;
        public object clickp = null;
        public bool isCannel = false; // 是否取消显示
		public int num = -1; // 
    }

    public static TextInfo AddText(string text)
    {
        TextInfo info = new TextInfo();
        info.text = text;
        MagicThread.StartForeground(AddTextMainThread(info));
        return info;
    }

    public static TextInfo AddButton(string text, OnButtonClick click, object p = null, int num = -1)
    {
        TextInfo info = new TextInfo();
        info.isbutton = true;
        info.click = click;
        info.clickp = p;
        info.text = text;
		info.num = num;
        MagicThread.StartForeground(AddTextMainThread(info));
        return info;
    }

    static IEnumerator AddTextMainThread(TextInfo info)
    {
        yield return new ForegroundTask();
        if (Instance != null)
        {
            Instance.Add(info);
        }
    }

    List<TextInfo> textList = new List<TextInfo>();

    public TextInfo Add(string text)
    {
        TextInfo info = new TextInfo();
        info.text = text;
        Add(info);
        return info;
    }

    public TextInfo Add(string text, OnButtonClick click, object p = null)
    {
        TextInfo info = new TextInfo();
        info.isbutton = true;
        info.click = click;
        info.clickp = p;
        info.text = text;
        Add(info);
        return info;
    }

    void Add(TextInfo info)
    {
        textList.Add(info);
        if (enabled == false)
            enabled = true;
    }

    Vector2 ScrollPosition;

    void OnGUI()
    {
        ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
        for (int i = 0; i < textList.Count; )
        {
            if (textList[i].isCannel)
            {
                textList.RemoveAt(i);
                continue;
            }
            else if (!string.IsNullOrEmpty(textList[i].text))
            {
                if (textList[i].isbutton)
                {
                    if (GUILayout.Button(textList[i].text, /*style, */GUILayout.Width(Screen.width - 20), GUILayout.Height(60)))
                    {
                        if (textList[i].click != null)
                        {
                            textList[i].click(textList[i].clickp);
                        }

						if (textList[i].num == 1)
						{
							textList[i].isCannel = true;
						}
						else if (textList[i].num != -1)
						{
							textList[i].num -= 1;
						}
                    }
                }
                else
                {
                    GUILayout.Label(textList[i].text, /*style, */GUILayout.Width(Screen.width - 20));
                }
            }

            ++i;
        }

        GUILayout.EndScrollView();

        if (textList.Count == 0)
        {
            enabled = false;
        }
    }
}
