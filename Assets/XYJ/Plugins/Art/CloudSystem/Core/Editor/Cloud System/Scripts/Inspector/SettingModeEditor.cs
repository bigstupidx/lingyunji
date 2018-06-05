//
// Edelweiss.CloudSystemEditor.SettingModeEditor.cs: Inspector view for the setting mode.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEditor;
using UnityEngine;
using System.Collections;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {

	public class SettingModeEditor {
	
		private GUIContent m_AccelerateEditorLabel;
		private GUIContent m_BackgroundColorLabel;
		private GUIContent m_HighlightColorLabel;
		private GUIContent m_StatisticsLabel;
		private GUIContent m_UseDefaultsButton;
		
		public void Initialize () {
			m_AccelerateEditorLabel = new GUIContent ("Accelerate", "Increase the editor performance with the drawback that undo operations are only visible for the selected cloud.");
			m_BackgroundColorLabel = new GUIContent ("Background", "Background color");
			m_HighlightColorLabel = new GUIContent ("Highlight", "Highlight color");
			m_StatisticsLabel = new GUIContent ("Statistics", "Show statistics");
			
			m_UseDefaultsButton = new GUIContent ("Use Defaults", "Reset the values to the default ones.");
		}
		
		public void InspectorGUI () {
			EditorGUILayout.Space ();
			InspectorSupport.Explanation ("Settings", "Modify the editor configurations.", null);
	
			EditorGUILayout.Space ();
			
			CloudSystemPrefs.AccelerateEditor = EditorGUILayout.Toggle (m_AccelerateEditorLabel, CloudSystemPrefs.AccelerateEditor);
			CloudSystemPrefs.TextureBackgroundColor = EditorGUILayout.ColorField (m_BackgroundColorLabel, CloudSystemPrefs.TextureBackgroundColor);
			CloudSystemPrefs.HighlightColor = EditorGUILayout.ColorField (m_HighlightColorLabel, CloudSystemPrefs.HighlightColor);
			CloudSystemPrefs.ShowStatistics = EditorGUILayout.Toggle (m_StatisticsLabel, CloudSystemPrefs.ShowStatistics);
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button (m_UseDefaultsButton)) {
				CloudSystemPrefs.DefaultSettings ();
			}
			EditorGUILayout.EndHorizontal ();
		}
		
		public void SceneGUI () {
		}
	}
}
