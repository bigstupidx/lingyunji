
using UnityEngine;
using System;

public class AutoGUILayoutScrollView : IDisposable
{

	public AutoGUILayoutScrollView(ref Vector2 scrollPosition, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, options);
    }

    public AutoGUILayoutScrollView(ref Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, style, options);
    }

    public AutoGUILayoutScrollView(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options);
    }

    public AutoGUILayoutScrollView(ref Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, horizontalScrollbar, verticalScrollbar, options);
    }

    public AutoGUILayoutScrollView(ref Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options);
    }

    public void Dispose()
    {
        GUILayout.EndScrollView();
    }
}
