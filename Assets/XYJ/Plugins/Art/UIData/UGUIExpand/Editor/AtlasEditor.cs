//using UnityEngine;
//using UnityEditor;
//using UnityEngine.UI;
//using UnityEditor.UI;
//using System.Collections.Generic;

//namespace UI
//{
//    [CustomEditor(typeof(Atlas))]
//    public class AtlasEditor : Editor
//    {
//        AtlasData mAtlasData;

//        void OnEnable()
//        {
//            Atlas atlas = target as Atlas;

//            if (mAtlasData == null)
//                mAtlasData = new AtlasData(atlas);
//            else
//                mAtlasData.Reset(atlas);
//        }

//        public override void OnInspectorGUI()
//        {
//            if (GUILayout.Button("更新"))
//            {
//                OnEnable();
//            }

//            EditorGUILayout.LabelField(mAtlasData.ToString());
//            base.OnInspectorGUI();
//        }
//    }
//}
