//
// CloudEditor.cs: Entry point for all inspector and scene views used for clouds.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Edelweiss.CloudSystem;
using Edelweiss.CloudSystemEditor;

namespace Edelweiss.CloudSystemEditor {
	
	public abstract class CloudEditor <C, PD, CD> : Editor
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		public const int c_PreviewTextureSize = 100;
		
		private C m_Cloud;
		public C Cloud {
			get {
				return (m_Cloud);
			}
		}
		
		private int m_CloudSystemEditorHash;
		public int CloudSystemEditorHash {
			get {
				return (m_CloudSystemEditorHash);
			}
		}
		
		
		protected CloudRendererTypeLookup m_CloudRendererTypeLookup;
		public abstract CloudRendererTypeLookup CloudRendererTypeLookup {
			get;
		}
		
		
		private CloudModeEditor <C, PD, CD> m_CloudModeEditor = new CloudModeEditor <C, PD, CD> ();
		private ShapeModeEditor <C, PD, CD> m_ShapeModeEditor = new ShapeModeEditor <C, PD, CD> ();
		private ParticleGroupModeEditor <C, PD, CD> m_ParticleTypeModeEditor = new ParticleGroupModeEditor <C, PD, CD> ();
		private ShadingModeEditor <C, PD, CD> m_ShadingModeEditor = new ShadingModeEditor <C, PD, CD> ();
		private VerticalColorModeEditor <C, PD, CD> m_VerticalColorModeEditor = new VerticalColorModeEditor <C, PD, CD> ();
		private ParticleModeEditor <C, PD, CD> m_ParticleModeEditor = new ParticleModeEditor <C, PD, CD> ();
		private SettingModeEditor m_SettingsModeEditor = new SettingModeEditor ();
	
		
		private GUIContent m_CloudButton;
		private GUIContent m_ShapeButton;
		private GUIContent m_ParticleTypeButton;
		private GUIContent m_ShadingButton;
		private GUIContent m_VerticalColorButton;
		private GUIContent m_ParticleButton;
		private GUIContent m_SettingButton;
		private GUIContent[] m_CloudToolbar;
		
		private void OnEnable () {
			if (!Application.isPlaying) {
				m_Cloud = target as C;
				m_CloudSystemEditorHash = GetHashCode ();
				
				ShurikenHack ();
				
				if (m_Cloud.CloudRenderer != null) {
					m_Cloud.CloudRenderer.HideAuxiliaryComponents ();
				}
				
					// Initialize all editors
				if (PrefabUtility.GetPrefabType (m_Cloud) != PrefabType.Prefab &&
				    m_Cloud.ParticleData != null &&
				    m_Cloud.CreatorData != null)
				{
					m_CloudModeEditor.Initialize (this);
					m_ShapeModeEditor.Initialize (this);
					m_ParticleTypeModeEditor.Initialize (this);
					m_ShadingModeEditor.Initialize (this);
					m_VerticalColorModeEditor.Initialize (this);
					m_ParticleModeEditor.Initialize (this);
					m_SettingsModeEditor.Initialize ();
					
					EditorApplication.modifierKeysChanged = EditorApplication.modifierKeysChanged + m_ShapeModeEditor.ModifierKeyChanged;
				}
				
				m_CloudButton = new GUIContent (CloudSystemIcons.CloudIcon, "Cloud Mode");
				m_ShapeButton = new GUIContent (CloudSystemIcons.ShapeIcon, "Shape Mode");
				m_ParticleTypeButton = new GUIContent (CloudSystemIcons.ParticleTypeIcon, "Particle Group Mode");
				m_ShadingButton = new GUIContent (CloudSystemIcons.ShadingIcon, "Shading Mode");
				m_VerticalColorButton = new GUIContent (CloudSystemIcons.VerticalColorIcon, "Vertical Color Mode");
				m_ParticleButton = new GUIContent (CloudSystemIcons.ParticleIcon, "Particle Mode");
				m_SettingButton = new GUIContent (CloudSystemIcons.SettingIcon, "Setting Mode");
				m_CloudToolbar = new GUIContent[] {m_CloudButton, m_ShapeButton, m_ParticleTypeButton, m_ShadingButton, m_VerticalColorButton, m_ParticleButton, m_SettingButton};
			}
		}
		
