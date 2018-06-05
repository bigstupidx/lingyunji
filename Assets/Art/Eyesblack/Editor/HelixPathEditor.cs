using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Eyesblack.FX {
    [CustomEditor(typeof(HelixPath))]
    [CanEditMultipleObjects]
    public class HelixPathEditor : Editor {

        void OnEnable() {
            foreach (Object obj in targets) {
                if (((HelixPath)obj)._pathPoints == null) {
                    ((HelixPath)obj).CreatePathPoints();
                }
            }
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck()) {

                foreach (Object obj in targets) {
                    ((HelixPath)obj).CreatePathPoints();
                }
            }
        }
    }
}

