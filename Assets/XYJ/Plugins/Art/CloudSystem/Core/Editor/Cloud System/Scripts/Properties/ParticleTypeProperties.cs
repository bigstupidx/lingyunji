//
// Edelweiss.CloudSystemEditor.ParticleTypeProperties.cs: Prefab functionality to serialize particle types.
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

	public class ParticleTypeProperties {
		
		public SerializedProperty uvIndexProperty;
		public SerializedProperty minimumRotationProperty;
		public SerializedProperty maximumRotationProperty;
		public SerializedProperty randomRotationAtStartProperty;
		public SerializedProperty squaredProperty;
		public SerializedProperty minimumXSizeProperty;
		public SerializedProperty maximumXSizeProperty;
		public SerializedProperty minimumYSizeProperty;
		public SerializedProperty maximumYSizeProperty;
		public SerializedProperty creationProbabilityProperty;
		
		public void Initialize <C, PD, CD> (SerializedObject a_SerializedCloudSystem, C a_CloudSystem, int a_ParticleTypeGroupIndex, int a_ParticleTypeIndex)
			where C : Cloud <C, PD, CD>
			where PD : ParticleData <C, PD, CD>
			where CD : CreatorData <C, PD, CD>
		{
			string l_ParticleTypePath = "particleGroups.Array.data[" + a_ParticleTypeGroupIndex + "].particleTypes.Array.data[" + a_ParticleTypeIndex + "]";
			uvIndexProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".uvIndex");		
			minimumRotationProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".minimumRotation");
			maximumRotationProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".maximumRotation");
			randomRotationAtStartProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".randomRotationAtStart");
			squaredProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".squared");
			minimumXSizeProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".minimumXSize");
			maximumXSizeProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".maximumXSize");
			minimumYSizeProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".minimumYSize");
			maximumYSizeProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".maximumYSize");
			creationProbabilityProperty = a_SerializedCloudSystem.FindProperty (l_ParticleTypePath + ".creationProbability");
		}
	}
}
