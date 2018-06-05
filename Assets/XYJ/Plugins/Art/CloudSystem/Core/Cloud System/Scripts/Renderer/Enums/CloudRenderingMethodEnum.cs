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
	/// Cloud rendering method enum.
	/// </summary>
	public enum CloudRenderingMethodEnum {
		
		/// <summary>
		/// Constant tint.
		/// </summary>
		Tint = 0,
		
		/// <summary>
		/// Constant vertical color.
		/// </summary>
		VerticalColor = 1,
		
		/// <summary>
		/// Constant shading group.
		/// </summary>
		ShadingGroup = 2,
		
		/// <summary>
		/// Constant vertical color with shading group.
		/// </summary>
		VerticalColorWithShadingGroup = 3
	}
}