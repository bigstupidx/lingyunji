//
// CloudSystemPreviewTextures.cs: Static access to the preview textures with a workaround to avoid leaked textures warnings.
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
	
	public class CloudSystemPreviewTextures : System.IDisposable {
		
		private const string c_PreviewTexturePath = "Assets/Art/ThirdPartyPlugins/CloudSystem/Core/Editor/Cloud System/Res/PreviewTextures/";
		
		private const string c_BackgroundName = "background.png";
		private const string c_HighlightName = "highlight.png";
		
		private static Texture2D m_PreviewTextureBackground;
		public static GUIStyle PreviewTextureBackgroundStyle {
			get {
				if (m_PreviewTextureBackground == null) {
					UpdatePreviewTextures ();
				}
				GUIStyle l_Result = new GUIStyle (EditorStyles.label);
				l_Result.normal.background = m_PreviewTextureBackground;
	
				return (l_Result);
			}
		}
		
		private static Texture2D m_PreviewTextureHighlight;
		public static GUIStyle PreviewTextureHighlightStyle {
			get {
				if (m_PreviewTextureHighlight == null) {
					UpdatePreviewTextures ();
				}
				GUIStyle l_Result = new GUIStyle (EditorStyles.label);
				l_Result.normal.background = m_PreviewTextureHighlight;
				
				return (l_Result);
			}
		}
		
		public static void UpdatePreviewTextures () {
			if (m_PreviewTextureBackground == null) {
				m_PreviewTextureBackground = IconLoader.LoadIcon (c_PreviewTexturePath, c_BackgroundName);
			}
			for (int i = 0; i < m_PreviewTextureBackground.width; i = i + 1) {
				for (int j = 0; j < m_PreviewTextureBackground.height; j = j + 1) {
					m_PreviewTextureBackground.SetPixel (i, j, CloudSystemPrefs.TextureBackgroundColor);
				}
			}
			m_PreviewTextureBackground.Apply ();
			
			if (m_PreviewTextureHighlight == null) {
				m_PreviewTextureHighlight = IconLoader.LoadIcon (c_PreviewTexturePath, c_HighlightName);
			}
			for (int i = 0; i < m_PreviewTextureHighlight.width; i = i + 1) {
				for (int j = 0; j < m_PreviewTextureHighlight.height; j = j + 1) {
					m_PreviewTextureHighlight.SetPixel (i, j, CloudSystemPrefs.HighlightColor);
				}
			}
			m_PreviewTextureHighlight.Apply ();
		}

		public void Dispose () {
			if (IconLoader.IsIconFromAssembly (c_BackgroundName)) {
				Object.DestroyImmediate (m_PreviewTextureBackground);
			}
			if (IconLoader.IsIconFromAssembly (c_HighlightName)) {
				Object.DestroyImmediate (m_PreviewTextureHighlight);
			}
		}
	}
}
