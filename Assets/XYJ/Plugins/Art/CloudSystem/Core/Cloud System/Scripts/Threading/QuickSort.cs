//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System;
using System.Collections;
using System.Threading;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// Threaded and sequential quick sort.
	/// </summary>
	/// <typeparam name="T">
	/// The comparable type which's instances are going to be sorted.
	/// </typeparam>
	internal class QuickSort <T> where T : IComparable <T> {
		
		private static AvailableThreads s_AvailableThreads;
		private static WaitCallback[] s_WaitCallbacks;
		private static QuickSortThreadArguments <T> [] s_QuickSortThreadArguments;
		private static AutoResetEvent[][] s_AutoResetEvents;
		
		/// <summary>
		/// Threaded quick sort.
		/// </summary>
		/// <param name='a_Array'>
		/// A_ array.
		/// </param>
		public static void ThreadedSort (T[] a_Array) {
			PrepareThreading ();
			AvailableThreads l_AvailableThreads = new AvailableThreads (ThreadingManager.ProcessorUseCount - 1);
			if (l_AvailableThreads.totalThreadsCount > 0) {
				QuickSortThreadArguments <T> l_Arguments = s_QuickSortThreadArguments [0];
				l_Arguments.Initialize (a_Array, 0, a_Array.Length - 1);
				ThreadedSort ((object) l_Arguments);
			} else {
				Sort (a_Array);
			}
		}
		
		/// <summary>
		/// All objects that are needed for multithreading are reused to make the life of the
		/// garbage collector easier.
		/// </summary>
		private static void PrepareThreading () {
			int l_Processes = ThreadingManager.ProcessorUseCount;
			if (s_AvailableThreads == null) {
				s_AvailableThreads = new AvailableThreads (l_Processes - 1);
				s_AvailableThreads.startedThreads = 0;
				s_WaitCallbacks = new WaitCallback [l_Processes - 1];
				for (int i = 0; i < s_WaitCallbacks.Length; i = i + 1) {
					s_WaitCallbacks [i] = new WaitCallback (ThreadedSort);
				}
				s_AutoResetEvents = new AutoResetEvent [l_Processes][];
				s_QuickSortThreadArguments = new QuickSortThreadArguments <T> [l_Processes];
				for (int i = 0; i < s_AutoResetEvents.Length; i = i + 1) {
					AutoResetEvent l_AutoResetEvent = new AutoResetEvent (false);
					s_AutoResetEvents [i] = new AutoResetEvent[] {l_AutoResetEvent};
					s_QuickSortThreadArguments [i] = new QuickSortThreadArguments <T> (l_AutoResetEvent);
				}
			} else {
				s_AvailableThreads.totalThreadsCount = l_Processes - 1;
				s_AvailableThreads.startedThreads = 0;
			}
		}
		
		/// <summary>
		/// Threaded quick sort for a certain range.
		/// </summary>
		/// <param name='a_State'>
		/// This has to be a <see cref="T:Edelweiss.CloudSystem.QuickSortThreadArguments`1"/> instance with valid members. <see cref="System.Object"/>
		/// </param>
		private static void ThreadedSort (object a_State) {
			QuickSortThreadArguments <T> l_Arguments = (QuickSortThreadArguments <T>) a_State;
			
			T[] l_Array = l_Arguments.array;
			int l_LeftIndex = l_Arguments.leftIndex;
			int l_RightIndex = l_Arguments.rightIndex;
			AutoResetEvent l_AutoResetEvent = l_Arguments.autoResetEvent;
			
			if (l_LeftIndex < l_RightIndex) {
				
					// Partition
				int l_PivotIndex = (l_LeftIndex + l_RightIndex) / 2;
				T l_PivotValue = l_Array [l_PivotIndex];
				SwapElements (l_Array, l_PivotIndex, l_RightIndex);
				int l_StoreIndex = l_LeftIndex;
				for (int i = l_LeftIndex; i < l_RightIndex; i = i + 1) {
					if (l_Array [i].CompareTo (l_PivotValue) < 0) {
						SwapElements (l_Array, i, l_StoreIndex);
						l_StoreIndex = l_StoreIndex + 1;
					}
				}
				SwapElements (l_Array, l_StoreIndex, l_RightIndex);

				
					// We start at most one new thread because this code is also executed by
					// a thread.
				
				int l_Threads = 0;
				int l_ThreadIndex;
				lock (s_AvailableThreads) {
					l_Threads = s_AvailableThreads.RemainingThreads;
					if (l_Threads > 1) {
						l_Threads = 1;
					}
					l_ThreadIndex = s_AvailableThreads.startedThreads;
					s_AvailableThreads.startedThreads = s_AvailableThreads.startedThreads + l_Threads;
				}
				
				if (l_Threads == 1) {
					WaitCallback l_WaitCallback1 = s_WaitCallbacks [l_ThreadIndex + 0];
					QuickSortThreadArguments <T> l_Arguments1 = s_QuickSortThreadArguments [l_ThreadIndex + 1];
					l_Arguments1.Initialize (l_Array, l_LeftIndex, l_StoreIndex - 1);
					AutoResetEvent[] l_AutoResetEvent1 = s_AutoResetEvents [l_ThreadIndex + 1];
					
					ThreadPool.QueueUserWorkItem (l_WaitCallback1, (object) l_Arguments1);
					SequentialSort (l_Array, l_StoreIndex + 1, l_RightIndex);
				
					WaitHandle.WaitAll (l_AutoResetEvent1);
				} else {
					SequentialSort (l_Array, l_LeftIndex, l_StoreIndex - 1);
					SequentialSort (l_Array, l_StoreIndex + 1, l_RightIndex);
				}
			}
			
			l_AutoResetEvent.Set ();
		}
		
		/// <summary>
		/// Sequential quick sort.
		/// </summary>
		/// <param name='a_Array'>
		/// Array to be sorted. It is not allowed to be null.
		/// </param>
		public static void Sort (T[] a_Array) {
			SequentialSort (a_Array, 0, a_Array.Length - 1);
		}
		
		/// <summary>
		/// Sequential quick sort for a certain range.
		/// </summary>
		/// <param name="a_Array">
		/// Array to be sorted. It is not allowed to be null. <see cref="T[]"/>
		/// </param>
		/// <param name="a_LeftIndex">
		/// Smallest index of the range that is sorted. It has to be a valid index of the array. <see cref="System.Int32"/>
		/// </param>
		/// <param name="a_RightIndex">
		/// Greatest index of the range that is sorted. It has to be a valid index of the array. <see cref="System.Int32"/>
		/// </param>
		private static void SequentialSort (T[] a_Array, int a_LeftIndex, int a_RightIndex) {
			if (a_LeftIndex < a_RightIndex) {
				
					// Partition
				int l_PivotIndex = (a_LeftIndex + a_RightIndex) / 2;
				T l_PivotValue = a_Array [l_PivotIndex];
				SwapElements (a_Array, l_PivotIndex, a_RightIndex);
				int l_StoreIndex = a_LeftIndex;
				for (int i = a_LeftIndex; i < a_RightIndex; i = i + 1) {
					if (a_Array [i].CompareTo (l_PivotValue) < 0) {
						SwapElements (a_Array, i, l_StoreIndex);
						l_StoreIndex = l_StoreIndex + 1;
					}
				}
				SwapElements (a_Array, l_StoreIndex, a_RightIndex);
				
				SequentialSort (a_Array, a_LeftIndex, l_StoreIndex - 1);
				SequentialSort (a_Array, l_StoreIndex + 1, a_RightIndex);
			}
		}
		
		/// <summary>
		/// Swap two elements of the array.
		/// </summary>
		/// <param name="a_Array">
		/// Array in which the elements are swapped. It is not allowed to be null. <see cref="T[]"/>
		/// </param>
		/// <param name="a_Index1">
		/// First index of an element that is swapped. It has to be a valid index of the array. <see cref="System.Int32"/>
		/// </param>
		/// <param name="a_Index2">
		/// Second index of an element that is swapped. It has to be a valid index of the array. <see cref="System.Int32"/>
		/// </param>
		private static void SwapElements (T[] a_Array, int a_Index1, int a_Index2) {
			T l_Temporary = a_Array [a_Index1];
			a_Array [a_Index1] = a_Array [a_Index2];
			a_Array [a_Index2] = l_Temporary;
		}
	}
}