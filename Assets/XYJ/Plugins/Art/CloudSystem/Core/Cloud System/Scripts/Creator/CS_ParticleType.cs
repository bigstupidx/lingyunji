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
	/// Particle type definitions used for the cloud creation.
	/// </summary>
	[System.Serializable]
	public class CS_ParticleType {
	
		/// <summary>
		/// The index of the uv.
		/// </summary>
		public int uvIndex;
		
		/// <summary>
		/// The minimum rotation.
		/// </summary>
		public float minimumRotation;
		
		/// <summary>
		/// The maximum rotation.
		/// </summary>
		public float maximumRotation;
		
		/// <summary>
		/// Is the particle squared?
		/// </summary>
		public bool squared;
		
		/// <summary>
		/// The minimum horizontal size.
		/// </summary>
		public float minimumXSize;
		
		/// <summary>
		/// The maximum horizontal size.
		/// </summary>
		public float maximumXSize;
		
		/// <summary>
		/// The minimum vertical size.
		/// </summary>
		public float minimumYSize;
		
		/// <summary>
		/// The maximum vertical size.
		/// </summary>
		public float maximumYSize;
		
		/// <summary>
		/// The creation probability.
		/// </summary>
		public float creationProbability;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Edelweiss.CloudSystem.CS_ParticleType"/> class.
		/// </summary>
		public CS_ParticleType () {
			uvIndex = 1;
			minimumRotation = -180.0f;
			maximumRotation = +180.0f;
			squared = true;
			minimumXSize = 1.0f;
			maximumXSize = 1.0f;
			minimumYSize = 1.0f;
			maximumYSize = 1.0f;
			creationProbability = 1.0f;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Edelweiss.CloudSystem.CS_ParticleType"/> class.
		/// </summary>
		/// <param name='a_Other'>
		/// The new instance becomes a copy of this parameter.
		/// </param>
		public CS_ParticleType (CS_ParticleType a_Other) {
			uvIndex = a_Other.uvIndex;
			minimumRotation = a_Other.minimumRotation;
			maximumRotation = a_Other.maximumRotation;
			squared = a_Other.squared;
			minimumXSize = a_Other.minimumXSize;
			maximumXSize = a_Other.maximumXSize;
			minimumYSize = a_Other.minimumYSize;
			maximumYSize = a_Other.maximumYSize;
			creationProbability = 1.0f;
		}
	}
}
