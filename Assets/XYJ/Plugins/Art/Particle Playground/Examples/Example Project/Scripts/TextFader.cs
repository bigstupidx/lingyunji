using UnityEngine;
using System.Collections;

public class TextFader : MonoBehaviour {

	GUIText gText;
	Color startColor;
	Color fadedColor;

	void Start () {
		gText = GetComponent<GUIText>();
		startColor = gText.material.color;
		fadedColor = new Color(startColor.r, startColor.g, startColor.b, .5f);
	}
	
	void Update () {
		gText.material.color = Color.Lerp (startColor, fadedColor, Mathf.PingPong (Time.time*2f, 1f));
	}
}
