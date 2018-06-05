using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(FxCycle))]
class FxCycleInspector : Editor {

	private SerializedObject fxCycle;
	private SerializedProperty particleFx;
	private SerializedProperty nextParticleFx;
	private SerializedProperty nextParticleSmokeFx;
	private SerializedProperty circleLights;
	private SerializedProperty camSpline;
	private SerializedProperty splineOffsetSpeed;
	private SerializedProperty camPivot;
	private SerializedProperty camRotation;
	private SerializedProperty camMovementSpeed;
	private SerializedProperty camRotationSpeed;
	private SerializedProperty skyboxStartColor;
	private SerializedProperty skyboxNextFxColor;
	private SerializedProperty skyboxNextFxColorTime;
	private SerializedProperty skyboxColorChangeTime;
	private SerializedProperty pressSpaceText;
	private SerializedProperty particleBlastSound;
	private SerializedProperty isSelfRunning;
	private SerializedProperty looping;

	GUIStyle boxStyle;

	void OnEnable () {
		fxCycle = new SerializedObject((FxCycle)target);
		particleFx = fxCycle.FindProperty("particleFx");
		nextParticleFx = fxCycle.FindProperty("nextParticleFx");
		nextParticleSmokeFx = fxCycle.FindProperty("nextParticleSmokeFx");
		circleLights = fxCycle.FindProperty("circleLights");
		camSpline = fxCycle.FindProperty("camSpline");
		splineOffsetSpeed = fxCycle.FindProperty("splineOffsetSpeed");
		camPivot = fxCycle.FindProperty("camPivot");
		camRotation = fxCycle.FindProperty("camRotation");
		camMovementSpeed = fxCycle.FindProperty("camMovementSpeed");
		camRotationSpeed = fxCycle.FindProperty("camRotationSpeed");
		skyboxStartColor = fxCycle.FindProperty("skyboxStartColor");
		skyboxNextFxColor = fxCycle.FindProperty("skyboxNextFxColor");
		skyboxNextFxColorTime = fxCycle.FindProperty("skyboxNextFxColorTime");
		skyboxColorChangeTime = fxCycle.FindProperty("skyboxColorChangeTime");
		particleBlastSound = fxCycle.FindProperty("particleBlastSound");
		pressSpaceText = fxCycle.FindProperty("pressSpaceText");
		isSelfRunning = fxCycle.FindProperty("isSelfRunning");
		looping = fxCycle.FindProperty("looping");
	}

	public override void OnInspectorGUI () {
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");

		fxCycle.Update();

		// Settings
		EditorGUILayout.PropertyField(nextParticleFx);
		EditorGUILayout.PropertyField(nextParticleSmokeFx);
		EditorGUILayout.PropertyField(circleLights);
		EditorGUILayout.PropertyField(camSpline);
		EditorGUILayout.PropertyField(splineOffsetSpeed);
		EditorGUILayout.PropertyField(camPivot);
		EditorGUILayout.PropertyField(camRotation);
		EditorGUILayout.PropertyField(camMovementSpeed);
		EditorGUILayout.PropertyField(camRotationSpeed);
		EditorGUILayout.PropertyField(skyboxStartColor);
		EditorGUILayout.PropertyField(skyboxNextFxColor);
		EditorGUILayout.PropertyField(skyboxNextFxColorTime);
		EditorGUILayout.PropertyField(skyboxColorChangeTime);
		EditorGUILayout.PropertyField(pressSpaceText);
		EditorGUILayout.PropertyField(particleBlastSound);
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(isSelfRunning);
		EditorGUILayout.PropertyField(looping);
		EditorGUILayout.Separator();

		// Fx list
		EditorGUILayout.BeginVertical(boxStyle);
		for (int i = 0; i<particleFx.arraySize; i++) {
			EditorGUILayout.BeginVertical(boxStyle);
			SerializedProperty foldout = particleFx.GetArrayElementAtIndex(i).FindPropertyRelative("unfolded");
			SerializedProperty particles = particleFx.GetArrayElementAtIndex(i).FindPropertyRelative("particles");
			EditorGUILayout.BeginHorizontal();
			foldout.boolValue = GUILayout.Toggle(foldout.boolValue, "FX "+i, EditorStyles.foldout);
			EditorGUILayout.Space ();
			if (GUILayout.Button ("Move Up", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false)))
				particleFx.MoveArrayElement(i, i==0?particleFx.arraySize-1:i-1);
			if (GUILayout.Button ("Move Down", EditorStyles.miniButtonMid, GUILayout.ExpandWidth(false)))
				particleFx.MoveArrayElement(i, i<particleFx.arraySize-1?i+1:0);
			if (GUILayout.Button ("Remove", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false))) {
				if (EditorUtility.DisplayDialog("Remove Fx?", "This will remove the fx item from the list. Do you want to continue?", "Delete", "Cancel")) {
					particleFx.DeleteArrayElementAtIndex(i);
					fxCycle.ApplyModifiedProperties();
					return;
				}
			}
			EditorGUILayout.EndHorizontal();
			if (foldout.boolValue) {
				EditorGUILayout.PropertyField(particles);
				EditorGUILayout.PropertyField(particleFx.GetArrayElementAtIndex(i).FindPropertyRelative("colorProfile"));
				EditorGUILayout.PropertyField(particleFx.GetArrayElementAtIndex(i).FindPropertyRelative("moveInOut"));
			}
			EditorGUILayout.EndVertical();
		}
		if (GUILayout.Button ("Create", GUILayout.ExpandWidth (false)))
			particleFx.InsertArrayElementAtIndex(particleFx.arraySize==0? 0 : particleFx.arraySize-1);
		EditorGUILayout.EndVertical();

		fxCycle.ApplyModifiedProperties();
	}
}
