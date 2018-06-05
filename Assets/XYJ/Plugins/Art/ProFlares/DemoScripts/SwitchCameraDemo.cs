/// ProFlares - v1.05 - Copyright 2013-2014 All rights reserved - ProFlares.com

using UnityEngine;
using System.Collections;

public class SwitchCameraDemo : MonoBehaviour {
	
	public ProFlareBatch flareBatch;
	
	public Camera camera1;
	public Camera camera2;
	
	public bool switchNow;
	
	bool ping; 
	void Start () {
		camera2.enabled = false;
	}
	 
	void Update () {
		if(switchNow){
		
			switchNow = false;
			
			if(!ping){
				ping = true;
				flareBatch.SwitchCamera(camera2);
				camera2.enabled = true;
				camera1.enabled = false;
			}else{
				ping = false;
				flareBatch.SwitchCamera(camera1);
				camera1.enabled = true;
				camera2.enabled = false;
				
			} 
		} 
	}
}
