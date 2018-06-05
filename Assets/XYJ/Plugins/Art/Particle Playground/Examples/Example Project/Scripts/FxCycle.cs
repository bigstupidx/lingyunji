using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;
using PlaygroundSplines;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#else
using UnityEngine.SceneManagement;
#endif

/// <summary>
/// The Fx Cycle takes care of everything happening in the scene.
/// </summary>
public class FxCycle : MonoBehaviour {
	
	public bool isSelfRunning = true;
	public bool looping = true;
	
	public List<PlaygroundFxCycleItem> particleFx;
	public PlaygroundParticlesC nextParticleFx;
	public PlaygroundParticlesC nextParticleSmokeFx;
	public PlaygroundParticlesC circleLights;
	
	public PlaygroundSpline camSpline;
	public float splineOffsetSpeed = .5f;
	public Transform camPivot;
	public Transform camRotation;
	public float camMovementSpeed = 1f;
	public float camRotationSpeed = 20f;
	
	public Color skyboxStartColor;
	public Color skyboxNextFxColor;
	public float skyboxNextFxColorTime = .1f;
	public float skyboxColorChangeTime = 1f;
	public GameObject pressSpaceText;
	public AudioSource particleBlastSound;
	AudioClip[] particleBlastClips;
	Color skyboxColorOnLoad;
	int currentIndex = -1;
	
	float camSplineTimeOffset = 0;
	float initialSplineSpeed = 0;
	float transitionTime = 1f;
	float cameraPushBack = 10f;
	float cameraPushBackForce = 15f;
	float cameraPushBackRestoreSpeed = 1f;
	
	Camera cam;
	Transform camTransform;
	ManipulatorObjectC repellentManipulator;
	float camFov;
	
	bool isSimulating = false;
	bool inTransition = false;
	bool canLoadNext = true;
	
	void Start () {
		
		// Mouse cursor not wanted
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		Cursor.visible = false;
#else
		Cursor.visible = false;
#endif

		
		// Make sure particle systems are inactive
		foreach (PlaygroundFxCycleItem p in particleFx) {
			p.particles.emit = false;
			p.particles.particleSystemGameObject.SetActive (false);
		}
		
		// Set initial skybox values
		skyboxColorOnLoad = RenderSettings.skybox.GetColor ("_Tint");
		RenderSettings.skybox.SetColor ("_Tint", Color.black);
		
		// Cache up!
		cam = Camera.main;
		camTransform = cam.transform;
		camFov = cam.fieldOfView;
		repellentManipulator = PlaygroundC.GetManipulator(0);
		repellentManipulator.enabled = false;
		particleBlastClips = particleBlastSound.GetComponent<SoundFxArray>().sounds;
		
		// Set initial camera values
		camPivot.position = camSpline.GetPoint(0f);
		camRotation.rotation = Quaternion.LookRotation(-camRotation.position);
		
		// Run the intro
		StartCoroutine(FadeIn());
		
		// Get started right away if no user interaction is required
		if (isSelfRunning) {
			pressSpaceText.SetActive(false);
			StartCoroutine(Selfrunning());
		}
	}
	
	/// <summary>
	/// Runs the intro.
	/// </summary>
	IEnumerator FadeIn () {
		circleLights.splines[0].splineTransform.position = new Vector3(0,30f,0);
		circleLights.splines[1].splineTransform.position = new Vector3(0,-30f,0);
		StartCoroutine(FadeInStartSkybox());
		if (isSelfRunning)
			yield return new WaitForSeconds(6f);
		float animTime = 5f;
		float startTime = Time.time;
		
		while (Time.time<startTime+animTime && currentIndex<0) {
			circleLights.splines[0].splineTransform.position = new Vector3(0,Mathf.Lerp (30f, 16f, Easing(2, (Time.time-startTime)/animTime)),0);
			circleLights.splines[1].splineTransform.position = new Vector3(0,Mathf.Lerp (-30f, -16f, Easing(2, (Time.time-startTime)/animTime)),0);
			yield return null;
		}
		
		if (!isSelfRunning && currentIndex<0)
			pressSpaceText.SetActive(true);
	}
	
