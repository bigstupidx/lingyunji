//
// Edelweiss.CloudSystemEditor.ParticleModeEditor.cs: Inspector and scene view for the particle mode.
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

	public class ParticleModeEditor <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		private C m_Cloud;
		private CloudEditor <C, PD, CD> m_CloudEditor;
		
		private SerializedObject m_SerializedParticleData;
		private bool m_ReloadSerializedParticleData;
		
		private SerializedProperty m_SelectedParticlePosition;
		private SerializedProperty m_SelectedParticleRotation;
		private SerializedProperty m_SelectedParticleSize;
		private SerializedProperty m_SelectedParticleUVIndex;
		private SerializedProperty m_SelectedParticleIsSquared;
		private SerializedProperty m_SelectedParticleIsCoreParticle;
		private SerializedProperty m_SelectedParticleShadingGroupIndex;
	
		
		private GUIContent m_AddButton;
		private GUIContent m_DuplicateButton;
		private GUIContent m_DeleteButton;
		
		private GUIContent m_ShadingGroupLabel;
		private GUIContent m_IsSquaredLabel;
		private GUIContent m_IsCoreParticleLabel;
		private GUIContent m_RotationLabel;
		private GUIContent m_SizeLabel;
		private GUIContent m_UVIndexLabel;
		private GUIContent m_SelectedParticleIndexLabel;
		
		private int m_SelectedParticleIndex;
		private int SelectedParticleIndex {
			get {
				return (m_SelectedParticleIndex);
			}
			set {
				m_SelectedParticleIndex = value;
				
				if (IsSelectedParticleIndexValid) {
					m_SerializedParticleData.ApplyModifiedProperties ();
					m_SerializedParticleData.Update ();
					
					m_SelectedParticlePosition = m_SerializedParticleData.FindProperty ("m_Positions.Array.data[" + m_SelectedParticleIndex + "]");
					m_SelectedParticleRotation = m_SerializedParticleData.FindProperty ("m_Rotations.Array.data[" + m_SelectedParticleIndex + "]");
					m_SelectedParticleSize = m_SerializedParticleData.FindProperty ("m_Sizes.Array.data[" + m_SelectedParticleIndex + "]");
					m_SelectedParticleUVIndex = m_SerializedParticleData.FindProperty ("m_UVIndices.Array.data[" + m_SelectedParticleIndex + "]");
					m_SelectedParticleIsSquared = m_SerializedParticleData.FindProperty ("m_IsSquareds.Array.data[" + m_SelectedParticleIndex + "]");
					m_SelectedParticleIsCoreParticle = m_SerializedParticleData.FindProperty ("m_IsCoreParticles.Array.data[" + m_SelectedParticleIndex + "]");
					m_SelectedParticleShadingGroupIndex = m_SerializedParticleData.FindProperty ("m_ShadingGroupIndices.Array.data[" + m_SelectedParticleIndex + "]");
				}
			}
		}
		private bool IsSelectedParticleIndexValid {
			get {
				bool l_Result = (0 <= SelectedParticleIndex && SelectedParticleIndex < m_Cloud.ParticleData.Count);
				return (l_Result);
			}
		}
		
		public void Initialize (CloudEditor <C, PD, CD> a_CloudSystemEditor) {
			m_CloudEditor = a_CloudSystemEditor;
			m_Cloud = m_CloudEditor.Cloud;
			
			m_SerializedParticleData = new SerializedObject (m_Cloud.ParticleData);
			SelectedParticleIndex = 0;
			
			m_AddButton = new GUIContent ("Add", "Add a new particle");
			m_DuplicateButton = new GUIContent ("Duplicate", "Duplicate the selected particle.");
			m_DeleteButton = new GUIContent ("Delete", "Delete the selected particle.");
			
			m_ShadingGroupLabel = new GUIContent ("Shading Group", "Change the shading group of the selected particle.");
			m_IsSquaredLabel = new GUIContent ("Squared", "Is the selected particle squared?");
			m_IsCoreParticleLabel = new GUIContent ("Core Particle", "Does the selected particle belong to the core?");
			m_RotationLabel = new GUIContent ("Rotation", "Change the rotation of the selected particle.");
			m_SizeLabel = new GUIContent ("Size", "Change the size of the selected particle.");
			m_UVIndexLabel = new GUIContent ("UV Index", "Change the UV index of the selected particle.");
			m_SelectedParticleIndexLabel = new GUIContent ("Particle Index", "Index of selected particle.");
			
			InitializeSerializedObject ();
		}
		
		private void InitializeSerializedObject () {
			bool l_ExistedBefore = (m_SerializedParticleData != null);
		
			m_SerializedParticleData = new SerializedObject (m_Cloud.ParticleData);
			
			SelectedParticleIndex = SelectedParticleIndex;
			
			if (l_ExistedBefore) {
				m_SerializedParticleData.Update ();
				EditorUtility.SetDirty (m_Cloud.ParticleData);
			}
		}
		
		public void InspectorGUI () {
			
			m_ReloadSerializedParticleData = false;
			m_SerializedParticleData.Update ();
			
			EditorGUILayout.Space ();
			if (m_Cloud.ParticleCount == 0) {
				InspectorSupport.Explanation ("Particles",
				                              "There are no particles in this cloud so far. You may either generate the particles using the Cloud Creator tools or add particle by particle.",
				                              null);
			} else {
				InspectorSupport.Explanation ("Particles",
				                              "Modify, add or remove particles.",
				                              "Click on the circle of a particle to change the selection\nUse the transform tools to modify the selected particle\nPress Shift-D to duplicate the selected particle\nPress Delete to remove the selected particle");
			}
			
			ParticleModeInspectorGUI ();
			
			if (m_ReloadSerializedParticleData) {
				InitializeSerializedObject ();
			}
			
			m_SerializedParticleData.ApplyModifiedProperties ();
			if (GUI.changed) {
				EditorUtility.SetDirty (m_Cloud.ParticleData);
			}
		}
		
		private void ParticleModeInspectorGUI () {
			if (m_Cloud.ParticleCount > 0) {
			
					// Make sure that the serialized data is always valid.
				if (m_SelectedParticlePosition == null) {
					InitializeSerializedObject ();
				}
				
				
					// Particles
			
				EditorGUILayout.Space ();
				GUILayout.Label ("Particle Properties", EditorStyles.boldLabel);
				
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				
				if (m_Cloud.CloudRenderer.SupportsShadingGroups) {
				
						// Shading group selection
					List <string> l_ShadingGroupNames = new List <string> ();
					for (int i = 0; i < m_Cloud.shadingGroups.Length; i = i + 1) {
						string l_Name = m_Cloud.shadingGroups [i].name;
						if (l_Name == "") {
							l_Name = " ";
						}
						while (l_ShadingGroupNames.Contains (l_Name)) {
							l_Name = l_Name + " ";
						}
						
						l_ShadingGroupNames.Add (l_Name);
					}
					List <GUIContent> l_ShadingGroupNamesGUIContent = new List <GUIContent> ();
					for (int i = 0; i < l_ShadingGroupNames.Count; i = i + 1) {
						l_ShadingGroupNamesGUIContent.Add (new GUIContent (l_ShadingGroupNames [i]));
					}
					m_SelectedParticleShadingGroupIndex.intValue = EditorGUILayout.Popup (m_ShadingGroupLabel, m_SelectedParticleShadingGroupIndex.intValue, l_ShadingGroupNamesGUIContent.ToArray ());
				}
				
				if (m_Cloud.CloudRenderer.SupportsNonSquaredParticles) {
					m_SelectedParticleIsSquared.boolValue = EditorGUILayout.Toggle (m_IsSquaredLabel, m_SelectedParticleIsSquared.boolValue);
				}
				
				m_SelectedParticleIsCoreParticle.boolValue = EditorGUILayout.Toggle (m_IsCoreParticleLabel, m_SelectedParticleIsCoreParticle.boolValue);
				m_SelectedParticlePosition.vector3Value = EditorGUILayout.Vector3Field ("Position", m_SelectedParticlePosition.vector3Value);
				if (m_SelectedParticleIsSquared.boolValue || !m_Cloud.CloudRenderer.SupportsNonSquaredParticles) {
					float l_NewSize = EditorGUILayout.FloatField (m_SizeLabel, m_SelectedParticleSize.vector2Value.x);
					if (l_NewSize < 0.0f) {
						l_NewSize = 0.0f;
					}
					m_SelectedParticleSize.vector2Value = new Vector2 (l_NewSize, l_NewSize);
				} else {
					Vector2 l_NewSize = EditorGUILayout.Vector2Field ("Size", m_SelectedParticleSize.vector2Value);
					if (l_NewSize != m_SelectedParticleSize.vector2Value) {
						if (l_NewSize.x != m_SelectedParticleSize.vector2Value.x) {
							if (l_NewSize.x < 0.0f) {
								l_NewSize.x = 0.0f;
							}
						} else {
							if (l_NewSize.y < 0.0f) {
								l_NewSize.y = 0.0f;
							}
						}
						m_SelectedParticleSize.vector2Value = l_NewSize;
					}
				}
				m_SelectedParticleRotation.floatValue = EditorGUILayout.FloatField (m_RotationLabel, m_SelectedParticleRotation.floatValue);
				m_SelectedParticleUVIndex.intValue = EditorGUILayout.IntSlider (m_UVIndexLabel, m_SelectedParticleUVIndex.intValue, 1, m_Cloud.TileCount);
				
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
				
				EditorGUILayout.Space ();
			}
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (IsSelectedParticleIndexValid) {
				if (GUILayout.Button (m_DeleteButton)) {
					DeleteSelectedParticle ();
				}
				EditorGUILayout.Space ();
				if (GUILayout.Button (m_DuplicateButton)) {
					DuplicateSelectedParticle ();
				}
			}
			if (GUILayout.Button (m_AddButton)) {
				AddParticle ();
			}
			EditorGUILayout.EndHorizontal ();
			
			if (m_Cloud.ParticleCount > 1) {
				
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				
				GUILayout.Label ("Selected Particle", EditorStyles.boldLabel);
				
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				
				SelectedParticleIndex = EditorGUILayout.IntSlider (m_SelectedParticleIndexLabel, SelectedParticleIndex, 0, m_Cloud.ParticleCount - 1);
				
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
			
			EditorGUILayout.Space ();
		}
		
		public void SceneGUI () {
			ProcessParticleKeyEvents ();
			
			Matrix4x4 l_LocalToWorldMatrix = m_Cloud.TransformMatrix ();
			Matrix4x4 l_WorldToLocalMatrix = l_LocalToWorldMatrix.inverse;
			
			
				// Draw circles for all but the selected particle.
			
			Color l_PreviousColor = Handles.color;
			Handles.color = CloudSystemPrefs.HighlightColor;
			for (int i = 0; i < m_Cloud.ParticleData.Count; i = i + 1) {
				if (i != SelectedParticleIndex) {
					Vector3 l_Position = l_LocalToWorldMatrix.MultiplyPoint3x4 (m_Cloud.ParticleData [i].position);
					float l_HandleSize = HandleUtility.GetHandleSize (l_Position) * CloudSystemPrefs.c_SelectableCircleRadius;
					Handles.CircleCap (-i, l_Position, Camera.current.transform.rotation, l_HandleSize);
				}
			}
			Handles.color = l_PreviousColor;
			
			
				// Functionality for selected particle
			
			bool l_HandleIsUsed = false;
			
			if (IsSelectedParticleIndexValid) {
				Vector3 l_SelectedWorldPosition = l_LocalToWorldMatrix.MultiplyPoint3x4 (m_Cloud.ParticleData [SelectedParticleIndex].position);
				
					// HACK: Workaround for a Unity bug that may freeze the scene view.
//				if (HandlesSupport.IsHandleDrawingSave ()) {
				
					if (ToolsSupport.Current == TransformTool.Move) {
		
							// Move
						
						Undo.SetSnapshotTarget(m_Cloud.ParticleData, "Moved Particle");
						Quaternion l_HandleRotation;
						if (ToolsSupport.CurrentPivotRotation == PivotRotationTool.Global) {
							l_HandleRotation = Quaternion.identity;
						} else {
							l_HandleRotation = m_Cloud.transform.rotation;
						}
						
						int l_ControlIDBeforeHandle = GUIUtility.GetControlID (m_CloudEditor.CloudSystemEditorHash, FocusType.Passive);
						bool l_IsEventUsedBeforeHandle = (Event.current.type == EventType.used);
						
						Vector3 l_TmpPosition = l_WorldToLocalMatrix.MultiplyPoint3x4 (Handles.PositionHandle (l_SelectedWorldPosition, l_HandleRotation));
						
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
							CloudParticle l_Particle = m_Cloud.ParticleData [SelectedParticleIndex];
							l_Particle.position = l_TmpPosition;					
							m_Cloud.ParticleData [SelectedParticleIndex] = l_Particle;
						}
						
						if
							((l_ControlIDBeforeHandle < HandleUtility.nearestControl &&
							  HandleUtility.nearestControl < l_ControlIDAfterHandle) ||
							  l_IsEventUsedByHandle)
						{
							l_HandleIsUsed = true;
						}
					
					} else if (ToolsSupport.Current == TransformTool.Rotate) {
						
							// Rotate
						
						Undo.SetSnapshotTarget(m_Cloud.ParticleData, "Rotated Particle");
						
						int l_ControlIDBeforeHandle = GUIUtility.GetControlID (m_CloudEditor.CloudSystemEditorHash, FocusType.Passive);
						bool l_IsEventUsedBeforeHandle = (Event.current.type == EventType.used);
						
						Quaternion l_CameraRotation = Camera.current.transform.rotation;
						Quaternion l_ParticleRotation = Quaternion.Euler (0.0f, 0.0f, m_Cloud.ParticleData [SelectedParticleIndex].rotation);
						l_ParticleRotation = Quaternion.Inverse (l_ParticleRotation);
						Quaternion l_Rotation = l_CameraRotation * l_ParticleRotation;
						Quaternion l_NewRotation = Handles.RotationHandle (l_Rotation, l_SelectedWorldPosition);
						
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
								// Cam * Rot(-1) = Cur
								// => Cam * Rot(-1) * Rot = Cur * Rot
								// => Cur(-1) * Cam = Rot
							
							Quaternion l_NewZRotation = Quaternion.Inverse (l_NewRotation) * l_CameraRotation;
							CloudParticle l_Particle = m_Cloud.ParticleData [SelectedParticleIndex];
							l_Particle.rotation = l_NewZRotation.eulerAngles.z;
							m_Cloud.ParticleData [SelectedParticleIndex] = l_Particle;
						}
						
						if
							((l_ControlIDBeforeHandle < HandleUtility.nearestControl &&
							  HandleUtility.nearestControl < l_ControlIDAfterHandle) ||
							  l_IsEventUsedByHandle)
						{
							l_HandleIsUsed = true;
						}
						
					} else if (ToolsSupport.Current == TransformTool.Scale) {
						
							// Scale
						
						Undo.SetSnapshotTarget(m_Cloud.ParticleData, "Scaled Particle");
						CloudParticle l_Particle = m_Cloud.ParticleData [SelectedParticleIndex];
						
						int l_ControlIDBeforeHandle = GUIUtility.GetControlID (m_CloudEditor.CloudSystemEditorHash, FocusType.Passive);
						bool l_IsEventUsedBeforeHandle = (Event.current.type == EventType.used);
						
						float l_HandleSize = HandleUtility.GetHandleSize (l_SelectedWorldPosition);
						Quaternion l_Rotation = Quaternion.Euler (0.0f, 0.0f, - l_Particle.rotation);
						l_Rotation = Camera.current.transform.rotation * l_Rotation;
						Vector3 l_Scale;
						if (l_Particle.isSquared || !m_Cloud.CloudRenderer.SupportsNonSquaredParticles) {
							l_Scale = new Vector3 (l_Particle.size.x, l_Particle.size.x, 0.0f);
						} else {
							l_Scale = new Vector3 (l_Particle.size.x, l_Particle.size.y, 0.0f);
						}
						l_Scale = Handles.ScaleHandle (l_Scale, l_SelectedWorldPosition, l_Rotation, l_HandleSize);
						
						if (l_Particle.isSquared || !m_Cloud.CloudRenderer.SupportsNonSquaredParticles) {
							if (l_Scale.y != l_Particle.size.y) {
								
									// Changes made to y are going to be applied to x.
								l_Particle.size.x = l_Scale.y;
								l_Particle.size.y = l_Scale.y;
							} else {
								l_Particle.size.x = l_Scale.x;
								l_Particle.size.y = l_Scale.x;
							}
						} else {
							l_Particle.size.x = l_Scale.x;
							l_Particle.size.y = l_Scale.y;
						}
						
						if (l_Particle.size.x < 0.0f) {
							l_Particle.size.x = 0.0f;
						}
						if (l_Particle.size.y < 0.0f) {
							l_Particle.size.y = 0.0f;
						}
						
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
							m_Cloud.ParticleData [SelectedParticleIndex] = l_Particle;
						}
						
						if
							((l_ControlIDBeforeHandle < HandleUtility.nearestControl &&
							  HandleUtility.nearestControl < l_ControlIDAfterHandle) ||
							  l_IsEventUsedByHandle)
						{
							l_HandleIsUsed = true;
						}
					}
