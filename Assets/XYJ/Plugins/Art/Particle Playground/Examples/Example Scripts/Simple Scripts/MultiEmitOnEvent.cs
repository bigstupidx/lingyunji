using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// Emit several particles upon a particle event.
/// </summary>
public class MultiEmitOnEvent : MonoBehaviour {

	public PlaygroundParticlesC particlesEvent;					// The event particle system
	public PlaygroundParticlesC particlesEmit;					// The emission particle system
	public int emitCount = 8;									// The amount of particles
	public Vector3 randomVelocityMin = new Vector3(-1f,0,-1f);	// The minimum random velocity
	public Vector3 randomVelocityMax = new Vector3(1f,1f,1f);	// The maximum random velocity
	public Color32 color = Color.white;							// The color of particles

	void Start () {

		// Specify which event you would like to listen to
		PlaygroundC.GetEvent (0, particlesEvent).particleEvent += EmitOnEvent;
	}

	/// <summary>
	/// Emits particles whenever the event is triggered. You could use more info from the passed in event particle if you'd like for more advanced emission behaviors.
	/// Note that this will by default be called on a second thread.
	/// </summary>
	/// <param name="particle">Event Particle.</param>
	void EmitOnEvent (PlaygroundEventParticle particle) {
		particlesEmit.ThreadSafeEmit (emitCount, particle.collisionParticlePosition, randomVelocityMin, randomVelocityMax, color);
	}
}
