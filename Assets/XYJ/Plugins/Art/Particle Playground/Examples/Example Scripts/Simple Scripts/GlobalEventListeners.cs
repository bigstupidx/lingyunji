using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// An example of assigning to the global event delegates. This requires that your particle system(s) have enabled "Send To Manager" in the events you wish to broadcast.
/// </summary>
public class GlobalEventListeners : MonoBehaviour {
	
	void OnEnable () {

		// Here we assign our functions to be called whenever an event occurs
		PlaygroundC.particleEventBirth 		+= OnEventParticleBirth;
		PlaygroundC.particleEventDeath 		+= OnEventParticleDeath;
		PlaygroundC.particleEventCollision 	+= OnEventParticleCollision;
		PlaygroundC.particleEventTime 		+= OnEventParticleTime;
	}

	void OnDisable () {

		// Here we remove our functions
		PlaygroundC.particleEventBirth 		-= OnEventParticleBirth;
		PlaygroundC.particleEventDeath 		-= OnEventParticleDeath;
		PlaygroundC.particleEventCollision 	-= OnEventParticleCollision;
		PlaygroundC.particleEventTime 		-= OnEventParticleTime;
	}

	void OnEventParticleBirth (PlaygroundEventParticle particle) {
		Debug.Log ("A particle came to life at "+particle.position+" with a size of "+particle.size);
	}

	void OnEventParticleDeath (PlaygroundEventParticle particle) {
		Debug.Log ("A particle died at "+particle.position+" with the velocity of "+particle.velocity+". It was born in "+particle.targetPosition);
	}

	void OnEventParticleCollision (PlaygroundEventParticle particle) {
		Debug.Log ("A particle collided at "+particle.position+" with "+particle.collisionTransform.name+". The id of the particle was "+particle.particleId+" from particle system "+particle.particleSystemId);
	}

	void OnEventParticleTime (PlaygroundEventParticle particle) {
		Debug.Log ("A particle called in at "+particle.position+" with the lifetime of "+particle.life);
	}
}
