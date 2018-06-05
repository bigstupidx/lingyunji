using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// Scripted emission during specified time.
/// </summary>
public class EmitDuringSpecifiedTime : MonoBehaviour {

	public float seconds = 5f;
	public Vector3 position;
	public Vector3 minVelocity = new Vector3(-1f,-1f,-1f);
	public Vector3 maxVelocity = new Vector3(1f,1f,1f);
	public Color32 color = Color.white;

	PlaygroundParticlesC particles;

	IEnumerator Start () {
		particles = GetComponent<PlaygroundParticlesC>();
		yield return null;
		StartCoroutine (Emit());
	}
	
	IEnumerator Emit () {
		float emissionRate = 0;
		int emitted = 0;
		while (emitted<particles.particleCount) {
			emissionRate += (particles.particleCount/seconds)*(Time.deltaTime);
			int r = Mathf.RoundToInt(emissionRate);
			if (r>0) {
				if (emitted+r>=particles.particleCount)
					r = (particles.particleCount-emitted);
				particles.Emit (
					r,
					position,
					minVelocity,
					maxVelocity,
					color
				);
				emitted += r;
				emissionRate = 0;
			}
			yield return null;
		}
	}
}