//				}
				
					// Update inspector gui
				m_CloudEditor.Repaint ();
			}
			
			
				// If the handle for the particle is not used, check if there is another particle that needs
				// to be selected.
			
			// HACK: Workaround for a Unity bug that may freeze the scene view.
//			if (HandlesSupport.IsHandleDrawingSave ()) {
				if (!l_HandleIsUsed) {
					for (int i = 0; i < m_Cloud.ParticleData.Count; i = i + 1) {
						if (i != SelectedParticleIndex) {
							Vector3 l_Position = l_LocalToWorldMatrix.MultiplyPoint3x4 (m_Cloud.ParticleData [i].position);
							float l_HandleSize = HandleUtility.GetHandleSize (l_Position) * CloudSystemPrefs.c_SelectableCircleRadius;
							if (Handles.Button (l_Position, Camera.current.transform.rotation, 0.0f, l_HandleSize, Handles.RectangleCap)) {
								SelectedParticleIndex = i;
								m_CloudEditor.Repaint ();
							}
						}
					}
				}
//			}
		}
		
		private void ProcessParticleKeyEvents () {		
			if (Event.current.type == EventType.keyDown) {
				if (Event.current.keyCode == KeyCode.Delete) {
					DeleteSelectedParticle ();
					Event.current.Use ();
					
						// HACK:
						// For any reason Ctrl+D does not work, as it copies the whole cloud system. So we use
						// Shift+D.
				} else if (Event.current.modifiers == EventModifiers.Shift && Event.current.keyCode == KeyCode.D) {
					DuplicateSelectedParticle ();
					Event.current.Use ();
				}
			}
		}
		
		private void AddParticle () {
			Undo.RegisterUndo (m_Cloud.ParticleData, "Add particle");
			CloudParticle l_Particle = new CloudParticle ();
			l_Particle.InitializeValues ();
			
			m_Cloud.ParticleData.Add (l_Particle);
			
			SelectedParticleIndex = m_Cloud.ParticleData.Count - 1;
			
			m_ReloadSerializedParticleData = true;
			m_CloudEditor.Repaint ();
		}
		
		public void DuplicateSelectedParticle () {
			if (IsSelectedParticleIndexValid) {
				Undo.RegisterUndo (m_Cloud.ParticleData, "Duplicate particle");
				
					// That's the way to copy structs.
				CloudParticle l_NewParticle = m_Cloud.ParticleData [SelectedParticleIndex];
				m_Cloud.ParticleData.Add (l_NewParticle);
				
				m_ReloadSerializedParticleData = true;
				m_CloudEditor.Repaint ();
			}
		}
		
		public void DeleteSelectedParticle () {
			if (IsSelectedParticleIndexValid) {
				Undo.RegisterUndo (m_Cloud.ParticleData, "Delete particle");
				m_Cloud.ParticleData.RemoveAt (SelectedParticleIndex);
				
					// With this we have still a selected particle, even if we
					// delete the last one from the array.
				if (!IsSelectedParticleIndexValid) {
					SelectedParticleIndex = SelectedParticleIndex - 1;
				}
				
				m_ReloadSerializedParticleData = true;
				m_CloudEditor.Repaint ();
			}
		}
	}
}