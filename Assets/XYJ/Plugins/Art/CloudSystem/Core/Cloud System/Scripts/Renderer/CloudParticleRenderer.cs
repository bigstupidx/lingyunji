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
	/// Abstract definition for a cloud renderer that uses CS_Particle's internally.
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
	public abstract class CloudParticleRenderer <C, PD, CD> : CloudRenderer <C, PD, CD>
		where C : Cloud <C, PD, CD>
		where PD : ParticleData <C, PD, CD>
		where CD : CreatorData <C, PD, CD>
	{
		/// <summary>
		/// The particles.
		/// </summary>
		protected CloudParticle[] m_Particles = new CloudParticle [0];

		/// <inheritdoc/>
		public override int Count {
			get {
				return (m_Particles.Length);
			}
		}

		/// <inheritdoc/>
		public override CloudParticle ParticleAt (int a_Index) {
			return (m_Particles [a_Index]);
		}

		/// <inheritdoc/>
		public override void SetParticleAt (int a_Index, CloudParticle a_Particle) {
			m_Particles [a_Index] = a_Particle;
			SetParticleSystemHasChanged ();
		}
	}
}
