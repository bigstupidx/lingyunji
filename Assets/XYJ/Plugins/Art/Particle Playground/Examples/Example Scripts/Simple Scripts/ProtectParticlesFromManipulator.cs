using UnityEngine;
using System;
using System.Collections;
using ParticlePlayground;

public class ProtectParticlesFromManipulator : MonoBehaviour {

	public ParticleProtectionRange[] protectedParticles;	// Assign your ranges in Inspector
	public PlaygroundParticlesC particles;					// Assign your particle system in Inspector
	public int manipulatorNumber = 0;						// Which manipulator is this?
	ManipulatorObjectC manipulator;							// Cached manipulator

	void Start () {

		// Get the manipulator from the particle system
		manipulator = PlaygroundC.GetManipulator(manipulatorNumber, particles);

		// Set all protected particles within specified ranges from protectedParticles
		SetProtectedParticles();
	}

	void SetProtectedParticles () {

		// Add each particle inside the protected ranges
		foreach (ParticleProtectionRange protectRange in protectedParticles)
			for (int i = protectRange.start; i<protectRange.end; i++)
				manipulator.AddNonAffectedParticle(particles.particleSystemId, i);
	}

	void RemoveProtectedParticles () {

		// Remove each particle inside the protected ranges
		foreach (ParticleProtectionRange protectRange in protectedParticles)
			for (int i = protectRange.start; i<protectRange.end; i++)
				manipulator.RemoveNonAffectedParticle(particles.particleSystemId, i);
	}

}

[Serializable]
public class ParticleProtectionRange {
	public int start;
	public int end;
}