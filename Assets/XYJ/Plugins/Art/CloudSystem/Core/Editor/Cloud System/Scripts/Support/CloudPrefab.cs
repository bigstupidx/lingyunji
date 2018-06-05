//
// CloudPrefab.cs: Prefab functionality for the Cloud System.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using UnityEditor;
using System.Collections;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {
	public class CloudPrefab {

		public static void CreateCloudPrefab <C, PD, CD> (CloudRendererTypeLookup a_CloudRendererTypeLookup)
			where C : Cloud <C, PD, CD>
			where PD : ParticleData <C, PD, CD>
			where CD : CreatorData <C, PD, CD>
		{
			GameObject l_GameObject = new GameObject ("Cloud");
			C l_Cloud = l_GameObject.AddComponent <C> ();
			ChangeCloudRenderer <C, PD, CD> (l_Cloud, CloudRendererTypeEnum.CustomTintRenderer, a_CloudRendererTypeLookup);
			
			string l_Path = "Assets/" + l_GameObject.name + ".prefab";
			l_Path = AssetDatabase.GenerateUniqueAssetPath (l_Path);
			
			UnityEngine.Object l_PrefabObject = PrefabUtility.CreateEmptyPrefab (l_Path);
			GameObject l_Prefab = PrefabUtility.ReplacePrefab (l_GameObject, l_PrefabObject);
			C l_PrefabCloudSystem = l_Prefab.GetComponent <C> ();
			AssetDatabase.Refresh ();
			
			l_PrefabCloudSystem.CreatePrefabData ();
			
			AssetDatabase.AddObjectToAsset (l_PrefabCloudSystem.CreatorData, l_Prefab);
			AssetDatabase.AddObjectToAsset (l_PrefabCloudSystem.ParticleData, l_Prefab);
			
			AssetDatabase.Refresh ();
			AssetDatabase.ImportAsset (l_Path);
			
			GameObject l_PrefabInstance = PrefabUtility.InstantiatePrefab (l_Prefab) as GameObject;
			Selection.activeGameObject = l_PrefabInstance;
			
			UnityEngine.Object.DestroyImmediate (l_GameObject);
		}
		
		public static void ChangeCloudRenderer <C, PD, CD> (C a_Cloud, CloudRendererTypeEnum a_CloudRendererTypeEnum, CloudRendererTypeLookup a_CloudRendererTypeLookup)
			where C : Cloud <C, PD, CD>
			where PD : ParticleData <C, PD, CD>
			where CD : CreatorData <C, PD, CD>
		{
		
			bool l_WasEditorAccelerated = CloudSystemPrefs.AccelerateEditor;
			CloudSystemPrefs.AccelerateEditor = false;
			
			if (a_Cloud.IsInitialized) {
				a_Cloud.CloudRenderer.DestroyRendererComponents ();
				Object.DestroyImmediate (a_Cloud.CloudRenderer);
			}
			
			System.Type l_Type = a_CloudRendererTypeLookup.TypeForCloudSystemRendererEnum (a_CloudRendererTypeEnum);
			a_Cloud.CloudRenderer = a_Cloud.gameObject.AddComponent (l_Type) as CloudRenderer <C, PD, CD>;
			a_Cloud.CloudRendererType = a_CloudRendererTypeEnum;
			
			a_Cloud.InitializeRendererComponents ();
			if (a_Cloud.ParticleData != null) {
				a_Cloud.OnEnable ();
			}
			
			CloudSystemPrefs.AccelerateEditor = l_WasEditorAccelerated;
		}
	}
}