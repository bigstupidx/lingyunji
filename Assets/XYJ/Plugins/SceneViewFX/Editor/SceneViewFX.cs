using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling; // HACK by hdh

[Serializable]
public class SceneViewFX : SceneView {
	[SerializeField] private bool postFXOn;
	[SerializeField] private bool hideAllGizmos;
	[SerializeField] private bool fovMatch;
	
	
	
	private const KeyCode shortcutPostFx = KeyCode.P;
	private const KeyCode shortcutGizmos = KeyCode.G;

	private int controlId;

	private Camera[] cams;

	[SerializeField] private int currentSelection;

	private Camera postFxCamera;

	private bool filtering;

	private bool isDirty;

	private bool sceneCameraCleared;
	private GUIContent[] camList;

	private bool useSceneFiltering;
	private object sceneViewOverlay;


	[MenuItem("Window/Scene View FX")] private static void ShowWindow() {
		GetWindow<SceneViewFX>("SceneViewFX", true);
	}

	private void OnGUI() {
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6

		SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_CurrentDrawingSceneView",
			BindingFlags.Static | BindingFlags.NonPublic, this, this);

		if (Event.current.type == EventType.Repaint) {
			var sMouseRects = SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_MouseRects",
				BindingFlags.NonPublic | BindingFlags.Static, null);
			sMouseRects.GetType()
				.GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance)
				.Invoke(sMouseRects, null);
		}
		Color color = GUI.color;
		
		if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) {
			SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_LastActiveSceneView",
			BindingFlags.Static | BindingFlags.NonPublic, this, this);
		} else {
			if (SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_LastActiveSceneView", BindingFlags.Static | BindingFlags.NonPublic, null) == null)
			{
				SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_LastActiveSceneView",
			BindingFlags.Static | BindingFlags.NonPublic, this, this);
			}
		}

		Type dragLockStateType = SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_DraggingLockedState",
			BindingFlags.NonPublic | BindingFlags.Instance, this).GetType();
		Array enumValues = Enum.GetValues(dragLockStateType);
		if (Event.current.type == EventType.MouseDrag) {
			SceneViewFXReflections.SetField("UnityEditor.SceneView", "m_DraggingLockedState", BindingFlags.NonPublic | BindingFlags.Instance, this,
				enumValues.GetValue(1));
		} else {
			if (Event.current.type == EventType.MouseUp) {
				SceneViewFXReflections.SetField("UnityEditor.SceneView", "m_DraggingLockedState",
					BindingFlags.NonPublic | BindingFlags.Instance, this, enumValues.GetValue(2));
			}
		}
		if (Event.current.type == EventType.MouseDown) {
			SceneViewFXReflections.SetField("UnityEditor.Tools", "s_ButtonDown",
				BindingFlags.NonPublic | BindingFlags.Static, null,
				Event.current.button);
			if (Event.current.button == 1 && Application.platform == RuntimePlatform.OSXEditor) {
				base.Focus();
			}
		}
		if (Event.current.type == EventType.Layout) {
			SceneViewFXReflections.SetField("UnityEditor.SceneView", "m_ShowSceneViewWindows",
				BindingFlags.NonPublic | BindingFlags.Instance, this,
				(lastActiveSceneView == this));
		}
		
		object svo = SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_SceneViewOverlay",
			BindingFlags.NonPublic | BindingFlags.Instance, this);
		svo.GetType().GetMethod("Begin", BindingFlags.Public | BindingFlags.Instance).Invoke(svo, null);
		
		bool fog = RenderSettings.fog;
		float shadowDistance = QualitySettings.shadowDistance;

		object sceneViewState = SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_SceneViewState",
			BindingFlags.Instance | BindingFlags.NonPublic, this);
		bool showFog = (bool) sceneViewState.GetType().GetField("showFog", BindingFlags.Public | BindingFlags.Instance).GetValue(sceneViewState);

		if (Event.current.type == EventType.Repaint) {
			if (!showFog) {
				Unsupported.SetRenderSettingsUseFogNoDirty(false);
			}
			if (camera.orthographic) {
				Unsupported.SetQualitySettingsShadowDistanceTemporarily(QualitySettings.shadowDistance + 0.5f * (float) SceneViewFXReflections.GetProperty("UnityEditor.SceneView", "cameraDistance",
					BindingFlags.NonPublic | BindingFlags.Instance, this));
			}
		}

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "DoStatusBarGUI",
			BindingFlags.NonPublic | BindingFlags.Instance, this);
		
		GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
		EditorGUIUtility.labelWidth = 100f;

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "SetupCamera",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		RenderingPath renderingPath = camera.renderingPath;
		
		
		Light[] lights = (Light[]) SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_Light",
			BindingFlags.NonPublic | BindingFlags.Instance, this);
		if (!m_SceneLighting) {
			lights[0].transform.rotation = camera.transform.rotation;
			if (Event.current.type == EventType.Repaint) {
				InternalEditorUtility.SetCustomLighting(lights, (Color) SceneViewFXReflections.GetField("UnityEditor.SceneView", "kSceneViewMidLight", BindingFlags.NonPublic | BindingFlags.Static, null));
			}
		}
		PostProcessingUIAndEffects(new Rect(332, 0, 320, 16));
		GUI.BeginGroup(new Rect(0f, 17f, base.position.width, base.position.height - 17f));
		Rect rect = new Rect(0f, 0f, base.position.width, base.position.height - 17f);
		bool viewToolActive = (bool) SceneViewFXReflections.GetProperty("UnityEditor.Tools", "viewToolActive", BindingFlags.NonPublic | BindingFlags.Static, null);
		
		if (viewToolActive && Event.current.type == EventType.Repaint) {
			MouseCursor mouseCursor = MouseCursor.Arrow;
			switch (Tools.viewTool) {
				case ViewTool.Orbit:
					mouseCursor = MouseCursor.Orbit;
					break;
				case ViewTool.Pan:
					mouseCursor = MouseCursor.Pan;
					break;
				case ViewTool.Zoom:
					mouseCursor = MouseCursor.Zoom;
					break;
				case ViewTool.FPS:
					mouseCursor = MouseCursor.FPS;
					break;
			}
			if (mouseCursor != MouseCursor.Arrow) {
			SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "AddCursorRect", BindingFlags.NonPublic | BindingFlags.Static, null, 
				new object[] {new Rect(0f, 17f, base.position.width, base.position.height - 17f), mouseCursor});
			}
		}

		RenderTexture searchFilterTexture = (RenderTexture)SceneViewFXReflections.GetField("UnityEditor.SceneView",
				"m_SearchFilterTexture", BindingFlags.NonPublic | BindingFlags.Instance, this);

		if (useSceneFiltering) {
			SceneViewFXReflections.RunMethod("UnityEditor.EditorUtility", "SetTemporarilyAllowIndieRenderTexture",
				BindingFlags.Static | BindingFlags.NonPublic, null, new object[] {true});

			
			if (searchFilterTexture == null) {
				searchFilterTexture = new RenderTexture(0, 0, 24);
				searchFilterTexture.hideFlags = (HideFlags)13;//.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable);
				SceneViewFXReflections.SetField("UnityEditor.SceneView", "m_SearchFilterTexture", BindingFlags.NonPublic | BindingFlags.Instance, this, searchFilterTexture);
			}
			Rect cameraRect = (Rect) SceneViewFXReflections.RunMethod("UnityEditor.Handles", "GetCameraRect", BindingFlags.NonPublic | BindingFlags.Static, null, new object[] {rect});
			if (searchFilterTexture.width != (int)cameraRect.width || searchFilterTexture.height != (int)cameraRect.height) {
				searchFilterTexture.Release();
				searchFilterTexture.width = (int)cameraRect.width;
				searchFilterTexture.height = (int)cameraRect.height;
				SceneViewFXReflections.SetField("UnityEditor.SceneView", "m_SearchFilterTexture", BindingFlags.NonPublic | BindingFlags.Instance, this, searchFilterTexture);
			}
			this.camera.targetTexture = searchFilterTexture;
			if (this.camera.actualRenderingPath == RenderingPath.DeferredLighting) {
				this.camera.renderingPath = RenderingPath.Forward;
			}
		} else {
			this.camera.targetTexture = null;
		}
		float verticalFOV = (float) SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "GetVerticalFOV", BindingFlags.NonPublic | BindingFlags.Instance, this, new object[] {90f});
		float fieldOfView = this.camera.fieldOfView;
		this.camera.fieldOfView = verticalFOV;
		Handles.ClearCamera(rect, camera);
		this.camera.fieldOfView = fieldOfView;
		this.camera.cullingMask = Tools.visibleLayers;

		useSceneFiltering = (bool)SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "UseSceneFiltering",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		
		
		// Custom Post processing HERE (After setup camera and before camera drawing).
		if (postFXOn && postFxCamera != null) {
			camera.renderingPath = postFxCamera.renderingPath;
			camera.hdr = postFxCamera.hdr;
			camera.nearClipPlane = postFxCamera.nearClipPlane;
			camera.farClipPlane = postFxCamera.farClipPlane;
			camera.depthTextureMode = postFxCamera.depthTextureMode;
		}
		if (postFxCamera != null && fovMatch) camera.fieldOfView = postFxCamera.fieldOfView;

		
		
		if (!useSceneFiltering) {
			Handles.SetCamera(rect, camera);
			SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "CallOnPreSceneGUI",
				BindingFlags.NonPublic | BindingFlags.Instance, this);
		}

		Shader replacementShader = (Shader)SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_ReplacementShader",
						BindingFlags.NonPublic | BindingFlags.Instance, this);
		string replacementString = (string)SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_ReplacementString",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		if (Event.current.type == EventType.Repaint) {
			var sceneViewWire = SceneViewFXReflections.GetField("UnityEditor.SceneView", "kSceneViewWire",
				BindingFlags.NonPublic | BindingFlags.Static, null);
			var sceneViewWireOverlay = SceneViewFXReflections.GetField("UnityEditor.SceneView", "kSceneViewWireOverlay",
				BindingFlags.NonPublic | BindingFlags.Static, null);
			var sceneViewWireActive = SceneViewFXReflections.GetField("UnityEditor.SceneView", "kSceneViewWireActive",
				BindingFlags.NonPublic | BindingFlags.Static, null);
			var sceneViewWireSelected = SceneViewFXReflections.GetField("UnityEditor.SceneView", "kSceneViewWireSelected",
				BindingFlags.NonPublic | BindingFlags.Static, null);
			Type prefColorType = sceneViewWire.GetType();
			
			//Debug.Log(sceneViewWire);
			SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetSceneViewColors",
				BindingFlags.NonPublic | BindingFlags.Static, null,
				new object[] { 
					prefColorType.GetField("m_color", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sceneViewWire), 
					prefColorType.GetField("m_color", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sceneViewWireOverlay), 
					prefColorType.GetField("m_color", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sceneViewWireActive), 
					prefColorType.GetField("m_color", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sceneViewWireSelected)
				});
			

			if (this.m_OverlayMode == 2) {
				Shader showOverdrawShader = (Shader) SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_ShowOverdrawShader",
					BindingFlags.NonPublic | BindingFlags.Static, null);
				if (showOverdrawShader == null) {
					showOverdrawShader = EditorGUIUtility.LoadRequired("SceneView/SceneViewShowOverdraw.shader") as Shader;
					SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_ShowOverdrawShader",
						BindingFlags.NonPublic | BindingFlags.Static, null, showOverdrawShader);
				}
				this.camera.SetReplacementShader(showOverdrawShader, "RenderType");
			} else {
				if (this.m_OverlayMode == 3) {
					Shader showMipsShader = (Shader)SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_ShowMipsShader",
					BindingFlags.NonPublic | BindingFlags.Static, null);
					if (showMipsShader == null) {
						showMipsShader = (Shader) EditorGUIUtility.LoadRequired("SceneView/SceneViewShowMips.shader");
						SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_ShowMipsShader",
							BindingFlags.NonPublic | BindingFlags.Static, null, showMipsShader);
					}
					if (showMipsShader.isSupported) {
						SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "CreateMipColorsTexture",
							BindingFlags.NonPublic | BindingFlags.Static, null);
						showMipsShader = (Shader) SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_ShowMipsShader",
							BindingFlags.NonPublic | BindingFlags.Static, null);
						this.camera.SetReplacementShader(showMipsShader, "RenderType");
					} else {
						this.camera.SetReplacementShader(replacementShader, replacementString);
					}
				} else {
					if (this.m_OverlayMode == 4) {
						Shader showLightmapsShader =
							(Shader)
								SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_ShowLightmapsShader",
									BindingFlags.Static | BindingFlags.NonPublic, null);
						if (showLightmapsShader == null) {
							showLightmapsShader = (Shader) EditorGUIUtility.LoadRequired("SceneView/SceneViewShowLightmap.shader");
							SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_ShowLightmapsShader",
							BindingFlags.NonPublic | BindingFlags.Static, null, showLightmapsShader);
						}
						if (showLightmapsShader.isSupported) {
							this.camera.SetReplacementShader(showLightmapsShader, "RenderType");
						} else {
							this.camera.SetReplacementShader(replacementShader, replacementString);
						}
					} else {
						this.camera.SetReplacementShader(replacementShader, replacementString);
					}
				}
			}
		}


		controlId = GUIUtility.GetControlID(FocusType.Keyboard);
		if (Event.current.GetTypeForControl(controlId) == EventType.MouseDown) {
			GUIUtility.keyboardControl = controlId;
		}


		if (this.camera.gameObject.activeInHierarchy) {
			var grid = SceneViewFXReflections.GetField("UnityEditor.SceneView", "grid",
				BindingFlags.Instance | BindingFlags.NonPublic, this);
			var showGrid = SceneViewFXReflections.GetProperty("UnityEditor.AnnotationUtility", "showGrid",
				BindingFlags.Static | BindingFlags.NonPublic, null);
			var gridParam = grid.GetType().GetMethod("PrepareGridRender", BindingFlags.Instance | BindingFlags.Public).Invoke(grid,
				new object[] {
					this.camera, this.pivot, this.rotation, this.size, this.orthographic, showGrid
				});
			
			if (useSceneFiltering) {
				if (Event.current.type == EventType.Repaint) {
					
					SceneViewFXReflections.RunMethod("UnityEditor.Handles", "EnableCameraFx",
						BindingFlags.NonPublic | BindingFlags.Static, null, new object[] { camera, true });
					SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
						BindingFlags.Static | BindingFlags.NonPublic, null,
						new object[] {this.camera, 2});

					double startSearchFilterTime = (double) SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_StartSearchFilterTime",
						BindingFlags.NonPublic | BindingFlags.Instance, this);
					float num = Mathf.Clamp01((float)(EditorApplication.timeSinceStartup - startSearchFilterTime));
					Handles.DrawCamera(rect, camera, m_RenderMode);
					SceneViewFXReflections.RunMethod("UnityEditor.Handles", "DrawCameraFade",
						BindingFlags.NonPublic | BindingFlags.Static, null, new object[] {camera, num});
					
					RenderTexture.active = null;
					SceneViewFXReflections.RunMethod("UnityEditor.Handles", "EnableCameraFx",
						BindingFlags.NonPublic | BindingFlags.Static, null, new object[] { camera, false});

					SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
						BindingFlags.Static | BindingFlags.NonPublic, null,
						new object[] { this.camera, 1 });
					//Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.ShowFiltered);

					Shader auraShader = (Shader) SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_AuraShader",
						BindingFlags.NonPublic | BindingFlags.Static, null);

					if (auraShader == null) {
						auraShader = (EditorGUIUtility.LoadRequired("SceneView/SceneViewAura.shader") as Shader);
						SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_AuraShader",
							BindingFlags.NonPublic | BindingFlags.Static, null, auraShader);
					}
					this.camera.SetReplacementShader(auraShader, string.Empty);
					Handles.DrawCamera(rect, camera, this.m_RenderMode);
					this.camera.SetReplacementShader(replacementShader, replacementString);
					SceneViewFXReflections.RunMethod("UnityEditor.Handles", "DrawCamera",
						BindingFlags.NonPublic | BindingFlags.Static, null, new object[] {
							rect, camera, this.m_RenderMode, gridParam
						});
					if (num < 1f) {
						base.Repaint();
					}
				}
				Rect position = rect;
				GUI.EndGroup();
				GUI.BeginGroup(new Rect(0f, 17f, base.position.width, base.position.height - 17f));
				GUI.DrawTexture(position, searchFilterTexture, ScaleMode.StretchToFill, false, 0f);
				Handles.SetCamera(rect, this.camera);
				SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "HandleSelectionAndOnSceneGUI",
				BindingFlags.NonPublic | BindingFlags.Instance, this);
			} else {
				SceneViewFXReflections.RunMethod("UnityEditor.Handles", "DrawCameraStep1",
					BindingFlags.NonPublic | BindingFlags.Static, null, new object[] {
						rect, this.camera, this.m_RenderMode, gridParam
					});
				SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "DrawAlphaOverlay",
					BindingFlags.NonPublic | BindingFlags.Instance, this);
			}
		}

		if (!this.m_SceneLighting && Event.current.type == EventType.Repaint) {
			InternalEditorUtility.RemoveCustomLighting();
		}

		if (useSceneFiltering) {
			SceneViewFXReflections.RunMethod("UnityEditor.EditorUtility", "SetTemporarilyAllowIndieRenderTexture",
				BindingFlags.NonPublic | BindingFlags.Static, null, new object[] {false});
		}

		if (Event.current.type == EventType.ExecuteCommand || Event.current.type == EventType.ValidateCommand) {
			SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "CommandsGUI",
				BindingFlags.NonPublic | BindingFlags.Instance, this);
		}
		if (Event.current.type == EventType.Repaint) {
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			Unsupported.SetQualitySettingsShadowDistanceTemporarily(shadowDistance);
		}

		camera.renderingPath = renderingPath;

		if (!useSceneFiltering) {
			SceneViewFXReflections.RunMethod("UnityEditor.Handles", "DrawCameraStep2",
				BindingFlags.NonPublic | BindingFlags.Static, null, new object[] {
					camera, m_RenderMode
				});
		}

		if (useSceneFiltering) {
			SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
				BindingFlags.NonPublic | BindingFlags.Static,
				null, new object[] { Camera.current, 1 });
		} else {
			SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
				BindingFlags.NonPublic | BindingFlags.Static,
				null, new object[] { Camera.current, 0 });
		}

		if (!useSceneFiltering) {
			SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "HandleSelectionAndOnSceneGUI",
				BindingFlags.NonPublic | BindingFlags.Instance, this);
		}

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "DefaultHandles",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		
		SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
			BindingFlags.NonPublic | BindingFlags.Static,
			null, new object[] { Camera.current, 0 });
		SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
			BindingFlags.NonPublic | BindingFlags.Static,
			null, new object[] { camera, 0 });

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "HandleDragging",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		object svr = SceneViewFXReflections.GetField("UnityEditor.SceneView", "svRot",
			BindingFlags.NonPublic | BindingFlags.Instance, this);
		svr.GetType()
			.GetMethod("HandleContextClick", BindingFlags.NonPublic | BindingFlags.Instance)
			.Invoke(svr, new object[] { this });
		svr.GetType().GetMethod("OnGUI", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(svr, new object[] { this });

		SceneViewFXReflections.RunMethod("UnityEditor.SceneViewMotion", "ArrowKeys",
				BindingFlags.Public | BindingFlags.Static, this, new object[] { this });
		SceneViewFXReflections.RunMethod("UnityEditor.SceneViewMotion", "DoViewTool",
				BindingFlags.Public | BindingFlags.Static, this, new object[] { camera.transform, this });

		
		var k2DMode = SceneViewFXReflections.GetField("UnityEditor.SceneView", "k2DMode",
			BindingFlags.NonPublic | BindingFlags.Static, null);
		bool k2Dactivated = (bool) k2DMode.GetType().GetProperty("activated", BindingFlags.Public | BindingFlags.Instance).GetGetMethod().Invoke(k2DMode, null);
		bool waitingForKeyUp = (bool) SceneViewFXReflections.GetField("UnityEditor.SceneView", "waitingFor2DModeKeyUp",
			BindingFlags.Static | BindingFlags.NonPublic, null);
		if (k2Dactivated && !waitingForKeyUp) {
			SceneViewFXReflections.SetField("UnityEditor.SceneView", "waitingFor2DModeKeyUp",
				BindingFlags.NonPublic | BindingFlags.Static, null, true);
			in2DMode = !in2DMode;
			Event.current.Use();
		} else {
			if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Alpha2) {
				SceneViewFXReflections.SetField("UnityEditor.SceneView", "waitingFor2DModeKeyUp",
				BindingFlags.NonPublic | BindingFlags.Static, null, false);
			}
		}
		
		GUI.EndGroup();
		GUI.color = color;

		svo.GetType().GetMethod("End", BindingFlags.Public | BindingFlags.Instance).Invoke(svo, null);

		
		bool draggingCursorIsCashed = (bool) SceneViewFXReflections.GetField("UnityEditor.SceneView",
			"s_DraggingCursorIsCashed", BindingFlags.NonPublic | BindingFlags.Instance, this);
		
		
		if (GUIUtility.hotControl == 0) {
			draggingCursorIsCashed = false;
			SceneViewFXReflections.SetField("UnityEditor.SceneView",
			"s_DraggingCursorIsCashed", BindingFlags.NonPublic | BindingFlags.Instance, this, false);
		}
		
		Rect rect2 = new Rect(0f, 0f, base.position.width, base.position.height);
		object lastCursor;
		if (!draggingCursorIsCashed) {
			MouseCursor mouseCursor2 = MouseCursor.Arrow;
			if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.Repaint) {

				var sMouseRects = SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_MouseRects",
					BindingFlags.NonPublic | BindingFlags.Static, null);

				foreach (var current in (IEnumerable) sMouseRects) {
					Rect currentRect = (Rect) current.GetType().GetField("rect").GetValue(current);
					MouseCursor currentCursor = (MouseCursor) current.GetType().GetField("cursor").GetValue(current);
					if (currentRect.Contains(Event.current.mousePosition)) {
						mouseCursor2 = currentCursor;
						rect2 = currentRect;
					}
				}

				if (GUIUtility.hotControl != 0) {
					SceneViewFXReflections.SetField("UnityEditor.SceneView",
						"s_DraggingCursorIsCashed", BindingFlags.NonPublic | BindingFlags.Instance, this, true);
				}
				lastCursor = SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_LastCursor",
					BindingFlags.NonPublic | BindingFlags.Static, null);
				if (mouseCursor2 != (MouseCursor) lastCursor) {
					SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_LastCursor",
					BindingFlags.NonPublic | BindingFlags.Static, null, mouseCursor2);
					InternalEditorUtility.ResetCursor();
					base.Repaint();
				}
			}
		}
		lastCursor = SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_LastCursor",
			BindingFlags.NonPublic | BindingFlags.Static, null);
		if (Event.current.type == EventType.Repaint && (MouseCursor)lastCursor != MouseCursor.Arrow) {
			EditorGUIUtility.AddCursorRect(rect2, (MouseCursor)lastCursor);
		}
		
