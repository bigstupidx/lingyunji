//
// CloudSystemPrefs.cs: Static access to the preferences.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Edelweiss.CloudSystemEditor {
	
	public class CloudSystemPrefs {
	
		private const string c_CloudToolbarIndexKey = "CloudSystemToolbarIndex";
		public static int CloudToolbarIndex {
			get {
				return (EditorPrefs.GetInt (c_CloudToolbarIndexKey, 0));
			}
			set {
				int l_CurrentValue = CloudToolbarIndex;
				if (l_CurrentValue != value) {
					EditorPrefs.SetInt (c_CloudToolbarIndexKey, value);
				}
			}
		}
		
		private const string c_CloudCreatorFoldoutKey = "CloudSystemCloudCreatorFoldout";
		public static bool CloudCreatorFoldout {
			get {
				return (EditorPrefs.GetBool (c_CloudCreatorFoldoutKey, true));
			}
			set {
				EditorPrefs.SetBool (c_CloudCreatorFoldoutKey, value);
			}
		}
		
		private const string c_ShapeColorTypeIndexKey = "CloudSystemShapeModeShapeColorTypeIndex";
		public static ShapeColorTypeEnum ShapeColorType {
			get {
				return ((ShapeColorTypeEnum) EditorPrefs.GetInt (c_ShapeColorTypeIndexKey, 0));
			}
			set {
				if (ShapeColorType != value) {
					EditorPrefs.SetInt (c_ShapeColorTypeIndexKey, (int) value);
					SceneView.RepaintAll ();
				}
			}
		}
		
		#region Settings
		
		private const string c_AccelerateEditorKey = "CloudSystemAccelerateEditor";
		public static bool AccelerateEditor {
			get {
				return (UnityEditor.EditorPrefs.GetBool (c_AccelerateEditorKey, true));
			}
			set {
				UnityEditor.EditorPrefs.SetBool (c_AccelerateEditorKey, value);
			}
		}
		
		private const string c_TextureBackgroundColorRKey = "CloudSystemTextureBackgroundColorR";
		private const string c_TextureBackgroundColorGKey = "CloudSystemTextureBackgroundColorG";
		private const string c_TextureBackgroundColorBKey = "CloudSystemTextureBackgroundColorB";
		public static Color TextureBackgroundColor {
			get {
				Color l_Result;
				if (!EditorPrefs.HasKey (c_TextureBackgroundColorRKey)) {
					
						// Light blue for the preview texture background
					
					l_Result = new Color (0.0f, 0.0f, 0.5f);
				} else {
					l_Result = new Color (EditorPrefs.GetFloat (c_TextureBackgroundColorRKey),
					                      EditorPrefs.GetFloat (c_TextureBackgroundColorGKey),
					                      EditorPrefs.GetFloat (c_TextureBackgroundColorBKey));
				}
				return (l_Result);
			}
			set {
				if (TextureBackgroundColor != value) {
					EditorPrefs.SetFloat (c_TextureBackgroundColorRKey, value.r);
					EditorPrefs.SetFloat (c_TextureBackgroundColorGKey, value.g);
					EditorPrefs.SetFloat (c_TextureBackgroundColorBKey, value.b);
					
					CloudSystemPreviewTextures.UpdatePreviewTextures ();
				}
			}
		}
		
		private const string c_HighlightColorRKey = "CloudSystemSelectionColorR";
		private const string c_HighlightColorGKey = "CloudSystemSelectionColorG";
		private const string c_HighlightColorBKey = "CloudSystemSelectionColorB";
		private const string c_HighlightColorAKey = "CloudSystemSelectionColorA";
		public static Color HighlightColor {
			get {
				Color l_Result;
				if (!EditorPrefs.HasKey (c_HighlightColorRKey)) {
					l_Result = new Color (1.0f, 0.5f, 0.0f);
				} else {
					l_Result = new Color (EditorPrefs.GetFloat (c_HighlightColorRKey),
					                      EditorPrefs.GetFloat (c_HighlightColorGKey),
					                      EditorPrefs.GetFloat (c_HighlightColorBKey),
					                      EditorPrefs.GetFloat (c_HighlightColorAKey));
				}
				return (l_Result);
			}
			set {
				if (HighlightColor != value) {
					EditorPrefs.SetFloat (c_HighlightColorRKey, value.r);
					EditorPrefs.SetFloat (c_HighlightColorGKey, value.g);
					EditorPrefs.SetFloat (c_HighlightColorBKey, value.b);
					EditorPrefs.SetFloat (c_HighlightColorAKey, value.a);
					
					CloudSystemPreviewTextures.UpdatePreviewTextures ();
				}
			}
		}
		public const float c_SelectableCircleRadius = 0.075f;
		
		private const string c_ShowStatisticsKey = "CloudSystemShowStatistics";
		public static bool ShowStatistics {
			get {
				return (EditorPrefs.GetBool (c_ShowStatisticsKey, true));
			}
			set {
				EditorPrefs.SetBool (c_ShowStatisticsKey, value);
			}
		}
		
		public static void DefaultSettings () {
			EditorPrefs.DeleteKey (c_TextureBackgroundColorRKey);
			EditorPrefs.DeleteKey (c_TextureBackgroundColorGKey);
			EditorPrefs.DeleteKey (c_TextureBackgroundColorBKey);
			
			EditorPrefs.DeleteKey (c_HighlightColorRKey);
			EditorPrefs.DeleteKey (c_HighlightColorGKey);
			EditorPrefs.DeleteKey (c_HighlightColorBKey);
			EditorPrefs.DeleteKey (c_HighlightColorAKey);
			
			CloudSystemPreviewTextures.UpdatePreviewTextures ();
			
			EditorPrefs.DeleteKey (c_ShowStatisticsKey);
		}
		
		#endregion
	}
}
