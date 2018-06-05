
using UnityEngine;
using System;

// Automatically laid out scrollviews will take whatever content you have inside them and display normally.
// If it doesn't fit, scrollbars will appear. A call to BeginScrollView must always be matched with a call to EndScrollView.
public class AutoGUIScrollView : IDisposable
{

    public AutoGUIScrollView(Rect position, Vector2 scrollPosition, Rect viewRect)
    {
        GUI.BeginScrollView(position, scrollPosition, viewRect);
    }

    public AutoGUIScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
    {
        GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical);
    }

    public AutoGUIScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
    {
        GUI.BeginScrollView(position, scrollPosition, viewRect, horizontalScrollbar, verticalScrollbar);
    }

    public AutoGUIScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
    {
        GUI.BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar);
    }

    public void Dispose()
    {
        GUI.EndScrollView();
    }

}
