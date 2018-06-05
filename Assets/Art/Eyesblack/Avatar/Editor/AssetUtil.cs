// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace mset {
	public class AssetUtil {
		public static TextureImporter getTextureImporter(String path) { return getTextureImporter("getTextureImporter"); }
		public static TextureImporter getTextureImporter(String path, String errorLabel) {
			if( path.Length == 0 ) {
				Debug.LogError(errorLabel + " needs an asset path (empty string provided).");
				return null;
			}
			AssetImporter ai = AssetImporter.GetAtPath(path);
			if(ai == null) {
				Debug.LogError(errorLabel + " failed to fetch any asset importer for path '" + path + "'.");
				return null;
			}
			TextureImporter ti = ai as TextureImporter;
			if(ti == null) {
				Debug.LogError(errorLabel + " failed to cast AssetImporter of type " + ai.GetType() + " to TextureImporter at path '" + path + "'.");
				return null;
			}
			return ti;
		}
		/*
		public static bool makeReadable( ref Texture tex ) {
			string path = AssetDatabase.GetAssetPath(tex);
			TextureImporter ti = getTextureImporter(path,"generic mset.Util.makeReadable");
			if( ti == null ) return false;
			ti.isReadable = true;
			AssetDatabase.ImportAsset(path);
			return true;
		}
		
		public static bool makeCubeReadable( ref Cubemap cube ) {
			string path = AssetDatabase.GetAssetPath(cube);
			TextureImporter ti = getTextureImporter(path, "mset.Util.makeCubeReadable");
			if( ti == null ) return false;
			ti.isReadable = true;
			AssetDatabase.ImportAsset(path);
			cube = (Cubemap)AssetDatabase.LoadAssetAtPath(path,typeof(Cubemap));
			return true;
		}
		public static bool makeCubeFrom2D( String path, bool mipmap ) {
			return makeCubeFrom2D(path,mipmap,-1);
		}
		public static bool makeCubeFrom2D( String path, bool mipmap, int maxSize ) {
			TextureImporter ti = getTextureImporter(path, "mset.Util.makeCubeFrom2D");
			ti.isReadable = true;
			if( maxSize > 0 ) ti.maxTextureSize = maxSize;
			if( ti.generateCubemap != TextureImporterGenerateCubemap.Cylindrical ) {
				ti.generateCubemap = TextureImporterGenerateCubemap.Cylindrical;
				ti.mipmapEnabled = mipmap;
				AssetDatabase.ImportAsset(path);
				AssetDatabase.Refresh();
			}
			return true;
		}
		public static void resampleTexture2D( ref Texture2D srcdst, int width, int height ) { resampleTexture2D(ref srcdst, srcdst, width, height); }
		public static void resampleTexture2D( ref Texture2D dst, Texture2D src ) 			{ resampleTexture2D(ref dst, src, dst.width, dst.height); }
		public static void resampleTexture2D( ref Texture2D dst, Texture2D src, int width, int height ) {
			Color[] c = new Color[width*height];
			
			float ow = (float)width;
			float oh = (float)height;
			float ih = (float)src.height;
			
			for( int y = 0; y < height; ++y )
			for( int x = 0; x < width; ++x ) {
				float u = (float)x/ow;
				float v = (float)y/oh;
				v = Mathf.Min(v,(ih-1f)/ih);
				c[y*width+x] = src.GetPixelBilinear(u,v);
			}
			
			dst.Resize(width,height);
			dst.SetPixels(c);
		}
		public static Texture2D cloneTexture2D( Texture2D src ) {
			string srcpath = AssetDatabase.GetAssetPath(src);
			string dstpath = AssetDatabase.GenerateUniqueAssetPath(srcpath);
			AssetDatabase.CopyAsset(srcpath,dstpath);
			AssetDatabase.Refresh();
			return (Texture2D)AssetDatabase.LoadAssetAtPath(dstpath,typeof(Texture2D));
		}
		public static void resizeTexture2D( ref Texture2D tex, int maxSize ) {
			string path = AssetDatabase.GetAssetPath(tex);
			TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
			ti.maxTextureSize = maxSize;
			AssetDatabase.ImportAsset(path);
			tex = (Texture2D)AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D));
			AssetDatabase.Refresh();
		}
		public static bool resizeCube( ref Cubemap cube, int maxSize ) {
			string path = AssetDatabase.GetAssetPath(cube);
			TextureImporter ti = getTextureImporter(path, "mset.Util.resizeCube");
			if( ti == null ) return false;
			ti.maxTextureSize = maxSize;
			AssetDatabase.ImportAsset(path);
			cube = (Cubemap)AssetDatabase.LoadAssetAtPath(path,typeof(Cubemap));
			AssetDatabase.Refresh();
			return true;
		}
		public static void writeTexture2D( Texture2D tex, string dir, string name ) {
			string path = Application.dataPath + "/";
			if( dir.Length > 0 ) path += dir;
			path += name + ".png";
		    FileStream fs = new FileStream(path, FileMode.Create);
		    BinaryWriter bw = new BinaryWriter(fs);
		    bw.Write(tex.EncodeToPNG());
		    bw.Close();
		    fs.Close();			
			AssetDatabase.ImportAsset(path);
			AssetDatabase.Refresh();
		}
		public static bool makeReflection( Texture tex ) { return makeReflection( AssetDatabase.GetAssetPath(tex) ); }
		public static bool makeReflection( string path ) {
			TextureImporter ti = getTextureImporter(path,"mset.Util.makeReflection");	
			if( ti == null ) return false;
			ti.textureType = TextureImporterType.Reflection;
			ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
			ti.generateCubemap = TextureImporterGenerateCubemap.Cylindrical;
			AssetDatabase.ImportAsset(path);
			AssetDatabase.Refresh();
			return true;
		}
		*/

		public static string writePNG( ref Texture2D tex, string assetPath ) {
			try {
				assetPath = assetPath.Substring(7);
				string path = Application.dataPath + "/" + assetPath;
				FileStream fs = new FileStream(path, FileMode.Create);
			    BinaryWriter bw = new BinaryWriter(fs);
			    bw.Write(tex.EncodeToPNG());
			    bw.Close();
			    fs.Close();
				assetPath = "Assets/" + assetPath;
			} catch(Exception e) {
				Debug.LogError("FileStream exception: " + e.ToString());
				return "";
			}
			return assetPath;
		}
		
		// serialized asset management
		public static void printSerializedProperties(UnityEngine.Object obj) {
			SerializedObject srobj = new SerializedObject(obj);
			printSerializedProperties(srobj);
		}
		public static void printSerializedProperties(SerializedObject srobj) {
			int breaker = 1000;
			SerializedProperty itr = srobj.GetIterator();
			
			string props = "";
			itr.Next(true);
			do {
				if(itr.name.StartsWith("m_")) props += itr.name + "\n";
				breaker--;
			} while(breaker>0 && itr.Next(true));
			Debug.Log("Properties of " + srobj.targetObject.name + ":\n" + props);
		}

		public static void printSerializedProperties(SerializedObject srobj, string containing) {
			int breaker = 1000;
			SerializedProperty itr = srobj.GetIterator();
			
			string props = "";
			itr.Next(true);
			do {
				if(itr.name.Contains(containing)) props += itr.name + "\n";
				breaker--;
			} while(breaker>0 && itr.Next(true));
			Debug.Log("Properties of " + srobj.targetObject.name + ":\n" + props);
		}
		
		public static bool isReadable(SerializedObject serialTex) {
			if(serialTex == null) return false;
			SerializedProperty prop;
			prop = serialTex.FindProperty("m_IsReadable"); 	if( prop != null ) return prop.boolValue;
			prop = serialTex.FindProperty("m_ReadAllowed"); if( prop != null ) return prop.boolValue;
			Debug.LogError("m_IsReadable or m_ReadAllowed SerializedProperty not found!");
			return false;
		}
		public static void setReadable(SerializedObject serialTex, bool readable) {
			if(serialTex == null) return;
			SerializedProperty prop;
			prop = serialTex.FindProperty("m_IsReadable"); 	if( prop != null ) prop.boolValue = readable;
			prop = serialTex.FindProperty("m_ReadAllowed"); if( prop != null ) prop.boolValue = readable;
			serialTex.ApplyModifiedProperties();
		}

		//not all texture compression formats are getPixel readable
		public static bool isReadableFormat(Texture2D tex) {
			//must be ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT
			return 
				tex.format == TextureFormat.Alpha8 || 
				tex.format == TextureFormat.ARGB32 ||
				tex.format == TextureFormat.RGBA32 ||
				tex.format == TextureFormat.BGRA32 ||
				tex.format == TextureFormat.RGB24 ||
				tex.format == TextureFormat.DXT1 ||
				tex.format == TextureFormat.DXT5;
		}


		// returns whether the "Linear" checkbox is checked on a cubemap or 2D texture
		public static bool isLinear(SerializedObject serialTex) {
			if(serialTex == null) return false;
			SerializedProperty prop = serialTex.FindProperty("m_ColorSpace");
			if( prop != null ) {
				//lol wut? Did unity get this backwards?
				return prop.intValue == (int)ColorSpace.Gamma;
			}
			Debug.LogError("m_ColorSpace SerializedProperty not found!");
			return false;
		}
		public static void setLinear(SerializedObject serialTex, bool linear) {
			if(serialTex == null) return;
			SerializedProperty prop = serialTex.FindProperty("m_ColorSpace");
			if( prop != null ) {
				//lol wut? Did unity get this backwards?
				prop.intValue = linear ? (int)ColorSpace.Gamma : (int)ColorSpace.Linear;
				serialTex.ApplyModifiedProperties();
			} else {
				Debug.LogError("m_ColorSpace SerializedProperty not found!");
			}
		}

		public static bool isUncompressed(SerializedObject serialTex) {
			if(serialTex == null) return false;
			SerializedProperty prop = serialTex.FindProperty("m_TextureFormat");
			if( prop != null ) { 
				return prop.intValue == (int)TextureFormat.ARGB32;
			}
			Debug.LogError("m_TextureFormat SerializedProperty not found!");
			return false;
		}
		//NOTE: can only be set to true, default false would be setting to DXT5, on PC anyway
		public static void setUncompressed(SerializedObject serialTex) {
			SerializedProperty prop = serialTex.FindProperty("m_TextureFormat");
			if( prop != null ) {
				prop.intValue = (int)TextureFormat.ARGB32;
				serialTex.ApplyModifiedProperties();
			} else {
				Debug.LogError("m_TextureFormat SerializedProperty not found!");
			}
		}

		public static bool getTextureFormat(ref TextureFormat result, SerializedObject serialTex) {
			if(serialTex == null) return false;
			SerializedProperty prop = serialTex.FindProperty("m_TextureFormat");
			if( prop != null ) { 
				result = (TextureFormat)prop.intValue;
				return true;
			}
			Debug.LogError("m_TextureFormat SerializedProperty not found!");
			return false;
		}
		public static void setTextureFormat(SerializedObject serialTex, TextureFormat format) {
			SerializedProperty prop = serialTex.FindProperty("m_TextureFormat");
			if( prop != null ) {
				prop.intValue = (int)format;
				serialTex.ApplyModifiedProperties();
			} else {
				Debug.LogError("m_TextureFormat SerializedProperty not found!");
				printSerializedProperties(serialTex);
			}
		}
		public static bool isMipmapped(SerializedObject serialTex) {
			if(serialTex == null) return false;
		
			//Unity 5.2+ check
			SerializedProperty prop = serialTex.FindProperty("m_MipCount");
			if( prop != null ) return prop.intValue > 1;

			//Unity 5.1- check
			prop = serialTex.FindProperty("m_MipMap");
			if( prop != null ) { return prop.boolValue; }
			Debug.LogError("Neither m_MipCount nor m_MipMap SerializedProperty not found!");
			return false;
		}
		public static void setMipmapped(SerializedObject serialTex, bool mipmap) {
			if(serialTex == null) return;
			SerializedProperty prop = serialTex.FindProperty("m_MipMap");
			if( prop != null ) {
				prop.boolValue = mipmap;
				serialTex.ApplyModifiedProperties();
			} else {
				Debug.LogError("m_MipMap SerializedProperty not found!");
			}
		}
		public static bool hasAlpha(Texture2D tex) {
			if(tex==null) return false;
			TextureImporter ti = mset.AssetUtil.getTextureImporter(AssetDatabase.GetAssetPath(tex), "mset.Util.hasAlpha");
			if( ti ) return ti.DoesSourceTextureHaveAlpha();
			return false;
		}
	};
}

