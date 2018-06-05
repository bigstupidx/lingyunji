//
// Edelweiss.CloudSystemEditor.VerticalColorProperties.cs: Prefab functionality to serialize vertical colors.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using UnityEditor;
using System.Collections;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {

	public class VerticalColorProperties {
		
		public SerializedProperty verticalFactorProperty;
		public SerializedProperty colorProperty;
		
		public void Initialize <C, PD, CD> (SerializedObject a_SerializedCloudSystem, C a_CloudSystem, int a_VerticalColorIndex)
			where C : Cloud <C, PD, CD>
			where PD : ParticleData <C, PD, CD>
			where CD : CreatorData <C, PD, CD>
		{
			string l_VerticalColorPath = "verticalColors.Array.data[" + a_VerticalColorIndex + "]";
			verticalFactorProperty = a_SerializedCloudSystem.FindProperty (l_VerticalColorPath + ".verticalFactor");
			colorProperty = a_SerializedCloudSystem.FindProperty (l_VerticalColorPath + ".color");
		}
	}
}
