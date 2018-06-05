//
// Edelweiss.CloudSystemEditor.CloudCreator.cs: Cloud creator implementation.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {

	public class CloudCreator {
	
		public static void CreateCloud <C, PD, CD> (C a_Cloud)
			where C : Cloud <C, PD, CD>
			where PD : ParticleData <C, PD, CD>
			where CD : CreatorData <C, PD, CD>
		{
				// Create particles
			
			int l_Identification = 1;
			Dictionary <int, CloudParticle> l_ParticleTable = new Dictionary <int, CloudParticle> ();
			List <string> l_Warnings = new List <string> ();
			
			for (int l_ShapeIndex = 0; l_ShapeIndex < a_Cloud.CreatorData.boxShapes.Length; l_ShapeIndex = l_ShapeIndex + 1) {
				CS_Shape l_BoxShape = a_Cloud.CreatorData.boxShapes [l_ShapeIndex];
				Matrix4x4 l_BoxShapeMatrix = l_BoxShape.LocalTransformMatrix ();
				
				CS_ParticleGroup l_ParticleGroup = a_Cloud.CreatorData.particleGroups [l_BoxShape.particleGroupIndex];
				float l_TotalParticleAmount = l_BoxShape.particleCount;
				
				for (int l_ParticleTypeIndex = 0; l_ParticleTypeIndex < l_ParticleGroup.particleTypes.Length; l_ParticleTypeIndex = l_ParticleTypeIndex + 1) {
					CS_ParticleType l_ParticleType = l_ParticleGroup.particleTypes [l_ParticleTypeIndex];
					int l_ParticleAmount = Mathf.RoundToInt (l_TotalParticleAmount * l_ParticleType.creationProbability);
					
					int l_TooBigParticle = 0;
					for (int i = 0; i < l_ParticleAmount; i = i + 1) {
						float l_ParticleSizeX = Random.Range (l_ParticleType.minimumXSize, l_ParticleType.maximumXSize);
						float l_ParticleSizeY = Random.Range (l_ParticleType.minimumYSize, l_ParticleType.maximumYSize);
						
						Vector3 l_ParticleSpaceX = l_BoxShape.scale;
						l_ParticleSpaceX.x = 0.5f * (l_ParticleSpaceX.x - l_ParticleSizeX);
						l_ParticleSpaceX.y = 0.5f * (l_ParticleSpaceX.y - l_ParticleSizeX);
						l_ParticleSpaceX.z = 0.5f * (l_ParticleSpaceX.z - l_ParticleSizeX);
						
						Vector3 l_ParticleSpaceY = l_BoxShape.scale;
						l_ParticleSpaceY.x = 0.5f * (l_ParticleSpaceY.x - l_ParticleSizeY);
						l_ParticleSpaceY.y = 0.5f * (l_ParticleSpaceY.y - l_ParticleSizeY);
						l_ParticleSpaceY.z = 0.5f * (l_ParticleSpaceY.z - l_ParticleSizeY);
						
						if
							(IsParticleSpacePositive (l_ParticleSpaceX) &&
							 (!l_ParticleType.squared || IsParticleSpacePositive (l_ParticleSpaceY)))
						{
							Vector3 l_Position = RandomPosition (l_ParticleSpaceX);
							l_Position = l_BoxShapeMatrix.MultiplyPoint3x4 (l_Position);
							
							CloudParticle l_Particle = new CloudParticle ();
							l_Particle.position = l_Position;
							l_Particle.uvIndex = l_ParticleType.uvIndex;
							l_Particle.rotation = Random.Range (l_ParticleType.minimumRotation, l_ParticleType.maximumRotation);
							l_Particle.shadingGroupIndex = l_BoxShape.shadingGroupIndex;
							
							l_Particle.size.x = l_ParticleSizeX;
							l_Particle.isSquared = l_ParticleType.squared;
							if (l_Particle.isSquared) {
								l_Particle.size.y = l_ParticleSizeX;
							} else {
								l_Particle.size.y = l_ParticleSizeY;
							}
	
							l_ParticleTable.Add (l_Identification, l_Particle);
							l_Identification = l_Identification + 1;
						} else {
							l_TooBigParticle = l_TooBigParticle + 1;
						}
					}
					
					if (l_TooBigParticle * 2 > l_ParticleAmount) {
						l_Warnings.Add ("Most of the particles for the shape with index " + l_ShapeIndex + " that are based on the particle type at position " + (l_ParticleTypeIndex + 1) + " from \"" + l_ParticleGroup.name + "\" could not be created, because they don't fit into that shape.");
					}
				}
			}
			
			
				// Eliminate particles
			
				// Find all pairs that are too close to each other
			
			float l_EliminationDistanceFactor = (-a_Cloud.CreatorData.density) + 1.0f;
			
			List <int> l_ParticleList1 = new List <int> ();
			List <int> l_ParticleList2 = new List <int> ();
			
			for (int i = 1; i <= l_ParticleTable.Count; i = i + 1) {
				CloudParticle l_ParticleI = l_ParticleTable [i];
				float l_RadiusI = l_ParticleI.Radius ();
				
				for (int j = i + 1; j <= l_ParticleTable.Count; j = j + 1) {
					CloudParticle l_ParticleJ = l_ParticleTable [j];
					float l_RadiusJ = l_ParticleJ.Radius ();
					
					float l_Radius = Mathf.Min (l_RadiusI, l_RadiusJ);
					float l_WantedDistance = l_EliminationDistanceFactor * l_Radius;
					float l_Distance = Vector3.Distance (l_ParticleI.position, l_ParticleJ.position);
					if (l_Distance < l_WantedDistance) {
						l_ParticleList1.Add (i);
						l_ParticleList2.Add (j);
					}
				}
			}
			
			
				// Now remove all pairs in a random order. This handles all particles more equally which
				// results in a better selection.
				// The probability for each particle in the selected pair is equal to be eliminated. As
				// we already have the wanted distribution from the generation of the particles, we
				// will still have it if all particles are handled equally.
			
			while (l_ParticleList1.Count != 0) {
				int l_Index = Random.Range (0, l_ParticleList1.Count);
				int l_Key1 = l_ParticleList1 [l_Index];
				int l_Key2 = l_ParticleList2 [l_Index];
				
				if
					(l_ParticleTable.ContainsKey (l_Key1) &&
					 l_ParticleTable.ContainsKey (l_Key2))
				{
					if (Random.Range (0.0f, 1.0f) < 0.5f) {
						l_ParticleTable.Remove (l_Key1);
					} else {
						l_ParticleTable.Remove (l_Key2);
					}
				}
				
				l_ParticleList1.RemoveAt (l_Index);
				l_ParticleList2.RemoveAt (l_Index);
			}
	
			
				// Pass particles to cloud
			
			a_Cloud.ParticleData.Clear ();
			foreach (CloudParticle l_Particle in l_ParticleTable.Values) {
				a_Cloud.ParticleData.Add (l_Particle);
			}
		
			
				// Calculate the shading group's centers
			
			for (int i = 0; i < a_Cloud.shadingGroups.Length; i = i + 1) {
				CS_ShadingGroup l_ShadingGroup = a_Cloud.shadingGroups [i];
				l_ShadingGroup.center = Vector3.zero;
				
				int l_ParticlesOfShadingGroup = 0;
				for (int j = 0; j < a_Cloud.ParticleData.Count; j = j + 1) {
					CloudParticle l_Particle = a_Cloud.ParticleData [j];
					if (l_Particle.shadingGroupIndex == i) {
						l_ParticlesOfShadingGroup = l_ParticlesOfShadingGroup + 1;
						
						l_ShadingGroup.center = l_ShadingGroup.center + l_Particle.position;
					}
				}
				
				if (l_ParticlesOfShadingGroup != 0) {
					l_ShadingGroup.center = l_ShadingGroup.center / l_ParticlesOfShadingGroup;
				}
			}
			
				// Fading
			
			List <float> l_ParticleDistance = new List <float> ();
			float l_MaximumDistance = 0.0f;
			for (int i = 0; i < a_Cloud.ParticleData.Count; i = i + 1) {
				Vector3 l_Position = a_Cloud.ParticleData [i].position;
				float l_Distance = Vector3.Magnitude (l_Position);
				l_ParticleDistance.Add (l_Distance);
				
				if (l_Distance > l_MaximumDistance) {
					l_MaximumDistance = l_Distance;
				}
			}
			
			float l_CoreDistance = 0.5f * l_MaximumDistance;
			for (int i = 0; i < a_Cloud.ParticleData.Count; i = i + 1) {
				CloudParticle l_Particle = a_Cloud.ParticleData [i];
				if (l_ParticleDistance [i] < l_CoreDistance) {
					l_Particle.isCoreParticle = true;
				} else {
					l_Particle.isCoreParticle = false;
				}
				a_Cloud.ParticleData [i] = l_Particle;
			}
			
			if (l_Warnings.Count > 0) {
				string l_WarningString = "You need to either enlarge the shape or lower the particle size." + '\n';
				foreach (string l_Warning in l_Warnings) {
					l_WarningString = l_WarningString + l_Warning + '\n';
				}
				EditorUtility.DisplayDialog ("Warning: Particles don't fit into shape", l_WarningString, "Ok");
			}
			
			if (a_Cloud.CreatorData.boxShapes.Length == 0) {
				EditorUtility.DisplayDialog ("Warning: No shape", "There is no shape for which particles could be created!", "Ok");
			}
		}
		
		private static bool IsParticleSpacePositive (Vector3 a_ParticleSpace) {
			return (a_ParticleSpace.x > 0.0f && a_ParticleSpace.y > 0.0f && a_ParticleSpace.z > 0.0f);
		}
		
		private static Vector3 RandomPosition (Vector3 a_ParticleSpace) {
			Vector3 l_Result;
			l_Result.x = Random.Range (- a_ParticleSpace.x, a_ParticleSpace.x);
			l_Result.y = Random.Range (- a_ParticleSpace.y, a_ParticleSpace.y);
			l_Result.z = Random.Range (- a_ParticleSpace.z, a_ParticleSpace.z);
			return (l_Result);
		}
	}
}
