using UnityEngine;
using System.Collections;

public class PlaygroundPingPongMove : MonoBehaviour {

	public float speed = 1f;
	public Vector3 moveLength;
	Vector3 startPos;
	Transform thisTransform;

	void Start () {
		thisTransform = transform;
		startPos = transform.position;
	}
	
	void Update () {
		Vector3 move = new Vector3(
			moveLength.x==0?0:Mathf.PingPong (Time.time*speed, moveLength.x),
			moveLength.y==0?0:Mathf.PingPong (Time.time*speed, moveLength.y),
			moveLength.z==0?0:Mathf.PingPong (Time.time*speed, moveLength.z)
		);
		thisTransform.position = startPos+move;
	}
}
