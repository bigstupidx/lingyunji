// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace mset {
	public class EditorUtil {
		public static void RegisterUndo(UnityEngine.Object obj, string name) {
			#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				Undo.RegisterUndo(obj, name);
			#else
				Undo.RecordObject(obj, name);
			#endif
		}
		public static void RegisterUndo(UnityEngine.Object[] objs, string name) {
			#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				Undo.RegisterUndo(objs, name);
			#else
				Undo.RecordObjects(objs, name);	
			#endif
		}
		public static void RegisterCreatedObjectUndo(UnityEngine.Object obj, string name) {
			#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				Undo.RegisterCreatedObjectUndo(obj, name);
			#else
				Undo.RegisterCreatedObjectUndo(obj, name);
			#endif
		}
		public class GUILayout {
			public static Rect drawTexture( float width, float height, string label, Texture2D tex, bool blended ) { return drawTexture(0,0,width,height,label,tex,blended); }
			public static Rect drawTexture( float xoffset, float yoffset, float width, float height, string label, Texture2D tex, bool blended ) {
				Rect border = GUILayoutUtility.GetRect(width+2, height+2);
				border.width = width+2;
				border.x += xoffset;
				border.y += yoffset;
				UnityEngine.GUI.Box(border, label, "HelpBox");
				border.x++;
				border.y++;
				border.width-=2;
				border.height-=2;		
				if( tex != null ) UnityEngine.GUI.DrawTexture(border, tex, ScaleMode.StretchToFill, blended);
				return border;
			}
			public static bool tinyButton( float x, float y, string label, float label_x, float label_y ) {
				return tinyButton(x,y,label,"",label_x,label_y);
			}
			public static bool tinyButton( float x, float y, string label, string tip, float label_x, float label_y ) {
				Rect rect = new Rect(x,y,12,14);
				bool b = GUI.Button(rect,new GUIContent("",tip),"Toggle");
				rect.x += label_x;
				rect.y += label_y;
			#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2)
				rect.y -= 1;
			#endif
				GUI.Label(rect,label);
				return b;
			}
			public static bool tinyButton( float x, float y, Texture2D icon, string tip, float label_x, float label_y ) {
				Rect rect = new Rect(x,y,12,14);
				bool b = GUI.Button(rect,new GUIContent("",tip),"Toggle");
				rect.x += label_x;
				rect.y += label_y;
			#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2)
				rect.y -= 1;
			#endif
				rect.width = 16;
				rect.height = 14;
				if(icon) GUI.DrawTexture(rect,icon);
				return b;
			}
			public static bool tinyToggle( float x, float y, string label, float label_x, float label_y, bool val ) {
				return tinyToggle(x,y,label,"",label_x,label_y,val);
			}
			public static bool tinyToggle( float x, float y, string label, string tip, float label_x, float label_y, bool val ) {
				Rect rect = new Rect(x,y,12,14);
				GUIStyle style = new GUIStyle("Toggle");
				if( val ) style.normal = style.active;
				if( GUI.Button(rect,new GUIContent("",tip),style) ) val = !val;
				rect.x += label_x;
				rect.y += label_y;
				GUI.Label(rect,label);
				return val;
			}
		};
	};
}

