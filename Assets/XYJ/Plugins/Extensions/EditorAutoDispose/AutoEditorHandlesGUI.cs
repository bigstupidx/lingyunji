
#if UNITY_EDITOR

using UnityEditor;
using System;

namespace EditorExtensions
{
    /// <summary>
    /// Begin a 2D GUI block on top of the current handle camera.
    /// 可以在场景摄像机视窗上绘制EditorGUI
    /// </summary>
    public class AutoEditorHandlesGUI : IDisposable
    {

        public AutoEditorHandlesGUI()
        {
            Handles.BeginGUI();
        }

        public void Dispose()
        {
            Handles.EndGUI();
        }
    }

}
#endif