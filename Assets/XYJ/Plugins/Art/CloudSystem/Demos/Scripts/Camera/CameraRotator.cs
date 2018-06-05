//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class CameraRotator : CameraButtons {

	private Transform m_Transform;
	private Camera m_Camera;
	private AudioListener m_AudioListener;
	
	public float startScale = 4.0f;
	public float minScale = 1.0f;
	public float maxScale = 5.0f;
	public float scaleSpeedMultiplier = 2.0f;
	private float m_Scale;
	
	public float startXRotation = 30.0f;
	public float minXRotation = -85.0f;
	public float maxXRotation = 85.0f;
	public float horizontalRotationSpeedMultiplier = 100.0f;
	public float verticalRotationSpeedMultiplier = 50.0f;
	
	private void Awake () {
		m_Transform = transform;
		m_Camera = GetComponentInChildren <Camera> ();
		m_AudioListener = GetComponentInChildren <AudioListener> ();
		if (m_Camera == null) {
			Debug.LogError ("CameraRotator has no Camera in a child.");
		}
		if (m_AudioListener == null) {
			Debug.LogError ("CameraRotator has no AudioListener in a child.");
		}
		
		Vector3 l_Rotation = m_Transform.rotation.eulerAngles;
		l_Rotation.x = startXRotation;
		m_Transform.rotation = Quaternion.Euler (l_Rotation);
		
		m_Scale = startScale;
		m_Transform.localScale = Vector3.one * m_Scale;
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
		
		Vector3 l_Rotation = m_Transform.rotation.eulerAngles;
		
		
			// Vertical rotation
		
		float l_VerticalRotationFactor = 0.0f;
		if (downPressed) {
			l_VerticalRotationFactor = l_VerticalRotationFactor - 1.0f;
		}
		if (upPressed) {
			l_VerticalRotationFactor = l_VerticalRotationFactor + 1.0f;
		}

		l_Rotation.x = l_Rotation.x + l_VerticalRotationFactor * verticalRotationSpeedMultiplier * Time.deltaTime;
		if (l_Rotation.x > 90.0f) {
			l_Rotation.x = l_Rotation.x - 360.0f;
		}
		l_Rotation.x = Mathf.Clamp (l_Rotation.x, minXRotation, maxXRotation);
		
		
			// Horizontal rotation
		
		float l_HorizontalRotationFactor = 0.0f;
		if (leftPressed) {
			l_HorizontalRotationFactor = l_HorizontalRotationFactor + 1.0f;
		}
		if (rightPressed) {
			l_HorizontalRotationFactor = l_HorizontalRotationFactor - 1.0f;
		}
		
		l_Rotation.y = l_Rotation.y + l_HorizontalRotationFactor * horizontalRotationSpeedMultiplier * Time.deltaTime;
		m_Transform.rotation = Quaternion.Euler (l_Rotation);
		
		
			// Scale
		
		float l_ScaleFactor = 0.0f;
		if (plusPressed) {
			l_ScaleFactor = l_ScaleFactor - 1.0f;
		}
		if (minusPressed) {
			l_ScaleFactor = l_ScaleFactor + 1.0f;
		}
		m_Scale = m_Scale + l_ScaleFactor * scaleSpeedMultiplier * Time.deltaTime;
		m_Scale = Mathf.Clamp (m_Scale, minScale, maxScale);
		m_Transform.localScale = Vector3.one * m_Scale;
		
		
			// Switch
		
		if (switchPressed) {
			CameraSwitcher.Instance.Switch ();
		}
	}
}
