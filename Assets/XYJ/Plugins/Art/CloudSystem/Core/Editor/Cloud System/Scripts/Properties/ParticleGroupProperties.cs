//
// Edelweiss.CloudSystemEditor.ParticleGroupProperties.cs: Prefab functionality to serialize particle groups.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {

	public class ParticleGroupProperties {
		
		public SerializedProperty nameProperty;
		public SerializedProperty shapeColorProperty;
		
		public List <ParticleTypeProperties> particleTypesProperties = new List <ParticleTypeProperties> ();
		
		public void Initialize <C, PD, CD> (SerializedObject a_SerializedCloudSystem, C a_CloudSystem, int a_ParticleGroupIndex)
			where C : Cloud <C, PD, CD>
			where PD : ParticleData <C, PD, CD>
			where CD : CreatorData <C, PD, CD>
		{
			string l_ParticleGroupPath = "particleGroups.Array.data[" + a_ParticleGroupIndex + "]";
			nameProperty = a_SerializedCloudSystem.FindProperty (l_ParticleGroupPath + ".name");
			shapeColorProperty = a_SerializedCloudSystem.FindProperty (l_ParticleGroupPath + ".shapeColor");
			
			CS_ParticleGroup l_ParticleTypeGroup = a_CloudSystem.CreatorData.particleGroups [a_ParticleGroupIndex];
			for (int i = 0; i < l_ParticleTypeGroup.particleTypes.Length; i = i + 1) {
				ParticleTypeProperties l_ParticleTypeProperties = new ParticleTypeProperties ();
				l_ParticleTypeProperties.Initialize <C, PD, CD> (a_SerializedCloudSystem, a_CloudSystem, a_ParticleGroupIndex, i);
				particleTypesProperties.Add (l_ParticleTypeProperties);
			}
		}
	}
}
