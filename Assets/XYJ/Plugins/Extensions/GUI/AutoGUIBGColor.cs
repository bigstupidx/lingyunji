


using UnityEngine;
using System;

public class AutoGUIBGColor : IDisposable
{

    Color preColor;

    public AutoGUIBGColor(Color color)
    {
        preColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
    }

    public void Dispose()
    {
        GUI.backgroundColor = preColor;
    }
}
