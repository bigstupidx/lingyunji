//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class MoveShadingGroupCenter : MonoBehaviour {
	
	public int shadingGroupIndex = 0;
	public Vector3 startPosition;
	public Vector3 endPosition;
	
	public float duration = 4.0f;
	public float waitDuration = 2.0f;
	private float m_PassedTime;
	private CS_Cloud m_Cloud;
	
	private void OnEnable () {
		m_Cloud = GetComponent <CS_Cloud> ();
		
		if (m_Cloud != null) {
			StartCoroutine (Move ());
		} else {
			Debug.LogError ("MoveShadingGroupCenter script is not in a GameObject that contains a CS_Cloud!");
		}
	}
	
	private IEnumerator Move () {
		yield return (null);
		while (true) {
			Vector3 l_Position;
			
				// Start to end
			m_PassedTime = 0.0f;
			while (m_PassedTime < duration) {
				l_Position = Vector3.Lerp (startPosition, endPosition, m_PassedTime / duration);
				MoveShadingGroupCenterTo (l_Position, shadingGroupIndex);
				m_PassedTime = m_PassedTime + Time.deltaTime;
				yield return (null);
			}
			l_Position = endPosition;
			MoveShadingGroupCenterTo (l_Position, shadingGroupIndex);
			
			yield return (new WaitForSeconds (waitDuration));
			
				// End to start
			m_PassedTime = 0.0f;
			while (m_PassedTime < duration) {
				l_Position = Vector3.Lerp (endPosition, startPosition, m_PassedTime / duration);
				MoveShadingGroupCenterTo (l_Position, shadingGroupIndex);
				m_PassedTime = m_PassedTime + Time.deltaTime;
				yield return (null);
			}
			l_Position = startPosition;
			MoveShadingGroupCenterTo (l_Position, shadingGroupIndex);
			yield return (new WaitForSeconds (waitDuration));
		}
	}
	
	private void MoveShadingGroupCenterTo (Vector3 a_Position, int a_ShadingGroupIndex) {
		m_Cloud.shadingGroups [a_ShadingGroupIndex].center = a_Position;
		m_Cloud.shadingGroups [a_ShadingGroupIndex].RecalculateScaledCenter (m_Cloud);
		m_Cloud.SetParticleSystemHasChanged ();
	}
}
