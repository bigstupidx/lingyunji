
using UnityEngine;
using System;

public class AutoGUIGroup : IDisposable
{

    public AutoGUIGroup(Rect position)
    {
        GUI.BeginGroup(position);
    }

    public AutoGUIGroup(Rect position, string text)
    {
        GUI.BeginGroup(position, text);
    }

    public AutoGUIGroup(Rect position, Texture image)
    {
        GUI.BeginGroup(position, image);
    }

    public AutoGUIGroup(Rect position, GUIContent content)
    {
        GUI.BeginGroup(position, content);
    }

    public AutoGUIGroup(Rect position, GUIStyle style)
    {
        GUI.BeginGroup(position, style);
    }

    public AutoGUIGroup(Rect position, string text, GUIStyle style)
    {
        GUI.BeginGroup(position, text, style);
    }

    public AutoGUIGroup(Rect position, Texture image, GUIStyle style)
    {
        GUI.BeginGroup(position, image, style);
    }

    public AutoGUIGroup (Rect position, GUIContent content, GUIStyle style)
    {
        GUI.BeginGroup(position, content, style);
    }

    public void Dispose()
    {
        GUI.EndGroup();
    }
}
