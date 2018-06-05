//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class LightningCreator : MonoBehaviour {

	public Vector3 border1;
	public Vector3 border2;
	
	public GameObject[] lightnings;
	
	public float minimumDelay = 0.2f;
	public float maximumDelay = 2.0f;
	
	private void Start () {
		if (lightnings.Length != 0) {
			StartCoroutine (CreateLightnings ());
		}
	}
	
	private IEnumerator CreateLightnings () {
		while (true) {
			float l_Wait = Random.Range (minimumDelay, maximumDelay);
			yield return (new WaitForSeconds (l_Wait));
			
			GameObject l_Lightning = lightnings [Random.Range (0, lightnings.Length)];
			
			Vector3 l_Area = border2 - border1;
			Vector3 l_Point = new Vector3 (Random.Range (0.0f, l_Area.x), Random.Range (0.0f, l_Area.y), Random.Range (0.0f, l_Area.z));
			l_Point = l_Point + border1;
			
			Instantiate (l_Lightning, l_Point, Quaternion.identity);
		}
	}
}
