//
// Edelweiss.CloudSystemEditor.IconLoader.cs: Icon loading for textures and for textures within dll's.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Edelweiss.CloudSystemEditor {

	public class IconLoader {
		
		public static bool IsIconFromAssembly (string a_FileName) {
			bool l_Result = false;
			Assembly l_Assembly = Assembly.GetExecutingAssembly ();
			List <string> l_ResourceNames = new List <string> (l_Assembly.GetManifestResourceNames ());
			if (l_ResourceNames.Contains (a_FileName)) {
				l_Result = true;
			}
			return (l_Result);
		}
		
		public static Texture2D LoadIcon (string a_PathName, string a_FileName) {
			Texture2D l_Result = null;
			
			Assembly l_Assembly = Assembly.GetExecutingAssembly ();
			List <string> l_ResourceNames = new List <string> (l_Assembly.GetManifestResourceNames ());
			if (l_ResourceNames.Contains (a_FileName)) {
				l_Result = LoadIconFromAssembly (l_Assembly, a_FileName);
			} else {
				l_Result = LoadIconFromResources (a_PathName, a_FileName);
			}
					
			return (l_Result);
		}
		
		private static Texture2D LoadIconFromAssembly (Assembly a_Assembly, string a_FileName) {
			Texture2D l_Result = new Texture2D (0, 0, TextureFormat.ARGB32, false);
			Stream l_ByteStream = a_Assembly.GetManifestResourceStream (a_FileName);
			byte[] l_Buffer = new byte[l_ByteStream.Length];
			l_ByteStream.Read (l_Buffer, 0, (int) l_ByteStream.Length);
			l_ByteStream.Close ();
			l_Result.LoadImage (l_Buffer);
			return (l_Result);
		}
		
		private static Texture2D LoadIconFromResources (string a_PathName, string a_FileName) {
			Texture2D l_Result = UnityEditor.AssetDatabase.LoadAssetAtPath (a_PathName + a_FileName, typeof (Texture2D)) as Texture2D;
			return (l_Result);
		}
	}
}
