using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(UIFxRQGraphic), true)]
public class UIFxRQGraphicEditor : Editor
{
	public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();

        List<Renderer> renderers = ((UIFxRQGraphic)target).Renderers;

        for (int i = 0; i < renderers.Count; ++i)
        {
            EditorGUILayout.ObjectField(string.Format("sortingOrder:{0}", renderers[i].sortingOrder), renderers[i], typeof(Renderer), true);
        }
    }
}