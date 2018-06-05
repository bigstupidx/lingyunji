using UnityEngine;
using System.Collections;
using ParticlePlayground;

namespace UnityEngine {
	public class PlaygroundSwitchMultithreadingMethod : MonoBehaviour {

		/// <summary>
		/// Reference to the global manipulator.
		/// </summary>
		ManipulatorObjectC globalManipulator;

		void Start () {
			globalManipulator = PlaygroundC.GetManipulator(0);
		}

		void Update () {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000f)) {
				globalManipulator.transform.transform.position = hit.point;
			}
		}

		/// <summary>
		/// SwitchMultithreading is its own class with static functions called for jumping through available thread methods.
		/// </summary>
		void OnGUI () {

			// Manipulator settings
			if (PlaygroundC.reference.manipulators.Count>0) {
				GUI.Label (new Rect(Screen.width-620, 10, 300, 25), "Global Manipulator:");
				globalManipulator.enabled = GUI.Toggle (new Rect(Screen.width-620, 35, 300, 20), globalManipulator.enabled, "Enabled");
				if (GUI.Button (new Rect(Screen.width-620, 55, 300, 20), "Type: "+globalManipulator.type.ToString()))
					SwitchManipulator();
				GUI.Label (new Rect(Screen.width-620, 80, 120, 30), "Strength");
				globalManipulator.strength = GUI.HorizontalSlider(new Rect(Screen.width-560, 85, 210, 30), globalManipulator.strength, 0f, 10f);
				GUI.Label (new Rect(Screen.width-340, 80, 60, 30), globalManipulator.strength.ToString("f1"));
			}

			// Multithreading methods
			GUI.Label (new Rect(Screen.width-310, 10, 300, 25), "Multithreading:");
			if (GUI.Button (new Rect(Screen.width-310, 35, 300, 20), "Thread Pool Method: "+PlaygroundC.reference.threadPoolMethod.ToString()))
				SwitchMultithreading.NextThreadPoolMethod();
			if (GUI.Button (new Rect(Screen.width-310, 55, 300, 20), "Particle System Method: "+PlaygroundC.reference.threadMethod.ToString()))
				SwitchMultithreading.NextParticleSystemMethod();
			if (GUI.Button (new Rect(Screen.width-310, 75, 300, 20), "Skinned Mesh Method: "+PlaygroundC.reference.skinnedMeshThreadMethod.ToString()))
				SwitchMultithreading.NextSkinnedMeshMethod();
			if (GUI.Button (new Rect(Screen.width-310, 95, 300, 20), "Turbulence Method: "+PlaygroundC.reference.turbulenceThreadMethod.ToString()))
				SwitchMultithreading.NextTurbulenceMethod();
		}

		void SwitchManipulator () {
			switch (globalManipulator.type) {
			case MANIPULATORTYPEC.Attractor: globalManipulator.type = MANIPULATORTYPEC.AttractorGravitational; break;
			case MANIPULATORTYPEC.AttractorGravitational: globalManipulator.type = MANIPULATORTYPEC.Repellent; break;
			case MANIPULATORTYPEC.Repellent: globalManipulator.type = MANIPULATORTYPEC.Vortex; break;
			case MANIPULATORTYPEC.Vortex: globalManipulator.type = MANIPULATORTYPEC.Attractor; break;
			}
		}
	}
}
