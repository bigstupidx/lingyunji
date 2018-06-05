﻿using UnityEngine;
using System.Collections;
using ParticlePlayground;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#else
using UnityEngine.SceneManagement;
#endif

public class PlaygroundScenes : MonoBehaviour {

	public bool showInfo = true;
	public bool listParticleSystems = true;
	public bool canSwitchSceneByClick = true;
	public PlaygroundParticlesC[] particles;

	string productLabel = "Particle Playground";
	string sceneLabel;
	string contentLabel;
	string systemInfoLabel;
	string fpsLabel = "wait...";
	string timeLabel;
	string totalParticlesLabel;
	int totalParticles;
	int totalParticleSystems;

	float updateRate = 2.0f;
	float fps;
	float highestFps;
	float lowestFps=9999f;
	float deltaCount;
	int frameCount;

	bool ready = false;

	IEnumerator Start () {
		// Wait for particle systems to be fully ready (due to events data 3 is needed)
		yield return null;
		yield return null;
		yield return null;

		// Prepare info
		productLabel += " "+PlaygroundC.version+PlaygroundC.specialVersion;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		sceneLabel = Application.loadedLevelName+" Scene";
#else
		sceneLabel = SceneManager.GetActiveScene().name+" Scene";
#endif
		InvokeRepeating("UpdateParticlesLabel", 0f, 1f);
		systemInfoLabel = SystemInfo.deviceModel+"\n"+SystemInfo.processorType+"\n("+SystemInfo.processorCount+" cores, "+SystemInfo.systemMemorySize+"MB RAM)\n"+Screen.width+"x"+Screen.height+" @"+Screen.currentResolution.refreshRate+" ("+SystemInfo.graphicsMemorySize+"MB VRAM)";
		ready = true;
	}
	
	void Update () {
		if (!ready) return;
		if (showInfo) {
			frameCount++;
			deltaCount += Time.deltaTime;
			if (deltaCount > 1.0f/updateRate) {
				fps = frameCount / deltaCount ;
				frameCount = 0;
				deltaCount -= 1.0f/updateRate;
				if (fps>highestFps)
					highestFps = fps;
				if (fps<lowestFps)
					lowestFps = fps;
				fpsLabel = "FPS: "+fps.ToString("f0")+"\nHighest: "+highestFps.ToString("f0")+"\nLowest: "+lowestFps.ToString("f0");
			}
		}
		if (((Input.GetMouseButtonDown(0) && canSwitchSceneByClick || Input.touchCount>0) && canSwitchSceneByClick) || Input.GetKeyDown (KeyCode.Return))
			LoadNext();
		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();
	}

	void LoadNext () {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		Application.LoadLevel ((Application.loadedLevel+1)%Application.levelCount);
#else
		SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex+1)%SceneManager.sceneCount);
#endif
	}

	void OnGUI () {
		GUILayout.Label (productLabel);
		if (showInfo) {
			GUILayout.Label (sceneLabel);
			GUILayout.Label (contentLabel);
			GUILayout.Label (totalParticlesLabel);
			GUILayout.Label (systemInfoLabel);
			GUILayout.Label (fpsLabel);
			GUILayout.Label (timeLabel);
		}
	}

	public void UpdateParticlesLabel () {
		totalParticles = 0;
		totalParticleSystems = 0;
		contentLabel = "";
		foreach (PlaygroundParticlesC p in particles) {
			if (listParticleSystems)
				contentLabel += p.particleSystemTransform.name+" ("+p.particleCount+" "+SourceMethod(p)+" particles)"+"\n";
			totalParticles+=p.particleCount;
			totalParticleSystems++;
		}

		totalParticlesLabel = "Total "+totalParticles+" particles ("+totalParticleSystems+")";

		// Update time here as well
		timeLabel = "Seconds: "+Time.timeSinceLevelLoad.ToString ("f0");
	}

	string SourceMethod (PlaygroundParticlesC source) {
		string returnString;
		switch (source.source) {
		case SOURCEC.Paint:returnString = "painted";break;
		case SOURCEC.Projection:returnString = "projected";break;
		case SOURCEC.SkinnedWorldObject:returnString = "skinned mesh";break;
		case SOURCEC.State:returnString = "state";break;
		case SOURCEC.Transform:returnString = "transform";break;
		case SOURCEC.WorldObject:returnString = "mesh";break;
		case SOURCEC.Script:returnString =(source.eventControlledBy.Count>0)? "event" : "scripted";break;
		default:returnString = "";break;
		}
		return returnString;
	}
}
