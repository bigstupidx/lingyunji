//
// Edelweiss.CloudSystemEditor.CloudModeEditor.cs: Inspector and scene view for the cloud mode.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {

	public class CloudModeEditor <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{	
		private C m_Cloud;
		private CloudEditor <C, PD, CD> m_CloudEditor;
		
		private SerializedObject m_SerializedCloud;
		private SerializedProperty m_ParticleMaterialProperty;
		private SerializedProperty m_SunProperty;
		private SerializedProperty m_XTileProperty;
		private SerializedProperty m_YTileProperty;
		private SerializedProperty m_TintProperty;
		private SerializedProperty m_VerticalShadingInfluence;
		private SerializedProperty m_ShadingGroupInfluence;
		private SerializedProperty m_ScaleProperty;
		private SerializedProperty m_FadingProperty;
		
		private GUIContent m_RendererLabel;
		private GUIContent m_RenderingMethodLabel;
		private GUIContent m_MaterialLabel;
		private GUIContent m_SunLabel;
		private GUIContent m_VerticalShadingInfluenceLabel;
		private GUIContent m_ShadingGroupInfluenceLabel;
		private GUIContent m_TintLabel;
//		private GUIContent m_UseParticleColorLabel;
		private GUIContent m_XTileLabel;
		private GUIContent m_YTileLabel;
		private GUIContent m_ScaleLabel;
		private GUIContent m_FadingLabel;
		
		public void Initialize (CloudEditor <C, PD, CD> a_CloudSystemEditor) {
			m_CloudEditor = a_CloudSystemEditor;
			m_Cloud = m_CloudEditor.Cloud;
			
			m_RendererLabel = new GUIContent ("Renderer", "Renderer for the cloud.");
			m_RenderingMethodLabel = new GUIContent ("Rendering Method", "Method used to compute the particle colors.");
			m_MaterialLabel = new GUIContent ("Material", "Material for the cloud.");
			m_SunLabel = new GUIContent ("Sun", "The sun which affects the shading groups.");
			m_VerticalShadingInfluenceLabel = new GUIContent ("Vertical Shading", "Vertical shading influence in the rendering.");
			m_ShadingGroupInfluenceLabel = new GUIContent ("Shading Group", "Shading group influence in the rendering.");
			m_TintLabel = new GUIContent ("Tint", "Tint color for the cloud.");
//			m_UseParticleColorLabel = new GUIContent ("Use Particle Color", "Should the particle color be used in the shading?");
			m_XTileLabel = new GUIContent ("X Tile", "Number of vertical tiles.");
			m_YTileLabel = new GUIContent ("Y Tile", "Number of horizontal tiles");
			m_ScaleLabel = new GUIContent ("Scale", "Size of the cloud.");
			m_FadingLabel = new GUIContent ("Fade", "Transparency for the cloud.");
			
			InitializeSerializedObject ();
		}
		
		public void InitializeSerializedObject () {
			bool l_ExistedBefore = (m_SerializedCloud != null);
		
			m_SerializedCloud = new SerializedObject (m_Cloud);
			
			m_ParticleMaterialProperty = m_SerializedCloud.FindProperty ("m_ParticleMaterial");
			m_SunProperty = m_SerializedCloud.FindProperty ("m_Sun");
			m_XTileProperty = m_SerializedCloud.FindProperty ("m_XTile");
			m_YTileProperty = m_SerializedCloud.FindProperty ("m_YTile");
			m_TintProperty = m_SerializedCloud.FindProperty ("m_Tint");
			m_VerticalShadingInfluence = m_SerializedCloud.FindProperty ("m_VerticalShadingInfluence");
			m_ShadingGroupInfluence = m_SerializedCloud.FindProperty ("m_ShadingGroupInfluence");
			m_ScaleProperty = m_SerializedCloud.FindProperty ("m_Scale");
			m_FadingProperty = m_SerializedCloud.FindProperty ("m_Fading");
			
			if (l_ExistedBefore) {
				m_SerializedCloud.Update ();
				EditorUtility.SetDirty (m_Cloud);
			}
		}
		
		public void InspectorGUI () {
	
			EditorGUILayout.Space ();
			InspectorSupport.Explanation ("Cloud",
			                              "Set the high level properties for the cloud.", null);
			
			m_SerializedCloud.Update ();
			
			EditorGUILayout.Space ();
			GUILayout.Label ("Shading", EditorStyles.boldLabel);
			
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			
			
			CloudRendererEnum l_OldCloudRenderer = m_CloudEditor.CloudRendererTypeLookup.CloudRendererFromType (m_Cloud.CloudRendererType);
			CloudRenderingMethodEnum l_OldCloudRenderingMethod = m_CloudEditor.CloudRendererTypeLookup.CloudRenderingMethodFromType (m_Cloud.CloudRendererType);
			
			CloudRendererEnum l_NewCloudRenderer = (CloudRendererEnum) EditorGUILayout.EnumPopup (m_RendererLabel, l_OldCloudRenderer);
			CloudRenderingMethodEnum l_NewCloudRenderingMethod = (CloudRenderingMethodEnum) EditorGUILayout.EnumPopup (m_RenderingMethodLabel, l_OldCloudRenderingMethod);
			
			if
				(l_OldCloudRenderer != l_NewCloudRenderer ||
				 l_OldCloudRenderingMethod != l_NewCloudRenderingMethod)
			{
				CloudRendererTypeEnum l_CloudRendererType = m_CloudEditor.CloudRendererTypeLookup.CloudRendererTypeFromRendererAndMethod (l_NewCloudRenderer, l_NewCloudRenderingMethod);
				CloudPrefab.ChangeCloudRenderer <C, PD, CD> (m_Cloud, l_CloudRendererType, m_CloudEditor.CloudRendererTypeLookup);
				
				if (l_NewCloudRenderer == CloudRendererEnum.Shuriken) {
					m_CloudEditor.ShurikenHack ();
				}
			}
			
			EditorGUILayout.Space ();
						
			Material l_OldMaterial = (Material) m_ParticleMaterialProperty.objectReferenceValue;
			EditorGUILayout.PropertyField (m_ParticleMaterialProperty, m_MaterialLabel);
			Material l_NewMaterial = (Material) m_ParticleMaterialProperty.objectReferenceValue;
			if (l_OldMaterial != l_NewMaterial) {
				m_Cloud.ParticleMaterial = l_NewMaterial;
			}
	
				// Sun
			if (!m_Cloud.CloudRenderer.SupportsShadingGroups) {
				GUI.enabled = false;
			}	
			object l_OldSun = m_SunProperty.objectReferenceValue;
			EditorGUILayout.PropertyField (m_SunProperty, m_SunLabel);
			object l_NewSun = m_SunProperty.objectReferenceValue;
			if (l_OldSun != l_NewSun) {
				
					// Very special case for prefabs that are duplicated.
				if (l_OldSun == null) {
					m_Cloud.InitializeRendererComponents ();
				}
			}
			if (!m_Cloud.CloudRenderer.SupportsShadingGroups) {
				GUI.enabled = true;
			}
			
			
				// Tint
			if (!m_Cloud.CloudRenderer.SupportsTint) {
				GUI.enabled = false;
			}
			EditorGUILayout.PropertyField (m_TintProperty, m_TintLabel);
			if (!m_Cloud.CloudRenderer.SupportsTint) {
				GUI.enabled = true;
			}
			
			
//				// Use particle color
//			bool l_NewUseParticleColor = EditorGUILayout.Toggle (m_UseParticleColorLabel, m_Cloud.CloudRenderer.useParticleColor);
//			if (l_NewUseParticleColor != m_Cloud.CloudRenderer.useParticleColor) {
//				Undo.RegisterUndo (m_Cloud.CloudRenderer, "Use Particle Color Changed");
//				m_Cloud.CloudRenderer.useParticleColor = l_NewUseParticleColor;
//			}
			
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			
			
				// Shading influence
			if
				(!(m_Cloud.CloudRenderer.SupportsShadingGroups &&
				   m_Cloud.CloudRenderer.SupportsVerticalColors))
			{
				GUI.enabled = false;
			}
				
			EditorGUILayout.Space ();
			GUILayout.Label ("Shading Influence", EditorStyles.boldLabel);
			
			bool l_VerticalShadingInfluenceChanged = false;
			bool l_ShadingGroupInfluenceChanged = false;
			
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			
			float l_OldVerticalShadingInfluence = m_VerticalShadingInfluence.floatValue;
			EditorGUILayout.Slider (m_VerticalShadingInfluence, 0.0f, 1.0f, m_VerticalShadingInfluenceLabel);
			if (l_OldVerticalShadingInfluence != m_VerticalShadingInfluence.floatValue) {
				if (m_VerticalShadingInfluence.floatValue < 0.0f) {
					m_VerticalShadingInfluence.floatValue = 0.0f;
				} else if (m_VerticalShadingInfluence.floatValue > 1.0f) {
					m_VerticalShadingInfluence.floatValue = 1.0f;
				}
				l_VerticalShadingInfluenceChanged = true;
			}
			
			float l_OldShadingGroupInfluence = m_ShadingGroupInfluence.floatValue;
			EditorGUILayout.Slider (m_ShadingGroupInfluence, 0.0f, 1.0f, m_ShadingGroupInfluenceLabel);
			if (l_OldShadingGroupInfluence != m_ShadingGroupInfluence.floatValue) {
				if (m_ShadingGroupInfluence.floatValue < 0.0f) {
					m_ShadingGroupInfluence.floatValue = 0.0f;
				} else if (m_ShadingGroupInfluence.floatValue > 1.0f) {
					m_ShadingGroupInfluence.floatValue = 1.0f;
				}
				l_ShadingGroupInfluenceChanged = true;
			}
			
			if (l_VerticalShadingInfluenceChanged) {
				m_ShadingGroupInfluence.floatValue = 1.0f - m_VerticalShadingInfluence.floatValue;
			} else if (l_ShadingGroupInfluenceChanged) {
				m_VerticalShadingInfluence.floatValue = 1.0f - m_ShadingGroupInfluence.floatValue;
			}
			
				// Check if one of them is reverted to the original prefab value. In that case the
				// other one needs to be reverted too.
			if (m_VerticalShadingInfluence.prefabOverride != m_ShadingGroupInfluence.prefabOverride) {
				m_VerticalShadingInfluence.prefabOverride = false;
				m_VerticalShadingInfluence.prefabOverride = false;
			}
			
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			
			if
				(!(m_Cloud.CloudRenderer.SupportsShadingGroups &&
				   m_Cloud.CloudRenderer.SupportsVerticalColors))
			{
				GUI.enabled = true;
			}
			
	
			EditorGUILayout.Space ();
			
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.BeginVertical ();
			
			GUILayout.Label ("Texture Tiling", EditorStyles.boldLabel);
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			
			int l_OldXTile = m_XTileProperty.intValue;
			EditorGUILayout.PropertyField (m_XTileProperty, m_XTileLabel);
			int l_NewXTile = m_XTileProperty.intValue;
			if (l_OldXTile != l_NewXTile) {
				bool l_IsNewTileValid = (l_NewXTile >= 1) &&  IsNewTileCountValid (l_NewXTile * m_Cloud.YTile);
				if (l_IsNewTileValid) {
					m_Cloud.XTile = l_NewXTile;
				} else {
					m_Cloud.XTile = l_OldXTile;
				}
				m_XTileProperty.intValue = m_Cloud.XTile;
				
				m_CloudEditor.ShurikenHack ();
			}
			
			int l_OldYTile = m_YTileProperty.intValue;
			EditorGUILayout.PropertyField (m_YTileProperty, m_YTileLabel);
			int l_NewYTile = m_YTileProperty.intValue;
			if (l_OldYTile != l_NewYTile) {
				bool l_IsNewTileValid = (l_NewYTile >= 1) && IsNewTileCountValid (m_Cloud.XTile * l_NewYTile);
				if (l_IsNewTileValid) {
					m_Cloud.YTile = l_NewYTile;
				} else {
					m_Cloud.YTile = l_OldYTile;
				}
				m_YTileProperty.intValue = m_Cloud.YTile;
				
				m_CloudEditor.ShurikenHack ();
			}
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			
			EditorGUILayout.EndVertical ();
			
			
				// Texture preview with split lines.
			if (m_Cloud.ParticleMaterial != null && m_Cloud.ParticleMaterial.mainTexture != null) {
				float l_TextureSize = CloudEditor <C, PD, CD>.c_PreviewTextureSize;
				GUILayout.Label ("",
				                 CloudSystemPreviewTextures.PreviewTextureBackgroundStyle,
				                 GUILayout.MinWidth (l_TextureSize),
				                 GUILayout.MinHeight (l_TextureSize),
				                 GUILayout.MaxWidth (l_TextureSize),
				                 GUILayout.MaxHeight (l_TextureSize));
				Rect l_AreaRect = GUILayoutUtility.GetLastRect();
				
				GUI.DrawTextureWithTexCoords (l_AreaRect, m_Cloud.GetComponent<Renderer>().sharedMaterial.mainTexture, new Rect (0.0f, 0.0f, 1.0f, 1.0f));

				GUI.BeginGroup (l_AreaRect);
				int l_XTiles = m_Cloud.XTile;
				int l_YTiles = m_Cloud.YTile;
				float l_XDelta = l_AreaRect.width / l_XTiles;
				float l_YDelta = l_AreaRect.height / l_YTiles;
				for (int i = 0; i < l_XTiles; i = i + 1) {
					Rect l_LineRect = new Rect ((i + 1) * l_XDelta, 0.0f, 1.0f, l_AreaRect.height);
					GUI.Label (l_LineRect, "", CloudSystemPreviewTextures.PreviewTextureHighlightStyle);
				}
				for (int i = 0; i < l_YTiles; i = i + 1) {
					Rect l_LineRect = new Rect (0.0f, (i + 1) * l_YDelta, l_AreaRect.width, 1.0f);
					GUI.Label (l_LineRect, "", CloudSystemPreviewTextures.PreviewTextureHighlightStyle);
				}
				GUI.EndGroup ();
			}
			
			EditorGUILayout.EndHorizontal ();
					
			EditorGUILayout.Space ();
			EditorGUILayout.PropertyField (m_ScaleProperty, m_ScaleLabel);
			if (m_ScaleProperty.floatValue < 0.0f) {
				m_ScaleProperty.floatValue = 0.0f;
			}
		
			float l_OldFading = m_FadingProperty.floatValue;
			EditorGUILayout.Slider (m_FadingProperty, 0.0f, 1.0f, m_FadingLabel);
			if (m_FadingProperty.floatValue < 0.0f) {
				m_FadingProperty.floatValue = 0.0f;
			} else if (m_FadingProperty.floatValue > 1.0f) {
				m_FadingProperty.floatValue = 1.0f;
			}
			
			m_SerializedCloud.ApplyModifiedProperties ();
				
			if (GUI.changed) {
				EditorUtility.SetDirty (m_Cloud);
				
					// We only get the correct undo behaviour, if those things are made here.
				if (l_OldFading != m_FadingProperty.floatValue) {
					m_Cloud.Fading = m_FadingProperty.floatValue;
				}
			}
		}
		
		private bool IsNewTileCountValid (int a_NewTileCount) {
			bool l_Result = true;
			
			foreach (CS_ParticleGroup l_ParticleTypeGroup in m_Cloud.CreatorData.particleGroups) {
				foreach (CS_ParticleType l_ParticleType in l_ParticleTypeGroup.particleTypes) {
					if (l_ParticleType.uvIndex > a_NewTileCount) {
						l_Result = false;
					}
				}
			}
			
			for (int i = 0; i < m_Cloud.ParticleData.Count; i = i + 1) {
				CloudParticle l_Particle = m_Cloud.ParticleData [i];
				if (l_Particle.uvIndex > a_NewTileCount) {
					l_Result = false;
				}
			}
			
			return (l_Result);
		}
		
		public void SceneGUI () {
		}
	}
}
