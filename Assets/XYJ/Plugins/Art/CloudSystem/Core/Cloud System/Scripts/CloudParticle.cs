//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Particles for the Cloud System.
	/// </summary>
	public struct CloudParticle : System.IComparable <CloudParticle> {
		
		/// <summary>
		/// The original index.
		/// </summary>
		public int originalIndex;
		
		/// <summary>
		/// The local position.
		/// </summary>
		public Vector3 position;
		
		/// <summary>
		/// The z rotation.
		/// </summary>
		public float rotation;
		
		/// <summary>
		/// The size.
		/// </summary>
		public Vector2 size;
		
		/// <summary>
		/// The index of the uv.
		/// </summary>
		public int uvIndex;
		
		/// <summary>
		/// Is the particle squared?
		/// </summary>
		public bool isSquared;
		
		/// <summary>
		/// Is this a core particle?
		/// </summary>
		public bool isCoreParticle;
		
		/// <summary>
		/// The index of the shading group.
		/// </summary>
		public int shadingGroupIndex;
		
		/// <summary>
		/// The distance to the camera.
		/// </summary>
		public float distanceToCamera;
		
		/// <summary>
		/// The color of the particle. It is only take into account if
		/// the cloud <see cref="P:Edelweiss.CloudSystem.Cloud`3.UseParticleColor">uses particle colors</see>.
		/// </summary>
		public Color particleColor;
		
		/// <summary>
		/// Initializes the values.
		/// </summary>
		public void InitializeValues () {
			position = Vector3.zero;
			rotation = 0.0f;
			size = Vector2.one;
			uvIndex = 1;
			isSquared = false;
			isCoreParticle = false;
			shadingGroupIndex = 0;
			particleColor = Color.white;
		}
		
		/// <summary>
		/// The radius of the particle.
		/// </summary>
		public float Radius () {
			float l_Result;
			if (isSquared) {
				l_Result = Mathf.Sqrt (2.0f * size.x * size.x);
			} else {
				l_Result = size.magnitude;
			}
			return (l_Result);
		}
		
		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// Less than zero: This object is less than the parameter.
		/// Zero: This object is equal to the parameter.
		/// Greater than zero: This object is greater than the parameter.
		/// </returns>
		/// <param name='a_Other'>
		/// An object to compare with this object.
		/// </param>
		public int CompareTo (CloudParticle a_Other) {
			return (- distanceToCamera.CompareTo (a_Other.distanceToCamera));
		}
	}
}

