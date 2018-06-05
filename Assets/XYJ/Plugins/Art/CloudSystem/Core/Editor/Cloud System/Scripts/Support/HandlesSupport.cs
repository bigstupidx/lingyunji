//
// HandlesSupport.cs: Workaround for a Unity bug. Handles can not just be used, as the scene view may freeze.
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
	public class HandlesSupport {
		
//		public static bool IsHandleDrawingSave () {
//				
//				// TODO:
//				// There is one more missing case. As the mouse is over a handle
//				// and one first holds down the middle mouse button and then the
//				// left one, the scene view still freezes.
//			
//			bool l_Result = true;
//			
//			if
//				(Event.current.type == EventType.MouseDown &&
//				Event.current.button == 0 &&
//				GUIUtility.hotControl != 0)
//			{
//				if (Tools.viewTool != ViewTool.Pan) {
//					l_Result = false;
//				}
//			}
//			
//			return (l_Result);
//		}
	}
}
