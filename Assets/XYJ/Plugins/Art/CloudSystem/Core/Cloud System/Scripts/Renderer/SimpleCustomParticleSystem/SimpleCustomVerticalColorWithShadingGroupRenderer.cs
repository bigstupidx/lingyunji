//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using System.Threading;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Generic base class of <see cref="T:CS_SimpleCustomVerticalColorWithShadingGroupRenderer"/>.
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
	public abstract class SimpleCustomVerticalColorWithShadingGroupRenderer <C, PD, CD> : SimpleCustomCloudRenderer <C, PD, CD>
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
				return (true);
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
		protected override void UpdateParticleColors (object a_State) {
			CustomRendererArguments l_Arguments = (CustomRendererArguments) a_State;
			int l_LeftIndex = l_Arguments.leftIndex;
			int l_RightIndex = l_Arguments.rightIndex;
			AutoResetEvent l_AutoResetEvent = l_Arguments.autoResetEvent;
			
			for (int i = l_LeftIndex; i < l_RightIndex; i = i + 1) {
#if CLOUD_SYSTEM_FREE
				m_Particles [i].color = new Color (0.0f, 0.0f, 0.0f, 0.0f);
#else
				CloudParticle l_Particle = m_Particles [i].particle;
				
				float l_HeightFactor = m_CloudRendererData.HeightToVerticalFactor (l_Particle.position.y);
				CS_ShadingGroup l_ShadingGroup = m_CloudRendererData.shadingGroups [l_Particle.shadingGroupIndex];
				Vector3 l_ShadingGroupCenter = l_ShadingGroup.scaledCenter;
				Vector3 l_NormalizedCenterToPosition = (l_Particle.position - l_ShadingGroupCenter).normalized;
				float l_ScalarProduct = Vector3.Dot (m_NormalizedSunDirection, l_NormalizedCenterToPosition);
				
				Color l_Color = (m_CloudRendererData.VerticalShadingInfluence * m_CloudRendererData.VerticalColorAt (l_HeightFactor)) + (m_CloudRendererData.ShadingGroupInfluence * m_CloudRendererData.ShadingColorAt (l_ScalarProduct));
				
				if (l_Particle.isCoreParticle) {
					l_Color.a = l_Color.a * m_CloudRendererData.CoreParticleTransparency;
				} else {
					l_Color.a = l_Color.a * m_CloudRendererData.NonCoreParticleTransparency;
				}
				l_Color = l_Color * m_CloudRendererData.Tint;
				if (useParticleColor) {
					l_Color = l_Color * l_Particle.particleColor;
				}
				m_Particles [i].color = l_Color;
#endif
			}
			l_AutoResetEvent.Set ();
		}
	}
}