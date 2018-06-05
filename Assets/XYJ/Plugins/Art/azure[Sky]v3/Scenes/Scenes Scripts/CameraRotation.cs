using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour {

	private Vector3 v3;
	public float speed = 100;
	
	void Start()
	{
		v3= transform.localEulerAngles;
		Application.targetFrameRate = 60;
	}
	
	void LateUpdate()
	{
		if(Input.GetMouseButton(1))
		{
			
			v3.x -= (Input.GetAxis("Mouse Y") * speed)*Time.deltaTime;
			v3.y += (Input.GetAxis("Mouse X") * speed)*Time.deltaTime;      
		}
		v3=clamp(v3);
		transform.localEulerAngles=v3;
	}
	
	private Vector3 clamp(Vector3 v3)
	{
		if (v3.x>360)v3.x-=360;
		if (v3.x<-360)v3.x+=360;
		if (v3.y>360)v3.y-=360;
		if (v3.y<-360)v3.y+=360;
		return v3;
	}
}
