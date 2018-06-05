using UnityEngine;
using System;
using System.Collections;

public class AutoGUIContentColor : IDisposable
{
    Color preColor;

    public AutoGUIContentColor(Color color)
    {
        preColor = GUI.contentColor;
        GUI.contentColor = color;
    }

    public void Dispose()
    {
        GUI.contentColor = preColor;
    }
}
