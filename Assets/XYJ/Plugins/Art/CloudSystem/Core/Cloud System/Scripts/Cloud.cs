//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Edelweiss.CloudSystem {

	/// <summary>
	/// Generic base class of <see cref="T:CS_Cloud"/>.
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
	/// <exception cref='System.InvalidOperationException'>
	/// Is thrown when an editor only operation is called while the application is playing.
	/// </exception>
	public abstract class Cloud <C, PD, CD> : CloudBase
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		[SerializeField] private PD m_ParticleData;
		/// <summary>
		/// Particles are structs and can not be serialized directly in Unity.
		/// That's the reason why we have this particle data. It is only intended
		/// to be used in the editor and for the initialization at runtime.
		/// </summary>
		/// <value>
		/// The particle data.
		/// </value>
		public PD ParticleData {
			get {
				return (m_ParticleData);
			}
		}
		
//#if UNITY_EDITOR
		
		// HACK: Those can not be editor only, as it causes Windows Standalone builds to freeze!
		[SerializeField] private CD m_CreatorData;
		[SerializeField] private CloudRendererTypeEnum m_CloudRendererType;
//#endif
		
		private CloudRendererData m_CloudRendererData = new CloudRendererData ();
		/// <summary>
		/// Gets the cloud renderer data for multithreading.
		/// </summary>
		/// <value>
		/// The cloud renderer data.
		/// </value>
		internal CloudRendererData CloudRendererData {
			get {
				
					// HACK:
					// The cloud renderer data is needed for multithreading.
					// We update the data on every call to be sure that it is
					// always in the correct state.
					// For future updates, we should have a separate class where
					// all this data is serialized, such that that one can be used
					// and it is the only place where the data is available.
				m_CloudRendererData.m_Tint = m_Tint;
			
				m_CloudRendererData.m_CoreParticleTransparency = m_CoreParticleTransparency;
				m_CloudRendererData.m_NonCoreParticleTransparency = m_NonCoreParticleTransparency;
				
				m_CloudRendererData.m_VerticalShadingInfluence = m_VerticalShadingInfluence;
				m_CloudRendererData.m_ShadingGroupInfluence = m_ShadingGroupInfluence;
				
				m_CloudRendererData.verticalColors = verticalColors;
				m_CloudRendererData.m_VerticalColorsInverseHeightFactors = m_VerticalColorsInverseHeightFactors;
				m_CloudRendererData.m_VerticalColorBottom = m_VerticalColorBottom;
				m_CloudRendererData.m_VerticalColorInverseHeight = m_VerticalColorInverseHeight;
				
				m_CloudRendererData.shadingGroups = shadingGroups;
				m_CloudRendererData.shadingColors = shadingColors;
				m_CloudRendererData.m_ShadingColorsInverseShadingFactors = m_ShadingColorsInverseShadingFactors;
					
				return (m_CloudRendererData);
			}
		}
		
//#if UNITY_EDITOR
		
		/// <summary>
		/// All kinds of data, that is only used in the Editor to create the
		/// clouds, is available in here.
		/// </summary>
		/// <value>
		/// The creator data.
		/// </value>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public CD CreatorData {
			get {
				if (Application.isPlaying) {
					throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
				}
				return (m_CreatorData);
			}
		}
		
		/// <summary>
		/// Special enum for the renderer type. It is only used in the Editor.
		/// </summary>
		/// <value>
		/// The type of the cloud renderer.
		/// </value>
		public CloudRendererTypeEnum CloudRendererType {
			get {
				return (m_CloudRendererType);
			}
			set {
				m_CloudRendererType = value;
			}
		}
