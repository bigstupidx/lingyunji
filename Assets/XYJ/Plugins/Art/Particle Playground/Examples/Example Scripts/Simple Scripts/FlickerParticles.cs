using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class FlickerParticles : MonoBehaviour {

	[Range(.01f, 1f)]
	public float flickerInterval = .1f;
	PlaygroundParticlesC particles;

	float _lastUpdate;

	void OnEnable () 
	{
		if (particles == null)
			particles = GetComponent<PlaygroundParticlesC>();

		particles.particleMask = particles.particleCount;
	}

	void OnDisable ()
	{
		particles.applyParticleMask = false;
	}
	
	void Update () 
	{
		if (Time.time >= _lastUpdate + flickerInterval)
		{
			particles.applyParticleMask = !particles.applyParticleMask;
			_lastUpdate = Time.time;
		}
	}
}
