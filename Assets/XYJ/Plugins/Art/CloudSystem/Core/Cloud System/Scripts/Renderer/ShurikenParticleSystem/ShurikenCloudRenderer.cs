//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012-2013 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
#pragma warning disable

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Abstract definition for a Shuriken particle system cloud renderer.
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
	/// <exception cref='System.InvalidOperationException'>
	/// Is thrown when an editor only operation is called while the application is playing.
	/// </exception>
	public abstract class ShurikenCloudRenderer <C, PD, CD> : CloudParticleRenderer <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		[SerializeField][HideInInspector] private ParticleSystem m_ParticleSystem;
		[SerializeField][HideInInspector] private ParticleSystemRenderer m_ParticleSystemRenderer;
		
		private ParticleSystem.Particle[] m_ShurikenParticles = new ParticleSystem.Particle [0];
		
		/// <inheritdoc/>
		public override void OnEnable () {
			m_HasParticleSystemChanged = true;
			
			if (Application.isPlaying) {
				
					// We don't want the animation to be played, but still need the
					// initial animation. Simulate updates everything and then
					// stops the animation.
				m_ParticleSystem.Simulate (0.0f);
			}
		}
		
		/// <inheritdoc/>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public override void InitializeRendererComponents () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
			m_ParticleSystem = GetComponent <ParticleSystem> ();
			if (m_ParticleSystem == null) {
				m_ParticleSystem = gameObject.AddComponent <ParticleSystem> ();
			}
			m_ParticleSystem.enableEmission = false;
			
			m_ParticleSystemRenderer = GetComponent <ParticleSystemRenderer> ();
			if (m_ParticleSystemRenderer == null) {
				m_ParticleSystemRenderer = gameObject.AddComponent <ParticleSystemRenderer> ();
			}
			
			ChangedMaterial ();
			m_ParticleSystemRenderer.renderMode = ParticleSystemRenderMode.Billboard;
			
				// Remark:
				// Sort mode and tiling are both set in the
				// editor code as the api is not exposed.
			
			m_ParticleSystemRenderer.maxParticleSize = Mathf.Infinity;
			m_ParticleSystemRenderer.receiveShadows = false;
			
			HideAuxiliaryComponents ();
			
				// HACK:
				// Workaround for Unity bug (Case 414572)
			Cloud.CachedTransform.localScale = Cloud.MeshScale;
		}
		
		/// <inheritdoc/>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public override void DestroyRendererComponents () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
			if (m_ParticleSystem != null) {
				DestroyImmediate (m_ParticleSystem);
			}
			if (m_ParticleSystemRenderer != null) {
				DestroyImmediate (m_ParticleSystemRenderer);
			}
		}
		
		/// <inheritdoc/>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public override void HideAuxiliaryComponents () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}

			m_ParticleSystem.hideFlags = HideFlags.HideInInspector;
			m_ParticleSystemRenderer.hideFlags = HideFlags.HideInInspector;
			hideFlags = HideFlags.HideInInspector;
		}
		
		/// <inheritdoc/>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public override void DestroyResources () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
		}
		
		/// <inheritdoc/>
		public override void InitializeWithParticleData (ParticleDataBase a_ParticleData) {
			m_Particles = new CloudParticle [a_ParticleData.Count];
			for (int i = 0; i < a_ParticleData.Count; i = i + 1) {
				CloudParticle l_Particle = a_ParticleData [i];
				l_Particle.originalIndex = i;
				l_Particle.position = l_Particle.position * Cloud.Scale;
				l_Particle.size = l_Particle.size * Cloud.Scale;
				SetParticleAt (i, l_Particle);
			}
			m_ShurikenParticles = new ParticleSystem.Particle [a_ParticleData.Count];
			
			SetParticleSystemHasChanged ();
			UpdateParticlesIfNeeded (true);
		}
		
		/// <inheritdoc/>
		public override void RecalculateTiling () {
			
				// Remark:
				// Tiling is set in editor code, as the api is not exposed.
		}
		
		/// <inheritdoc/>
		public override void ChangedMaterial () {
			m_ParticleSystemRenderer.sharedMaterial = Cloud.ParticleMaterial;
		}

		/// <summary>
		/// Converts the cloud particle into a particle for Unity's Shuriken particle system.
		/// </summary>
		/// <returns>
		/// The shuriken particle.
		/// </returns>
		/// <param name='a_Particle'>
		/// A cloud particle.
		/// </param>
		protected abstract ParticleSystem.Particle ConvertParticle (CloudParticle a_Particle);
		
		/// <inheritdoc/>
		public override void UpdateParticlesIfNeeded (bool a_IsEnabling) {
			UpdateCurrentSunTransform ();
			
			if
				(m_HasParticleSystemChanged ||
				 HasSunTransformChanged ())
			{

				if (!Application.isPlaying) {
					if (m_ShurikenParticles.Length != m_Particles.Length) {
						m_ShurikenParticles = new ParticleSystem.Particle [m_Particles.Length];
					}
		
					for (int i = 0; i < m_Particles.Length; i = i + 1) {
						m_ShurikenParticles [i] = ConvertParticle (m_Particles [i]);
					}
					
					m_ParticleSystem.SetParticles (m_ShurikenParticles, m_ShurikenParticles.Length);

					m_HasParticleSystemChanged = false;
					UpdatePreviousSunTransform ();

						// HACK: Simulate doesn't work anymore with Unity 4.
					m_ParticleSystem.Play ();

						// Shuriken particles are cleared sometimes. If that happens,
						// we just need to update it.
					if (m_ParticleSystem.particleCount != Cloud.ParticleCount) {
						m_HasParticleSystemChanged = true;
					}
					
				} else {
					for (int i = 0; i < m_Particles.Length; i = i + 1) {
						m_ShurikenParticles [i] = ConvertParticle (m_Particles [i]);
					}
					
					m_ParticleSystem.SetParticles (m_ShurikenParticles, m_ShurikenParticles.Length);
					m_HasParticleSystemChanged = false;
					UpdatePreviousSunTransform ();
				}
			}
		}
	}
}