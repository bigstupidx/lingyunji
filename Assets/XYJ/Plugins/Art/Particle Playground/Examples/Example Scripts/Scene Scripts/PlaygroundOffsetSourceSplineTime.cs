using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class PlaygroundOffsetSourceSplineTime : MonoBehaviour {

	public PlaygroundParticlesC particles;
	public float offsetSpeed = 1f;
	public bool showGui = true;

	void Start () {
		if (particles==null)
			particles = GetComponent<PlaygroundParticlesC>();
	}
	
	void Update () {
		particles.splineTimeOffset = (particles.splineTimeOffset+offsetSpeed*Time.deltaTime)%1f;
	}

	void OnGUI () {
		if (showGui) {
			GUI.Box (new Rect(Screen.width-220,0,220,70), "Spline Offset");
			GUI.Label (new Rect(Screen.width-210, 30, 200, 30), "Offset Speed: "+offsetSpeed.ToString("f1"));
			offsetSpeed = GUI.HorizontalSlider(new Rect(Screen.width-210, 55, 200, 30), offsetSpeed, 0, 2f);
		}
	}
}
