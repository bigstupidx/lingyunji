
using UnityEngine;
using System;

public class AutoGUISkinStyle : IDisposable
{
    string mStyleName;
    GUIStyle preStyle;
	public AutoGUISkinStyle(string styleName, out GUIStyle outStyle)
    {
        mStyleName = styleName;
        outStyle = null;
        GUIStyle tmp = GUI.skin.GetStyle(styleName);
        if (tmp!=null)
        {
            outStyle = tmp;
            preStyle = new GUIStyle(tmp);
        }
        
    }

    public void Dispose()
    {
        if (preStyle!=null)
        {
            GUIStyle tmp = GUI.skin.GetStyle(mStyleName);
            tmp = preStyle;
        }
    }
}
