//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using System.Threading;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Arguments for a thread of the multithreaded custom renderer.
	/// </summary>
	internal class CustomRendererArguments {
		
		/// <summary>
		/// Lower particle index.
		/// </summary>
		public int leftIndex;
		
		/// <summary>
		/// Upper particle index.
		/// </summary>
		public int rightIndex;
		
		/// <summary>
		/// The auto reset event.
		/// </summary>
		public AutoResetEvent autoResetEvent;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Edelweiss.CloudSystem.CustomRendererArguments"/> class.
		/// </summary>
		/// <param name='a_LeftIndex'>
		/// A left index.
		/// </param>
		/// <param name='a_RightIndex'>
		/// A right index.
		/// </param>
		/// <param name='a_AutoResetEvent'>
		/// A auto reset event.
		/// </param>
		public CustomRendererArguments (int a_LeftIndex, int a_RightIndex, AutoResetEvent a_AutoResetEvent) {
			leftIndex = a_LeftIndex;
			rightIndex = a_RightIndex;
			autoResetEvent = a_AutoResetEvent;
		}
	}
}