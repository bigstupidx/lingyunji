using UnityEngine;
using System.Collections;
using ParticlePlayground;

namespace UnityEngine {
	public class PlaygroundRecordingExample : MonoBehaviour {

		public PlaygroundRecorder playgroundRecorder;
		public float keyframeInterval = .1f;
		public bool recordFromStart = true;

		Rect guiArea;
		bool hoveringScrubSlider = false;
		bool isScrubbing = false;
		float playbackSliderValue = 0;

		IEnumerator Start () 
		{
			guiArea = new Rect(10f, 10f, Screen.width*.5f, 130f);
			if (recordFromStart)
			{
				while (!playgroundRecorder.playgroundSystem.IsReady())
					yield return null;
				playgroundRecorder.StartRecording (keyframeInterval);
			}
		}

		void Update ()
		{
			// Scrubbing particles to playback slider time
			if (hoveringScrubSlider && Input.GetMouseButton(0))
				isScrubbing = true;
			else if (Input.GetMouseButtonUp(0))
				isScrubbing = false;
			if (isScrubbing)
				playgroundRecorder.Scrub (playbackSliderValue);
		}

		void OnGUI () 
		{
			GUILayout.BeginArea(guiArea, "Playground Recorder");

			GUILayout.Space (18f);

			GUILayout.BeginHorizontal();

			// Record button
			if (GUILayout.Button (!playgroundRecorder.IsRecording()? "Record" : "Stop Recording"))
			{
				if (!playgroundRecorder.IsRecording())
					playgroundRecorder.StartRecording (keyframeInterval);
				else
					playgroundRecorder.StopRecording();
			}

			GUILayout.EndHorizontal();

			// Time scrub slider
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Time (" + (playgroundRecorder.FrameCount() * keyframeInterval * playbackSliderValue).ToString ("F1") + ")", new GUILayoutOption[]{GUILayout.MaxWidth(40f)});
			playbackSliderValue = GUILayout.HorizontalSlider(playbackSliderValue, 0f, 1f);
			if (playgroundRecorder.IsRecording())
				playbackSliderValue = 1f;
			hoveringScrubSlider = (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition));
			GUILayout.EndHorizontal();

			GUILayout.EndArea();
		}
	}
}