	/// <summary>
	/// Fades in the start skybox. This is called from FadeIn().
	/// </summary>
	IEnumerator FadeInStartSkybox () {
		Color currentColor = RenderSettings.skybox.GetColor ("_Tint");
		float startTime = Time.time;
		while (Time.time<startTime+11f && currentIndex<0) {
			RenderSettings.skybox.SetColor ("_Tint", Color.Lerp (currentColor, skyboxStartColor, (Time.time-startTime)/11f));
			yield return null;
		}
	}
	
	/// <summary>
	/// Runs the outro. This only gets called if looping is set to false.
	/// </summary>
	IEnumerator FadeOut () {
		Color currentColor = RenderSettings.skybox.GetColor ("_Tint");
		Color endColor = Color.black;
		float animTime = 3f;
		float startTime = Time.time;
		while (Time.time<startTime+animTime) {
			circleLights.splines[0].splineTransform.position = new Vector3(0,Mathf.Lerp (16f, 30, Easing(1, (Time.time-startTime)/animTime)),0);
			circleLights.splines[1].splineTransform.position = new Vector3(0,Mathf.Lerp (-16f, -30f, Easing(1, (Time.time-startTime)/animTime)),0);
			RenderSettings.skybox.SetColor ("_Tint", Color.Lerp (currentColor, endColor, (Time.time-startTime)/animTime));
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		#if UNITY_STANDALONE
		Application.Quit();
		#elif UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		Application.LoadLevel (Application.loadedLevel);
		#else
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		#endif
	}
	
	/// <summary>
	/// Reset.
	/// </summary>
	void OnDisable () {
		RenderSettings.skybox.SetColor ("_Tint", skyboxColorOnLoad);
	}
	
	void Update () {
		
		// Move camera along spline
		CameraMovement();
		
		if (!isSelfRunning) {
			
			// Call for next effect
			if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space))
				StartCoroutine(NextEffect(1));
			
			// Call for previous effect
			if (Input.GetKeyDown(KeyCode.LeftArrow))
				StartCoroutine(NextEffect(-1));
		}
		
		// Time simulation
		if (Input.GetKey(KeyCode.UpArrow))
			TimeScale(1);
		if (Input.GetKey(KeyCode.DownArrow))
			TimeScale(-1);
		if (Input.GetKey(KeyCode.T))
			TimeScale(0);
		
