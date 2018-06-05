using UnityEngine;
using System;
using System.Collections;
using ParticlePlayground;

public class ProtectParticlesFromManipulatorById : MonoBehaviour {

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

		// Protect each particle inside the protected ranges
		foreach (ParticleProtectionRange protectRange in protectedParticles)
			for (int i = protectRange.start; i<protectRange.end; i++)
				particles.ProtectParticleFromManipulator(i, manipulator);
	}

	void RemoveProtectedParticles () {

		// Remove each particle inside the protected ranges
		foreach (ParticleProtectionRange protectRange in protectedParticles)
			for (int i = protectRange.start; i<protectRange.end; i++)
				particles.RemoveParticleProtection(i);
	}

}