#else
		SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_CurrentDrawingSceneView",
			BindingFlags.Static | BindingFlags.NonPublic, this, this);
		Event current = Event.current;
		if (current.type == EventType.Repaint) {
			var sMouseRects = SceneViewFXReflections.GetField("UnityEditor.SceneView", "s_MouseRects",
				BindingFlags.NonPublic | BindingFlags.Static, null);
			sMouseRects.GetType()
				.GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance)
				.Invoke(sMouseRects, null);
			Profiler.BeginSample("SceneViewFX.Repaint");
		}
		Color color = GUI.color;
		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "HandleClickAndDragToFocus",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		if (current.type == EventType.Layout) {
			SceneViewFXReflections.SetField("UnityEditor.SceneView", "m_ShowSceneViewWindows",
				BindingFlags.NonPublic | BindingFlags.Instance, this, (lastActiveSceneView == this));
		}
		object svo = SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_SceneViewOverlay",
			BindingFlags.NonPublic | BindingFlags.Instance, this);
		svo.GetType().GetMethod("Begin", BindingFlags.Public | BindingFlags.Instance).Invoke(svo, null);

		bool oldFog = false;
		float oldShadowDistance = 0f;
		object[] parameters = {oldFog, oldShadowDistance};
		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "SetupFogAndShadowDistance",
			BindingFlags.NonPublic | BindingFlags.Instance,
			this, parameters);
		oldFog = (bool) parameters[0];
		oldShadowDistance = (float) parameters[1];

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "DoToolbarGUI",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
		EditorGUIUtility.labelWidth = 100f;
		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "SetupCamera",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		RenderingPath renderingPath = camera.renderingPath;

		// Custom Post processing HERE (After setup camera and before camera drawing).
		if (postFXOn && postFxCamera != null) {
			camera.renderingPath = postFxCamera.renderingPath;
			camera.hdr = postFxCamera.hdr;
			camera.nearClipPlane = postFxCamera.nearClipPlane;
			camera.farClipPlane = postFxCamera.farClipPlane;
			camera.depthTextureMode = postFxCamera.depthTextureMode;
		}
		if (postFxCamera != null && fovMatch) camera.fieldOfView = postFxCamera.fieldOfView;

        PostProcessingUIAndEffects(new Rect(249, 0, 320, 16));

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "SetupCustomSceneLighting",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		GUI.BeginGroup(new Rect(0f, 17f, position.width, position.height - 17f));
		Rect rect = new Rect(0f, 0f, position.width, position.height - 17f);
		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "HandleViewToolCursor",
			BindingFlags.NonPublic | BindingFlags.Instance, this);
		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "PrepareCameraTargetTexture",
			BindingFlags.NonPublic | BindingFlags.Instance, this, new object[] {rect});
		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "DoClearCamera",
			BindingFlags.NonPublic | BindingFlags.Instance, this, new object[] {rect});

		this.camera.cullingMask = Tools.visibleLayers;
		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "DoOnPreSceneGUICallbacks",
			BindingFlags.NonPublic | BindingFlags.Instance, this, new object[] {rect});
		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "PrepareCameraReplacementShader",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		controlId = GUIUtility.GetControlID(FocusType.Keyboard);
		if (current.GetTypeForControl(controlId) == EventType.MouseDown) {
			GUIUtility.keyboardControl = controlId;
		}
		bool flag = false;

		object[] params2 = {rect, flag};
		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "DoDrawCamera",
			BindingFlags.NonPublic | BindingFlags.Instance, this, params2);




		flag = (bool) params2[1];

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "CleanupCustomSceneLighting",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		useSceneFiltering = (bool) SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "UseSceneFiltering",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		if (!useSceneFiltering) {
			SceneViewFXReflections.RunMethod("UnityEditor.Handles", "DrawCameraStep2",
				BindingFlags.NonPublic | BindingFlags.Static, null, new object[] {camera, renderMode});
			#if !UNITY_5_4_OR_NEWER
		    SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "DoTonemapping",
				BindingFlags.NonPublic | BindingFlags.Instance, this);
		    #endif
			SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "HandleSelectionAndOnSceneGUI",
				BindingFlags.NonPublic | BindingFlags.Instance, this);
		}
