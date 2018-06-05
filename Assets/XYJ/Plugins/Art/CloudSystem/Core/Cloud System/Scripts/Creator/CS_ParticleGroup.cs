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
	/// Collection of particle types needed by the cloud creator.
	/// </summary>
	[System.Serializable]
	public class CS_ParticleGroup {
		
		/// <summary>
		/// The name for an identification in the inspector.
		/// </summary>
		public string name;
		
		/// <summary>
		/// The color of the shape in the scene view.
		/// </summary>
		public Color shapeColor;
		
		/// <summary>
		/// The particle types.
		/// </summary>
		public CS_ParticleType[] particleTypes;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Edelweiss.CloudSystem.CS_ParticleGroup"/> class.
		/// </summary>
		public CS_ParticleGroup () {
			name = "Particle Group";
			shapeColor = Color.yellow;
			particleTypes = new CS_ParticleType [0];
		}
	}
}