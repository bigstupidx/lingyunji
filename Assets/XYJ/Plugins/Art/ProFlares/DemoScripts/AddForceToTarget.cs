/// ProFlares - v1.05 - Copyright 2013-2014 All rights reserved - ProFlares.com

using UnityEngine;
using System.Collections;
namespace ProFlares {
public class AddForceToTarget : MonoBehaviour {
	public Transform target;
	public float force;
	
	public ForceMode mode;
	 
	void FixedUpdate () {
		 
		 
			float dist = (Vector3.Distance(transform.position,target.position)*0.01f);
		
 
			Vector3 dir = target.position-transform.position;
		 
		 	GetComponent<Rigidbody>().AddForce(dir*(force*dist),mode);
		
	}
}
}