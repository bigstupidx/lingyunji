using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class EmitOnOffMultiByDistance : MonoBehaviour {

	public Transform target;
	public float distance = 10f;
	public PlaygroundParticlesC[] particles;
	
	void Update () {
		for (int i = 0; i<particles.Length; i++)
			particles[i].emit = (Vector3.Distance (target.position, particles[i].particleSystemTransform.position)<=distance);
	}
}
