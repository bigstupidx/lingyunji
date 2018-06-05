using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;

public class EventListenerEnableSystems : MonoBehaviour {
	
	public PlaygroundParticlesC mainParticles;						// You particle system sending events

	// Particle system pool settings (for performance)
	public GameObject particleSystemPoolPrefab;						// The repeated particle system to instantiate and keep as a pool
	public int quantity = 10;										// The number of pooled particle systems
	int currentEnabled = 0;											// The current enabled particle system
	List <PlaygroundParticlesC> cachedParticles;					// The pooled particle systems
	List <Vector3> queuedSystems = new List<Vector3>();				// The queued list of particle system positions to enable on main-thread

	void Start () {

		// Get the event
		PlaygroundEventC mainParticleEvent = PlaygroundC.GetEvent (0, mainParticles);

		// Make sure the event delegate is set to target event listeners
		mainParticleEvent.broadcastType = EVENTBROADCASTC.EventListeners;

		// Hook up to the event delegate
		mainParticleEvent.particleEvent += OnParticleEvent;

		// Cache the particle systems in the pool
		cachedParticles = new List<PlaygroundParticlesC>();
		for (int i = 0; i<quantity; i++) {
			GameObject go = (GameObject)Instantiate ((Object)particleSystemPoolPrefab);
			cachedParticles.Add (go.GetComponent<PlaygroundParticlesC>());
			cachedParticles[i].particleSystemGameObject.SetActive(false);
		}
	}

	void Update () {

		// Check if we have any new particle systems in queue
		if (queuedSystems.Count>0) {

			// Use a temporary list as the queuedSystems list might update in the middle of execution (from another thread)
			Vector3[] queue = queuedSystems.ToArray();
			queuedSystems.Clear();
			foreach (Vector3 pos in queue)
				EnableParticleSystem(pos);
		}
	}

	/// <summary>
	/// Called upon the event, this function will be called from a different thread if multithreading is enabled.
	/// </summary>
	void OnParticleEvent (PlaygroundEventParticle particle) {
		queuedSystems.Add (particle.position);
	}

	/// <summary>
	/// Enables the next particle system in the particle pool.
	/// </summary>
	/// <param name="position">Position.</param>
	public void EnableParticleSystem (Vector3 position) {
		cachedParticles[currentEnabled].particleSystemTransform.position = position;
		cachedParticles[currentEnabled].Emit (true);
		currentEnabled++;
		currentEnabled = currentEnabled%cachedParticles.Count;
	}
}
