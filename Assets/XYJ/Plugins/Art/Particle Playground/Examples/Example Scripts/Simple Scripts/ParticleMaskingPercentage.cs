using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// Mask particles by percentage.
/// </summary>
[ExecuteInEditMode()]
public class ParticleMaskingPercentage : MonoBehaviour {

	public PlaygroundParticlesC particles;	// Assign your particle system in the Inspector
	public float maskingPercentage;			// The percent of particles to be masked

	void Start () {
		particles.applyParticleMask = true;
	}

	void Update () {
		maskingPercentage = Mathf.Clamp (maskingPercentage, 0, 100f);
		particles.particleMask = Mathf.RoundToInt(particles.particleCount*(maskingPercentage/100f));
	}
}
