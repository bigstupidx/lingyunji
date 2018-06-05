using System.Collections.Generic;

public class GUITextShowList
{
    List<GUITextShow.TextInfo> textList = new List<GUITextShow.TextInfo>();

    public GUITextShow.TextInfo AddText(string text)
    {
        GUITextShow.TextInfo info = GUITextShow.AddText(text);
        textList.Add(info);

        return info;
    }

    public GUITextShow.TextInfo AddButton(string text, GUITextShow.OnButtonClick click, object p = null, int num = -1)
    {
        GUITextShow.TextInfo info = GUITextShow.AddButton(text, click, p, num);
        textList.Add(info);

        return info;
    }

    public void Release()
    {
        GUITextShow.CannelList(textList);
    }
}
