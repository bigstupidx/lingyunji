using UnityEngine;
using System;

//All controls rendered inside this element will be placed horizontally next to each other. The group must be closed with a call to EndHorizontal.
public class AutoGUILayoutHorizontal : IDisposable
{

    public AutoGUILayoutHorizontal(params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(options);
    }

    public AutoGUILayoutHorizontal(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(content, style, options);
    }

    public AutoGUILayoutHorizontal(string text, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.BeginHorizontal(text, style, options);
    }

	public void Dispose ()
    {
        GUILayout.EndHorizontal();
    }
}
