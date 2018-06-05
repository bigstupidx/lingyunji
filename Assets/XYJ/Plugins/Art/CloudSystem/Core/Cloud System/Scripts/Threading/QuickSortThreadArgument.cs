using UnityEngine;
using System;
using System.Collections;
using System.Threading;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Arguments need to be bundled for the threading.
	/// </summary>
	internal class QuickSortThreadArguments <U> where U : IComparable <U> {
		public U[] array;
		public int leftIndex;
		public int rightIndex;
		public AutoResetEvent autoResetEvent;
		
		public QuickSortThreadArguments (AutoResetEvent a_AutoResetEvent) {
			autoResetEvent = a_AutoResetEvent;
		}
		
		public QuickSortThreadArguments (U[] a_Array, int a_LeftIndex, int a_RightIndex, AutoResetEvent a_AutoResetEvent) {
			array = a_Array;
			leftIndex = a_LeftIndex;
			rightIndex = a_RightIndex;
			autoResetEvent = a_AutoResetEvent;
		}
		
		public void Initialize (U[] a_Array, int a_LeftIndex, int a_RightIndex) {
			array = a_Array;
			leftIndex = a_LeftIndex;
			rightIndex = a_RightIndex;
		}
	}
}