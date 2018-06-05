//
// Edelweiss.CloudSystemEditor.VerticalColorModeEditor.cs: Inspector and scene view for the vertical color mode.
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

	public class VerticalColorModeEditor <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		private C m_Cloud;
		private CloudEditor <C, PD, CD> m_CloudEditor;
		
		private SerializedObject m_SerializedCloud;
		private bool m_ReloadSerializedCreatorData;
		private List <VerticalColorProperties> m_VerticalColorProperties = new List <VerticalColorProperties> ();
		private CS_VerticalColor m_VerticalColorToDelete;
		
		private GUIContent m_DeleteColorButton;
		private GUIContent m_SortButton;
		private GUIContent m_AddColorButton;
		private GUIContent m_HeightLabel;
		private GUIContent m_ColorLabel;
		
		public void Initialize (CloudEditor <C, PD, CD> a_CloudSystemEditor) {
			m_CloudEditor = a_CloudSystemEditor;
			m_Cloud = m_CloudEditor.Cloud;
			
			m_DeleteColorButton = new GUIContent ("Delete Color", "Delete this color.");
			m_SortButton = new GUIContent ("Sort", "Sort the vertical colors.");
			m_AddColorButton = new GUIContent ("Add Color", "Add a new color.");
			m_HeightLabel = new GUIContent ("Height", "Height of this vertical color.");
			m_ColorLabel = new GUIContent ("Color", "Color of this vertical color.");
			
			InitializeSerializedObject ();
		}
		
		private void InitializeSerializedObject () {
			bool l_ExistedBefore = (m_SerializedCloud != null);
		
			m_SerializedCloud = new SerializedObject (m_Cloud);
			
			m_VerticalColorProperties.Clear ();
			for (int i = m_Cloud.verticalColors.Length - 1; i >= 0; i = i - 1) {
				VerticalColorProperties l_VerticalColorProperties = new VerticalColorProperties ();
				l_VerticalColorProperties.Initialize <C, PD, CD> (m_SerializedCloud, m_Cloud, i);
				m_VerticalColorProperties.Add (l_VerticalColorProperties);
			}
			
			if (l_ExistedBefore) {
				m_SerializedCloud.Update ();
				EditorUtility.SetDirty (m_Cloud);
			}
		}
		
		public void InspectorGUI () {
			
				// Support Undo/Redo if a vertical color is added or deleted.
			if (m_VerticalColorProperties.Count != m_Cloud.verticalColors.Length) {
				InitializeSerializedObject ();
			}
			
			m_ReloadSerializedCreatorData = false;
			m_SerializedCloud.Update ();
			
			
			EditorGUILayout.Space ();	
			if (m_Cloud.CloudRenderer.SupportsVerticalColors) {
				InspectorSupport.Explanation ("Vertical Colors",
				                              "Modify, add or remove vertical colors. ",
				                              null);
				
				if (!m_Cloud.AreVerticalColorsSorted ()) {
					InspectorSupport.Explanation ("Warning", "Vertical colors are not sorted. They need to be sorted in order to get valid results.", null);
				}
			
				EditorGUILayout.Space ();
				VerticalColorsInspectorGUI ();
			} else {
				InspectorSupport.Explanation ("Vertical Colors",
				                              "The used renderer does not support vertical colors.",
				                              null);
			}
			
			if (m_ReloadSerializedCreatorData) {
				InitializeSerializedObject ();
			}
			
			m_SerializedCloud.ApplyModifiedProperties ();
			if (GUI.changed) {
				EditorUtility.SetDirty (m_Cloud);
			}
		}
		
		private void VerticalColorsInspectorGUI () {
			m_VerticalColorToDelete = null;
			
			for (int i = 0; i < m_VerticalColorProperties.Count; i = i + 1) {
				VerticalColorProperties l_VerticalColorProperties = m_VerticalColorProperties [i];
				CS_VerticalColor l_VerticalColor = m_Cloud.verticalColors [m_Cloud.verticalColors.Length - 1 - i];
				
				VerticalColorInspectorGUI (l_VerticalColor, l_VerticalColorProperties);
			}
			
			if (m_VerticalColorToDelete != null) {
				Undo.RegisterUndo (m_Cloud, "Remove vertical color");
				int l_VerticalColorIndex = System.Array.IndexOf (m_Cloud.verticalColors, m_VerticalColorToDelete);
				ArraySupport.RemoveAt (ref m_Cloud.verticalColors, l_VerticalColorIndex);
				m_ReloadSerializedCreatorData = true;
			}
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			
			if (m_Cloud.verticalColors.Length > 1) {
				if (GUILayout.Button (m_SortButton)) {
					Undo.RegisterUndo (m_Cloud, "Sort colors");
					m_Cloud.SortVerticalColors ();
					m_ReloadSerializedCreatorData = true;
				}
			}
			
			if (GUILayout.Button (m_AddColorButton)) {
				Undo.RegisterUndo (m_Cloud, "Add vertical color");
				ArraySupport.Add (ref m_Cloud.verticalColors , new CS_VerticalColor ());
				m_Cloud.SortVerticalColors ();
				m_ReloadSerializedCreatorData = true;
				
					// HACK:
					// That call should not be necessary, as the factors should automatically be recalculated
					// as the scene is drawn. (CloudEditor.OnSceneGUI)
				m_Cloud.RecalculateVerticalColorFactors ();
			}
			EditorGUILayout.EndHorizontal ();
		}
		
		private void VerticalColorInspectorGUI (CS_VerticalColor a_VerticalColor, VerticalColorProperties a_VerticalColorProperties) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			if (a_VerticalColor.isPermanent) {
				GUI.enabled = false;
			}
	
			EditorGUILayout.Slider (a_VerticalColorProperties.verticalFactorProperty, 0.0f, 1.0f, m_HeightLabel);
			
			if (a_VerticalColor.isPermanent) {
				GUI.enabled = true;
			}
			
			EditorGUILayout.PropertyField (a_VerticalColorProperties.colorProperty, m_ColorLabel);
			
			if (!a_VerticalColor.isPermanent) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button (m_DeleteColorButton)) {
					m_VerticalColorToDelete = a_VerticalColor;
				}
				EditorGUILayout.EndHorizontal ();
			}
			
			EditorGUILayout.EndVertical ();
		}
		
		public void SceneGUI () {
		}
	}
}
