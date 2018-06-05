//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Boxes to build the cloud shape needed by the cloud creator.
	/// </summary>
	[System.Serializable]
	public class CS_Shape {
		
		/// <summary>
		/// The maximal number of particles that is created by the cloud creator for this shape.
		/// </summary>
		public int particleCount;
		
		/// <summary>
		/// The position of this shape in local space.
		/// </summary>
		public Vector3 position;
		
		/// <summary>
		/// The rotation of this shape in local space.
		/// </summary>
		public Vector3 rotation;
		
		/// <summary>
		/// The scale of this shape in local space.
		/// </summary>
		public Vector3 scale;
		
		/// <summary>
		/// The index of the particle group based on which the particles are created.
		/// </summary>
		public int particleGroupIndex;
		
		/// <summary>
		/// The index of the shading group to which the created particles will belong.
		/// </summary>
		public int shadingGroupIndex;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Edelweiss.CloudSystem.CS_Shape"/> class.
		/// </summary>
		public CS_Shape () {
			particleCount = 100;
			position = Vector3.zero;
			rotation = Quaternion.identity.eulerAngles;
			scale = Vector3.one;
			particleGroupIndex = 0;
			shadingGroupIndex = 0;
		}
		
		/// <summary>
		/// Copy this instance.
		/// </summary>
		public CS_Shape Copy () {
			CS_Shape l_Result = new CS_Shape ();
			l_Result.position = position;
			l_Result.rotation = rotation;
			l_Result.scale = scale;
			l_Result.particleCount = particleCount;
			l_Result.particleGroupIndex = particleGroupIndex;
			l_Result.shadingGroupIndex = shadingGroupIndex;
			return (l_Result);
		}
		
		/// <summary>
		/// Local to world matrix of this shape.
		/// </summary>
		/// <returns>
		/// The resulting matrix.
		/// </returns>
		public Matrix4x4 LocalTransformMatrix () {
			Matrix4x4 l_Result = Matrix4x4.TRS (position, Quaternion.Euler (rotation), Vector3.one);
			return (l_Result);
		}
	}
}

