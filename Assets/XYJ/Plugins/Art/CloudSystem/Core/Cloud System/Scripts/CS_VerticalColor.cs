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
	/// Vertical colors for renderers that support vertical colors.
	/// </summary>
	[System.Serializable]
	public class CS_VerticalColor : System.IComparable <CS_VerticalColor> {
		
		/// <summary>
		/// Is this instance a required entry of <see cref="F:Edelweiss.CloudSystem.Cloud`3.verticalColors"/>?
		/// </summary>
		public bool isPermanent = false;
		
		/// <summary>
		/// The vertical factor, meaning the height within the cloud. This value has to be in [0.0f, 1.0f].
		/// </summary>
		public float verticalFactor = 0.0f;
		
		/// <summary>
		/// The color for the corresponding <see cref="F:Edelweiss.CloudSystem.CS_VerticalColor.verticalFactor"/>.
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
		public int CompareTo (CS_VerticalColor a_Other) {
			int l_Result = verticalFactor.CompareTo (a_Other.verticalFactor);
			if (l_Result == 0) {
				if (verticalFactor == 0.0f) {
					if (isPermanent != a_Other.isPermanent) {
						if (isPermanent) {
							l_Result = -1;
						} else if (a_Other.isPermanent) {
							l_Result = 1;
						}
					}
				} else if (verticalFactor == 1.0f) {
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
