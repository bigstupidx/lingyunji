//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public abstract class CameraButtons : MonoBehaviour {

	public const float c_FixedWidth = 1024.0f;
	public const float c_FixedHeight = 768.0f;
	public const float c_InvertedFixedHeight = 1.0f / c_FixedHeight;
	
	public bool showSwitchButton = true;
	
	public float guiScale = 1.0f;
	public float borderScale = 1.0f;
	public float spaceScale = 1.0f;
	
	private float m_ScreenScale;
	private float m_HorizontalSizeCorrection;
	private Rect m_ScaledScreenRect;
	
	public float topBorder = 30.0f;
	public float bottomBorder = 30.0f;
	public float leftBorder = 30.0f;
	public float rightBorder = 30.0f;
	public float defaultSpace = 15.0f;
	
	public Texture2D upArrow;
	public Texture2D downArrow;
	public Texture2D leftArrow;
	public Texture2D rightArrow;
	public Texture2D plus;
	public Texture2D minus;
	
	public GUISkin skin;
	private GUIStyle m_CircleButtonStyle;
	
	[HideInInspector] public bool upPressed = false;
	[HideInInspector] public bool downPressed = false;
	[HideInInspector] public bool leftPressed = false;
	[HideInInspector] public bool rightPressed = false;
	[HideInInspector] public bool plusPressed = false;
	[HideInInspector] public bool minusPressed = false;
	[HideInInspector] public bool switchPressed = false;
	
	private bool m_ShowGUI = true;
	
	private void InitializeScaling () {
		m_ScreenScale = Screen.height * c_InvertedFixedHeight;
		m_HorizontalSizeCorrection = Screen.width / m_ScreenScale - c_FixedWidth;
		float l_InverseGUIScale = 1.0f / guiScale;
		float l_ScaledScreenWidth = Mathf.Ceil ((c_FixedWidth + m_HorizontalSizeCorrection) * l_InverseGUIScale);
		float l_ScaledScreenHeight = Mathf.Ceil (c_FixedHeight * l_InverseGUIScale);
		m_ScaledScreenRect.Set (0.0f, 0.0f, l_ScaledScreenWidth, l_ScaledScreenHeight);
	}
	
	private void InitializeStyles () {
		m_CircleButtonStyle = new GUIStyle (skin.GetStyle ("Circle Button"));
	}
	
	private void InitializeOnGUI () {
		InitializeScaling ();
		if
			(m_CircleButtonStyle == null ||
			 Application.isEditor)
		{
			InitializeStyles ();
		}
		GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, m_ScreenScale * guiScale * Vector3.one);
		GUI.skin = skin;
	}
	
	protected void ResetButtonPressed () {
		upPressed = false;
		downPressed = false;
		leftPressed = false;
		rightPressed = false;
		plusPressed = false;
		minusPressed = false;
		switchPressed = false;
	}
	
	protected void OnGUI () {
		
		if (m_ShowGUI) {
		
			InitializeOnGUI ();
			
				// Up
			
			GUILayout.BeginArea (m_ScaledScreenRect);
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			
			GUILayout.BeginVertical ();
			GUILayout.Space (topBorder * borderScale);
			
			upPressed = GUILayout.RepeatButton (upArrow, m_CircleButtonStyle);
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			
			GUILayout.EndArea ();
			
			
				// Down
			
			GUILayout.BeginArea (m_ScaledScreenRect);
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();
			
			downPressed = GUILayout.RepeatButton (downArrow, m_CircleButtonStyle);
			
			GUILayout.Space (bottomBorder * borderScale);
			GUILayout.EndVertical ();
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			
			GUILayout.EndArea ();
			
			
				// Left
			
			GUILayout.BeginArea (m_ScaledScreenRect);
			
			GUILayout.BeginHorizontal ();
			GUILayout.Space (leftBorder * borderScale);
			
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();
			
			leftPressed = GUILayout.RepeatButton (leftArrow, m_CircleButtonStyle);
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			
			GUILayout.EndArea ();
			
			
				// Right
			
			GUILayout.BeginArea (m_ScaledScreenRect);
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();
			
			rightPressed = GUILayout.RepeatButton (rightArrow, m_CircleButtonStyle);
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			
			GUILayout.Space (rightBorder * borderScale);
			GUILayout.EndHorizontal ();
			
			GUILayout.EndArea ();
			
			
				// Minus and plus
			
			GUILayout.BeginArea (m_ScaledScreenRect);
			
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			
			minusPressed = GUILayout.RepeatButton (minus, m_CircleButtonStyle);
			GUILayout.Space (defaultSpace * spaceScale);
			plusPressed = GUILayout.RepeatButton (plus, m_CircleButtonStyle);
			
			GUILayout.Space (rightBorder * borderScale);
			GUILayout.EndHorizontal ();
			
			GUILayout.Space (bottomBorder * borderScale);
			GUILayout.EndVertical ();
			
			GUILayout.EndArea ();
			
			
				// Switch
			
			if (showSwitchButton) {
				GUILayout.BeginArea (m_ScaledScreenRect);
				
				GUILayout.BeginVertical ();
				GUILayout.Space (topBorder * borderScale);
				
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				
				switchPressed = GUILayout.Button ("Switch");
				
				GUILayout.Space (rightBorder * borderScale);
				GUILayout.EndHorizontal ();
				
				GUILayout.FlexibleSpace ();
				GUILayout.EndVertical ();
				
				GUILayout.EndArea ();
			}
		}
	}
	
	private void Update () {
		
		if (Input.GetKeyDown (KeyCode.H)) {
			m_ShowGUI = !m_ShowGUI;
		}
		
		float l_HorizontalInput = Input.GetAxisRaw ("Horizontal");
		if (l_HorizontalInput < 0.0f) {
			leftPressed = true;
		} else if (l_HorizontalInput > 0.0f) {
			rightPressed = true;
		}
		
		float l_VerticalInput = Input.GetAxisRaw ("Vertical");
		if (l_VerticalInput < 0.0f) {
			downPressed = true;
		} else if (l_VerticalInput > 0.0f) {
			upPressed = true;
		}
		
		if
			(Input.GetKey (KeyCode.Plus) ||
			 Input.GetKey (KeyCode.KeypadPlus))
		{
			plusPressed = true;
		}
		
		if
			(Input.GetKey (KeyCode.Minus) ||
			 Input.GetKey (KeyCode.KeypadMinus))
		{
			minusPressed = true;
		}
		
		PerformUpdate ();
		
		ResetButtonPressed ();
	}
	
	protected abstract void PerformUpdate ();
}
