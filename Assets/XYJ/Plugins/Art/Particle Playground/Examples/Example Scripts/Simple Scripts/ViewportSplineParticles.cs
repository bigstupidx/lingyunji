using UnityEngine;
using System.Collections;
using ParticlePlayground;
using PlaygroundSplines;
#pragma warning disable

public class ViewportSplineParticles : MonoBehaviour {

	public PlaygroundParticlesC particles;				// Assign your particle system in Inspector
	public float screenOffset = 0;						// Determines the offset from viewport (screen border)
	
	PlaygroundSpline spline;
	Camera mainCamera;
	int currentScreenX;
	int currentScreenY;
	float prevScreenOffset;

	void Start () {

		// Cache camera
		mainCamera = Camera.main;

		// Parent the particle system to the main camera
		particles.particleSystemTransform.parent = mainCamera.transform;
		particles.shurikenParticleSystem.simulationSpace = ParticleSystemSimulationSpace.Local;

		// Create a new spline and set it up
		spline = new GameObject("Viewport Spline", typeof(PlaygroundSpline)).GetComponent<PlaygroundSpline>();
		spline.Loop = true;
		spline.Reset ();
		spline.SetControlPointMode(0, BezierControlPointMode.Free);
		spline.SetControlPointMode(1, BezierControlPointMode.Free);

		// Add three additional nodes (a basic spline will contain two initial nodes)
		spline.AddNode();
		spline.AddNode();
		spline.AddNode();

		// Set the nodes to match the viewport
		currentScreenX = Screen.width;
		currentScreenY = Screen.height;
		SetViewportNodes();

		// Assign the spline to the particle system
		particles.splines.Add (spline);

		// Make sure we're using the spline as source
		particles.source = SOURCEC.Spline;

		prevScreenOffset = screenOffset;
	}

	void Update ()
	{
		// In case screen width/height would change in middle of simulation
		if (currentScreenX != Screen.width || currentScreenY != Screen.height || prevScreenOffset!=screenOffset)
		{
			currentScreenX = Screen.width;
			currentScreenY = Screen.height;
			prevScreenOffset = screenOffset;

			SetViewportNodes();
		}
	}

	void SetViewportNodes ()
	{
		// Create viewport nodes (5 main nodes + 8 bezier nodes)
		Vector3[] points = new Vector3[13];
		
		// These are the main nodes:
		points[0] = mainCamera.ViewportToWorldPoint(new Vector3(0+screenOffset,	0+screenOffset, 10f));
		points[3] = mainCamera.ViewportToWorldPoint(new Vector3(0+screenOffset,	1f-screenOffset, 10f));
		points[6] = mainCamera.ViewportToWorldPoint(new Vector3(1f-screenOffset, 1f-screenOffset, 10f));
		points[9] = mainCamera.ViewportToWorldPoint(new Vector3(1f-screenOffset, 0+screenOffset, 10f));
		points[12] = mainCamera.ViewportToWorldPoint(new Vector3(0+screenOffset,	0+screenOffset, 10f));
		
		// These are the bezier handles:
		points[1] = mainCamera.ViewportToWorldPoint(new Vector3(0+screenOffset,	.5f, 10f));
		points[2] = mainCamera.ViewportToWorldPoint(new Vector3(0+screenOffset,	.5f, 10f));
		
		points[4] = mainCamera.ViewportToWorldPoint(new Vector3(.5f, 1f-screenOffset, 10f));
		points[5] = mainCamera.ViewportToWorldPoint(new Vector3(.5f, 1f-screenOffset, 10f));
		
		points[7] = mainCamera.ViewportToWorldPoint(new Vector3(1f-screenOffset, .5f, 10f));
		points[8] = mainCamera.ViewportToWorldPoint(new Vector3(1f-screenOffset, .5f, 10f));
		
		points[10] = mainCamera.ViewportToWorldPoint(new Vector3(.5f, 0+screenOffset, 10f));
		points[11] = mainCamera.ViewportToWorldPoint(new Vector3(.5f, 0+screenOffset, 10f));
		
		// Add the nodes into the spline
		spline.SetPoints(points);
	}
}
