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
	/// Shading groups are used for that special kind of rendering.
	/// </summary>
	[System.Serializable]
	public class CS_ShadingGroup {
		
		/// <summary>
		/// The name for the identification in the inspector.
		/// </summary>
		public string name;
		
		/// <summary>
		/// Center of this shading group. If you modify this value at runtime, you
		/// need to recalculate <see cref="F:Edelweiss.CloudSystem.CS_ShadingGroup.scaledCenter"/> too.
		/// <see cref="M:Edelweiss.CloudSystem.CS_ShadingGroup.RecalculateScaledCenter(Edelweiss.CloudSystem.CloudBase)"/> can be used for that.
		/// </summary>
		public Vector3 center;
		
		/// <summary>
		/// The center which takes the scaling of the cloud into account.
		/// </summary>
		[System.NonSerialized] public Vector3 scaledCenter;
		
		/// <summary>
		/// The color of the shape in the scene view.
		/// </summary>
		public Color shapeColor;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Edelweiss.CloudSystem.CS_ShadingGroup"/> class.
		/// </summary>
		public CS_ShadingGroup () {
			name = "Shading Group";
			center = Vector3.zero;
			shapeColor = new Color (0.0f, 1.0f, 0.0f);
		}
		
		/// <summary>
		/// Recalculates the <see cref="F:Edelweiss.CloudSystem.CS_ShadingGroup.scaledCenter"/>.
		/// </summary>
		/// <param name='a_Cloud'>
		/// The cloud.
		/// </param>
		public void RecalculateScaledCenter (CloudBase a_Cloud) {
			scaledCenter = a_Cloud.Scale * center;
			a_Cloud.SetParticleSystemHasChanged ();
		}
	}
}
