using UnityEngine;
using System;

//Begin a GUILayout block of GUI controls in a fixed screen area.
//By default, any GUI controls made using GUILayout are placed in the top-left corner of the screen.
//If you want to place a series of automatically laid out controls in an arbitrary area, use GUILayout.
//BeginArea to define a new area for the automatic layouting system to use.
public class AutoGUILayoutArea : IDisposable
{
    
	public AutoGUILayoutArea(Rect screenRect)
    {
        GUILayout.BeginArea(screenRect);
    }

    public AutoGUILayoutArea(Rect screenRect, string text, GUIStyle style=null)
    {
        GUILayout.BeginArea(screenRect, text, null);
    }

    public AutoGUILayoutArea(Rect screenRect, GUIContent content, GUIStyle style=null)
    {
        GUILayout.BeginArea(screenRect, content, style);
    }

    public void Dispose()
    {
        GUILayout.EndArea();
    }

}
