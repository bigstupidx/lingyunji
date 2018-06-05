//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System;
using System.Collections;

namespace Edelweiss.CloudSystem {

	/// <summary>
	/// Exception support.
	/// </summary>
	internal class ExceptionSupport {
	
		/// <summary>
		/// Editor only exception text.
		/// </summary>
		public const string c_EditorOnlyExceptionText = "This operation can only be called in the Unity Editor and never if the application is playing!";		
	}
}
