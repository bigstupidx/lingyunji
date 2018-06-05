//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class ChangeTint : MonoBehaviour {
	
	public Color tint1 = Color.white;
	public Color tint2 = Color.black;
	public float duration = 10.0f;
	public float waitDuration = 3.0f;
	private float m_PassedTime;
	private CS_Cloud m_Cloud;
	
	private void OnEnable () {
		m_Cloud = GetComponent <CS_Cloud> ();
		
		if (m_Cloud != null) {
			StartCoroutine (Fading ());
		} else {
			Debug.LogError ("ChangeTint script is not in a GameObject that contains a CS_Cloud!");
		}
	}
	
	private IEnumerator Fading () {
		yield return (null);
		while (true) {
			
				// Fade out
			m_PassedTime = 0.0f;
			while (m_PassedTime < duration) {
				float l_Factor = 1.0f - (m_PassedTime / duration);
				m_Cloud.Tint = Color.Lerp (tint1, tint2, l_Factor);
				m_PassedTime = m_PassedTime + Time.deltaTime;
				yield return (null);
			}
			m_Cloud.Tint = tint1;
			yield return (new WaitForSeconds (waitDuration));
			
				// Fade in
			m_PassedTime = 0.0f;
			while (m_PassedTime < duration) {
				float l_Factor = m_PassedTime / duration;
				m_Cloud.Tint = Color.Lerp (tint1, tint2, l_Factor);
				m_PassedTime = m_PassedTime + Time.deltaTime;
				yield return (null);
			}
			m_Cloud.Tint = tint2;
			yield return (new WaitForSeconds (waitDuration));
		}
	}
}
