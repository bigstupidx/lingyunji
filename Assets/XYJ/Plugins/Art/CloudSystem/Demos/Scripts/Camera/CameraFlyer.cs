//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class CameraFlyer : CameraButtons {
	
	private Transform m_Transform;
	private Camera m_Camera;
	private AudioListener m_AudioListener;
	
	public float horizontalRotationSpeedMultiplier = 50.0f;
	public float verticalRotationSpeedMultiplier = 50.0f;
	
	public float startForwardSpeed = 10.0f;
	public float minForwardSpeed = 0.0f;
	public float maxForwardSpeed = 100.0f;
	public float forwardSpeedMultiplier = 50.0f;
	private float m_CurrentSpeed;
	
	private void Awake () {
		m_Transform = transform;
		m_Camera = GetComponentInChildren <Camera> ();
		m_AudioListener = GetComponentInChildren <AudioListener> ();
		if (m_Camera == null) {
			Debug.LogError ("CameraFlyer has no Camera in a child.");
		}
		if (m_AudioListener == null) {
			Debug.LogError ("CameraFlyer has no AudioListener in a child.");
		}
		
		m_CurrentSpeed = startForwardSpeed;
	}
	
	private void OnEnable () {
		m_Camera.enabled = true;
		m_AudioListener.enabled = true;
	}
	
	public void Disable () {
		m_Camera.enabled = false;
		m_AudioListener.enabled = false;
	}
	
	protected override void PerformUpdate () {
		
			// Vertical rotation
		
		float l_VerticalRotationFactor = 0.0f;
		if (downPressed) {
			l_VerticalRotationFactor = l_VerticalRotationFactor + 1.0f;
		}
		if (upPressed) {
			l_VerticalRotationFactor = l_VerticalRotationFactor - 1.0f;
		}
		m_Transform.Rotate (Vector3.right, l_VerticalRotationFactor * verticalRotationSpeedMultiplier * Time.deltaTime, Space.Self);
		
		
			// Horizontal rotation
		
		float l_HorizontalRotationFactor = 0.0f;
		if (leftPressed) {
			l_HorizontalRotationFactor = l_HorizontalRotationFactor - 1.0f;
		}
		if (rightPressed) {
			l_HorizontalRotationFactor = l_HorizontalRotationFactor + 1.0f;
		}
		m_Transform.Rotate (Vector3.up, l_HorizontalRotationFactor * horizontalRotationSpeedMultiplier * Time.deltaTime, Space.World);
		
		
			// Scale
		
		float l_ScaleFactor = 0.0f;
		if (plusPressed) {
			l_ScaleFactor = l_ScaleFactor + 1.0f;
		}
		if (minusPressed) {
			l_ScaleFactor = l_ScaleFactor - 1.0f;
		}
		m_CurrentSpeed = m_CurrentSpeed + l_ScaleFactor * forwardSpeedMultiplier * Time.deltaTime;
		m_CurrentSpeed = Mathf.Clamp (m_CurrentSpeed, minForwardSpeed, maxForwardSpeed);
		
		m_Transform.position = m_Transform.position + (m_Transform.forward * m_CurrentSpeed * Time.deltaTime);
		
		
			// Switch
		
		if (switchPressed) {
			CameraSwitcher.Instance.Switch ();
		}
	}
}
