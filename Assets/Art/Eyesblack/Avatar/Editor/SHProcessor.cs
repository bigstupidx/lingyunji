// Marmoset Skyshop
// Copyright 2014 Marmoset LLC
// http://marmoset.co

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;

namespace mset {
	//This class will be run after every .ash file is imported
	public class SHProcessor : AssetPostprocessor {

		private delegate bool DataReader(ref SHEncoding SH, string path);

		public static void OnPostprocessAllAssets ( string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
			for (int a=0; a<importedAssets.Length; ++a) {
				string filePath = importedAssets[a];
				string ext = Path.GetExtension(filePath).ToLowerInvariant();
				
				//images with the ignore string in their filename will not be imported
				string name = Path.GetFileNameWithoutExtension(filePath).ToLowerInvariant();
				if(name.Contains("_noimport")) continue;

				SHProcessor.DataReader readSH = null;
				ext = ext.ToLowerInvariant();
				if( ext.Equals(".ash") ) readSH = readASH;

				if( readSH != null ) {
					string ashPath = filePath;
					string assetPath = Path.ChangeExtension(filePath, ".asset");

					SHEncoding SH = new SHEncoding();
					if( !readSH(ref SH, ashPath) ) {
						Debug.LogError("Failed to import spherical harmonics file \"" + ashPath + "\"");
						continue;
					}

					SHEncodingFile SHF = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SHEncodingFile)) as SHEncodingFile;

					if(SHF) {
						SHF.SH.copyFrom(SH);
						AssetDatabase.SaveAssets();
					} else {
						SHF = SHEncodingFile.CreateInstance<SHEncodingFile>();
						if(!SHF) {
							Debug.LogError("Failed to create spherical harmonics asset \"" + assetPath + "\"");
							continue;
						}
						SHF.SH = SH;
						AssetDatabase.CreateAsset(SHF, assetPath);
						AssetDatabase.Refresh();
						AssetDatabase.ImportAsset(assetPath);
					}
				}
			}
	    }

		//LOADERS
		public static bool readASH(ref SHEncoding sh, string ashPath) {
			/*# Generated with Lys by Knald Technologies, LLC - http://knaldtech.com
			# The following triplets are spherical harmonic coefficients for RGB in linear color space.

			l=0:
			m=0: 1.145362 1.249203 2.230852

			l=1:
			m=-1: 0.735545 0.874465 1.770959
			m=0: -0.000499 0.000527 0.005869
			m=1: 0.028654 0.038187 0.089021

			l=2:
			m=-2: -0.000077 0.004732 0.020932
			m=-1: 0.009634 0.014970 0.028454
			m=0: -0.190027 -0.232803 -0.469445
			m=1: -0.016092 -0.022949 -0.044332
			m=2: -0.183113 -0.226407 -0.481815
			*/
			StreamReader stream = new StreamReader(Application.dataPath + "/" + ashPath.Substring(7));
			string str = stream.ReadToEnd();
			str = str.ToLower();
			string[] lines = str.Split(new char[] {'\n','\r'});			
			int L = 0;
			int M = 0;
			bool valid = false;
			for(int i=0; i<lines.Length; ++i) {
				if( lines[i].Length == 0 ) continue;
				string line = lines[i];
				line = line.Trim();
				if( line.StartsWith("l=") ) {
					int a = line.IndexOf('=')+1;
					int b = line.IndexOf(':');
					string sub = line.Substring(a, b-a);
					L = Convert.ToInt32(sub);
				} else if( line.StartsWith("m=") ) {
					int a = line.IndexOf('=')+1;
					int b = line.IndexOf(':');
					string sub = line.Substring(a, b-a);
					M = Convert.ToInt32(sub);
					
					if(L >= 0 && (M <= L || M >= -L)) {
						valid |= true;
						
						sub = line.Substring(b+1).TrimStart();
						string[] triplet = sub.Split(new char[] {' ', '\t'});
						int j = M;
						if( L == 1 ) j = M+2;
						else if( L == 2 ) j = M+6;
						
						//Lys exports SH coefficients in Ramamoorthi's range 0-pi but our shaders need them as 0-1
						sh.c[j*3 + 0] = (float)Convert.ToDouble(triplet[0]) / 3.14159f;
						sh.c[j*3 + 1] = (float)Convert.ToDouble(triplet[1]) / 3.14159f;
						sh.c[j*3 + 2] = (float)Convert.ToDouble(triplet[2]) / 3.14159f;
					}
				}
			}
			return valid;
		}
	};
}