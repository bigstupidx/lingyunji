//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Generic base class of <see cref="T:CS_ShurikenShadingGroupRenderer"/>.
	/// This class was created to preserve the compatibility with clouds created before the Cloud System dll's were used.
	/// </summary>
	/// <typeparam name="C">
	/// The cloud's type.
	/// </typeparam>
	/// <typeparam name="PD">
	/// The particle data's type.
	/// </typeparam>
	/// <typeparam name="CD">
	/// The creator data's type.
	/// </typeparam>
	public abstract class ShurikenShadingGroupRenderer <C, PD, CD> : ShurikenCloudRenderer <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		/// <inheritdoc/>
		public override bool IsSupportedInCloudSystemFree {
			get {
				return (true);
			}
		}
		
		/// <inheritdoc/>
		public override bool SupportsNonSquaredParticles {
			get {
				return (false);
			}
		}
		
		/// <inheritdoc/>
		public override bool SupportsVerticalColors {
			get {
				return (false);
			}
		}
		
		/// <inheritdoc/>
		public override bool SupportsShadingGroups {
			get {
				return (true);
			}
		}
		
		/// <inheritdoc/>
		public override bool SupportsTint {
			get {
				return (true);
			}
		}
		
		/// <inheritdoc/>
		protected override ParticleSystem.Particle ConvertParticle (CloudParticle a_Particle) {
			ParticleSystem.Particle l_Result = new ParticleSystem.Particle ();
			l_Result.position = a_Particle.position;
			l_Result.startSize = a_Particle.size.x;
			l_Result.rotation = a_Particle.rotation;
			l_Result.remainingLifetime = Cloud.TileCount + 1 - a_Particle.uvIndex;
			l_Result.startLifetime = Cloud.TileCount;
			
			CS_ShadingGroup l_ShadingGroup = Cloud.shadingGroups [a_Particle.shadingGroupIndex];
			Vector3 l_ShadingGroupCenter = l_ShadingGroup.scaledCenter;
			Vector3 l_NormalizedCenterToPosition = (a_Particle.position - l_ShadingGroupCenter).normalized;
			float l_ScalarProduct = Vector3.Dot (m_NormalizedSunDirection, l_NormalizedCenterToPosition);
				
			Color l_Color = Cloud.ShadingColorAt (l_ScalarProduct);
			
			if (a_Particle.isCoreParticle) {
				l_Color.a = l_Color.a * Cloud.CoreParticleTransparency;
			} else {
				l_Color.a = l_Color.a * Cloud.NonCoreParticleTransparency;
			}
			l_Color = l_Color * Cloud.Tint;
			if (useParticleColor) {
				l_Color = l_Color * a_Particle.particleColor;
			}
			l_Result.startColor = l_Color;
			
			return (l_Result);
		}
	}
}