		// Reset scene
		if (Input.GetKeyDown(KeyCode.R))
		{
			#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel (Application.loadedLevel);
			#else
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			#endif
		}
	}
	
	/// <summary>
	/// Move camera along the camera trail and always look at center. Whenever a transition occur push the camera back a bit.
	/// </summary>
	void CameraMovement () {
		
		if (isSimulating) {
			
			// Camera spline rail movement
			if (initialSplineSpeed<1f)
				initialSplineSpeed += .1f*Time.deltaTime;
			else initialSplineSpeed = 1f;
			camSplineTimeOffset += (splineOffsetSpeed*initialSplineSpeed)*Time.deltaTime;
			camSplineTimeOffset = camSplineTimeOffset%1f;
			
			// Camera movement along current spline
			camPivot.position = Vector3.Lerp (camPivot.position, camSpline.GetPoint(camSplineTimeOffset), camMovementSpeed*Time.deltaTime);
			
			// Camera rotation to always look towards particle system
			camRotation.rotation = Quaternion.Slerp(camRotation.rotation, Quaternion.LookRotation(-camRotation.position), camRotationSpeed*Time.deltaTime);
			
			// Camera push back (when a transition occurs camera will be pushed back and always want to reach local position 0)
			if (!inTransition) 
				cameraPushBack = Mathf.Lerp(cameraPushBack, 0, cameraPushBackRestoreSpeed*Time.deltaTime);
		}
		
		// Apply pushback and fov changes to camera
		camTransform.localPosition = new Vector3(0,0, -cameraPushBack);
		cam.fieldOfView = camFov+cameraPushBack;
		
	}
	
	/// <summary>
	/// Invoke repeat friendly call for next effect.
	/// </summary>
	void NextEffect () {
		StartCoroutine (NextEffect(1));
	}
	
	/// <summary>
	/// Load next effect while transitioning.
	/// </summary>
	IEnumerator NextEffect (int increment) {
		if (!canLoadNext) yield break;
		isSimulating = true;
		inTransition = true;
		canLoadNext = false;
		
		pressSpaceText.SetActive(false);
		
		// Internally used variables
		bool canContinue = looping||!looping&&currentIndex<particleFx.Count-1;
		float startTime;
		
		// Store current particle system's values before we change them
		int previousIndex = currentIndex;
		bool previousOnlySourcePositioning = previousIndex>=0?particleFx[previousIndex].particles.onlySourcePositioning:false;
		bool previousOnlyLifetimePositioning = previousIndex>=0?particleFx[previousIndex].particles.onlyLifetimePositioning:false;
		bool previousTransitionBackToSource = previousIndex>=0?particleFx[previousIndex].particles.transitionBackToSource:false;
		
		// Make Lifetime Positioning unable to set new positions (this makes particles behave like a target manipulator has affected them)
		if (previousOnlyLifetimePositioning) {
			for (int p = 0; p<particleFx[previousIndex].particles.particleCount; p++)
				particleFx[previousIndex].particles.playgroundCache.changedByPropertyTarget[p] = true;
		}
		
		// Prepare current (previous) particle system for transition
		if (previousIndex>=0) {
			particleFx[previousIndex].particles.emit = false;
			particleFx[previousIndex].particles.onlySourcePositioning = false;
			particleFx[previousIndex].particles.onlyLifetimePositioning = false;
			particleFx[previousIndex].particles.transitionBackToSource = false;
			particleFx[previousIndex].particles.particleMaskTime = transitionTime*.75f;
			particleFx[previousIndex].particles.particleMask = particleFx[previousIndex].particles.particleCount;
			particleFx[previousIndex].particles.applyParticleMask = true;
		}
		
		if (canContinue) {
			if (!isSelfRunning) {
				particleBlastSound.clip = particleBlastClips[UnityEngine.Random.Range (0, particleBlastClips.Length)];
				particleBlastSound.pitch = UnityEngine.Random.Range (.9f, 1.1f);
				particleBlastSound.Play();
			}
			currentIndex = (currentIndex+increment)%particleFx.Count;
			if (currentIndex<0)
				currentIndex = particleFx.Count-1;
			
			// Scene transition fx
			StartCoroutine(NextSkyboxColor());
			nextParticleFx.Emit (true);
			nextParticleSmokeFx.Emit (true);
			repellentManipulator.enabled = true;
			
			// Camera push back 
			yield return new WaitForSeconds(.05f);
			startTime = Time.time;
			while (Time.time<startTime+(transitionTime/2f)) {
				cameraPushBack = Mathf.Lerp (cameraPushBack, cameraPushBackForce, (Time.time-startTime)/(transitionTime/2f));
				yield return null;
			}
			
			// Prepare new (next) particle system for transition
			particleFx[currentIndex].particles.particleMaskTime = 0;
			particleFx[currentIndex].particles.particleMask = particleFx[currentIndex].particles.particleCount;
			particleFx[currentIndex].particles.applyParticleMask = true;
			yield return null;
			repellentManipulator.enabled = false;
			if (particleFx[currentIndex].moveInOut)
				particleFx[currentIndex].particles.particleSystemTransform.position = new Vector3(0,-20f,0);
			particleFx[currentIndex].particles.Emit(true);
			while (!particleFx[currentIndex].particles.IsReady()) {
				yield return null;
			}
			for (int p = 0; p<particleFx[currentIndex].particles.particleCount; p++) {
				particleFx[currentIndex].particles.playgroundCache.maskAlpha[p] = 0f;
				particleFx[currentIndex].particles.playgroundCache.isMasked[p] = true;
			}
			yield return null;
			particleFx[currentIndex].particles.particleMaskTime = transitionTime/2f;
			particleFx[currentIndex].particles.particleMask = 0;
			
			nextParticleFx.Emit (false);
			nextParticleSmokeFx.Emit (false);
			
			inTransition = false;
			
			// Move particle system in if needed
			if (particleFx[currentIndex].moveInOut) {
				startTime = Time.time;
				while (Time.time<startTime+(transitionTime/2f)) {
					particleFx[currentIndex].particles.particleSystemTransform.position = 
						new Vector3(
							0, 
							Mathf.Lerp (-20f, 0, Easing(2, (Time.time-startTime)/(transitionTime/2f))),
							0
							);
					yield return null;
				}
				particleFx[currentIndex].particles.particleSystemTransform.position = Vector3.zero;
			}
		}
		
		// Wait around till all fx is ready, move previous particle system if needed
		startTime = Time.time;
		while (Time.time<startTime+transitionTime+.5f) {
			if (previousIndex>=0 && particleFx[previousIndex].moveInOut) {
				particleFx[previousIndex].particles.particleSystemTransform.position = 
					new Vector3(
						0, 
						Mathf.Lerp (0, 20f, Easing(1, (Time.time-startTime)/(transitionTime/2f))),
						0
						);
			}
			yield return null;
		}
		
		// Restore previous particle system (for when it returns in the fx cycle)
		if (previousIndex>=0 && canContinue) {
			yield return null;
			particleFx[previousIndex].particles.onlySourcePositioning = previousOnlySourcePositioning;
			particleFx[previousIndex].particles.onlyLifetimePositioning = previousOnlyLifetimePositioning;
			particleFx[previousIndex].particles.transitionBackToSource = previousTransitionBackToSource;
			particleFx[previousIndex].particles.particleSystemGameObject.SetActive (false);
		}
		
		if (canContinue)
			canLoadNext = true;
		else StartCoroutine(FadeOut());
	}
	
	/// <summary>
	/// Sets next color to the skybox. Each particle system has its own color profile.
	/// </summary>
	IEnumerator NextSkyboxColor () {
		
		Color currentColor = RenderSettings.skybox.GetColor ("_Tint");
		float startTime = Time.time;
		while (Time.time<startTime+skyboxNextFxColorTime) {
			RenderSettings.skybox.SetColor ("_Tint", Color.Lerp (currentColor, skyboxNextFxColor, (Time.time-startTime)/skyboxNextFxColorTime));
			yield return null;
		}
		startTime = Time.time;
		currentColor = RenderSettings.skybox.GetColor ("_Tint");
		while (Time.time<startTime+skyboxColorChangeTime) {
			RenderSettings.skybox.SetColor ("_Tint", Color.Lerp (currentColor, particleFx[currentIndex].colorProfile, (Time.time-startTime)/skyboxColorChangeTime));
			yield return null;
		}
	}
	
	/// <summary>
	/// Changes time scale without Time.timeScale dependency (as long as it's not 0).
	/// </summary>
	/// <param name="level">Determines if the change is positive or negative.</param>
	void TimeScale (int level) {
		if (level==0) {
			Time.timeScale = 1f;
			return;
		}
		Time.timeScale = Mathf.Clamp (Time.timeScale+((Time.deltaTime/Time.timeScale)*(level*1f)), .1f, 1f);
	}
	
	/// <summary>
	/// Eases a float value.
	/// Transition types: 0 = none, 1 = ease in, 2 = ease out
	/// </summary>
	/// <param name="transitionType">Transition type.</param>
	/// <param name="time">Time.</param>
	float Easing (int transitionType, float time) {
		if (transitionType==0)
			return time;
		else if (transitionType==1)
			return Mathf.Lerp (0f, 1f, 1f-Mathf.Cos(time*Mathf.PI*.5f));
		else if (transitionType==2)
			return Mathf.Lerp (0f, 1f, Mathf.Sin(time*Mathf.PI*.5f));
		return time;
	}
	
	/// <summary>
	/// Runs all effects with a time interval, timed to this song: https://soundcloud.com/polyfied/patterns-evolved
	/// </summary>
	IEnumerator Selfrunning () {
		yield return new WaitForSeconds(15.5f);
		InvokeRepeating ("NextEffect", 0, 8f);
	}
}

[Serializable]
public class PlaygroundFxCycleItem {
	public PlaygroundParticlesC particles;
	public Color colorProfile;
	public bool moveInOut = false;
	public bool unfolded = false;
}
