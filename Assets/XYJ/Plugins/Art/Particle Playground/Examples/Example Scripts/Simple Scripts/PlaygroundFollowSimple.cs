using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class PlaygroundFollowSimple : MonoBehaviour {

	public PlaygroundParticlesC particles;
	public Transform followerReference;
	public bool rotateTowardsDirection = false;
	private Transform[] _followers;

	void Start () {
		_followers = new Transform[particles.particleCount];
		for (int i = 0; i<particles.particleCount; i++) {
			_followers[i] = ((Transform)Instantiate (followerReference)).transform;
			_followers[i].parent = transform;
		}
		followerReference.gameObject.SetActive (false);
	}

	void Update () {

		// Return if lengths doesn't match
		if (_followers.Length != particles.particleCount)
			return;

		for (int i = 0; i<_followers.Length; i++) {

			// Position each follower
			_followers[i].position = particles.particleCache[i].position;

			// Rotate each follower
			if (rotateTowardsDirection && particles.playgroundCache.velocity[i] != Vector3.zero)
				_followers[i].rotation = Quaternion.LookRotation(particles.playgroundCache.velocity[i]);
		}
	}
}
