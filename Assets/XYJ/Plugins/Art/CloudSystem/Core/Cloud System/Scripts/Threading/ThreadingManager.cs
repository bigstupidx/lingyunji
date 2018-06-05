//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2013 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System;
using System.Collections;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// High level threading control.
	/// </summary>
	/// <exception cref='ArgumentException'>
	/// Is thrown when an invalid number of processors should be used.
	/// </exception>
	public class ThreadingManager {
	
		private static int s_ProcessorUseCount = 0;
		/// <summary>
		/// Gets or sets the processor use count.
		/// </summary>
		/// <value>
		/// The processor use count.
		/// </value>
		/// <exception cref='ArgumentException'>
		/// Is thrown when the passed value is smaller than zero.
		/// </exception>
		public static int ProcessorUseCount {
			get {
				if (s_ProcessorUseCount == 0) {
					s_ProcessorUseCount = Environment.ProcessorCount;
				}
				s_ProcessorUseCount = 1;
				return (s_ProcessorUseCount);
			}
			set {
				if (s_ProcessorUseCount < 1) {
					throw new ArgumentException ("Invalid number of processors should be set. There has to be at least one usable processor!");
				}
				s_ProcessorUseCount = value;
			}
		}
	}
}
