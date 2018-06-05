using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class CircleEmit : MonoBehaviour {

	public float velocity = 1f;
	public Color color = Color.white;
	public float width = 1f;
	public float height = 1f;

	IEnumerator Start () {
		PlaygroundParticlesC particles = GetComponent<PlaygroundParticlesC>();
		while (!particles.IsReady())
			yield return null;

		Vector3 position = particles.particleSystemTransform.position;
		for (int i = 0; i<particles.particleCount; i++) {
			float iCircle = 360f*((i*1f)/(particles.particleCount*1f));
			Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad*iCircle)*width, Mathf.Sin(Mathf.Deg2Rad*iCircle)*height, 0);
			particles.Emit (position, direction*velocity, color);
		}
	}
}
