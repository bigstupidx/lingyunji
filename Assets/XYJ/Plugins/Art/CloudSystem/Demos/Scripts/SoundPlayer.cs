//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class SoundPlayer : MonoBehaviour {
	
		// This method is started from an animation event.
	public void PlaySound () {
		AudioSource l_AudioSource = GetComponent <AudioSource> ();
		l_AudioSource.Play ();
	}
}
