using UnityEngine;
using System.Collections;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Needed to make number of available threads lockable among the different threads.
	/// </summary>
	internal class AvailableThreads {
		
		public int totalThreadsCount;
		public int startedThreads;
		
		public int RemainingThreads {
			get {
				return (totalThreadsCount - startedThreads);
			}
		}
		
		public AvailableThreads (int a_TotalThreadsCount) {
			totalThreadsCount = a_TotalThreadsCount;
		}
	}
}
