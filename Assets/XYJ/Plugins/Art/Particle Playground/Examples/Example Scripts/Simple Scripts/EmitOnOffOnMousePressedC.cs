using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class EmitOnOffOnMousePressedC : MonoBehaviour {

	public PlaygroundParticlesC particles;	// Set the Particle Playground System through Inspector
	
	void Update () {
		particles.emit = Input.GetMouseButton (0);
	}
}