//#endif
		
		[HideInInspector] private CloudRenderer <C, PD, CD> m_CloudRenderer;
		
		/// <summary>
		/// Renderer that is used for this cloud. It is not possible to
		/// change the renderer at runtime. It needs to be changed in the
		/// Editor.
		/// </summary>
		/// <value>
		/// The cloud renderer.
		/// </value>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public CloudRenderer <C, PD, CD> CloudRenderer {
			get {
				if (Application.isPlaying) {
					throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
				}
				return (m_CloudRenderer);
			}
			set {
				if (Application.isPlaying) {
					throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
				}
				m_CloudRenderer = value;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this instance is initialized.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
		/// </value>
		public bool IsInitialized {
			get {
				return (m_CloudRenderer != null);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Edelweiss.CloudSystem.Cloud`3"/> supports non squared particles.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports non squared particles; otherwise, <c>false</c>.
		/// </value>
		public bool SupportsNonSquaredParticles {
			get {
				return (m_CloudRenderer.SupportsNonSquaredParticles);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Edelweiss.CloudSystem.Cloud`3"/> supports vertical colors.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports vertical colors; otherwise, <c>false</c>.
		/// </value>
		public bool SupportsVerticalColors {
			get {
				return (m_CloudRenderer.SupportsVerticalColors);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Edelweiss.CloudSystem.Cloud`3"/> supports shading groups.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports shading groups; otherwise, <c>false</c>.
		/// </value>
		public bool SupportsShadingGroups {
			get {
				return (m_CloudRenderer.SupportsShadingGroups);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Edelweiss.CloudSystem.Cloud`3"/> supports tint.
		/// </summary>
		/// <value>
		/// <c>true</c> if supports tint; otherwise, <c>false</c>.
		/// </value>
		public bool SupportsTint {
			get {
				return (m_CloudRenderer.SupportsTint);
			}
		}
		
		
		[SerializeField] private Material m_ParticleMaterial;
		/// <summary>
		/// Material used to render the particles. 
		/// </summary>
		/// <value>
		/// The particle material.
		/// </value>
		public Material ParticleMaterial {
			get {
				return (m_ParticleMaterial);
			}
			set {
				if (m_ParticleMaterial != value) {
					m_ParticleMaterial = value;
					m_CloudRenderer.ChangedMaterial ();
				}
			}
		}
		
		[SerializeField] private int m_XTile = 0;
		/// <summary>
		/// The number of columns into which the particle material is splitted.
		/// This value has to be strictly greater than zero.
		/// </summary>
		/// <value>
		/// The X tile.
		/// </value>
		public int XTile {
			get {
				return (m_XTile);
			}
			set {
				if (m_XTile != value) {
					m_XTile = value;
					if (m_XTile < 1) {
						m_XTile = 1;
					}
					m_CloudRenderer.RecalculateTiling ();
				}
			}
		}
		
		
		[SerializeField] private int m_YTile = 0;
		/// <summary>
		/// The number of rows into which the particle material is splitted.
		/// This has to be strictly greater than zero.
		/// </summary>
		/// <value>
		/// The Y tile.
		/// </value>
		public int YTile {
			get {
				return (m_YTile);
			}
			set {
				if (m_YTile != value) {
					m_YTile = value;
					if (m_YTile < 1) {
						m_YTile = 1;
					}
					m_CloudRenderer.RecalculateTiling ();
				}
			}
		}
		
		/// <summary>
		/// The number of particle sprites that are available in the particle material.
		/// </summary>
		/// <value>
		/// The tile count.
		/// </value>
		public int TileCount {
			get {
				return (XTile * YTile);
			}
		}
		
		
		[SerializeField] private Color m_Tint = Color.white;
		/// <summary>
		/// Base color of the cloud.
		/// If this value is changed, all particles are going to be recalculated.
		/// </summary>
		/// <value>
		/// The tint.
		/// </value>
		public Color Tint {
			get {
				return (m_Tint);
			}
			set {
				m_Tint = value;
				SetParticleSystemHasChanged ();
			}
		}
		
		
		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="F:Edelweiss.CloudSystem.CloudParticle.particleColor"/>
		/// should be used for the cloud shading.
		/// </summary>
		/// <value>
		/// <c>true</c> if use particle color; otherwise, <c>false</c>.
		/// </value>
		public bool UseParticleColor {
			get {
				return (m_CloudRenderer.useParticleColor);
			}
			set {
				m_CloudRenderer.useParticleColor = value;
			}
		}
		
		
		[SerializeField] private float m_VerticalShadingInfluence = 0.5f;
		/// <summary>
		/// For renderers which support both vertical colors and shading groups, you may
		/// change the amount by which they are visible.
		/// This value has to be in the range [0.0f, 1.0f]. By changing this value, the
		/// other influence factor gets manipulated too, such that the sum of them is
		/// exactly one.
		/// If this value is changed, all particles are going to be recalculated.
		/// </summary>
		/// <value>
		/// The vertical shading influence.
		/// </value>
		public float VerticalShadingInfluence {
			get {
				return (m_VerticalShadingInfluence);
			}
			set {
				m_VerticalShadingInfluence = value;
				m_ShadingGroupInfluence = 1.0f - m_VerticalShadingInfluence;
				if (m_CloudRenderer.SupportsVerticalColors && m_CloudRenderer.SupportsShadingGroups) {
					SetParticleSystemHasChanged ();
				}
			}
		}
		
		[SerializeField] private float m_ShadingGroupInfluence = 0.5f;
		/// <summary>
		/// For renderers which support both vertical colors and shading groups, you may
		/// change the amount by which they are visible.
		/// This value has to be in the range [0.0f, 1.0f]. By changing this value, the
		/// other influence factor gets manipulated too, such that the sum of them is
		/// exactly one.
		/// If this value is changed, all particles are going to be recalculated.
		/// </summary>
		/// <value>
		/// The shading group influence.
		/// </value>
		public float ShadingGroupInfluence {
			get {
				return (m_ShadingGroupInfluence);
			}
			set {
				m_ShadingGroupInfluence = value;
				m_VerticalShadingInfluence = 1.0f - m_ShadingGroupInfluence;
				if (m_CloudRenderer.SupportsVerticalColors && m_CloudRenderer.SupportsShadingGroups) {
					SetParticleSystemHasChanged ();
				}
			}
		}
		
		
		[SerializeField] private float m_Scale = 1.0f;
		/// <inheritdoc/>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when the setter is called while the application is playing.
		/// </exception>
		public override float Scale {
			get {
				return (m_Scale);
			}
			set {
				if (Application.isPlaying) {
					throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
				}
				if (m_Scale != value) {
					m_Scale = value;
					if (m_Scale < 0.0f) {
						m_Scale = 0.0f;
					}
				}		
			}
		}
		
		
		[SerializeField] private float m_Fading = 1.0f;
		/// <summary>
		/// A simple fading factor for the cloud. I has to be in [0.0f, 1.0f].
		/// By changing this value, both the core particle transparency and the
		/// non core particle transparency are going to be changed.
		/// If this value is changed, all particles are going to be recalculated.
		/// </summary>
		/// <value>
		/// The fading.
		/// </value>
		public float Fading {
			get {
				return (m_Fading);
			}
			set {
				m_Fading = value;
				RecalculateFadingFactors ();
				SetParticleSystemHasChanged ();
			}
		}
		
		private void RecalculateFadingFactors () {
			if (m_Fading > 0.5f) {
				m_CoreParticleTransparency = 1.0f;
				m_NonCoreParticleTransparency = (m_Fading - 0.5f) * 2.0f;
			} else {
				m_CoreParticleTransparency = m_Fading * 2.0f;
				m_NonCoreParticleTransparency = 0.0f;
			}
		}
		
		
		private float m_CoreParticleTransparency = 1.0f;
		/// <summary>
		/// The core particle transparency controls the visibility of particles
		/// which have the core flag set to true. The value has to be in the
		/// range [0.0f, 1.0f].
		/// Changing this value overrides any previous setting made using
		/// <see cref="P:Edelweiss.CloudSystem.Cloud`3.Fading"/>.
		/// If this value is changed, all particles are going to be recalculated.
		/// </summary>
		/// <value>
		/// The core particle transparency.
		/// </value>
		public float CoreParticleTransparency {
			get {
				return (m_CoreParticleTransparency);
			}
			set {
				m_CoreParticleTransparency = value;
				SetParticleSystemHasChanged ();
			}
		}
		
		private float m_NonCoreParticleTransparency = 1.0f;
		/// <summary>
		/// The non core particle transparency controls the visibility of particles
		/// which have the core flag set to false. The value has to be in the
		/// range [0.0f, 1.0f].
		/// Changing this value overrides any previous setting made using
		/// <see cref="P:Edelweiss.CloudSystem.Cloud`3.Fading"/>.
		/// If this value is changed, all particles are going to be recalculated.
		/// </summary>
		/// <value>
		/// The non core particle transparency.
		/// </value>
		public float NonCoreParticleTransparency {
			get {
				return (m_NonCoreParticleTransparency);
			}
			set {
				m_NonCoreParticleTransparency = value;
				SetParticleSystemHasChanged ();
			}
		}
		
		
		private Transform m_Transform;
		
		/// <summary>
		/// Cached transform to slightly increase the performance. 
		/// </summary>
		/// <value>
		/// The cached transform.
		/// </value>
		public Transform CachedTransform {
			get {
				if (m_Transform == null) {
					m_Transform = transform;
				}
				return (m_Transform);
			}
		}
		
		/// <summary>
		/// Transform matrix that is used by the custom renderers. 
		/// </summary>
		/// <returns>
		/// The transform matrix of this cloud.
		/// A <see cref="T:Matrix4x4"/>
		/// </returns>
		public Matrix4x4 TransformMatrix () {
			Matrix4x4 l_Result = Matrix4x4.TRS (CachedTransform.position, CachedTransform.rotation, new Vector3 (Scale, Scale, Scale));
			return (l_Result);
		}
		
		/// <summary>
		/// Unscaled transform matrix of the cloud. 
		/// </summary>
		/// <returns>
		/// The unscaled transform matrix of this cloud.
		/// A <see cref="T:Matrix4x4"/>
		/// </returns>
		public Matrix4x4 UnscaledTransformMatrix () {
			Matrix4x4 l_Result = Matrix4x4.TRS (CachedTransform.position, CachedTransform.rotation, Vector3.one);
			return (l_Result);
		}
	
		
		#region Bug Workaround
			// HACK:
			// Workaround for a Unity bug. If a transparent particle system and a transparent mesh have exactly
			// the scales and/or other settings, the mesh is always drawn in front of the particle system.
			// (Case 414572)
		
		/// <summary>
		/// Gets the mesh scale for the workaround.
		/// </summary>
		/// <value>
		/// The mesh scale.
		/// </value>
		public Vector3 MeshScale {
			get {
				return (new Vector3 (1.0f, 1.0f, 1.0f));
			}
		}
		
		/// <summary>
		/// Gets the particle system scale for the workaround.
		/// </summary>
		/// <value>
		/// The particle system scale.
		/// </value>
		public Vector3 ParticleSystemScale {
			get {
				return (new Vector3 (1.0001f, 1.0001f, 1.0001f));
			}
		}
		#endregion
	

		private Bounds m_BoundingBox;
		
		/// <summary>
		/// Bounding box of the cloud in local space. 
		/// </summary>
		/// <value>
		/// The bounding box.
		/// </value>
		public Bounds BoundingBox {
			get {
				return (m_BoundingBox);
			}
		}
		
		/// <summary>
		/// If the particle position or size is changed, such that the current
		/// bounding box is not anymore valid, this method has to be called in
		/// order to get correct visual results.
		/// </summary>
		public void RecalculateBoundingBox () {
			if (m_ParticleData == null) {
				if (m_CloudRenderer.Count == 0) {
					m_BoundingBox = new Bounds (Vector3.zero, Vector3.zero);
				} else {
					CloudParticle l_Particle = m_CloudRenderer.ParticleAt (0);
					float l_ParticleSize = l_Particle.Radius ();
					m_BoundingBox = new Bounds (l_Particle.position, new Vector3 (l_ParticleSize, l_ParticleSize, l_ParticleSize));
				}
				
				for (int i = 0; i < m_CloudRenderer.Count; i = i + 1) {
					CloudParticle l_Particle = m_CloudRenderer.ParticleAt (i);
					float l_ParticleSize = l_Particle.Radius ();
					Bounds l_Bounds = new Bounds (l_Particle.position, new Vector3 (l_ParticleSize, l_ParticleSize, l_ParticleSize));
					m_BoundingBox.Encapsulate (l_Bounds);
				}
			} else {
				
					// Here we need to multiply the scale, as it is not applied to the position and size of the particles yet.
				if (m_ParticleData.Count == 0) {
					m_BoundingBox = new Bounds (Vector3.zero, Vector3.zero);
				} else {
					CloudParticle l_Particle = m_ParticleData [0];
					float l_ParticleSize = l_Particle.Radius () * Scale;
					m_BoundingBox = new Bounds (l_Particle.position, new Vector3 (l_ParticleSize, l_ParticleSize, l_ParticleSize));
				}
				for (int i = 0; i < m_ParticleData.Count; i = i + 1) {
					CloudParticle l_Particle = m_ParticleData [i];
					float l_ParticleSize = l_Particle.Radius () * Scale;
					Bounds l_Bounds = new Bounds (l_Particle.position * Scale, new Vector3 (l_ParticleSize, l_ParticleSize, l_ParticleSize));
					m_BoundingBox.Encapsulate (l_Bounds);
				}
			}
			m_CloudRenderer.BoundingBoxUpdated ();
			
				// Values for vertical colors
			m_VerticalColorBottom = m_BoundingBox.min.y;
			m_VerticalColorHeight = (m_BoundingBox.max.y - m_BoundingBox.min.y);
			
			if (m_VerticalColorHeight != 0.0f) {
				m_VerticalColorInverseHeight = 1.0f / m_VerticalColorHeight;
			}
		}
		
		/// <inheritdoc/>
		public override void SetParticleSystemHasChanged () {
			m_CloudRenderer.SetParticleSystemHasChanged ();
		}	
		
		
		#region Vertical Colors
		
		/// <summary>
		/// Vertical colors are used by renderers which support that specific functionality.
		/// If the size of this array is changed or any <see cref="F:Edelweiss.CloudSystem.CS_VerticalColor.verticalFactor"/>
		/// in it, you have to call
		/// <see cref="M:Edelweiss.CloudSystem.Cloud`3.RecalculateVerticalColorFactors"/> to get valid results. If the
		/// vertical colors are not sorted, it is necessary to call
		/// <see cref="M:Edelweiss.CloudSystem.Cloud`3.SortVerticalColors"/> first.
		/// There always have to be at least two element. One with a
		/// <see cref="F:Edelweiss.CloudSystem.CS_VerticalColor.verticalFactor"/> of 0.0f,
		/// and one with a <see cref="F:Edelweiss.CloudSystem.CS_VerticalColor.verticalFactor"/> of 1.0f.
		/// All other <see cref="F:Edelweiss.CloudSystem.CS_VerticalColor.verticalFactor">vertical factors</see> have to be
		/// values from [0.0f, 1.0f]. It is not allowed that there are two identical
		/// <see cref="F:Edelweiss.CloudSystem.CS_VerticalColor.verticalFactor">vertical factors</see>.
		/// </summary>
		public CS_VerticalColor[] verticalColors = new CS_VerticalColor [0];
		private float[] m_VerticalColorsInverseHeightFactors;
		
		private float m_VerticalColorBottom;
		private float m_VerticalColorHeight;
		private float m_VerticalColorInverseHeight;
		
		/// <summary>
		/// Checks if the <see cref="F:Edelweiss.CloudSystem.Cloud`3.verticalColors"/> array is sorted.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c>, if <see cref="F:Edelweiss.CloudSystem.Cloud`3.verticalColors"/> is sorted.
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool AreVerticalColorsSorted () {
			bool l_Result = true;
			for (int i = 0; i < verticalColors.Length - 1; i = i + 1) {
				if (verticalColors [i].CompareTo (verticalColors [i + 1]) > 0) {
					l_Result = false;
					break;
				}
			}
			return (l_Result);
		}
	
		/// <summary>
		/// Sort the <see cref="F:Edelweiss.CloudSystem.Cloud`3.verticalColors"/> array. 
		/// </summary>
		public void SortVerticalColors () {
			System.Array.Sort (verticalColors);
		}
		
		/// <summary>
		/// Convert a scaled height within the cloud to a vertical factor that can be
		/// used to calculate a vertical color.
		/// It is necessary that the bounding box of this cloud had a height that was strictly
		/// greater than zero when <see cref="M:Edelweiss.CloudSystem.Cloud`3.RecalculateBoundingBox"/>
		/// was called the last time.
		/// </summary>
		/// <param name="a_Height">
		/// Height within the cloud. The height should be within the bounding box in order to get
		/// a valid result.
		/// A <see cref="T:System.Single"/>
		/// </param>
		/// <returns>
		/// Factor that is usable to calculate the actual color at the height using
		/// <see cref="M:Edelweiss.CloudSystem.Cloud`3.VerticalColorAt"/>.
		/// If a height is a valid height in the bounding box, the result is going to be in [0.0f, 1.0f].
		/// A <see cref="T:System.Single"/>
		/// </returns>
		public float HeightToVerticalFactor (float a_Height) {
			float l_Result = (a_Height - m_VerticalColorBottom) * (m_VerticalColorInverseHeight);
			return (l_Result);
		}
		
		/// <summary>
		/// The color we get for the given vertical factor using the <see cref="F:Edelweiss.CloudSystem.Cloud`3.verticalColors"/> array.
		/// It is necessary that <see cref="F:Edelweiss.CloudSystem.Cloud`3.verticalColors"/> was set up correctly in order to get valid results.
		/// </summary>
		/// <param name="a_VerticalFactor">
		/// Height factor that can be calculated using <see cref="M:Edelweiss.CloudSystem.Cloud`3.HeightToVerticalFactor"/>. This value
		/// has to be in [0.0f, 1.0f].
		/// A <see cref="T:System.Single"/>
		/// </param>
		/// <returns>
		/// Color for the given vertical factor.
		/// A <see cref="T:Color"/>
		/// </returns>
		public Color VerticalColorAt (float a_VerticalFactor) {		
			int i;
			for (i = 0; i < verticalColors.Length - 2; i = i + 1) {
				if
					(verticalColors [i].verticalFactor <= a_VerticalFactor &&
				     a_VerticalFactor <= verticalColors [i + 1].verticalFactor)
				{	
					break;
				}
			}
			float l_Factor = (a_VerticalFactor - verticalColors [i].verticalFactor) * m_VerticalColorsInverseHeightFactors [i];
			Color l_Result = Color.Lerp (verticalColors [i].color, verticalColors [i + 1].color, l_Factor);
			return (l_Result);
		}
		
		/// <summary>
		/// Recalculate factors that are based on <see cref="F:Edelweiss.CloudSystem.Cloud`3.verticalColors"/>
		/// and are needed for the correct results. This operation is only allowed to be called, if
		/// <see cref="F:Edelweiss.CloudSystem.Cloud`3.verticalColors"/> is valid according to its
		/// description.
		/// </summary>
		public void RecalculateVerticalColorFactors () {
			if
				(m_VerticalColorsInverseHeightFactors == null ||
				 m_VerticalColorsInverseHeightFactors.Length != verticalColors.Length - 1)
			{
				m_VerticalColorsInverseHeightFactors = new float [verticalColors.Length - 1];
			}
			for (int i = 0; i < m_VerticalColorsInverseHeightFactors.Length; i = i + 1) {
				m_VerticalColorsInverseHeightFactors [i] = 1.0f / (verticalColors [i + 1].verticalFactor - verticalColors [i].verticalFactor);
			}
			SetParticleSystemHasChanged ();
		}
		
		#endregion
		
		
		#region Shading Groups
		
		[SerializeField] private Transform m_Sun;
		/// <summary>
		/// Transform of the object that is used as sun for the shading group rendering.
		/// This reference is not allowed to be null, if a renderer is used that supports
		/// shading groups.
		/// </summary>
		/// <value>
		/// The sun.
		/// </value>
		public Transform Sun {
			get {
				return (m_Sun);
			}
			set {
				m_Sun = value;
			}
		}
		
		/// <summary>
		/// Shading groups are used by renderers which support that specific functionality.
		/// Every particle belongs to a shading group, that's why you have to make sure, that
		/// this array contains enough shading groups.
		/// If you manipulate the <see cref="F:Edelweiss.CloudSystem.CS_ShadingGroup.center"/>
		/// of a shading group, you need to update the
		/// <see cref="F:Edelweiss.CloudSystem.CS_ShadingGroup.scaledCenter"/> too. Use 
		/// <see cref="M:Edelweiss.CloudSystem.CS_ShadingGroup.RecalculateScaledCenter(Edelweiss.CloudSystem.CloudBase)"/> for that.
		/// </summary>
		public CS_ShadingGroup[] shadingGroups = new CS_ShadingGroup [0];
		
		/// <summary>
		/// Shading colors are used by renderers which support shading groups.
		/// if the size of this array is changed or any <see cref="F:Edelweiss.CloudSystem.CS_ShadingColor.shadingFactor"/>
		/// in it, you have to call <see cref="M:Edelweiss.CloudSystem.Cloud`3.RecalculateShadingColorFactors"/>
		/// to get valid results. If the shading colors are not sorted, it is necessary to call
		/// <see cref="M:Edelweiss.CloudSystem.Cloud`3.SortShadingColors"/> first.
		/// There always have to be at least two elements. One with a
		/// <see cref="F:Edelweiss.CloudSystem.CS_ShadingColor.shadingFactor"/> of -1.0f,
		/// and one with a <see cref="F:Edelweiss.CloudSystem.CS_ShadingColor.shadingFactor"/>
		/// of 1.0f. All other <see cref="F:Edelweiss.CloudSystem.CS_ShadingColor.shadingFactor">shading factors</see>
		/// have to be values from [-1.0f, 1.0f]. It is not allowed that there are two identical
		/// <see cref="F:Edelweiss.CloudSystem.CS_ShadingColor.shadingFactor">shading factors</see>.
		/// </summary>
		public CS_ShadingColor[] shadingColors = new CS_ShadingColor [0];
		private float [] m_ShadingColorsInverseShadingFactors;
		
		/// <summary>
		/// Checks if the <see cref="F:Edelweiss.CloudSystem.Cloud`3.shadingColors"/> array is sorted.
		/// </summary>
		/// <returns>
		/// Returns true, if <see cref="F:Edelweiss.CloudSystem.Cloud`3.shadingColors"/> is sorted.
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool AreShadingColorsSorted () {
			bool l_Result = true;
			for (int i = 0; i < shadingColors.Length - 1; i = i + 1) {
				if (shadingColors [i].CompareTo (shadingColors [i + 1]) > 0) {
					l_Result = false;
					break;
				}
			}
			return (l_Result);
		}
		
		/// <summary>
		/// Sort the <see cref="F:Edelweiss.CloudSystem.Cloud`3.shadingColors"/> array. 
		/// </summary>
		public void SortShadingColors () {
			System.Array.Sort (shadingColors);
		}
		
		/// <summary>
		/// The color we get for the given shading factor using the
		/// <see cref="F:Edelweiss.CloudSystem.Cloud`3.shadingColors"/> array.
		/// It is necessary that <see cref="F:Edelweiss.CloudSystem.Cloud`3.shadingColors"/> was
		/// set up correctly in order to get valid results.
		/// </summary>
		/// <param name="a_ShadingFactor">
		/// Shading factor is intended to be calculated in the renderer. It is the scalar product of the
		/// normalized sun direction with the normalized vector from the scaled shading group center to
		/// a certain position in the cloud, for which the color is calculated.
		/// This valud has to be in [-1.0f, 1.0f].
		/// A <see cref="System.Single"/>
		/// </param>
		/// <returns>
		/// Color for the given shading factor.
		/// A <see cref="Color"/>
		/// </returns>
		public Color ShadingColorAt (float a_ShadingFactor) {
			int i;
			for (i = 0; i < shadingColors.Length - 2; i = i + 1) {
				if
					(shadingColors [i].shadingFactor <= a_ShadingFactor &&
					 a_ShadingFactor <= shadingColors [i + 1].shadingFactor)
				{
					break;
				}
			}
			float l_Factor = (a_ShadingFactor - shadingColors [i].shadingFactor) * m_ShadingColorsInverseShadingFactors [i];
			Color l_Result = Color.Lerp (shadingColors [i].color, shadingColors [i + 1].color, l_Factor);
			return (l_Result);
		}
		
		/// <summary>
		/// Recalculate factors that are based on <see cref="F:Edelweiss.CloudSystem.Cloud`3.shadingColors"/> and are needed for the correct results.
		/// This operation is only allowed to be called, if <see cref="F:Edelweiss.CloudSystem.Cloud`3.shadingColors"/> is valid according to its
		/// description.
		/// </summary>
		public void RecalculateShadingColorFactors () {
			if
				(m_ShadingColorsInverseShadingFactors == null ||
				 m_ShadingColorsInverseShadingFactors.Length != shadingColors.Length - 1)
			{
				m_ShadingColorsInverseShadingFactors = new float [shadingColors.Length - 1];
			}
			for (int i = 0; i < m_ShadingColorsInverseShadingFactors.Length; i = i + 1) {
				m_ShadingColorsInverseShadingFactors [i] = 1.0f / (shadingColors [i + 1].shadingFactor - shadingColors [i].shadingFactor);
			}
			SetParticleSystemHasChanged ();
		}
		
		/// <summary>
		/// Recalculate the scaled centers for all shading groups. 
		/// </summary>
		public void RecalculateScaledShadingGroupCenters () {
			foreach (CS_ShadingGroup l_ShadingGroup in shadingGroups) {
				l_ShadingGroup.RecalculateScaledCenter (this);
			}
			SetParticleSystemHasChanged ();
		}
		
		#endregion
		
		
		#region Particles
		
		/// <summary>
		/// Number of particles this cloud consists of.
		/// This value is intended to be used at runtime.
		/// </summary>
		/// <value>
		/// The particle count.
		/// </value>
		public int ParticleCount {
			get {
				return (m_CloudRenderer.Count);
			}
		}
		
		/// <summary>
		/// Returns the particle from this cloud at index.
		/// This method is indended to be used at runtime only.
		/// Editor scripts preferably use <see cref="P:Edelweiss.CloudSystem.Cloud`3.ParticleData"/> instead.
		/// </summary>
		/// <param name="a_Index">
		/// The position at which the wanted particle is. This value has to be in
		/// [0, <see cref="P:Edelweiss.CloudSystem.Cloud`3.ParticleCount"/> - 1].
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Particle at index from this cloud.
		/// A <see cref="T:Edelweiss.CloudSystem.CloudParticle"/>
		/// </returns>
		public CloudParticle ParticleAt (int a_Index) {
			return (m_CloudRenderer.ParticleAt (a_Index));
		}
		
		/// <summary>
		/// Set the particle to this cloud at index.
		/// This method is intended to be used at runtime only.
		/// Editor scripts preferably use
		/// <see cref="P:Edelweiss.CloudSystem.Cloud`3.ParticleData"/> instead.
		/// </summary>
		/// <param name="a_Index">
		/// The position at which the particle is set. This value has to be in
		/// [0, ParticleCount - 1].
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="a_Particle">
		/// The particle that has to be set at index.
		/// A <see cref="T:Edelweiss.CloudSystem.CloudParticle"/>
		/// </param>
		public void SetParticleAt (int a_Index, CloudParticle a_Particle) {
			m_CloudRenderer.SetParticleAt (a_Index, a_Particle);
		}
		
		#endregion
		
		
		/// <summary>
		/// Raises the enable event.
		/// </summary>
		public void OnEnable () {
			m_CloudRenderer = GetComponent <CloudRenderer <C, PD, CD>> ();
			
			if (!Application.isPlaying) {
				if (!IsInitialized) {
					PrepareVerticalColors ();
					PrepareShadingColors ();
					PrepareShadingGroups ();
					PrepareParticleGroups ();
					PrepareMaterialAndTiling ();
					
						// Initialize renderer and pass values to it.
					m_CloudRenderer = GetComponent <CloudRenderer <C, PD, CD>> ();
				}
			}
	
			if (IsInitialized) {
				RecalculateBoundingBox ();
				RecalculateFadingFactors ();
				RecalculateVerticalColorFactors ();
				RecalculateShadingColorFactors ();
				RecalculateScaledShadingGroupCenters ();
				
				if (m_ParticleData != null) {
					m_CloudRenderer.InitializeWithParticleData (m_ParticleData);
				}
			}
			
			if (Application.isPlaying) {
					// We only need the serializable particles for the initialization.
				m_ParticleData = null;
			}
		}
		
		private void OnDestroy () {
			if (!Application.isPlaying) {
				CloudRenderer.DestroyResources ();
			}
		}
		
		private void OnWillRenderObject () {	
			m_CloudRenderer.UpdateParticlesIfNeeded (false);
		}
		
		
		/// <summary>
		/// Initializes the renderer components.
		/// </summary>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public void InitializeRendererComponents () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			CloudRenderer.InitializeRendererComponents ();
		}
		
		
		#region Variable Preparations
		
		private void PrepareVerticalColors () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
				// Make sure the array is valid.
			bool l_ContainsPermanentZero = false;
			for (int i = 0; i < verticalColors.Length; i = i + 1) {
				CS_VerticalColor l_VerticalColor = verticalColors [i];
				if (l_VerticalColor.isPermanent && l_VerticalColor.verticalFactor == 0.0f) {
					l_ContainsPermanentZero = true;
					break;
				}
			}
			if (!l_ContainsPermanentZero) {
				CS_VerticalColor l_VerticalColor = new CS_VerticalColor ();
				l_VerticalColor.isPermanent = true;
				l_VerticalColor.verticalFactor = 0.0f;
				l_VerticalColor.color = Color.gray;
				System.Array.Resize (ref verticalColors, verticalColors.Length + 1);
				verticalColors [verticalColors.Length - 1] = l_VerticalColor;
			}
			
			bool l_ContainsPermanentOne = false;
			for (int i = 0; i < verticalColors.Length; i = i + 1) {
				CS_VerticalColor l_VerticalColor = verticalColors [i];
				if (l_VerticalColor.isPermanent && l_VerticalColor.verticalFactor == 1.0f) {
					l_ContainsPermanentOne = true;
					break;
				}
			}
			if (!l_ContainsPermanentOne) {
				CS_VerticalColor l_VerticalColor = new CS_VerticalColor ();
				l_VerticalColor.isPermanent = true;
				l_VerticalColor.verticalFactor = 1.0f;
				l_VerticalColor.color = Color.white;
				System.Array.Resize (ref verticalColors, verticalColors.Length + 1);
				verticalColors [verticalColors.Length - 1] = l_VerticalColor;
			}
			
			if (!l_ContainsPermanentZero || ! l_ContainsPermanentOne) {
				SortVerticalColors ();
			}
		}
		
		private void PrepareShadingColors () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
				// Make sure the array is valid.
			bool l_ContainsPermanentMinusOne = false;
			for (int i = 0; i < shadingColors.Length; i = i + 1) {
				CS_ShadingColor l_ShadingColor = shadingColors [i];
				if (l_ShadingColor.isPermanent && l_ShadingColor.shadingFactor == -1.0f) {
					l_ContainsPermanentMinusOne = true;
					break;
				}
			}
			if (!l_ContainsPermanentMinusOne) {
				CS_ShadingColor l_ShadingColor = new CS_ShadingColor ();
				l_ShadingColor.isPermanent = true;
				l_ShadingColor.shadingFactor = -1.0f;
				l_ShadingColor.color = Color.white;
				System.Array.Resize (ref shadingColors, shadingColors.Length + 1);
				shadingColors [shadingColors.Length - 1] = l_ShadingColor;
			}
			
			bool l_ContainsPermanentOne = false;
			for (int i = 0; i < shadingColors.Length; i = i + 1) {
				CS_ShadingColor l_ShadingColor = shadingColors [i];
				if (l_ShadingColor.isPermanent && l_ShadingColor.shadingFactor == 1.0f) {
					l_ContainsPermanentOne = true;
					break;
				}
			}
			if (!l_ContainsPermanentOne) {
				CS_ShadingColor l_ShadingColor = new CS_ShadingColor ();
				l_ShadingColor.isPermanent = true;
				l_ShadingColor.shadingFactor = 1.0f;
				l_ShadingColor.color = Color.gray;
				System.Array.Resize (ref shadingColors, shadingColors.Length + 1);
				shadingColors [shadingColors.Length - 1] = l_ShadingColor;
			}
			
			if (!l_ContainsPermanentMinusOne || !l_ContainsPermanentOne) {
				SortShadingColors ();
			}
		}
		
		private void PrepareShadingGroups () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
				// Make sure there is at least one shading group.
			if (shadingGroups.Length == 0) {
				System.Array.Resize (ref shadingGroups, 1);
				shadingGroups [0] = new CS_ShadingGroup ();
			}
		}
		
		private void PrepareParticleGroups () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
			if (m_CreatorData != null) {
				if (m_CreatorData.particleGroups.Length == 0) {
					System.Array.Resize (ref m_CreatorData.particleGroups, 1);
					CS_ParticleGroup l_ParticleGroup = new CS_ParticleGroup ();
					m_CreatorData.particleGroups [0] = l_ParticleGroup;
					System.Array.Resize (ref l_ParticleGroup.particleTypes, 1);
					CS_ParticleType l_ParticleType = new CS_ParticleType ();
					l_ParticleGroup.particleTypes [0] = l_ParticleType;
				} else {
					foreach (CS_ParticleGroup l_ParticleGroup in m_CreatorData.particleGroups) {
						if (l_ParticleGroup.particleTypes.Length == 0) {
							System.Array.Resize (ref l_ParticleGroup.particleTypes, 1);
							CS_ParticleType l_ParticleType = new CS_ParticleType ();
							l_ParticleGroup.particleTypes [0] = l_ParticleType;
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Prepares the material and tiling.
		/// </summary>
		private void PrepareMaterialAndTiling () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
				// This should only happen for the very first time. After
				// that no tiling value should become 0.
			if (m_XTile == 0 || m_YTile == 0) {
				Material l_Material = Resources.Load ("Materials/SoftCloudMaterial") as Material;
				if (l_Material != null) {
					m_ParticleMaterial = l_Material;
					m_XTile = 4;
					m_YTile = 4;
				} else {
					m_XTile = 1;
					m_YTile = 1;
				}
			}
		}
		
		/// <summary>
		/// Creates the prefab data. This method can only be called in the editor.
		/// </summary>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public void CreatePrefabData () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
			m_CreatorData = ScriptableObject.CreateInstance <CD> ();
			m_ParticleData = ScriptableObject.CreateInstance <PD> ();
			m_CreatorData.name = "CreatorData";
			m_ParticleData.name = "ParticleData";
			
			PrepareParticleGroups ();
		}
		
		#endregion
	}
}