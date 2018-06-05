#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;

using UnityEditor;

namespace JsonEditor
{
    public class Helper
    {
        static bool isHorizontal = false;

        public static Rect BeginHorizontal(params GUILayoutOption[] options)
        {
            if (isHorizontal)
                EditorGUILayout.EndHorizontal();

            isHorizontal = true;
            return EditorGUILayout.BeginHorizontal(options);
        }

        public static void EndHorizontal(params GUILayoutOption[] options)
        {
            if (isHorizontal)
                EditorGUILayout.EndHorizontal();

            isHorizontal = false;
        }
    }
}

#endif