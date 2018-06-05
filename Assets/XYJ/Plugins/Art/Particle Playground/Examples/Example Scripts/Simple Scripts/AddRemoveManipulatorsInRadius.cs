using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// Adds and removes manipulators within specified radius. Note that your objects within layer must have a collider to work with the OverlapSphere.
/// </summary>
public class AddRemoveManipulatorsInRadius : MonoBehaviour {

	public PlaygroundParticlesC particles;
	public Transform originTransform;
	public float radius = 10f;
	public LayerMask layer;
	public float checkRate = 1f;

	void Start () {
		InvokeRepeating ("AddRemoveInRange", .01f, checkRate);
	}

	void AddRemoveInRange () {

		RemoveOutOfRange();

		Collider[] inRangeObjects = Physics.OverlapSphere(originTransform.position, radius, layer);
		for (int i = 0; i<inRangeObjects.Length; i++) {
			if (!ManipulatorHasTransform (inRangeObjects[i].transform)) {
				ManipulatorObjectC newManipulator = PlaygroundC.ManipulatorObject (inRangeObjects[i].transform, particles);

				// Setup the added manipulator here
				newManipulator.type = MANIPULATORTYPEC.Repellent;
				newManipulator.size = 5f;
				newManipulator.strength = 3f;
			}
		}
	}

	void RemoveOutOfRange () {
		for (int i = 0; i<particles.manipulators.Count; i++) {
			if (Vector3.Distance (originTransform.position, particles.manipulators[i].transform.transform.position)>radius) {
				particles.manipulators.RemoveAt(i);
				if (i>0) i--;
			}
		}
	}

	bool ManipulatorHasTransform (Transform comparer) {
		for (int i = 0; i<particles.manipulators.Count; i++) {
			if (particles.manipulators[i].transform.transform == comparer)
				return true;
		}
		return false;
	}
}
