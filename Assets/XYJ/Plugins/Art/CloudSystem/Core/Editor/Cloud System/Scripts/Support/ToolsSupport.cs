//
// Edelweiss.CloudSystemEditor.ToolsSupport.cs: Unity transform tool manipulation.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

namespace Edelweiss.CloudSystemEditor {
	
	public class ToolsSupport {
	
		public static TransformTool Current {
			get {
				TransformTool l_Result = (TransformTool) Tools.current;
				return (l_Result);
			}
			set {
				Tools.current = (Tool) value;
			}
		}
		
		public static PivotRotationTool CurrentPivotRotation {
			get {
				PivotRotationTool l_Result;
				if (Tools.pivotRotation == PivotRotation.Local) {
					l_Result = PivotRotationTool.Local;
				} else {
					l_Result = PivotRotationTool.Global;
				}
				return (l_Result);
			}
			set {
				PivotRotation l_PivotRotation;
				if (value == PivotRotationTool.Local) {
					l_PivotRotation = PivotRotation.Local;
				} else {
					l_PivotRotation = PivotRotation.Global;
				}
				Tools.pivotRotation = l_PivotRotation;
			}
		}
		
		public static bool Hidden {
			get {
				bool l_Result;
				Type l_ToolsType = typeof (Tools);
				FieldInfo l_HiddenField = l_ToolsType.GetField ("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
				l_Result = (bool) l_HiddenField.GetValue (null);
				
				return (l_Result);
			}
			set {
				Type l_ToolsType = typeof (Tools);
				FieldInfo l_HiddenField = l_ToolsType.GetField ("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
				l_HiddenField.SetValue (null, value);
			}
		}
	}
}
