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
	/// Shading color for renderers that support shading groups.
	/// </summary>
	[System.Serializable]
	public class CS_ShadingColor : System.IComparable <CS_ShadingColor> {
		
		/// <summary>
		/// Is this instance a required entry of <see cref="F:Edelweiss.CloudSystem.Cloud`3.shadingColors"/>?
		/// </summary>
		public bool isPermanent = false;
		
		/// <summary>
		/// The shading factor. This value has to be in [-1.0f, 1.0f].
		/// </summary>
		public float shadingFactor = -1.0f;
		
		/// <summary>
		/// The color for the corresponding <see cref="F:Edelweiss.CloudSystem.CS_ShadingColor.shadingFactor"/>.
		/// </summary>
		public Color color = Color.white;
		
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
		public int CompareTo (CS_ShadingColor a_Other) {
			int l_Result = shadingFactor.CompareTo (a_Other.shadingFactor);
			if (l_Result == 0) {
				if (shadingFactor == -1.0f) {
					if (isPermanent != a_Other.isPermanent) {
						if (isPermanent) {
							l_Result = -1;
						} else if (a_Other.isPermanent) {
							l_Result = 1;
						}
					}
				} else if (shadingFactor == 1.0f) {
					if (isPermanent != a_Other.isPermanent) {
						if (isPermanent) {
							l_Result = 1;
						} else if (a_Other.isPermanent) {
							l_Result = -1;
						}
					}
				}
			}
			
			return (l_Result);
		}
	}
}
