using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// Scale a Particle Playground system. Assign this script to the Particle Playground system's GameObject.
/// Comment/Uncomment [ExecuteInEditMode()] below to enable or disable the effect in Edit Mode.
/// </summary>
//[ExecuteInEditMode()]
public class ParticleSystemScaler : MonoBehaviour {

	/// <summary>
	/// The overall scale. Note that original source positions won't be affected.
	/// </summary>
	public float scale = 1f;
	public bool scaleVelocity = true;
	public bool scaleParticleSize = true;
	public bool scaleLifetimePositioning = true;
	public bool scaleOverflowOffset = true;
	public bool scaleSourceScatter = true;
	float origVelocityScale;
	float origScale;
	float origLifetimePositioningScale;
	Vector3 origOverflowOffset;
	Vector3 origSourceScatterMin;
	Vector3 origSourceScatterMax;
	PlaygroundParticlesC particles;

	void OnEnable () {
		if (particles==null)
			particles = GetComponent<PlaygroundParticlesC>();
		origVelocityScale = particles.velocityScale;
		origScale = particles.scale;
		origLifetimePositioningScale = particles.lifetimePositioningScale;
		origOverflowOffset = particles.overflowOffset;
		origSourceScatterMin = particles.sourceScatterMin;
		origSourceScatterMax = particles.sourceScatterMax;
	}

	void OnDisable () {
		particles.velocityScale = origVelocityScale;
		particles.scale = origScale;
		particles.lifetimePositioningScale = origLifetimePositioningScale;
		particles.overflowOffset = origOverflowOffset;
		particles.sourceScatterMin = origSourceScatterMin;
		particles.sourceScatterMax = origSourceScatterMax;
	}
	
	void Update () {
		if (scaleVelocity)
			particles.velocityScale = origVelocityScale*scale;
		if (scaleParticleSize)
			particles.scale = origScale*scale;
		if (scaleLifetimePositioning)
			particles.lifetimePositioningScale = origLifetimePositioningScale*scale;
		if (scaleOverflowOffset)
			particles.overflowOffset = origOverflowOffset*scale;
		if (scaleSourceScatter && particles.applySourceScatter && (particles.sourceScatterMin!=origSourceScatterMin*scale || particles.sourceScatterMax!=origSourceScatterMax*scale)) {
			particles.sourceScatterMin = origSourceScatterMin*scale;
			particles.sourceScatterMax = origSourceScatterMax*scale;
			particles.RefreshScatter();
		}
	}
}
