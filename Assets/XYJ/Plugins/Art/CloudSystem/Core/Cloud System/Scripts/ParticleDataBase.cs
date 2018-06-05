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
	/// Base class for the generic <see cref="T:Edelweiss.CloudSystem.ParticleData`3"/> class.
	/// This class was created to preserve the compatibility with clouds created before the Cloud System dll's were used.
	/// </summary>
	/// <exception cref='System.InvalidOperationException'>
	/// Is thrown when an editor only operation is called while the application is playing.
	/// </exception>
	public abstract class ParticleDataBase : ScriptableObject {
		[SerializeField] private Vector3[] m_Positions = new Vector3 [0];
		[SerializeField] private float[] m_Rotations = new float [0];
		[SerializeField] private Vector2[] m_Sizes = new Vector2 [0];
		[SerializeField] private int[] m_UVIndices = new int[0];
		[SerializeField] private bool[] m_IsSquareds = new bool [0];
		[SerializeField] private bool[] m_IsCoreParticles = new bool [0];
		[SerializeField] private int[] m_ShadingGroupIndices = new int [0];
		
		#region Runtime
		
		/// <summary>
		/// Gets the particle count.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public int Count {
			get {
				return (m_Positions.Length);
			}
		}
		
		/// <summary>
		/// Clear this particle data.
		/// </summary>
		public void Clear () {
			m_Positions = new Vector3 [0];
			m_Rotations = new float [0];
			m_Sizes = new Vector2 [0];
			m_UVIndices = new int [0];
			m_IsSquareds = new bool [0];
			m_IsCoreParticles = new bool [0];
			m_ShadingGroupIndices = new int [0];
		}
		
		/// <summary>
		/// Gets or sets the <see cref="Edelweiss.CloudSystem.ParticleDataBase"/> with the specified a_Index.
		/// Setting the particle data is only possible in the editor.
		/// </summary>
		/// <param name='a_Index'>
		/// The index.
		/// </param>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when the setter is called while the application is playing.
		/// </exception>
		public CloudParticle this [int a_Index] {
			get {
				CloudParticle l_Result = new CloudParticle ();
				l_Result.position = m_Positions [a_Index];
				l_Result.rotation = m_Rotations [a_Index];
				l_Result.size = m_Sizes [a_Index];
				l_Result.uvIndex = m_UVIndices [a_Index];
				l_Result.isSquared = m_IsSquareds [a_Index];
				l_Result.isCoreParticle = m_IsCoreParticles [a_Index];
				l_Result.shadingGroupIndex = m_ShadingGroupIndices [a_Index];
				l_Result.particleColor = Color.white;
				return (l_Result);
			}
			
//#if UNITY_EDITOR
			set {
				if (Application.isPlaying) {
					throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
				}
				m_Positions [a_Index] = value.position;
				m_Rotations [a_Index] = value.rotation;
				m_Sizes [a_Index] = value.size;
				m_UVIndices [a_Index] = value.uvIndex;
				m_IsSquareds [a_Index] = value.isSquared;
				m_IsCoreParticles [a_Index] = value.isCoreParticle;
				m_ShadingGroupIndices [a_Index] = value.shadingGroupIndex;
			}
//#endif
		}
		
		#endregion
	
		
		#region Editor
		
		/// <summary>
		/// Resize the arrays to a_NewCount.
		/// </summary>
		/// <param name='a_NewCount'>
		/// The new count.
		/// </param>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public void Resize (int a_NewCount) {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
			System.Array.Resize (ref m_Positions, a_NewCount);
			System.Array.Resize (ref m_Rotations, a_NewCount);
			System.Array.Resize (ref m_Sizes, a_NewCount);
			System.Array.Resize (ref m_UVIndices, a_NewCount);
			System.Array.Resize (ref m_IsSquareds, a_NewCount);
			System.Array.Resize (ref m_IsCoreParticles, a_NewCount);
			System.Array.Resize (ref m_ShadingGroupIndices, a_NewCount);
		}
		
		/// <summary>
		/// Add the specified a_Particle. Calling this method is only valid in the editor.
		/// </summary>
		/// <param name='a_Particle'>
		/// The particle.
		/// </param>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public void Add (CloudParticle a_Particle) {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
			Resize (Count + 1);
			this [Count - 1] = a_Particle;
		}
		
		/// <summary>
		/// Removes the particle at a_Index.
		/// </summary>
		/// <param name='a_Index'>
		/// The index has to be in [0, <see cref="P:Edelweiss.CloudSystem.ParticleDataBase.Count"/> - 1].
		/// </param>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public void RemoveAt (int a_Index) {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
				// Swap the particle data from the last position to the one that needs to be deleted.
				// Then we can simply resize the arrays.
			int l_LastIndex = Count - 1;
			m_Positions [a_Index] = m_Positions [l_LastIndex];
			m_Rotations [a_Index] = m_Rotations [l_LastIndex];
			m_Sizes [a_Index] = m_Sizes [l_LastIndex];
			m_UVIndices [a_Index] = m_UVIndices [l_LastIndex];
			m_IsSquareds [a_Index] = m_IsSquareds [l_LastIndex];
			m_IsCoreParticles [a_Index] = m_IsCoreParticles [l_LastIndex];
			m_ShadingGroupIndices [a_Index] = m_ShadingGroupIndices [l_LastIndex];
			Resize (Count - 1);
		}
		
		#endregion
	}
}