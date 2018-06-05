#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using xys.UI;
[CustomEditor(typeof(ScrollRectWrapper), true)]
public class ScrollRectWrapperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        ScrollRectWrapper scroll = (ScrollRectWrapper)target;

        //scroll.PrefabName = EditorGUILayout.TextField("Element预制名",scroll.PrefabName);
        //scroll.Horizontal = EditorGUILayout.Toggle("横向滑动", scroll.Horizontal);
        //scroll.Vertical = EditorGUILayout.Toggle("纵向滑动", scroll.Vertical);
        //scroll.MoveType = (ScrollRectWrapper.MovementType) EditorGUILayout.EnumPopup("移动模式", scroll.MoveType);/*("移动模式", scroll.Horizontal);*/


        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("清空"))
        {
            scroll.ClearCells();
        }
        if(GUILayout.Button("重置"))
        {
            scroll.RefillCells();
        }
        if(GUILayout.Button("刷新"))
        {
            scroll.RefreshCells();
        }
        EditorGUILayout.EndHorizontal();
    }
} 
#endif

