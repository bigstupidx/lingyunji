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
	/// Edition information.
	/// </summary>
	public class Edition {
	
		/// <summary>
		/// Gets a value indicating whether this is Cloud System Free.
		/// </summary>
		/// <value>
		/// <c>true</c> if this is Cloud System Free; otherwise, <c>false</c>.
		/// </value>
		public static bool IsCloudSystemFree {
			get {
				bool l_Result = false;
#if CLOUD_SYSTEM_FREE
				l_Result = true;
#endif
				return (l_Result);
			}
		}
	}
}