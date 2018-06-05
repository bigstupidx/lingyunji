//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2013 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Abstract definition for a cloud renderer.
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
	public abstract class CloudRenderer <C, PD, CD> : MonoBehaviour
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		/// <summary>
		/// The cloud.
		/// </summary>
		protected Cloud <C, PD, CD> m_Cloud;
		/// <summary>
		/// Gets the cloud.
		/// </summary>
		/// <value>
		/// The cloud.
		/// </value>
		public Cloud <C, PD, CD> Cloud {
			get {
				if (m_Cloud	== null) {
					m_Cloud = GetComponent <C> ();
				}
				return (m_Cloud);
			}
		}
		
		/// <summary>
		/// Normalized sun direction in cloud space.
		/// Only used for renderers that support shading groups.
		/// </summary>
		protected Vector3 m_NormalizedSunDirection;
		
		/// <summary>
		/// The current sun rotation.
		/// </summary>
		protected Quaternion m_CurrentSunRotation;
		private Quaternion m_PreviousSunRotation;
		
		/// <summary>
		/// Has the particle system changed?
		/// </summary>
		protected bool m_HasParticleSystemChanged;
		/// <summary>
		/// Gets a value indicating whether the particle system has changed.
		/// </summary>
		/// <value>
		/// <c>true</c> if this particle system has changed; otherwise, <c>false</c>.
		/// </value>
		public bool HasParticleSystemChanged {
			get {
				return (m_HasParticleSystemChanged);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this instance is supported in Cloud System Free.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is supported in Cloud System Free; otherwise, <c>false</c>.
		/// </value>
		public abstract bool IsSupportedInCloudSystemFree {
			get;
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Edelweiss.CloudSystem.CloudRenderer`3"/> supports non squared particles.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports non squared particles; otherwise, <c>false</c>.
		/// </value>
		public abstract bool SupportsNonSquaredParticles {
			get;
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Edelweiss.CloudSystem.CloudRenderer`3"/> supports vertical colors.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports vertical colors; otherwise, <c>false</c>.
		/// </value>
		public abstract bool SupportsVerticalColors {
			get;
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Edelweiss.CloudSystem.CloudRenderer`3"/> supports shading groups.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports shading groups; otherwise, <c>false</c>.
		/// </value>
		public abstract bool SupportsShadingGroups {
			get;
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Edelweiss.CloudSystem.CloudRenderer`3"/> supports tint.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports tint; otherwise, <c>false</c>.
		/// </value>
		public abstract bool SupportsTint {
			get;
		}
		
		/// <summary>
		/// Use the <see cref="F:Edelweiss.CloudSystem.CloudParticle.particleColor"/> for the shading.
		/// </summary>
		public bool useParticleColor = false;
		
		/// <summary>
		/// Raises the enable event.
		/// </summary>
		public virtual void OnEnable () {
		}
		
		/// <summary>
		/// Initializes the renderer components.
		/// </summary>
		public abstract void InitializeRendererComponents ();
		
		/// <summary>
		/// Destroies the renderer components.
		/// </summary>
		public abstract void DestroyRendererComponents ();
		
		/// <summary>
		/// Destroies the resources.
		/// </summary>
		public abstract void DestroyResources ();
		
		/// <summary>
		/// Initializes this instance with particle data.
		/// </summary>
		/// <param name='a_ParticleData'>
		/// The particle data.
		/// </param>
		public abstract void InitializeWithParticleData (ParticleDataBase a_ParticleData);
		
		/// <summary>
		/// Recalculates the tiling.
		/// </summary>
		public abstract void RecalculateTiling ();
		
		/// <summary>
		/// Gets the number of cloud particles.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public abstract int Count {
			get;
		}
		
		/// <summary>
		/// Particle at a_Index.
		/// </summary>
		/// <returns>
		/// The <see cref="CloudParticle"/>.
		/// </returns>
		/// <param name='a_Index'>
		/// The index.
		/// </param>
		public abstract CloudParticle ParticleAt (int a_Index);
		
		/// <summary>
		/// Sets the particle at a_Index to a_Particle.
		/// </summary>
		/// <param name='a_Index'>
		/// The index.
		/// </param>
		/// <param name='a_Particle'>
		/// The particle.
		/// </param>
		public abstract void SetParticleAt (int a_Index, CloudParticle a_Particle);
		
		/// <summary>
		/// Calling this method indicates that the material has changed.
		/// </summary>
		public abstract void ChangedMaterial ();
		
		/// <summary>
		/// Hides the auxiliary components in the inspector.
		/// </summary>
		public abstract void HideAuxiliaryComponents ();
		
		/// <summary>
		/// Updates the current sun transform.
		/// </summary>
		protected void UpdateCurrentSunTransform () {
			if (SupportsShadingGroups) {
				
				Vector3 l_Forward;
				if (Cloud.Sun != null) {
					m_CurrentSunRotation = Cloud.Sun.transform.rotation;
					l_Forward = Cloud.Sun.transform.forward;
				} else {
					m_CurrentSunRotation = Quaternion.identity;
					l_Forward = Vector3.forward;
				}
				
					// Normalized sun direction in cloud space
				Matrix4x4 l_InverseCloudTransformMatrix = Cloud.TransformMatrix ().inverse;
				m_NormalizedSunDirection = l_InverseCloudTransformMatrix.MultiplyVector (l_Forward).normalized;
			}
		}
		
		/// <summary>
		/// Updates the previous sun transform.
		/// </summary>
		protected void UpdatePreviousSunTransform () {
			if (SupportsShadingGroups) {
				m_PreviousSunRotation = m_CurrentSunRotation;
			}
		}
		
		/// <summary>
		/// Determines whether the sun transform has changed.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the sun transform has changed; otherwise, <c>false</c>.
		/// </returns>
		protected bool HasSunTransformChanged () {
			return (SupportsShadingGroups && m_PreviousSunRotation != m_CurrentSunRotation);
		}
		
		/// <summary>
		/// Sets the flag that the particle system has changed.
		/// </summary>
		public void SetParticleSystemHasChanged () {
			m_HasParticleSystemChanged = true;
		}
		
		/// <summary>
		/// Updates the particles if needed.
		///
		/// REMARK: The enabling parameter is only used to prevent a Unity crash bug for custom renderers!
		/// </summary>
		public abstract void UpdateParticlesIfNeeded (bool a_IsEnabling);
		
		/// <summary>
		/// Called when boundings box of cloud was updated.
		/// </summary>
		public virtual void BoundingBoxUpdated () {
		}
		
		/// <summary>
		/// The amount of space the cloud covers on the screen. Off screen parts are included as well.
		/// </summary>
		/// <returns>
		/// The cloud coverage factor.
		/// </returns>
		/// <param name='a_Camera'>
		/// The camera.
		/// </param>
		protected float ScreenCoverageFactor (Camera a_Camera) {
			Bounds l_BoundingBox = Cloud.BoundingBox;
			Vector3 l_BoundingBoxCenter = l_BoundingBox.center;
			Vector3 l_BoundingBoxExtends = l_BoundingBox.extents;
			
				// Bring the bounding box corners to viewport space.
			Vector3 l_Corner1 = new Vector3 (+l_BoundingBoxExtends.x, +l_BoundingBoxExtends.y, +l_BoundingBoxExtends.z);
			Vector3 l_Corner2 = new Vector3 (+l_BoundingBoxExtends.x, +l_BoundingBoxExtends.y, -l_BoundingBoxExtends.z);
			Vector3 l_Corner3 = new Vector3 (+l_BoundingBoxExtends.x, -l_BoundingBoxExtends.y, +l_BoundingBoxExtends.z);
			Vector3 l_Corner4 = new Vector3 (+l_BoundingBoxExtends.x, -l_BoundingBoxExtends.y, -l_BoundingBoxExtends.z);
			Vector3 l_Corner5 = new Vector3 (-l_BoundingBoxExtends.x, +l_BoundingBoxExtends.y, +l_BoundingBoxExtends.z);
			Vector3 l_Corner6 = new Vector3 (-l_BoundingBoxExtends.x, +l_BoundingBoxExtends.y, -l_BoundingBoxExtends.z);
			Vector3 l_Corner7 = new Vector3 (-l_BoundingBoxExtends.x, -l_BoundingBoxExtends.y, +l_BoundingBoxExtends.z);
			Vector3 l_Corner8 = new Vector3 (-l_BoundingBoxExtends.x, -l_BoundingBoxExtends.y, -l_BoundingBoxExtends.z);
			
			l_Corner1 = a_Camera.WorldToViewportPoint (l_BoundingBoxCenter + l_Corner1);
			l_Corner2 = a_Camera.WorldToViewportPoint (l_BoundingBoxCenter + l_Corner2);
			l_Corner3 = a_Camera.WorldToViewportPoint (l_BoundingBoxCenter + l_Corner3);
			l_Corner4 = a_Camera.WorldToViewportPoint (l_BoundingBoxCenter + l_Corner4);
			l_Corner5 = a_Camera.WorldToViewportPoint (l_BoundingBoxCenter + l_Corner5);
			l_Corner6 = a_Camera.WorldToViewportPoint (l_BoundingBoxCenter + l_Corner6);
			l_Corner7 = a_Camera.WorldToViewportPoint (l_BoundingBoxCenter + l_Corner7);
			l_Corner8 = a_Camera.WorldToViewportPoint (l_BoundingBoxCenter + l_Corner8);
			
				// Find the maximum and minimum
			Vector2 l_Minimum = new Vector2 (l_Corner1.x, l_Corner1.y);
			l_Minimum = Vector2.Min (l_Minimum, l_Corner2);
			l_Minimum = Vector2.Min (l_Minimum, l_Corner3);
			l_Minimum = Vector2.Min (l_Minimum, l_Corner4);
			l_Minimum = Vector2.Min (l_Minimum, l_Corner5);
			l_Minimum = Vector2.Min (l_Minimum, l_Corner6);
			l_Minimum = Vector2.Min (l_Minimum, l_Corner7);
			l_Minimum = Vector2.Min (l_Minimum, l_Corner8);
			
			Vector2 l_Maximum = new Vector2 (l_Corner1.x, l_Corner1.y);
			l_Maximum = Vector2.Max (l_Maximum, l_Corner2);
			l_Maximum = Vector2.Max (l_Maximum, l_Corner3);
			l_Maximum = Vector2.Max (l_Maximum, l_Corner4);
			l_Maximum = Vector2.Max (l_Maximum, l_Corner5);
			l_Maximum = Vector2.Max (l_Maximum, l_Corner6);
			l_Maximum = Vector2.Max (l_Maximum, l_Corner7);
			l_Maximum = Vector2.Max (l_Maximum, l_Corner8);
			
			Vector2 l_Area = l_Maximum - l_Minimum;
			float l_Result = l_Area.x * l_Area.y;
			l_Result = Mathf.Abs (l_Result);
			return (l_Result);
		}
	}
}