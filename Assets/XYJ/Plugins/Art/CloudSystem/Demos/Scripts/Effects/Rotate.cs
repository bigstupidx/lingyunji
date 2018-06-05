//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2013 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public float duration = 100.0f;
	
	private void Update () {
		transform.Rotate (Vector3.up, 360.0f * Mathf.Deg2Rad / duration * Time.deltaTime);
	}
}
