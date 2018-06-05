using UnityEngine;
using System.Collections;
using ParticlePlayground;

[ExecuteInEditMode()]
public class EmitOnTranslation : MonoBehaviour {

	public PlaygroundParticlesC particles;				// Assign the particle system in Inspector
	public Transform movingTransform;					// Assign the moving Transform in Inspector
	public float minSpace = .1f;						// The minimum space required for emission

	bool isReady;
	Vector3 previousPosition;
	
	void Start () {
		if (particles==null)
			particles = GetComponent<PlaygroundParticlesC>();
		if (movingTransform==null)
			movingTransform = transform;
		if (particles!=null&&movingTransform!=null)
			isReady = true;
		previousPosition = movingTransform.position;
	}
	
	void Update () {
		if (isReady)
			EmissionMoveCheck();
	}

	/// <summary>
	/// Emit when particle system is moving.
	/// </summary>
	void EmissionMoveCheck () {
		if (movingTransform.position!=previousPosition) {
			float emissionStepper = 0;
			float deltaDistance = Vector3.Distance (previousPosition, movingTransform.position);
			if (deltaDistance<minSpace) return;
			Vector3 delta = previousPosition;
			Vector3 velocity = Vector3.zero;
			Color color = Color.white;
			while (emissionStepper<deltaDistance) {
				delta = Vector3.Lerp (previousPosition, movingTransform.position, emissionStepper/deltaDistance);
				particles.Emit(delta, velocity, color);
				emissionStepper += minSpace;
			}
		}
		previousPosition = movingTransform.position;
	}
}
