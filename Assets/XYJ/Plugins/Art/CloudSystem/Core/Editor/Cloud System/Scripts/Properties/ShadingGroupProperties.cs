//
// Edelweiss.CloudSystemEditor.ShadingGroupProperties.cs: Prefab functionality to serialize shading groups.
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

	public class ShadingGroupProperties {
		
		public SerializedProperty nameProperty;
		public SerializedProperty centerProperty;
		public SerializedProperty shapeColorProperty;
		
		public void Initialize <C, PD, CD> (SerializedObject a_SerializedCloudSystem, C a_CloudSystem, int a_ShadingGroupIndex)
			where C : Cloud <C, PD, CD>
			where PD : ParticleData <C, PD, CD>
			where CD : CreatorData <C, PD, CD>
		{
			string l_ShadingGroupPath = "shadingGroups.Array.data[" + a_ShadingGroupIndex + "]";
			nameProperty = a_SerializedCloudSystem.FindProperty (l_ShadingGroupPath + ".name");
			centerProperty = a_SerializedCloudSystem.FindProperty (l_ShadingGroupPath + ".center");
			shapeColorProperty = a_SerializedCloudSystem.FindProperty (l_ShadingGroupPath + ".shapeColor");
		}
	}
}