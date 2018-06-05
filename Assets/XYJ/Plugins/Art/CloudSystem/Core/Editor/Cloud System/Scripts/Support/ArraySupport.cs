//
// Edelweiss.CloudSystemEditor.ArraySupport.cs: Functinality to modify and/or resize arrays.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using System;

namespace Edelweiss.CloudSystemEditor {

	public class ArraySupport {
		
		public static void Remove <T> (ref T[] a_Array, T a_Value) {
			int i = Array.IndexOf (a_Array, a_Value);
			RemoveAt <T> (ref a_Array, i);
		}
		
		public static void RemoveAt <T> (ref T[] a_Array, int a_Index) {
			int i = a_Index;
			if (i >= 0) {
				for (int j = i + 1; j < a_Array.Length; i = i + 1, j = j + 1) {
					T l_TmpObject = (T) a_Array.GetValue (i);
					a_Array.SetValue (a_Array.GetValue (j), i);
					a_Array.SetValue (l_TmpObject, j);
				}
				Array.Resize (ref a_Array, a_Array.Length - 1);
			}
		}
		
		public static void Add <T> (ref T[] a_Array, T a_Value) {
			Array.Resize (ref a_Array, a_Array.Length + 1);
			a_Array.SetValue (a_Value, a_Array.Length - 1);
		}
	}
}
