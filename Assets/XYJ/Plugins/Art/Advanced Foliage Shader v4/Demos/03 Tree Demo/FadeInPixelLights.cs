using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FadeInPixelLights : MonoBehaviour {
	//public GameObject Camera;
	public float MaxDistance = 70.0f;
	public float Fadelength = 10.0f;
	public float Intensity = 1.0f;

	private float distance;
	private float factor = 1.0f;
	private Light lt;

	// Use this for initialization
	void Awake () {
		lt = gameObject.GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		distance = (transform.position - Camera.main.transform.position).magnitude;
		factor = Mathf.Clamp01( (MaxDistance - distance) / Fadelength);
		lt.intensity = Intensity*factor;
	}
}
