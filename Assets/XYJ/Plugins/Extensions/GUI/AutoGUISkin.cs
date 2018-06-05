
using UnityEngine;
using System;

public class AutoGUISkin : IDisposable {

    GUISkin preSkin;
    public AutoGUISkin (GUISkin skin)
    {
        preSkin = GUI.skin;
        GUI.skin = skin;
    }

    public void Dispose()
    {
        GUI.skin = preSkin;
    }
}
