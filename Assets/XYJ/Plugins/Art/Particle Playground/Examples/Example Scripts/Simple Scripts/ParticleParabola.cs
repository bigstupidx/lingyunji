using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// Example of distributing particles from start to end with a bending.
/// </summary>
public class ParticleParabola : MonoBehaviour {

	public PlaygroundParticlesC particles;
	public Vector3 startPosition;
	public Vector3 endPosition = new Vector3(10,0,0);
	public Vector3 bending = Vector3.up;

	void Start () {
		// Assume this GameObect is particles is not assigned
		if (particles==null)
			particles = GetComponent<PlaygroundParticlesC>();
	}

	void Update () {
		if (Input.GetMouseButtonDown (0))
			ShootParabola();
	}

	void ShootParabola () {
		for (int i = 0; i<particles.particleCount; i++) {
			float movePercentage = (i*1f)/(particles.particleCount*1f);
			Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, movePercentage);
			
			currentPos.x += bending.x*Mathf.Sin(Mathf.Clamp01(movePercentage) * Mathf.PI);
			currentPos.y += bending.y*Mathf.Sin(Mathf.Clamp01(movePercentage) * Mathf.PI);
			currentPos.z += bending.z*Mathf.Sin(Mathf.Clamp01(movePercentage) * Mathf.PI);

			particles.Emit(currentPos, Vector3.zero, Color.white);

		}
	}

}
