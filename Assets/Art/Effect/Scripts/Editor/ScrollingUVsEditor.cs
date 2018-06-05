//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(ScrollingUVs), true)]
//public class ScrollingUVsEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        ScrollingUVs s = target as ScrollingUVs;
//        Material src = s.srcMaterial;
//        Material dst = s.currentMaterial;
//        EditorGUILayout.ObjectField(string.Format("src id:{0}", src == null ? "" : src.GetInstanceID().ToString()), src, typeof(Material), true);
//        EditorGUILayout.ObjectField(string.Format("dst id:{0}", dst == null ? "" : dst.GetInstanceID().ToString()), dst, typeof(Material), true);
//    }
//}