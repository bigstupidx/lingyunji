using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;

public class PlaygroundManipulatorEvents : MonoBehaviour {

	public PlaygroundParticlesC bubbleParticles;			// The large bubbles, this system also contain the manipulator
	public PlaygroundParticlesC popParticles;				// The spark effect when a bubble pops
	public PlaygroundParticlesC popTurbulenceParticles;		// The small bubbles when a bubble pops
	public float maxRandomTurbulence = 1f;					// The maximum random velocity for popTurbulenceParticles
	public float guiUpdateTime = .25f;						// The time between gui updates for the upper left info text
	public GUIText text;									// The upper left info text
	public GUIText popText;									// The text to display whenever highest pop changes

	ManipulatorObjectC manipulator;							// Cached manipulator

	Transform manipulatorTransform;							// Cached manipulator transform
	Camera mainCamera;										// Cached main camera
	Color popTextColor;										// Cached color for popText
	int highestPop;											// Keep track of highest pop
	
	void Start () {

		// Cache the manipulator of bubbleParticles (at list position 0)
		manipulator = PlaygroundC.GetManipulator (0, bubbleParticles);

		// Cache the manipulator transform, this looks a bit awkward at first, however the transform of a
		// manipulator is a thread-safe wrapper class which has a variable containing the Transform component.
		// In this example scene manipulatorTransform = transform; is the same.
		manipulatorTransform = manipulator.transform.transform;

		// Hook up functions to the Event Delegates on the manipulator
		manipulator.particleEventEnter += OnManipulatorEnter;
		manipulator.particleEventExit += OnManipulatorExit;
		manipulator.particleEventBirth += OnManipulatorBirth;
		manipulator.particleEventDeath += OnManipulatorDeath;
		manipulator.particleEventCollision += OnManipulatorCollision;

		mainCamera = Camera.main;
		popTextColor = popText.color;
		InvokeRepeating ("UpdateGUI", 0, guiUpdateTime);
	}

	void Update () {

		// Make the manipulator follow the mouse position on screen (camera is 14 units from 0)
		manipulatorTransform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition+new Vector3(0,0,14));

		// Call Pop() whenever left mouse button is pressed
		if (Input.GetMouseButton (0))
			StartCoroutine (Pop());
	}

	IEnumerator Pop () {

		/* A manipulator using events will contain a list of all its particles in form of List<ManipulatorParticle>
		 * where each item has a particle system id and a particle id.
		 * When using manipulator.GetParticles() the list will be converted into List<PlaygroundEventParticle> and
		 * contain detailed information about each particle currently inside its shape.
		 */

		// Get the list of particles inside the manipulator
		List<PlaygroundEventParticle> manipulatorParticles = manipulator.GetParticles();

		// Work through the list and emit popParticles and popTurbulenceParticles
		foreach (PlaygroundEventParticle particle in manipulatorParticles) {
			popParticles.Emit (particle.position, particle.velocity, particle.color);
			for (int i = 0; i<5; i++)
				popTurbulenceParticles.Emit (particle.position, particle.velocity+RandomV3(), particle.color);

		}

		// Kill all tracked particles within the Manipulator
		manipulator.KillAllParticles();

		// This part is just for the gui text
		if (manipulatorParticles.Count>0) {
			if (manipulatorParticles.Count>highestPop) {
				popTextColor.a = 1f;
				popText.color = popTextColor;
				highestPop = manipulatorParticles.Count;
				popText.text = highestPop.ToString();
				Vector2 pixelOffset = mainCamera.WorldToScreenPoint(manipulatorTransform.position);
				pixelOffset.y += 50;
				popText.pixelOffset = pixelOffset;

				int highestNow = highestPop;
				while (popTextColor.a>0 && highestNow==highestPop) {
					popTextColor.a -= .5f*Time.deltaTime;
					popText.color = popTextColor;
					yield return null;
				}
			}
		}


	}
	
	void OnManipulatorEnter (PlaygroundEventParticle particle) {

		/* A manipulator using events will send information to any Event Listeners (such as this function).
		 * OnManipulatorEnter() has hooked up in Start() to the Event Delegate of the manipulator attached to bubbleParticles.
		 * Whenever a particle enters the confined space of the manipulator OnManipulatorEnter() will be called.
		 * You can use this to know which particle it is, where it entered and extract further information such as velocity,
		 * size, color and if it has been altered by any properties. The particle id is the same number as within its position 
		 * in the playgroundCache. Uncomment the section below for an example in the console.
		*/
		/*
		Debug.Log ("Position: "+			particle.position			+"\n"+
		           "Velocity: "+			particle.velocity			+"\n"+
		           "Color: "+				particle.color				+"\n"+
		           "Particle System Id: "+	particle.particleSystemId	+"\n"+
		           "Particle Id: "+			particle.particleId			+"\n"
		           );
		*/
	}

	void OnManipulatorExit (PlaygroundEventParticle particle) {

		// A particle left the manipulator. Uncomment the line below for an example in the console.
		//Debug.Log ("Particle "+particle.particleId+" from system "+particle.particleSystemId+" left the manipulator.");
	}

	void OnManipulatorBirth (PlaygroundEventParticle particle) {

		// A particle has birth inside the manipulator. Uncomment the line below for an example in the console.
		//Debug.Log ("Particle "+particle.particleId+" from system "+particle.particleSystemId+" is alive and inside the manipulator.");
	}

	void OnManipulatorDeath (PlaygroundEventParticle particle) {
		
		// A particle has died inside the manipulator. Uncomment the line below for an example in the console.
		//Debug.Log ("Particle "+particle.particleId+" from system "+particle.particleSystemId+" has died inside the manipulator.");
	}

	void OnManipulatorCollision (PlaygroundEventParticle particle) {
		
		// A particle has collided inside the manipulator. You need to activate collision on bubbleParticles for this function to have effect. Uncomment the line below for an example in the console.
		//Debug.Log ("Particle "+particle.particleId+" from system "+particle.particleSystemId+" has collided with "+particle.collisionTransform.name+" at position "+particle.collisionParticlePosition+" inside the manipulator.");
	}

	Vector3 RandomV3 () {

		// Add some random velocity to the smaller bubbles (popTurbulenceParticles)
		return new Vector3(
			Random.Range (-maxRandomTurbulence, maxRandomTurbulence),
			Random.Range (-maxRandomTurbulence, maxRandomTurbulence),
			Random.Range (-maxRandomTurbulence, maxRandomTurbulence)
		);
	}

	void UpdateGUI () {

		// Update the gui text in the upper left corner (called each guiUpdateTime)
		text.text = "Bubbles in control: "+manipulator.particles.Count+"\nHighest pop: "+highestPop;
	}
}
