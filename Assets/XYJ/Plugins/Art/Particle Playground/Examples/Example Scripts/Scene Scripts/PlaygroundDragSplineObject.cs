using UnityEngine;
using System.Collections;
using ParticlePlayground;
using PlaygroundSplines;

public class PlaygroundDragSplineObject : MonoBehaviour {

	static Camera mainCam;
	static Transform mainCamTransform;
	static bool isDragging;
	static ManipulatorObjectC manipulator;
	static PlaygroundTurbulenceOnGameObjects turbulenceScript;

	public PlaygroundSpline mySpline;
	public Material enterMaterial;
	Material exitMaterial;
	Transform t;
	Renderer r;
	float camZ;

	void Start () {
		if (mainCam==null) {
			mainCam = Camera.main;
			mainCamTransform = mainCam.transform;
			PlaygroundParticlesC[] ps = GameObject.FindObjectsOfType<PlaygroundParticlesC>();
			foreach (PlaygroundParticlesC p in ps) {
				if (p.manipulators.Count>0) {
					manipulator = p.manipulators[0];
					break;
				}
			}
			turbulenceScript = GameObject.FindObjectOfType<PlaygroundTurbulenceOnGameObjects>();
		}
		r = GetComponent<Renderer>();
		t = transform;
		exitMaterial = r.sharedMaterial;
		camZ = -mainCamTransform.position.z+t.position.z;
	}
	
	void OnMouseOver () {
		if (!isDragging)
			r.sharedMaterial = enterMaterial;
	}

	void OnMouseExit () {
		if (!isDragging)
			r.sharedMaterial = exitMaterial;
	}

	void OnMouseDrag () {
		if (isDragging==false)
			turbulenceScript.StopTurbulenceOnTransform(t);
		isDragging = true;
		r.sharedMaterial = enterMaterial;
		t.position = Vector3.Lerp (t.position, mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camZ)), Time.deltaTime*10f);
		manipulator.strength = Mathf.Lerp (manipulator.strength, 3f, Time.deltaTime);
		manipulator.size = manipulator.strength*4f;
		float s = t.localScale.x;
		s = Mathf.Lerp (s, 1f+(Mathf.Sin(Time.time*20f)*.5f), Time.deltaTime*3f);
		t.localScale = new Vector3(s,s,s);
	}

	void OnMouseDown () {
		SetManipulatorTransform(t);
	}

	void OnMouseUp () {
		isDragging = false;
		r.sharedMaterial = exitMaterial;
		t.localScale = new Vector3(1f,1f,1f);
		SetManipulatorTransform(null);
		manipulator.strength = 0;
		manipulator.size = 0;
		turbulenceScript.StartTurbulenceOnTransform(t);
	}

	void SetManipulatorTransform (Transform mTransform) {
		manipulator.transform.transform = mTransform;
		manipulator.properties[0].splineTarget = mySpline;
	}
}
