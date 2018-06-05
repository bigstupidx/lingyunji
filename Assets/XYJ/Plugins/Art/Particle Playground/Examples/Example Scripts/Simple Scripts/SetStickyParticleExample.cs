using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// This example shows how to set a certain particle as sticky. Having a particle sticky means that it will follow a transform in the scene during its lifetime.
/// </summary>
public class SetStickyParticleExample : MonoBehaviour {
	public int stickyParticle = 0;		// The sticky particle to start with (set to negative value if you don't want to start with a sticky particle)
	public Transform stickyTransform;	// The transform to stick onto
	public Vector3 stickyOffset;		// The offset from the transform
	PlaygroundParticlesC particles;		// Cached reference to the particle system
	
	IEnumerator Start () {
		
		// Cache a reference to the particle system
		particles = GetComponent<PlaygroundParticlesC>();
		
		// Set a particle as sticky if a negative value isn't set
		if (stickyParticle>-1) {
			while (!particles.IsReady())
				yield return null;
			particles.SetSticky (stickyParticle, stickyTransform.position+stickyOffset, stickyTransform.up, particles.stickyCollisionsSurfaceOffset, stickyTransform);
		}
	}
	
	void Update () {
		
		// Update the sticky position
		particles.UpdateSticky(stickyParticle);
	}
	
	/// <summary>
	/// Use this function to turn any of your particles sticky during runtime. 
	/// </summary>
	public void SetSticky (int index, Vector3 position, Vector3 normal, float offset, Transform parent) {
		particles.SetSticky (index, position, normal, offset, parent);
	}
	
	/// <summary>
	/// Make all particles non-sticky.
	/// </summary>
	public void ClearAllSticky () {
		particles.ClearCollisions();
	}
	
	/// <summary>
	/// Make specified particle non-sticky.
	/// </summary>
	public void ClearSticky (int index) {
		particles.ClearCollisions(index);
	}
}
