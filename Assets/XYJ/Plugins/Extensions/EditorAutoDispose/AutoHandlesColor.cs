#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{

    public class AutoHandlesColor : IDisposable
    {
        Color preColor;
        public AutoHandlesColor(Color color)
        {
            preColor = Handles.color;
            Handles.color = color;
        }

        public void Dispose()
        {
            Handles.color = preColor;
        }
    }

}
#endif