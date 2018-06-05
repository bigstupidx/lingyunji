//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour {
	
		// Singleton
	
	private static CameraSwitcher s_Instance;
	public static CameraSwitcher Instance {
		get {
			return (s_Instance);
		}
	}
	
	private void Awake () {
		if (s_Instance == null) {
			s_Instance = this;
		} else {
			Debug.LogError ("More than one CameraSwitcher in the scene!");
		}
	}
	
	
	private CameraRotator m_CameraRotator;
	private CameraFlyer m_CameraFlyer;
	
	public bool startWithCameraRotator = true;
	
	private void Start () {
		m_CameraRotator = GetComponentInChildren <CameraRotator> ();
		m_CameraFlyer = GetComponentInChildren <CameraFlyer> ();
		
		if (m_CameraRotator == null) {
			Debug.LogError ("No CameraRotator in a child of CameraSwitcher.");
		}
		if (m_CameraFlyer == null) {
			Debug.LogError ("No CameraFlyer in a child of CameraSwitcher.");
		}
		
		if (startWithCameraRotator) {
			m_CameraRotator.enabled = true;
			m_CameraFlyer.enabled = false;
			m_CameraFlyer.Disable ();
		} else {
			m_CameraRotator.enabled = false;
			m_CameraRotator.Disable ();
			m_CameraFlyer.enabled = true;
		}
	}
	
	public void Switch () {
		if (m_CameraRotator.enabled) {
			m_CameraRotator.enabled = false;
			m_CameraRotator.Disable ();
			m_CameraFlyer.enabled = true;
		} else {
			m_CameraRotator.enabled = true;
			m_CameraFlyer.enabled = false;
			m_CameraFlyer.Disable ();
		}
	}
}
