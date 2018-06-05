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
	/// Abstract definition for a simple custom cloud renderer.
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
	public abstract class SimpleCustomCloudRenderer <C, PD, CD> : ColoredCloudParticleRenderer <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{	
		[HideInInspector][SerializeField] private MeshFilter m_MeshFilter;
		[HideInInspector][SerializeField] private MeshRenderer m_MeshRenderer;
		[HideInInspector][SerializeField] private Vector2[,] m_UVLookupTable;
		
			// HACK
		internal CloudRendererData m_CloudRendererData;
		
		private Matrix4x4 m_CloudToCameraSpaceMatrix;
		private Matrix4x4 m_ParticleToCameraSpaceMatrix;
		private Mesh m_Mesh;
		private Vector3[] m_Vertices;
		private Vector2[] m_UVs;
		private Color[] m_VertexColors;
		private int[] m_Triangles;
		
		private Quaternion m_PreviousCloudRotation;
		private Quaternion m_PreviousCameraRotation;
				
		private AutoResetEvent[] m_AutoResetEvents;
		private CustomRendererArguments[] m_CustomRendererArguments;
		private WaitCallback[] m_UpdateParticleColorsCallbacks;
		private WaitCallback[] m_UpdateDistanceToCameraCallbacks;
		private WaitCallback[] m_UpdateMeshCallbacks;
		
		/// <inheritdoc/>
		/// <exception cref='System.InvalidOperationException'>
		/// Is thrown when this operation is called while the application is playing.
		/// </exception>
		public override void InitializeRendererComponents () {
			if (Application.isPlaying) {
				throw new System.InvalidOperationException (ExceptionSupport.c_EditorOnlyExceptionText);
			}
			
			m_MeshFilter = GetComponent <MeshFilter> ();
			if (m_MeshFilter == null) {
				m_MeshFilter = gameObject.AddComponent <MeshFilter> ();
			}
			
			m_Mesh = new Mesh ();
			m_Mesh.name = "Cloud Mesh";
			m_MeshFilter.sharedMesh = m_Mesh;
			
			m_MeshRenderer = GetComponent <MeshRenderer> ();
			if (m_MeshRenderer == null) {
				m_MeshRenderer = gameObject.AddComponent <MeshRenderer> ();
			}
			
			ChangedMaterial ();
			m_MeshRenderer.receiveShadows = false;
			
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
			
			if (m_MeshRenderer != null) {
				DestroyImmediate (m_MeshRenderer);
			}
	
			if (m_MeshFilter != null) {
				DestroyImmediate (m_MeshFilter.sharedMesh);
				DestroyImmediate (m_MeshFilter);
			}
	
			if (m_Mesh != null) {
				DestroyImmediate (m_Mesh);
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
			
			m_MeshRenderer.hideFlags = HideFlags.HideInInspector;
			m_MeshFilter.hideFlags = HideFlags.HideInInspector;
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
			
			if (m_MeshFilter != null) {
				DestroyImmediate (m_MeshFilter.sharedMesh);
			}
		}
		
		/// <inheritdoc/>
		public override void InitializeWithParticleData (ParticleDataBase a_ParticleData) {
			PrepareMembersForCloudData (a_ParticleData);
			PrepareMultithreading ();
			RecalculateTiling ();
			
			for (int i = 0; i < m_Particles.Length; i = i + 1) {
				
					// Get particle data and apply the scales
				CloudParticle l_Particle = a_ParticleData [i];
				l_Particle.originalIndex = i;
				l_Particle.position = Cloud.Scale * l_Particle.position;
				l_Particle.size = Cloud.Scale * l_Particle.size;
				m_Particles [i].particle = l_Particle;
				
					// Calculate uv's and triangles
				int l_VertexIndex = i * 4;
				
				m_UVs [l_VertexIndex + 0] = m_UVLookupTable [l_Particle.uvIndex, 0];
				m_UVs [l_VertexIndex + 1] = m_UVLookupTable [l_Particle.uvIndex, 1];
				m_UVs [l_VertexIndex + 2] = m_UVLookupTable [l_Particle.uvIndex, 2];
				m_UVs [l_VertexIndex + 3] = m_UVLookupTable [l_Particle.uvIndex, 3];
				
				int l_TriangleIndex = i * 6;
				m_Triangles [l_TriangleIndex + 0] = l_VertexIndex + 2;
				m_Triangles [l_TriangleIndex + 1] = l_VertexIndex + 1;
				m_Triangles [l_TriangleIndex + 2] = l_VertexIndex + 0;
				m_Triangles [l_TriangleIndex + 3] = l_VertexIndex + 1;
				m_Triangles [l_TriangleIndex + 4] = l_VertexIndex + 2;
				m_Triangles [l_TriangleIndex + 5] = l_VertexIndex + 3;
			}
			
			m_Mesh.Clear ();
			m_Mesh.vertices = m_Vertices;
			m_Mesh.normals = null;
			m_Mesh.uv = m_UVs;
			m_Mesh.colors = m_VertexColors;
			m_Mesh.triangles = m_Triangles;
			m_Mesh.bounds = Cloud.BoundingBox;
			
			SetParticleSystemHasChanged ();
			UpdateParticlesIfNeeded (true);
		}
		
		private void PrepareMembersForCloudData (ParticleDataBase a_ParticleData) {
			if (m_Particles == null ||
			    m_Particles.Length != a_ParticleData.Count ||
			    m_Vertices == null)
			{
				m_Particles = new ColoredCloudParticle [a_ParticleData.Count];
				m_Vertices = new Vector3 [m_Particles.Length * 4];
				m_UVs = new Vector2 [m_Particles.Length * 4];
				m_VertexColors = new Color [m_Particles.Length * 4];
				m_Triangles = new int [m_Particles.Length * 3 * 2];
			}
			
			if (m_Mesh == null) {
				if (Application.isPlaying) {
					m_Mesh = m_MeshFilter.mesh;
				} else if (m_MeshFilter != null) {
					m_Mesh = new Mesh ();
					m_Mesh.name = "Cloud Mesh";
					m_MeshFilter.sharedMesh = m_Mesh;
				}
			}
		}
		
		private void PrepareMultithreading () {
			m_CloudRendererData = Cloud.CloudRendererData;
			
			int l_Processors = ThreadingManager.ProcessorUseCount;
			//int l_Processors = 1;
			
			m_AutoResetEvents = new AutoResetEvent [l_Processors];
			for (int i = 0; i < m_AutoResetEvents.Length; i = i + 1) {
				m_AutoResetEvents [i] = new AutoResetEvent (false);
			}
			m_CustomRendererArguments = new CustomRendererArguments [l_Processors];
			int l_ParticlesPerProcessor = m_Particles.Length / l_Processors;
			int l_Index = 0;
			for (int i = 0; i < m_CustomRendererArguments.Length; i = i + 1) {
				int l_LeftIndex = l_Index;
				int l_RightIndex = l_LeftIndex + l_ParticlesPerProcessor;
				if (i == m_CustomRendererArguments.Length - 1) {
					l_RightIndex = m_Particles.Length;
				}
				m_CustomRendererArguments [i] = new CustomRendererArguments (l_LeftIndex, l_RightIndex, m_AutoResetEvents [i]);
				l_Index = l_RightIndex;
			}
			m_UpdateParticleColorsCallbacks = new WaitCallback [l_Processors];
			m_UpdateDistanceToCameraCallbacks = new WaitCallback [l_Processors];
			m_UpdateMeshCallbacks = new WaitCallback [l_Processors];
			for (int i = 0; i < m_UpdateDistanceToCameraCallbacks.Length; i = i + 1) {
				m_UpdateParticleColorsCallbacks [i] = new WaitCallback (UpdateParticleColors);
				m_UpdateDistanceToCameraCallbacks [i] = new WaitCallback (UpdateDistanceToCamera);
				m_UpdateMeshCallbacks [i] = new WaitCallback (UpdateMesh);
			}
		}
		
		/// <inheritdoc/>
		public override void RecalculateTiling () {
			m_UVLookupTable = new Vector2 [Cloud.TileCount + 1, 4];
			
			int l_XTile = Cloud.XTile;
			int l_YTile = Cloud.YTile;
			
			float l_DeltaU = 1.0f / l_XTile;
			float l_DeltaV = 1.0f / l_YTile;
			
			for (int j = 0; j < l_YTile; j = j + 1) {
				
				float l_VTop = (l_YTile - j) * l_DeltaV;
				float l_VBottom = (l_YTile - j - 1) * l_DeltaV;
				
				for (int i = 0; i < l_XTile; i = i + 1) {
					
					float l_ULeft = i * l_DeltaU;
					float l_URight = (i + 1) * l_DeltaU;
					
					int l_Index = i + (j * l_XTile) + 1;
					
					m_UVLookupTable [l_Index, 0] = new Vector2 (l_ULeft, l_VTop);
					m_UVLookupTable [l_Index, 1] = new Vector2 (l_ULeft, l_VBottom);
					m_UVLookupTable [l_Index, 2] = new Vector2 (l_URight, l_VTop);
					m_UVLookupTable [l_Index, 3] = new Vector2 (l_URight, l_VBottom);
				}
			}
			m_HasParticleSystemChanged = true;
		}
		
		/// <inheritdoc/>
		public override void ChangedMaterial () {
			m_MeshRenderer.sharedMaterial = Cloud.ParticleMaterial;
		}	
		
		private void UpdatePreviousCloudTransform (Transform a_CloudTransform) {
			m_PreviousCloudRotation = a_CloudTransform.rotation;
		}
		
		private bool HasCloudTransformChanged (Transform a_CloudTransform) {
			return (m_PreviousCloudRotation != a_CloudTransform.rotation);
		}
		
		private void UpdatePreviousCameraTransform (Transform a_CameraTransform) {
			m_PreviousCameraRotation = a_CameraTransform.rotation;
		}
		
		private bool HasCameraTransformChanged (Transform a_CameraTransform) {
			return (m_PreviousCameraRotation != a_CameraTransform.rotation);
		}
		
		/// <inheritdoc/>
		public override void UpdateParticlesIfNeeded (bool a_IsEnabling) {
			
				// HACK: This updates the cloud renderer data
			m_CloudRendererData = Cloud.CloudRendererData;
			
			if (m_CustomRendererArguments == null) {
				return;
			}
			
				// HACK: Ugly workaround for a Unity crash bug!
			if (a_IsEnabling || Camera.current != null) {
				Transform l_CachedCameraTransform;
				
					// HACK: Ugly workaround for a Unity crash bug!
				if (a_IsEnabling) {
					l_CachedCameraTransform = transform;
				} else {
					l_CachedCameraTransform = Camera.current.transform;
				}
				
				UpdateCurrentSunTransform ();
				
					// Recalculate the particle colors if needed.
				if
					(m_HasParticleSystemChanged ||
					 ((SupportsShadingGroups || SupportsVerticalColors) &&
					  (HasCloudTransformChanged (Cloud.CachedTransform) ||
					   HasCameraTransformChanged (l_CachedCameraTransform) ||
					   HasSunTransformChanged ())))
				{
					if (m_CustomRendererArguments.Length < 2) {
						UpdateParticleColors (m_CustomRendererArguments [0]);
					} else {
						
						for (int i = 0; i < m_CustomRendererArguments.Length; i = i + 1) {
							ThreadPool.QueueUserWorkItem (m_UpdateParticleColorsCallbacks [i], m_CustomRendererArguments [i]);
						}
						WaitHandle.WaitAll (m_AutoResetEvents);
					}
				}
				
				if
					(m_HasParticleSystemChanged ||
					 HasCloudTransformChanged (Cloud.CachedTransform) ||
					 HasCameraTransformChanged (l_CachedCameraTransform) ||
					 HasSunTransformChanged ())
				{
					m_CloudToCameraSpaceMatrix = l_CachedCameraTransform.worldToLocalMatrix * Cloud.TransformMatrix ();
					m_ParticleToCameraSpaceMatrix = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (Cloud.CachedTransform.rotation) * l_CachedCameraTransform.rotation, Vector3.one);
	
					if (m_CustomRendererArguments.Length < 2) {
						
							// Sort the particles in camera space.
						UpdateDistanceToCamera (m_CustomRendererArguments [0]);
						QuickSort <ColoredCloudParticle>.ThreadedSort (m_Particles);
						
							// Update the mesh.
						UpdateMesh (m_CustomRendererArguments [0]);
						
					} else {
						
							// Sort the particles in camera space.
						for (int i = 0; i < m_CustomRendererArguments.Length; i = i + 1) {
							ThreadPool.QueueUserWorkItem (m_UpdateDistanceToCameraCallbacks [i], m_CustomRendererArguments [i]);
						}
						WaitHandle.WaitAll (m_AutoResetEvents);
						QuickSort <ColoredCloudParticle>.ThreadedSort (m_Particles);
						
							// Update the mesh.
						for (int i = 0; i < m_CustomRendererArguments.Length; i = i + 1) {
							ThreadPool.QueueUserWorkItem (m_UpdateMeshCallbacks [i], m_CustomRendererArguments [i]);
						}
						WaitHandle.WaitAll (m_AutoResetEvents);
					}

					m_Mesh.vertices = m_Vertices;
					m_Mesh.normals = null;
					m_Mesh.uv = m_UVs;
					m_Mesh.colors = m_VertexColors;
					
						// Update the previous values
					UpdatePreviousCloudTransform (Cloud.CachedTransform);
					UpdatePreviousCameraTransform (l_CachedCameraTransform);
					UpdatePreviousSunTransform ();
					
						// HACK: Needed for Unity crash bug workaround.
					if (a_IsEnabling) {
						m_HasParticleSystemChanged = true;
					} else {
						m_HasParticleSystemChanged = false;
					}
				}
			}
		}
		
		/// <inheritdoc/>
		public override void BoundingBoxUpdated () {
			if (m_Mesh) {
				m_Mesh.bounds = Cloud.BoundingBox;
			}
		}

		/// <summary>
		/// Updates the particle colors.
		/// </summary>
		/// <param name='a_State'>
		/// The required data.
		/// </param>
		protected abstract void UpdateParticleColors (object a_State);
		
		private void UpdateDistanceToCamera (object a_State) {
			CustomRendererArguments l_Arguments = (CustomRendererArguments) a_State;
			int l_LeftIndex = l_Arguments.leftIndex;
			int l_RightIndex = l_Arguments.rightIndex;
			AutoResetEvent l_AutoResetEvent = l_Arguments.autoResetEvent;
			
			for (int i = l_LeftIndex; i < l_RightIndex; i = i + 1) {
				ColoredCloudParticle l_Particle = m_Particles [i];
				Vector3 l_PositionInCameraSpace = m_CloudToCameraSpaceMatrix.MultiplyPoint3x4 (l_Particle.particle.position);
				l_Particle.particle.distanceToCamera = l_PositionInCameraSpace.z;
				m_Particles [i] = l_Particle;
			}
			
			l_AutoResetEvent.Set ();
		}
		
		private void UpdateMesh (object a_State) {
			CustomRendererArguments l_Arguments = (CustomRendererArguments) a_State;
			int l_LeftIndex = l_Arguments.leftIndex;
			int l_RightIndex = l_Arguments.rightIndex;
			AutoResetEvent l_AutoResetEvent = l_Arguments.autoResetEvent;
		
			for (int i = l_LeftIndex; i < l_RightIndex; i = i + 1) {
				
					// Slower than the actual implementation
				//ColoredCloudParticle l_ColoredParticle = m_Particles [i];
				//CloudParticle l_Particle = l_ColoredParticle.particle;
				//Color l_Color = l_ColoredParticle.color;
				
				CloudParticle l_Particle = m_Particles [i].particle;
				Color l_Color = m_Particles [i].color;
				
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
							
				m_VertexColors [l_Index + 0] = l_Color;
				m_VertexColors [l_Index + 1] = l_Color;
				m_VertexColors [l_Index + 2] = l_Color;
				m_VertexColors [l_Index + 3] = l_Color;
			}
			l_AutoResetEvent.Set ();
		}
	}
}
