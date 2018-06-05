using UnityEngine;
using System;

// All controls rendered inside this element will be placed vertically below each other. The group must be closed with a call to EndVertical.
public class AutoGUILayoutVertical : IDisposable
{

    public AutoGUILayoutVertical(params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(options);
    }

    public AutoGUILayoutVertical(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(content, style, options);
    }

    public AutoGUILayoutVertical(string text, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginVertical(text, style, options);
    }

	public void Dispose()
    {
        GUILayout.EndVertical();
    }
}
