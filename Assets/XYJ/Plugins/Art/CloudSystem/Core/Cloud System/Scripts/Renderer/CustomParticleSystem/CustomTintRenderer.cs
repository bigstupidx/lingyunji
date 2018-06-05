//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using System.Threading;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Generic base class of <see cref="T:CS_CustomTintRenderer"/>.
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
	public abstract class CustomTintRenderer <C, PD, CD> : CustomCloudRenderer <C, PD, CD>
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
				return (true);
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
		protected override void UpdateMesh (object a_State) {
			CustomRendererArguments l_Arguments = (CustomRendererArguments) a_State;
			int l_LeftIndex = l_Arguments.leftIndex;
			int l_RightIndex = l_Arguments.rightIndex;
			AutoResetEvent l_AutoResetEvent = l_Arguments.autoResetEvent;
			
			for (int i = l_LeftIndex; i < l_RightIndex; i = i + 1) {
				CloudParticle l_Particle = m_Particles [i];
				
					// Calculate x and y translations for the vertices
				Vector3 l_xTranslation;
				Vector3 l_yTranslation;
				if (l_Particle.isSquared) {
					float l_halfSize = l_Particle.size.x * 0.5f;
					l_xTranslation = new Vector3 (l_halfSize, 0.0f, 0.0f);
					l_yTranslation = new Vector3 (0.0f, l_halfSize, 0.0f);
				} else {
					float l_HalfWidth = l_Particle.size.x * 0.5f;
					float l_HalfHeight = l_Particle.size.y * 0.5f;
					l_xTranslation = new Vector3 (l_HalfWidth, 0.0f, 0.0f);
					l_yTranslation = new Vector3 (0.0f, l_HalfHeight, 0.0f);
				}
				
					// Take the z rotation into account
				Quaternion l_Rotation = Quaternion.Euler (0.0f, 0.0f, - l_Particle.rotation);
				Matrix4x4 l_RotationMatrix = Matrix4x4.TRS (Vector3.zero, l_Rotation, Vector3.one);		
				l_xTranslation = l_RotationMatrix.MultiplyVector (l_xTranslation);
				l_yTranslation = l_RotationMatrix.MultiplyVector (l_yTranslation);
				
					// Bring them to camera space, but be sure to
					// take the CloudSystem roation into account
				l_xTranslation = m_ParticleToCameraSpaceMatrix.MultiplyVector (l_xTranslation);
				l_yTranslation = m_ParticleToCameraSpaceMatrix.MultiplyVector (l_yTranslation);
				
					// Calculate and apply the position of all the vertices
				Vector3 l_Position1 = l_Particle.position - l_xTranslation + l_yTranslation;
				Vector3 l_Position2 = l_Particle.position - l_xTranslation - l_yTranslation;
				Vector3 l_Position3 = l_Particle.position + l_xTranslation + l_yTranslation;
				Vector3 l_Position4 = l_Particle.position + l_xTranslation - l_yTranslation;
				
				int l_Index = i * 4;
				m_Vertices [l_Index + 0] = l_Position1;
				m_Vertices [l_Index + 1] = l_Position2;
				m_Vertices [l_Index + 2] = l_Position3;
				m_Vertices [l_Index + 3] = l_Position4;
				
					// Update UVs
				m_UVs [l_Index + 0] = m_UVLookupTable [l_Particle.uvIndex, 0];
				m_UVs [l_Index + 1] = m_UVLookupTable [l_Particle.uvIndex, 1];
				m_UVs [l_Index + 2] = m_UVLookupTable [l_Particle.uvIndex, 2];
				m_UVs [l_Index + 3] = m_UVLookupTable [l_Particle.uvIndex, 3];
					
				
					// Mix the colors
				
				Color l_VertexColor = m_CloudRendererData.Tint;
				if (l_Particle.isCoreParticle) {
					l_VertexColor.a = l_VertexColor.a * m_CloudRendererData.CoreParticleTransparency;
				} else {
					l_VertexColor.a = l_VertexColor.a * m_CloudRendererData.NonCoreParticleTransparency;
				}
				
				
					// Color
				
				if (useParticleColor) {
					l_VertexColor = l_VertexColor * l_Particle.particleColor;
				}
				m_VertexColors [l_Index + 0] = l_VertexColor;
				m_VertexColors [l_Index + 1] = l_VertexColor;
				m_VertexColors [l_Index + 2] = l_VertexColor;
				m_VertexColors [l_Index + 3] = l_VertexColor;
			}
			
			l_AutoResetEvent.Set ();
		}
	}
}