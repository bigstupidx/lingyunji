//
// Edelweiss.CloudSystemEditor.ShadingModeEditor.cs: Inspector and scene view for the shading mode.
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

	public class ShadingModeEditor <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		private C m_Cloud;
		private CloudEditor <C, PD, CD> m_CloudEditor;
		
		private SerializedObject m_SerializedCloud;
		private bool m_ReloadSerializedCreatorData;
		private List <ShadingColorProperties> m_ShadingColorProperties = new List <ShadingColorProperties> ();
		private CS_ShadingColor m_ShadingColorToDelete;
			
		private List <ShadingGroupProperties> m_ShadingGroupProperties = new List <ShadingGroupProperties> ();
		private CS_ShadingGroup m_ShadingGroupToDelete;
		
		private GUIContent m_DeleteColorButton;
		private GUIContent m_AddColorButton;
		private GUIContent m_SortButton;
		private GUIContent m_FactorLabel;
		private GUIContent m_VerticalColorLabel;
		private GUIContent m_NameLabel;
		private GUIContent m_ShapeColorLabel;
		private GUIContent m_DeleteGroupButton;
		private GUIContent m_AddGroupButton;
		
		public void Initialize (CloudEditor <C, PD, CD> a_CloudSystemEditor) {
			m_CloudEditor = a_CloudSystemEditor;
			m_Cloud = m_CloudEditor.Cloud;
			
			m_DeleteColorButton = new GUIContent ("Delete Color", "Delete this color.");
			m_SortButton = new GUIContent ("Sort", "Sort the shading colors.");
			m_AddColorButton = new GUIContent ("Add Color", "Add a new color.");
			m_FactorLabel = new GUIContent ("Factor", "Factor of this shading color.");
			m_VerticalColorLabel = new GUIContent ("Color", "Color of this shading color.");
			m_NameLabel = new GUIContent ("Name", "Name of shading group.");
			m_ShapeColorLabel = new GUIContent ("Color", "Color of shading group shape");
			m_DeleteGroupButton = new GUIContent ("Delete Group", "Delete this shading group.");
			m_AddGroupButton = new GUIContent ("Add Group", "Add a new shading group.");
			
			InitializeSerializedObject ();
		}
		
		private void InitializeSerializedObject () {
			bool l_ExistedBefore = (m_SerializedCloud != null);
		
			m_SerializedCloud = new SerializedObject (m_Cloud);
			
			m_ShadingColorProperties.Clear ();
			for (int i = m_Cloud.shadingColors.Length - 1; i >= 0; i = i - 1) {
				ShadingColorProperties l_ShadingColorProperties = new ShadingColorProperties ();
				l_ShadingColorProperties.Initialize <C, PD, CD> (m_SerializedCloud, m_Cloud, i);
				m_ShadingColorProperties.Add (l_ShadingColorProperties);
			}		
			
			m_ShadingGroupProperties.Clear ();
			for (int i = 0; i < m_Cloud.shadingGroups.Length; i = i + 1) {
				ShadingGroupProperties l_ShadingGroupProperties = new ShadingGroupProperties ();
				l_ShadingGroupProperties.Initialize <C, PD, CD> (m_SerializedCloud, m_Cloud, i);
				m_ShadingGroupProperties.Add (l_ShadingGroupProperties);
			}
			
			if (l_ExistedBefore) {
				m_SerializedCloud.Update ();
				EditorUtility.SetDirty (m_Cloud);
			}
		}
		
		public void InspectorGUI () {
			
				// Support Undo/Redo if a shading color or group is added or deleted.
			if
				(m_ShadingColorProperties.Count != m_Cloud.shadingColors.Length ||
				 m_ShadingGroupProperties.Count != m_Cloud.shadingGroups.Length)
			{
				InitializeSerializedObject ();
			}
			
			m_ReloadSerializedCreatorData = false;
			m_SerializedCloud.Update ();
			
			EditorGUILayout.Space ();
			if (m_Cloud.CloudRenderer.SupportsShadingGroups) {
				InspectorSupport.Explanation ("Shading",
				                              "Set the shading colors. Modify, add or remove shading groups.", "Drag the circles to move the center of a specific shading group.");
				
				EditorGUILayout.Space ();
				GUILayout.Label ("Shading Colors", EditorStyles.boldLabel);
				
				EditorGUILayout.Space ();
				ShadingColorsInspectorGUI ();
				
				EditorGUILayout.Space ();
				ShadingGroupsInspectorGUI ();
			} else {
				InspectorSupport.Explanation ("Shading",
				                              "The used renderer does not support shading groups.", null);
			}
	
			if (m_ReloadSerializedCreatorData) {
				InitializeSerializedObject ();
			}
			
			m_SerializedCloud.ApplyModifiedProperties ();
			if (GUI.changed) {
				EditorUtility.SetDirty (m_Cloud);
			}
		}
		
		private void ShadingColorsInspectorGUI () {
			m_ShadingColorToDelete = null;
			
			for (int i = 0; i < m_ShadingColorProperties.Count; i = i + 1) {
				ShadingColorProperties l_ShadingColorProperties = m_ShadingColorProperties [i];
				CS_ShadingColor l_ShadingColor = m_Cloud.shadingColors [m_Cloud.shadingColors.Length - 1 - i];
				
				ShadingColorInspectorGUI (l_ShadingColor, l_ShadingColorProperties);
			}
			
			if (m_ShadingColorToDelete != null) {
				Undo.RegisterUndo (m_Cloud, "Remove shading color");
				int l_ShadingColorIndex = System.Array.IndexOf (m_Cloud.shadingColors, m_ShadingColorToDelete);
				ArraySupport.RemoveAt (ref m_Cloud.shadingColors, l_ShadingColorIndex);
				m_ReloadSerializedCreatorData = true;
			}
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			
			if (m_Cloud.shadingColors.Length > 1) {
				if (GUILayout.Button (m_SortButton)) {
					Undo.RegisterUndo (m_Cloud, "Sort colors");
					m_Cloud.SortShadingColors ();
					m_ReloadSerializedCreatorData = true;
				}
			}
			
			if (GUILayout.Button (m_AddColorButton)) {
				Undo.RegisterUndo (m_Cloud, "Add shading color");
				ArraySupport.Add (ref m_Cloud.shadingColors , new CS_ShadingColor ());
				m_Cloud.SortShadingColors ();
				m_ReloadSerializedCreatorData = true;
				
					// HACK:
					// That call should not be necessary, as the factors should automatically be recalculated
					// as the scene is drawn. (CloudEditor.OnSceneGUI)
				m_Cloud.RecalculateShadingColorFactors ();
			}
			EditorGUILayout.EndHorizontal ();
		}
		
		private void ShadingColorInspectorGUI (CS_ShadingColor a_ShadingColor, ShadingColorProperties a_ShadingColorProperties) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			if (a_ShadingColor.isPermanent) {
				GUI.enabled = false;
			}
			//a_ShadingColorProperties.verticalFactorProperty.floatValue = EditorGUILayout.Slider (m_HeightLabel, a_ShadingColorProperties.verticalFactorProperty.floatValue, -1.0f, 1.0f);
			EditorGUILayout.Slider (a_ShadingColorProperties.verticalFactorProperty, -1.0f, 1.0f, m_FactorLabel);
			if (a_ShadingColor.isPermanent) {
				GUI.enabled = true;
			}
			//a_ShadingColorProperties.colorProperty.colorValue = EditorGUILayout.ColorField (m_VerticalColorLabel, a_ShadingColorProperties.colorProperty.colorValue);
			EditorGUILayout.PropertyField (a_ShadingColorProperties.colorProperty, m_VerticalColorLabel);
	
			if (!a_ShadingColor.isPermanent) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button (m_DeleteColorButton)) {
					m_ShadingColorToDelete = a_ShadingColor;
				}
				EditorGUILayout.EndHorizontal ();
			}
			
			EditorGUILayout.EndVertical ();
		}
		
		private void ShadingGroupsInspectorGUI () {
			
			GUILayout.Label ("Shading Groups", EditorStyles.boldLabel);
			
			m_ShadingGroupToDelete = null;
			
			for (int i = 0; i < m_ShadingGroupProperties.Count; i = i + 1) {
				CS_ShadingGroup l_ShadingGroup = m_Cloud.shadingGroups [i];
				ShadingGroupProperties l_ShadingGroupProperties = m_ShadingGroupProperties [i];
				
				ShadingGroupInspectorGUI (l_ShadingGroup, l_ShadingGroupProperties);
				EditorGUILayout.Space ();
			}
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button (m_AddGroupButton)) {
				AddShadingGroup ();
			}
			EditorGUILayout.EndHorizontal ();
			
			if (m_ShadingGroupToDelete != null) {
				DeleteShadingGroup ();
			}
		}
		
		private void ShadingGroupInspectorGUI (CS_ShadingGroup a_ShadingGroup, ShadingGroupProperties a_ShadingGroupProperties) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			a_ShadingGroupProperties.nameProperty.stringValue = EditorGUILayout.TextField (m_NameLabel, a_ShadingGroupProperties.nameProperty.stringValue);
			a_ShadingGroupProperties.shapeColorProperty.colorValue = EditorGUILayout.ColorField (m_ShapeColorLabel, a_ShadingGroupProperties.shapeColorProperty.colorValue);
			a_ShadingGroupProperties.centerProperty.vector3Value = EditorGUILayout.Vector3Field ("Center", a_ShadingGroupProperties.centerProperty.vector3Value);
			
			if (m_Cloud.shadingGroups.Length > 1) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button (m_DeleteGroupButton)) {
					m_ShadingGroupToDelete = a_ShadingGroup;
				}
				EditorGUILayout.EndHorizontal ();
			}
			
			EditorGUILayout.EndVertical ();
		}
		
		private void AddShadingGroup () {
			Undo.RegisterUndo (m_Cloud.CreatorData, "Add shading type group");
			ArraySupport.Add (ref m_Cloud.shadingGroups, new CS_ShadingGroup ());
			m_ReloadSerializedCreatorData = true;
		}
		
		private void DeleteShadingGroup () {
			
				// Only delete if there is no shape that uses that particle type group
			int l_ShadingGroupIndex = System.Array.IndexOf (m_Cloud.shadingGroups, m_ShadingGroupToDelete);
			bool l_IsShadingGroupUsed = false;
			foreach (CS_Shape l_BoxShape in m_Cloud.CreatorData.boxShapes) {
				if (l_BoxShape.shadingGroupIndex == l_ShadingGroupIndex) {
					l_IsShadingGroupUsed = true;
					break;
				}
			}
			
			if (!l_IsShadingGroupUsed) {
				Undo.RegisterUndo (m_Cloud.CreatorData, "Remove shading group");
				
				foreach (CS_Shape l_BoxShape in m_Cloud.CreatorData.boxShapes) {
					if (l_BoxShape.shadingGroupIndex > l_ShadingGroupIndex) {
						l_BoxShape.shadingGroupIndex = l_BoxShape.shadingGroupIndex - 1;
					}
				}
				for (int i = 0; i < m_Cloud.ParticleData.Count; i = i + 1) {
					CloudParticle l_Particle = m_Cloud.ParticleData [i];
					if (l_Particle.shadingGroupIndex > l_ShadingGroupIndex) {
						l_Particle.shadingGroupIndex = l_Particle.shadingGroupIndex - 1;
						m_Cloud.ParticleData [i] = l_Particle;
					}
				}
				
				ArraySupport.RemoveAt (ref m_Cloud.shadingGroups, l_ShadingGroupIndex);
				m_ReloadSerializedCreatorData = true;
			} else {
				
				EditorUtility.DisplayDialog ("Warning: Shading group is used", "You can not delete this shading group because there is a least one shape using it. If all shapes use other shading groups, it will be possible to delete this one.", "Ok");
			}
			
			m_ShadingGroupToDelete = null;
		}
		
		public void SceneGUI () {
			
			if (m_Cloud.SupportsShadingGroups) {
			
				Matrix4x4 l_LocalToWorldMatrix = m_Cloud.TransformMatrix ();
				Matrix4x4 l_WorldToLocalMatrix = l_LocalToWorldMatrix.inverse;
				
				Undo.SetSnapshotTarget (m_Cloud.CreatorData, "Move shading group");
				
				bool l_HandleIsUsed = false;
				
				Color l_HandlesColor = Handles.color;
				for (int i = 0; i < m_Cloud.shadingGroups.Length; i = i + 1) {
					
					int l_ControlIDBeforeHandle = GUIUtility.GetControlID (m_CloudEditor.CloudSystemEditorHash, FocusType.Passive);
					bool l_IsEventUsedBeforeHandle = (Event.current.type == EventType.used);
					
					CS_ShadingGroup l_ShadingGroup = m_Cloud.shadingGroups [i];
					Handles.color = l_ShadingGroup.shapeColor;
					Vector3 l_Position = l_LocalToWorldMatrix.MultiplyPoint3x4 (l_ShadingGroup.center);
					float l_HandleSize = HandleUtility.GetHandleSize (l_Position) * CloudSystemPrefs.c_SelectableCircleRadius;
					
						// HACK: Workaround for a Unity bug that may freeze the scene view.
					Vector3 l_NewPosition = l_Position;
//					if (HandlesSupport.IsHandleDrawingSave ()) {
						l_NewPosition = Handles.FreeMoveHandle (l_Position, Camera.current.transform.rotation, l_HandleSize, Vector3.one, Handles.CircleCap);
//					}
					
					int l_ControlIDAfterHandle = GUIUtility.GetControlID (m_CloudEditor.CloudSystemEditorHash, FocusType.Passive);
					bool l_IsEventUsedByHandle = false;
					if (!l_IsEventUsedBeforeHandle) {
						l_IsEventUsedByHandle = (Event.current.type == EventType.used);
					}
		
						// Only apply changes if the handle uses the current event or if the
						// hot control is from the handle.
		
					if
						((l_ControlIDBeforeHandle < GUIUtility.hotControl &&
						  GUIUtility.hotControl < l_ControlIDAfterHandle) ||
						  l_IsEventUsedByHandle)
					{
						l_NewPosition = l_WorldToLocalMatrix.MultiplyPoint3x4 (l_NewPosition);
						l_ShadingGroup.center = l_NewPosition;
					}
					
					if
						((l_ControlIDBeforeHandle < HandleUtility.nearestControl &&
						  HandleUtility.nearestControl < l_ControlIDAfterHandle) ||
						  l_IsEventUsedByHandle)
					{
						l_HandleIsUsed = true;
					}
				}
				
				if (l_HandleIsUsed) {
					m_CloudEditor.Repaint ();
				}
				
				Handles.color = l_HandlesColor;
			}
		}
	}
}
