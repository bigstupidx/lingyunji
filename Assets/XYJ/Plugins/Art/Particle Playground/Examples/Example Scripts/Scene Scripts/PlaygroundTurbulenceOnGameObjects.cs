using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class PlaygroundTurbulenceOnGameObjects : MonoBehaviour {

	/// <summary>
	/// The transforms you wish to apply turbulence on.
	/// </summary>
	public Transform[] transforms;
	public bool turbulence3d = false;
	public float turbulenceFieldScale = 1f;
	public float turbulenceTimeScale = 1f;
	public float turbulenceStrength = 1f;
	public float damping = 1f;
	public bool returnToOrigin = true;
	public float returnSpeed = 1f;
	
	SimplexNoise simplex = new SimplexNoise();
	Vector3[] originPositions;
	Vector3[] velocity;
	bool[] hasTurbulence;
	Vector3 zero = Vector3.zero;

	void Start () {
		originPositions = new Vector3[transforms.Length];
		velocity = new Vector3[transforms.Length];
		hasTurbulence = new bool[transforms.Length];
		for (int i=0; i<transforms.Length; i++) {
			originPositions[i] = transforms[i].position;
			velocity[i] = Vector3.zero;
			hasTurbulence[i] = true;
		}
	}

	void Update () {
		Turbulence();
	}

	/// <summary>
	/// Run all objects through the turbulence algorithm.
	/// </summary>
	void Turbulence () {
		for (int i = 0; i<transforms.Length; i++) {
			if (hasTurbulence[i]) {
				velocity[i].x += SimplexValue (transforms[i].position.x,transforms[i].position.y,transforms[i].position.z);
				velocity[i].y += SimplexValue (transforms[i].position.z,transforms[i].position.x,transforms[i].position.y);
				if (turbulence3d) velocity[i].z += SimplexValue (transforms[i].position.y,transforms[i].position.z,transforms[i].position.x);
				velocity[i] = Vector3.Lerp (velocity[i], zero, Time.deltaTime*damping);
				transforms[i].position += velocity[i]*Time.deltaTime;
				if (returnToOrigin)
					transforms[i].position = Vector3.Lerp (transforms[i].position, originPositions[i], Time.deltaTime*returnSpeed);
			}
		}
	}

	/// <summary>
	/// Calculates the Simplex algorithm and returns a float.
	/// </summary>
	/// <returns>The Simplex value.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	float SimplexValue (float x, float y, float z) {
		if (turbulence3d)
			return (float) simplex.noise(x*turbulenceFieldScale,y*turbulenceFieldScale,z*turbulenceFieldScale, Time.time*turbulenceTimeScale)*Time.deltaTime*turbulenceStrength;
		else
			return (float) simplex.noise(x*turbulenceFieldScale,y*turbulenceFieldScale, Time.time*turbulenceTimeScale)*Time.deltaTime*turbulenceStrength;
	}

	public void StopTurbulenceOnTransform (Transform stopOnTransform) {
		for (int i = 0; i<transforms.Length; i++)
			if (transforms[i]==stopOnTransform)
				hasTurbulence[i] = false;
	}

	public void StartTurbulenceOnTransform (Transform startOnTransform) {
		for (int i = 0; i<transforms.Length; i++)
			if (transforms[i]==startOnTransform)
				hasTurbulence[i] = true;
	}
}