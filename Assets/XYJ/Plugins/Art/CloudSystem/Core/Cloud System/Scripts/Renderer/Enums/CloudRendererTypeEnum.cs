//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System;
using System.Collections;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Cloud renderer type enum.
	/// </summary>
	public enum CloudRendererTypeEnum {
		
		/// <summary>
		/// Constant custom tint renderer.
		/// </summary>
		CustomTintRenderer = 0,
		
		/// <summary>
		/// Constant custom vertical color renderer.
		/// </summary>
		CustomVerticalColorRenderer = 1,
		
		/// <summary>
		/// Constant custom shading group renderer.
		/// </summary>
		CustomShadingGroupRenderer = 2,
		
		/// <summary>
		/// Constant custom vertical color with shading group renderer.
		/// </summary>
		CustomVerticalColorWithShadingGroupRenderer = 3,
		
		
		/// <summary>
		/// Constant simple custom tint renderer.
		/// </summary>
		SimpleCustomTintRenderer = 4,
		
		/// <summary>
		/// Constant simple custom vertical color renderer.
		/// </summary>
		SimpleCustomVerticalColorRenderer = 5,
		
		/// <summary>
		/// Constant simple custom shading group renderer.
		/// </summary>
		SimpleCustomShadingGroupRenderer = 6,
		
		/// <summary>
		/// Constant simple custom vertical color with shading group renderer.
		/// </summary>
		SimpleCustomVerticalColorWithShadingGroupRenderer = 7,
		
		
		/// <summary>
		/// Constant legacy tint renderer.
		/// </summary>
		LegacyTintRenderer = 8,
		
		/// <summary>
		/// Constant legacy vertical color renderer.
		/// </summary>
		LegacyVerticalColorRenderer = 9,
		
		/// <summary>
		/// Constant legacy shading group renderer.
		/// </summary>
		LegacyShadingGroupRenderer = 10,
		
		/// <summary>
		/// Constant legacy vertical color with shading group renderer.
		/// </summary>
		LegacyVerticalColorWithShadingGroupRenderer = 11,
		
		
		/// <summary>
		/// Constant shuriken tint renderer.
		/// </summary>
		ShurikenTintRenderer = 12,
		
		/// <summary>
		/// Constant shuriken vertical color renderer.
		/// </summary>
		ShurikenVerticalColorRenderer = 13,
		
		/// <summary>
		/// Constant shuriken shading group renderer.
		/// </summary>
		ShurikenShadingGroupRenderer = 14,
		
		/// <summary>
		/// Constant shuriken vertical color with shading group renderer.
		/// </summary>
		ShurikenVerticalColorWithShadingGroupRenderer = 15
	}
}