		private void OnDisable () {
			if (!Application.isPlaying) {
				ToolsSupport.Hidden = false;
				EditorApplication.modifierKeysChanged = EditorApplication.modifierKeysChanged - m_ShapeModeEditor.ModifierKeyChanged;
			}
		}
		
		
		public override void OnInspectorGUI () {
			if
				(!Application.isPlaying &&
			    m_Cloud.ParticleData == null &&
				m_Cloud.CreatorData == null)
			{
				EditorGUILayout.Space ();
				InspectorSupport.Error ("Error: No Data", "Unable to find necessary data! This happens if the cloud prefab which contains the particle and creator data is deleted.");
				
			} else if
				(!Application.isPlaying &&
			    m_Cloud.ParticleData != null &&
				 m_Cloud.CreatorData != null &&
				m_Cloud.CloudRenderer != null &&
			    PrefabUtility.GetPrefabType (m_Cloud) != PrefabType.Prefab)
			{
				if (m_Cloud.enabled) {
		 
						// Toolbar
					
					EditorGUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace ();
					CloudSystemPrefs.CloudToolbarIndex = GUILayout.Toolbar (CloudSystemPrefs.CloudToolbarIndex, m_CloudToolbar);
					GUILayout.FlexibleSpace ();
					EditorGUILayout.EndHorizontal ();
					
					
					if (m_Cloud.shadingGroups.Length != ((C) PrefabUtility.GetPrefabParent (m_Cloud)).shadingGroups.Length) {
						InspectorSupport.Warning ("Warning: Invalid number of shading groups",
						                          "This cloud and the prefab it belongs to don't have the same number of shading groups. If this cloud has the correct amount of shading groups, you need to apply those changes to the prefab in order to avoid null reference exceptions.");
					}
					
					
						// HACK:
						// Workaround for Unity bug (Case 414572)
					if
						(
						 ((m_Cloud.CloudRenderer as UnityParticleSystemCloudRenderer <C, PD, CD>) != null &&
						  (m_Cloud.CachedTransform.localScale != m_Cloud.ParticleSystemScale ||
						  m_Cloud.CachedTransform.lossyScale != m_Cloud.ParticleSystemScale))
						 
						 ||
						 
						 ((m_Cloud.CloudRenderer as UnityParticleSystemCloudRenderer <C, PD, CD>) == null &&
						  m_Cloud.CachedTransform.localScale != m_Cloud.MeshScale)
						)
					
//					if
//						(m_Cloud.Transform.localScale != Vector3.one ||
//						 m_Cloud.Transform.lossyScale != Vector3.one)
					{
						InspectorSupport.Warning ("Warning: Potential scaling issue",
						                          "Either the GameObject containing this cloud or at least one of its parent objects is scaled. This may produce unwanted artifacts with the custom renderers. For better results, you should use the clouds own scaling.\nRenderes that use Unity's particle system get and need a scale of 1.0001 as workaround for a Unity bug.");
					}
					
					if (m_Cloud.CloudRenderer.SupportsShadingGroups && m_Cloud.Sun == null) {
						EditorGUILayout.Space ();
						InspectorSupport.Error ("Error: Sun is not set!",
						                        "This cloud uses a rendering method which takes shading groups into account. Thus it needs a sun for the computations, but not sun is assigned!");
					}
					
					if (Edition.IsCloudSystemFree && !m_Cloud.CloudRenderer.IsSupportedInCloudSystemFree) {
						EditorGUILayout.Space ();
						InspectorSupport.Error ("Error: Unsupported cloud renderer!",
						                        "The selected cloud renderer is not supported in Cloud System Free.");
					}
					
					if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Cloud) {
						m_CloudModeEditor.InspectorGUI ();
					
					} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Shape) {
						m_ShapeModeEditor.InspectorGUI ();
						
					} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.ParticleType) {
						m_ParticleTypeModeEditor.InspectorGUI ();
						
					} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Shading) {
						m_ShadingModeEditor.InspectorGUI ();
						
					} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.VerticalColor) {
						m_VerticalColorModeEditor.InspectorGUI ();
						
					} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Particle) {
						m_ParticleModeEditor.InspectorGUI ();
						
					} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Setting) {
						m_SettingsModeEditor.InspectorGUI ();
					}
		
					
						// Statistics
					
					if (CloudSystemPrefs.ShowStatistics) {
						EditorGUILayout.Space ();
						
						EditorGUILayout.BeginHorizontal ();
						GUILayout.FlexibleSpace ();
						
						EditorGUILayout.BeginHorizontal (GUI.skin.box);
						GUILayout.Label ("Particles");
						EditorGUILayout.Space ();
						GUILayout.Label (Cloud.ParticleData.Count.ToString ());
						EditorGUILayout.EndHorizontal ();
						
						EditorGUILayout.EndHorizontal ();
					}
					
					EditorGUILayout.Space ();
					
					if (m_Cloud.GetComponent <CustomCloudRenderer <C, PD, CD>> () != null) {
						SceneView.RepaintAll ();
					}
				} else {
				
					InspectorSupport.Warning ("Warning: Script is not enabled", "It is not possible to work with the Cloud System Editor, if the cloud script is not enabled."); 
				}
			}
		}
		
		protected virtual void OnSceneGUI () {
			if
				(!Application.isPlaying &&
				 m_Cloud.ParticleData != null &&
				 m_Cloud.CreatorData != null)
			{
				
					// Update the particle data of the selected cloud.
				CloudRenderer <C, PD, CD> l_CloudRenderer = m_Cloud.GetComponent <CloudRenderer <C, PD, CD>> ();
				l_CloudRenderer.InitializeWithParticleData (m_Cloud.ParticleData);
				
				m_Cloud.RecalculateBoundingBox ();
				m_Cloud.RecalculateVerticalColorFactors ();
				m_Cloud.RecalculateShadingColorFactors ();
				m_Cloud.RecalculateScaledShadingGroupCenters ();
								
				if (!CloudSystemPrefs.AccelerateEditor) {
					
						// Update the particle data of all the clouds that are based on
						// the same prefab.
					Object l_PrefabParent = PrefabUtility.GetPrefabParent (m_Cloud);
					C[] l_OtherClouds = (C[]) FindObjectsOfType (typeof (C));
					foreach (C l_OtherCloud in l_OtherClouds) {
						Object l_OtherCloudPrefab = PrefabUtility.GetPrefabParent (l_OtherCloud);
						if (l_PrefabParent == l_OtherCloudPrefab) {
							CloudRenderer <C, PD, CD> l_OtherCloudRenderer = l_OtherCloud.GetComponent <CloudRenderer <C, PD, CD>> ();
							l_OtherCloudRenderer.InitializeWithParticleData (l_OtherCloud.ParticleData);
							
							l_OtherCloud.RecalculateBoundingBox ();
							l_OtherCloud.RecalculateVerticalColorFactors ();
							l_OtherCloud.RecalculateShadingColorFactors ();
							l_OtherCloud.RecalculateScaledShadingGroupCenters ();
						}
					}
				}
				
				
				
				if (m_Cloud.GetComponent<Renderer>() != null) {
					EditorUtility.SetSelectedWireframeHidden (m_Cloud.GetComponent<Renderer>(), true);
				}
				ToolsSupport.Hidden = false;
				
				if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Cloud) {
					m_CloudModeEditor.SceneGUI ();
				
				} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Shape) {
					ToolsSupport.Hidden = true;
					m_ShapeModeEditor.SceneGUI ();
					
				} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.ParticleType) {
					m_ParticleTypeModeEditor.SceneGUI ();
					
				} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Shading) {
					ToolsSupport.Hidden = true;
					m_ShadingModeEditor.SceneGUI ();
					
				} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.VerticalColor) {
					m_VerticalColorModeEditor.SceneGUI ();
					
				} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Particle) {
					ToolsSupport.Hidden = true;
					if (m_Cloud.GetComponent<Renderer>() != null) {
						EditorUtility.SetSelectedWireframeHidden (m_Cloud.GetComponent<Renderer>(), false);
					}
					m_ParticleModeEditor.SceneGUI ();
					
				} else if (CloudSystemPrefs.CloudToolbarIndex == (int) CloudToolbar.Setting) {
					m_SettingsModeEditor.SceneGUI ();
				}
			}
		}
		
		public void CreateCloud () {
			UnityEditor.Undo.RegisterUndo (m_Cloud.ParticleData, "Create particles");
			CloudCreator.CreateCloud <C, PD, CD> (m_Cloud);
		}	
		
		
		#region ShurikenHack
		
		public void ShurikenHack () {
			if
				(Cloud.CloudRendererType == CloudRendererTypeEnum.ShurikenTintRenderer ||
				 Cloud.CloudRendererType == CloudRendererTypeEnum.ShurikenShadingGroupRenderer ||
				 Cloud.CloudRendererType == CloudRendererTypeEnum.ShurikenVerticalColorRenderer ||
				 Cloud.CloudRendererType == CloudRendererTypeEnum.ShurikenVerticalColorWithShadingGroupRenderer)
			{
				ParticleSystem l_Shuriken = Cloud.GetComponent <ParticleSystem> ();
				ParticleSystemRenderer l_ShurikenRenderer = Cloud.GetComponent <ParticleSystemRenderer> ();
				if (l_Shuriken != null && l_ShurikenRenderer != null) {
					SerializedObject l_SerializedShuriken = new SerializedObject (l_Shuriken);
					SerializedProperty l_UVModuleEnabled = l_SerializedShuriken.FindProperty ("UVModule.enabled");
					SerializedProperty l_TilesX = l_SerializedShuriken.FindProperty ("UVModule.tilesX");
					SerializedProperty l_TilesY = l_SerializedShuriken.FindProperty ("UVModule.tilesY");

					l_SerializedShuriken.Update ();
					l_UVModuleEnabled.boolValue = true;
					l_TilesX.intValue = Cloud.XTile;
					l_TilesY.intValue = Cloud.YTile;
					l_SerializedShuriken.ApplyModifiedProperties ();
					
					
					SerializedObject l_SerializedShurikenRenderer = new SerializedObject (l_ShurikenRenderer);
					SerializedProperty l_SortMode = l_SerializedShurikenRenderer.FindProperty ("m_SortMode");
					l_SerializedShurikenRenderer.Update ();
						// Sort by distance has index 1
					l_SortMode.intValue = 1;
					l_SerializedShurikenRenderer.ApplyModifiedProperties ();
			
						// HACK: Editor visibility
					PropertyInfo l_PropertyInfo = l_ShurikenRenderer.GetType().GetProperty ("editorEnabled", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					l_PropertyInfo.SetValue (l_ShurikenRenderer, true, null);
					
					if (Cloud.CloudRenderer == null) {
						Cloud.OnEnable ();
					}
					Cloud.CloudRenderer.OnEnable ();
				}
			}
		}
		
		#endregion
	}
}