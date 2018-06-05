using UnityEngine;
using System.Collections;

public class AzureTargetRender : MonoBehaviour {

	public RenderTexture targetRender;

	// Use this for initialization
	void Awake () {
		SetTargetTexture ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void SetTargetTexture() {
		GetComponent<Camera>().targetTexture = targetRender;
	}
}