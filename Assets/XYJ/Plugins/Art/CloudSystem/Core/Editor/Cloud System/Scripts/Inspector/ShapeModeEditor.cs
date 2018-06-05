//
// Edelweiss.CloudSystemEditor.ShapeModeEditor.cs: Inspector and scene view for the shape mode.
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
	
	public class ShapeModeEditor <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		private C m_Cloud;
		private CloudEditor <C, PD, CD> m_CloudEditor;
		
		private SerializedObject m_SerializedCloudData;
		private SerializedProperty m_DensityProperty;
		private bool m_ReloadSerializedCreatorData;
		
		
		private SerializedProperty m_SelectedShapePosition;
		private SerializedProperty m_SelectedShapeRotation;
		private SerializedProperty m_SelectedShapeScale;
		private SerializedProperty m_SelectedShapeParticleCount;
		private SerializedProperty m_SelectedParticleGroupIndex;
		private SerializedProperty m_SelectedShadingGroupIndex;
		
		
		private GUIContent m_ParticleCountLabel;
		private GUIContent m_ParticleGroupLabel;
		private GUIContent m_ShadingGroupLabel;
		private GUIContent m_ShapeColorLabel;
		private GUIContent m_ShapeIndexLabel;
		
		private GUIContent m_AddButton;
		private GUIContent m_DuplicateButton;
		private GUIContent m_DeleteButton;
		
		private bool m_ShiftPressed = false;
		
		private const float c_BackfaceTransparencyFactor = 0.05f;
		
		private int m_SelectedShapeIndex;
		private int SelectedShapeIndex {
			get {
				return (m_SelectedShapeIndex);
			}
			set {
				m_SelectedShapeIndex = value;
				
				if (IsSelectedShapeIndexValid) {
					m_SelectedShapePosition = m_SerializedCloudData.FindProperty ("boxShapes.Array.data[" + SelectedShapeIndex + "].position");
					m_SelectedShapeRotation = m_SerializedCloudData.FindProperty ("boxShapes.Array.data[" + SelectedShapeIndex + "].rotation");
					m_SelectedShapeScale = m_SerializedCloudData.FindProperty ("boxShapes.Array.data[" + SelectedShapeIndex + "].scale");
					m_SelectedShapeParticleCount = m_SerializedCloudData.FindProperty ("boxShapes.Array.data[" + SelectedShapeIndex + "].particleCount");
					m_SelectedParticleGroupIndex = m_SerializedCloudData.FindProperty ("boxShapes.Array.data[" + SelectedShapeIndex + "].particleGroupIndex");
					m_SelectedShadingGroupIndex = m_SerializedCloudData.FindProperty ("boxShapes.Array.data[" + SelectedShapeIndex + "].shadingGroupIndex");
				}
			}
		}
		
		private bool IsSelectedShapeIndexValid {
			get {
				bool l_Result = (0 <= SelectedShapeIndex && SelectedShapeIndex < m_Cloud.CreatorData.boxShapes.Length);
				return (l_Result);
			}
		}
		
		
		public void Initialize (CloudEditor <C, PD, CD> a_CloudSystemEditor) {
			m_CloudEditor = a_CloudSystemEditor;
			m_Cloud = m_CloudEditor.Cloud;
			
			m_ParticleCountLabel = new GUIContent ("Particle Count", "Number of particles that are created for this shape.");
			m_ParticleGroupLabel = new GUIContent ("Particle Group", "That particle group is used to create the particles.");
			m_ShadingGroupLabel = new GUIContent ("Shading Group", "Particles that are created for that shape will belong to this shading group.");
			m_ShapeColorLabel = new GUIContent ("Shape Color", "The shapes can either be shown with the colors of the associated shading groups or particle groups.");
			m_ShapeIndexLabel = new GUIContent ("Shape Index", "Currently selected shape index.");
			
			m_AddButton = new GUIContent ("Add", "Create a new shape.");
			m_DuplicateButton = new GUIContent ("Duplicate", "Duplicate the selected shape.");
			m_DeleteButton = new GUIContent ("Delete", "Remove the selected shape.");
			
			InitializeSerializedObject ();
		}
		
		private void InitializeSerializedObject () {
			bool l_ExistedBefore = (m_SerializedCloudData != null);
		
			m_SerializedCloudData = new SerializedObject (m_Cloud.CreatorData);
			m_DensityProperty = m_SerializedCloudData.FindProperty ("density");
			
			SelectedShapeIndex = SelectedShapeIndex;
			
			if (l_ExistedBefore) {
				m_SerializedCloudData.Update ();
				EditorUtility.SetDirty (m_Cloud.CreatorData);
			}
		}
		
		public void InspectorGUI () {
			m_ReloadSerializedCreatorData = false;
			m_SerializedCloudData.Update ();
			
			ShapeModeInspectorGUI ();
			
			if (m_ReloadSerializedCreatorData) {
				InitializeSerializedObject ();
			}
			
			m_SerializedCloudData.ApplyModifiedProperties ();
			if (GUI.changed) {
				EditorUtility.SetDirty (m_Cloud.CreatorData);
			}
		}
		
		
		private void ShapeModeInspectorGUI () {
			
			EditorGUILayout.Space ();
			if (!IsSelectedShapeIndexValid || m_Cloud.CreatorData.boxShapes.Length == 0) {

				InspectorSupport.Explanation ("Shapes",
				                              "This cloud does not have shapes so far. You need them in order to use the Cloud Creator.",
				                              null);
				
			} else {
				
					// Explanation
				
				InspectorSupport.Explanation ("Shapes",
				                              "Modify, add or remove shapes.",
				                              "Click on the circle of a shape to change the selection\nUse the transform tools to modify the selected shape\nHold Shift to make the draggable face elements visible\nPress Shift-D to duplicate the selected shape\nPress Delete to remove the selected shape");
				
				EditorGUILayout.Space ();
				CloudCreatorEditorSupport <C, PD, CD>.CloudCreatorInspectorGUI (m_CloudEditor, m_DensityProperty);

				
					// Shape properties
			
				EditorGUILayout.Space ();
				GUILayout.Label ("Shape Properties", EditorStyles.boldLabel);
				
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				
				m_SelectedShapeParticleCount.intValue = EditorGUILayout.IntField (m_ParticleCountLabel, m_SelectedShapeParticleCount.intValue);
				if (m_SelectedShapeParticleCount.intValue < 0) {
					m_SelectedShapeParticleCount.intValue = 0;
				}
				
					// Particle group selection
				List <string> l_ParticleGroupNames = new List <string> ();
				for (int i = 0; i < m_Cloud.CreatorData.particleGroups.Length; i = i + 1) {
					string l_Name = m_Cloud.CreatorData.particleGroups [i].name;
					if (l_Name == "") {
						l_Name = " ";
					}
					while (l_ParticleGroupNames.Contains (l_Name)) {
						l_Name = l_Name + " ";
					}
					
					l_ParticleGroupNames.Add (l_Name);
				}
				List <GUIContent> l_ParticleGroupNamesGUIContent = new List <GUIContent> ();
				for (int i = 0; i < l_ParticleGroupNames.Count; i = i + 1) {
					l_ParticleGroupNamesGUIContent.Add (new GUIContent (l_ParticleGroupNames [i]));
				}
				m_SelectedParticleGroupIndex.intValue = EditorGUILayout.Popup (m_ParticleGroupLabel, m_SelectedParticleGroupIndex.intValue, l_ParticleGroupNamesGUIContent.ToArray ());
				
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
				m_SelectedShadingGroupIndex.intValue = EditorGUILayout.Popup (m_ShadingGroupLabel, m_SelectedShadingGroupIndex.intValue, l_ShadingGroupNamesGUIContent.ToArray ());
				
				m_SelectedShapePosition.vector3Value = EditorGUILayout.Vector3Field ("Position", m_SelectedShapePosition.vector3Value);
				m_SelectedShapeRotation.vector3Value = EditorGUILayout.Vector3Field ("Rotation", m_SelectedShapeRotation.vector3Value);
				m_SelectedShapeScale.vector3Value = EditorGUILayout.Vector3Field ("Scale", m_SelectedShapeScale.vector3Value);
				Vector3 l_Scale = m_SelectedShapeScale.vector3Value;
				if (l_Scale.x < 0.0f) {
					l_Scale.x = 0.0f;
				}
				if (l_Scale.y < 0.0f) {
					l_Scale.y = 0.0f;
				}
				if (l_Scale.z < 0.0f) {
					l_Scale.z = 0.0f;
				}
				m_SelectedShapeScale.vector3Value = l_Scale;
				
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
			
			if (m_Cloud.CreatorData.particleGroups.Length > 0 && m_Cloud.shadingGroups.Length > 0) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (IsSelectedShapeIndexValid) {
					if (GUILayout.Button (m_DeleteButton)) {
						DeleteSelectedShape ();
					}
					EditorGUILayout.Space ();
					if (GUILayout.Button (m_DuplicateButton)) {
						DuplicateSelectedShape ();
					}
				}
				if (GUILayout.Button (m_AddButton)) {
					AddShape ();
				}
				EditorGUILayout.EndHorizontal ();
				
			} else {
				EditorGUILayout.Space ();
				EditorGUILayout.BeginVertical (GUI.skin.box);
				GUILayout.Label ("Information", EditorStyles.boldLabel);
				GUILayout.Label ("You need at least one particle group and one shading group, before you can start to work with shapes.", EditorStyles.wordWrappedLabel);
				EditorGUILayout.EndVertical ();
			}
			
			if (m_Cloud.CreatorData.boxShapes.Length > 1) {
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				GUILayout.Label ("Selected Shape", EditorStyles.boldLabel);
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				SelectedShapeIndex = EditorGUILayout.IntSlider (m_ShapeIndexLabel, SelectedShapeIndex, 0, m_Cloud.CreatorData.boxShapes.Length - 1);
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
			
			if
				(m_Cloud.CreatorData.boxShapes.Length > 0 &&
				 m_Cloud.CreatorData.particleGroups.Length > 0 &&
				 m_Cloud.shadingGroups.Length > 0)
			{
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				GUILayout.Label ("Visualization", EditorStyles.boldLabel);
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
				CloudSystemPrefs.ShapeColorType = (ShapeColorTypeEnum) EditorGUILayout.EnumPopup (m_ShapeColorLabel, CloudSystemPrefs.ShapeColorType);
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		}
		
		private void AddShape () {
			Undo.RegisterUndo (m_Cloud.CreatorData, "Add shape");
			CS_Shape l_Shape = new CS_Shape ();
			ArraySupport.Add (ref m_Cloud.CreatorData.boxShapes, l_Shape);
			SelectedShapeIndex = m_Cloud.CreatorData.boxShapes.Length - 1;
			
			m_ReloadSerializedCreatorData = true;
			m_CloudEditor.Repaint ();
		}
		
		public void DuplicateSelectedShape () {
			if (IsSelectedShapeIndexValid) {
				Undo.RegisterUndo (m_Cloud, "Duplicate shape");
				CS_Shape l_NewShape = m_Cloud.CreatorData.boxShapes [SelectedShapeIndex].Copy ();
				ArraySupport.Add (ref m_Cloud.CreatorData.boxShapes, l_NewShape);
				
				m_ReloadSerializedCreatorData = true;
				m_CloudEditor.Repaint ();
			}
		}
		
		public void DeleteSelectedShape () {
			if (IsSelectedShapeIndexValid) {
				Undo.RegisterUndo (m_Cloud.CreatorData, "Delete shape");
				ArraySupport.RemoveAt (ref m_Cloud.CreatorData.boxShapes, SelectedShapeIndex);
				
					// With this we have still a selected particle, even if we
					// delete the last one from the array.
				if (!IsSelectedShapeIndexValid) {
					SelectedShapeIndex = SelectedShapeIndex - 1;
				}
				
				m_ReloadSerializedCreatorData = true;
				m_CloudEditor.Repaint ();
			}
		}
	
		private float m_ShapeTransparency = 0.01f;
		private float m_SelectedShapeTransparency = 0.1f;
		private bool m_EditorFillShapes = true;
		
		public void SceneGUI () {
			ProcessShapeKeyEvents ();
			
			Camera l_Camera = SceneView.currentDrawingSceneView.camera;
			Matrix4x4 l_CloudLocalToWorldMatrix = m_Cloud.TransformMatrix ();
			Matrix4x4 l_WorldToLocalMatrix = l_CloudLocalToWorldMatrix.inverse;
			
			
				// Draw boxes
			
			for (int i = 0; i < m_Cloud.CreatorData.boxShapes.Length; i = i + 1) {
				
				CS_Shape l_BoxShape = m_Cloud.CreatorData.boxShapes [i];
			
				Color l_FaceColor;
				Color l_BackFaceColor;
				if (CloudSystemPrefs.ShapeColorType == ShapeColorTypeEnum.ShadingGroup) {
					l_FaceColor = m_Cloud.shadingGroups [l_BoxShape.shadingGroupIndex].shapeColor;
				} else {
					l_FaceColor = m_Cloud.CreatorData.particleGroups [l_BoxShape.particleGroupIndex].shapeColor;
				}
				l_BackFaceColor = l_FaceColor;
				l_BackFaceColor.a = l_BackFaceColor.a * c_BackfaceTransparencyFactor;
				
				Color l_OutlineColor = l_FaceColor;
				Color l_BackOutlineColor;
				if (i != m_SelectedShapeIndex) {
					l_FaceColor.a = l_FaceColor.a * m_ShapeTransparency;
					l_OutlineColor.a = 0.5f;
				} else {
					l_FaceColor.a = l_FaceColor.a * m_SelectedShapeTransparency;
					l_OutlineColor.a = 1.0f;
				}
				l_BackOutlineColor = l_OutlineColor;
				l_BackOutlineColor.a = l_BackOutlineColor.a * c_BackfaceTransparencyFactor;
				
				if (!m_EditorFillShapes) {
					l_FaceColor = new Color (0.0f, 0.0f, 0.0f, 0.0f);
				}
								
				Matrix4x4 l_BoxTransformMatrix = l_CloudLocalToWorldMatrix * l_BoxShape.LocalTransformMatrix ();
				Matrix4x4 l_BoxNormalTransformMatrix = l_BoxTransformMatrix.inverse.transpose;
				
					// Prepare box faces.
				List <BoxFace> l_BoxFaces = BoxShapeEditorSupport.BoxFaces (l_BoxShape);
				for (int j = 0; j < l_BoxFaces.Count; j = j + 1) {
					BoxFace l_BoxFace = l_BoxFaces [j];
					
					l_BoxFace = BoxFace.TransformBoxFace (l_BoxTransformMatrix, l_BoxNormalTransformMatrix, l_BoxFace);
					
					Vector3 l_ScreenPoint = l_Camera.WorldToScreenPoint (l_BoxFace.center);
					Ray l_CameraRay = l_Camera.ScreenPointToRay (l_ScreenPoint);
					l_BoxFace.dotProduct = Vector3.Dot (l_CameraRay.direction, l_BoxFace.normal);
					
					l_BoxFaces [j] = l_BoxFace;
				}
				l_BoxFaces.Sort ();
				
					// Draw box faces.
				foreach (BoxFace l_BoxFace in l_BoxFaces) {
					if (l_BoxFace.dotProduct <= 0.0f) {
						Handles.DrawSolidRectangleWithOutline (l_BoxFace.faceVertices, l_FaceColor, l_OutlineColor);
					} else {
						Handles.DrawSolidRectangleWithOutline (l_BoxFace.faceVertices, l_BackFaceColor, l_BackOutlineColor);
					}
				}
				
			}
			
				// Draw circles
			
			Color l_PreviousColor = Handles.color;
			Handles.color = CloudSystemPrefs.HighlightColor;
			for (int i = 0; i < m_Cloud.CreatorData.boxShapes.Length; i = i + 1) {
				if (i != m_SelectedShapeIndex) {
					CS_Shape l_BoxShape = m_Cloud.CreatorData.boxShapes [i];
					Vector3 l_Position = l_CloudLocalToWorldMatrix.MultiplyPoint3x4 (l_BoxShape.position);
					float l_HandleSize = HandleUtility.GetHandleSize (l_Position) * CloudSystemPrefs.c_SelectableCircleRadius;
					Handles.CircleCap (-i, l_Position, Camera.current.transform.rotation, l_HandleSize);
				}
			}
			Handles.color = l_PreviousColor;
			
			
				// Selected box functionality (handles)
			
			bool l_HandleIsUsed = false;
			
			if (IsSelectedShapeIndexValid) {
				CS_Shape l_SelectedShape = m_Cloud.CreatorData.boxShapes [m_SelectedShapeIndex];
				Vector3 l_SelectedWorldPosition = l_CloudLocalToWorldMatrix.MultiplyPoint3x4 (l_SelectedShape.position);
				
					// HACK: Workaround for a Unity bug that may freeze the scene view.
//				if (HandlesSupport.IsHandleDrawingSave ()) {
				
					if (ToolsSupport.Current == TransformTool.Move) {
		
							// Move
		
						Undo.SetSnapshotTarget (m_Cloud.CreatorData, "Moved Shape");
						Quaternion l_HandleRotation;
						if (ToolsSupport.CurrentPivotRotation == PivotRotationTool.Global) {
							l_HandleRotation = Quaternion.identity;
						} else {
							l_HandleRotation = m_Cloud.transform.rotation * Quaternion.Euler (l_SelectedShape.rotation);
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
							l_SelectedShape.position = l_TmpPosition;
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
						
						Undo.SetSnapshotTarget (m_Cloud.CreatorData, "Rotated Shape");
	
						Quaternion l_Rotation = m_Cloud.transform.rotation * Quaternion.Euler (l_SelectedShape.rotation);
						
						int l_ControlIDBeforeHandle = GUIUtility.GetControlID (m_CloudEditor.CloudSystemEditorHash, FocusType.Passive);
						bool l_IsEventUsedBeforeHandle = (Event.current.type == EventType.used);
						Quaternion l_NewRotation = Handles.RotationHandle (l_Rotation, l_SelectedWorldPosition);
						int l_ControlIDAfterHandle = GUIUtility.GetControlID (m_CloudEditor.CloudSystemEditorHash, FocusType.Passive);
						bool l_IsEventUsedByHandle = false;
						if (!l_IsEventUsedBeforeHandle) {
							l_IsEventUsedByHandle = (Event.current.type == EventType.used);
						}
						
							// Only apply changes if the handle uses the current event or if the
							// hot control is from the handle.
						
							// CloudRot * ShapeRot = Rot
							// => ShapeRot = CloudRot(-1) * Rot
						
						if
							((l_ControlIDBeforeHandle < GUIUtility.hotControl &&
							  GUIUtility.hotControl < l_ControlIDAfterHandle) ||
							  l_IsEventUsedByHandle)
						{
							l_SelectedShape.rotation = (Quaternion.Inverse (m_Cloud.transform.rotation) * l_NewRotation).eulerAngles;
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
						
						Undo.SetSnapshotTarget (m_Cloud.CreatorData, "Scaled Shape");
						
						l_PreviousColor = GUI.color;
						Handles.color = CloudSystemPrefs.HighlightColor;
						
						float l_HandleSize = HandleUtility.GetHandleSize (l_SelectedWorldPosition);
						Quaternion l_Rotation = m_Cloud.transform.rotation * Quaternion.Euler (l_SelectedShape.rotation);
						l_SelectedShape.scale = Handles.ScaleHandle (l_SelectedShape.scale, l_SelectedWorldPosition, l_Rotation, l_HandleSize);
							
						Handles.color = l_PreviousColor;
					}
					
					
						// Face selection
					
						// HACK: Workaround for a Unity bug that may freeze the scene view.
//					if (HandlesSupport.IsHandleDrawingSave ()) {
						if (m_ShiftPressed) {
							
							Undo.SetSnapshotTarget (m_Cloud.CreatorData, "Scaled Shape");
							
							List <BoxFace> l_SelectedBoxFaces = BoxShapeEditorSupport.BoxFaces (l_SelectedShape);
							
							Matrix4x4 l_BoxTransformMatrix = l_CloudLocalToWorldMatrix * l_SelectedShape.LocalTransformMatrix ();
							Matrix4x4 l_BoxNormalTransformMatrix = l_BoxTransformMatrix.inverse.transpose;
							for (int i = 0; i < l_SelectedBoxFaces.Count; i = i + 1) {
								BoxFace l_BoxFace = l_SelectedBoxFaces [i];
								l_BoxFace = BoxFace.TransformBoxFace (l_BoxTransformMatrix, l_BoxNormalTransformMatrix, l_BoxFace);
								Vector3 l_ScreenPoint = l_Camera.WorldToScreenPoint (l_BoxFace.center);
								Ray l_CameraRay = l_Camera.ScreenPointToRay (l_ScreenPoint);
								l_BoxFace.dotProduct = Vector3.Dot (l_CameraRay.direction, l_BoxFace.normal);
								l_SelectedBoxFaces [i] = l_BoxFace;
							}
							
							int l_UsedFaceIndex = -1;
							Vector3 l_NewPosition = Vector3.zero;
							for (int i = 0; i < l_SelectedBoxFaces.Count; i = i + 1) {
								BoxFace l_BoxFace = l_SelectedBoxFaces [i];
								float l_HandleSize = HandleUtility.GetHandleSize (l_BoxFace.center) * CloudSystemPrefs.c_SelectableCircleRadius;
								
								int l_ControlIDBeforeHandle = GUIUtility.GetControlID (m_CloudEditor.CloudSystemEditorHash, FocusType.Passive);
								bool l_IsEventUsedBeforeHandle = (Event.current.type == EventType.used);
								if (l_BoxFace.dotProduct < 0.0f) {
									Handles.color = new Color (1.0f, 1.0f, 1.0f, 0.8f);
								} else {
									Handles.color = new Color (1.0f, 1.0f, 1.0f, c_BackfaceTransparencyFactor);
								}
								Vector3 l_TmpNewPosition = Handles.FreeMoveHandle (l_BoxFace.center, Quaternion.identity, l_HandleSize, Vector3.one, Handles.CubeCap);
									
								
								int l_ControlIDAfterHandle = GUIUtility.GetControlID (m_CloudEditor.CloudSystemEditorHash, FocusType.Passive);
								bool l_IsEventUsedByHandle = false;
								if (!l_IsEventUsedBeforeHandle) {
									l_IsEventUsedByHandle = (Event.current.type == EventType.used);
								}
						
									// Set l_UsedFaceIndex if needed
		
								if
									((l_ControlIDBeforeHandle < GUIUtility.hotControl &&
									  GUIUtility.hotControl < l_ControlIDAfterHandle) ||
									  l_IsEventUsedByHandle)
								{
									l_UsedFaceIndex = i;
									l_NewPosition = l_TmpNewPosition;
								}
								
								if
									((l_ControlIDBeforeHandle < HandleUtility.nearestControl &&
									  HandleUtility.nearestControl < l_ControlIDAfterHandle) ||
									  l_IsEventUsedByHandle)
								{
									l_HandleIsUsed = true;
								}
							}
							
							if (l_UsedFaceIndex != -1) {
								Matrix4x4 l_ShapeMatrix = m_Cloud.TransformMatrix () * l_SelectedShape.LocalTransformMatrix ();
								Matrix4x4 l_InverseShapeMatrix = l_ShapeMatrix.inverse;
								l_NewPosition = l_InverseShapeMatrix.MultiplyPoint3x4 (l_NewPosition);
								BoxShapeEditorSupport.MoveBoxFace (l_SelectedShape, l_UsedFaceIndex, l_NewPosition);
							}
						}
//					}
//				}
				
					// Update inspector gui
				m_CloudEditor.Repaint ();
			}
			
			
				// Other box selected
			
				// HACK: Workaround for a Unity bug that may freeze the scene view.
//			if (HandlesSupport.IsHandleDrawingSave ()) {
				if (!l_HandleIsUsed) {
					for (int i = 0; i < m_Cloud.CreatorData.boxShapes.Length; i = i + 1) {
						if (i != SelectedShapeIndex) {
							CS_Shape l_BoxShape = m_Cloud.CreatorData.boxShapes [i];
							Vector3 l_Position = l_CloudLocalToWorldMatrix.MultiplyPoint3x4 (l_BoxShape.position);
							float l_HandleSize = HandleUtility.GetHandleSize (l_Position) * CloudSystemPrefs.c_SelectableCircleRadius;
							if (Handles.Button (l_Position, Camera.current.transform.rotation, 0.0f, l_HandleSize, Handles.RectangleCap)) {
								SelectedShapeIndex = i;
								m_CloudEditor.Repaint ();
							}
						}
					}
				}
//			}
		}
		
		private void ProcessShapeKeyEvents () {
			m_ShiftPressed = Event.current.shift;
			
			if (Event.current.type == EventType.keyDown) {
				if (Event.current.keyCode == KeyCode.Delete) {
					DeleteSelectedShape ();
					Event.current.Use ();
						
						// HACK:
						// For any reason Ctrl+D does not work, as it copies the whole cloud system. So we use
						// Shift+D.
					
				} else if (Event.current.modifiers == EventModifiers.Shift && Event.current.keyCode == KeyCode.D) {
					DuplicateSelectedShape ();
					Event.current.Use ();
				}
			}
		}
		
		public void ModifierKeyChanged () {
			
				// That one is needed to show the draggable face elements
				// as soon as shift is beeing pressed.
			
			SceneView.RepaintAll ();
		}
	}
}