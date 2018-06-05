//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animation))]
public class DestroyOnFinish : MonoBehaviour {

	private void Update () {
		if (!GetComponent<Animation>().isPlaying) {
			Destroy (gameObject);
		}
	}
}
