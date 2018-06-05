using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class EmitWhenMoving : MonoBehaviour {

	public PlaygroundParticlesC particles;
	public Transform movingTransform;
	Vector3 previousPosition;

	void Start () {
		if (particles==null)
			GetComponent<PlaygroundParticlesC>();
	}
	
	void Update () {
		if (movingTransform.position!=previousPosition) {
			particles.emit = true;
			previousPosition = movingTransform.position;
		} else particles.emit = false;
	}
}
