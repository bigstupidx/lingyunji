using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class PlaygroundRotationNormal : MonoBehaviour {

	PlaygroundParticlesC particles;
	Transform cameraTransform;

	bool isUpsideDown = false;
	bool isBackward = false;

	void Start () {
		particles = GetComponent<PlaygroundParticlesC>();
		cameraTransform = Camera.main.transform;
	}

	void Update () {

		if (particles.rotateTowardsDirection) {
			isUpsideDown = Vector3.Dot(cameraTransform.up, Vector3.up)<0;
			isBackward = Vector3.Dot (cameraTransform.forward, Vector3.forward)<0;

			if (!isUpsideDown && !isBackward) {
				particles.rotationNormal = new Vector3 (0,0,-1f);
				particles.initialRotationMin = 0;
			} else if (!isUpsideDown && isBackward) {
				particles.rotationNormal = new Vector3 (0,0,1f);
				particles.initialRotationMin = 0;
			} else if (isUpsideDown && !isBackward) {
				particles.rotationNormal = new Vector3 (0,0,-1f);
				particles.initialRotationMin = 180f;
			} else if (isUpsideDown && isBackward) {
				particles.rotationNormal = new Vector3 (0,0,1f);
				particles.initialRotationMin = 180f;
			}

			particles.initialRotationMax = particles.initialRotationMin;
		}
	}
}
