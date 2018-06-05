//#define ENABLE_SUNSHAFT_GUI
//#define ENABLE_BLOOM_GUI
//#define ENABLE_DOF_GUI
//#define ENABLE_RADIALBLUR_GUI
//#define ENABLE_CC_GUI
//#define ENABLE_VIGN_GUI
//#define ENABLE_SAT_GUI


using UnityEngine;
using UnityEditor;
using System.Collections;
#pragma warning disable 414
namespace Eyesblack.ImageEffects {
    [CustomEditor(typeof(PostEffect))]
    public class YSPostEffectEditor : Editor {
        SerializedObject _serializedObj;

#if ENABLE_DOF_GUI
        SerializedProperty _DOFEnabled;
        SerializedProperty _DOFForegroundBlur;    
        SerializedProperty _DOFFocalDistance;
        SerializedProperty _DOFSmoothness;
        SerializedProperty _DOFBlurWidth;
        SerializedProperty _DOFFocalObject;
#endif

        SerializedProperty _sunShaftsEnabled;
        SerializedProperty _sunShaftsScreenBlendMode;
        SerializedProperty _sunTransform;
        SerializedProperty _sunColor;
        SerializedProperty _sunShaftsBlurRadius;
        SerializedProperty _sunShaftsIntensity;
        SerializedProperty _sunShaftsMaxRadius;
        private static bool _showCrosshair = false;

        SerializedProperty _bloomEnabled;
        SerializedProperty _bloomIntensity;
        SerializedProperty _bloomThreshhold;
        SerializedProperty _bloomBlurWidth;

#if ENABLE_RADIALBLUR_GUI
        SerializedProperty _radialBlurEnabled;
        SerializedProperty _radialBlurWidth;
        SerializedProperty _radialBlurRange;
#endif

        SerializedProperty _colorCorrectionEnabled;
        SerializedProperty _colorCorrectionMode;
        SerializedProperty _CCRedChannel;
        SerializedProperty _CCGreenChannel;
        SerializedProperty _CCBlueChannel;
        SerializedProperty _CCLutTexture;

        SerializedProperty _vignettingEnabled;
        SerializedProperty _vignettingIntensity;

        SerializedProperty _saturationEnable;
        SerializedProperty _saturation;


        void OnEnable() {
            _serializedObj = new SerializedObject(target);

#if ENABLE_SUNSHAFT_GUI
            _sunShaftsEnabled = _serializedObj.FindProperty("_sunShaftsEnabled");
            _sunShaftsScreenBlendMode = _serializedObj.FindProperty("_sunShaftsScreenBlendMode");
            _sunTransform = _serializedObj.FindProperty("_sunTransform");
            _sunColor = _serializedObj.FindProperty("_sunColor");
            _sunShaftsBlurRadius = _serializedObj.FindProperty("_sunShaftsBlurRadius");
            _sunShaftsIntensity = _serializedObj.FindProperty("_sunShaftsIntensity");
            _sunShaftsMaxRadius = _serializedObj.FindProperty("_sunShaftsMaxRadius");
#endif

#if ENABLE_DOF_GUI
            _DOFEnabled = _serializedObj.FindProperty("_DOFEnabled");
            _DOFForegroundBlur = _serializedObj.FindProperty("_DOFForegroundBlur");
            _DOFFocalDistance = _serializedObj.FindProperty("_DOFFocalDistance");
            _DOFSmoothness = _serializedObj.FindProperty("_DOFSmoothness");
            _DOFBlurWidth = _serializedObj.FindProperty("_DOFBlurWidth");
            _DOFFocalObject = _serializedObj.FindProperty("_DOFFocalObject");
#endif

#if ENABLE_BLOOM_GUI
            _bloomEnabled = _serializedObj.FindProperty("_bloomEnabled");
            _bloomIntensity = _serializedObj.FindProperty("_bloomIntensity");
            _bloomThreshhold = _serializedObj.FindProperty("_bloomThreshhold");
            _bloomBlurWidth = _serializedObj.FindProperty("_bloomBlurWidth");
#endif

#if ENABLE_RADIALBLUR_GUI
            _radialBlurEnabled = _serializedObj.FindProperty("_radialBlurEnabled");
            _radialBlurWidth = _serializedObj.FindProperty("_radialBlurWidth");
            _radialBlurRange = _serializedObj.FindProperty("_radialBlurRange");
#endif

#if ENABLE_CC_GUI
            _colorCorrectionEnabled = _serializedObj.FindProperty("_colorCorrectionEnabled");
            _colorCorrectionMode = _serializedObj.FindProperty("_colorCorrectionMode");
            _CCRedChannel = _serializedObj.FindProperty("_CCRedChannel");
            _CCGreenChannel = _serializedObj.FindProperty("_CCGreenChannel");
            _CCBlueChannel = _serializedObj.FindProperty("_CCBlueChannel");
            _CCLutTexture = _serializedObj.FindProperty("_CCLutTexture");
            if (_colorCorrectionMode.enumValueIndex == (int)PostEffect.ColorCorrectionMode.Simple) {
                if (_CCRedChannel.animationCurveValue.length == 0)
                    _CCRedChannel.animationCurveValue = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
                if (_CCGreenChannel.animationCurveValue.length == 0)
                    _CCGreenChannel.animationCurveValue = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
                if (_CCBlueChannel.animationCurveValue.length == 0)
                    _CCBlueChannel.animationCurveValue = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

                _serializedObj.ApplyModifiedProperties();
            }
#endif


#if ENABLE_VIGN_GUI
            _vignettingEnabled = _serializedObj.FindProperty("_vignettingEnabled");
            _vignettingIntensity = _serializedObj.FindProperty("_vignettingIntensity");
#endif

#if ENABLE_SAT_GUI
            _saturationEnable = _serializedObj.FindProperty("_saturationEnable");
            _saturation = _serializedObj.FindProperty("_saturation");
#endif
        }

