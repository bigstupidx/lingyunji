//
// Edelweiss.CloudSystemEditor.InspectorSupport.cs: Frequently used inspector funtions for explanations, warnings and errors.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Edelweiss.CloudSystemEditor {

	public class InspectorSupport {
	
		public static void Explanation (string a_Title, string a_Text, string a_ShortCuts) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			if (a_Title != null && a_Title != "") {
				GUILayout.Label (a_Title, EditorStyles.boldLabel);
			}
			if (a_Text != null && a_Text != "") {
				GUILayout.Label (a_Text, EditorStyles.wordWrappedLabel);
			}
			if (a_ShortCuts != null && a_ShortCuts != "") {
				GUILayout.Label (a_ShortCuts, EditorStyles.miniLabel);
			}
			EditorGUILayout.EndVertical ();
		}
		
		public static void Warning (string a_Title, string a_Text) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			GUIStyle l_ErrorStyle = new GUIStyle (EditorStyles.boldLabel);
			l_ErrorStyle.normal.textColor = new Color (1.0f, 0.5f, 0.0f);
			if (a_Title != null && a_Title != "") {
				GUILayout.Label (a_Title, l_ErrorStyle);
			}
			
			if (a_Text != null && a_Text != "") {
				GUILayout.Label (a_Text, EditorStyles.wordWrappedLabel);
			}
			EditorGUILayout.EndVertical ();
		}
		
		public static void Error (string a_Title, string a_Text) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			GUIStyle l_ErrorStyle = new GUIStyle (EditorStyles.boldLabel);
			l_ErrorStyle.normal.textColor = Color.red;
			if (a_Title != null && a_Title != "") {
				GUILayout.Label (a_Title, l_ErrorStyle);
			}
			
			if (a_Text != null && a_Text != "") {
				GUILayout.Label (a_Text, EditorStyles.wordWrappedLabel);
			}
			EditorGUILayout.EndVertical ();
		}
	}
}
