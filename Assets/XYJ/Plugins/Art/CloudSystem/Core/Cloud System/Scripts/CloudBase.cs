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
	/// Base class for the generic <see cref="T:Edelweiss.CloudSystem.Cloud`3"/> class.
	/// This class was created to preserve the compatibility with clouds created before the Cloud System dll's were used.
	/// </summary>
	public abstract class CloudBase : MonoBehaviour {
		
		/// <summary>
		/// Scale of this cloud. You can not change this value at runtime.
		/// Instead you have to iterate through all particles and multiply the
		/// position and particle size by the needed factor.
		/// The scale is not allowed to be smaller than zero.
		/// </summary>
		/// <value>
		/// The scale.
		/// </value>
		public abstract float Scale {
			get;
			set;
		}
		
		/// <summary>
		/// This method tells the renderer that all particles need to
		/// be recalculated. It is mostly called automatically, such that
		/// you don't have to deal with it.
		/// But there are two exceptions. If you make changes in 'verticalColors'
		/// or 'shadingColors', you have to call this method in order to update
		/// the cloud's particles.
		/// </summary>
		public abstract void SetParticleSystemHasChanged ();
	}
}