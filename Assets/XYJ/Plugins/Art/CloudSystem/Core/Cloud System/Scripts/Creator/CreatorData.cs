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
	/// Generic base class of <see cref="T:CS_CreatorData"/>.
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
	public abstract class CreatorData <C, PD, CD> : CreatorDataBase
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		/// <summary>
		/// Create the cloud particles with this density which is a valie from [0.0f, 1.0f].
		/// </summary>
		public float density = 1.0f;
		
		/// <summary>
		/// The cloud particles are created within those shapes.
		/// </summary>
		public CS_Shape[] boxShapes = new CS_Shape [0];
		
		/// <summary>
		/// The particle groups define which kind and amount of particles are created.
		/// </summary>
		public CS_ParticleGroup[] particleGroups = new CS_ParticleGroup [0];
	}
}
