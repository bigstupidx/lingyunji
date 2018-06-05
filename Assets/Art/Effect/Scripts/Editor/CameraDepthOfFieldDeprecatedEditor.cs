using System;
using UnityEditor;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [CustomEditor(typeof(CameraDepthOfFieldDeprecated))]
    public class CameraDepthOfFieldDeprecatedEditor : DepthOfFieldDeprecatedBaseEditor
    {
        public override void OnInspectorGUI()
        {
            Effect effect = ((CameraDepthOfFieldDeprecated)target).effect;
            effect.cameraStruct.cameraType = (CameraStruct.CameraType)(object)EditorGUILayout.EnumPopup("相机类型", effect.cameraStruct.cameraType);

            base.OnInspectorGUI();
        }
    }
}
