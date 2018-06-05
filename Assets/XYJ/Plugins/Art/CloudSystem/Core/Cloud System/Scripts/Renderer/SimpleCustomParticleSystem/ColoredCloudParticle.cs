//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Colored particles for the Cloud System, used for the fast custom particle system.
	/// </summary>
	public struct ColoredCloudParticle : System.IComparable <ColoredCloudParticle> {
	
		/// <summary>
		/// The particle.
		/// </summary>
		public CloudParticle particle;
		
		/// <summary>
		/// The color.
		/// </summary>
		public Color color;
		
		/// <summary>
		/// Initializes the values.
		/// </summary>
		public void InitializeValues () {
			particle.InitializeValues ();
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
		public int CompareTo (ColoredCloudParticle a_Other) {
			return (- particle.distanceToCamera.CompareTo (a_Other.particle.distanceToCamera));
		}
	}
}
