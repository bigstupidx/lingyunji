//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

#pragma warning disable

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Generic base class of <see cref="T:CS_UnityParticleSystemVerticalColorRenderer"/>.
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
	public abstract class UnityParticleSystemVerticalColorRenderer <C, PD, CD> : UnityParticleSystemCloudRenderer <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		/// <inheritdoc/>
		public override bool IsSupportedInCloudSystemFree {
			get {
				return (false);
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
				return (true);
			}
		}
		
		/// <inheritdoc/>
		public override bool SupportsShadingGroups {
			get {
				return (false);
			}
		}
		
		/// <inheritdoc/>
		public override bool SupportsTint {
			get {
				return (true);
			}
		}
		
		/// <inheritdoc/>
		protected override Particle ConvertParticle (CloudParticle a_Particle) {
			Particle l_Result = new Particle ();
			l_Result.position = a_Particle.position;
			l_Result.size = a_Particle.size.x;
			l_Result.rotation = a_Particle.rotation;
			l_Result.energy = Cloud.TileCount + 1 - a_Particle.uvIndex;
			l_Result.startEnergy = Cloud.TileCount;
			
#if CLOUD_SYSTEM_FREE
			l_Result.color = new Color (0.0f, 0.0f, 0.0f, 0.0f);
#else
			float l_HeightFactor = Cloud.HeightToVerticalFactor (a_Particle.position.y);
			Color l_Color = Cloud.VerticalColorAt (l_HeightFactor);
			if (a_Particle.isCoreParticle) {
				l_Color.a = l_Color.a * Cloud.CoreParticleTransparency;
			} else {
				l_Color.a = l_Color.a * Cloud.NonCoreParticleTransparency;
			}
			l_Color = l_Color * Cloud.Tint;
			if (useParticleColor) {
				l_Color = l_Color * a_Particle.particleColor;
			}
			l_Result.color = l_Color;
#endif
			
			return (l_Result);
		}
	}
}
