//
// Edelweiss.CloudSystemEditor.CloudCreatorEditorSupport.cs: Common inspector functionality for the cloud creator.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {

	public class CloudCreatorEditorSupport <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		public static void CloudCreatorInspectorGUI (CloudEditor <C, PD, CD> a_CloudSystemEditor, SerializedProperty a_DensityProperty) {
			
			GUIContent l_CloudCreatorLabel = new GUIContent ("Cloud Creator", "Show/Hide cloud creator");
			GUIContent l_DensityLabel = new GUIContent ("Density", "The particle density used for the creation.");
			GUIContent l_CreateParticlesButton = new GUIContent ("Create Particles", "Create the particles using the given density, shapes and particle groups.");;
			
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			CloudSystemPrefs.CloudCreatorFoldout = EditorGUILayout.Foldout (CloudSystemPrefs.CloudCreatorFoldout, l_CloudCreatorLabel);
			if (CloudSystemPrefs.CloudCreatorFoldout) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				
				InspectorSupport.Explanation (null, "Set the cloud density to control the particle count of the cloud.", null);
				EditorGUILayout.Space ();
				
				a_DensityProperty.floatValue = EditorGUILayout.Slider (l_DensityLabel, a_DensityProperty.floatValue, 0.0f, 1.0f);
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button (l_CreateParticlesButton)) {
					a_CloudSystemEditor.CreateCloud ();
				}
				EditorGUILayout.EndHorizontal ();
				
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
			
			EditorGUILayout.EndVertical ();
		}
	}
}
