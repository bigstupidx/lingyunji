using UnityEngine;
using System.Collections;
using ParticlePlayground;

namespace UnityEngine {
	public class PlaygroundRecordingMultiExample : MonoBehaviour {

		public PlaygroundMultiRecorder playgroundMultiRecorder;
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
				for (int i = 0; i < playgroundMultiRecorder.playgroundRecorders.Length; i++)
				{
					while (!playgroundMultiRecorder.playgroundRecorders[i].playgroundSystem.IsReady())
						yield return null;
				}
				playgroundMultiRecorder.StartRecording (keyframeInterval);
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
				playgroundMultiRecorder.Scrub (playbackSliderValue);
		}

		void OnGUI () 
		{
			GUILayout.BeginArea(guiArea, "Playground Multi Recorder");

			GUILayout.Space (18f);

			GUILayout.BeginHorizontal();

			// Record button
			if (GUILayout.Button (!playgroundMultiRecorder.playgroundRecorders[0].IsRecording()? "Record" : "Stop Recording"))
			{
				if (!playgroundMultiRecorder.playgroundRecorders[0].IsRecording())
					playgroundMultiRecorder.StartRecording (keyframeInterval);
				else
					playgroundMultiRecorder.StopRecording();
			}

			GUILayout.EndHorizontal();

			// Time scrub slider
			GUILayout.BeginHorizontal();
			GUILayout.Label ("Time (" + (playgroundMultiRecorder.playgroundRecorders[0].FrameCount() * keyframeInterval * playbackSliderValue).ToString ("F1") + ")", new GUILayoutOption[]{GUILayout.MaxWidth(40f)});
			playbackSliderValue = GUILayout.HorizontalSlider(playbackSliderValue, 0f, 1f);
			if (playgroundMultiRecorder.playgroundRecorders[0].IsRecording())
				playbackSliderValue = 1f;
			hoveringScrubSlider = (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition));
			GUILayout.EndHorizontal();

			GUILayout.EndArea();
		}
	}
}
