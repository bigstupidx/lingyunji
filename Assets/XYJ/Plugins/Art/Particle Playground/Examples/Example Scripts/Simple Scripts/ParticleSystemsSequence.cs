using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;

/// <summary>
/// Enables/disables emission on sequenced particle systems determined by minimum to maximum range. 
/// This can be used to control emission of several systems by one single input.
/// </summary>
public class ParticleSystemsSequence : MonoBehaviour {

	public float input = 0f;											// The current input
	public float maximumSequenceRange = 100f;							// The maximum sequence possible
	public List<SequenceRange> sequences = new List<SequenceRange>();	// The list of sequenced particle systems

	void Update () {
		Sequencer();
	}

	void Sequencer () {

		// Repeat the input
		input = Mathf.Repeat (input, maximumSequenceRange);

		// Enable emission if input is within the min-max range of the assigned sequences
		for (int i = 0; i<sequences.Count; i++)
			sequences[i].particles.emit = sequences[i].IsWithin(input);
	}
}

[Serializable]
public class SequenceRange {
	public PlaygroundParticlesC particles;	// Particle systems
	public float minRange;					// The minimum value for enabling
	public float maxRange;					// The maximum value for enabling

	/// <summary>
	/// Determines if f is within min-max range.
	/// </summary>
	/// <param name="f">Float to compare with.</param>
	public bool IsWithin (float f) {
		return (f>=minRange&&f<=maxRange);
	}
}