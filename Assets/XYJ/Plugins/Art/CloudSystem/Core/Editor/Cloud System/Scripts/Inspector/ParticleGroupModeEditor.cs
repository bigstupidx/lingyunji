//
// Edelweiss.CloudSystemEditor.ParticleGroupModeEditor.cs: Inspector and scene view for the particle group mode.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {

	public class ParticleGroupModeEditor <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		private C m_Cloud;
		private CloudEditor <C, PD, CD> m_CloudEditor;
		
		private SerializedObject m_SerializedCloudData;
		private bool m_ReloadSerializedCreatorData;
		private SerializedProperty m_DensityProperty;
		private List <ParticleGroupProperties> m_ParticleGroupProperties = new List <ParticleGroupProperties> ();
		private CS_ParticleGroup m_ParticleGroupToDelete;
		private CS_ParticleType m_ParticleTypeToDelete;
	
		private GUIContent m_GroupNameLabel;
		private GUIContent m_ShapeColorLabel;
		private GUIContent m_UVIndexLabel;
		private GUIContent m_CreationProbabilityLabel;
		private GUIContent m_RotationLabel;
		private GUIContent m_SquaredLabel;
		private GUIContent m_SizeLabel;
		private GUIContent m_XSizeLabel;
		private GUIContent m_YSizeLabel;
		private GUIContent m_AddTypeButton;
		private GUIContent m_DeleteTypeButton;
		private GUIContent m_AddGroupButton;
		private GUIContent m_DeleteGroupButton;
		
		public void Initialize (CloudEditor <C, PD, CD> a_CloudSystemEditor) {
			m_CloudEditor = a_CloudSystemEditor;
			m_Cloud = m_CloudEditor.Cloud;
			
			m_GroupNameLabel = new GUIContent ("Group Name", "Name of the particle group.");
			m_ShapeColorLabel = new GUIContent ("Shape Color", "Color used in scene view to represent the particle group.");
			m_UVIndexLabel = new GUIContent ("UV Index", "UV index of this particle type.");
			m_CreationProbabilityLabel = new GUIContent ("Probability", "Creation probability for this particle type in the particle group.");
			m_RotationLabel = new GUIContent ("Rotation", "Allowed rotation range for this particle type");
			m_SquaredLabel = new GUIContent ("Squared", "Should the particles for this type be squared?");
			m_SizeLabel = new GUIContent ("Size", "Size for particles of this type.");
			m_XSizeLabel = new GUIContent ("X Size", "X size for particles of this type.");
			m_YSizeLabel = new GUIContent ("Y Size", "Y size for particles of this type.");
			m_AddTypeButton = new GUIContent ("Add Type", "Add a new particle type to the particle group.");
			m_DeleteTypeButton = new GUIContent ("Delete Type", "Delete this particle type from the particle group.");
			m_AddGroupButton = new GUIContent ("Add Group", "Add a new particle group.");
			m_DeleteGroupButton = new GUIContent ("Delete Group", "Remove this particle group.");
			
			InitializeSerializedObject ();
		}
		
		private void InitializeSerializedObject () {
			bool l_ExistedBefore = (m_SerializedCloudData != null);
		
			m_SerializedCloudData = new SerializedObject (m_Cloud.CreatorData);
			m_DensityProperty = m_SerializedCloudData.FindProperty ("density");
			
			m_ParticleGroupProperties.Clear ();
			for (int i = 0; i < m_Cloud.CreatorData.particleGroups.Length; i = i + 1) {
				ParticleGroupProperties l_ParticleGroupProperties = new ParticleGroupProperties ();
				l_ParticleGroupProperties.Initialize <C, PD, CD> (m_SerializedCloudData, m_Cloud, i);
				m_ParticleGroupProperties.Add (l_ParticleGroupProperties);
			}
			
			if (l_ExistedBefore) {
				m_SerializedCloudData.Update ();
				EditorUtility.SetDirty (m_Cloud.CreatorData);
			}
		}
		
		public void InspectorGUI () {
			
				// Support Undo/Redo if a group or type is added or deleted.
			if (m_ParticleGroupProperties.Count == m_Cloud.CreatorData.particleGroups.Length) {
				for (int i = 0; i < m_ParticleGroupProperties.Count; i = i + 1) {
					if (m_ParticleGroupProperties [i].particleTypesProperties.Count != m_Cloud.CreatorData.particleGroups [i].particleTypes.Length) {
						InitializeSerializedObject ();
						break;
					}
				}
			} else {
				InitializeSerializedObject ();
			}
			
			m_ReloadSerializedCreatorData = false;
			m_SerializedCloudData.Update ();
			
			EditorGUILayout.Space ();
			InspectorSupport.Explanation ("Particle Groups", "Modify, add or remove particle groups that are used by shapes to define which kind of particles should be created by the creator. The particle groups further consist of types.", null);
			
			EditorGUILayout.Space ();
			CloudCreatorEditorSupport <C, PD, CD>.CloudCreatorInspectorGUI (m_CloudEditor, m_DensityProperty);
			
			EditorGUILayout.Space ();
			ParticleGroupsInspectorGUI ();
			
			if (m_ReloadSerializedCreatorData) {
				InitializeSerializedObject ();
			}
			
			m_SerializedCloudData.ApplyModifiedProperties ();
			if (GUI.changed) {
				EditorUtility.SetDirty (m_Cloud.CreatorData);
			}
		}
	
		
		private void ParticleGroupsInspectorGUI () {
					
			GUILayout.Label ("Particle Groups", EditorStyles.boldLabel);
			
			m_ParticleGroupToDelete = null;
			
			for (int i = 0; i < m_ParticleGroupProperties.Count; i = i + 1) {
				CS_ParticleGroup l_ParticleGroup = m_Cloud.CreatorData.particleGroups [i];
				ParticleGroupProperties l_ParticleGroupProperties = m_ParticleGroupProperties [i];
				
				ParticleGroupInspectorGUI (l_ParticleGroup, l_ParticleGroupProperties);
				EditorGUILayout.Space ();
			}
			if (m_ParticleGroupToDelete != null) {
				DeleteParticleGroup ();
			}
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button (m_AddGroupButton)) {
				Undo.RegisterUndo (m_Cloud, "Add particle group");
				CS_ParticleGroup l_ParticleGroup = new CS_ParticleGroup ();
				ArraySupport.Add (ref m_Cloud.CreatorData.particleGroups, l_ParticleGroup);
				CS_ParticleType l_ParticleType = new CS_ParticleType ();
				ArraySupport.Add (ref l_ParticleGroup.particleTypes, l_ParticleType);
				m_ReloadSerializedCreatorData = true;
			}
			EditorGUILayout.EndHorizontal ();
		}
		
		private void ParticleGroupInspectorGUI (CS_ParticleGroup a_ParticleGroup, ParticleGroupProperties a_ParticleGroupProperties) {
			
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			EditorGUILayout.Space ();
		
			a_ParticleGroupProperties.nameProperty.stringValue = EditorGUILayout.TextField (m_GroupNameLabel, a_ParticleGroupProperties.nameProperty.stringValue);
			a_ParticleGroupProperties.shapeColorProperty.colorValue = EditorGUILayout.ColorField (m_ShapeColorLabel, a_ParticleGroupProperties.shapeColorProperty.colorValue);
			
			EditorGUILayout.Space ();
			GUILayout.Label ("Particle Types", EditorStyles.boldLabel);
			
			EditorGUILayout.Space ();
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			
			m_ParticleTypeToDelete = null;
			int l_ChangedProbabilityIndex = -1;
			for (int i = 0; i < a_ParticleGroupProperties.particleTypesProperties.Count; i = i + 1) {
				CS_ParticleType l_ParticleType = a_ParticleGroup.particleTypes [i];
				ParticleTypeProperties l_ParticleTypeProperties = a_ParticleGroupProperties.particleTypesProperties [i];
				
				float l_OldCreationProbatility = l_ParticleTypeProperties.creationProbabilityProperty.floatValue;
				ParticleTypeInspectorGUI (a_ParticleGroup, l_ParticleType, l_ParticleTypeProperties);
				if (l_OldCreationProbatility != l_ParticleTypeProperties.creationProbabilityProperty.floatValue) {
					l_ChangedProbabilityIndex = i;
				}
			}
			
			if (l_ChangedProbabilityIndex != -1) {
				ChangedProbabilityOfParticleType (a_ParticleGroupProperties, l_ChangedProbabilityIndex);
			}
			
			if (m_ParticleTypeToDelete != null) {
				Undo.RegisterUndo (m_Cloud.CreatorData, "Remove particle type");
				ArraySupport.Remove <CS_ParticleType> (ref a_ParticleGroup.particleTypes, m_ParticleTypeToDelete);
				
					// Add the probability from the type that is beeing deleted to the remaining types.
				float l_Probability = m_ParticleTypeToDelete.creationProbability / a_ParticleGroup.particleTypes.Length;
				foreach (CS_ParticleType l_ParticleType in a_ParticleGroup.particleTypes) {
					l_ParticleType.creationProbability = l_ParticleType.creationProbability + l_Probability;
				}
				
				m_ReloadSerializedCreatorData = true;
				m_ParticleTypeToDelete = null;
			}
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button (m_AddTypeButton)) {
				Undo.RegisterUndo (m_Cloud.CreatorData, "Add particle type");
				CS_ParticleType l_LastParticleType = a_ParticleGroup.particleTypes [a_ParticleGroup.particleTypes.Length - 1];
				CS_ParticleType l_ParticleType = new CS_ParticleType (l_LastParticleType);
				ArraySupport.Add (ref a_ParticleGroup.particleTypes, l_ParticleType);
				
				if (a_ParticleGroup.particleTypes.Length > 1) {
						// Probability for new type.
					float l_Probability = 1.0f / a_ParticleGroup.particleTypes.Length;
					l_ParticleType.creationProbability = l_Probability;
					
						// Subtract the needed parts from other types. 
					l_Probability = l_Probability / (a_ParticleGroup.particleTypes.Length - 1);
					for (int i = 0; i < a_ParticleGroup.particleTypes.Length - 1; i = i + 1) {
						CS_ParticleType l_OtherParticleType = a_ParticleGroup.particleTypes [i];
						l_OtherParticleType.creationProbability = l_OtherParticleType.creationProbability - l_Probability;
					}
				}
				
				m_ReloadSerializedCreatorData = true;
			}
			EditorGUILayout.EndHorizontal ();
			
			EditorGUILayout.Space ();
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			
			if (m_Cloud.CreatorData.particleGroups.Length > 1) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button (m_DeleteGroupButton)) {
					m_ParticleGroupToDelete = a_ParticleGroup;
				}
				EditorGUILayout.EndHorizontal ();
			}
			
			EditorGUILayout.EndVertical ();
		}
		
			// TODO: Should be an Editor Setting?
		private float m_MaximumSizeValueForSlider = 20.0f;
		
		private void ParticleTypeInspectorGUI (CS_ParticleGroup a_ParticleGroup, CS_ParticleType a_ParticleType, ParticleTypeProperties a_ParticleTypeProperties) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.BeginVertical ();
			
			a_ParticleTypeProperties.uvIndexProperty.intValue = EditorGUILayout.IntSlider (m_UVIndexLabel, a_ParticleTypeProperties.uvIndexProperty.intValue, 1, m_Cloud.TileCount);
			EditorGUILayout.BeginHorizontal ();
			a_ParticleTypeProperties.creationProbabilityProperty.floatValue = EditorGUILayout.Slider (m_CreationProbabilityLabel, a_ParticleTypeProperties.creationProbabilityProperty.floatValue, 0.0f, 1.0f);
			EditorGUILayout.EndHorizontal ();
			
			EditorGUILayout.BeginHorizontal ();
			float l_MinimumRotation = a_ParticleTypeProperties.minimumRotationProperty.floatValue;
			float l_MaximumRotation = a_ParticleTypeProperties.maximumRotationProperty.floatValue;
			EditorGUILayout.MinMaxSlider (new GUIContent (m_RotationLabel), ref l_MinimumRotation, ref l_MaximumRotation, -180.0f, 180.0f);
			a_ParticleTypeProperties.minimumRotationProperty.floatValue = l_MinimumRotation;
			a_ParticleTypeProperties.maximumRotationProperty.floatValue = l_MaximumRotation;
			
			a_ParticleTypeProperties.minimumRotationProperty.floatValue = EditorGUILayout.FloatField (a_ParticleTypeProperties.minimumRotationProperty.floatValue);
			a_ParticleTypeProperties.maximumRotationProperty.floatValue = EditorGUILayout.FloatField (a_ParticleTypeProperties.maximumRotationProperty.floatValue);
			EditorGUILayout.EndHorizontal ();
			
			a_ParticleTypeProperties.squaredProperty.boolValue = EditorGUILayout.Toggle (m_SquaredLabel, a_ParticleTypeProperties.squaredProperty.boolValue);
			
			EditorGUILayout.BeginHorizontal ();
			float l_MinimumXSize = a_ParticleTypeProperties.minimumXSizeProperty.floatValue;
			float l_MaximumXSize = a_ParticleTypeProperties.maximumXSizeProperty.floatValue;
			GUIContent l_XSizeLabel;
			if (a_ParticleTypeProperties.squaredProperty.boolValue) {
				l_XSizeLabel = m_SizeLabel;
			} else {
				l_XSizeLabel = m_XSizeLabel;
			}
			EditorGUILayout.MinMaxSlider (l_XSizeLabel, ref l_MinimumXSize, ref l_MaximumXSize, 0.0f, m_MaximumSizeValueForSlider);
			a_ParticleTypeProperties.minimumXSizeProperty.floatValue = l_MinimumXSize;
			a_ParticleTypeProperties.maximumXSizeProperty.floatValue = l_MaximumXSize;
			a_ParticleTypeProperties.minimumXSizeProperty.floatValue = EditorGUILayout.FloatField (a_ParticleTypeProperties.minimumXSizeProperty.floatValue);
			a_ParticleTypeProperties.maximumXSizeProperty.floatValue = EditorGUILayout.FloatField (a_ParticleTypeProperties.maximumXSizeProperty.floatValue);
			EditorGUILayout.EndHorizontal ();
			if (a_ParticleTypeProperties.minimumXSizeProperty.floatValue < 0.0f) {
				a_ParticleTypeProperties.minimumXSizeProperty.floatValue = 0.0f;
			}
			if (a_ParticleTypeProperties.minimumXSizeProperty.floatValue > a_ParticleTypeProperties.maximumXSizeProperty.floatValue) {
				a_ParticleTypeProperties.maximumXSizeProperty.floatValue = a_ParticleTypeProperties.minimumXSizeProperty.floatValue;
			}
			
			if (!a_ParticleTypeProperties.squaredProperty.boolValue) {
				EditorGUILayout.BeginHorizontal ();
				float l_MinimumYSize = a_ParticleTypeProperties.minimumYSizeProperty.floatValue;
				float l_MaximumYSize = a_ParticleTypeProperties.maximumYSizeProperty.floatValue;
				EditorGUILayout.MinMaxSlider (m_YSizeLabel, ref l_MinimumYSize, ref l_MaximumYSize, 0.0f, m_MaximumSizeValueForSlider);
				a_ParticleTypeProperties.minimumYSizeProperty.floatValue = l_MinimumYSize;
				a_ParticleTypeProperties.maximumYSizeProperty.floatValue = l_MaximumYSize;
				a_ParticleTypeProperties.minimumYSizeProperty.floatValue = EditorGUILayout.FloatField (a_ParticleTypeProperties.minimumYSizeProperty.floatValue);
				a_ParticleTypeProperties.maximumYSizeProperty.floatValue = EditorGUILayout.FloatField (a_ParticleTypeProperties.maximumYSizeProperty.floatValue);
				EditorGUILayout.EndHorizontal ();
				if (a_ParticleTypeProperties.minimumYSizeProperty.floatValue < 0.0f) {
					a_ParticleTypeProperties.minimumYSizeProperty.floatValue = 0.0f;
				}
				if (a_ParticleTypeProperties.minimumYSizeProperty.floatValue > a_ParticleTypeProperties.maximumYSizeProperty.floatValue) {
					a_ParticleTypeProperties.maximumYSizeProperty.floatValue = a_ParticleTypeProperties.minimumYSizeProperty.floatValue;
				}	
			}
			
			if (a_ParticleGroup.particleTypes.Length > 1) {
				EditorGUILayout.Space ();
				
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button (m_DeleteTypeButton)) {
					m_ParticleTypeToDelete = a_ParticleType;
				}
				EditorGUILayout.EndHorizontal ();
			}
			
			EditorGUILayout.Space ();
			
			EditorGUILayout.EndVertical ();
			
				// Preview texture
			if (m_Cloud.ParticleMaterial != null && m_Cloud.ParticleMaterial.mainTexture != null) {
				float l_TextureSize = CloudEditor <C, PD, CD>.c_PreviewTextureSize;
				GUILayout.Label ("",
				                 CloudSystemPreviewTextures.PreviewTextureBackgroundStyle,
				                 GUILayout.MinWidth (l_TextureSize),
				                 GUILayout.MinHeight (l_TextureSize),
				                 GUILayout.MaxWidth (l_TextureSize),
				                 GUILayout.MaxHeight (l_TextureSize));
				
				Rect l_AreaRect = GUILayoutUtility.GetLastRect ();
				int l_TotalU = m_Cloud.XTile;
				int l_TotalV = m_Cloud.YTile;
				int l_CurrentU = (a_ParticleType.uvIndex - 1) / l_TotalU;
				int l_CurrentV = (a_ParticleType.uvIndex - 1) % l_TotalU;
				float l_UFactor = 1.0f / l_TotalU;
				float l_VFactor = 1.0f / l_TotalV;
			
				Rect l_TextureRect = new Rect (l_CurrentV * l_UFactor, (l_TotalV - l_CurrentU - 1) * l_VFactor, l_UFactor, l_VFactor);
				GUI.DrawTextureWithTexCoords (l_AreaRect, m_Cloud.GetComponent<Renderer>().sharedMaterial.mainTexture, l_TextureRect);
			}

			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
		}
		
		private void DeleteParticleGroup () {
			
				// Only delete if there is no shape that uses that particle type group
			int l_ParticleGroupIndex = System.Array.IndexOf (m_Cloud.CreatorData.particleGroups, m_ParticleGroupToDelete);
			bool l_IsParticleGroupUsed = false;
			foreach (CS_Shape l_BoxShape in m_Cloud.CreatorData.boxShapes) {
				if (l_BoxShape.particleGroupIndex == l_ParticleGroupIndex) {
					l_IsParticleGroupUsed = true;
					break;
				}
			}
			
			if (!l_IsParticleGroupUsed) {
				Undo.RegisterUndo (m_Cloud.CreatorData, "Remove particle group");
				
				foreach (CS_Shape l_BoxShape in m_Cloud.CreatorData.boxShapes) {
					if (l_BoxShape.particleGroupIndex > l_ParticleGroupIndex) {
						l_BoxShape.particleGroupIndex = l_BoxShape.particleGroupIndex - 1;
					}
				}
				
				ArraySupport.RemoveAt (ref m_Cloud.CreatorData.particleGroups, l_ParticleGroupIndex);
				m_ReloadSerializedCreatorData = true;
			} else {
				
				EditorUtility.DisplayDialog ("Warning: Particle group is used", "You can not delete this particle group because there is a least one shape using it. If all shapes use other particle groups, it will be possible to delete this one.", "Ok");
			}
			
			m_ParticleGroupToDelete = null;
		}
		
		private void ChangedProbabilityOfParticleType (ParticleGroupProperties a_ParticleGroupProperties, int a_ParticleTypeIndex) {
			if (a_ParticleGroupProperties.particleTypesProperties.Count == 1) {
				a_ParticleGroupProperties.particleTypesProperties [0].creationProbabilityProperty.floatValue = 1.0f;
			} else {
				float l_RemainingPercentage = 1.0f - a_ParticleGroupProperties.particleTypesProperties [a_ParticleTypeIndex].creationProbabilityProperty.floatValue;
				
				if (l_RemainingPercentage > 0.0f) {
					float l_SumOfRest = 0.0f;
					for (int i = 0; i < a_ParticleGroupProperties.particleTypesProperties.Count; i = i + 1) {
						if (i != a_ParticleTypeIndex) {
							l_SumOfRest = l_SumOfRest + a_ParticleGroupProperties.particleTypesProperties [i].creationProbabilityProperty.floatValue;
						}
					}
					if (l_SumOfRest > 0.0f) {
						for (int i = 0; i < a_ParticleGroupProperties.particleTypesProperties.Count; i = i + 1) {
							if (i != a_ParticleTypeIndex) {
								a_ParticleGroupProperties.particleTypesProperties [i].creationProbabilityProperty.floatValue =
									l_RemainingPercentage * a_ParticleGroupProperties.particleTypesProperties [i].creationProbabilityProperty.floatValue / l_SumOfRest;
							}
						}
					}
				}
			}
		}
	
		public void SceneGUI () {
		}
	}
}
