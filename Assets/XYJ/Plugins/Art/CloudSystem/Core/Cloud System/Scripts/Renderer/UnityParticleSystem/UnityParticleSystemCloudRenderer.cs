//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

#pragma warning disable

using UnityEngine;
using System.Collections;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Abstract definition for a unity particle system cloud renderer.
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
	public abstract class UnityParticleSystemCloudRenderer <C, PD, CD> : CloudParticleRenderer <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		[SerializeField][HideInInspector] private ParticleRenderer m_ParticleRenderer;
		[SerializeField][HideInInspector] private ParticleEmitter m_ParticleEmitter;
		
		private Particle[] m_UnityParticles = new Particle[0];
		
		/// <inheritdoc/>
		public override void OnEnable () {
			m_HasParticleSystemChanged = true;
		}
		
		/// <inheritdoc/>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public override void InitializeRendererComponents () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
			m_ParticleEmitter = GetComponent <ParticleEmitter> ();
			if (m_ParticleEmitter == null) {
				m_ParticleEmitter = (ParticleEmitter) gameObject.AddComponent <EllipsoidParticleEmitter>();
			}
			m_ParticleEmitter.emit = false;
			m_ParticleEmitter.useWorldSpace = false;
			
			m_ParticleRenderer = GetComponent <ParticleRenderer> ();
			if (m_ParticleRenderer == null) {
				m_ParticleRenderer = gameObject.AddComponent <ParticleRenderer> ();
			}
			
			ChangedMaterial ();
			m_ParticleRenderer.particleRenderMode = ParticleRenderMode.SortedBillboard;
			m_ParticleRenderer.maxParticleSize = Mathf.Infinity;
			m_ParticleRenderer.receiveShadows = false;
			
			RecalculateTiling ();
			m_ParticleRenderer.uvAnimationCycles = 1;
			
			HideAuxiliaryComponents ();
			
				// HACK:
				// Workaround for Unity bug (Case 414572)
			Cloud.CachedTransform.localScale = Cloud.ParticleSystemScale;
		}
		
		/// <inheritdoc/>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public override void DestroyRendererComponents () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
			if (m_ParticleRenderer != null) {
				DestroyImmediate (m_ParticleRenderer);
			}
			if (m_ParticleEmitter != null) {
				DestroyImmediate (m_ParticleEmitter);
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
			
			m_ParticleEmitter.hideFlags = HideFlags.HideInInspector;
			m_ParticleRenderer.hideFlags = HideFlags.HideInInspector;
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
			m_UnityParticles = new Particle [a_ParticleData.Count];
			
			SetParticleSystemHasChanged ();
			UpdateParticlesIfNeeded (true);
		}
		
		/// <inheritdoc/>
		public override void RecalculateTiling () {
			m_ParticleRenderer.uvAnimationXTile = Cloud.XTile;
			m_ParticleRenderer.uvAnimationYTile = Cloud.YTile;
			
				// HACK: Unity doesn't update uvTiles if uvAnimationXTile or uvAnimationYTile are changed.
			float l_DeltaX = 1.0f / Cloud.XTile;
			float l_DeltaY = 1.0f / Cloud.YTile;
			Rect[] l_UVTiles = new Rect [Cloud.TileCount];
			for (int j = 0; j < Cloud.YTile; j = j + 1) {
				float y = (Cloud.YTile - j - 1) * l_DeltaY;
				for (int i = 0; i < Cloud.XTile; i = i + 1) {
					float x = i * l_DeltaX;
					Rect l_Rect = new Rect (x, y, l_DeltaX, l_DeltaY);
					l_UVTiles [j * Cloud.XTile + i] = l_Rect;
				}
			}
			m_ParticleRenderer.uvTiles = l_UVTiles;
		}
		
		/// <inheritdoc/>
		public override void ChangedMaterial () {
			m_ParticleRenderer.material = Cloud.ParticleMaterial;
		}
		
		/// <summary>
		/// Converts the cloud particle into a Unity particle for the legacy particle system.
		/// </summary>
		/// <returns>
		/// The legacy particle.
		/// </returns>
		/// <param name='a_Particle'>
		/// A cloud particle.
		/// </param>
		protected abstract Particle ConvertParticle (CloudParticle a_Particle);
		
		/// <inheritdoc/>
		public override void UpdateParticlesIfNeeded (bool a_IsEnabling) {
			UpdateCurrentSunTransform ();
			
			if
				(m_HasParticleSystemChanged ||
				 HasSunTransformChanged ())
			{

				if (!Application.isPlaying) {

					if (m_UnityParticles.Length != m_Particles.Length) {
						m_UnityParticles = new Particle [m_Particles.Length];
					}
		
					for (int i = 0; i < m_Particles.Length; i = i + 1) {
						m_UnityParticles [i] = ConvertParticle (m_Particles [i]);
					}
					
					m_ParticleEmitter.particles = m_UnityParticles;
					m_HasParticleSystemChanged = false;
					UpdatePreviousSunTransform ();
	
				} else {
					for (int i = 0; i < m_Particles.Length; i = i + 1) {
						m_UnityParticles [i] = ConvertParticle (m_Particles [i]);
					}
					
					m_ParticleEmitter.particles = m_UnityParticles;
					m_HasParticleSystemChanged = false;
					UpdatePreviousSunTransform ();
				}
			}
		}
	}
}