        public override void OnInspectorGUI() {
            _serializedObj.Update();
            PostEffect pe = target as PostEffect;

            bool isModuleChanged = false;


            //Color bgColor = GUI.backgroundColor = new Color(.74f, .74f, 1f, 1f);
            //Color contentColor = GUI.contentColor = new Color(.8f, .8f, 1f, 1f);

#if ENABLE_DOF_GUI
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_DOFEnabled, new GUIContent("Depth Of Field"));
            if (EditorGUI.EndChangeCheck())
                isModuleChanged = true;
            if (_DOFEnabled.boolValue) {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_DOFForegroundBlur, new GUIContent("Foreground Blur"));
                if (EditorGUI.EndChangeCheck())
                    isModuleChanged = true;
                EditorGUILayout.PropertyField(_DOFFocalDistance, new GUIContent("Focal Distance"));
                EditorGUILayout.PropertyField(_DOFSmoothness, new GUIContent("Smoothness"));
                EditorGUILayout.PropertyField(_DOFBlurWidth, new GUIContent("Blur Width"));
                EditorGUILayout.PropertyField(_DOFFocalObject, new GUIContent("Focal Object"));
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
#endif

#if ENABLE_SUNSHAFT_GUI
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_sunShaftsEnabled, new GUIContent("Sun Shafts"));
            if (EditorGUI.EndChangeCheck())
                isModuleChanged = true;
            if (_sunShaftsEnabled.boolValue) {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_sunShaftsScreenBlendMode, new GUIContent("Blend Mode"));
                if (EditorGUI.EndChangeCheck())
                    isModuleChanged = true;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_sunTransform, new GUIContent("Shafts Caster", "Chose a transform that acts as a root point for the produced sun shafts"));
                if (pe._sunTransform) {
                    if (GUILayout.Button("Move to screen center")) {
                        if (EditorUtility.DisplayDialog("", "" + pe._sunTransform.name + "将被移动到当前摄像机的前方。 确定进行吗? ", "确定", "取消")) {
                            Camera editorCamera = SceneView.currentDrawingSceneView.camera;
                            Ray ray = editorCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                            pe._sunTransform.position = ray.origin + ray.direction * 500.0f;
                            pe._sunTransform.LookAt(editorCamera.transform);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (pe._sunTransform) {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    _showCrosshair = GUILayout.Toggle(_showCrosshair, "Show Crosshair");
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.PropertyField(_sunColor, new GUIContent("Shafts Color"));
                _sunShaftsMaxRadius.floatValue = 1.0f - EditorGUILayout.Slider("Distance Falloff", 1.0f - _sunShaftsMaxRadius.floatValue, 0, 1);
                EditorGUILayout.PropertyField(_sunShaftsBlurRadius, new GUIContent("Blur Size"));
                EditorGUILayout.PropertyField(_sunShaftsIntensity, new GUIContent("Intensity"));
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }     
#endif

#if ENABLE_BLOOM_GUI
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_bloomEnabled, new GUIContent("Bloom"));
            if (EditorGUI.EndChangeCheck())
                isModuleChanged = true;
            if (_bloomEnabled.boolValue) {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_bloomIntensity, new GUIContent("Intensity"));
                EditorGUILayout.PropertyField(_bloomThreshhold, new GUIContent("Threshhold"));
                EditorGUILayout.PropertyField(_bloomBlurWidth, new GUIContent("Blur Width"));
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
#endif

#if ENABLE_RADIALBLUR_GUI
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_radialBlurEnabled, new GUIContent("Radial Blur"));
            if (EditorGUI.EndChangeCheck())
                isModuleChanged = true;
            if (_radialBlurEnabled.boolValue) {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_radialBlurWidth, new GUIContent("Blur Width"));
                EditorGUILayout.PropertyField(_radialBlurRange, new GUIContent("Blur Range"));
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
#endif

#if ENABLE_CC_GUI
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_colorCorrectionEnabled, new GUIContent("Color Correction"));
            if (EditorGUI.EndChangeCheck())
                isModuleChanged = true;
            if (_colorCorrectionEnabled.boolValue) {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_colorCorrectionMode, new GUIContent("Type"));
                if (EditorGUI.EndChangeCheck())
                    isModuleChanged = true;
                if (_colorCorrectionMode.enumValueIndex == (int)PostEffect.ColorCorrectionMode.Simple) {
                    GUILayout.Label("Curves", EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(_CCRedChannel, new GUIContent("  Red"));
                    EditorGUILayout.PropertyField(_CCGreenChannel, new GUIContent("  Green"));
                    EditorGUILayout.PropertyField(_CCBlueChannel, new GUIContent("  Blue"));
                    if (EditorGUI.EndChangeCheck()) {
                        _serializedObj.ApplyModifiedProperties();
                        pe.UpdateTextures();
                    }
                } else if (_colorCorrectionMode.enumValueIndex == (int)PostEffect.ColorCorrectionMode.Amplify) {
                    EditorGUILayout.PropertyField(_CCLutTexture, new GUIContent("Lut Texture"));
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
#endif

#if ENABLE_VIGN_GUI
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_vignettingEnabled, new GUIContent("Vignetting"));
            if (EditorGUI.EndChangeCheck())
                isModuleChanged = true;
            if (_vignettingEnabled.boolValue) {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_vignettingIntensity, new GUIContent("Intensity"));
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
#endif

#if ENABLE_SAT_GUI
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_saturationEnable, new GUIContent("Saturation"));
            if (EditorGUI.EndChangeCheck())
                isModuleChanged = true;
            if (_saturationEnable.boolValue) {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_saturation, new GUIContent("Saturation"));
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
#endif

            _serializedObj.ApplyModifiedProperties();

            if (isModuleChanged) {
                pe.EnableShaderKeywords();
            }
        }


        void OnSceneGUI() {
#if ENABLE_SUNSHAFT_GUI
            if ((target as PostEffect).SunShaftsEnabled) {               
                if (_showCrosshair) {
                    Camera editorCamera = SceneView.currentDrawingSceneView.camera;

                    float halfViewWidth = editorCamera.pixelWidth / 2;
                    float halfViewHeight = editorCamera.pixelHeight / 2;

                    Handles.color = Color.green;

                    Vector3 p1 = editorCamera.ScreenToWorldPoint(new Vector3(halfViewWidth - 15, halfViewHeight, editorCamera.nearClipPlane + 0.2f));
                    Vector3 p2 = editorCamera.ScreenToWorldPoint(new Vector3(halfViewWidth + 15, halfViewHeight, editorCamera.nearClipPlane + 0.2f));
                    Handles.DrawLine(p1, p2);

                    p1 = editorCamera.ScreenToWorldPoint(new Vector3(halfViewWidth, halfViewHeight - 15, editorCamera.nearClipPlane + 0.2f));
                    p2 = editorCamera.ScreenToWorldPoint(new Vector3(halfViewWidth, halfViewHeight + 15, editorCamera.nearClipPlane + 0.2f));
                    Handles.DrawLine(p1, p2);
                }
            }
#endif

        }
    }
}

