using UnityEngine;
using System;

public class AutoGUIColor : IDisposable
{

    Color preColor;

    public AutoGUIColor(Color color)
    {
        preColor = GUI.color;
        GUI.color = color;
    }

	public void Dispose ()
    {
        GUI.color = preColor;
    }

}
