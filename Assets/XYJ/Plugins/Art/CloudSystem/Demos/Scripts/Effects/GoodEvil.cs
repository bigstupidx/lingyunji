//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class GoodEvil : MonoBehaviour {
	
	private CS_Cloud m_Cloud;
	
	private float m_EvilGoodFactor = 0.0f;
	public Vector3 evilShadingGroupCenter;
	public Vector3 goodShadingGroupCenter;
	public float evilVerticalShadingInfluence = 0.5f;
	public float goodVerticalShadingInfluence = 0.5f;
	
	public GameObject evilThunderPrefab;
	public Vector3 minimumInstantiationArea = new Vector3 (-7.0f, 0.0f, -11.0f);
	public Vector3 maximumInstantiationArea = new Vector3 (3.0f, 2.0f, 11.0f);
	private float m_ShadingGroupZ;
	private float m_MaximumInstantiationVolume;
	
	private bool m_HideGUI = false;
	
	private void Start () {
		m_Cloud = GetComponent <CS_Cloud> ();
		
		if (m_Cloud == null) {
			Debug.LogError ("GoodEvil does not contain a CS_Cloud component!");
		} else {
			UpdateShadingGroupCenter ();
		}
		
		Vector3 l_Volume = maximumInstantiationArea - minimumInstantiationArea;
		m_MaximumInstantiationVolume = Mathf.Abs (l_Volume.x * l_Volume.y * l_Volume.z);
		if (Mathf.Approximately (m_MaximumInstantiationVolume, 0.0f)) {
			Debug.LogError ("The maximum volume in which thunders can be instantiated is not allowed to be zero!");
		}
		
		StartCoroutine (CreateEvilThunder ());
	}
	
	private void UpdateShadingGroupCenter () {

		float l_VerticalShadingInfluence = Mathf.Lerp (evilVerticalShadingInfluence, goodVerticalShadingInfluence, m_EvilGoodFactor);
		m_Cloud.ShadingGroupInfluence = l_VerticalShadingInfluence;
		
		Vector3 l_ShadingGroupCenter = Vector3.Lerp (evilShadingGroupCenter, goodShadingGroupCenter, m_EvilGoodFactor);
		for (int i = 0; i < m_Cloud.shadingGroups.Length; i = i + 1) {
			m_Cloud.shadingGroups [i].center = l_ShadingGroupCenter;
			m_Cloud.shadingGroups [i].RecalculateScaledCenter (m_Cloud);
			
			m_ShadingGroupZ = m_Cloud.shadingGroups [i].center.z;
		}
		
		m_Cloud.SetParticleSystemHasChanged ();
	}
	
	private IEnumerator CreateEvilThunder () {
		while (true) {
			if (minimumInstantiationArea.z < m_ShadingGroupZ) {
				float l_MaximumZ = Mathf.Min (m_ShadingGroupZ, maximumInstantiationArea.z);
				
				float x = Random.Range (minimumInstantiationArea.x, maximumInstantiationArea.x);
				float y = Random.Range (minimumInstantiationArea.y, maximumInstantiationArea.y);
				float z = Random.Range (minimumInstantiationArea.z, l_MaximumZ);
				
				Instantiate (evilThunderPrefab, new Vector3 (x, y, z), Quaternion.identity);
				
				Vector3 l_Volume = maximumInstantiationArea - minimumInstantiationArea;
				l_Volume.z = l_MaximumZ;
				float l_InstantiationVolume = Mathf.Abs (l_Volume.x * l_Volume.y * l_Volume.z);
				
				if (Mathf.Approximately (l_InstantiationVolume, 0.0f)) {
					yield return (new WaitForSeconds (1.0f));
				} else {
					
						// That section is pretty hackish. We just tried the values until
						// we got the wanted effect.
					float l_NextInstantiation = 0.4f * m_MaximumInstantiationVolume / l_InstantiationVolume;
					l_NextInstantiation = Mathf.Clamp (l_NextInstantiation, 0.2f, 1.5f);
					l_NextInstantiation = Random.Range (0.5f, 1.0f) * l_NextInstantiation;
					
					yield return (new WaitForSeconds (l_NextInstantiation));
				}
				
			} else {
				yield return (new WaitForSeconds (0.2f));
			}
		}
	}
	
	private void Update () {
		if (Input.GetKeyDown (KeyCode.H)) {
			m_HideGUI = !m_HideGUI;
		}
	}
	
	private void OnGUI () {
		if (!m_HideGUI) {
			GUILayout.BeginArea (new Rect (0.0f, 0.0f, Screen.width, Screen.height));
			
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();
			
			GUILayout.BeginHorizontal ();
			GUILayout.Space (20.0f);
			
			GUILayout.BeginVertical (GUI.skin.box);
			
			GUILayout.Label ("Good and Evil");
			
			float l_NewEvilGoodFactor = GUILayout.HorizontalSlider (m_EvilGoodFactor, 0.0f, 1.0f);
			if (l_NewEvilGoodFactor != m_EvilGoodFactor) {
				m_EvilGoodFactor = l_NewEvilGoodFactor;
				UpdateShadingGroupCenter ();
			}
			
			GUILayout.EndVertical ();
			
			GUILayout.Space (20.0f);
			GUILayout.EndHorizontal ();
			
			GUILayout.Space (20.0f);
			GUILayout.EndVertical ();
			
			GUILayout.EndArea ();
		}
	}
}
