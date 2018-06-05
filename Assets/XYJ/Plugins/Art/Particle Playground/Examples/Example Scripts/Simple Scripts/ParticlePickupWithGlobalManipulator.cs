﻿using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class ParticlePickupWithGlobalManipulator : MonoBehaviour {

	public int manipulatorIndex;		// The index of your Global Manipulator
	ManipulatorObjectC manipulator;		// Cached reference to the Global Manipulator

	int pickups = 0;					// As an example count the amount of pickups made

	void Start () {

		// Cache the Global Manipulator
		manipulator = PlaygroundC.GetManipulator(manipulatorIndex);

		// Note that you can set the Manipulator to Type: None in case you have no intention to also modify the particles behavior
		// manipulator.type = MANIPULATORTYPEC.None;

		// Make sure we're tracking particles
		manipulator.trackParticles = true;

		// Make sure we're sending enter events
		manipulator.sendEventEnter = true;

		// Make sure we're using the fastest tracking method
		manipulator.trackingMethod = TrackingMethod.ManipulatorId;

		// Assign your function to the event delegate
		manipulator.particleEventEnter += OnManipulatorEnter;
	}

	/// <summary>
	/// This function will run whenever a particle enters the extents of the Manipulator.
	/// </summary>
	/// <param name="particle">Particle.</param>
	void OnManipulatorEnter (PlaygroundEventParticle particle) {

		// Do something here
		// Debug.Log("Particle "+particle.particleId+" from particle system "+particle.particleSystemId+" at position "+particle.position+" is within the Manipulator.");

		// Then remove the particle from its current location
		PlaygroundC.reference.particleSystems[particle.particleSystemId].Kill (particle.particleId);

		// Increase pickups by one
		pickups++;
	}
}