//#if !UNITY_5_3
//        SceneViewFXReflections.RunMethod("UnityEditor.EditorUtility", "SetTemporarilyAllowIndieRenderTexture",
//			BindingFlags.NonPublic | BindingFlags.Static, null, new object[] {false});
//#endif
        if (current.type == EventType.ExecuteCommand || current.type == EventType.ValidateCommand) {
			SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "CommandsGUI",
				BindingFlags.NonPublic | BindingFlags.Instance, this);
		}

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "RestoreFogAndShadowDistance",
			BindingFlags.NonPublic | BindingFlags.Instance, this, new object[] {oldFog, oldShadowDistance});

		camera.renderingPath = renderingPath;

		if (useSceneFiltering) {
			SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
				BindingFlags.NonPublic | BindingFlags.Static,
				null, new object[] {Camera.current, 1});
		} else {
			SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
				BindingFlags.NonPublic | BindingFlags.Static,
				null, new object[] {Camera.current, 0});
		}

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "DefaultHandles",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		if (!useSceneFiltering) {
			if (current.type == EventType.Repaint) {
				Profiler.BeginSample("SceneView.BlitRT");
				Graphics.SetRenderTarget(null);
			}
			if (flag) {
				Assembly asm = Assembly.GetAssembly(typeof (MonoBehaviour));
				asm.GetType("UnityEngine.GUIClip")
					.GetMethod("Pop", BindingFlags.NonPublic | BindingFlags.Static)
					.Invoke(null, null);
			}
			if (current.type == EventType.Repaint) {
				GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
				RenderTexture targetTexture =
					(RenderTexture) SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_SceneTargetTexture",
						BindingFlags.Instance | BindingFlags.NonPublic, this);
#if UNITY_5_4_OR_NEWER
			    GUI.DrawTexture(rect, targetTexture , ScaleMode.StretchToFill, false);
#else
                RenderTexture targetTextureLDR =
					(RenderTexture) SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_SceneTargetTextureLDR",
						BindingFlags.Instance | BindingFlags.NonPublic, this);

				GUI.DrawTexture(rect, (!camera.hdr) ? targetTexture : targetTextureLDR, ScaleMode.StretchToFill, false);
