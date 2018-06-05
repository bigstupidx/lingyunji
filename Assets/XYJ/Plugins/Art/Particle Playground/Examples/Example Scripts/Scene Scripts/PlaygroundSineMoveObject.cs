using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
public class PlaygroundSineMoveObject : MonoBehaviour {

	public Vector2 minPos = new Vector2(-6.5f, -3.5f);
	public Vector2 maxPos = new Vector2(6.5f, 3.5f);
	public float speed = 1f;
	public float timeOffsetX = 0f;
	public float timeOffsetY = 0f;
	Transform thisTransform;

	void Start () {
		thisTransform = transform;
	}
	
	void Update () {
		thisTransform.position = new Vector3(
			minPos.x+Mathf.PingPong ((Time.time+timeOffsetX)*speed, maxPos.x*2f),
			minPos.y+Mathf.PingPong ((Time.time+timeOffsetY)*speed, maxPos.y*2f),
			0
		);
	}
}
