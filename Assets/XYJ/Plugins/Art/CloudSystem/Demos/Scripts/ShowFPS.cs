//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

public class XYJShowFPS : MonoBehaviour {

	public float fpsMeasuringDelta = 2.0f;

	private float timePassed;
	private int m_FrameCount = 0;
	private float m_FPS = 0.0f;
	
	private void Start () {
		timePassed = 0.0f;
	}
	
	private void Update () {
		m_FrameCount = m_FrameCount + 1;
		timePassed = timePassed + Time.deltaTime;
		
		if (timePassed > fpsMeasuringDelta) {
			m_FPS = m_FrameCount / timePassed;
			
			timePassed = 0.0f;
			m_FrameCount = 0;
		}
	}
	
	private void OnGUI () {
		GUI.color = new Color (1.0f, 0.5f, 0.0f);
		GUILayout.Label ("FPS: " + m_FPS);
	}
}