#endif
				GL.sRGBWrite = false;
				Profiler.EndSample();
			}
		}
		SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
			BindingFlags.NonPublic | BindingFlags.Static,
			null, new object[] {Camera.current, 0});
		SceneViewFXReflections.RunMethod("UnityEditor.Handles", "SetCameraFilterMode",
			BindingFlags.NonPublic | BindingFlags.Static,
			null, new object[] {camera, 0});

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "HandleDragging",
			BindingFlags.NonPublic | BindingFlags.Instance, this);
		object svr = SceneViewFXReflections.GetField("UnityEditor.SceneView", "svRot",
			BindingFlags.NonPublic | BindingFlags.Instance, this);
		svr.GetType()
			.GetMethod("HandleContextClick", BindingFlags.NonPublic | BindingFlags.Instance)
			.Invoke(svr, new[] {this});
		svr.GetType().GetMethod("OnGUI", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(svr, new[] {this});

		if (lastActiveSceneView == this) {
			SceneViewFXReflections.RunMethod("UnityEditor.SceneViewMotion", "ArrowKeys",
				BindingFlags.Public | BindingFlags.Static, this, new object[] {this});
#if UNITY_5_0 || UNITY_5_1
			SceneViewFXReflections.RunMethod("UnityEditor.SceneViewMotion", "DoViewTool",
				BindingFlags.Public | BindingFlags.Static, this, new object[] {camera.transform, this});
#else
			SceneViewFXReflections.RunMethod("UnityEditor.SceneViewMotion", "DoViewTool",
				BindingFlags.Public | BindingFlags.Static, this, new object[] {this});
#endif
		}

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "Handle2DModeSwitch",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		GUI.EndGroup();
		GUI.color = color;

		svo.GetType().GetMethod("End", BindingFlags.Public | BindingFlags.Instance).Invoke(svo, null);

		SceneViewFXReflections.RunMethod("UnityEditor.SceneView", "HandleMouseCursor",
			BindingFlags.NonPublic | BindingFlags.Instance, this);

		if (current.type == EventType.Repaint) {
			Profiler.EndSample();
		}
        SceneViewFXReflections.SetField("UnityEditor.SceneView", "s_CurrentDrawingSceneView",
            BindingFlags.Static | BindingFlags.NonPublic, this, null);

#endif
    }


	private void PostProcessingUIAndEffects(Rect rect) {
		
		const float postFXButtonSize = 52;
		const float camListSize = 90;


		// If editor is playing keep effects off.
		if (Application.isPlaying) {
			GUI.enabled = false;
			GUILayout.BeginArea(rect);
			GUILayout.BeginHorizontal();
			GUILayout.Button(new GUIContent("Post FX Disabled", "Scene View effects disabled whilst in play mode"), EditorStyles.toolbarButton,
				GUILayout.Width(postFXButtonSize + camListSize));
			GUI.enabled = true;
			DrawFovGizmoButtons();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			return;
		}
		
		if (postFxCamera == null) ChangeCamera();

		// if there's no cams active keeps effects off.
		if (cams == null || cams.Length == 0) {
			ChangeCamera();
			GUI.enabled = false;
			GUILayout.BeginArea(rect);
			GUILayout.BeginHorizontal();
			GUILayout.Button(new GUIContent("Post FX Disabled", "No cameras found in the scene"), EditorStyles.toolbarButton,
				GUILayout.Width(postFXButtonSize + camListSize));
			GUI.enabled = true;
			DrawFovGizmoButtons();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			return;
		}

		// prevent effects when using search field
		if (useSceneFiltering || renderMode != DrawCameraMode.Textured) {
			filtering = true;
			GUI.enabled = false;
			GUILayout.BeginArea(rect);
			GUILayout.BeginHorizontal();
			GUILayout.Button(new GUIContent("Post FX Disabled", "Scene filtering enabled or render mode not set to 'Textured / Shaded'"), EditorStyles.toolbarButton,
				GUILayout.Width(postFXButtonSize + camListSize));
			GUI.enabled = true;
			DrawFovGizmoButtons();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			ClearCurrentEffects();
			return;
		}

		if (sceneViewOverlay == null) {
			CheckSceneViewOverlay();
			isDirty = true;
		}

		if (filtering) {
			filtering = false;
			TogglePostFx();
		}

		if (postFXOn && postFxCamera == null) {
			TogglePostFx();
		}

		if (isDirty) {
			isDirty = false;
			TogglePostFx();
			return;
		}

		bool prevPostFx = postFXOn;
		int prevSelection = currentSelection;

#if UNITY_5_4_OR_NEWER
	    if (postFXOn) {
	        var sceneViewState = SceneViewFXReflections.GetField("UnityEditor.SceneView", "m_SceneViewState", BindingFlags.NonPublic | BindingFlags.Instance, this);
	        SceneViewState svs = sceneViewState as SceneViewState;
	        svs.showImageEffects = true;
	    }
#endif

		controlId = GUIUtility.GetControlID(FocusType.Keyboard);
		if (Event.current.GetTypeForControl(controlId) == EventType.MouseDown) {
			GUIUtility.keyboardControl = controlId;
		}

		if (Event.current.type == EventType.KeyDown) {
			switch (Event.current.keyCode) {
				case shortcutGizmos:
					hideAllGizmos = !hideAllGizmos;
					Event.current.Use();
					break;
				case shortcutPostFx:
					postFXOn = !postFXOn;
					Event.current.Use();
					break;
			}
		}

		// Over-draw UI in the toolbar area.
		try {
			GUILayout.BeginArea(rect);
		} catch (ArgumentException) {
			return;
		}
		try {
			GUILayout.BeginHorizontal();
		} catch (ArgumentException) {
			GUILayout.EndArea();
			return;
		}

		postFXOn = GUILayout.Toggle(postFXOn, new GUIContent("Post FX  ", "Toggles the display of image effects in the scene view"), EditorStyles.toolbarButton,
			GUILayout.Width(postFXButtonSize));
		if (postFXOn != prevPostFx) TogglePostFx();
		if (GUILayout.Button(new GUIContent(EditorGUIUtility.isProSkin ? Icons.IconRefresh : Icons.IconRefreshPersonal, "Refresh list of cameras"), EditorStyles.toolbarButton)) {
			GetCamList();
		}
		currentSelection = EditorGUILayout.Popup(currentSelection, camList, EditorStyles.toolbarPopup,
			GUILayout.Width(camListSize));
		if (currentSelection != prevSelection) ChangeCamera();
		if (GUILayout.Button(new GUIContent(EditorGUIUtility.isProSkin ? Icons.IconGoto : Icons.IconGotoPersonal, "Select current camera"), EditorStyles.toolbarButton)) {
			if (postFxCamera != null) Selection.activeGameObject = postFxCamera.gameObject;
		}
		
		DrawFovGizmoButtons();

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void DrawFovGizmoButtons() {

		const float gizmoButtonWidth = 89;

		fovMatch = GUILayout.Toggle(fovMatch, new GUIContent("FOV", "Toggles FOV matching of selected camera"), EditorStyles.toolbarButton);
		bool previousGizmos = hideAllGizmos;
		hideAllGizmos = GUILayout.Toggle(hideAllGizmos, new GUIContent("Hide All Gizmos", "Toggles hiding / showing all the scene gizmos"), EditorStyles.toolbarButton,
			GUILayout.Width(gizmoButtonWidth));
		if (hideAllGizmos != previousGizmos) {
			ToggleGizmos(!hideAllGizmos);
		}
	}

#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
	private new void Update() {
		base.Update();
#else 
		private void Update() {
#endif
		if (Application.isPlaying) return;
		if (postFxCamera == null) return;
		// when camera is selected refresh components to show real time update.
		if (Selection.activeInstanceID == postFxCamera.gameObject.GetInstanceID() && postFXOn) {
			UpdateComponents();
			SetBackgroundColor(postFxCamera.backgroundColor);
		}
	}

	private Component[] GetComponents() {
		Component[] result = postFxCamera.GetComponents<Component>();
		if (result != null && result.Length > 0) {
			// exlude these components:
			List<Component> excludes = new List<Component>();
			excludes.Add(postFxCamera.transform);
			excludes.Add(postFxCamera);
			if (postFxCamera.GetComponent<GUILayer>()) excludes.Add(postFxCamera.GetComponent<GUILayer>());
			if (postFxCamera.GetComponent("FlareLayer")) excludes.Add(postFxCamera.GetComponent("FlareLayer"));
			if (postFxCamera.GetComponent<AudioListener>()) excludes.Add(postFxCamera.GetComponent<AudioListener>());
			if (postFxCamera.GetComponent("Tonemapping")) excludes.Add(postFxCamera.GetComponent("Tonemapping"));
			result = result.Except(excludes).ToArray();
		}
		return result;
	}

	// update scene view components
	public void UpdateComponents() {
#if UNITY_5_4_OR_NEWER
	    ComponentUtility.ReplaceComponentsIfDifferent(postFxCamera.gameObject, camera.gameObject, ComponentFilter);
#else
        ClearCurrentEffects();
		Component[] components = GetComponents();
	#if UNITY_5_3
	    bool hasFoundLDRAttribute = false;
	#endif
		if (components != null && components.Length > 0) {
			GameObject cameraGo = camera.gameObject;
			for (int i = 0; i < components.Length; i++) {
				Component c = components[i];
				if (c == null) continue;
				if (!((Behaviour)c).enabled) continue;

				Type cType = c.GetType();
			    Type check = c.GetType();

			    bool add = false;
			    while (check != null) {
                    var m = check.GetMethod("OnRenderImage", BindingFlags.NonPublic | BindingFlags.Instance);
			        if (m == null) m = check.GetMethod("OnRenderImage", BindingFlags.Public | BindingFlags.Instance);
			        if (m != null) {
    #if UNITY_5_3
			            if (!hasFoundLDRAttribute) {
			            var l = Attribute.GetCustomAttribute(m, typeof(ImageEffectTransformsToLDR));
			                if (l != null) {
			                    add = false;
			                    hasFoundLDRAttribute = true;
			                    break;
			                }
			            }
    #endif
			            add = true;
			            break;
			        }
                    check = check.BaseType;
                }
			    if (add) {
			        Component existing = cameraGo.AddComponent(cType);
			        EditorUtility.CopySerialized(c, existing);
			    }
			}
		}
#endif
		SetBackgroundColor(postFxCamera.backgroundColor);
		sceneCameraCleared = false;
	}

#if UNITY_5_4_OR_NEWER
    private bool ComponentFilter(Component c) {
        Type check = c.GetType();
        bool add = false;
        while (check != null) {
            var m = check.GetMethod("OnRenderImage", BindingFlags.NonPublic | BindingFlags.Instance);
            if (m == null) m = check.GetMethod("OnRenderImage", BindingFlags.Public | BindingFlags.Instance);
            if (m != null) {
                add = true;
                break;
            }
            check = check.BaseType;
        }
        return add;
    }
#endif

    public void ClearCurrentEffects() {
		if (sceneCameraCleared) return;
		Component[] compsOnCam = camera.GetComponents<Component>();
		for (int i = compsOnCam.Length - 1; i >= 0; i--) {
			// these components are default on the SceneView camera so do not destroy them
			if (camera.GetComponent("HaloLayer") == compsOnCam[i]) continue;
			if (camera.GetComponent("FlareLayer") == compsOnCam[i]) continue;
			if (compsOnCam[i] is Transform) continue;
			if (compsOnCam[i] is Camera) continue;
			DestroyImmediate(compsOnCam[i]);
		}
		SetBackgroundColor(Color.white, true);
		sceneCameraCleared = true;
	}

	private void TogglePostFx() {
		if (!postFXOn) {
			ClearCurrentEffects();
		} else {
			ChangeCamera();
			if (postFxCamera != null) {
				UpdateComponents();
			}
		}
	}

	private void ToggleGizmos(bool gizmosOn) {
		int val = gizmosOn ? 1 : 0;
		Assembly asm = Assembly.GetAssembly(typeof (Editor));
		Type type = asm.GetType("UnityEditor.AnnotationUtility");
		if (type != null) {
			MethodInfo getAnnotations = type.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo setGizmoEnabled = type.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo setIconEnabled = type.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);
			IEnumerable annotations = (IEnumerable) getAnnotations.Invoke(null, null);
			foreach (object annotation in annotations) {
				Type annotationType = annotation.GetType();
				FieldInfo classIdField = annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
				FieldInfo scriptClassField = annotationType.GetField("scriptClass",
					BindingFlags.Public | BindingFlags.Instance);
				if (classIdField != null && scriptClassField != null) {
					int classId = (int) classIdField.GetValue(annotation);
					string scriptClass = (string) scriptClassField.GetValue(annotation);
					setGizmoEnabled.Invoke(null, new object[] {classId, scriptClass, val});
					setIconEnabled.Invoke(null, new object[] {classId, scriptClass, val});
				}
			}
		}
	}

	private void ChangeCamera() {
		GetCamList();
		if (cams.Length == 0) {
			postFxCamera = null;
			return;
		}
		if (currentSelection >= cams.Length) {
			currentSelection = cams.Length - 1;
		}
		postFxCamera = cams[currentSelection];

		if (postFXOn) {
			SetBackgroundColor(postFxCamera.backgroundColor);
			UpdateComponents();
		}
	}

	private void GetCamList() {
		List<Camera> camerasInScene = new List<Camera>();
		foreach (Camera c in FindObjectsOfType<Camera>()) {
			if (c.gameObject.hideFlags == HideFlags.NotEditable || c.gameObject.hideFlags == HideFlags.HideAndDontSave) continue;
			camerasInScene.Add(c);
		}
		cams = camerasInScene.ToArray();
		camList = new GUIContent[cams.Length];
		for (int i = 0; i < cams.Length; i++) {
			camList[i] = new GUIContent(cams[i].name, "Choose a camera for image effects / FOV matching");
		}
	}

	private void SetBackgroundColor(Color color, bool setDefault = false) {
		Assembly asm = Assembly.GetAssembly(typeof (Editor));
		Type type = asm.GetType("UnityEditor.SceneView");
		FieldInfo field = type.GetField("kSceneViewBackground", BindingFlags.NonPublic | BindingFlags.Static);
		if (field != null) {
			object obj = field.GetValue(null);
			if (obj != null) {
				Type t = obj.GetType();
				if (setDefault) {
					t.GetMethod("ResetToDefault", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(obj, null);
				} else {
					PropertyInfo f = t.GetProperty("Color", BindingFlags.Public | BindingFlags.Instance);
					f.SetValue(obj, color, null);
				}
				Repaint();
			}
		}
	}

	private void CheckSceneViewOverlay() {
		Assembly asm = Assembly.GetAssembly(typeof (Editor));
		Type sceneView = asm.GetType("UnityEditor.SceneView");
		FieldInfo overlayField = sceneView.GetField("m_SceneViewOverlay",
			BindingFlags.NonPublic | BindingFlags.Instance);
		if (overlayField != null) sceneViewOverlay = overlayField.GetValue(this);
	}
}